"""
Modelos de banco de dados para Ferritine.
Implementa persistência usando PostgreSQL e SQLAlchemy.
"""

from sqlalchemy import (
    create_engine, Column, Integer, String, Float, Boolean,
    DateTime, ForeignKey, Text, DECIMAL, CHAR, Enum as SQLEnum,
    JSON, CheckConstraint, Index, TypeDecorator, Time
)
from sqlalchemy.dialects.postgresql import UUID as PG_UUID
from sqlalchemy.ext.declarative import declarative_base
from sqlalchemy.orm import relationship, sessionmaker, validates
from datetime import datetime, timedelta, time
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
    """Status atual do agente.

    Lista expandida para suportar uma simulação complexa com estados de
    mobilidade, trabalho, social, manutenção, emergência e lifecycle.

    Exemplos de uso na simulação:
    - `CONSTRUCTING` para agentes que estão construindo/participando da construção
    - `WAITING` / `QUEUED` para agentes em filas ou aguardando um evento
    - `RESPONDING_EMERGENCY` para serviços de emergência ou agentes que reagem
    - `AT_HOME` / `AT_WORK` para estados estáticos de localização/ocupação
    - `TRAVELING` / `COMMUTING` para deslocamentos de longa duração
    """

    # Estados básicos / mobilidade
    IDLE = 'idle'                 # Parado, sem tarefa ativa
    MOVING = 'moving'             # Movendo-se (curto percurso)
    TRAVELING = 'traveling'       # Em viagem (deslocamento mais longo)
    COMMUTING = 'commuting'       # Deslocando-se entre trabalho/casa (pendular)

    # Localização ocupacional
    AT_HOME = 'at_home'           # Em casa
    AT_WORK = 'at_work'           # No local de trabalho

    # Trabalho / atividades produtivas
    WORKING = 'working'           # Trabalhando em sua profissão
    CONSTRUCTING = 'constructing' # Construindo/obras/obras civis
    MAINTENANCE = 'maintenance'   # Manutenção preventiva
    REPAIRING = 'repairing'       # Reparo de objetos/infraestrutura
    HARVESTING = 'harvesting'     # Colhendo recursos (para jogos/sims rurais)
    FARMING = 'farming'           # Agricultura / plantio
    STUDYING = 'studying'         # Estudando / aprendendo

    # Filas / espera / sincronização
    WAITING = 'waiting'           # Aguardando por algo (transporte, atendimento)
    QUEUED = 'queued'             # Em fila especificamente
    IDLE_WAIT = 'idle_wait'       # Parado porque espera para sincronizar

    # Social / lazer / consumição
    EATING = 'eating'             # Comendo
    SLEEPING = 'sleeping'         # Dormindo
    RESTING = 'resting'           # Descansando (não dormindo)
    SOCIALIZING = 'socializing'   # Socializando
    SHOPPING = 'shopping'         # Comprando
    ENTERTAINING = 'entertaining' # Assistindo/participando de entretenimento
    ATTENDING_EVENT = 'attending_event'  # Em evento público/privado
    PARENTING = 'parenting'       # Cuidando de filhos
    CARING = 'caring'             # Cuidados (idosos, doentes)

    # Saúde / emergência
    SICK = 'sick'                 # Doente (comportamento reduzido)
    SEEKING_MEDICAL = 'seeking_medical'  # Procura atendimento médico
    RESPONDING_EMERGENCY = 'responding_emergency'  # Respondendo a emergência
    FLEEING = 'fleeing'           # Fugindo de perigo
    SEEKING_SHELTER = 'seeking_shelter'  # Buscando abrigo

    # Eventos de infraestrutura / backend
    IN_MEETING = 'in_meeting'     # Reunião / compromisso
    PERFORMING_JOB_TASK = 'performing_job_task'  # Tarefa específica do trabalho
    IDLE_TASK = 'idle_task'       # Aguardando próxima tarefa designada

    # Estados transitórios e utilitários
    QUEUED_FOR_RESOURCE = 'queued_for_resource'  # Em fila por recurso (p.ex. energia/água)
    CHARGING = 'charging'         # Recarregando (bateria / veículo)
    RECHARGING = 'recharging'     # Sinônimo alternativo (se necessário)


# Enumeração de Gênero
class Gender(str, enum.Enum):
    """Gênero do agente.

    Lista extensa (não exaustiva) de identidades de gênero, com códigos curtos
    para armazenamento. Comentários explicam nuances: cis/trans, não-binário como
    guarda-chuva, inclusão de intersexo e estados de questionamento/inscerteza.
    """

    CIS_MALE = 'CM'            # Cisgênero homem
    CIS_FEMALE = 'CF'          # Cisgênero mulher

    TRANS_MALE = 'TM'          # Homem trans / trans man
    TRANS_FEMALE = 'TF'        # Mulher trans / trans woman
    TRANS_NONBINARY = 'TN'     # Pessoa trans não-binária

    NON_BINARY = 'NB'          # Guarda-chuva: não-binário
    GENDERQUEER = 'GQ'         # Genderqueer
    GENDERFLUID = 'GF'         # Genderfluid
    AGENDER = 'AG'             # Sem gênero
    BIGENDER = 'BG'            # Dois gêneros
    DEMIBOY = 'DB'             # demiboy
    DEMIGIRL = 'DG'           # demigirl
    ANDROGYNE = 'AN'           # Andrógeno
    PANGENDER = 'PG'           # Multigênero / pangênero
    TWO_SPIRIT = 'TS'          # Conceito indígena norte-americano (two-spirit)
    TRAVESTI = 'TR'        # travesti / reconhecido culturalmente

    INTERSEX = 'IS'            # Intersexo (variação biológica do sexo)

    QUESTIONING = 'Q'          # Questionando / em dúvida
    UNSURE = 'U'               # Não sabe / incerto


# Estimativas aproximadas de prevalência global (valores de exemplo, em porcentagem).
# Fonte: compilações públicas e estudos variados; valores devem ser ajustados conforme a população alvo.
GENDER_PREVALENCE = {
    Gender.CIS_MALE: 49.0,          # Aproximadamente metade da população
    Gender.CIS_FEMALE: 49.0,

    # Trans (estimativas conservadoras; variações grandes entre estudos):
    Gender.TRANS_MALE: 0.2,
    Gender.TRANS_FEMALE: 0.2,
    Gender.TRANS_NONBINARY: 0.1,

    # Não-binário e variações (estimativas para uso em simulação — podem ser maiores em populações jovens):
    Gender.NON_BINARY: 0.5,
    Gender.GENDERQUEER: 0.1,
    Gender.GENDERFLUID: 0.1,
    Gender.AGENDER: 0.05,
    Gender.BIGENDER: 0.03,
    Gender.DEMIBOY: 0.02,
    Gender.DEMIGIRL: 0.02,
    Gender.ANDROGYNE: 0.01,
    Gender.PANGENDER: 0.01,
    Gender.TWO_SPIRIT: 0.01,
    Gender.TRAVESTI: 0.01,

    # Intersexo (estimativas amplas; inclui várias condições):
    Gender.INTERSEX: 1.7,

    # Questionando / incerto (mais comum entre jovens; valor ilustrativo):
    Gender.QUESTIONING: 0.5,
    Gender.UNSURE: 0.5,
}


def get_gender_prevalence(gender: "Gender") -> float:
    """Retorna a estimativa de prevalência (porcentagem) para um gênero dado.

    Se o gênero não estiver mapeado, retorna 0.0. Use isto apenas como heurística para geração.
    """
    return GENDER_PREVALENCE.get(gender, 0.0)


# Enums para Ticket (definidos antes de Agent que os usa)
class TicketStatus(str, enum.Enum):
    """Status do bilhete de transporte."""
    ACTIVE = 'active'           # Bilhete válido e ativo
    USED = 'used'               # Já utilizado
    EXPIRED = 'expired'         # Expirado
    CANCELLED = 'cancelled'     # Cancelado


class TicketType(str, enum.Enum):
    """Tipo de bilhete."""
    SINGLE = 'single'           # Passagem única
    RETURN = 'return'           # Ida e volta
    DAY_PASS = 'day_pass'       # Passe diário
    WEEK_PASS = 'week_pass'     # Passe semanal
    MONTH_PASS = 'month_pass'   # Passe mensal
    TRANSFER = 'transfer'       # Transferência/integração


class StationType(str, enum.Enum):
    """Tipos de estações de transporte por era histórica e tecnologia"""

    # ==================== ERA 1: VAPOR (1860-1920) ====================
    TRAIN_STEAM = "train_steam"  # Estação de maria fumaça
    TRAM_HORSE = "tram_horse"  # Bonde puxado a cavalo

    # ==================== ERA 2: INDUSTRIALIZAÇÃO (1920-1960) ====================
    TRAIN_DIESEL = "train_diesel"  # Estação diesel
    TRAIN_ELECTRIC = "train_electric"  # Estação elétrica
    BUS_DIESEL = "bus_diesel"  # Terminal de ônibus a diesel
    TRAM_ELECTRIC = "tram_electric"  # Bonde elétrico
    TROLLEYBUS = "trolleybus"  # Ônibus elétrico (trólebus)

    # ==================== ERA 3: MODERNIZAÇÃO (1960-2000) ====================
    SUBWAY = "subway"  # Estação de metrô
    BRT = "brt"  # Bus Rapid Transit
    MONORAIL = "monorail"  # Monotrilho
    CABLE_CAR = "cable_car"  # Teleférico urbano

    # ==================== ERA 4: CONTEMPORÂNEO (2000+) ====================
    TRAIN_HIGH_SPEED = "train_high_speed"  # Trem de alta velocidade
    LIGHT_RAIL = "light_rail"  # VLT (Veículo Leve sobre Trilhos)
    BUS_ELECTRIC = "bus_electric"  # Ônibus elétrico moderno
    BUS_HYDROGEN = "bus_hydrogen"  # Ônibus a hidrogênio
    MAGLEV = "maglev"  # Trem de levitação magnética
    AUTONOMOUS_SHUTTLE = "autonomous_shuttle"  # Shuttle autônomo

    # ==================== FUTURO/ESPECIAL ====================
    HYPERLOOP = "hyperloop"  # Hyperloop (futurista)
    AERIAL_TRAM = "aerial_tram"  # Teleférico aéreo
    WATER_TAXI = "water_taxi"  # Táxi aquático
    FERRY_ELECTRIC = "ferry_electric"  # Balsa elétrica

    # ==================== MULTIMODAL ====================
    MIXED_TRAIN_BUS = "mixed_train_bus"  # Estação trem + ônibus
    MIXED_SUBWAY_BUS = "mixed_subway_bus"  # Estação metrô + ônibus
    MIXED_FULL = "mixed_full"  # Integração completa
    TRANSPORT_HUB = "transport_hub"  # Hub central multimodal

    # ==================== CARGA ====================
    CARGO_TRAIN = "cargo_train"  # Terminal de carga ferroviária
    CARGO_PORT = "cargo_port"  # Porto de carga
    LOGISTICS_CENTER = "logistics_center"  # Centro logístico


class RouteFrequencyPattern(str, enum.Enum):
    """Padrões de frequência de rotas"""
    CONSTANT = "constant"  # Mesma frequência o dia to do
    PEAK_HOURS = "peak_hours"  # Mais frequente em horários de pico
    RUSH_HOUR = "rush_hour"  # Concentrado em rush (manhã/tarde)
    WEEKEND = "weekend"  # Padrão de fim de semana
    NIGHT_SERVICE = "night_service"  # Serviço noturno reduzido
    SEASONAL = "seasonal"  # Varia por estação do ano
    EVENT_BASED = "event_based"  # Baseado em eventos especiais
    ON_DEMAND = "on_demand"  # Sob demanda (futuro)


class RoutePriority(str, enum.Enum):
    """Prioridade da rota no sistema"""
    CRITICAL = "critical"  # Linha crítica (não pode parar)
    HIGH = "high"  # Alta prioridade
    MEDIUM = "medium"  # Prioridade média
    LOW = "low"  # Baixa prioridade
    TOURIST = "tourist"  # Turística (pode ser desativada)
    EXPERIMENTAL = "experimental"  # Experimental (teste)


