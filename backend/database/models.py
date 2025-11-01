"""
Modelos de banco de dados para Ferritine.
Implementa persistência usando PostgreSQL e SQLAlchemy.
"""

from sqlalchemy import (
    create_engine, Column, Integer, String, Float, Boolean,
    DateTime, ForeignKey, Text, DECIMAL, CHAR, Enum as SQLEnum,
    JSON, CheckConstraint, TypeDecorator
)
from sqlalchemy.dialects.postgresql import UUID as PG_UUID
from sqlalchemy.ext.declarative import declarative_base
from sqlalchemy.orm import relationship, sessionmaker
from datetime import datetime
from typing import List, Optional
import uuid
import enum

Base = declarative_base()


# Tipo UUID compatível com SQLite e PostgreSQL
class GUID(TypeDecorator):
    """Platform-independent GUID type.

    Uses PostgreSQL's UUID type, otherwise uses
    CHAR(36), storing as stringified hex values.
    """
    impl = CHAR
    cache_ok = True

    def load_dialect_impl(self, dialect):
        if dialect.name == 'postgresql':
            return dialect.type_descriptor(PG_UUID(as_uuid=True))
        else:
            return dialect.type_descriptor(CHAR(36))

    def process_bind_param(self, value, dialect):
        if value is None:
            return value
        elif dialect.name == 'postgresql':
            return value
        else:
            if not isinstance(value, uuid.UUID):
                return str(uuid.UUID(value))
            else:
                return str(value)

    def process_result_value(self, value, dialect):
        if value is None:
            return value
        else:
            if not isinstance(value, uuid.UUID):
                return uuid.UUID(value)
            else:
                return value


# Enums para campos específicos
class CreatedBy(str, enum.Enum):
    """Origem de criação do agente."""
    IA = 'I'  # Criado pela IA
    ADMIN = 'A'  # Criado por administrador
    BIRTH = 'B'  # Nascido na simulação


class HealthStatus(str, enum.Enum):
    """Status de saúde do agente."""
    HEALTHY = 'H'  # Saudável
    SICK = 'S'  # Doente
    CRITICAL = 'C'  # Crítico
    DEAD = 'D'  # Morto


class AgentStatus(str, enum.Enum):
    """Status atual do agente."""
    IDLE = 'idle'  # Parado
    MOVING = 'moving'  # Movendo
    WORKING = 'working'  # Trabalhando
    SLEEPING = 'sleeping'  # Dormindo
    SICK = 'sick'  # Doente
    EATING = 'eating'  # Comendo
    SOCIALIZING = 'socializing'  # Socializando


class Gender(str, enum.Enum):
    """Gênero do agente."""
    MALE = 'M'
    FEMALE = 'F'
    NON_BINARY = 'NB'
    OTHER = 'O'


