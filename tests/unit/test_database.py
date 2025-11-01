"""
Testes unitários para modelos de banco de dados.
"""

import pytest
from datetime import datetime
from decimal import Decimal
import uuid

from backend.database.models import (
    Agent, Building, Vehicle, Event, EconomicStat,
    Profession, Routine, NamePool,
    CreatedBy, HealthStatus, AgentStatus, Gender
)
from backend.database.connection import DatabaseManager
from backend.database.queries import DatabaseQueries


@pytest.fixture
def db_manager():
    """Fixture que cria gerenciador de banco de dados em memória."""
    import os
    os.environ['SQLITE_PATH'] = ':memory:'
    manager = DatabaseManager(use_sqlite=True)
    manager.init_database()
    yield manager
    manager.close()


@pytest.fixture
def session(db_manager):
    """Fixture que fornece sessão de banco de dados."""
    return db_manager.get_session()


@pytest.fixture
def queries(session):
    """Fixture que fornece objeto de queries."""
    return DatabaseQueries(session)


class TestAgentModel:
    """Testes para modelo Agent."""

    def test_create_agent(self, session):
        """Testa criação de agente básico."""
        agent = Agent(
            name="João Silva",
            created_by=CreatedBy.IA,
            birth_date=datetime(2000, 1, 1),
            gender=Gender.MALE,
            version="0.1.0"
        )
        session.add(agent)
        session.commit()

        assert agent.id is not None
        assert agent.name == "João Silva"
        assert agent.created_by == CreatedBy.IA
        assert agent.gender == Gender.MALE
        assert agent.wallet == Decimal('0.00')
        assert agent.is_deleted is False

    def test_agent_age_property(self, session):
        """Testa cálculo de idade do agente."""
        birth_date = datetime(2000, 1, 1)
        agent = Agent(
            name="Maria Santos",
            created_by=CreatedBy.BIRTH,
            birth_date=birth_date,
            gender=Gender.FEMALE,
            version="0.1.0"
        )
        session.add(agent)
        session.commit()

        # Idade deve ser aproximadamente a diferença de anos
        expected_age = (datetime.utcnow() - birth_date).days // 365
        assert agent.age == expected_age

    def test_agent_with_skills(self, session):
        """Testa agente com skills em JSON."""
        skills = {
            "programming": 80,
            "communication": 60,
            "leadership": 40
        }

        agent = Agent(
            name="Carlos Tech",
            created_by=CreatedBy.ADMIN,
            birth_date=datetime(1995, 5, 15),
            gender=Gender.MALE,
            skills=skills,
            version="0.1.0"
        )
        session.add(agent)
        session.commit()

        assert agent.skills == skills
        assert agent.skills["programming"] == 80

    def test_agent_with_genetics(self, session):
        """Testa agente com genética complexa."""
        genetics = {
            "hair_color": "brown",
            "eye_color": "blue",
            "height_factor": 1.05,
            "parent1_id": str(uuid.uuid4()),
            "parent2_id": str(uuid.uuid4())
        }

        agent = Agent(
            name="Ana Genetic",
            created_by=CreatedBy.BIRTH,
            birth_date=datetime(2020, 1, 1),
            gender=Gender.FEMALE,
            genetics=genetics,
            version="0.1.0"
        )
        session.add(agent)
        session.commit()

        assert agent.genetics["hair_color"] == "brown"
        assert agent.genetics["height_factor"] == 1.05

    def test_agent_wallet_limits(self, session):
        """Testa limites da carteira."""
        # Agente rico
        rich_agent = Agent(
            name="Rico Abastado",
            created_by=CreatedBy.ADMIN,
            birth_date=datetime(1980, 1, 1),
            gender=Gender.MALE,
            wallet=Decimal('99999999999.99'),  # 11 dígitos
            version="0.1.0"
        )
        session.add(rich_agent)

        # Agente pobre (negativo)
        poor_agent = Agent(
            name="Pobre Endividado",
            created_by=CreatedBy.IA,
            birth_date=datetime(1990, 1, 1),
            gender=Gender.FEMALE,
            wallet=Decimal('-100000.00'),
            version="0.1.0"
        )
        session.add(poor_agent)
        session.commit()

        assert rich_agent.wallet == Decimal('99999999999.99')
        assert poor_agent.wallet == Decimal('-100000.00')

    def test_agent_energy_level(self, session):
        """Testa níveis de energia incluindo negativos."""
        # Agente com deficiência
        agent = Agent(
            name="Pessoa Autista",
            created_by=CreatedBy.IA,
            birth_date=datetime(2000, 1, 1),
            gender=Gender.NON_BINARY,
            energy_level=-50,  # Pode ser negativo
            version="0.1.0"
        )
        session.add(agent)
        session.commit()

        assert agent.energy_level == -50


class TestBuildingModel:
    """Testes para modelo Building."""

    def test_create_building(self, session):
        """Testa criação de edifício."""
        building = Building(
            name="Casa Teste",
            building_type="residential",
            x=10,
            y=20,
            capacity=4,
            rent_cost=Decimal('800.00')
        )
        session.add(building)
        session.commit()

        assert building.id is not None
        assert building.name == "Casa Teste"
        assert building.building_type == "residential"
        assert building.capacity == 4
        assert building.current_occupancy == 0


