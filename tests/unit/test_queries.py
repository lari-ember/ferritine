"""Tests for TicketQueries (Issue 4.8)."""
from decimal import Decimal
from datetime import datetime, timedelta
import uuid
import pytest

from backend.database.models import Agent, TicketType, TicketStatus, Route, StationType, CreatedBy, HealthStatus, AgentStatus, Gender
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
        route_type=StationType.TRAIN_STEAM,
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

