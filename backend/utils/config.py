"""
Configuration module for Ferritine backend.

Provides centralized configuration management for the simulation engine,
database connections, API settings, and logging.

Configuration can be loaded from environment variables, config files, or set programmatically.
"""

import os
from pathlib import Path
from typing import Dict, Any


class Config:
    """Base configuration class with default settings."""

    # Project root directory
    PROJECT_ROOT = Path(__file__).parent.parent.parent

    # Data directories
    DATA_DIR = PROJECT_ROOT / "data"
    LOG_DIR = DATA_DIR / "logs"
    DB_DIR = DATA_DIR / "db"
    CONFIG_DIR = DATA_DIR / "config"

    # Simulation settings
    SIMULATION_STEP_DURATION = 0.1  # seconds
    SIMULATION_DEFAULT_HOURS = 24
    SIMULATION_DEBUG_MODE = os.getenv("DEBUG", "False").lower() == "true"

    # Database settings
    DATABASE_TYPE = os.getenv("DATABASE_TYPE", "sqlite")
    DATABASE_URL = os.getenv(
        "DATABASE_URL",
        f"sqlite:///{DB_DIR}/ferritine.db"
    )

    # API settings
    API_HOST = os.getenv("API_HOST", "0.0.0.0")
    API_PORT = int(os.getenv("API_PORT", "8000"))
    API_DEBUG = os.getenv("API_DEBUG", "False").lower() == "true"

    # Logging settings
    LOG_LEVEL = os.getenv("LOG_LEVEL", "INFO")
    LOG_FORMAT = "%(asctime)s - %(name)s - %(levelname)s - %(message)s"
    LOG_FILE = LOG_DIR / "ferritine.log"

    # Agent settings
    AGENT_MAX_SPEED = 5.0  # m/s
    AGENT_DEFAULT_HOME = "Casa"
    AGENT_DEFAULT_WORK = "Trabalho"
    AGENT_WORK_START_HOUR = 7
    AGENT_WORK_END_HOUR = 17

    @classmethod
    def get_config_dict(cls) -> Dict[str, Any]:
        """
        Return configuration as a dictionary.

        Returns:
            Dict[str, Any]: Configuration dictionary with all settings.
        """
        return {
            "project_root": str(cls.PROJECT_ROOT),
            "data_dir": str(cls.DATA_DIR),
            "log_dir": str(cls.LOG_DIR),
            "db_dir": str(cls.DB_DIR),
            "config_dir": str(cls.CONFIG_DIR),
            "simulation": {
                "step_duration": cls.SIMULATION_STEP_DURATION,
                "default_hours": cls.SIMULATION_DEFAULT_HOURS,
                "debug_mode": cls.SIMULATION_DEBUG_MODE,
            },
            "database": {
                "type": cls.DATABASE_TYPE,
                "url": cls.DATABASE_URL,
            },
            "api": {
                "host": cls.API_HOST,
                "port": cls.API_PORT,
                "debug": cls.API_DEBUG,
            },
            "logging": {
                "level": cls.LOG_LEVEL,
                "format": cls.LOG_FORMAT,
                "file": str(cls.LOG_FILE),
            },
            "agents": {
                "max_speed": cls.AGENT_MAX_SPEED,
                "default_home": cls.AGENT_DEFAULT_HOME,
                "default_work": cls.AGENT_DEFAULT_WORK,
                "work_start_hour": cls.AGENT_WORK_START_HOUR,
                "work_end_hour": cls.AGENT_WORK_END_HOUR,
            },
        }

    @classmethod
    def ensure_directories(cls) -> None:
        """Create necessary directories if they don't exist."""
        for directory in [cls.DATA_DIR, cls.LOG_DIR, cls.DB_DIR, cls.CONFIG_DIR]:
            directory.mkdir(parents=True, exist_ok=True)


class DevelopmentConfig(Config):
    """Configuration for development environment."""

    SIMULATION_DEBUG_MODE = True
    API_DEBUG = True
    LOG_LEVEL = "DEBUG"


class ProductionConfig(Config):
    """Configuration for production environment."""

    SIMULATION_DEBUG_MODE = False
    API_DEBUG = False
    LOG_LEVEL = "INFO"


class TestConfig(Config):
    """Configuration for testing environment."""

    SIMULATION_DEBUG_MODE = True
    DATABASE_URL = "sqlite:///:memory:"
    LOG_LEVEL = "DEBUG"


def get_config(env: str = None) -> Config:
    """
    Get configuration object based on environment.

    Args:
        env (str, optional): Environment name ('development', 'production', 'testing').
                            Defaults to FERRITINE_ENV environment variable or 'development'.

    Returns:
        Config: Configuration object for the specified environment.
    """
    if env is None:
        env = os.getenv("FERRITINE_ENV", "development").lower()

    config_map = {
        "development": DevelopmentConfig,
        "production": ProductionConfig,
        "testing": TestConfig,
        "test": TestConfig,
    }

    config_class = config_map.get(env, DevelopmentConfig)
    return config_class()


# Ensure directories exist on import
config = get_config()
config.ensure_directories()

