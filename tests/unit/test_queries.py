"""Tests for TicketQueries (Issue 4.8)."""
from decimal import Decimal
from datetime import datetime, timedelta, time
import uuid
import pytest

from backend.database.models import (
    Agent, TicketType, TicketStatus, Route, StationType,
    CreatedBy, HealthStatus, AgentStatus, Gender, Vehicle,
    VehicleStatus, FuelType, Schedule
)
from backend.database.queries import DatabaseQueries


@pytest.fixture
def db(db_session):
    return DatabaseQueries(db_session)

@pytest.fixture
def sample_agent(db_session):
    agent = Agent(
        name="Agente Teste",
        birth_date=datetime(1990,1,1),
        gender=Gender.CIS_MALE,
        health_status=HealthStatus.HEALTHY,
        current_status=AgentStatus.IDLE,
        created_by=CreatedBy.IA,
        version="1.0",
        wallet=Decimal('100.00')
    )
    db_session.add(agent)
    db_session.commit()
    return agent

@pytest.fixture
def sample_route(db_session):
    # Rota mínima para teste de fare_base
    route = Route(
        name="Linha Trem",
        route_type=StationType.BUS_DIESEL,  # Use a valid value from the first StationType enum
        fare_base=Decimal('3.50'),
        is_active=True
    )
    db_session.add(route)
    db_session.commit()
    return route

