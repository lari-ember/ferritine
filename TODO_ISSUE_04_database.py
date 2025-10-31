# TODO: Issue #4 - Configurar banco de dados SQLite
# Labels: feat, phase-0: fundamentals, priority: critical, area: database, complexity: intermediate
# Milestone: Milestone 0: Fundamentos e Infraestrutura

"""
Implementar persistência de dados usando SQLite e SQLAlchemy.

Tarefas:
- [ ] Instalar SQLAlchemy (pip install sqlalchemy)
- [ ] Criar backend/database/models.py com modelos:
  - Agente (Agent)
  - Edifício (Building)
  - Veículo (Vehicle)
  - Evento (Event)
  - Estatística Econômica (EconomicStat)
- [ ] Implementar backend/database/queries.py com consultas comuns
- [ ] Criar sistema de migrations (Alembic)
- [ ] Implementar funções de save/load da simulação
- [ ] Adicionar testes para modelos de banco de dados

Critérios de Aceitação:
- Banco de dados criado em data/city.db
- Modelos SQLAlchemy funcionais
- Operações CRUD funcionando
- Migrations configuradas
"""

from sqlalchemy import create_engine, Column, Integer, String, Float, Boolean, DateTime, ForeignKey
from sqlalchemy.ext.declarative import declarative_base
from sqlalchemy.orm import relationship, sessionmaker
from datetime import datetime
from typing import List, Optional

Base = declarative_base()

# TODO: Modelo Agent (Agente)
class Agent(Base):
    """Modelo de banco de dados para Agentes."""
    __tablename__ = 'agents'
    
    id = Column(Integer, primary_key=True)
    name = Column(String(100), nullable=False)
    
    # Localização
    home_building_id = Column(Integer, ForeignKey('buildings.id'))
    work_building_id = Column(Integer, ForeignKey('buildings.id'))
    current_location_x = Column(Integer, default=0)
    current_location_y = Column(Integer, default=0)
    
    # Atributos físicos
    health = Column(Float, default=100.0)
    hunger = Column(Float, default=0.0)
    energy = Column(Float, default=100.0)
    
    # Atributos econômicos
    money = Column(Float, default=1000.0)
    salary = Column(Float, default=1000.0)
    
    # Estado
    current_state = Column(String(50), default='idle')  # idle, working, traveling, sleeping
    
    # Timestamps
    created_at = Column(DateTime, default=datetime.utcnow)
    updated_at = Column(DateTime, default=datetime.utcnow, onupdate=datetime.utcnow)
    
    # Relacionamentos
    home = relationship("Building", foreign_keys=[home_building_id], back_populates="residents")
    workplace = relationship("Building", foreign_keys=[work_building_id], back_populates="workers")
    
    def __repr__(self):
        return f"<Agent(id={self.id}, name='{self.name}', state='{self.current_state}')>"


# TODO: Modelo Building (Edifício)
class Building(Base):
    """Modelo de banco de dados para Edifícios."""
    __tablename__ = 'buildings'
    
    id = Column(Integer, primary_key=True)
    name = Column(String(100), nullable=False)
    building_type = Column(String(50), nullable=False)  # residential, commercial, industrial, public
    
    # Localização
    x = Column(Integer, nullable=False)
    y = Column(Integer, nullable=False)
    width = Column(Integer, default=1)
    height = Column(Integer, default=1)
    
    # Propriedades
    capacity = Column(Integer, default=10)
    current_occupancy = Column(Integer, default=0)
    
    # Economia
    rent_cost = Column(Float, default=500.0)
    operating_cost = Column(Float, default=100.0)
    
    # Estado
    is_active = Column(Boolean, default=True)
    
    # Timestamps
    created_at = Column(DateTime, default=datetime.utcnow)
    updated_at = Column(DateTime, default=datetime.utcnow, onupdate=datetime.utcnow)
    
    # Relacionamentos
    residents = relationship("Agent", foreign_keys=[Agent.home_building_id], back_populates="home")
    workers = relationship("Agent", foreign_keys=[Agent.work_building_id], back_populates="workplace")
    
    def __repr__(self):
        return f"<Building(id={self.id}, name='{self.name}', type='{self.building_type}')>"


# TODO: Modelo Vehicle (Veículo/Trem)
class Vehicle(Base):
    """Modelo de banco de dados para Veículos."""
    __tablename__ = 'vehicles'
    
    id = Column(Integer, primary_key=True)
    name = Column(String(100), nullable=False)
    vehicle_type = Column(String(50), nullable=False)  # train, bus, car
    
    # Localização
    current_x = Column(Integer, default=0)
    current_y = Column(Integer, default=0)
    
    # Capacidade
    passenger_capacity = Column(Integer, default=50)
    current_passengers = Column(Integer, default=0)
    cargo_capacity = Column(Float, default=0.0)
    current_cargo = Column(Float, default=0.0)
    
    # Estado
    is_moving = Column(Boolean, default=False)
    speed = Column(Float, default=1.0)
    
    # Timestamps
    created_at = Column(DateTime, default=datetime.utcnow)
    updated_at = Column(DateTime, default=datetime.utcnow, onupdate=datetime.utcnow)
    
    def __repr__(self):
        return f"<Vehicle(id={self.id}, name='{self.name}', type='{self.vehicle_type}')>"


