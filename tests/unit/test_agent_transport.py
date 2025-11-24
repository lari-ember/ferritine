"""
Testes unitários para funcionalidades de transporte público do Agent.
"""
import pytest
from decimal import Decimal
from datetime import datetime, timedelta
from backend.database.models import (
    Agent, Station, Ticket, Route,
    HealthStatus, AgentStatus, Gender, CreatedBy,
    StationType, StationStatus, TicketType, TicketStatus
)
from backend.database.queries import DatabaseQueries


class TestAgentTransport:
    """Testes para funcionalidades de transporte público do Agent."""

    @pytest.fixture
    def sample_station(self, db_session):
        """Cria uma estação de exemplo."""
        station = Station(
            name="Estação Central",
            station_type=StationType.METRO_STATION,
            code="M01",
            x=100,
            y=200,
            max_queue_length=150,
            is_operational=True,
            status=StationStatus.ACTIVE
        )
        db_session.add(station)
        db_session.commit()
        return station

    @pytest.fixture
    def sample_route(self, db_session):
        """Cria uma rota de exemplo."""
        route = Route(
            name="Linha 1 - Metrô",
            route_type="metro",
            is_active=True
        )
        db_session.add(route)
        db_session.commit()
        return route

    @pytest.fixture
    def sample_agents(self, db_session):
        """Cria agentes de exemplo."""
        agents = []

        for i in range(5):
            agent = Agent(
                name=f"Agente {i}",
                birth_date=datetime(1990, 1, 1),
                gender=Gender.CIS_MALE if i % 2 == 0 else Gender.CIS_FEMALE,
                health_status=HealthStatus.HEALTHY,
                current_status=AgentStatus.IDLE,
                created_by=CreatedBy.IA,
                version="1.0",
                wallet=Decimal('100.00')
            )
            agents.append(agent)

        db_session.add_all(agents)
        db_session.commit()

        return agents

    def test_agent_has_transport_fields(self, db_session, sample_agents):
        """Testa que o agente tem os novos campos de transporte."""
        agent = sample_agents[0]

        assert hasattr(agent, 'waiting_at_station_id')
        assert hasattr(agent, 'current_ticket_id')
        assert hasattr(agent, 'waiting_at_station')
        assert hasattr(agent, 'current_ticket')
        assert hasattr(agent, 'tickets')

        # Valores padrão
        assert agent.waiting_at_station_id is None
        assert agent.current_ticket_id is None

    def test_agent_purchase_ticket(self, db_session, sample_agents, sample_route, sample_station):
        """Testa compra de bilhete pelo agente."""
        agent = sample_agents[0]
        initial_wallet = agent.wallet

        # Comprar bilhete
        ticket = agent.purchase_ticket(
            route_id=sample_route.id,
            origin_id=sample_station.id,
            price=Decimal('3.50')
        )

        db_session.add(ticket)
        db_session.flush()  # Flush para obter o ID do ticket

        # Agora atualizar o current_ticket_id do agente
        agent.current_ticket_id = ticket.id
        db_session.commit()

        # Verificar bilhete
        assert ticket is not None
        assert ticket.agent_id == agent.id
        assert ticket.route_id == sample_route.id
        assert ticket.origin_station_id == sample_station.id
        assert ticket.price == Decimal('3.50')
        assert ticket.status == TicketStatus.ACTIVE
        assert ticket.ticket_type == TicketType.SINGLE

        # Verificar desconto da carteira
        assert agent.wallet == initial_wallet - Decimal('3.50')

        # Verificar que é o bilhete atual
        assert agent.current_ticket_id == ticket.id

    def test_agent_purchase_ticket_insufficient_funds(self, db_session, sample_agents):
        """Testa compra de bilhete sem saldo suficiente."""
        agent = sample_agents[0]
        agent.wallet = Decimal('1.00')
        db_session.commit()

        # Tentar comprar bilhete mais caro
        with pytest.raises(ValueError, match="Saldo insuficiente"):
            agent.purchase_ticket(price=Decimal('5.00'))

    def test_agent_purchase_different_ticket_types(self, db_session, sample_agents, sample_route):
        """Testa compra de diferentes tipos de bilhete."""
        agent = sample_agents[0]
        agent.wallet = Decimal('1000.00')
        db_session.commit()

        # Bilhete único
        ticket1 = agent.purchase_ticket(
            route_id=sample_route.id,
            ticket_type=TicketType.SINGLE,
            price=Decimal('3.50')
        )
        db_session.add(ticket1)
        assert ticket1.max_validations == 1
        assert ticket1.valid_until is not None

        # Passe diário
        ticket2 = agent.purchase_ticket(
            route_id=sample_route.id,
            ticket_type=TicketType.DAY_PASS,
            price=Decimal('10.00')
        )
        db_session.add(ticket2)
        assert ticket2.max_validations == 999

        # Passe semanal
        ticket3 = agent.purchase_ticket(
            route_id=sample_route.id,
            ticket_type=TicketType.WEEK_PASS,
            price=Decimal('50.00')
        )
        db_session.add(ticket3)
        assert ticket3.max_validations == 999

        db_session.commit()

    def test_agent_wait_at_station(self, db_session, sample_agents, sample_station):
        """Testa agente aguardando em estação."""
        agent = sample_agents[0]

        # Aguardar na estação
        agent.wait_at_station(sample_station.id)
        db_session.commit()

        assert agent.waiting_at_station_id == sample_station.id
        assert agent.current_location_type == 'station'
        assert agent.current_location_id == sample_station.id

    def test_agent_leave_station(self, db_session, sample_agents, sample_station):
        """Testa agente saindo da estação."""
        agent = sample_agents[0]

        # Aguardar e depois sair
        agent.wait_at_station(sample_station.id)
        db_session.commit()

        agent.leave_station()
        db_session.commit()

        assert agent.waiting_at_station_id is None

    def test_get_waiting_at_station(self, db_session, sample_agents, sample_station):
        """Testa busca de agentes aguardando em estação."""
        db = DatabaseQueries(db_session)

        # Fazer 3 agentes aguardarem
        for agent in sample_agents[:3]:
            agent.wait_at_station(sample_station.id)

        db_session.commit()

        # Buscar agentes aguardando
        waiting = db.agents.get_waiting_at_station(sample_station.id)

        assert len(waiting) == 3
        assert all(a.waiting_at_station_id == sample_station.id for a in waiting)

    def test_get_with_active_ticket(self, db_session, sample_agents, sample_route):
        """Testa busca de agentes com bilhete válido."""
        db = DatabaseQueries(db_session)

        # Dar bilhetes a 2 agentes
        for agent in sample_agents[:2]:
            agent.wallet = Decimal('100.00')
            ticket = agent.purchase_ticket(
                route_id=sample_route.id,
                price=Decimal('3.50')
            )
            db_session.add(ticket)
            db_session.flush()
            agent.current_ticket_id = ticket.id

        db_session.commit()

        # Buscar agentes com bilhete ativo
        with_ticket = db.agents.get_with_active_ticket()

        assert len(with_ticket) == 2
        assert all(a.current_ticket_id is not None for a in with_ticket)

    def test_ticket_validation(self, db_session, sample_agents, sample_route):
        """Testa validação de bilhete."""
        agent = sample_agents[0]
        agent.wallet = Decimal('100.00')

        # Comprar bilhete
        ticket = agent.purchase_ticket(
            route_id=sample_route.id,
            ticket_type=TicketType.SINGLE,
            price=Decimal('3.50')
        )
        db_session.add(ticket)
        db_session.commit()

        # Verificar se é válido
        assert ticket.is_valid() is True

        # Validar (usar) o bilhete
        success = ticket.validate()
        assert success is True
        assert ticket.validation_count == 1
        assert ticket.used_at is not None
        assert ticket.status == TicketStatus.USED

        # Tentar validar novamente (deve falhar)
        success = ticket.validate()
        assert success is False

    def test_ticket_expiration(self, db_session, sample_agents, sample_route):
        """Testa expiração de bilhete."""
        agent = sample_agents[0]
        agent.wallet = Decimal('100.00')

        # Comprar bilhete
        ticket = agent.purchase_ticket(
            route_id=sample_route.id,
            ticket_type=TicketType.SINGLE,
            price=Decimal('3.50')
        )
        db_session.add(ticket)
        db_session.commit()

        # Forçar expiração
        ticket.valid_until = datetime.utcnow() - timedelta(hours=1)
        db_session.commit()

        # Verificar se está expirado
        assert ticket.is_valid() is False
        assert ticket.status == TicketStatus.EXPIRED

    def test_ticket_cancellation(self, db_session, sample_agents, sample_route):
        """Testa cancelamento de bilhete."""
        agent = sample_agents[0]
        agent.wallet = Decimal('100.00')

        # Comprar bilhete
        ticket = agent.purchase_ticket(
            route_id=sample_route.id,
            price=Decimal('3.50')
        )
        db_session.add(ticket)
        db_session.commit()

        # Cancelar
        ticket.cancel()
        db_session.commit()

        assert ticket.status == TicketStatus.CANCELLED
        assert ticket.cancelled_at is not None
        assert ticket.is_valid() is False

    def test_agent_station_relationship(self, db_session, sample_agents, sample_station):
        """Testa relacionamento Agent-Station."""
        agent = sample_agents[0]

        # Aguardar na estação
        agent.wait_at_station(sample_station.id)
        db_session.commit()
        db_session.refresh(agent)

        # Verificar relacionamento
        assert agent.waiting_at_station is not None
        assert agent.waiting_at_station.id == sample_station.id
        assert agent.waiting_at_station.name == "Estação Central"

    def test_agent_ticket_relationship(self, db_session, sample_agents, sample_route):
        """Testa relacionamento Agent-Ticket."""
        agent = sample_agents[0]
        agent.wallet = Decimal('100.00')

        # Comprar múltiplos bilhetes
        tickets = []
        for i in range(3):
            ticket = agent.purchase_ticket(
                route_id=sample_route.id,
                price=Decimal('3.50')
            )
            tickets.append(ticket)
            db_session.add(ticket)

        db_session.flush()

        # Definir o último como atual
        agent.current_ticket_id = tickets[-1].id
        db_session.commit()
        db_session.refresh(agent)

        # Verificar relacionamento
        assert len(agent.tickets) >= 3
        assert agent.current_ticket is not None
        assert agent.current_ticket.id == tickets[-1].id

    def test_multiple_agents_same_station(self, db_session, sample_agents, sample_station):
        """Testa múltiplos agentes na mesma estação."""
        db = DatabaseQueries(db_session)

        # Todos os agentes aguardam na mesma estação
        for agent in sample_agents:
            agent.wait_at_station(sample_station.id)

        db_session.commit()

        # Verificar
        waiting = db.agents.get_waiting_at_station(sample_station.id)
        assert len(waiting) == len(sample_agents)

    def test_agent_journey_complete(self, db_session, sample_agents, sample_station, sample_route):
        """Teste de jornada completa de um agente."""
        db = DatabaseQueries(db_session)
        agent = sample_agents[0]
        agent.wallet = Decimal('100.00')

        # 1. Comprar bilhete
        ticket = agent.purchase_ticket(
            route_id=sample_route.id,
            origin_id=sample_station.id,
            ticket_type=TicketType.SINGLE,
            price=Decimal('3.50')
        )
        db_session.add(ticket)
        db_session.commit()

        assert agent.wallet == Decimal('96.50')

        # 2. Aguardar na estação
        agent.wait_at_station(sample_station.id)
        db_session.commit()

        waiting = db.agents.get_waiting_at_station(sample_station.id)
        assert agent in waiting

        # 3. Validar bilhete
        assert ticket.is_valid() is True
        ticket.validate()
        db_session.commit()

        assert ticket.validation_count == 1
        assert ticket.status == TicketStatus.USED

        # 4. Embarcar (sair da estação)
        agent.leave_station()
        db_session.commit()

        assert agent.waiting_at_station_id is None

        # 5. Verificar que não está mais na lista de espera
        waiting = db.agents.get_waiting_at_station(sample_station.id)
        assert agent not in waiting

    def test_day_pass_multiple_validations(self, db_session, sample_agents, sample_route):
        """Testa passe diário com múltiplas validações."""
        agent = sample_agents[0]
        agent.wallet = Decimal('100.00')

        # Comprar passe diário
        ticket = agent.purchase_ticket(
            route_id=sample_route.id,
            ticket_type=TicketType.DAY_PASS,
            price=Decimal('10.00')
        )
        db_session.add(ticket)
        db_session.commit()

        # Validar múltiplas vezes
        for i in range(10):
            assert ticket.is_valid() is True
            success = ticket.validate()
            assert success is True
            assert ticket.validation_count == i + 1

        # Ainda deve ser válido
        assert ticket.status == TicketStatus.ACTIVE
        assert ticket.is_valid() is True

    def test_transfer_ticket(self, db_session, sample_agents, sample_route):
        """Testa bilhete de transferência."""
        agent = sample_agents[0]
        agent.wallet = Decimal('100.00')

        # Comprar bilhete de transferência
        ticket = agent.purchase_ticket(
            route_id=sample_route.id,
            ticket_type=TicketType.TRANSFER,
            price=Decimal('0.00')  # Transferência gratuita
        )
        db_session.add(ticket)
        db_session.commit()

        # Verificar validade curta (30 minutos)
        assert ticket.valid_until is not None
        time_diff = (ticket.valid_until - ticket.valid_from).total_seconds()
        assert time_diff == 30 * 60  # 30 minutos

    def test_return_ticket(self, db_session, sample_agents, sample_route, sample_station):
        """Testa bilhete de ida e volta."""
        agent = sample_agents[0]
        agent.wallet = Decimal('100.00')

        # Criar estação de destino
        station_dest = Station(
            name="Estação Sul",
            station_type=StationType.METRO_STATION,
            code="M02",
            x=200,
            y=100,
            max_queue_length=150,
            is_operational=True,
            status=StationStatus.ACTIVE
        )
        db_session.add(station_dest)
        db_session.commit()

        # Comprar bilhete de ida e volta
        ticket = agent.purchase_ticket(
            route_id=sample_route.id,
            origin_id=sample_station.id,
            destination_id=station_dest.id,
            ticket_type=TicketType.RETURN,
            price=Decimal('6.00')
        )
        db_session.add(ticket)
        db_session.commit()

        # Validar ida
        ticket.validate()
        assert ticket.validation_count == 1
        assert ticket.status == TicketStatus.ACTIVE

        # Validar volta
        ticket.validate()
        assert ticket.validation_count == 2
        assert ticket.status == TicketStatus.USED

    def test_agent_no_ticket_not_in_active_list(self, db_session, sample_agents):
        """Testa que agentes sem bilhete não aparecem na lista de bilhetes ativos."""
        db = DatabaseQueries(db_session)

        # Buscar agentes com bilhete
        with_ticket = db.agents.get_with_active_ticket()

        # Nenhum agente tem bilhete ainda
        assert len(with_ticket) == 0

    def test_empty_station_waiting_list(self, db_session, sample_station):
        """Testa lista vazia de espera em estação."""
        db = DatabaseQueries(db_session)

        waiting = db.agents.get_waiting_at_station(sample_station.id)
        assert len(waiting) == 0