class TestTicketQueries:
    def test_create_ticket_basic(self, db, sample_agent):
        ticket = db.tickets.create_ticket(agent_id=sample_agent.id)
        assert ticket.id is not None
        assert ticket.agent_id == sample_agent.id
        assert ticket.status == TicketStatus.ACTIVE
        assert ticket.ticket_type == TicketType.SINGLE
        assert ticket.max_validations == 1
        assert ticket.valid_until is not None

    def test_create_ticket_with_route_price_inference(self, db, sample_agent, sample_route):
        ticket = db.tickets.create_ticket(agent_id=sample_agent.id, route_id=sample_route.id)
        assert Decimal(str(ticket.price)) == Decimal('3.50')

    def test_validate_ticket_flow(self, db, sample_agent):
        ticket = db.tickets.create_ticket(agent_id=sample_agent.id, ticket_type=TicketType.SINGLE)
        assert ticket.validate() is True
        assert ticket.status == TicketStatus.USED
        assert ticket.validation_count == 1
        assert ticket.validate() is False  # segunda vez falha

    def test_get_active_tickets_filters(self, db, sample_agent):
        # Criar 2 ativos e 1 usado
        t1 = db.tickets.create_ticket(agent_id=sample_agent.id)
        t2 = db.tickets.create_ticket(agent_id=sample_agent.id)
        t3 = db.tickets.create_ticket(agent_id=sample_agent.id)
        t3.validate()  # marca usado
        active = db.tickets.get_active_tickets(agent_id=sample_agent.id)
        assert len(active) == 2
        assert all(t.status == TicketStatus.ACTIVE for t in active)

    def test_get_by_agent_returns_all(self, db, sample_agent):
        for _ in range(5):
            db.tickets.create_ticket(agent_id=sample_agent.id)
        all_tickets = db.tickets.get_by_agent(sample_agent.id)
        assert len(all_tickets) == 5

    def test_get_revenue_by_period_excludes_cancelled(self, db, sample_agent):
        now = datetime.utcnow()
        t1 = db.tickets.create_ticket(agent_id=sample_agent.id)
        t2 = db.tickets.create_ticket(agent_id=sample_agent.id)
        t3 = db.tickets.create_ticket(agent_id=sample_agent.id)
        # cancelar t2
        t2.cancel()
        db.session.flush()
        start = now - timedelta(hours=1)
        end = now + timedelta(hours=1)
        revenue = db.tickets.get_revenue_by_period(start, end)
        # Cada ticket default price 0.00, revenue deve ser 0
        assert revenue == 0.0
        # Ajustar preço manual para validar cálculo
        t1.price = Decimal('5.00')
        t3.price = Decimal('2.50')
        db.session.flush()
        revenue2 = db.tickets.get_revenue_by_period(start, end)
        assert revenue2 == 7.50  # 5.00 + 2.50 (cancelado excluído)

    def test_get_usage_statistics_day(self, db, sample_agent):
        # Criar tickets variados
        active = db.tickets.create_ticket(agent_id=sample_agent.id)
        used = db.tickets.create_ticket(agent_id=sample_agent.id)
        used.validate()
        expired = db.tickets.create_ticket(agent_id=sample_agent.id)
        expired.valid_until = datetime.utcnow() - timedelta(hours=1)
        expired.is_valid()  # força status EXPIRED
        cancelled = db.tickets.create_ticket(agent_id=sample_agent.id)
        cancelled.cancel()
        stats = db.tickets.get_usage_statistics('day')
        assert stats['total'] == 4
        assert stats['active'] == 1
        assert stats['used'] == 1
        assert stats['expired'] == 1
        assert stats['cancelled'] == 1
        assert stats['avg_validations'] >= 0.0

    def test_get_usage_statistics_all(self, db, sample_agent):
        for _ in range(3):
            db.tickets.create_ticket(agent_id=sample_agent.id)
        stats = db.tickets.get_usage_statistics('all')
        assert stats['total'] == 3

    def test_validate_ticket_invalid(self, db):
        fake_id = uuid.uuid4()
        assert db.tickets.validate_ticket(fake_id) is False

    def test_create_ticket_different_types(self, db, sample_agent):
        mapping = {
            TicketType.SINGLE: 1,
            TicketType.RETURN: 2,
            TicketType.DAY_PASS: 999,
            TicketType.WEEK_PASS: 999,
            TicketType.MONTH_PASS: 999,
            TicketType.TRANSFER: 1,
        }
        for tt, max_valid in mapping.items():
            t = db.tickets.create_ticket(agent_id=sample_agent.id, ticket_type=tt)
            assert t.max_validations == max_valid
            assert t.ticket_type == tt

    def test_usage_statistics_empty(self, db):
        stats = db.tickets.get_usage_statistics('all')
        assert stats['total'] == 0
        assert stats['avg_validations'] == 0.0
        assert stats['revenue_total'] == 0.0
        assert stats['usage_rate'] == 0.0

    def test_get_active_tickets_no_agent_filter(self, db, sample_agent):
        db.tickets.create_ticket(agent_id=sample_agent.id)
        active = db.tickets.get_active_tickets()
        assert len(active) == 1

    def test_revenue_period_outside(self, db, sample_agent):
        t = db.tickets.create_ticket(agent_id=sample_agent.id)
        t.price = Decimal('10.00')
        db.session.flush()
        start = datetime.utcnow() + timedelta(hours=1)
        end = start + timedelta(hours=2)
        assert db.tickets.get_revenue_by_period(start, end) == 0.0

    def test_usage_statistics_invalid_period(self, db):
        with pytest.raises(ValueError):
            db.tickets.get_usage_statistics('year')


# ==================== ISSUE 4.9: SCHEDULE TESTS ====================

@pytest.fixture
def sample_vehicle(db_session, sample_route):
    """Create a sample vehicle for schedule testing."""
    vehicle = Vehicle(
        name="Trem #001",
        vehicle_type="train",
        status=VehicleStatus.ACTIVE,
        fuel_type=FuelType.DIESEL,
        passenger_capacity=200,
        route_id=sample_route.id
    )
    db_session.add(vehicle)
    db_session.commit()
    return vehicle