class RouteStatus(str, enum.Enum):
    """Status operacional da rota"""
    ACTIVE = "active"  # Operando normalmente
    DELAYED = "delayed"  # Atrasada
    SUSPENDED = "suspended"  # Suspensa temporariamente
    MAINTENANCE = "maintenance"  # Manutenção programada
    EMERGENCY_STOP = "emergency_stop"  # Parada emergencial
    STRIKE = "strike"  # Em greve
    WEATHER_SUSPENDED = "weather_suspended"  # Suspensa por clima
    ACCIDENT = "accident"  # Acidente na linha
    OVERCROWDED = "overcrowded"  # Superlotada
    UNDER_CONSTRUCTION = "under_construction"  # Em construção
    DEACTIVATED = "deactivated"  # Desativada permanentemente


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
    skills = Column(JSON, default=lambda: {}, comment="Habilidades do agente em formato JSON")
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

    # Transporte público
    waiting_at_station_id = Column(GUID(), ForeignKey('stations.id'), nullable=True,
                                   comment="Estação onde o agente está aguardando")
    current_ticket_id = Column(GUID(), ForeignKey('tickets.id'), nullable=True,
                              comment="Bilhete de transporte atual")

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
        default=lambda: {},
        comment="Sistema complexo de humor que afeta comportamento"
    )

    # Carteira (11 dígitos antes, 2 depois, pode ser negativo até -100k)
    wallet = Column(
        DECIMAL(13, 2),
        default=0.00,
        comment="Formato: 11 dígitos antes da vírgula, 2 depois. Pode ser negativo até -100000"
    )

    # Inventário
    inventory = Column(JSON, default=lambda: [], comment="Itens que o agente possui")

    # Status atual
    current_status = Column(SQLEnum(AgentStatus), default=AgentStatus.IDLE)

    # Destino (polimórfico para agentes em trânsito)
    destination_type = Column(String(50), nullable=True)
    destination_id = Column(GUID(), nullable=True)

    # Objetivos (curto/médio/longo prazo, sonhos)
    goals = Column(
        JSON,
        default=lambda: {},
        comment="Objetivos: curto prazo, médio prazo, longo prazo, sonhos"
    )

    # Personalidade (adaptativa)
    personality = Column(
        JSON,
        default=lambda: {},
        comment="Aleatória para IA, adaptativa por genética e traumas"
    )

    # Versão do programa
    version = Column(String(20), nullable=False, comment="Versão do programa quando criado/nasceu")

    # Soft delete
    # TODO: Implementar lógica de soft delete para agentes mortos.
    # Recomendação/descrição:
    # - Quando um agente tiver `health_status == HealthStatus.DEAD`, marcar `is_deleted = True`
    #   em vez de removê-lo fisicamente do banco. Isso mantém histórico e relações.
    # - Adicionar um campo `deleted_at = Column(DateTime, nullable=True)` para registrar quando
    #   ocorreu o soft-delete (opcional, útil para auditoria e políticas de retenção).
    # - Garantir que todas as consultas de leitura (queries) ignorem por padrão agentes com
    #   `is_deleted == True`, por exemplo adicionando filtros no nível de query/ORM ou usando
    #   um mixin/QueryBase que aplique o filtro automaticamente.
    # - Decidir políticas adicionais: cascata lógica para relacionamentos, anonimização de dados
    #   pessoais ao marcar como deletado, gravação em tabela de arquivo (archive) se necessário.
    # - Fornecer funções utilitárias: `soft_delete()`, `restore()`, e `hard_delete()` (irreversível).
    # - Garantir testes que validem transição de estado (alive -> dead -> is_deleted) e que
    #   agentes deletados não apareçam em listas ativas.
    is_deleted = Column(Boolean, default=False, index=True)

    # Histórico (eventos importantes, família, traumas, etc)
    history = Column(
        JSON,
        default=lambda: [],
        comment="Eventos importantes, origem familiar, interesses para biografia"
    )

    # Genética complexa
    genetics = Column(
        JSON,
        default=lambda: {},
        comment="Sistema de genética complexa herdada dos pais"
    )

    # Relacionamentos
    routine = relationship("Routine", back_populates="agents", foreign_keys=[routine_id])
    profession = relationship("Profession", back_populates="agents")

    # Edifícios
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

    # Transporte público
    waiting_at_station = relationship("Station", foreign_keys=[waiting_at_station_id])
    current_ticket = relationship("Ticket", foreign_keys=[current_ticket_id])
    tickets = relationship("Ticket", foreign_keys='Ticket.agent_id', back_populates="agent")
    owned_buildings = relationship(
        "Building",
        foreign_keys="Building.owner_id",
        back_populates="owner"
    )

    # Veículos
    driven_vehicles = relationship(
        "Vehicle",
        foreign_keys="Vehicle.current_driver_id",
        back_populates="current_driver"
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

    # Métodos de transporte público
    def purchase_ticket(self, route_id: uuid.UUID = None,
                       origin_id: uuid.UUID = None,
                       destination_id: uuid.UUID = None,
                       ticket_type: TicketType = TicketType.SINGLE,
                       price: DECIMAL = DECIMAL('3.50')) -> 'Ticket':
        """Compra um bilhete de transporte.

        Args:
            route_id: ID da rota (opcional)
            origin_id: ID da estação de origem (opcional)
            destination_id: ID da estação de destino (opcional)
            ticket_type: Tipo de bilhete
            price: Preço do bilhete

        Returns:
            Ticket criado

        Raises:
            ValueError: Se o agente não tem dinheiro suficiente
        """
        # Verificar se tem dinheiro
        if self.wallet < price:
            raise ValueError(f"Saldo insuficiente. Necessário: {price}, Disponível: {self.wallet}")

        # Descontar do saldo
        self.wallet -= price

        # Criar bilhete
        ticket = Ticket(
            agent_id=self.id,
            ticket_type=ticket_type,
            route_id=route_id,
            origin_station_id=origin_id,
            destination_station_id=destination_id,
            price=price,
            status=TicketStatus.ACTIVE
        )

        # Definir validade baseado no tipo
        from datetime import timedelta
        now = datetime.utcnow()
        ticket.valid_from = now

        if ticket_type == TicketType.SINGLE:
            ticket.valid_until = now + timedelta(hours=2)
            ticket.max_validations = 1
        elif ticket_type == TicketType.RETURN:
            ticket.valid_until = now + timedelta(days=1)
            ticket.max_validations = 2
        elif ticket_type == TicketType.DAY_PASS:
            ticket.valid_until = now + timedelta(days=1)
            ticket.max_validations = 999
        elif ticket_type == TicketType.WEEK_PASS:
            ticket.valid_until = now + timedelta(days=7)
            ticket.max_validations = 999
        elif ticket_type == TicketType.MONTH_PASS:
            ticket.valid_until = now + timedelta(days=30)
            ticket.max_validations = 999
        elif ticket_type == TicketType.TRANSFER:
            ticket.valid_until = now + timedelta(minutes=30)
            ticket.max_validations = 1

        # Nota: current_ticket_id deve ser definido após o ticket ser commitado
        # pois ticket.id só estará disponível após o flush/commit

        return ticket

    def wait_at_station(self, station_id: uuid.UUID):
        """Faz o agente aguardar em uma estação.

        Args:
            station_id: ID da estação
        """
        self.waiting_at_station_id = station_id
        self.current_location_type = 'station'
        self.current_location_id = station_id

    def leave_station(self):
        """Faz o agente sair da estação."""
        self.waiting_at_station_id = None

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
        default=lambda: [],
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
    required_skills = Column(JSON, default=lambda: [])

    # Tipo de trabalho
    work_sector = Column(String(50), comment="residential, commercial, industrial, public, etc")

    created_at = Column(DateTime, default=datetime.utcnow)

    # Relacionamentos
    agents = relationship("Agent", back_populates="profession")

    def __repr__(self):
        return f"<Profession(id={self.id}, name='{self.name}')>"


# ENUMS PARA BUILDING (EDIFÍCIOS)
class BuildingType(str, enum.Enum):
    """Tipos detalhados de edifícios na cidade"""

    # RESIDENCIAIS
    RESIDENTIAL_HOUSE_SMALL = "residential_house_small"          # Casa térrea simples
    RESIDENTIAL_HOUSE_MEDIUM = "residential_house_medium"        # Casa de 2 andares
    RESIDENTIAL_HOUSE_LARGE = "residential_house_large"          # Mansão, sobrado
    RESIDENTIAL_APARTMENT_LOW = "residential_apartment_low"      # Prédio 2-3 andares
    RESIDENTIAL_APARTMENT_MID = "residential_apartment_mid"      # Prédio 4-8 andares
    RESIDENTIAL_APARTMENT_HIGH = "residential_apartment_high"    # Arranha-céu residencial
    RESIDENTIAL_TENEMENT = "residential_tenement"                # Cortiço (histórico)
    RESIDENTIAL_VILLA = "residential_villa"                      # Casa de campo luxuosa
    RESIDENTIAL_CONDOMINIUM = "residential_condominium"          # Condomínio fechado
    RESIDENTIAL_MOBILE_HOME = "residential_mobile_home"          # Trailer park (futuro)

    # COMERCIAIS
    COMMERCIAL_STORE_SMALL = "commercial_store_small"            # Loja de bairro
    COMMERCIAL_STORE_MEDIUM = "commercial_store_medium"          # Loja especializada
    COMMERCIAL_SUPERMARKET = "commercial_supermarket"            # Supermercado
    COMMERCIAL_MALL = "commercial_mall"                          # Shopping center
    COMMERCIAL_MARKET = "commercial_market"                      # Mercado municipal
    COMMERCIAL_RESTAURANT = "commercial_restaurant"              # Restaurante
    COMMERCIAL_CAFE = "commercial_cafe"                          # Café, padaria
    COMMERCIAL_BAR = "commercial_bar"                            # Bar, pub
    COMMERCIAL_HOTEL_SMALL = "commercial_hotel_small"            # Pousada
    COMMERCIAL_HOTEL_LARGE = "commercial_hotel_large"            # Hotel de luxo
    COMMERCIAL_BANK = "commercial_bank"                          # Banco
    COMMERCIAL_OFFICE_SMALL = "commercial_office_small"          # Escritório pequeno
    COMMERCIAL_OFFICE_TOWER = "commercial_office_tower"          # Torre comercial
    COMMERCIAL_GAS_STATION = "commercial_gas_station"            # Posto de gasolina
    COMMERCIAL_CAR_DEALER = "commercial_car_dealer"              # Concessionária
    COMMERCIAL_PHARMACY = "commercial_pharmacy"                  # Farmácia
    COMMERCIAL_BOOKSTORE = "commercial_bookstore"                # Livraria
    COMMERCIAL_BARBERSHOP = "commercial_barbershop"              # Barbearia, salão

    # INDUSTRIAIS
    INDUSTRIAL_FACTORY_TEXTILE = "industrial_factory_textile"    # Fábrica têxtil
    INDUSTRIAL_FACTORY_METAL = "industrial_factory_metal"        # Metalúrgica
    INDUSTRIAL_FACTORY_FOOD = "industrial_factory_food"          # Processamento de alimentos
    INDUSTRIAL_FACTORY_ELECTRONICS = "industrial_factory_electronics"  # Eletrônicos
    INDUSTRIAL_FACTORY_FURNITURE = "industrial_factory_furniture"      # Móveis
    INDUSTRIAL_WAREHOUSE = "industrial_warehouse"                # Armazém
    INDUSTRIAL_BREWERY = "industrial_brewery"                    # Cervejaria
    INDUSTRIAL_MILL = "industrial_mill"                          # Moinho (histórico)
    INDUSTRIAL_SAWMILL = "industrial_sawmill"                    # Serraria
    INDUSTRIAL_POWER_PLANT_COAL = "industrial_power_plant_coal"  # Usina a carvão
    INDUSTRIAL_POWER_PLANT_HYDRO = "industrial_power_plant_hydro"  # Hidrelétrica
    INDUSTRIAL_POWER_PLANT_SOLAR = "industrial_power_plant_solar"  # Solar (futuro)
    INDUSTRIAL_WATER_TREATMENT = "industrial_water_treatment"    # Estação de tratamento
    INDUSTRIAL_RECYCLING = "industrial_recycling"                # Centro de reciclagem
    INDUSTRIAL_CONSTRUCTION_YARD = "industrial_construction_yard"  # Depósito de construção

    # PÚBLICOS
    PUBLIC_SCHOOL_ELEMENTARY = "public_school_elementary"        # Escola fundamental
    PUBLIC_SCHOOL_HIGH = "public_school_high"                    # Colégio
    PUBLIC_UNIVERSITY = "public_university"                      # Universidade
    PUBLIC_LIBRARY = "public_library"                            # Biblioteca
    PUBLIC_HOSPITAL_SMALL = "public_hospital_small"              # Posto de saúde
    PUBLIC_HOSPITAL_LARGE = "public_hospital_large"              # Hospital geral
    PUBLIC_CLINIC = "public_clinic"                              # Clínica
    PUBLIC_CITY_HALL = "public_city_hall"                        # Prefeitura
    PUBLIC_COURTHOUSE = "public_courthouse"                      # Fórum
    PUBLIC_FIRE_STATION = "public_fire_station"                  # Corpo de bombeiros
    PUBLIC_POST_OFFICE = "public_post_office"                    # Correios
    PUBLIC_CEMETERY = "public_cemetery"                          # Cemitério
    PUBLIC_MONUMENT = "public_monument"                          # Monumento, estátua
    PUBLIC_MUSEUM = "public_museum"                              # Museu
    PUBLIC_ARCHIVE = "public_archive"                            # Arquivo histórico

    # TRANSPORTE
    TRANSPORT_TRAIN_STATION_SMALL = "transport_train_station_small"    # Estação de bairro
    TRANSPORT_TRAIN_STATION_CENTRAL = "transport_train_station_central"  # Estação central
    TRANSPORT_BUS_TERMINAL = "transport_bus_terminal"            # Terminal de ônibus
    TRANSPORT_BUS_STOP = "transport_bus_stop"                    # Ponto de ônibus
    TRANSPORT_TAXI_STAND = "transport_taxi_stand"                # Ponto de táxi
    TRANSPORT_PARKING_LOT = "transport_parking_lot"              # Estacionamento
    TRANSPORT_GARAGE = "transport_garage"                        # Garagem pública
    TRANSPORT_TRAIN_DEPOT = "transport_train_depot"              # Depósito de trens
    TRANSPORT_BUS_DEPOT = "transport_bus_depot"                  # Garagem de ônibus
    TRANSPORT_SIGNAL_BOX = "transport_signal_box"                # Cabine de sinalização
    TRANSPORT_BRIDGE = "transport_bridge"                        # Ponte
    TRANSPORT_TUNNEL = "transport_tunnel"                        # Túnel
    TRANSPORT_PORT = "transport_port"                            # Porto fluvial/marítimo

    # LAZER
    LEISURE_PARK_SMALL = "leisure_park_small"                    # Praça
    LEISURE_PARK_LARGE = "leisure_park_large"                    # Parque municipal
    LEISURE_PLAYGROUND = "leisure_playground"                    # Parquinho infantil
    LEISURE_SPORTS_FIELD = "leisure_sports_field"                # Campo de futebol
    LEISURE_STADIUM = "leisure_stadium"                          # Estádio
    LEISURE_GYM = "leisure_gym"                                  # Academia
    LEISURE_POOL = "leisure_pool"                                # Piscina pública
    LEISURE_THEATER = "leisure_theater"                          # Teatro
    LEISURE_CINEMA = "leisure_cinema"                            # Cinema
    LEISURE_NIGHTCLUB = "leisure_nightclub"                      # Boate
    LEISURE_CASINO = "leisure_casino"                            # Cassino (histórico)
    LEISURE_BOTANICAL_GARDEN = "leisure_botanical_garden"        # Jardim botânico
    LEISURE_AMUSEMENT_PARK = "leisure_amusement_park"            # Parque de diversões
    LEISURE_LAKE = "leisure_lake"                                # Lago recreativo
    LEISURE_BEACH = "leisure_beach"                              # Praia artificial

    # INFRAESTRUTURA
    INFRASTRUCTURE_POWER_SUBSTATION = "infrastructure_power_substation"  # Subestação
    INFRASTRUCTURE_WATER_TOWER = "infrastructure_water_tower"    # Caixa d'água
    INFRASTRUCTURE_WATER_PUMP = "infrastructure_water_pump"      # Bomba de água
    INFRASTRUCTURE_SEWAGE = "infrastructure_sewage"              # Estação de esgoto
    INFRASTRUCTURE_TELECOM_TOWER = "infrastructure_telecom_tower"  # Torre de telefonia
    INFRASTRUCTURE_RADIO_TOWER = "infrastructure_radio_tower"    # Torre de rádio
    INFRASTRUCTURE_CELL_TOWER = "infrastructure_cell_tower"      # Antena celular (futuro)
    INFRASTRUCTURE_STREET_LIGHT = "infrastructure_street_light"  # Poste de luz
    INFRASTRUCTURE_TRAFFIC_LIGHT = "infrastructure_traffic_light"  # Semáforo
    INFRASTRUCTURE_BILLBOARD = "infrastructure_billboard"        # Outdoor
    INFRASTRUCTURE_BRIDGE_PEDESTRIAN = "infrastructure_bridge_pedestrian"  # Passarela

    # ESPECIAIS
    SPECIAL_RUINS = "special_ruins"                              # Ruínas (pós-demolição)
    SPECIAL_CONSTRUCTION_SITE = "special_construction_site"      # Canteiro de obras
    SPECIAL_EMPTY_LOT = "special_empty_lot"                      # Terreno vazio
    SPECIAL_FARM = "special_farm"                                # Fazenda (rural)
    SPECIAL_LANDMARK = "special_landmark"                        # Marco histórico único
    SPECIAL_MILITARY_BASE = "special_military_base"              # Base militar (histórico)
    SPECIAL_LIGHTHOUSE = "special_lighthouse"                    # Farol (se houver costa)
    SPECIAL_DAM = "special_dam"                                  # Barragem


class BuildingStatus(str, enum.Enum):
    """Status detalhado de construção e operação"""

    # PLANEJAMENTO
    PLANNING_PROPOSED = "planning_proposed"                      # Apenas proposta
    PLANNING_APPROVED = "planning_approved"                      # Aprovada, aguardando fundos
    PLANNING_FUNDED = "planning_funded"                          # Financiada, aguarda início

    # CONSTRUÇÃO
    CONSTRUCTION_FOUNDATION = "construction_foundation"          # Fundação (0-25%)
    CONSTRUCTION_STRUCTURE = "construction_structure"            # Estrutura (25-50%)
    CONSTRUCTION_WALLS = "construction_walls"                    # Paredes (50-75%)
    CONSTRUCTION_FINISHING = "construction_finishing"            # Acabamento (75-99%)
    CONSTRUCTION_PAUSED = "construction_paused"                  # Obra paralisada
    CONSTRUCTION_DELAYED = "construction_delayed"                # Atraso (falta material/verba)

    # OPERAÇÃO
    OPERATIONAL_NEW = "operational_new"                          # Recém-inaugurado
    OPERATIONAL_ACTIVE = "operational_active"                    # Funcionando normalmente
    OPERATIONAL_BUSY = "operational_busy"                        # Lotado (100%+ capacidade)
    OPERATIONAL_SLOW = "operational_slow"                        # Movimento fraco (<30%)
    OPERATIONAL_CLOSED_TEMPORARY = "operational_closed_temporary"  # Fechado temporariamente
    OPERATIONAL_NIGHT_SHIFT = "operational_night_shift"          # Operação noturna apenas
    OPERATIONAL_SEASONAL = "operational_seasonal"                # Sazonal (verão/inverno)

    # MANUTENÇÃO
    MAINTENANCE_ROUTINE = "maintenance_routine"                  # Manutenção preventiva
    MAINTENANCE_EMERGENCY = "maintenance_emergency"              # Reparo urgente
    MAINTENANCE_RENOVATION = "maintenance_renovation"            # Reforma
    MAINTENANCE_EXPANSION = "maintenance_expansion"              # Ampliação
    MAINTENANCE_MODERNIZATION = "maintenance_modernization"      # Modernização

    # PROBLEMAS
    DAMAGED_MINOR = "damaged_minor"                              # Danos leves (>80% funcional)
    DAMAGED_MODERATE = "damaged_moderate"                        # Danos moderados (40-80%)
    DAMAGED_SEVERE = "damaged_severe"                            # Danos severos (<40%)
    DAMAGED_STRUCTURAL = "damaged_structural"                    # Risco estrutural
    DAMAGED_FIRE = "damaged_fire"                                # Pós-incêndio
    DAMAGED_FLOOD = "damaged_flood"                              # Pós-enchente
    DAMAGED_EARTHQUAKE = "damaged_earthquake"                    # Pós-terremoto

    # DESATIVADO
    ABANDONED_RECENT = "abandoned_recent"                        # Abandonado há <1 ano
    ABANDONED_OLD = "abandoned_old"                              # Abandonado há 1-5 anos
    ABANDONED_RUIN = "abandoned_ruin"                            # Ruína (>5 anos)
    CONDEMNED = "condemned"                                      # Interditado (perigo)
    HISTORICAL_PRESERVATION = "historical_preservation"          # Preservação histórica

    # DEMOLIÇÃO
    DEMOLITION_SCHEDULED = "demolition_scheduled"                # Agendada para demolir
    DEMOLITION_IN_PROGRESS = "demolition_in_progress"            # Sendo demolido
    DEMOLISHED = "demolished"                                    # Demolido (terreno vazio)

    # EVENTOS ESPECIAIS
    EVENT_HOSTING = "event_hosting"                              # Sediando evento
    QUARANTINED = "quarantined"                                  # Quarentena (epidemia)
    SEIZED = "seized"                                            # Apreendido (governo/dívida)
    STRIKE = "strike"                                            # Em greve (parado)


class BuildingCondition(str, enum.Enum):
    """Condição física do edifício (0-100 internamente, mas categórico aqui)"""
    EXCELLENT = "excellent"      # 90-100: Novo ou recém-reformado
    GOOD = "good"                # 70-89: Bem mantido
    FAIR = "fair"                # 50-69: Desgaste visível, funcional
    POOR = "poor"                # 30-49: Precisa de reparos urgentes
    VERY_POOR = "very_poor"      # 10-29: Quase inabitável
    RUINOUS = "ruinous"          # 0-9: Ruínas


class BuildingArchitectureStyle(str, enum.Enum):
    """Estilos arquitetônicos por era"""
    # Era 1 (1850-1900)
    COLONIAL_PORTUGUESE = "colonial_portuguese"
    VICTORIAN = "victorian"
    NEOCLASSICAL = "neoclassical"
    ECLETIC = "ecletic"

    # Era 2 (1900-1920)
    ART_DECO = "art_deco"
    MODERNIST = "modernist"
    BAUHAUS = "bauhaus"
    INDUSTRIAL = "industrial"

    # Era 3 (1920-1960)
    BRUTALIST = "brutalist"
    POSTMODERN = "postmodern"
    HIGH_TECH = "high_tech"

    # Era 4 (2000+)
    CONTEMPORARY = "contemporary"
    ECO_SUSTAINABLE = "eco_sustainable"
    MINIMALIST = "minimalist"
    PARAMETRIC = "parametric"

    # Universal
    VERNACULAR = "vernacular"    # Arquitetura popular tradicional
    GENERIC = "generic"          # Sem estilo específico


class BuildingOwnershipType(str, enum.Enum):
    """Tipo de propriedade"""
    PRIVATE_INDIVIDUAL = "private_individual"        # Pessoa física
    PRIVATE_COMPANY = "private_company"              # Empresa privada
    PRIVATE_COOPERATIVE = "private_cooperative"      # Cooperativa
    PUBLIC_MUNICIPAL = "public_municipal"            # Prefeitura
    PUBLIC_STATE = "public_state"                    # Governo estadual
    PUBLIC_FEDERAL = "public_federal"                # Governo federal
    RELIGIOUS = "religious"                          # Instituição religiosa
    NGO = "ngo"                                      # ONG
    FOREIGN = "foreign"                              # Estrangeiro
    ABANDONED_UNCLAIMED = "abandoned_unclaimed"      # Sem dono (abandonado)


class BuildingZoning(str, enum.Enum):
    """Zoneamento urbano (onde pode ser construído)"""
    RESIDENTIAL_LOW_DENSITY = "residential_low_density"      # R1: casas térreas
    RESIDENTIAL_MEDIUM_DENSITY = "residential_medium_density"  # R2: até 4 andares
    RESIDENTIAL_HIGH_DENSITY = "residential_high_density"    # R3: arranha-céus
    COMMERCIAL_LOCAL = "commercial_local"                    # C1: comércio de bairro
    COMMERCIAL_CENTRAL = "commercial_central"                # C2: centro comercial
    INDUSTRIAL_LIGHT = "industrial_light"                    # I1: indústria limpa
    INDUSTRIAL_HEAVY = "industrial_heavy"                    # I2: indústria pesada
    MIXED_USE = "mixed_use"                                  # Misto (res + com)
    RURAL = "rural"                                          # Área rural
    PROTECTED = "protected"                                  # Preservação ambiental
    SPECIAL_USE = "special_use"                              # Uso especial (hospital, escola)

# MODELO: BUILDING (EDIFÍCIO COMPLETO)
class Building(Base):
    """
    Edifícios da cidade com sistema detalhado.
    Suporta todos os tipos de construções com atributos complexos.
    """
    __tablename__ = 'buildings'

    # IDENTIFICAÇÃO
    id = Column(GUID(), primary_key=True, default=uuid.uuid4)
    name = Column(String(200), nullable=False, index=True)
    building_type = Column(SQLEnum(BuildingType), nullable=False)

    # LOCALIZAÇÃO
    x = Column(Integer, nullable=False, comment="Coordenada X no grid")
    y = Column(Integer, nullable=False, comment="Coordenada Y no grid")
    address = Column(String(200), default="")
    neighborhood = Column(String(100), default="")
    postal_code = Column(String(20), default="")
    zoning = Column(SQLEnum(BuildingZoning), default=BuildingZoning.MIXED_USE)

    # DIMENSÕES
    width = Column(Float, default=10.0, comment="Largura em metros")
    length = Column(Float, default=10.0, comment="Comprimento em metros")
    height = Column(Float, default=5.0, comment="Altura em metros")
    floors = Column(Integer, default=1, comment="Número de andares")

    # STATUS E CONDIÇÃO
    status = Column(SQLEnum(BuildingStatus), default=BuildingStatus.OPERATIONAL_ACTIVE)
    condition = Column(SQLEnum(BuildingCondition), default=BuildingCondition.GOOD)
    condition_value = Column(Integer, default=80, comment="0-100 (numérico)")
    __table_args__ = (
        CheckConstraint('condition_value >= 0 AND condition_value <= 100', name='check_building_condition_range'),
    )

    # PROPRIEDADE
    owner_id = Column(GUID(), ForeignKey('agents.id'), nullable=True, comment="ID do agente dono")
    owner_type = Column(SQLEnum(BuildingOwnershipType), default=BuildingOwnershipType.PRIVATE_INDIVIDUAL)

    # ARQUITETURA
    architecture_style = Column(SQLEnum(BuildingArchitectureStyle), default=BuildingArchitectureStyle.GENERIC)
    construction_year = Column(Integer, default=1900)
    era = Column(Integer, default=1, comment="1-4, baseado no ano")

    # CAPACIDADE E OCUPAÇÃO
    max_occupancy = Column(Integer, default=10, comment="Pessoas simultâneas")
    current_occupancy = Column(Integer, default=0)
    units = Column(Integer, default=0, comment="Apartamentos/salas/leitos")
    parking_spaces = Column(Integer, default=0)

    # CONSTRUÇÃO
    foundation_type = Column(String(50), default="concrete", comment="concreto, madeira, pedra")
    structure_type = Column(String(50), default="brick", comment="tijolo, concreto, aço, madeira")
    roof_type = Column(String(50), default="tile", comment="telha, laje, zinco")
    exterior_finish = Column(String(50), default="painted")
    interior_finish = Column(String(50), default="basic")

    # UTILIDADES
    has_electricity = Column(Boolean, default=True)
    has_water = Column(Boolean, default=True)
    has_sewage = Column(Boolean, default=True)
    has_heating = Column(Boolean, default=False)
    has_ac = Column(Boolean, default=False)
    has_elevator = Column(Boolean, default=False)
    has_generator = Column(Boolean, default=False)

    # ACESSIBILIDADE E EXTRAS
    wheelchair_accessible = Column(Boolean, default=False)
    has_garden = Column(Boolean, default=False)
    has_balcony = Column(Boolean, default=False)
    has_basement = Column(Boolean, default=False)
    has_attic = Column(Boolean, default=False)

    # ECONOMIA
    land_value = Column(DECIMAL(13, 2), default=50000.00)
    construction_cost = Column(DECIMAL(13, 2), default=100000.00)
    current_market_value = Column(DECIMAL(13, 2), default=150000.00)
    maintenance_cost = Column(DECIMAL(13, 2), default=0.00, comment="Mensal")
    utility_costs = Column(DECIMAL(13, 2), default=0.00, comment="Água, luz, gás (mensal)")
    tax_property = Column(DECIMAL(13, 2), default=0.00, comment="IPTU (mensal)")
    insurance_cost = Column(DECIMAL(13, 2), default=0.00, comment="Mensal")
    rental_income = Column(DECIMAL(13, 2), default=0.00, comment="Aluguel recebido (mensal)")
    business_revenue = Column(DECIMAL(13, 2), default=0.00, comment="Receita de negócio (mensal)")
    jobs_created = Column(Integer, default=0)
    total_invested = Column(DECIMAL(13, 2), default=0.00)
    expected_roi = Column(Float, default=0.0, comment="Retorno esperado (%)")

    #  HISTÓRICO
    construction_started = Column(DateTime, nullable=True)
    construction_completed = Column(DateTime, nullable=True)
    inauguration_date = Column(DateTime, nullable=True)
    last_renovation = Column(DateTime, nullable=True)
    last_inspection = Column(DateTime, nullable=True)
    major_events = Column(JSON, default=lambda: [], comment="Eventos importantes (incêndios, reformas, etc)")
    ownership_history = Column(JSON, default=lambda: [], comment="Mudanças de proprietário")
    renovations = Column(JSON, default=lambda: [], comment="Histórico de reformas")

    # MEIO AMBIENTE
    energy_consumption_kwh_month = Column(Float, default=0.0)
    water_consumption_m3_month = Column(Float, default=0.0)
    waste_production_kg_month = Column(Float, default=0.0)
    co2_emissions_kg_year = Column(Float, default=0.0)
    noise_level_db = Column(Float, default=40.0)
    has_solar_panels = Column(Boolean, default=False)
    has_rainwater_harvesting = Column(Boolean, default=False)
    has_green_roof = Column(Boolean, default=False)
    has_recycling_system = Column(Boolean, default=False)
    leed_certified = Column(Boolean, default=False)
    energy_efficiency_rating = Column(String(5), default="D", comment="A+ a G")

    # SEGURANÇA
    has_fire_alarm = Column(Boolean, default=False)
    has_sprinklers = Column(Boolean, default=False)
    has_fire_extinguishers = Column(Boolean, default=False)
    has_emergency_exits = Column(Integer, default=1)
    has_smoke_detectors = Column(Boolean, default=False)
    has_security_guard = Column(Boolean, default=False)
    has_cameras = Column(Boolean, default=False)
    has_alarm_system = Column(Boolean, default=False)
    has_safe = Column(Boolean, default=False)
    seismic_resistant = Column(Boolean, default=False)
    flood_resistant = Column(Boolean, default=False)
    last_fire_inspection = Column(DateTime, nullable=True)
    last_structural_inspection = Column(DateTime, nullable=True)
    safety_violations = Column(Integer, default=0)

    #  CONSTRUÇÃO EM ANDAMENTO
    construction_progress = Column(Integer, default=100, comment="0-100%")
    construction_start_date = Column(DateTime, nullable=True)
    estimated_completion_date = Column(DateTime, nullable=True)

    # GAMEPLAY
    happiness_modifier = Column(Float, default=0.0, comment="Bônus/penalidade para moradores")
    crime_rate = Column(Float, default=0.0, comment="0-1, taxa de criminalidade local")
    noise_complaints = Column(Integer, default=0)
    health_violations = Column(Integer, default=0)

    # VISUAL (para renderização)
    texture_id = Column(String(200), nullable=True)
    model_id = Column(String(200), nullable=True)
    color = Column(String(7), default="#FFFFFF", comment="Cor principal (hex)")
    is_visible = Column(Boolean, default=True)

    #  INTEGRAÇÃO FÍSICA (IoT)
    has_led = Column(Boolean, default=False, comment="Tem LED físico na maquete?")
    led_pin = Column(Integer, nullable=True, comment="Pino do Arduino para controlar LED")

    # METADATA
    created_at = Column(DateTime, default=datetime.utcnow, nullable=False)
    updated_at = Column(DateTime, default=datetime.utcnow, onupdate=datetime.utcnow)
    demolished_at = Column(DateTime, nullable=True, comment="Soft delete")
    tags = Column(JSON, default=lambda: [], comment="Ex: ['historic', 'landmark']")
    notes = Column(Text, default="", comment="Notas do jogador")

    # RELACIONAMENTOS
    # Proprietário (agente que é dono)
    owner = relationship(
        "Agent",
        foreign_keys=[owner_id],
        back_populates="owned_buildings"
    )

    # Moradores (agentes que moram aqui)
    residents = relationship(
        "Agent",
        foreign_keys="Agent.home_building_id",
        back_populates="home"
    )

    # Trabalhadores (agentes que trabalham aqui)
    workers = relationship(
        "Agent",
        foreign_keys="Agent.work_building_id",
        back_populates="workplace"
    )

    # Veículos estacionados/garagem
    parked_vehicles = relationship(
        "Vehicle",
        foreign_keys="Vehicle.home_building_id",
        back_populates="home_building"
    )

    # Estações dentro ou anexas ao edifício
    stations = relationship(
        "Station",
        foreign_keys="Station.building_id",
        back_populates="building"
    )

    # MÉTODOS

    def calculate_monthly_costs(self) -> float:
        """Calcula custos operacionais mensais totais"""
        return float(
            self.maintenance_cost +
            self.utility_costs +
            self.tax_property +
            self.insurance_cost)

    def calculate_monthly_income(self) -> float:
        """Calcula receita mensal total"""
        return float(
            self.rental_income +
            self.business_revenue
        )

    def is_profitable(self) -> bool:
        """Retorna se o edifício é lucrativo (receita > custos)"""
        return self.calculate_monthly_income() > self.calculate_monthly_costs()

    def get_occupancy_rate(self) -> float:
        """Retorna taxa de ocupação (0.0 a 1.0)"""
        if self.max_occupancy == 0:
            return 0.0
        return min(1.0, self.current_occupancy / self.max_occupancy)

    def is_operational(self) -> bool:
        """Verifica se edifício está operacional"""
        return (
            self.status == BuildingStatus.OPERATIONAL_ACTIVE and
            self.condition_value > 20
        )

    def can_accommodate(self, num_people: int) -> bool:
        """Verifica se há espaço para mais pessoas"""
        return (self.current_occupancy + num_people) <= self.max_occupancy

    @property
    def age(self) -> int:
        """Calcula idade do edifício em anos"""
        current_year = datetime.utcnow().year
        return current_year - self.construction_year

    def __repr__(self):
        return f"<Building(id={self.id}, name='{self.name}', type='{self.building_type.value}')>"


# Enums para Vehicle
class VehicleStatus(str, enum.Enum):
    """Status do veículo."""
    ACTIVE = 'active'
    INACTIVE = 'inactive'
    MAINTENANCE = 'maintenance'
    RETIRED = 'retired'


class MaintenanceStatus(str, enum.Enum):
    """Status de manutenção do veículo."""
    GOOD = 'good'
    FAIR = 'fair'
    POOR = 'poor'
    CRITICAL = 'critical'


class FuelType(str, enum.Enum):
    """Tipo de combustível."""
    GASOLINE = 'gasoline'
    DIESEL = 'diesel'
    ELECTRIC = 'electric'
    HYBRID = 'hybrid'
    COAL = 'coal'
    STEAM = 'steam'
    BIODIESEL = 'biodiesel'
    ETHANOL = 'ethanol'


# Modelo: Vehicle (Veículo)
class Vehicle(Base):
    """Veículos de transporte (trens, ônibus, carros, etc).

    Representa veículos na simulação com capacidades completas de transporte,
    manutenção, combustível, economia e operação.
    """
    __tablename__ = 'vehicles'

    # ==================== IDENTIFICAÇÃO ====================
    id = Column(GUID(), primary_key=True, default=uuid.uuid4)
    name = Column(String(200), nullable=False, index=True)
    vehicle_type = Column(String(50), nullable=False, comment="train, bus, car, truck, tram, etc")
    license_plate = Column(String(20), unique=True, nullable=True)

    # ==================== FABRICAÇÃO ====================
    manufacturer = Column(String(100), nullable=True, comment="Fabricante do veículo")
    model = Column(String(100), nullable=True, comment="Modelo do veículo")
    year = Column(Integer, nullable=True, comment="Ano de fabricação")

    # ==================== CAPACIDADE ====================
    passenger_capacity = Column(Integer, default=0, comment="Capacidade máxima de passageiros")
    cargo_capacity = Column(Float, default=0.0, comment="Capacidade de carga em toneladas")
    current_passengers = Column(Integer, default=0, comment="Passageiros atuais")
    current_cargo = Column(Float, default=0.0, comment="Carga atual em toneladas")

    # ==================== COMBUSTÍVEL ====================
    fuel_type = Column(SQLEnum(FuelType), default=FuelType.DIESEL)
    fuel_capacity = Column(Float, default=100.0, comment="Capacidade do tanque em litros")
    current_fuel = Column(Float, default=100.0, comment="Combustível atual em litros")
    fuel_consumption = Column(Float, default=10.0, comment="Consumo em litros por 100km")
    fuel_cost_total = Column(DECIMAL(13, 2), default=0.00, comment="Custo total gasto com combustível")

    # ==================== STATUS E MANUTENÇÃO ====================
    status = Column(SQLEnum(VehicleStatus), default=VehicleStatus.ACTIVE, index=True)
    maintenance_status = Column(SQLEnum(MaintenanceStatus), default=MaintenanceStatus.GOOD)
    condition_value = Column(Integer, default=100, comment="Condição geral do veículo (0-100)")

    # ==================== ODÔMETRO E MANUTENÇÃO ====================
    odometer = Column(Float, default=0.0, comment="Quilometragem total acumulada")
    next_maintenance_km = Column(Float, default=10000.0, comment="Próxima manutenção em km")
    last_maintenance_km = Column(Float, default=0.0, comment="Km da última manutenção")
    last_maintenance_at = Column(DateTime, nullable=True)

    # ==================== ECONOMIA ====================
    purchase_price = Column(DECIMAL(13, 2), nullable=True, comment="Preço de compra")
    current_value = Column(DECIMAL(13, 2), nullable=True, comment="Valor atual de mercado")
    depreciation_rate = Column(Float, default=0.15, comment="Taxa anual de depreciação (0.15 = 15%)")
    maintenance_cost_total = Column(DECIMAL(13, 2), default=0.00, comment="Custo total de manutenções")

    # ==================== LOCALIZAÇÃO ====================
    current_location_type = Column(String(50), nullable=True, comment="building, stop, route, tile")
    current_location_id = Column(GUID(), nullable=True)
    current_x = Column(Integer, nullable=True, comment="Coordenada X atual")
    current_y = Column(Integer, nullable=True, comment="Coordenada Y atual")
    current_tile_id = Column(GUID(), nullable=True, comment="Tile atual no mapa")

    # ==================== RELACIONAMENTOS (Foreign Keys) ====================
    route_id = Column(GUID(), ForeignKey('routes.id'), nullable=True)
    company_id = Column(GUID(), ForeignKey('companies.id'), nullable=True)
    current_driver_id = Column(GUID(), ForeignKey('agents.id'), nullable=True)
    home_building_id = Column(GUID(), ForeignKey('buildings.id'), nullable=True,
                             comment="Garagem/depósito onde fica quando inativo")

    # Novos campos para integração com Station
    current_station_id = Column(GUID(), ForeignKey('stations.id'), nullable=True,
                               comment="Estação atual onde o veículo está (se aplicável)")
    assigned_route_id = Column(GUID(), ForeignKey('routes.id'), nullable=True,
                              comment="Rota atribuída ao veículo (diferente de route_id que é rota ativa)")
    current_route_id = Column(GUID(), ForeignKey('routes.id'), nullable=True)
    current_route = relationship("Route", foreign_keys=[current_route_id], back_populates="vehicles")

    # ==================== OPERAÇÃO ====================
    is_docked = Column(Boolean, default=False, comment="Se está acoplado/parado em uma estação")
    is_operational = Column(Boolean, default=True, comment="Se está operacional ou não")
    is_moving = Column(Boolean, default=False, comment="Se está em movimento")
    speed = Column(Float, default=0.0, comment="Velocidade atual em km/h")
    max_speed = Column(Float, default=60.0, comment="Velocidade máxima em km/h")
    last_trip_at = Column(DateTime, nullable=True, comment="Data/hora da última viagem")
    total_trips = Column(Integer, default=0, comment="Total de viagens realizadas")
    total_distance = Column(Float, default=0.0, comment="Distância total percorrida em km")

    # ==================== SEGURANÇA ====================
    has_insurance = Column(Boolean, default=False, comment="Possui seguro")
    insurance_expiry = Column(DateTime, nullable=True, comment="Data de expiração do seguro")
    accident_count = Column(Integer, default=0, comment="Número de acidentes")
    last_accident_date = Column(DateTime, nullable=True, comment="Data do último acidente")

    # ==================== VISUAL (IoT) ====================
    has_led = Column(Boolean, default=False, comment="Tem LED físico na maquete")
    led_pin = Column(Integer, nullable=True, comment="Pino do Arduino para LED")
    model_id = Column(String(200), nullable=True, comment="ID do modelo 3D")

    # ==================== METADATA ====================
    purchase_date = Column(DateTime, nullable=True, comment="Data de aquisição")
    retirement_date = Column(DateTime, nullable=True, comment="Data de aposentadoria")
    created_at = Column(DateTime, default=datetime.utcnow)
    updated_at = Column(DateTime, default=datetime.utcnow, onupdate=datetime.utcnow)
    notes = Column(Text, default="", comment="Notas adicionais sobre o veículo")

    # ==================== CONSTRAINTS E ÍNDICES ====================
    __table_args__ = (
        CheckConstraint('current_fuel >= 0 AND current_fuel <= fuel_capacity', name='check_fuel_range'),
        CheckConstraint('condition_value >= 0 AND condition_value <= 100', name='check_vehicle_condition_range'),
        CheckConstraint('current_passengers >= 0 AND current_passengers <= passenger_capacity', name='check_passengers'),
        CheckConstraint('current_cargo >= 0 AND current_cargo <= cargo_capacity', name='check_cargo'),
        CheckConstraint('odometer >= 0', name='check_odometer_positive'),
        CheckConstraint('speed >= 0 AND speed <= max_speed', name='check_speed_range'),
        CheckConstraint('total_distance >= 0', name='check_total_distance_positive'),
        Index('idx_vehicle_status', 'status'),
        Index('idx_vehicle_company', 'company_id'),
        Index('idx_vehicle_route', 'route_id'),
        Index('idx_vehicle_driver', 'current_driver_id'),
        Index('idx_vehicle_type', 'vehicle_type'),
        Index('idx_vehicle_location', 'current_location_type', 'current_location_id'),
        Index('idx_vehicle_station', 'current_station_id'),
        Index('idx_vehicle_docked', 'is_docked'),
    )

    # ==================== RELACIONAMENTOS ORM ====================

    # Relacionamentos diretos
    route = relationship('Route', foreign_keys=[route_id], back_populates='vehicles')
    company = relationship('Company', foreign_keys=[company_id], back_populates='vehicles')
    current_driver = relationship('Agent', foreign_keys=[current_driver_id], back_populates='driven_vehicles')
    home_building = relationship('Building', foreign_keys=[home_building_id], back_populates='parked_vehicles')
    current_station = relationship('Station', foreign_keys=[current_station_id])
    assigned_route = relationship('Route', foreign_keys=[assigned_route_id])

    # Issue 4.9: Horários de veículos
    schedules = relationship(
        'Schedule',
        back_populates='vehicle',
        cascade='all, delete-orphan'
    )

    # Relacionamentos polimórficos (viewonly para performance)
    maintenance_records = relationship(
        'MaintenanceRecord',
        primaryjoin="and_(MaintenanceRecord.target_type=='vehicle', "
                   "foreign(MaintenanceRecord.target_id)==Vehicle.id)",
        viewonly=True,
        lazy='dynamic'
    )

    inventory_items = relationship(
        'Inventory',
        primaryjoin="and_(Inventory.owner_type=='vehicle', "
                   "foreign(Inventory.owner_id)==Vehicle.id)",
        viewonly=True,
        lazy='dynamic'
    )

    # ==================== VALIDAÇÕES ====================

    @validates('current_fuel')
    def validate_fuel(self, key, value):
        """Valida o combustível atual."""
        if value < 0:
            raise ValueError("Combustível não pode ser negativo")
        if hasattr(self, 'fuel_capacity') and value > self.fuel_capacity:
            raise ValueError(f"Combustível ({value}) excede capacidade do tanque ({self.fuel_capacity})")
        return value

    @validates('status')
    def validate_status(self, key, value):
        """Valida mudança de status."""
        if value == VehicleStatus.RETIRED and not self.retirement_date:
            self.retirement_date = datetime.utcnow()
        return value

    @validates('condition_value')
    def validate_condition(self, key, value):
        """Valida o valor de condição."""
        if value < 0 or value > 100:
            raise ValueError("Condição deve estar entre 0 e 100")

        # Atualiza status de manutenção baseado na condição
        if value < 30:
            self.maintenance_status = MaintenanceStatus.CRITICAL
        elif value < 50:
            self.maintenance_status = MaintenanceStatus.POOR
        elif value < 70:
            self.maintenance_status = MaintenanceStatus.FAIR
        else:
            self.maintenance_status = MaintenanceStatus.GOOD

        return value

    # ==================== MÉTODOS DE NEGÓCIO ====================

    def needs_maintenance(self) -> bool:
        """Verifica se o veículo precisa de manutenção.

        Returns:
            bool: True se precisar de manutenção
        """
        return (
            self.odometer >= self.next_maintenance_km or
            self.maintenance_status in [MaintenanceStatus.POOR, MaintenanceStatus.CRITICAL] or
            self.condition_value < 30
        )

    def consume_fuel(self, distance_km: float) -> bool:
        """Consome combustível baseado na distância percorrida.

        Args:
            distance_km: Distância em quilômetros

        Returns:
            bool: True se havia combustível suficiente
        """
        consumption = (distance_km / 100) * self.fuel_consumption

        if self.current_fuel >= consumption:
            self.current_fuel -= consumption
            self.odometer += distance_km
            self.total_distance += distance_km

            # Degrada condição baseado no uso
            self.condition_value = max(0, self.condition_value - (distance_km / 1000))

            return True
        return False

    def refuel(self, amount: float = None) -> float:
        """Abastece o veículo.

        Args:
            amount: Quantidade de combustível (None = abastecer completamente)

        Returns:
            float: Quantidade efetivamente adicionada
        """
        if amount is None:
            added = self.fuel_capacity - self.current_fuel
            self.current_fuel = self.fuel_capacity
        else:
            added = min(amount, self.fuel_capacity - self.current_fuel)
            self.current_fuel += added

        return added

    def calculate_depreciation(self) -> Optional[DECIMAL]:
        """Calcula e atualiza a depreciação do veículo.

        Returns:
            DECIMAL: Novo valor do veículo ou None se não houver dados
        """
        if self.purchase_price and self.purchase_date:
            years = (datetime.utcnow() - self.purchase_date).days / 365.25
            self.current_value = self.purchase_price * ((1 - self.depreciation_rate) ** years)
            return self.current_value
        return None

    def can_operate(self) -> bool:
        """Verifica se o veículo pode operar.

        Returns:
            bool: True se pode operar
        """
        return (
            self.status == VehicleStatus.ACTIVE and
            self.is_operational and
            self.current_fuel > 0 and
            self.condition_value > 20 and
            self.maintenance_status != MaintenanceStatus.CRITICAL
        )

    def board_passenger(self) -> bool:
        """Embarca um passageiro.

        Returns:
            bool: True se conseguiu embarcar
        """
        if self.current_passengers < self.passenger_capacity:
            self.current_passengers += 1
            return True
        return False

    def board_passengers(self, count: int) -> bool:
        """Embarca múltiplos passageiros.

        Args:
            count: Número de passageiros

        Returns:
            bool: True se conseguiu embarcar todos
        """
        if (self.current_passengers + count) <= self.passenger_capacity:
            self.current_passengers += count
            return True
        return False

    def disembark_passenger(self) -> bool:
        """Desembarca um passageiro.

        Returns:
            bool: True se conseguiu desembarcar
        """
        if self.current_passengers > 0:
            self.current_passengers -= 1
            return True
        return False

    def disembark_passengers(self, count: int) -> bool:
        """Desembarca múltiplos passageiros.

        Args:
            count: Número de passageiros

        Returns:
            bool: True se conseguiu desembarcar todos
        """
        if self.current_passengers >= count:
            self.current_passengers -= count
            return True
        return False

    def load_cargo(self, weight: float) -> bool:
        """Carrega carga no veículo.

        Args:
            weight: Peso da carga em toneladas

        Returns:
            bool: True se conseguiu carregar
        """
        if (self.current_cargo + weight) <= self.cargo_capacity:
            self.current_cargo += weight
            return True
        return False

    def unload_cargo(self, weight: float) -> bool:
        """Descarrega carga do veículo.

        Args:
            weight: Peso da carga em toneladas

        Returns:
            bool: True se conseguiu descarregar
        """
        if self.current_cargo >= weight:
            self.current_cargo -= weight
            return True
        return False

    def perform_maintenance(self, cost: DECIMAL = None):
        """Realiza manutenção no veículo.

        Args:
            cost: Custo da manutenção (opcional)
        """
        self.last_maintenance_km = self.odometer
        self.next_maintenance_km = self.odometer + 10000.0
        self.last_maintenance_at = datetime.utcnow()
        self.condition_value = min(100, self.condition_value + 30)
        self.maintenance_status = MaintenanceStatus.GOOD

        if cost:
            self.maintenance_cost_total += cost

    def start_trip(self):
        """Inicia uma viagem."""
        self.is_moving = True
        self.last_trip_at = datetime.utcnow()

    def end_trip(self):
        """Finaliza uma viagem."""
        self.is_moving = False
        self.speed = 0.0
        self.total_trips += 1

    def register_accident(self):
        """Registra um acidente."""
        self.accident_count += 1
        self.last_accident_date = datetime.utcnow()
        self.condition_value = max(0, self.condition_value - 20)
        self.status = VehicleStatus.MAINTENANCE

    # ==================== PROPRIEDADES CALCULADAS ====================

    @property
    def age_years(self) -> float:
        """Idade do veículo em anos.

        Returns:
            float: Anos desde a compra
        """
        if self.purchase_date:
            return (datetime.utcnow() - self.purchase_date).days / 365.25
        return 0.0

    @property
    def occupancy_rate(self) -> float:
        """Taxa de ocupação de passageiros.

        Returns:
            float: Taxa de 0.0 a 1.0
        """
        if self.passenger_capacity == 0:
            return 0.0
        return min(1.0, self.current_passengers / self.passenger_capacity)

    @property
    def cargo_rate(self) -> float:
        """Taxa de ocupação de carga.

        Returns:
            float: Taxa de 0.0 a 1.0
        """
        if self.cargo_capacity == 0:
            return 0.0
        return min(1.0, self.current_cargo / self.cargo_capacity)

    @property
    def fuel_percentage(self) -> float:
        """Percentual de combustível.

        Returns:
            float: Percentual de 0.0 a 100.0
        """
        if self.fuel_capacity == 0:
            return 0.0
        return (self.current_fuel / self.fuel_capacity) * 100

    @property
    def needs_refuel(self) -> bool:
        """Verifica se precisa reabastecer.

        Returns:
            bool: True se combustível está abaixo de 20%
        """
        return self.fuel_percentage < 20.0

    @property
    def average_km_per_trip(self) -> float:
        """Média de quilômetros por viagem.

        Returns:
            float: Média de km/viagem
        """
        if self.total_trips == 0:
            return 0.0
        return self.total_distance / self.total_trips

    # ==================== MÉTODOS ADICIONAIS ====================

    def get_fuel_cost(self, fuel_price_per_liter: float) -> DECIMAL:
        """Calcula custo total de combustível gasto.

        Args:
            fuel_price_per_liter: Preço por litro do combustível

        Returns:
            DECIMAL: Custo total estimado
        """
        from decimal import Decimal
        total_fuel_used = (self.total_distance / 100) * self.fuel_consumption
        return Decimal(str(total_fuel_used * fuel_price_per_liter))

    def estimate_next_maintenance_date(self) -> Optional[datetime]:
        """Estima data da próxima manutenção baseada no uso médio.

        Returns:
            datetime: Data estimada ou None se não for possível calcular
        """
        from datetime import timedelta

        if self.total_trips == 0 or self.total_distance == 0:
            return None

        days_since_creation = max(1, (datetime.utcnow() - self.created_at).days)
        avg_km_per_day = self.total_distance / days_since_creation

        km_remaining = self.next_maintenance_km - self.odometer
        if km_remaining <= 0:
            return datetime.utcnow()  # Já passou

        days_remaining = km_remaining / max(0.1, avg_km_per_day)
        return datetime.utcnow() + timedelta(days=int(days_remaining))

    def is_overdue_maintenance(self) -> bool:
        """Verifica se está com manutenção atrasada.

        Returns:
            bool: True se passou do km de manutenção
        """
        return self.odometer > self.next_maintenance_km

    def efficiency_rating(self) -> str:
        """Retorna rating de eficiência A-F baseado em consumo.

        Returns:
            str: Rating de 'A' (melhor) a 'F' (pior)
        """
        if self.fuel_consumption <= 5:
            return 'A'
        elif self.fuel_consumption <= 8:
            return 'B'
        elif self.fuel_consumption <= 12:
            return 'C'
        elif self.fuel_consumption <= 15:
            return 'D'
        elif self.fuel_consumption <= 20:
            return 'E'
        return 'F'

    def get_maintenance_history(self):
        """Retorna histórico de manutenções.

        Returns:
            list: Lista de registros de manutenção
        """
        return list(self.maintenance_records.order_by('created_at'))

    def get_total_maintenance_cost(self) -> DECIMAL:
        """Calcula custo total de todas as manutenções.

        Returns:
            DECIMAL: Custo total
        """
        from decimal import Decimal
        total = Decimal('0.00')
        for record in self.maintenance_records:
            if record.cost:
                total += record.cost
        return total

    def calculate_total_operating_cost(self, fuel_price_per_liter: float = 5.0):
        """Calcula custo operacional total.

        Args:
            fuel_price_per_liter: Preço do combustível

        Returns:
            Decimal: Custo total (combustível + manutenção)
        """
        from decimal import Decimal
        fuel_cost = self.get_fuel_cost(fuel_price_per_liter)
        maintenance_cost = self.get_total_maintenance_cost()
        return Decimal(str(fuel_cost)) + Decimal(str(maintenance_cost))

    def __repr__(self):
        return f"<Vehicle(id={self.id}, type='{self.vehicle_type}', license='{self.license_plate}', status='{self.status.value}')>"


# Modelo: Ticket (Bilhete de Transporte)
class Ticket(Base):
    """Bilhetes de transporte público."""
    __tablename__ = 'tickets'

    # Identificação
    id = Column(GUID(), primary_key=True, default=uuid.uuid4)
    ticket_type = Column(SQLEnum(TicketType), default=TicketType.SINGLE, nullable=False)
    status = Column(SQLEnum(TicketStatus), default=TicketStatus.ACTIVE, nullable=False, index=True)

    # Proprietário
    agent_id = Column(GUID(), ForeignKey('agents.id'), nullable=False, index=True)

    # Rota e estações
    route_id = Column(GUID(), ForeignKey('routes.id'), nullable=True)
    origin_station_id = Column(GUID(), ForeignKey('stations.id'), nullable=True)
    destination_station_id = Column(GUID(), ForeignKey('stations.id'), nullable=True)

    # Validade
    purchased_at = Column(DateTime, default=datetime.utcnow, nullable=False)
    valid_from = Column(DateTime, default=datetime.utcnow, nullable=False)
    valid_until = Column(DateTime, nullable=True)
    used_at = Column(DateTime, nullable=True)

    # Valor
    price = Column(DECIMAL(10, 2), default=0.00)
    fare_zone = Column(Integer, default=1)

    # Validação
    validation_count = Column(Integer, default=0, comment="Número de validações")
    max_validations = Column(Integer, default=1, comment="Máximo de validações permitidas")

    # Metadata
    created_at = Column(DateTime, default=datetime.utcnow)
    cancelled_at = Column(DateTime, nullable=True)
    notes = Column(Text, default="")

    # Constraints
    __table_args__ = (
        CheckConstraint('validation_count >= 0', name='check_validation_count_positive'),
        CheckConstraint('validation_count <= max_validations', name='check_validation_within_max'),
        Index('idx_ticket_agent', 'agent_id'),
        Index('idx_ticket_status', 'status'),
        Index('idx_ticket_validity', 'valid_from', 'valid_until'),
    )

    # Relacionamentos
    agent = relationship('Agent', foreign_keys=[agent_id], back_populates='tickets')
    route = relationship('Route', foreign_keys=[route_id])
    origin_station = relationship('Station', foreign_keys=[origin_station_id])
    destination_station = relationship('Station', foreign_keys=[destination_station_id])

    def is_valid(self) -> bool:
        """Verifica se o bilhete é válido no momento atual."""
        if self.status != TicketStatus.ACTIVE:
            return False

        now = datetime.utcnow()
        if now < self.valid_from:
            return False

        if self.valid_until and now > self.valid_until:
            self.status = TicketStatus.EXPIRED
            return False

        if self.validation_count >= self.max_validations:
            return False

        return True

    def validate(self) -> bool:
        """Valida (usa) o bilhete."""
        if not self.is_valid():
            return False

        self.validation_count += 1
        if not self.used_at:
            self.used_at = datetime.utcnow()

        if self.validation_count >= self.max_validations:
            self.status = TicketStatus.USED

        return True

    def cancel(self):
        """Cancela o bilhete."""
        self.status = TicketStatus.CANCELLED
        self.cancelled_at = datetime.utcnow()

    def __repr__(self):
        return f"<Ticket(id={self.id}, type='{self.ticket_type.value}', status='{self.status.value}', agent_id={self.agent_id})>"


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
    event_data = Column(JSON, default=lambda: {})

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


# Modelo: Company / Organization
class Company(Base):
    """Empresas e organizações que empregam agentes e possuem assets."""
    __tablename__ = 'companies'

    id = Column(GUID(), primary_key=True, default=uuid.uuid4)
    name = Column(String(200), nullable=False, index=True)
    company_type = Column(String(50), nullable=False, comment="industry, retail, transport, public_service, etc")

    # Finanças
    balance = Column(DECIMAL(15, 2), default=0.00, comment="Saldo financeiro da empresa")
    revenue = Column(DECIMAL(15, 2), default=0.00, comment="Receita total")
    expenses = Column(DECIMAL(15, 2), default=0.00, comment="Despesas totais")

    # Reputação e status
    reputation = Column(Integer, default=50, comment="0-100, reputação da empresa")
    is_active = Column(Boolean, default=True)

    # Estatísticas
    active_employees_count = Column(Integer, default=0)
    total_vehicles = Column(Integer, default=0)
    total_buildings = Column(Integer, default=0)

    # Timestamps
    founded_at = Column(DateTime, default=datetime.utcnow)
    created_at = Column(DateTime, default=datetime.utcnow)
    updated_at = Column(DateTime, default=datetime.utcnow, onupdate=datetime.utcnow)

    # Relacionamentos reversos
    vehicles = relationship('Vehicle', foreign_keys='Vehicle.company_id', back_populates='company')

    def __repr__(self):
        return f"<Company(id={self.id}, name='{self.name}', type='{self.company_type}')>"


# Nota: Profession já definido anteriormente na linha ~416


# Modelo: Route / Line
class Route(Base):
    """
    Rotas de transporte público com sistema complexo de simulação.
    Suporta múltiplas eras, padrões dinâmicos e economia realista.
    """
    __tablename__ = 'routes'

    # ==================== IDENTIFICAÇÃO ====================
    id = Column(GUID(), primary_key=True, default=uuid.uuid4)
    name = Column(String(200), nullable=False, index=True)
    code = Column(String(20), unique=True, index=True, comment="Ex: L1, 501, Norte-Sul")
    description = Column(Text, comment="Descrição da linha para passageiros")

    # ==================== TIPO E ERA ====================
    route_type = Column(SQLEnum(StationType), nullable=False)
    era = Column(Integer, default=1, comment="1-4, era histórica da rota")
    technology_level = Column(Integer, default=1, comment="0-10, nível tecnológico")

    # ==================== OPERAÇÃO BÁSICA ====================
    operating_hours_start = Column(Time, nullable=False, default=time(6, 0))
    operating_hours_end = Column(Time, nullable=False, default=time(22, 0))
    frequency_minutes = Column(Integer, default=30, comment="Intervalo entre partidas")
    frequency_pattern = Column(
        SQLEnum(RouteFrequencyPattern),
        default=RouteFrequencyPattern.CONSTANT
    )

    # ==================== HORÁRIOS DINÂMICOS ====================
    # Frequência em horários de pico (se aplicável)
    peak_frequency_minutes = Column(Integer, nullable=True, comment="Frequência no pico")
    peak_morning_start = Column(Time, default=time(7, 0))
    peak_morning_end = Column(Time, default=time(9, 0))
    peak_evening_start = Column(Time, default=time(17, 0))
    peak_evening_end = Column(Time, default=time(19, 0))

    # Operação em finais de semana
    weekend_frequency_minutes = Column(Integer, nullable=True)
    operates_sunday = Column(Boolean, default=True)
    operates_holidays = Column(Boolean, default=True)

    # ==================== STATUS E PRIORIDADE ====================
    is_active = Column(Boolean, default=True, index=True)
    status = Column(SQLEnum(RouteStatus), default=RouteStatus.ACTIVE)
    priority = Column(SQLEnum(RoutePriority), default=RoutePriority.MEDIUM)

    # ==================== CAPACIDADE E DEMANDA ====================
    max_vehicles_operating = Column(Integer, default=1, comment="Veículos simultâneos na rota")
    current_vehicles_operating = Column(Integer, default=0)
    average_daily_passengers = Column(Integer, default=0)
    peak_daily_passengers = Column(Integer, default=0)
    capacity_utilization = Column(Float, default=0.0, comment="0.0-1.0, utilização média")

    # ==================== ECONOMIA ====================
    fare_base = Column(DECIMAL(10, 2), default=3.50, comment="Tarifa base")
    fare_peak = Column(DECIMAL(10, 2), nullable=True, comment="Tarifa em horário de pico")
    fare_weekend = Column(DECIMAL(10, 2), nullable=True, comment="Tarifa fim de semana")
    fare_integration = Column(DECIMAL(10, 2), nullable=True, comment="Tarifa integração")

    # Receitas e custos
    monthly_revenue = Column(DECIMAL(15, 2), default=0.00)
    monthly_operational_cost = Column(DECIMAL(15, 2), default=0.00)
    monthly_maintenance_cost = Column(DECIMAL(15, 2), default=0.00)
    fuel_cost_per_km = Column(DECIMAL(10, 2), default=0.00)

    # ==================== PERFORMANCE ====================
    total_distance_km = Column(Float, default=0.0, comment="Distância total da rota")
    average_trip_duration_minutes = Column(Integer, default=30)
    on_time_performance = Column(Float, default=1.0, comment="0.0-1.0, pontualidade")
    accident_count = Column(Integer, default=0)
    breakdown_count = Column(Integer, default=0)
    customer_satisfaction = Column(Float, default=0.75, comment="0.0-1.0")

    # ==================== INFRAESTRUTURA ====================
    requires_dedicated_lane = Column(Boolean, default=False, comment="BRT, faixa exclusiva")
    has_platform_doors = Column(Boolean, default=False, comment="Portas de plataforma")
    is_electrified = Column(Boolean, default=False)
    has_real_time_tracking = Column(Boolean, default=False)
    has_wifi = Column(Boolean, default=False)
    has_ac = Column(Boolean, default=False)
    is_accessible = Column(Boolean, default=True, comment="Acessível para PCD")

    # ==================== AMBIENTAL ====================
    co2_emissions_kg_per_km = Column(Float, default=0.0)
    noise_level_db = Column(Float, default=70.0)
    energy_consumption_kwh_per_km = Column(Float, default=0.0)
    is_zero_emission = Column(Boolean, default=False)

    # ==================== GAMEPLAY/SIMULAÇÃO ====================
    construction_progress = Column(Integer, default=100, comment="0-100%, se em construção")
    popularity = Column(Float, default=0.5, comment="0.0-1.0, popularidade com cidadãos")
    political_support = Column(Float, default=0.5, comment="0.0-1.0, apoio político")

    # Eventos históricos
    inaugurated_at = Column(DateTime, nullable=True)
    last_accident_at = Column(DateTime, nullable=True)
    last_strike_at = Column(DateTime, nullable=True)

    # ==================== INTEGRAÇÃO IOT ====================
    has_physical_model = Column(Boolean, default=False, comment="Tem modelo físico na maquete?")
    sensor_ids = Column(JSON, default=lambda: [], comment="IDs dos sensores físicos associados")

    # ==================== DADOS COMPLEXOS (JSON) ====================
    # Horários especiais (feriados, eventos)
    special_schedules = Column(
        JSON,
        default=lambda: [],
        comment="[{date, frequency, reason}]"
    )

    # Histórico de mudanças de tarifa
    fare_history = Column(
        JSON,
        default=lambda: [],
        comment="[{date, old_fare, new_fare, reason}]"
    )

    # Incidentes
    incidents = Column(
        JSON,
        default=lambda: [],
        comment="[{date, type, description, impact}]"
    )

    # Estatísticas por dia da semana
    weekly_stats = Column(
        JSON,
        default=lambda: {},
        comment="{monday: {passengers, revenue}, ...}"
    )

    # Rotas alternativas (em caso de interrupção)
    alternative_routes = Column(
        JSON,
        default=lambda: [],
        comment="[route_id1, route_id2] rotas alternativas"
    )

    # ==================== TIMESTAMPS ====================
    created_at = Column(DateTime, default=datetime.utcnow, nullable=False)
    updated_at = Column(DateTime, default=datetime.utcnow, onupdate=datetime.utcnow)
    deactivated_at = Column(DateTime, nullable=True)

    # ==================== RELACIONAMENTOS ====================
    stations = relationship(
        "RouteStation",
        back_populates="route",
        cascade="all, delete-orphan",
        order_by="RouteStation.sequence_order"
    )

    vehicles = relationship(
        "Vehicle",
        foreign_keys="Vehicle.current_route_id",
        back_populates="current_route"
    )

    # Issue 4.9: Horários de veículos
    schedules = relationship(
        "Schedule",
        back_populates="route",
        cascade="all, delete-orphan"
    )

    # ==================== MÉTODOS ====================

    def calculate_daily_trips(self) -> int:
        """Calcula número de viagens por dia"""
        hours_operating = (
                self.operating_hours_end.hour * 60 + self.operating_hours_end.minute -
                (self.operating_hours_start.hour * 60 + self.operating_hours_start.minute)
        )
        return hours_operating // self.frequency_minutes if self.frequency_minutes > 0 else 0

    def calculate_monthly_profit(self) -> float:
        """Calcula lucro mensal"""
        return float(self.monthly_revenue - self.monthly_operational_cost - self.monthly_maintenance_cost)

    def is_profitable(self) -> bool:
        """Verifica se a rota é lucrativa"""
        return self.calculate_monthly_profit() > 0

    def get_current_frequency(self, current_time: time) -> int:
        """Retorna frequência baseada no horário atual"""
        if self.frequency_pattern == RouteFrequencyPattern.CONSTANT:
            return self.frequency_minutes

        # Verificar se está em horário de pico
        if self.peak_frequency_minutes:
            if (self.peak_morning_start <= current_time <= self.peak_morning_end or
                    self.peak_evening_start <= current_time <= self.peak_evening_end):
                return self.peak_frequency_minutes

        return self.frequency_minutes

    def get_current_fare(self, is_weekend: bool = False, is_peak: bool = False) -> float:
        """Retorna tarifa baseada em condições"""
        if is_weekend and self.fare_weekend:
            return float(self.fare_weekend)
        if is_peak and self.fare_peak:
            return float(self.fare_peak)
        return float(self.fare_base)

    def add_incident(self, incident_type: str, description: str, impact: str):
        """Adiciona incidente ao histórico"""
        if self.incidents is None:
            self.incidents = []

        self.incidents.append({
            "date": datetime.utcnow().isoformat(),
            "type": incident_type,
            "description": description,
            "impact": impact
        })

    def update_satisfaction(self, delta: float):
        """Atualiza satisfação do cliente"""
        self.customer_satisfaction = max(0.0, min(1.0, self.customer_satisfaction + delta))

    @property
    def is_operational(self) -> bool:
        """Verifica se a rota está operacional"""
        return (
                self.is_active and
                self.status in [RouteStatus.ACTIVE, RouteStatus.DELAYED, RouteStatus.OVERCROWDED]
        )

    @property
    def needs_expansion(self) -> bool:
        """Verifica se precisa de mais veículos"""
        return self.capacity_utilization > 0.9

    def __repr__(self):
        return f"<Route(id={self.id}, code='{self.code}', name='{self.name}', type='{self.route_type.value}')>"


class RouteStation(Base):
    """
    Associação entre rotas e estações com dados complexos de operação.
    Suporta múltiplos veículos, horários dinâmicos e estatísticas detalhadas.
    """
    __tablename__ = 'route_stations'

    # ==================== CHAVES PRIMÁRIAS ====================
    route_id = Column(GUID(), ForeignKey('routes.id', ondelete='CASCADE'), primary_key=True)
    station_id = Column(GUID(), ForeignKey('buildings.id', ondelete='CASCADE'), primary_key=True)

    # ==================== ORDEM E TEMPO ====================
    sequence_order = Column(Integer, nullable=False, comment="Posição na rota (1, 2, 3...)")
    estimated_stop_minutes = Column(Integer, default=2, comment="Tempo de parada padrão")
    peak_stop_minutes = Column(Integer, nullable=True, comment="Tempo de parada no pico")

    # Distância da estação anterior (em km)
    distance_from_previous_km = Column(Float, default=0.0)

    # Tempo estimado de viagem da estação anterior (minutos)
    travel_time_from_previous = Column(Integer, default=0)

    # ==================== OPERAÇÃO ====================
    is_terminal = Column(Boolean, default=False, comment="É ponta de linha?")
    is_transfer_point = Column(Boolean, default=False, comment="Ponto de transferência?")
    allows_boarding = Column(Boolean, default=True)
    allows_disembarking = Column(Boolean, default=True)
    is_express_stop = Column(Boolean, default=False, comment="Só expressos param aqui?")

    # ==================== DEMANDA ====================
    average_daily_boardings = Column(Integer, default=0)
    average_daily_disembarkings = Column(Integer, default=0)
    peak_hour_boardings = Column(Integer, default=0)
    peak_hour_disembarkings = Column(Integer, default=0)

    # Taxa de ocupação média ao sair desta estação
    average_occupancy_leaving = Column(Float, default=0.0, comment="0.0-1.0")

    # ==================== INFRAESTRUTURA DA PARADA ====================
    platform_length_meters = Column(Float, nullable=True)
    number_of_platforms = Column(Integer, default=1)
    has_shelter = Column(Boolean, default=True)
    has_seating = Column(Boolean, default=True)
    has_real_time_display = Column(Boolean, default=False)
    has_ticket_machine = Column(Boolean, default=False)
    has_security_camera = Column(Boolean, default=False)
    lighting_quality = Column(Integer, default=5, comment="0-10")

    # ==================== ACESSIBILIDADE ====================
    is_wheelchair_accessible = Column(Boolean, default=True)
    has_tactile_paving = Column(Boolean, default=False)
    has_audio_announcements = Column(Boolean, default=False)
    has_elevator = Column(Boolean, default=False)
    has_escalator = Column(Boolean, default=False)

    # ==================== TRANSFERÊNCIAS ====================
    # Outras rotas que param nesta estação (JSON com route_ids)
    connected_routes = Column(
        JSON,
        default=lambda: [],
        comment="[route_id1, route_id2] rotas que conectam aqui"
    )

    # Tempo de transferência estimado para cada rota
    transfer_times = Column(
        JSON,
        default=lambda: {},
        comment="{route_id: minutes} tempo de transferência"
    )

    # ==================== HORÁRIOS ESPECIAIS ====================
    # Horários em que ônibus expressos param aqui
    express_stop_times = Column(
        JSON,
        default=lambda: [],
        comment="[{time, direction}] horários de expresso"
    )

    # ==================== ESTATÍSTICAS ====================
    # Passageiros por hora do dia
    hourly_demand = Column(
        JSON,
        default=lambda: {},
        comment="{0: count, 1: count, ..., 23: count}"
    )

    # Passageiros por dia da semana
    weekly_demand = Column(
        JSON,
        default=lambda: {},
        comment="{monday: count, tuesday: count, ...}"
    )

    # ==================== PROBLEMAS E MANUTENÇÃO ====================
    last_maintenance = Column(DateTime, nullable=True)
    condition = Column(Integer, default=100, comment="0-100, estado de conservação")
    reported_issues = Column(Integer, default=0)

    # Histórico de problemas
    issues_history = Column(
        JSON,
        default=lambda: [],
        comment="[{date, type, description, resolved}]"
    )

    # ==================== SATISFAÇÃO ====================
    passenger_satisfaction = Column(Float, default=0.75, comment="0.0-1.0")
    cleanliness_rating = Column(Integer, default=7, comment="0-10")
    safety_rating = Column(Integer, default=7, comment="0-10")

    # ==================== TIMESTAMPS ====================
    added_to_route_at = Column(DateTime, default=datetime.utcnow)
    removed_from_route_at = Column(DateTime, nullable=True)

    # ==================== CONSTRAINTS ====================
    __table_args__ = (
        CheckConstraint('sequence_order > 0', name='check_sequence_positive'),
        CheckConstraint('estimated_stop_minutes >= 0', name='check_stop_time_non_negative'),
        CheckConstraint('distance_from_previous_km >= 0', name='check_distance_non_negative'),
        CheckConstraint('condition >= 0 AND condition <= 100', name='check_condition_range'),
        CheckConstraint('average_occupancy_leaving >= 0 AND average_occupancy_leaving <= 1',
                        name='check_occupancy_range'),
        # Índice composto para buscar estações em ordem
        Index('ix_route_stations_route_sequence', 'route_id', 'sequence_order'),
    )

    # ==================== RELACIONAMENTOS ====================
    route = relationship("Route", back_populates="stations")
    station = relationship("Building", foreign_keys=[station_id])

    # ==================== MÉTODOS ====================

    def get_estimated_stop_time(self, is_peak: bool = False) -> int:
        """Retorna tempo de parada baseado em condições"""
        if is_peak and self.peak_stop_minutes:
            return self.peak_stop_minutes
        return self.estimated_stop_minutes

    def calculate_total_daily_passengers(self) -> int:
        """Calcula total de passageiros por dia"""
        return self.average_daily_boardings + self.average_daily_disembarkings

    def add_issue(self, issue_type: str, description: str):
        """Adiciona problema ao histórico"""
        if self.issues_history is None:
            self.issues_history = []

        self.issues_history.append({
            "date": datetime.utcnow().isoformat(),
            "type": issue_type,
            "description": description,
            "resolved": False
        })
        self.reported_issues += 1

    def resolve_latest_issue(self):
        """Marca último problema como resolvido"""
        if self.issues_history and len(self.issues_history) > 0:
            self.issues_history[-1]["resolved"] = True
            self.issues_history[-1]["resolved_at"] = datetime.utcnow().isoformat()
            self.reported_issues = max(0, self.reported_issues - 1)

    @property
    def needs_maintenance(self) -> bool:
        """Verifica se precisa de manutenção"""
        return self.condition < 60 or self.reported_issues > 3

    @property
    def is_overcrowded(self) -> bool:
        """Verifica se está superlotada"""
        return self.average_occupancy_leaving > 0.9

    def __repr__(self):
        return f"<RouteStation(route_id={self.route_id}, station_id={self.station_id}, order={self.sequence_order})>"

# Modelo: Stop / Station
class Stop(Base):
    """Estações e paradas de transporte."""
    __tablename__ = 'stops'

    id = Column(GUID(), primary_key=True, default=uuid.uuid4)
    name = Column(String(100), nullable=False)
    stop_type = Column(String(50), nullable=False, comment="train_station, bus_stop, tram_stop, etc")

    # Localização
    building_id = Column(GUID(), ForeignKey('buildings.id'), nullable=True)
    x = Column(Integer, nullable=False)
    y = Column(Integer, nullable=False)

    # Capacidade
    platform_count = Column(Integer, default=1)
    max_queue_length = Column(Integer, default=100)
    current_queue_length = Column(Integer, default=0)

    # Status
    is_active = Column(Boolean, default=True)

    # Timestamps
    created_at = Column(DateTime, default=datetime.utcnow)
    updated_at = Column(DateTime, default=datetime.utcnow, onupdate=datetime.utcnow)

    def __repr__(self):
        return f"<Stop(id={self.id}, name='{self.name}', type='{self.stop_type}')>"


# Modelo: Resource / Good / Cargo
class Resource(Base):
    """Recursos e bens que circulam na economia."""
    __tablename__ = 'resources'

    id = Column(GUID(), primary_key=True, default=uuid.uuid4)
    name = Column(String(100), nullable=False, unique=True)
    description = Column(Text, nullable=True)

    # Propriedades físicas
    unit = Column(String(20), default='unit', comment="kg, liter, unit, etc")
    base_price = Column(DECIMAL(10, 2), default=1.00)
    weight = Column(Float, default=1.0, comment="Peso em kg por unidade")

    # Características
    perishability = Column(Float, default=0.0, comment="0-1, taxa de deterioração")
    category = Column(String(50), comment="food, material, fuel, tool, luxury, etc")

    # Status
    is_tradeable = Column(Boolean, default=True)

    # Timestamps
    created_at = Column(DateTime, default=datetime.utcnow)

    def __repr__(self):
        return f"<Resource(id={self.id}, name='{self.name}', price={self.base_price})>"


# Modelo: Inventory / Stock
class Inventory(Base):
    """Inventário de recursos em buildings, vehicles ou companies."""
    __tablename__ = 'inventory'

    id = Column(GUID(), primary_key=True, default=uuid.uuid4)

    # Polimórfico: pode pertencer a building, vehicle ou company
    owner_type = Column(String(50), nullable=False, comment="building, vehicle, company")
    owner_id = Column(GUID(), nullable=False)

    # Recurso
    resource_id = Column(GUID(), ForeignKey('resources.id'), nullable=False)

    # Quantidade
    quantity = Column(Float, default=0.0)
    reserved_quantity = Column(Float, default=0.0, comment="Quantidade reservada para transações")

    # Timestamps
    last_updated = Column(DateTime, default=datetime.utcnow, onupdate=datetime.utcnow)

    __table_args__ = (
        CheckConstraint('quantity >= 0', name='check_inventory_positive'),
        CheckConstraint('reserved_quantity >= 0', name='check_reserved_positive'),
        CheckConstraint('reserved_quantity <= quantity', name='check_reserved_valid'),
    )

    def __repr__(self):
        return f"<Inventory(owner_type='{self.owner_type}', resource_id={self.resource_id}, qty={self.quantity})>"


# Modelo: MaintenanceRecord
class MaintenanceRecord(Base):
    """Registros de manutenção de veículos e infraestrutura."""
    __tablename__ = 'maintenance_records'

    id = Column(GUID(), primary_key=True, default=uuid.uuid4)

    # Polimórfico: pode ser veículo ou building
    target_type = Column(String(50), nullable=False, comment="vehicle, building")
    target_id = Column(GUID(), nullable=False)

    # Detalhes da manutenção
    maintenance_type = Column(String(50), nullable=False, comment="preventive, corrective, emergency")
    description = Column(Text, nullable=True)

    # Quem realizou
    performed_by_agent_id = Column(GUID(), ForeignKey('agents.id'), nullable=True)
    performed_by_company_id = Column(GUID(), ForeignKey('companies.id'), nullable=True)

    # Custos e tempo
    cost = Column(DECIMAL(10, 2), default=0.00)
    downtime_hours = Column(Float, default=0.0)

    # Timestamps
    scheduled_at = Column(DateTime, nullable=True)
    started_at = Column(DateTime, nullable=True)
    completed_at = Column(DateTime, nullable=True)
    created_at = Column(DateTime, default=datetime.utcnow)

    def __repr__(self):
        return f"<MaintenanceRecord(id={self.id}, type='{self.maintenance_type}', cost={self.cost})>"


# Modelo: Schedule / Timetable (Issue 4.9)
class Schedule(Base):
    """
    Horários de veículos em rotas (Issue 4.9).

    Representa a programação de saídas de veículos em rotas específicas,
    incluindo horário de partida e dias da semana de operação.
    """
    __tablename__ = 'schedules'

    # ==================== IDENTIFICAÇÃO ====================
    id = Column(GUID(), primary_key=True, default=uuid.uuid4)

    # ==================== RELACIONAMENTOS ====================
    route_id = Column(GUID(), ForeignKey('routes.id', ondelete='CASCADE'), nullable=False, index=True)
    vehicle_id = Column(GUID(), ForeignKey('vehicles.id', ondelete='CASCADE'), nullable=False, index=True)

    # ==================== HORÁRIO ====================
    departure_time = Column(Time, nullable=False, comment="Horário de partida (HH:MM:SS)")

    # ==================== DIAS DA SEMANA ====================
    # Array de inteiros [0-6] onde 0=Monday, 6=Sunday (compatível com datetime.weekday())
    days_of_week = Column(
        JSON,
        nullable=False,
        default=lambda: [0, 1, 2, 3, 4],  # Segunda a sexta por padrão
        comment="Array de dias da semana [0=Mon, 1=Tue, ..., 6=Sun]"
    )

    # ==================== STATUS ====================
    is_active = Column(Boolean, default=True, index=True, comment="Se o horário está ativo")

    # ==================== TIMESTAMPS ====================
    created_at = Column(DateTime, default=datetime.utcnow, nullable=False)
    updated_at = Column(DateTime, default=datetime.utcnow, onupdate=datetime.utcnow)

    # ==================== RELACIONAMENTOS ORM ====================
    route = relationship("Route", back_populates="schedules")
    vehicle = relationship("Vehicle", back_populates="schedules")

    # ==================== CONSTRAINTS ====================
    __table_args__ = (
        Index('idx_schedule_route_departure', 'route_id', 'departure_time'),
        Index('idx_schedule_vehicle', 'vehicle_id'),
        Index('idx_schedule_active', 'is_active'),
    )

    # ==================== MÉTODOS ====================

    def is_active_today(self, target_date: Optional[datetime] = None) -> bool:
        """
        Verifica se o horário está ativo no dia especificado.

        Args:
            target_date: Data para verificar (default: hoje)

        Returns:
            True se o horário está ativo e opera no dia da semana especificado
        """
        if not self.is_active:
            return False

        if target_date is None:
            target_date = datetime.utcnow()

        weekday = target_date.weekday()  # 0=Monday, 6=Sunday
        return weekday in self.days_of_week

    def get_next_departure(self, from_datetime: Optional[datetime] = None) -> Optional[datetime]:
        """
        Calcula a próxima data/hora de partida a partir de um momento.

        Args:
            from_datetime: Momento de referência (default: agora)

        Returns:
            DateTime da próxima partida ou None se não houver
        """
        if not self.is_active:
            return None

        if from_datetime is None:
            from_datetime = datetime.utcnow()

        if not self.days_of_week:
            return None

        # Buscar próxima ocorrência nos próximos 7 dias
        for day_offset in range(8):  # Verificar hoje + próximos 7 dias
            check_date = from_datetime + timedelta(days=day_offset)
            weekday = check_date.weekday()

            if weekday in self.days_of_week:
                # Combinar data com horário de partida
                next_departure = datetime.combine(
                    check_date.date(),
                    self.departure_time
                )

                # Se for hoje, verificar se já passou
                if day_offset == 0 and next_departure <= from_datetime:
                    continue

                return next_departure

        return None

    def __repr__(self):
        days_str = ','.join(str(d) for d in sorted(self.days_of_week)) if self.days_of_week else 'none'
        return f"<Schedule(id={self.id}, route_id={self.route_id}, vehicle_id={self.vehicle_id}, time={self.departure_time}, days=[{days_str}])>"


# Modelo: Tile / MapCell / Location
class Tile(Base):
    """Malha espacial da maquete (grid do mapa)."""
    __tablename__ = 'tiles'

    id = Column(GUID(), primary_key=True, default=uuid.uuid4)

    # Coordenadas
    x = Column(Integer, nullable=False)
    y = Column(Integer, nullable=False)
    z = Column(Integer, default=0, comment="Altura/andar")

    # Tipo de terreno
    terrain_type = Column(String(50), default='grass', comment="grass, water, road, rail, etc")

    # Região/zona
    region_id = Column(GUID(), nullable=True, comment="Referência a uma região/bairro")

    # Propriedades
    is_buildable = Column(Boolean, default=True)
    is_walkable = Column(Boolean, default=True)
    capacity = Column(Integer, default=1, comment="Quantos agentes/objetos podem ocupar")

    # Ocupação atual
    current_occupancy = Column(Integer, default=0)

    # Timestamps
    created_at = Column(DateTime, default=datetime.utcnow)

    __table_args__ = (
        CheckConstraint('current_occupancy >= 0', name='check_tile_occupancy_positive'),
        CheckConstraint('current_occupancy <= capacity', name='check_tile_capacity'),
    )

    def __repr__(self):
        return f"<Tile(x={self.x}, y={self.y}, terrain='{self.terrain_type}')>"


# Modelo: SensorEvent / IoTReading
class SensorEvent(Base):
    """Leituras de sensores (reed switch, luz, presença, etc)."""
    __tablename__ = 'sensor_events'

    id = Column(GUID(), primary_key=True, default=uuid.uuid4)
    sensor_id = Column(String(100), nullable=False, index=True)
    sensor_type = Column(String(50), nullable=False, comment="reed_switch, light, presence, temperature, etc")

    # Leitura
    reading_type = Column(String(50), nullable=False)
    value = Column(String(200), nullable=False)
    unit = Column(String(20), nullable=True)

    # Processamento
    processed = Column(Boolean, default=False)
    processed_at = Column(DateTime, nullable=True)

    # Contexto
    context = Column(JSON, default=lambda: {}, comment="Dados adicionais sobre a leitura")

    # Timestamps
    timestamp = Column(DateTime, default=datetime.utcnow, index=True)

    def __repr__(self):
        return f"<SensorEvent(sensor_id='{self.sensor_id}', type='{self.sensor_type}', value='{self.value}')>"


# Enums para LogEntry
class LogLevel(str, enum.Enum):
    """Níveis de log."""
    DEBUG = 'debug'
    INFO = 'info'
    WARNING = 'warning'
    ERROR = 'error'
    CRITICAL = 'critical'


# Modelo: LogEntry / AuditTrail
class LogEntry(Base):
    """Logs de sistema importantes (auditoria e debugging)."""
    __tablename__ = 'log_entries'

    id = Column(GUID(), primary_key=True, default=uuid.uuid4)
    level = Column(SQLEnum(LogLevel), default=LogLevel.INFO, index=True)
    message = Column(Text, nullable=False)

    # Contexto
    context = Column(JSON, default=lambda: {}, comment="Dados contextuais do log")
    source = Column(String(100), nullable=True, comment="Serviço/módulo de origem")

    # Relacionamentos opcionais
    actor_id = Column(GUID(), ForeignKey('agents.id'), nullable=True)

    # Timestamps
    created_at = Column(DateTime, default=datetime.utcnow, index=True)

    def __repr__(self):
        return f"<LogEntry(level='{self.level.value}', message='{self.message[:50]}...')>"


class StationType(str, enum.Enum):
    """Tipos de estações e pontos de parada de transporte.

    Focado em transporte público coletivo de uma cidade socialista/comunista.
    Enfatiza integração modal, acessibilidade universal e transporte de massas.

    Nota: Edifícios maiores (estações centrais, terminais) devem usar BuildingType.
    Este enum representa pontos de parada e infraestrutura de menor escala.
    """

    # ==================== TRANSPORTE FERROVIÁRIO ====================
    TRAIN_PLATFORM = "train_platform"
    TRAIN_STOP_LOCAL = "train_stop_local"
    TRAIN_STOP_EXPRESS = "train_stop_express"
    TRAIN_STOP_SUBURBAN = "train_stop_suburban"
    TRAIN_STOP_INTERURBAN = "train_stop_interurban"
    TRAIN_PLATFORM_ELEVATED = "train_platform_elevated"
    TRAIN_PLATFORM_UNDERGROUND = "train_platform_underground"

    METRO_STATION = "metro_station"
    METRO_PLATFORM = "metro_platform"
    METRO_PLATFORM_ISLAND = "metro_platform_island"
    METRO_PLATFORM_SIDE = "metro_platform_side"
    METRO_TRANSFER_CORRIDOR = "metro_transfer_corridor"

    LIGHT_RAIL_STOP = "light_rail_stop"
    LIGHT_RAIL_PLATFORM = "light_rail_platform"
    MONORAIL_STATION = "monorail_station"
    TRAM_STOP = "tram_stop"
    TRAM_STOP_PROTECTED = "tram_stop_protected"
    TRAM_PLATFORM = "tram_platform"

    # ==================== TRANSPORTE RODOVIÁRIO ====================
    BUS_STOP_SIMPLE = "bus_stop_simple"
    BUS_STOP_SHELTER = "bus_stop_shelter"
    BUS_STOP_TUBE = "bus_stop_tube"
    BUS_STOP_COVERED = "bus_stop_covered"
    BUS_STOP_HEATED = "bus_stop_heated"
    BUS_STOP_ARTICULATED = "bus_stop_articulated"
    BUS_STATION_LOCAL = "bus_station_local"
    BUS_STATION_DISTRICT = "bus_station_district"
    BUS_BAY = "bus_bay"
    BRT_STATION = "brt_station"
    BRT_PLATFORM_CENTER = "brt_platform_center"
    BRT_PLATFORM_SIDE = "brt_platform_side"
    TROLLEYBUS_STOP = "trolleybus_stop"
    TROLLEYBUS_TURNAROUND = "trolleybus_turnaround"

    # ==================== INTEGRAÇÃO / TERMINAIS ====================
    INTEGRATION_PLATFORM = "integration_platform"
    TRANSFER_POINT = "transfer_point"
    TRANSFER_POINT_COVERED = "transfer_point_covered"
    MULTIMODAL_HUB = "multimodal_hub"
    MULTIMODAL_PLATFORM = "multimodal_platform"
    PARK_AND_RIDE = "park_and_ride"
    BIKE_AND_RIDE = "bike_and_ride"
    KISS_AND_RIDE = "kiss_and_ride"
    INTERMODAL_EXCHANGE = "intermodal_exchange"

    # ==================== TRANSPORTE COLETIVO ESPECIAL ====================
    SHARED_VAN_STOP = "shared_van_stop"
    COLLECTIVE_TRANSPORT_POINT = "collective_transport_point"
    WORKERS_SHUTTLE_STOP = "workers_shuttle_stop"
    SCHOOL_BUS_STOP = "school_bus_stop"
    MOBILITY_SERVICE_POINT = "mobility_service_point"

    # ==================== VEÍCULOS DE CARGA ====================
    LOADING_DOCK = "loading_dock"
    LOADING_DOCK_COVERED = "loading_dock_covered"
    TRUCK_STOP = "truck_stop"
    TRUCK_REST_AREA = "truck_rest_area"
    CARGO_PLATFORM = "cargo_platform"
    CARGO_PLATFORM_REFRIGERATED = "cargo_platform_refrigerated"
    FREIGHT_SIDING = "freight_siding"
    FREIGHT_YARD = "freight_yard"
    CONTAINER_TRANSFER_POINT = "container_transfer_point"
    BULK_LOADING_FACILITY = "bulk_loading_facility"

    # ==================== ESTACIONAMENTOS ====================
    PARKING_SURFACE = "parking_surface"
    PARKING_UNDERGROUND = "parking_underground"
    PARKING_STRUCTURE = "parking_structure"
    PARKING_VALET = "parking_valet"
    PARKING_DISABLED = "parking_disabled"
    PARKING_MOTORCYCLE = "parking_motorcycle"
    PARKING_BICYCLE = "parking_bicycle"
    PARKING_BICYCLE_COVERED = "parking_bicycle_covered"
    PARKING_BICYCLE_SECURE = "parking_bicycle_secure"
    PARKING_ELECTRIC_CHARGING = "parking_electric_charging"
    PARKING_SOLAR_CHARGING = "parking_solar_charging"
    PARKING_CARPOOL = "parking_carpool"
    PARKING_CARGO_BIKE = "parking_cargo_bike"

    # ==================== SERVIÇOS DE EMERGÊNCIA ====================
    AMBULANCE_STATION = "ambulance_station"
    FIRE_TRUCK_BAY = "fire_truck_bay"
    EMERGENCY_VEHICLE_BAY = "emergency_vehicle_bay"
    CIVIL_DEFENSE_GARAGE = "civil_defense_garage"

    # ==================== TRANSPORTE AQUÁTICO (FUTURO/VIRTUAL) ====================
    FERRY_PIER = "ferry_pier"
    FERRY_TERMINAL = "ferry_terminal"
    BOAT_DOCK = "boat_dock"
    WATER_BUS_STOP = "water_bus_stop"
    RIVER_STATION = "river_station"
    CANAL_LOCK = "canal_lock"

    # ==================== TRANSPORTE ALTERNATIVO ====================
    CABLE_CAR_STATION = "cable_car_station"
    CABLE_CAR_TOWER = "cable_car_tower"
    FUNICULAR_STATION = "funicular_station"
    FUNICULAR_PLATFORM_UPPER = "funicular_platform_upper"
    FUNICULAR_PLATFORM_LOWER = "funicular_platform_lower"
    ELEVATOR_STATION = "elevator_station"
    PEOPLE_MOVER = "people_mover"
    ESCALATOR_STATION = "escalator_station"
    MOVING_WALKWAY = "moving_walkway"

    # ==================== MICROMODILIDADE ====================
    SCOOTER_DOCK = "scooter_dock"
    SCOOTER_PARKING = "scooter_parking"
    BIKESHARE_STATION = "bikeshare_station"
    BIKESHARE_DOCK = "bikeshare_dock"
    CARGO_BIKE_STATION = "cargo_bike_station"
    WHEELCHAIR_CHARGING = "wheelchair_charging"

    # ==================== INFRAESTRUTURA DE APOIO ====================
    TICKET_BOOTH = "ticket_booth"
    TICKET_MACHINE = "ticket_machine"
    TURNSTILE_GATE = "turnstile_gate"
    ACCESS_GATE = "access_gate"
    WAITING_AREA = "waiting_area"
    WAITING_ROOM = "waiting_room"
    WAITING_SHELTER = "waiting_shelter"
    INFORMATION_KIOSK = "information_kiosk"
    INFORMATION_BOARD = "information_board"
    HELP_POINT = "help_point"
    LOST_AND_FOUND = "lost_and_found"

    # ==================== ACESSIBILIDADE ====================
    ACCESSIBLE_PLATFORM = "accessible_platform"
    ACCESSIBLE_RAMP = "accessible_ramp"
    ACCESSIBLE_ELEVATOR = "accessible_elevator"
    TACTILE_PATH = "tactile_path"
    AUDIO_BEACON = "audio_beacon"
    BRAILLE_SIGNAGE = "braille_signage"

    # ==================== MANUTENÇÃO ====================
    VEHICLE_WASH = "vehicle_wash"
    VEHICLE_WASH_AUTOMATED = "vehicle_wash_automated"
    FUEL_PUMP = "fuel_pump"
    FUEL_DEPOT = "fuel_depot"
    CHARGING_STATION = "charging_station"
    FAST_CHARGING_STATION = "fast_charging_station"
    HYDROGEN_STATION = "hydrogen_station"
    MAINTENANCE_BAY = "maintenance_bay"
    MAINTENANCE_PIT = "maintenance_pit"
    INSPECTION_POINT = "inspection_point"
    REPAIR_SHOP = "repair_shop"
    TIRE_SERVICE = "tire_service"
    SPARE_PARTS_DEPOT = "spare_parts_depot"

    # ==================== INFRAESTRUTURA DUTOS ====================
    PNEUMATIC_TUBE_STATION = "pneumatic_tube_station"
    PIPELINE_JUNCTION = "pipeline_junction"
    PIPELINE_PUMP_STATION = "pipeline_pump_station"
    PIPELINE_VALVE_STATION = "pipeline_valve_station"
    PNEUMATIC_TERMINAL = "pneumatic_terminal"
    PNEUMATIC_DISTRIBUTION_HUB = "pneumatic_distribution_hub"
    PIPELINE_LOADING_BAY = "pipeline_loading_bay"
    PIPELINE_UNLOADING_BAY = "pipeline_unloading_bay"
    VACUUM_WASTE_INLET = "vacuum_waste_inlet"
    VACUUM_WASTE_COLLECTION = "vacuum_waste_collection"

    # ==================== LOGÍSTICA INTEGRADA ====================
    CARGO_CONSOLIDATION_POINT = "cargo_consolidation_point"
    DISTRIBUTION_CENTER_DOCK = "distribution_center_dock"
    CROSS_DOCK_FACILITY = "cross_dock_facility"
    SORTING_FACILITY = "sorting_facility"
    PACKAGE_LOCKER = "package_locker"
    DELIVERY_POINT = "delivery_point"
    COLLECTION_POINT = "collection_point"

    # ==================== HISTÓRICO / ESPECIAL ====================
    HORSE_CARRIAGE_STAND = "horse_carriage_stand"
    RICKSHAW_STAND = "rickshaw_stand"
    STEAM_TRAIN_STOP = "steam_train_stop"
    HERITAGE_STATION = "heritage_station"
    HERITAGE_TRAM_STOP = "heritage_tram_stop"
    MUSEUM_RAILWAY_STOP = "museum_railway_stop"

    # ==================== CONTROLE E OPERAÇÃO ====================
    SIGNAL_BOX = "signal_box"
    CONTROL_TOWER = "control_tower"
    SWITCHING_STATION = "switching_station"
    DEPOT_ENTRANCE = "depot_entrance"
    DEPOT_EXIT = "depot_exit"
    TURNAROUND_LOOP = "turnaround_loop"
    REVERSING_POINT = "reversing_point"
    LAYOVER_AREA = "layover_area"

    # ==================== FUTURO / EXPERIMENTAL ====================
    HYPERLOOP_POD = "hyperloop_pod"
    MAGLEV_STATION = "maglev_station"
    UNDERGROUND_FREIGHT = "underground_freight"
    AUTOMATED_GUIDEWAY = "automated_guideway"
    CARGO_ROBOT_DOCK = "cargo_robot_dock"
    AUTOMATED_PARCEL_HUB = "automated_parcel_hub"


class StationStatus(str, enum.Enum):
    """Status operacional da estação."""
    ACTIVE = "active"  # Em operação normal
    INACTIVE = "inactive"  # Fora de operação
    MAINTENANCE = "maintenance"  # Em manutenção
    CONSTRUCTION = "construction"  # Em construção
    CLOSED_TEMPORARILY = "closed_temporarily"  # Fechada temporariamente
    OVERCROWDED = "overcrowded"  # Superlotada
    EMERGENCY = "emergency"  # Situação de emergência


class Station(Base):
    """Estações e pontos de parada de transporte.

    Representa infraestrutura de transporte de menor escala que não
    constitui um edifício completo (usar Building para terminais grandes).
    """
    __tablename__ = 'stations'

    # ==================== IDENTIFICAÇÃO ====================
    id = Column(GUID(), primary_key=True, default=uuid.uuid4)
    name = Column(String(200), nullable=False, index=True)
    station_type = Column(SQLEnum(StationType), nullable=False, index=True)
    code = Column(String(20), unique=True, nullable=True, comment="Código alfanumérico (ex: M01, B234)")

    # ==================== LOCALIZAÇÃO ====================
    x = Column(Integer, nullable=False, index=True)
    y = Column(Integer, nullable=False, index=True)
    z = Column(Integer, default=0, comment="Nível/andar (0=térreo, -1=subsolo, 1=elevado)")
    tile_id = Column(GUID(), ForeignKey('tiles.id'), nullable=True)

    # Relacionamento com edifício (se estação está dentro/anexa a um prédio)
    building_id = Column(GUID(), ForeignKey('buildings.id'), nullable=True)

    # ==================== CAPACIDADE ====================
    platform_count = Column(Integer, default=1, comment="Número de plataformas/docas")
    max_queue_length = Column(Integer, default=50, comment="Capacidade máxima de fila")
    current_queue_length = Column(Integer, default=0, comment="Pessoas aguardando atualmente")
    max_simultaneous_vehicles = Column(Integer, default=1, comment="Veículos simultâneos")

    # ==================== OPERAÇÃO ====================
    status = Column(SQLEnum(StationStatus), default=StationStatus.ACTIVE, index=True)
    is_operational = Column(Boolean, default=True)
    serves_passengers = Column(Boolean, default=True)
    serves_cargo = Column(Boolean, default=False)

    # Horário de operação (JSON: {"monday": ["06:00-23:00"], ...})
    operating_hours = Column(JSON, default=lambda: {}, comment="Horários por dia da semana")

    # ==================== ACESSIBILIDADE ====================
    is_accessible = Column(Boolean, default=False, comment="Acessível para PCD")
    has_elevator = Column(Boolean, default=False)
    has_escalator = Column(Boolean, default=False)
    has_ramp = Column(Boolean, default=False)
    has_tactile_paving = Column(Boolean, default=False)
    has_audio_announcements = Column(Boolean, default=False)
    has_braille_signage = Column(Boolean, default=False)

    # ==================== COMODIDADES ====================
    has_shelter = Column(Boolean, default=False, comment="Cobertura/abrigo")
    has_heating = Column(Boolean, default=False, comment="Aquecimento")
    has_cooling = Column(Boolean, default=False, comment="Climatização")
    has_seating = Column(Boolean, default=False)
    seating_capacity = Column(Integer, default=0)
    has_lighting = Column(Boolean, default=True)
    has_cctv = Column(Boolean, default=False)
    has_wifi = Column(Boolean, default=False)
    has_restrooms = Column(Boolean, default=False)
    has_drinking_fountain = Column(Boolean, default=False)

    # ==================== INTEGRAÇÃO MODAL ====================
    connects_to_stations = Column(JSON, default=lambda: [], comment="IDs de estações conectadas")
    transfer_time_minutes = Column(Integer, default=5, comment="Tempo médio de transferência")
    allows_bike_parking = Column(Boolean, default=False)
    bike_parking_capacity = Column(Integer, default=0)
    allows_vehicle_parking = Column(Boolean, default=False)
    vehicle_parking_capacity = Column(Integer, default=0)

    # ==================== TARIFAÇÃO ====================
    requires_ticket = Column(Boolean, default=True)
    has_ticket_machine = Column(Boolean, default=False)
    has_ticket_booth = Column(Boolean, default=False)
    has_turnstile = Column(Boolean, default=False)
    fare_zone = Column(Integer, default=1, comment="Zona tarifária")

    # ==================== ESTATÍSTICAS ====================
    daily_passenger_avg = Column(Integer, default=0, comment="Média diária de passageiros")
    daily_passenger_peak = Column(Integer, default=0, comment="Pico diário")
    total_passengers_served = Column(Integer, default=0, comment="Total histórico")
    last_passenger_count_at = Column(DateTime, nullable=True)

    # Carga (se aplicável)
    daily_cargo_avg_kg = Column(Float, default=0.0)
    total_cargo_handled_kg = Column(Float, default=0.0)

    # ==================== MANUTENÇÃO ====================
    condition_value = Column(Integer, default=100, comment="Condição geral (0-100)")
    last_maintenance_at = Column(DateTime, nullable=True)
    next_maintenance_at = Column(DateTime, nullable=True)
    maintenance_interval_days = Column(Integer, default=90)
    maintenance_cost_total = Column(DECIMAL(10, 2), default=0.00)

    # ==================== ENERGIA E SUSTENTABILIDADE ====================
    energy_consumption_kwh_month = Column(Float, default=0.0)
    has_solar_panels = Column(Boolean, default=False)
    has_rainwater_harvesting = Column(Boolean, default=False)
    has_green_roof = Column(Boolean, default=False)

    # ==================== VISUAL (IoT/Renderização) ====================
    has_led = Column(Boolean, default=False, comment="LED físico na maquete")
    led_pin = Column(Integer, nullable=True)
    model_id = Column(String(200), nullable=True, comment="Modelo 3D")
    color = Column(String(7), default="#3498DB", comment="Cor (hex)")
    icon = Column(String(50), nullable=True, comment="Ícone para mapa")

    # ==================== METADATA ====================
    operator_company_id = Column(GUID(), ForeignKey('companies.id'), nullable=True)
    construction_date = Column(DateTime, nullable=True)
    opened_at = Column(DateTime, nullable=True)
    closed_at = Column(DateTime, nullable=True)
    created_at = Column(DateTime, default=datetime.utcnow)
    updated_at = Column(DateTime, default=datetime.utcnow, onupdate=datetime.utcnow)
    tags = Column(JSON, default=lambda: [], comment="Tags customizadas")
    notes = Column(Text, default="")

    # ==================== CONSTRAINTS ====================
    __table_args__ = (
        Index('idx_station_location', 'x', 'y', 'z'),
        Index('idx_station_type_status', 'station_type', 'status'),
        CheckConstraint('platform_count >= 0', name='check_platform_count_positive'),
        CheckConstraint('max_queue_length >= 0', name='check_queue_length_positive'),
        CheckConstraint('current_queue_length >= 0', name='check_current_queue_positive'),
        CheckConstraint('current_queue_length <= max_queue_length', name='check_queue_within_max'),
        CheckConstraint('condition_value >= 0 AND condition_value <= 100', name='check_condition_range'),
    )

    # ==================== RELACIONAMENTOS ====================
    building = relationship('Building', foreign_keys=[building_id], back_populates='stations')
    operator_company = relationship('Company', foreign_keys=[operator_company_id])
    docked_vehicles = relationship('Vehicle', foreign_keys='Vehicle.current_station_id', back_populates='current_station')

    # ==================== VALIDAÇÕES ====================


    @validates('current_queue_length')
    def validate_queue(self, key, value):
        if value < 0:
            return 0
        if value > self.max_queue_length:
            return self.max_queue_length
        return value

    @validates('condition_value')
    def validate_condition(self, key, value):
        return max(0, min(100, value))

    # ==================== MÉTODOS DE NEGÓCIO ====================

    def is_overcrowded(self) -> bool:
        """Verifica se a estação está superlotada."""
        if self.max_queue_length == 0:
            return False
        return (self.current_queue_length / self.max_queue_length) > 0.9

    def can_accept_passengers(self, count: int = 1) -> bool:
        """Verifica se pode aceitar mais passageiros."""
        if not self.is_operational or not self.serves_passengers:
            return False
        return (self.current_queue_length + count) <= self.max_queue_length

    def add_to_queue(self, count: int = 1) -> bool:
        """Adiciona passageiros à fila."""
        if not self.can_accept_passengers(count):
            return False
        self.current_queue_length += count
        return True

    def remove_from_queue(self, count: int = 1) -> int:
        """Remove passageiros da fila. Retorna quantos foram removidos."""
        removed = min(count, self.current_queue_length)
        self.current_queue_length -= removed
        return removed

    def clear_queue(self):
        """Limpa completamente a fila."""
        self.current_queue_length = 0

    def board_vehicle(self, vehicle_capacity: int) -> int:
        """Embarca passageiros em um veículo. Retorna quantos embarcaram."""
        boarded = min(self.current_queue_length, vehicle_capacity)
        self.current_queue_length -= boarded
        self.total_passengers_served += boarded
        self.last_passenger_count_at = datetime.utcnow()
        return boarded

    def needs_maintenance(self) -> bool:
        """Verifica se precisa de manutenção."""
        if self.condition_value < 50:
            return True
        if self.next_maintenance_at and datetime.utcnow() >= self.next_maintenance_at:
            return True
        return False

    def perform_maintenance(self, cost: DECIMAL = None):
        """Realiza manutenção da estação."""
        self.condition_value = min(100, self.condition_value + 30)
        self.last_maintenance_at = datetime.utcnow()
        self.next_maintenance_at = datetime.utcnow() + timedelta(days=self.maintenance_interval_days)
        if cost:
            self.maintenance_cost_total += cost

    def degrade_condition(self, amount: int = 1):
        """Degrada a condição da estação."""
        self.condition_value = max(0, self.condition_value - amount)
        if self.condition_value < 20:
            self.status = StationStatus.MAINTENANCE
            self.is_operational = False

    def is_operating_now(self, current_datetime: datetime = None) -> bool:
        """Verifica se está operando no momento."""
        if not self.is_operational or self.status != StationStatus.ACTIVE:
            return False

        if not self.operating_hours:
            return True

        dt = current_datetime or datetime.utcnow()
        weekday = dt.strftime('%A').lower()

        if weekday not in self.operating_hours:
            return True

        current_time = dt.strftime('%H:%M')
        for time_range in self.operating_hours[weekday]:
            start, end = time_range.split('-')
            if start <= current_time <= end:
                return True

        return False

    def calculate_occupancy_rate(self) -> float:
        """Calcula taxa de ocupação atual."""
        if self.max_queue_length == 0:
            return 0.0
        return (self.current_queue_length / self.max_queue_length) * 100

    def get_connected_stations(self, session) -> List['Station']:
        """Retorna estações conectadas."""
        if not self.connects_to_stations:
            return []
        return session.query(Station).filter(Station.id.in_(self.connects_to_stations)).all()

    def add_connection(self, station_id: str):
        """Adiciona conexão com outra estação."""
        if station_id not in self.connects_to_stations:
            self.connects_to_stations.append(station_id)

    def remove_connection(self, station_id: str):
        """Remove conexão com outra estação."""
        if station_id in self.connects_to_stations:
            self.connects_to_stations.remove(station_id)

    def get_accessibility_score(self) -> int:
        """Calcula score de acessibilidade (0-100)."""
        score = 0
        if self.is_accessible:
            score += 20
        if self.has_elevator:
            score += 15
        if self.has_escalator:
            score += 10
        if self.has_ramp:
            score += 15
        if self.has_tactile_paving:
            score += 15
        if self.has_audio_announcements:
            score += 15
        if self.has_braille_signage:
            score += 10
        return min(100, score)

    def get_comfort_score(self) -> int:
        """Calcula score de conforto (0-100)."""
        score = 0
        if self.has_shelter:
            score += 20
        if self.has_seating:
            score += 15
        if self.has_heating:
            score += 15
        if self.has_cooling:
            score += 15
        if self.has_restrooms:
            score += 15
        if self.has_wifi:
            score += 10
        if self.has_drinking_fountain:
            score += 10
        return min(100, score)

    def estimate_wait_time(self, vehicles_per_hour: int) -> int:
        """Estima tempo de espera em minutos."""
        if vehicles_per_hour == 0:
            return 999
        return int(60 / vehicles_per_hour)

    # ==================== PROPRIEDADES CALCULADAS ====================

    @property
    def is_multimodal(self) -> bool:
        """Verifica se é estação multimodal."""
        return len(self.connects_to_stations) > 0

    @property
    def occupancy_percentage(self) -> float:
        """Taxa de ocupação em porcentagem."""
        return self.calculate_occupancy_rate()

    @property
    def queue_status(self) -> str:
        """Status descritivo da fila."""
        rate = self.occupancy_percentage
        if rate == 0:
            return "empty"
        elif rate < 30:
            return "low"
        elif rate < 60:
            return "moderate"
        elif rate < 90:
            return "high"
        else:
            return "critical"

    @property
    def age_days(self) -> Optional[int]:
        """Idade em dias desde construção."""
        if not self.construction_date:
            return None
        return (datetime.utcnow() - self.construction_date).days

    # ==================== MÉTODOS UTILITÁRIOS ====================

    def get_daily_stats(self) -> dict:
        """Retorna estatísticas diárias."""
        return {
            'avg_passengers': self.daily_passenger_avg,
            'peak_passengers': self.daily_passenger_peak,
            'total_served': self.total_passengers_served,
            'avg_cargo_kg': self.daily_cargo_avg_kg,
            'occupancy_rate': self.occupancy_percentage,
            'queue_status': self.queue_status
        }

    def get_status_summary(self) -> dict:
        """Resumo do status da estação."""
        return {
            'id': str(self.id),
            'name': self.name,
            'type': self.station_type.value,
            'status': self.status.value,
            'operational': self.is_operational,
            'condition': self.condition_value,
            'queue': self.current_queue_length,
            'max_queue': self.max_queue_length,
            'accessibility_score': self.get_accessibility_score(),
            'comfort_score': self.get_comfort_score()
        }

    def __repr__(self):
        return (f"<Station(id={self.id}, name='{self.name}', "
                f"type={self.station_type.value}, status={self.status.value}, "
                f"queue={self.current_queue_length}/{self.max_queue_length})>")

#todo Adicionar relacionamento reverso em Building (adicionar à classe Building existente): stations = relationship('Station', foreign_keys='Station.building_id', back_populates='building')