# Modelo principal: Agent (Agente)
class Agent(Base):
    """
    Modelo de banco de dados para Agentes.
    Representa cidadãos da simulação com atributos complexos.
    """
    __tablename__ = 'agents'

    # Identificação
    id = Column(GUID(), primary_key=True, default=uuid.uuid4)
    name = Column(String(200), nullable=False, index=True)

    # Skills e rotina
    skills = Column(JSON, default=dict, comment="Habilidades do agente em formato JSON")
    routine_id = Column(GUID(), ForeignKey('routines.id'), nullable=True)

    # Metadata de criação
    created_at = Column(DateTime, default=datetime.utcnow, nullable=False)
    created_by = Column(
        SQLEnum(CreatedBy),
        nullable=False,
        default=CreatedBy.IA,
        comment="I=IA, A=Admin, B=Birth"
    )

    # Dados pessoais
    birth_date = Column(DateTime, nullable=False)
    gender = Column(SQLEnum(Gender), nullable=False)

    # Profissão e localização
    profession_id = Column(GUID(), ForeignKey('professions.id'), nullable=True)
    home_building_id = Column(GUID(), ForeignKey('buildings.id'), nullable=True)
    work_building_id = Column(GUID(), ForeignKey('buildings.id'), nullable=True)

    # Localização atual (polimórfica)
    current_location_type = Column(String(50), nullable=True, comment="building, vehicle, street, etc")
    current_location_id = Column(GUID(), nullable=True)
    last_seen_at = Column(DateTime, default=datetime.utcnow, onupdate=datetime.utcnow)

    # Status de saúde
    health_status = Column(
        SQLEnum(HealthStatus),
        default=HealthStatus.HEALTHY,
        comment="H=Healthy, S=Sick, C=Critical, D=Dead"
    )

    # Nível de energia (-100 a 100 para pessoas com deficiência)
    energy_level = Column(
        Integer,
        default=100,
        comment="0-100 (aceita negativos para deficiências como autismo)"
    )
    __table_args__ = (
        CheckConstraint('energy_level >= -100 AND energy_level <= 100', name='check_energy_range'),
    )

    # Humor/Emoção/Sentimento (polimórfico, complexo como The Sims 4)
    mood_data = Column(
        JSON,
        default=dict,
        comment="Sistema complexo de humor que afeta comportamento"
    )

    # Carteira (11 dígitos antes, 2 depois, pode ser negativo até -100k)
    wallet = Column(
        DECIMAL(13, 2),
        default=0.00,
        comment="Formato: 11 dígitos antes da vírgula, 2 depois. Pode ser negativo até -100000"
    )

    # Inventário
    inventory = Column(JSON, default=list, comment="Itens que o agente possui")

    # Status atual
    current_status = Column(SQLEnum(AgentStatus), default=AgentStatus.IDLE)

    # Destino (polimórfico para agentes em trânsito)
    destination_type = Column(String(50), nullable=True)
    destination_id = Column(GUID(), nullable=True)

    # Objetivos (curto/médio/longo prazo, sonhos)
    goals = Column(
        JSON,
        default=dict,
        comment="Objetivos: curto prazo, médio prazo, longo prazo, sonhos"
    )

    # Personalidade (adaptativa)
    personality = Column(
        JSON,
        default=dict,
        comment="Aleatória para IA, adaptativa por genética e traumas"
    )

    # Versão do programa
    version = Column(String(20), nullable=False, comment="Versão do programa quando criado/nasceu")

    # Soft delete
    is_deleted = Column(Boolean, default=False, index=True)

    # Histórico (eventos importantes, família, traumas, etc)
    history = Column(
        JSON,
        default=list,
        comment="Eventos importantes, origem familiar, interesses para biografia"
    )

    # Genética complexa
    genetics = Column(
        JSON,
        default=dict,
        comment="Sistema de genética complexa herdada dos pais"
    )

    # Relacionamentos
    routine = relationship("Routine", back_populates="agents", foreign_keys=[routine_id])
    profession = relationship("Profession", back_populates="agents")
    home = relationship(
        "Building",
        foreign_keys=[home_building_id],
        back_populates="residents"
    )
    workplace = relationship(
        "Building",
        foreign_keys=[work_building_id],
        back_populates="workers"
    )

    # Propriedades calculadas
    @property
    def age(self) -> int:
        """Calcula idade do agente baseado na data de nascimento."""
        delta = datetime.utcnow() - self.birth_date
        return delta.days // 365

    @property
    def full_biography(self) -> str:
        """Gera biografia do agente baseado no histórico e outros dados."""
        # TODO: Implementar geração de biografia
        bio_parts = []
        bio_parts.append(f"{self.name}, {self.age} anos")
        if self.profession:
            bio_parts.append(f"Profissão: {self.profession.name}")
        # Adicionar eventos importantes do histórico
        return ". ".join(bio_parts)

    def __repr__(self):
        return f"<Agent(id={self.id}, name='{self.name}', age={self.age})>"


# Modelo: Routine (Rotina)
class Routine(Base):
    """Rotina diária de agentes."""
    __tablename__ = 'routines'

    id = Column(GUID(), primary_key=True, default=uuid.uuid4)
    name = Column(String(100), nullable=False)
    description = Column(Text)

    # Estrutura da rotina em JSON
    schedule = Column(
        JSON,
        default=list,
        comment="Programação por hora do dia"
    )

    created_at = Column(DateTime, default=datetime.utcnow)
    updated_at = Column(DateTime, default=datetime.utcnow, onupdate=datetime.utcnow)

    # Relacionamentos
    agents = relationship("Agent", back_populates="routine")

    def __repr__(self):
        return f"<Routine(id={self.id}, name='{self.name}')>"


# Modelo: Profession (Profissão)
class Profession(Base):
    """Profissões disponíveis na simulação."""
    __tablename__ = 'professions'

    id = Column(GUID(), primary_key=True, default=uuid.uuid4)
    name = Column(String(100), nullable=False, unique=True)
    description = Column(Text)

    # Atributos econômicos
    base_salary = Column(DECIMAL(13, 2), default=1000.00)
    required_skills = Column(JSON, default=list)

    # Tipo de trabalho
    work_sector = Column(String(50), comment="residential, commercial, industrial, public, etc")

    created_at = Column(DateTime, default=datetime.utcnow)

    # Relacionamentos
    agents = relationship("Agent", back_populates="profession")

    def __repr__(self):
        return f"<Profession(id={self.id}, name='{self.name}')>"


