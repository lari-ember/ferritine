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
    owned_buildings = relationship(
        "Building",
        foreign_keys="Building.owner_id",
        back_populates="owner"
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


# ============================================================================
# ENUMS PARA BUILDING (EDIFÍCIOS)
# ============================================================================

class BuildingType(str, enum.Enum):
    """Tipos detalhados de edifícios na cidade"""

    # ==================== RESIDENCIAIS ====================
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

    # ==================== COMERCIAIS ====================
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

    # ==================== INDUSTRIAIS ====================
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

    # ==================== PÚBLICOS ====================
    PUBLIC_SCHOOL_ELEMENTARY = "public_school_elementary"        # Escola fundamental
    PUBLIC_SCHOOL_HIGH = "public_school_high"                    # Colégio
    PUBLIC_UNIVERSITY = "public_university"                      # Universidade
    PUBLIC_LIBRARY = "public_library"                            # Biblioteca
    PUBLIC_HOSPITAL_SMALL = "public_hospital_small"              # Posto de saúde
    PUBLIC_HOSPITAL_LARGE = "public_hospital_large"              # Hospital geral
    PUBLIC_CLINIC = "public_clinic"                              # Clínica
    PUBLIC_CITY_HALL = "public_city_hall"                        # Prefeitura
    PUBLIC_COURTHOUSE = "public_courthouse"                      # Fórum
    PUBLIC_POLICE_STATION = "public_police_station"              # Delegacia
    PUBLIC_FIRE_STATION = "public_fire_station"                  # Corpo de bombeiros
    PUBLIC_POST_OFFICE = "public_post_office"                    # Correios
    PUBLIC_CEMETERY = "public_cemetery"                          # Cemitério
    PUBLIC_CHURCH = "public_church"                              # Igreja
    PUBLIC_TEMPLE = "public_temple"                              # Templo religioso
    PUBLIC_MONUMENT = "public_monument"                          # Monumento, estátua
    PUBLIC_MUSEUM = "public_museum"                              # Museu
    PUBLIC_ARCHIVE = "public_archive"                            # Arquivo histórico
    PUBLIC_ORPHANAGE = "public_orphanage"                        # Orfanato
    PUBLIC_NURSING_HOME = "public_nursing_home"                  # Asilo

    # ==================== TRANSPORTE ====================
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
    TRANSPORT_AIRPORT = "transport_airport"                      # Aeroporto (futuro)
    TRANSPORT_HELIPAD = "transport_helipad"                      # Heliponto (futuro)
    TRANSPORT_PORT = "transport_port"                            # Porto fluvial/marítimo

    # ==================== LAZER ====================
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
    LEISURE_ZOO = "leisure_zoo"                                  # Zoológico
    LEISURE_BOTANICAL_GARDEN = "leisure_botanical_garden"        # Jardim botânico
    LEISURE_AMUSEMENT_PARK = "leisure_amusement_park"            # Parque de diversões
    LEISURE_LAKE = "leisure_lake"                                # Lago recreativo
    LEISURE_BEACH = "leisure_beach"                              # Praia artificial

    # ==================== INFRAESTRUTURA ====================
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

    # ==================== ESPECIAIS ====================
    SPECIAL_RUINS = "special_ruins"                              # Ruínas (pós-demolição)
    SPECIAL_CONSTRUCTION_SITE = "special_construction_site"      # Canteiro de obras
    SPECIAL_EMPTY_LOT = "special_empty_lot"                      # Terreno vazio
    SPECIAL_FARM = "special_farm"                                # Fazenda (rural)
    SPECIAL_RANCH = "special_ranch"                              # Sítio
    SPECIAL_LANDMARK = "special_landmark"                        # Marco histórico único
    SPECIAL_PRISON = "special_prison"                            # Prisão
    SPECIAL_MILITARY_BASE = "special_military_base"              # Base militar (histórico)
    SPECIAL_LIGHTHOUSE = "special_lighthouse"                    # Farol (se houver costa)
    SPECIAL_DAM = "special_dam"                                  # Barragem


class BuildingStatus(str, enum.Enum):
    """Status detalhado de construção e operação"""

    # ==================== PLANEJAMENTO ====================
    PLANNING_PROPOSED = "planning_proposed"                      # Apenas proposta
    PLANNING_APPROVED = "planning_approved"                      # Aprovada, aguardando fundos
    PLANNING_FUNDED = "planning_funded"                          # Financiada, aguarda início

    # ==================== CONSTRUÇÃO ====================
    CONSTRUCTION_FOUNDATION = "construction_foundation"          # Fundação (0-25%)
    CONSTRUCTION_STRUCTURE = "construction_structure"            # Estrutura (25-50%)
    CONSTRUCTION_WALLS = "construction_walls"                    # Paredes (50-75%)
    CONSTRUCTION_FINISHING = "construction_finishing"            # Acabamento (75-99%)
    CONSTRUCTION_PAUSED = "construction_paused"                  # Obra paralisada
    CONSTRUCTION_DELAYED = "construction_delayed"                # Atraso (falta material/verba)

    # ==================== OPERAÇÃO ====================
    OPERATIONAL_NEW = "operational_new"                          # Recém-inaugurado
    OPERATIONAL_ACTIVE = "operational_active"                    # Funcionando normalmente
    OPERATIONAL_BUSY = "operational_busy"                        # Lotado (100%+ capacidade)
    OPERATIONAL_SLOW = "operational_slow"                        # Movimento fraco (<30%)
    OPERATIONAL_CLOSED_TEMPORARY = "operational_closed_temporary"  # Fechado temporariamente
    OPERATIONAL_NIGHT_SHIFT = "operational_night_shift"          # Operação noturna apenas
    OPERATIONAL_SEASONAL = "operational_seasonal"                # Sazonal (verão/inverno)

    # ==================== MANUTENÇÃO ====================
    MAINTENANCE_ROUTINE = "maintenance_routine"                  # Manutenção preventiva
    MAINTENANCE_EMERGENCY = "maintenance_emergency"              # Reparo urgente
    MAINTENANCE_RENOVATION = "maintenance_renovation"            # Reforma
    MAINTENANCE_EXPANSION = "maintenance_expansion"              # Ampliação
    MAINTENANCE_MODERNIZATION = "maintenance_modernization"      # Modernização

    # ==================== PROBLEMAS ====================
    DAMAGED_MINOR = "damaged_minor"                              # Danos leves (>80% funcional)
    DAMAGED_MODERATE = "damaged_moderate"                        # Danos moderados (40-80%)
    DAMAGED_SEVERE = "damaged_severe"                            # Danos severos (<40%)
    DAMAGED_STRUCTURAL = "damaged_structural"                    # Risco estrutural
    DAMAGED_FIRE = "damaged_fire"                                # Pós-incêndio
    DAMAGED_FLOOD = "damaged_flood"                              # Pós-enchente
    DAMAGED_EARTHQUAKE = "damaged_earthquake"                    # Pós-terremoto

    # ==================== DESATIVADO ====================
    ABANDONED_RECENT = "abandoned_recent"                        # Abandonado há <1 ano
    ABANDONED_OLD = "abandoned_old"                              # Abandonado há 1-5 anos
    ABANDONED_RUIN = "abandoned_ruin"                            # Ruína (>5 anos)
    CONDEMNED = "condemned"                                      # Interditado (perigo)
    HISTORICAL_PRESERVATION = "historical_preservation"          # Preservação histórica

    # ==================== DEMOLIÇÃO ====================
    DEMOLITION_SCHEDULED = "demolition_scheduled"                # Agendada para demolir
    DEMOLITION_IN_PROGRESS = "demolition_in_progress"            # Sendo demolido
    DEMOLISHED = "demolished"                                    # Demolido (terreno vazio)

    # ==================== EVENTOS ESPECIAIS ====================
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
    # Era 1 (1860-1920)
    COLONIAL_PORTUGUESE = "colonial_portuguese"
    VICTORIAN = "victorian"
    NEOCLASSICAL = "neoclassical"
    ECLETIC = "ecletic"

    # Era 2 (1920-1960)
    ART_DECO = "art_deco"
    MODERNIST = "modernist"
    BAUHAUS = "bauhaus"
    INDUSTRIAL = "industrial"

    # Era 3 (1960-2000)
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


# ============================================================================
# MODELO: BUILDING (EDIFÍCIO COMPLETO)
# ============================================================================

class Building(Base):
    """
    Edifícios da cidade com sistema detalhado.
    Suporta todos os tipos de construções com atributos complexos.
    """
    __tablename__ = 'buildings'

    # ==================== IDENTIFICAÇÃO ====================
    id = Column(GUID(), primary_key=True, default=uuid.uuid4)
    name = Column(String(200), nullable=False, index=True)
    building_type = Column(SQLEnum(BuildingType), nullable=False)

    # ==================== LOCALIZAÇÃO ====================
    x = Column(Integer, nullable=False, comment="Coordenada X no grid")
    y = Column(Integer, nullable=False, comment="Coordenada Y no grid")
    address = Column(String(200), default="")
    neighborhood = Column(String(100), default="")
    postal_code = Column(String(20), default="")
    zoning = Column(SQLEnum(BuildingZoning), default=BuildingZoning.MIXED_USE)

    # ==================== DIMENSÕES ====================
    width = Column(Float, default=10.0, comment="Largura em metros")
    length = Column(Float, default=10.0, comment="Comprimento em metros")
    height = Column(Float, default=5.0, comment="Altura em metros")
    floors = Column(Integer, default=1, comment="Número de andares")

    # ==================== STATUS E CONDIÇÃO ====================
    status = Column(SQLEnum(BuildingStatus), default=BuildingStatus.OPERATIONAL_ACTIVE)
    condition = Column(SQLEnum(BuildingCondition), default=BuildingCondition.GOOD)
    condition_value = Column(Integer, default=80, comment="0-100 (numérico)")
    __table_args__ = (
        CheckConstraint('condition_value >= 0 AND condition_value <= 100', name='check_building_condition_range'),
    )

    # ==================== PROPRIEDADE ====================
    owner_id = Column(GUID(), ForeignKey('agents.id'), nullable=True, comment="ID do agente dono")
    owner_type = Column(SQLEnum(BuildingOwnershipType), default=BuildingOwnershipType.PRIVATE_INDIVIDUAL)

    # ==================== ARQUITETURA ====================
    architecture_style = Column(SQLEnum(BuildingArchitectureStyle), default=BuildingArchitectureStyle.GENERIC)
    construction_year = Column(Integer, default=1900)
    era = Column(Integer, default=1, comment="1-4, baseado no ano")

    # ==================== CAPACIDADE E OCUPAÇÃO ====================
    max_occupancy = Column(Integer, default=10, comment="Pessoas simultâneas")
    current_occupancy = Column(Integer, default=0)
    units = Column(Integer, default=0, comment="Apartamentos/salas/leitos")
    parking_spaces = Column(Integer, default=0)

    # ==================== CONSTRUÇÃO ====================
    foundation_type = Column(String(50), default="concrete", comment="concreto, madeira, pedra")
    structure_type = Column(String(50), default="brick", comment="tijolo, concreto, aço, madeira")
    roof_type = Column(String(50), default="tile", comment="telha, laje, zinco")
    exterior_finish = Column(String(50), default="painted")
    interior_finish = Column(String(50), default="basic")

    # ==================== UTILIDADES ====================
    has_electricity = Column(Boolean, default=True)
    has_water = Column(Boolean, default=True)
    has_sewage = Column(Boolean, default=True)
    has_heating = Column(Boolean, default=False)
    has_ac = Column(Boolean, default=False)
    has_elevator = Column(Boolean, default=False)
    has_generator = Column(Boolean, default=False)

    # ==================== ACESSIBILIDADE E EXTRAS ====================
    wheelchair_accessible = Column(Boolean, default=False)
    has_garden = Column(Boolean, default=False)
    has_balcony = Column(Boolean, default=False)
    has_basement = Column(Boolean, default=False)
    has_attic = Column(Boolean, default=False)

    # ==================== ECONOMIA ====================
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

    # ==================== HISTÓRICO ====================
    construction_started = Column(DateTime, nullable=True)
    construction_completed = Column(DateTime, nullable=True)
    inauguration_date = Column(DateTime, nullable=True)
    last_renovation = Column(DateTime, nullable=True)
    last_inspection = Column(DateTime, nullable=True)
    major_events = Column(JSON, default=list, comment="Eventos importantes (incêndios, reformas, etc)")
    ownership_history = Column(JSON, default=list, comment="Mudanças de proprietário")
    renovations = Column(JSON, default=list, comment="Histórico de reformas")

    # ==================== MEIO AMBIENTE ====================
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

    # ==================== SEGURANÇA ====================
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

    # ==================== CONSTRUÇÃO EM ANDAMENTO ====================
    construction_progress = Column(Integer, default=100, comment="0-100%")
    construction_start_date = Column(DateTime, nullable=True)
    estimated_completion_date = Column(DateTime, nullable=True)

    # ==================== GAMEPLAY ====================
    happiness_modifier = Column(Float, default=0.0, comment="Bônus/penalidade para moradores")
    crime_rate = Column(Float, default=0.0, comment="0-1, taxa de criminalidade local")
    noise_complaints = Column(Integer, default=0)
    health_violations = Column(Integer, default=0)

    # ==================== VISUAL (para renderização) ====================
    texture_id = Column(String(200), nullable=True)
    model_id = Column(String(200), nullable=True)
    color = Column(String(7), default="#FFFFFF", comment="Cor principal (hex)")
    is_visible = Column(Boolean, default=True)

    # ==================== INTEGRAÇÃO FÍSICA (IoT) ====================
    has_led = Column(Boolean, default=False, comment="Tem LED físico na maquete?")
    led_pin = Column(Integer, nullable=True, comment="Pino do Arduino para controlar LED")

    # ==================== METADATA ====================
    created_at = Column(DateTime, default=datetime.utcnow, nullable=False)
    updated_at = Column(DateTime, default=datetime.utcnow, onupdate=datetime.utcnow)
    demolished_at = Column(DateTime, nullable=True, comment="Soft delete")
    tags = Column(JSON, default=list, comment="Ex: ['historic', 'landmark']")
    notes = Column(Text, default="", comment="Notas do jogador")

    # ==================== RELACIONAMENTOS ====================
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

    # ==================== MÉTODOS ====================

    def calculate_monthly_costs(self) -> float:
        """Calcula custos operacionais mensais totais"""
        return float(
            self.maintenance_cost +
            self.utility_costs +
            self.tax_property +
            self.insurance_cost
        )

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

