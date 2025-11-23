"""
Unit tests for Ticket model (Issue 4.7).

Tests the ticket/fare system implementation including:
- Ticket creation
- Ticket validation
- Ticket usage
- Ticket expiration
"""
import pytest
from decimal import Decimal
from datetime import datetime, timedelta
from uuid import uuid4

from backend.database.models import (
    Agent, Station, Ticket, Route,
    HealthStatus, AgentStatus, Gender, CreatedBy,
    StationType, StationStatus, TicketType, TicketStatus,
    RouteStatus, RoutePriority
)


class TestTicketModel:
    """Test suite for Ticket model (Issue 4.7)."""

    @pytest.fixture
    def sample_agent(self, db_session):
        """Create a sample agent for testing."""
        agent = Agent(
            name="Test Agent",
            birth_date=datetime(1990, 1, 1),
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
    def sample_station_origin(self, db_session):
        """Create origin station for testing."""
        station = Station(
            name="Station A",
            station_type=StationType.TRAIN_STEAM,
            code="TA",
            x=100,
            y=100,
            status=StationStatus.ACTIVE,
            is_operational=True
        )
        db_session.add(station)
        db_session.commit()
        return station

    @pytest.fixture
    def sample_station_destination(self, db_session):
        """Create destination station for testing."""
        station = Station(
            name="Station B",
            station_type=StationType.TRAIN_STEAM,
            code="TB",
            x=200,
            y=200,
            status=StationStatus.ACTIVE,
            is_operational=True
        )
        db_session.add(station)
        db_session.commit()
        return station

    @pytest.fixture
    def sample_route(self, db_session):
        """Create a sample route for testing."""
        route = Route(
            name="Test Train Line",
            code="TT1",
            route_type=StationType.TRAIN_STEAM,  # Use enum that's confirmed to exist
            is_active=True,
            status=RouteStatus.ACTIVE,
            priority=RoutePriority.MEDIUM,
            fare_base=Decimal('3.50')
        )
        db_session.add(route)
        db_session.commit()
        return route

    # ==================== TEST: CREATE TICKET ====================

    def test_create_ticket(self, db_session, sample_agent, sample_route,
                          sample_station_origin, sample_station_destination):
        """Test ticket creation with all required fields (Issue 4.7)."""
        ticket = Ticket(
            agent_id=sample_agent.id,
            route_id=sample_route.id,
            origin_station_id=sample_station_origin.id,
            destination_station_id=sample_station_destination.id,
            price=Decimal('3.50'),
            ticket_type=TicketType.SINGLE,
            status=TicketStatus.ACTIVE
        )

        db_session.add(ticket)
        db_session.commit()

        # Verify ticket was created
        assert ticket.id is not None
        assert ticket.agent_id == sample_agent.id
        assert ticket.route_id == sample_route.id
        assert ticket.origin_station_id == sample_station_origin.id
        assert ticket.destination_station_id == sample_station_destination.id
        assert ticket.price == Decimal('3.50')
        assert ticket.ticket_type == TicketType.SINGLE
        assert ticket.status == TicketStatus.ACTIVE
        assert ticket.purchased_at is not None
        assert ticket.created_at is not None
        assert ticket.validation_count == 0
        assert ticket.max_validations == 1

    def test_create_ticket_minimal_fields(self, db_session, sample_agent):
        """Test ticket creation with minimal required fields."""
        ticket = Ticket(
            agent_id=sample_agent.id,
            price=Decimal('3.50')
        )

        db_session.add(ticket)
        db_session.commit()

        # Verify defaults
        assert ticket.id is not None
        assert ticket.agent_id == sample_agent.id
        assert ticket.ticket_type == TicketType.SINGLE
        assert ticket.status == TicketStatus.ACTIVE
        assert ticket.validation_count == 0
        assert ticket.max_validations == 1

    def test_create_ticket_different_types(self, db_session, sample_agent, sample_route):
        """Test creating different types of tickets."""
        ticket_types = [
            (TicketType.SINGLE, Decimal('3.50'), 1),
            (TicketType.RETURN, Decimal('6.00'), 2),
            (TicketType.DAY_PASS, Decimal('15.00'), 999),
            (TicketType.WEEK_PASS, Decimal('50.00'), 999),
            (TicketType.MONTH_PASS, Decimal('150.00'), 999),
            (TicketType.TRANSFER, Decimal('1.00'), 1),
        ]

        for ticket_type, price, expected_max_validations in ticket_types:
            ticket = Ticket(
                agent_id=sample_agent.id,
                route_id=sample_route.id,
                ticket_type=ticket_type,
                price=price,
                max_validations=expected_max_validations
            )
            db_session.add(ticket)

        db_session.commit()

        # Verify all tickets were created
        tickets = db_session.query(Ticket).filter_by(agent_id=sample_agent.id).all()
        assert len(tickets) == 6

    # ==================== TEST: VALIDATE TICKET ====================

    def test_validate_ticket(self, db_session, sample_agent, sample_route):
        """Test ticket validation method (Issue 4.7)."""
        ticket = Ticket(
            agent_id=sample_agent.id,
            route_id=sample_route.id,
            price=Decimal('3.50'),
            ticket_type=TicketType.SINGLE,
            status=TicketStatus.ACTIVE,
            valid_until=datetime.utcnow() + timedelta(hours=2)
        )

        db_session.add(ticket)
        db_session.commit()

        # Initial state
        assert ticket.is_valid() is True
        assert ticket.validation_count == 0
        assert ticket.used_at is None

        # Validate the ticket
        result = ticket.validate()
        assert result is True
        assert ticket.validation_count == 1
        assert ticket.used_at is not None
        assert ticket.status == TicketStatus.USED

        # Try to validate again (should fail)
        result = ticket.validate()
        assert result is False
        assert ticket.validation_count == 1  # Should not increment

    def test_validate_ticket_multiple_validations(self, db_session, sample_agent, sample_route):
        """Test ticket with multiple allowed validations."""
        ticket = Ticket(
            agent_id=sample_agent.id,
            route_id=sample_route.id,
            price=Decimal('10.00'),
            ticket_type=TicketType.DAY_PASS,
            status=TicketStatus.ACTIVE,
            max_validations=5,
            valid_until=datetime.utcnow() + timedelta(days=1)
        )

        db_session.add(ticket)
        db_session.commit()

        # Validate multiple times
        for i in range(5):
            result = ticket.validate()
            assert result is True
            assert ticket.validation_count == i + 1

        # Should still be active until max validations reached
        assert ticket.status == TicketStatus.USED

        # Try one more time (should fail)
        result = ticket.validate()
        assert result is False

    def test_validate_invalid_ticket(self, db_session, sample_agent):
        """Test validating an already invalid ticket."""
        ticket = Ticket(
            agent_id=sample_agent.id,
            price=Decimal('3.50'),
            status=TicketStatus.CANCELLED
        )

        db_session.add(ticket)
        db_session.commit()

        result = ticket.validate()
        assert result is False
        assert ticket.validation_count == 0

    # ==================== TEST: USE TICKET ====================

    def test_use_ticket(self, db_session, sample_agent, sample_route):
        """Test using a ticket (marking it as used) - Issue 4.7."""
        ticket = Ticket(
            agent_id=sample_agent.id,
            route_id=sample_route.id,
            price=Decimal('3.50'),
            ticket_type=TicketType.SINGLE,
            status=TicketStatus.ACTIVE,
            valid_until=datetime.utcnow() + timedelta(hours=2)
        )

        db_session.add(ticket)
        db_session.commit()

        # Use the ticket
        result = ticket.validate()
        assert result is True

        # Verify it's marked as used
        assert ticket.status == TicketStatus.USED
        assert ticket.used_at is not None
        assert ticket.validation_count == 1

        # Cannot use again
        assert ticket.is_valid() is False

    def test_use_ticket_tracks_first_use_time(self, db_session, sample_agent, sample_route):
        """Test that used_at is set on first validation only."""
        ticket = Ticket(
            agent_id=sample_agent.id,
            route_id=sample_route.id,
            price=Decimal('15.00'),
            ticket_type=TicketType.DAY_PASS,
            status=TicketStatus.ACTIVE,
            max_validations=10,
            valid_until=datetime.utcnow() + timedelta(days=1)
        )

        db_session.add(ticket)
        db_session.commit()

        # First validation
        ticket.validate()
        first_used_at = ticket.used_at
        assert first_used_at is not None

        # Second validation (used_at should not change)
        import time
        time.sleep(0.01)  # Small delay
        ticket.validate()
        assert ticket.used_at == first_used_at

    # ==================== TEST: EXPIRED TICKET ====================

    def test_expired_ticket(self, db_session, sample_agent, sample_route):
        """Test ticket expiration checking (Issue 4.7)."""
        # Create ticket that's already expired
        ticket = Ticket(
            agent_id=sample_agent.id,
            route_id=sample_route.id,
            price=Decimal('3.50'),
            ticket_type=TicketType.SINGLE,
            status=TicketStatus.ACTIVE,
            valid_until=datetime.utcnow() - timedelta(hours=1)
        )

        db_session.add(ticket)
        db_session.commit()

        # Check if expired
        is_valid = ticket.is_valid()

        assert is_valid is False
        assert ticket.status == TicketStatus.EXPIRED

    def test_expired_ticket_cannot_be_used(self, db_session, sample_agent, sample_route):
        """Test that expired tickets cannot be validated."""
        ticket = Ticket(
            agent_id=sample_agent.id,
            route_id=sample_route.id,
            price=Decimal('3.50'),
            status=TicketStatus.ACTIVE,
            valid_until=datetime.utcnow() - timedelta(minutes=1)
        )

        db_session.add(ticket)
        db_session.commit()

        # Try to use expired ticket
        result = ticket.validate()
        assert result is False
        assert ticket.validation_count == 0
        assert ticket.used_at is None

    def test_ticket_not_yet_valid(self, db_session, sample_agent, sample_route):
        """Test ticket that's not yet valid (valid_from in future)."""
        ticket = Ticket(
            agent_id=sample_agent.id,
            route_id=sample_route.id,
            price=Decimal('3.50'),
            status=TicketStatus.ACTIVE,
            valid_from=datetime.utcnow() + timedelta(hours=1),
            valid_until=datetime.utcnow() + timedelta(hours=3)
        )

        db_session.add(ticket)
        db_session.commit()

        # Should not be valid yet
        assert ticket.is_valid() is False

        # Cannot validate
        result = ticket.validate()
        assert result is False

    def test_ticket_with_no_expiration(self, db_session, sample_agent, sample_route):
        """Test ticket with no expiration date (valid_until is None)."""
        ticket = Ticket(
            agent_id=sample_agent.id,
            route_id=sample_route.id,
            price=Decimal('50.00'),
            ticket_type=TicketType.MONTH_PASS,
            status=TicketStatus.ACTIVE,
            max_validations=999,
            valid_until=None  # No expiration
        )

        db_session.add(ticket)
        db_session.commit()

        # Should be valid
        assert ticket.is_valid() is True

        # Can validate
        result = ticket.validate()
        assert result is True

    # ==================== TEST: TICKET CANCELLATION ====================

    def test_cancel_ticket(self, db_session, sample_agent, sample_route):
        """Test ticket cancellation."""
        ticket = Ticket(
            agent_id=sample_agent.id,
            route_id=sample_route.id,
            price=Decimal('3.50'),
            status=TicketStatus.ACTIVE,
            valid_until=datetime.utcnow() + timedelta(hours=2)
        )

        db_session.add(ticket)
        db_session.commit()

        # Cancel the ticket
        ticket.cancel()
        db_session.commit()

        assert ticket.status == TicketStatus.CANCELLED
        assert ticket.cancelled_at is not None
        assert ticket.is_valid() is False

    def test_cancelled_ticket_cannot_be_validated(self, db_session, sample_agent, sample_route):
        """Test that cancelled tickets cannot be validated."""
        ticket = Ticket(
            agent_id=sample_agent.id,
            route_id=sample_route.id,
            price=Decimal('3.50'),
            status=TicketStatus.ACTIVE
        )

        db_session.add(ticket)
        db_session.commit()

        # Cancel and try to use
        ticket.cancel()
        result = ticket.validate()

        assert result is False
        assert ticket.validation_count == 0

    # ==================== TEST: RELATIONSHIPS ====================

    def test_ticket_agent_relationship(self, db_session, sample_agent, sample_route):
        """Test relationship between Ticket and Agent."""
        ticket = Ticket(
            agent_id=sample_agent.id,
            route_id=sample_route.id,
            price=Decimal('3.50')
        )

        db_session.add(ticket)
        db_session.commit()

        # Access relationship
        assert ticket.agent is not None
        assert ticket.agent.id == sample_agent.id
        assert ticket.agent.name == "Test Agent"

        # Reverse relationship
        assert sample_agent.tickets is not None
        assert len(sample_agent.tickets) == 1
        assert sample_agent.tickets[0].id == ticket.id

    def test_ticket_route_relationship(self, db_session, sample_agent, sample_route):
        """Test relationship between Ticket and Route."""
        ticket = Ticket(
            agent_id=sample_agent.id,
            route_id=sample_route.id,
            price=Decimal('3.50')
        )

        db_session.add(ticket)
        db_session.commit()

        # Access relationship
        assert ticket.route is not None
        assert ticket.route.id == sample_route.id
        assert ticket.route.name == "Test Metro Line"

    def test_ticket_station_relationships(self, db_session, sample_agent, sample_route,
                                         sample_station_origin, sample_station_destination):
        """Test relationships between Ticket and Stations."""
        ticket = Ticket(
            agent_id=sample_agent.id,
            route_id=sample_route.id,
            origin_station_id=sample_station_origin.id,
            destination_station_id=sample_station_destination.id,
            price=Decimal('3.50')
        )

        db_session.add(ticket)
        db_session.commit()

        # Access relationships
        assert ticket.origin_station is not None
        assert ticket.origin_station.id == sample_station_origin.id
        assert ticket.origin_station.name == "Station A"

        assert ticket.destination_station is not None
        assert ticket.destination_station.id == sample_station_destination.id
        assert ticket.destination_station.name == "Station B"

    # ==================== TEST: CONSTRAINTS ====================

    def test_ticket_validation_count_constraints(self, db_session, sample_agent):
        """Test validation count constraints."""
        ticket = Ticket(
            agent_id=sample_agent.id,
            price=Decimal('3.50'),
            validation_count=2,
            max_validations=5
        )

        db_session.add(ticket)
        db_session.commit()

        # Should be within valid range
        assert ticket.validation_count <= ticket.max_validations

    def test_ticket_fare_zone(self, db_session, sample_agent, sample_route):
        """Test ticket fare zone field."""
        ticket = Ticket(
            agent_id=sample_agent.id,
            route_id=sample_route.id,
            price=Decimal('5.00'),
            fare_zone=3
        )

        db_session.add(ticket)
        db_session.commit()

        assert ticket.fare_zone == 3

    def test_ticket_notes(self, db_session, sample_agent, sample_route):
        """Test ticket notes field."""
        ticket = Ticket(
            agent_id=sample_agent.id,
            route_id=sample_route.id,
            price=Decimal('3.50'),
            notes="Promotional ticket - 50% discount"
        )

        db_session.add(ticket)
        db_session.commit()

        assert ticket.notes == "Promotional ticket - 50% discount"

    # ==================== TEST: REPR ====================

    def test_ticket_repr(self, db_session, sample_agent, sample_route):
        """Test ticket string representation."""
        ticket = Ticket(
            agent_id=sample_agent.id,
            route_id=sample_route.id,
            price=Decimal('3.50'),
            ticket_type=TicketType.SINGLE,
            status=TicketStatus.ACTIVE
        )

        db_session.add(ticket)
        db_session.commit()

        repr_str = repr(ticket)
        assert "Ticket" in repr_str
        assert "single" in repr_str
        assert "active" in repr_str
        assert str(sample_agent.id) in repr_str

    # ==================== TEST: EDGE CASES ====================

    def test_ticket_max_validations_reached(self, db_session, sample_agent, sample_route):
        """Test behavior when max validations is reached."""
        ticket = Ticket(
            agent_id=sample_agent.id,
            route_id=sample_route.id,
            price=Decimal('3.50'),
            max_validations=1,
            validation_count=0,
            valid_until=datetime.utcnow() + timedelta(hours=2)
        )

        db_session.add(ticket)
        db_session.commit()

        # First validation should succeed
        assert ticket.validate() is True
        assert ticket.status == TicketStatus.USED

        # Second validation should fail
        assert ticket.validate() is False
        assert ticket.validation_count == 1

    def test_ticket_price_decimal_precision(self, db_session, sample_agent):
        """Test that ticket price maintains decimal precision."""
        ticket = Ticket(
            agent_id=sample_agent.id,
            price=Decimal('3.75')
        )

        db_session.add(ticket)
        db_session.commit()

        # Reload from database
        db_session.expire(ticket)
        db_session.refresh(ticket)

        assert ticket.price == Decimal('3.75')
        assert isinstance(ticket.price, Decimal)