# Modelo: Building (Edifício)
class Building(Base):
    """Edifícios da cidade."""
    __tablename__ = 'buildings'

    id = Column(GUID(), primary_key=True, default=uuid.uuid4)
    name = Column(String(100), nullable=False)
    building_type = Column(
        String(50),
        nullable=False,
        comment="residential, commercial, industrial, public"
    )

    # Localização
    x = Column(Integer, nullable=False)
    y = Column(Integer, nullable=False)
    width = Column(Integer, default=1)
    height = Column(Integer, default=1)

    # Propriedades
    capacity = Column(Integer, default=10)
    current_occupancy = Column(Integer, default=0)

    # Economia
    rent_cost = Column(DECIMAL(13, 2), default=500.00)
    operating_cost = Column(DECIMAL(13, 2), default=100.00)

    # Estado
    is_active = Column(Boolean, default=True)

    # Timestamps
    created_at = Column(DateTime, default=datetime.utcnow)
    updated_at = Column(DateTime, default=datetime.utcnow, onupdate=datetime.utcnow)

    # Relacionamentos
    residents = relationship(
        "Agent",
        foreign_keys=[Agent.home_building_id],
        back_populates="home"
    )
    workers = relationship(
        "Agent",
        foreign_keys=[Agent.work_building_id],
        back_populates="workplace"
    )

    def __repr__(self):
        return f"<Building(id={self.id}, name='{self.name}', type='{self.building_type}')>"


# Modelo: Vehicle (Veículo)
class Vehicle(Base):
    """Veículos de transporte."""
    __tablename__ = 'vehicles'

    id = Column(GUID(), primary_key=True, default=uuid.uuid4)
    name = Column(String(100), nullable=False)
    vehicle_type = Column(String(50), nullable=False, comment="train, bus, car, etc")

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


# Modelo: Event (Evento)
class Event(Base):
    """Eventos da simulação."""
    __tablename__ = 'events'

    id = Column(GUID(), primary_key=True, default=uuid.uuid4)
    event_type = Column(String(50), nullable=False, index=True)
    description = Column(Text)

    # Relacionamentos opcionais
    agent_id = Column(GUID(), ForeignKey('agents.id'), nullable=True)
    building_id = Column(GUID(), ForeignKey('buildings.id'), nullable=True)
    vehicle_id = Column(GUID(), ForeignKey('vehicles.id'), nullable=True)

    # Dados adicionais
    event_data = Column(JSON, default=dict)

    # Timestamp
    occurred_at = Column(DateTime, default=datetime.utcnow, index=True)
    simulation_time = Column(Integer, comment="Hora da simulação (0-23)")

    def __repr__(self):
        return f"<Event(id={self.id}, type='{self.event_type}', time={self.simulation_time})>"


# Modelo: EconomicStat (Estatística Econômica)
class EconomicStat(Base):
    """Estatísticas econômicas da simulação."""
    __tablename__ = 'economic_stats'

    id = Column(GUID(), primary_key=True, default=uuid.uuid4)
    simulation_time = Column(Integer, nullable=False, index=True)

    # Estatísticas agregadas
    total_money_in_circulation = Column(DECIMAL(15, 2), default=0.00)
    average_agent_money = Column(DECIMAL(13, 2), default=0.00)
    total_transactions = Column(Integer, default=0)

    # Por setor
    residential_income = Column(DECIMAL(15, 2), default=0.00)
    commercial_income = Column(DECIMAL(15, 2), default=0.00)
    industrial_income = Column(DECIMAL(15, 2), default=0.00)
    public_spending = Column(DECIMAL(15, 2), default=0.00)

    # Timestamp
    recorded_at = Column(DateTime, default=datetime.utcnow)

    def __repr__(self):
        return f"<EconomicStat(time={self.simulation_time}, circulation={self.total_money_in_circulation})>"


# Modelo: NamePool (Pool de nomes para geração aleatória)
class NamePool(Base):
    """Pool de nomes para geração aleatória de agentes."""
    __tablename__ = 'name_pool'

    id = Column(GUID(), primary_key=True, default=uuid.uuid4)
    name = Column(String(100), nullable=False)
    name_type = Column(String(20), nullable=False, comment="first, middle, last")
    gender = Column(SQLEnum(Gender), nullable=True, comment="Alguns nomes são específicos de gênero")
    rarity = Column(Float, default=1.0, comment="0.0 (muito raro) a 1.0 (muito comum)")
    origin = Column(String(50), comment="Origem cultural do nome")

    def __repr__(self):
        return f"<NamePool(name='{self.name}', type='{self.name_type}', rarity={self.rarity})>"


