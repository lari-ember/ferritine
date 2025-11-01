"""
Backend database module - PostgreSQL persistence layer.
"""

from backend.database.models import (
    Base,
    Agent,
    Building,
    Vehicle,
    Event,
    EconomicStat,
    Profession,
    Routine,
    NamePool,
    CreatedBy,
    HealthStatus,
    AgentStatus,
    Gender,
)

from backend.database.connection import (
    DatabaseManager,
    DatabaseConfig,
    db_manager,
    get_engine,
    get_session,
    init_database,
    session_scope,
)

from backend.database.queries import (
    DatabaseQueries,
    AgentQueries,
    BuildingQueries,
    VehicleQueries,
    EventQueries,
    EconomicStatQueries,
    ProfessionQueries,
    NamePoolQueries,
)

__all__ = [
    # Models
    'Base',
    'Agent',
    'Building',
    'Vehicle',
    'Event',
    'EconomicStat',
    'Profession',
    'Routine',
    'NamePool',
    # Enums
    'CreatedBy',
    'HealthStatus',
    'AgentStatus',
    'Gender',
    # Connection
    'DatabaseManager',
    'DatabaseConfig',
    'db_manager',
    'get_engine',
    'get_session',
    'init_database',
    'session_scope',
    # Queries
    'DatabaseQueries',
    'AgentQueries',
    'BuildingQueries',
    'VehicleQueries',
    'EventQueries',
    'EconomicStatQueries',
    'ProfessionQueries',
    'NamePoolQueries',
]