class TestProfessionModel:
    """Testes para modelo Profession."""

    def test_create_profession(self, session):
        """Testa criação de profissão."""
        profession = Profession(
            name="Programador",
            description="Desenvolve software",
            base_salary=Decimal('5000.00'),
            work_sector="commercial",
            required_skills=["programming", "logic"]
        )
        session.add(profession)
        session.commit()

        assert profession.id is not None
        assert profession.name == "Programador"
        assert profession.base_salary == Decimal('5000.00')
        assert "programming" in profession.required_skills


class TestAgentQueries:
    """Testes para queries de agentes."""

    def test_get_by_id(self, session, queries):
        """Testa busca de agente por ID."""
        agent = Agent(
            name="Teste Query",
            created_by=CreatedBy.IA,
            birth_date=datetime(2000, 1, 1),
            gender=Gender.MALE,
            version="0.1.0"
        )
        session.add(agent)
        session.commit()

        found = queries.agents.get_by_id(agent.id)
        assert found is not None
        assert found.id == agent.id
        assert found.name == "Teste Query"

    def test_get_by_name(self, session, queries):
        """Testa busca de agente por nome."""
        agent = Agent(
            name="Maria Unique Name",
            created_by=CreatedBy.IA,
            birth_date=datetime(2000, 1, 1),
            gender=Gender.FEMALE,
            version="0.1.0"
        )
        session.add(agent)
        session.commit()

        found = queries.agents.get_by_name("Unique")
        assert len(found) > 0
        assert found[0].name == "Maria Unique Name"

    def test_get_by_status(self, session, queries):
        """Testa busca de agentes por status."""
        agent1 = Agent(
            name="Working Agent",
            created_by=CreatedBy.IA,
            birth_date=datetime(2000, 1, 1),
            gender=Gender.MALE,
            current_status=AgentStatus.WORKING,
            version="0.1.0"
        )
        agent2 = Agent(
            name="Idle Agent",
            created_by=CreatedBy.IA,
            birth_date=datetime(2000, 1, 1),
            gender=Gender.FEMALE,
            current_status=AgentStatus.IDLE,
            version="0.1.0"
        )
        session.add_all([agent1, agent2])
        session.commit()

        working = queries.agents.get_by_status(AgentStatus.WORKING)
        assert len(working) == 1
        assert working[0].name == "Working Agent"

    def test_soft_delete(self, session, queries):
        """Testa soft delete de agente."""
        agent = Agent(
            name="To Delete",
            created_by=CreatedBy.IA,
            birth_date=datetime(2000, 1, 1),
            gender=Gender.MALE,
            version="0.1.0"
        )
        session.add(agent)
        session.commit()

        agent_id = agent.id
        queries.agents.soft_delete(agent_id)
        session.commit()

        # Não deve aparecer em queries normais
        all_agents = queries.agents.get_all()
        assert len(all_agents) == 0

        # Mas deve existir com include_deleted
        all_with_deleted = queries.agents.get_all(include_deleted=True)
        assert len(all_with_deleted) == 1
        assert all_with_deleted[0].is_deleted is True

    def test_get_statistics(self, session, queries):
        """Testa obtenção de estatísticas."""
        agents = [
            Agent(
                name=f"Agent {i}",
                created_by=CreatedBy.IA,
                birth_date=datetime(2000, 1, 1),
                gender=Gender.MALE,
                wallet=Decimal(str(i * 1000)),
                current_status=AgentStatus.IDLE if i % 2 == 0 else AgentStatus.WORKING,
                version="0.1.0"
            )
            for i in range(5)
        ]
        session.add_all(agents)
        session.commit()

        stats = queries.agents.get_statistics()
        assert stats['total'] == 5
        assert stats['average_wallet'] > 0


class TestBuildingQueries:
    """Testes para queries de edifícios."""

    def test_get_by_type(self, session, queries):
        """Testa busca de edifícios por tipo."""
        building1 = Building(
            name="Casa 1",
            building_type="residential",
            x=0, y=0
        )
        building2 = Building(
            name="Loja 1",
            building_type="commercial",
            x=1, y=1
        )
        session.add_all([building1, building2])
        session.commit()

        residential = queries.buildings.get_by_type("residential")
        assert len(residential) == 1
        assert residential[0].name == "Casa 1"

    def test_get_with_vacancy(self, session, queries):
        """Testa busca de edifícios com vagas."""
        building1 = Building(
            name="Casa Cheia",
            building_type="residential",
            x=0, y=0,
            capacity=2,
            current_occupancy=2
        )
        building2 = Building(
            name="Casa com Vaga",
            building_type="residential",
            x=1, y=1,
            capacity=4,
            current_occupancy=2
        )
        session.add_all([building1, building2])
        session.commit()

        with_vacancy = queries.buildings.get_with_vacancy()
        assert len(with_vacancy) == 1
        assert with_vacancy[0].name == "Casa com Vaga"


class TestNamePoolQueries:
    """Testes para queries de pool de nomes."""

    def test_get_random_name(self, session, queries):
        """Testa obtenção de nome aleatório."""
        names = [
            NamePool(name="João", name_type="first", gender=Gender.MALE, rarity=1.0),
            NamePool(name="Maria", name_type="first", gender=Gender.FEMALE, rarity=1.0),
            NamePool(name="Silva", name_type="last", rarity=1.0),
        ]
        session.add_all(names)
        session.commit()

        first_name = queries.names.get_random_name("first", Gender.MALE)
        assert first_name is not None
        assert first_name.name_type == "first"

        last_name = queries.names.get_random_name("last")
        assert last_name is not None
        assert last_name.name_type == "last"