# TODO: Modelo Event (Evento)
class Event(Base):
    """Modelo de banco de dados para Eventos da simulação."""
    __tablename__ = 'events'
    
    id = Column(Integer, primary_key=True)
    event_type = Column(String(50), nullable=False)  # agent_action, economic, transport
    description = Column(String(500))
    
    # Relacionamentos opcionais
    agent_id = Column(Integer, ForeignKey('agents.id'), nullable=True)
    building_id = Column(Integer, ForeignKey('buildings.id'), nullable=True)
    vehicle_id = Column(Integer, ForeignKey('vehicles.id'), nullable=True)
    
    # Timestamp
    occurred_at = Column(DateTime, default=datetime.utcnow)
    simulation_time = Column(Integer)  # Hora da simulação (0-23)
    
    def __repr__(self):
        return f"<Event(id={self.id}, type='{self.event_type}', time={self.simulation_time})>"


# TODO: Modelo EconomicStat (Estatística Econômica)
class EconomicStat(Base):
    """Modelo de banco de dados para Estatísticas Econômicas."""
    __tablename__ = 'economic_stats'
    
    id = Column(Integer, primary_key=True)
    simulation_time = Column(Integer, nullable=False)  # Hora da simulação
    
    # Estatísticas agregadas
    total_money_in_circulation = Column(Float, default=0.0)
    average_agent_money = Column(Float, default=0.0)
    total_transactions = Column(Integer, default=0)
    
    # Por setor
    residential_income = Column(Float, default=0.0)
    commercial_income = Column(Float, default=0.0)
    industrial_income = Column(Float, default=0.0)
    
    # Timestamp
    recorded_at = Column(DateTime, default=datetime.utcnow)
    
    def __repr__(self):
        return f"<EconomicStat(time={self.simulation_time}, circulation={self.total_money_in_circulation})>"


# TODO: Criar engine e session
def get_engine(db_path: str = "data/db/city.db"):
    """Cria engine do SQLAlchemy."""
    from pathlib import Path
    Path(db_path).parent.mkdir(parents=True, exist_ok=True)
    return create_engine(f'sqlite:///{db_path}', echo=False)


def get_session(engine):
    """Cria session do SQLAlchemy."""
    Session = sessionmaker(bind=engine)
    return Session()


def init_database(db_path: str = "data/db/city.db"):
    """Inicializa banco de dados criando todas as tabelas."""
    engine = get_engine(db_path)
    Base.metadata.create_all(engine)
    return engine


# TODO: Implementar queries comuns (backend/database/queries.py)
class DatabaseQueries:
    """Queries comuns do banco de dados."""
    
    def __init__(self, session):
        self.session = session
    
    # TODO: Queries de Agent
    def get_agent_by_id(self, agent_id: int) -> Optional[Agent]:
        return self.session.query(Agent).filter(Agent.id == agent_id).first()
    
    def get_all_agents(self) -> List[Agent]:
        return self.session.query(Agent).all()
    
    def get_agents_at_location(self, x: int, y: int) -> List[Agent]:
        return self.session.query(Agent).filter(
            Agent.current_location_x == x,
            Agent.current_location_y == y
        ).all()
    
    # TODO: Queries de Building
    def get_building_by_id(self, building_id: int) -> Optional[Building]:
        return self.session.query(Building).filter(Building.id == building_id).first()
    
    def get_buildings_by_type(self, building_type: str) -> List[Building]:
        return self.session.query(Building).filter(Building.building_type == building_type).all()
    
    # TODO: Queries de Statistics
    def get_latest_economic_stats(self) -> Optional[EconomicStat]:
        return self.session.query(EconomicStat).order_by(
            EconomicStat.simulation_time.desc()
        ).first()
    
    # TODO: Mais queries conforme necessário
    pass


# TODO: Implementar save/load da simulação
def save_simulation_state(session, world_state: dict):
    """Salva estado completo da simulação no banco."""
    # TODO: Implementar
    # 1. Atualizar todos os agentes
    # 2. Atualizar todos os edifícios
    # 3. Salvar estatísticas econômicas
    # 4. Commit
    pass


def load_simulation_state(session):
    """Carrega estado da simulação do banco."""
    # TODO: Implementar
    # 1. Carregar todos os agentes
    # 2. Carregar todos os edifícios
    # 3. Carregar veículos
    # 4. Retornar estrutura de dados
    pass


# TODO: Configurar Alembic para migrations
"""
# Instalar:
pip install alembic

# Inicializar:
alembic init migrations

# Criar migration:
alembic revision --autogenerate -m "Initial tables"

# Aplicar migration:
alembic upgrade head
"""

# TODO: Adicionar ao requirements.txt
"""
SQLAlchemy>=2.0.0
alembic>=1.12.0
"""

# TODO: Criar testes (tests/unit/test_database.py)
"""
import pytest
from backend.database.models import Agent, Building, init_database, get_session, get_engine

def test_create_agent():
    engine = init_database(":memory:")
    session = get_session(engine)
    
    agent = Agent(name="Test Agent", money=1000.0)
    session.add(agent)
    session.commit()
    
    assert agent.id is not None
    assert agent.name == "Test Agent"
    assert agent.money == 1000.0

def test_create_building():
    # Similar...
    pass
"""