class TestScheduleQueries:
    """Test suite for Schedule queries (Issue 4.9)."""

    # ==================== CRUD TESTS ====================

    def test_create_schedule_basic(self, db, sample_route, sample_vehicle):
        """Test basic schedule creation."""
        schedule = db.schedules.create(
            route_id=sample_route.id,
            vehicle_id=sample_vehicle.id,
            departure_time=time(8, 0),
            days_of_week=[0, 1, 2, 3, 4]  # Mon-Fri
        )
        assert schedule.id is not None
        assert schedule.route_id == sample_route.id
        assert schedule.vehicle_id == sample_vehicle.id
        assert schedule.departure_time == time(8, 0)
        assert schedule.days_of_week == [0, 1, 2, 3, 4]
        assert schedule.is_active is True

    def test_create_schedule_weekend_only(self, db, sample_route, sample_vehicle):
        """Test creating a weekend-only schedule."""
        schedule = db.schedules.create(
            route_id=sample_route.id,
            vehicle_id=sample_vehicle.id,
            departure_time=time(10, 0),
            days_of_week=[5, 6]  # Sat-Sun
        )
        assert schedule.days_of_week == [5, 6]
        assert schedule.is_active is True

    def test_create_schedule_inactive(self, db, sample_route, sample_vehicle):
        """Test creating an inactive schedule."""
        schedule = db.schedules.create(
            route_id=sample_route.id,
            vehicle_id=sample_vehicle.id,
            departure_time=time(14, 30),
            days_of_week=[0, 1, 2, 3, 4, 5, 6],  # Every day
            is_active=False
        )
        assert schedule.is_active is False

    def test_get_by_id(self, db, sample_route, sample_vehicle):
        """Test retrieving schedule by ID."""
        created = db.schedules.create(
            route_id=sample_route.id,
            vehicle_id=sample_vehicle.id,
            departure_time=time(9, 0),
            days_of_week=[0, 1, 2, 3, 4]
        )
        db.session.commit()

        retrieved = db.schedules.get_by_id(created.id)
        assert retrieved is not None
        assert retrieved.id == created.id
        assert retrieved.departure_time == time(9, 0)

    def test_get_by_id_not_found(self, db):
        """Test get_by_id with non-existent ID."""
        fake_id = uuid.uuid4()
        result = db.schedules.get_by_id(fake_id)
        assert result is None

    def test_update_schedule(self, db, sample_route, sample_vehicle):
        """Test updating a schedule."""
        schedule = db.schedules.create(
            route_id=sample_route.id,
            vehicle_id=sample_vehicle.id,
            departure_time=time(8, 0),
            days_of_week=[0, 1, 2, 3, 4]
        )
        db.session.commit()

        updated = db.schedules.update(
            schedule.id,
            departure_time=time(9, 30),
            days_of_week=[0, 1, 2],
            is_active=False
        )

        assert updated.departure_time == time(9, 30)
        assert updated.days_of_week == [0, 1, 2]
        assert updated.is_active is False

    def test_update_schedule_not_found(self, db):
        """Test updating a non-existent schedule."""
        fake_id = uuid.uuid4()
        result = db.schedules.update(fake_id, departure_time=time(10, 0))
        assert result is None

    def test_delete_schedule(self, db, sample_route, sample_vehicle):
        """Test deleting a schedule."""
        schedule = db.schedules.create(
            route_id=sample_route.id,
            vehicle_id=sample_vehicle.id,
            departure_time=time(8, 0),
            days_of_week=[0, 1, 2, 3, 4]
        )
        db.session.commit()

        result = db.schedules.delete(schedule.id)
        assert result is True

        # Verify it's deleted
        retrieved = db.schedules.get_by_id(schedule.id)
        assert retrieved is None

    def test_delete_schedule_not_found(self, db):
        """Test deleting a non-existent schedule."""
        fake_id = uuid.uuid4()
        result = db.schedules.delete(fake_id)
        assert result is False

    # ==================== QUERY TESTS ====================

    def test_get_by_route(self, db, sample_route, sample_vehicle):
        """Test getting schedules by route."""
        s1 = db.schedules.create(
            route_id=sample_route.id,
            vehicle_id=sample_vehicle.id,
            departure_time=time(8, 0),
            days_of_week=[0, 1, 2, 3, 4]
        )
        s2 = db.schedules.create(
            route_id=sample_route.id,
            vehicle_id=sample_vehicle.id,
            departure_time=time(14, 0),
            days_of_week=[0, 1, 2, 3, 4]
        )
        db.session.commit()

        schedules = db.schedules.get_by_route(sample_route.id)
        assert len(schedules) == 2
        # Should be ordered by departure_time
        assert schedules[0].departure_time == time(8, 0)
        assert schedules[1].departure_time == time(14, 0)

    def test_get_by_route_only_active(self, db, sample_route, sample_vehicle):
        """Test filtering active schedules by route."""
        active = db.schedules.create(
            route_id=sample_route.id,
            vehicle_id=sample_vehicle.id,
            departure_time=time(8, 0),
            days_of_week=[0, 1, 2, 3, 4],
            is_active=True
        )
        inactive = db.schedules.create(
            route_id=sample_route.id,
            vehicle_id=sample_vehicle.id,
            departure_time=time(10, 0),
            days_of_week=[0, 1, 2, 3, 4],
            is_active=False
        )
        db.session.commit()

        schedules = db.schedules.get_by_route(sample_route.id, only_active=True)
        assert len(schedules) == 1
        assert schedules[0].id == active.id

        all_schedules = db.schedules.get_by_route(sample_route.id, only_active=False)
        assert len(all_schedules) == 2

    def test_get_by_vehicle(self, db, sample_route, sample_vehicle):
        """Test getting schedules by vehicle."""
        s1 = db.schedules.create(
            route_id=sample_route.id,
            vehicle_id=sample_vehicle.id,
            departure_time=time(7, 0),
            days_of_week=[0, 1, 2, 3, 4]
        )
        s2 = db.schedules.create(
            route_id=sample_route.id,
            vehicle_id=sample_vehicle.id,
            departure_time=time(16, 0),
            days_of_week=[5, 6]
        )
        db.session.commit()

        schedules = db.schedules.get_by_vehicle(sample_vehicle.id)
        assert len(schedules) == 2

    def test_get_active_schedules(self, db, sample_route, sample_vehicle):
        """Test getting all active schedules."""
        active1 = db.schedules.create(
            route_id=sample_route.id,
            vehicle_id=sample_vehicle.id,
            departure_time=time(8, 0),
            days_of_week=[0, 1, 2, 3, 4],
            is_active=True
        )
        inactive = db.schedules.create(
            route_id=sample_route.id,
            vehicle_id=sample_vehicle.id,
            departure_time=time(12, 0),
            days_of_week=[0, 1, 2, 3, 4],
            is_active=False
        )
        db.session.commit()

        schedules = db.schedules.get_active_schedules()
        assert len(schedules) == 1
        assert schedules[0].is_active is True

    def test_get_schedules_for_day(self, db, sample_route, sample_vehicle):
        """Test getting schedules for a specific weekday."""
        weekday_schedule = db.schedules.create(
            route_id=sample_route.id,
            vehicle_id=sample_vehicle.id,
            departure_time=time(8, 0),
            days_of_week=[0, 1, 2, 3, 4]  # Mon-Fri
        )
        weekend_schedule = db.schedules.create(
            route_id=sample_route.id,
            vehicle_id=sample_vehicle.id,
            departure_time=time(10, 0),
            days_of_week=[5, 6]  # Sat-Sun
        )
        db.session.commit()

        # Monday (0)
        monday_schedules = db.schedules.get_schedules_for_day(0)
        assert len(monday_schedules) == 1
        assert monday_schedules[0].id == weekday_schedule.id

        # Saturday (5)
        saturday_schedules = db.schedules.get_schedules_for_day(5)
        assert len(saturday_schedules) == 1
        assert saturday_schedules[0].id == weekend_schedule.id

    def test_get_schedules_for_day_with_route_filter(self, db_session, db):
        """Test getting schedules for a day filtered by route."""
        route1 = Route(
            name="Route 1",
            route_type=StationType.BUS_DIESEL,
            fare_base=Decimal('3.50'),
            is_active=True
        )
        route2 = Route(
            name="Route 2",
            route_type=StationType.TRAM_ELECTRIC,
            fare_base=Decimal('2.50'),
            is_active=True
        )
        db_session.add(route1)
        db_session.add(route2)
        db_session.commit()

        vehicle1 = Vehicle(
            name="Vehicle 1",
            vehicle_type="train",
            status=VehicleStatus.ACTIVE,
            fuel_type=FuelType.DIESEL
        )
        db_session.add(vehicle1)
        db_session.commit()

        s1 = db.schedules.create(
            route_id=route1.id,
            vehicle_id=vehicle1.id,
            departure_time=time(8, 0),
            days_of_week=[0, 1, 2]
        )
        s2 = db.schedules.create(
            route_id=route2.id,
            vehicle_id=vehicle1.id,
            departure_time=time(9, 0),
            days_of_week=[0, 1, 2]
        )
        db_session.commit()

        # Monday with route1 filter
        schedules = db.schedules.get_schedules_for_day(0, route_id=route1.id)
        assert len(schedules) == 1
        assert schedules[0].route_id == route1.id

    def test_get_schedules_for_today(self, db, sample_route, sample_vehicle):
        """Test getting schedules for today."""
        # Create schedules for all days
        schedule = db.schedules.create(
            route_id=sample_route.id,
            vehicle_id=sample_vehicle.id,
            departure_time=time(8, 0),
            days_of_week=[0, 1, 2, 3, 4, 5, 6]  # All days
        )
        db.session.commit()

        today_schedules = db.schedules.get_schedules_for_today()
        assert len(today_schedules) >= 1  # At least our all-days schedule

    def test_get_next_departures(self, db, sample_route, sample_vehicle):
        """Test getting next departures for a route."""
        # Create multiple schedules
        db.schedules.create(
            route_id=sample_route.id,
            vehicle_id=sample_vehicle.id,
            departure_time=time(8, 0),
            days_of_week=[0, 1, 2, 3, 4, 5, 6]
        )
        db.schedules.create(
            route_id=sample_route.id,
            vehicle_id=sample_vehicle.id,
            departure_time=time(14, 0),
            days_of_week=[0, 1, 2, 3, 4, 5, 6]
        )
        db.session.commit()

        # Get next departures from a specific time
        reference_time = datetime.now().replace(hour=6, minute=0, second=0, microsecond=0)
        departures = db.schedules.get_next_departures(
            sample_route.id,
            from_datetime=reference_time,
            limit=5
        )

        assert len(departures) > 0
        assert all('schedule' in d for d in departures)
        assert all('next_departure' in d for d in departures)
        # Should be sorted by next_departure
        if len(departures) > 1:
            assert departures[0]['next_departure'] <= departures[1]['next_departure']

    # ==================== SCHEDULE METHOD TESTS ====================

    def test_is_active_today_weekday(self, db, sample_route, sample_vehicle):
        """Test is_active_today on a weekday schedule."""
        schedule = db.schedules.create(
            route_id=sample_route.id,
            vehicle_id=sample_vehicle.id,
            departure_time=time(8, 0),
            days_of_week=[0, 1, 2, 3, 4]  # Mon-Fri
        )
        db.session.commit()

        # Test with a Monday
        monday = datetime(2025, 11, 24)  # Known Monday
        assert schedule.is_active_today(monday) is True

        # Test with a Saturday
        saturday = datetime(2025, 11, 22)  # Known Saturday
        assert schedule.is_active_today(saturday) is False

    def test_is_active_today_inactive_schedule(self, db, sample_route, sample_vehicle):
        """Test is_active_today returns False for inactive schedules."""
        schedule = db.schedules.create(
            route_id=sample_route.id,
            vehicle_id=sample_vehicle.id,
            departure_time=time(8, 0),
            days_of_week=[0, 1, 2, 3, 4, 5, 6],
            is_active=False
        )
        db.session.commit()

        assert schedule.is_active_today() is False

    def test_get_next_departure_basic(self, db, sample_route, sample_vehicle):
        """Test get_next_departure basic functionality."""
        schedule = db.schedules.create(
            route_id=sample_route.id,
            vehicle_id=sample_vehicle.id,
            departure_time=time(14, 0),
            days_of_week=[0, 1, 2, 3, 4, 5, 6]
        )
        db.session.commit()

        # From early morning, should get today's 14:00
        reference = datetime.now().replace(hour=6, minute=0, second=0, microsecond=0)
        next_dep = schedule.get_next_departure(reference)

        assert next_dep is not None
        assert next_dep.time() == time(14, 0)
        assert next_dep > reference

    def test_get_next_departure_after_time_today(self, db, sample_route, sample_vehicle):
        """Test get_next_departure when today's departure has passed."""
        schedule = db.schedules.create(
            route_id=sample_route.id,
            vehicle_id=sample_vehicle.id,
            departure_time=time(8, 0),
            days_of_week=[0, 1, 2, 3, 4, 5, 6]
        )
        db.session.commit()

        # From late evening, should get tomorrow's 8:00
        reference = datetime.now().replace(hour=22, minute=0, second=0, microsecond=0)
        next_dep = schedule.get_next_departure(reference)

        assert next_dep is not None
        assert next_dep.time() == time(8, 0)
        assert next_dep > reference

    def test_get_next_departure_weekend_only(self, db, sample_route, sample_vehicle):
        """Test get_next_departure for weekend-only schedule."""
        schedule = db.schedules.create(
            route_id=sample_route.id,
            vehicle_id=sample_vehicle.id,
            departure_time=time(10, 0),
            days_of_week=[5, 6]  # Sat-Sun only
        )
        db.session.commit()

        # From a Monday
        monday = datetime(2025, 11, 24, 12, 0)  # Monday noon
        next_dep = schedule.get_next_departure(monday)

        assert next_dep is not None
        # Should be on Saturday or Sunday
        assert next_dep.weekday() in [5, 6]
        assert next_dep.time() == time(10, 0)

    def test_get_next_departure_inactive(self, db, sample_route, sample_vehicle):
        """Test get_next_departure returns None for inactive schedules."""
        schedule = db.schedules.create(
            route_id=sample_route.id,
            vehicle_id=sample_vehicle.id,
            departure_time=time(8, 0),
            days_of_week=[0, 1, 2, 3, 4, 5, 6],
            is_active=False
        )
        db.session.commit()

        next_dep = schedule.get_next_departure()
        assert next_dep is None

    # ==================== VALIDATION TESTS ====================

    def test_check_vehicle_availability_no_conflict(self, db, sample_route, sample_vehicle):
        """Test vehicle availability check with no conflicts."""
        # Create existing schedule at 8:00
        db.schedules.create(
            route_id=sample_route.id,
            vehicle_id=sample_vehicle.id,
            departure_time=time(8, 0),
            days_of_week=[0, 1, 2, 3, 4]
        )
        db.session.commit()

        # Check if vehicle is available at 10:00 (2 hours later)
        available = db.schedules.check_vehicle_availability(
            sample_vehicle.id,
            time(10, 0),
            [0, 1, 2, 3, 4]
        )
        assert available is True

    def test_check_vehicle_availability_conflict(self, db, sample_route, sample_vehicle):
        """Test vehicle availability check with time conflict."""
        # Create existing schedule at 8:00
        db.schedules.create(
            route_id=sample_route.id,
            vehicle_id=sample_vehicle.id,
            departure_time=time(8, 0),
            days_of_week=[0, 1, 2, 3, 4]
        )
        db.session.commit()

        # Check if vehicle is available at 8:15 (too close, within 30 min)
        available = db.schedules.check_vehicle_availability(
            sample_vehicle.id,
            time(8, 15),
            [0, 1, 2, 3, 4]
        )
        assert available is False

    def test_check_vehicle_availability_different_days(self, db, sample_route, sample_vehicle):
        """Test vehicle availability on different days."""
        # Create weekday schedule
        db.schedules.create(
            route_id=sample_route.id,
            vehicle_id=sample_vehicle.id,
            departure_time=time(8, 0),
            days_of_week=[0, 1, 2, 3, 4]  # Mon-Fri
        )
        db.session.commit()

        # Check same time but on weekend - should be available
        available = db.schedules.check_vehicle_availability(
            sample_vehicle.id,
            time(8, 0),
            [5, 6]  # Sat-Sun
        )
        assert available is True

