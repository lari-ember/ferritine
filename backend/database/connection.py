"""
Gerenciamento de conexão com banco de dados PostgreSQL.
"""

import os
from sqlalchemy import create_engine
from sqlalchemy.orm import sessionmaker, scoped_session
from sqlalchemy.pool import QueuePool
from contextlib import contextmanager
from typing import Generator
import logging

from backend.database.models import Base

logger = logging.getLogger(__name__)


class DatabaseConfig:
    """Configuração do banco de dados."""
    
    def __init__(self):
        self.host = os.getenv('DB_HOST', 'localhost')
        self.port = os.getenv('DB_PORT', '5432')
        self.database = os.getenv('DB_NAME', 'ferritine')
        self.user = os.getenv('DB_USER', 'ferritine_user')
        self.password = os.getenv('DB_PASSWORD', 'ferritine_pass')
        self.echo = os.getenv('DB_ECHO', 'False').lower() == 'true'
        
    @property
    def url(self) -> str:
        """Retorna URL de conexão PostgreSQL."""
        return f"postgresql://{self.user}:{self.password}@{self.host}:{self.port}/{self.database}"
    
    @property
    def sqlite_url(self) -> str:
        """Retorna URL de conexão SQLite para desenvolvimento/testes."""
        db_path = os.getenv('SQLITE_PATH', 'data/db/ferritine.db')
        return f"sqlite:///{db_path}"


class DatabaseManager:
    """Gerenciador de conexões com banco de dados."""
    
    def __init__(self, config: DatabaseConfig = None, use_sqlite: bool = False):
        """
        Inicializa gerenciador de banco de dados.
        
        Args:
            config: Configuração do banco de dados
            use_sqlite: Se True, usa SQLite ao invés de PostgreSQL
        """
        self.config = config or DatabaseConfig()
        self.use_sqlite = use_sqlite
        self.engine = None
        self.session_factory = None
        self.Session = None
        
    def get_url(self) -> str:
        """Retorna URL de conexão apropriada."""
        if self.use_sqlite:
            return self.config.sqlite_url
        return self.config.url
    
    def create_engine(self):
        """Cria engine do SQLAlchemy."""
        url = self.get_url()
        
        engine_kwargs = {
            'echo': self.config.echo,
        }
        
        # Configurações específicas para PostgreSQL
        if not self.use_sqlite:
            engine_kwargs.update({
                'poolclass': QueuePool,
                'pool_size': 10,
                'max_overflow': 20,
                'pool_pre_ping': True,
                'pool_recycle': 3600,
            })
        
        self.engine = create_engine(url, **engine_kwargs)
        logger.info(f"Engine criado: {url}")
        return self.engine
    
    def create_session_factory(self):
        """Cria factory de sessões."""
        if not self.engine:
            self.create_engine()
        
        self.session_factory = sessionmaker(bind=self.engine)
        self.Session = scoped_session(self.session_factory)
        logger.info("Session factory criado")
        return self.Session
    
    def init_database(self):
        """Inicializa banco de dados criando todas as tabelas."""
        if not self.engine:
            self.create_engine()
        
        # Criar diretório se usando SQLite
        if self.use_sqlite:
            import pathlib
            db_path = self.config.sqlite_url.replace('sqlite:///', '')
            pathlib.Path(db_path).parent.mkdir(parents=True, exist_ok=True)
        
        Base.metadata.create_all(self.engine)
        logger.info("Banco de dados inicializado")
    
    def drop_all(self):
        """Remove todas as tabelas (CUIDADO!)."""
        if not self.engine:
            self.create_engine()
        
        Base.metadata.drop_all(self.engine)
        logger.warning("Todas as tabelas foram removidas")
    
    @contextmanager
    def session_scope(self) -> Generator:
        """
        Context manager para sessões de banco de dados.
        
        Uso:
            with db_manager.session_scope() as session:
                agent = session.query(Agent).first()
        """
        if not self.Session:
            self.create_session_factory()
        
        session = self.Session()
        try:
            yield session
            session.commit()
        except Exception as e:
            session.rollback()
            logger.error(f"Erro na sessão do banco de dados: {e}")
            raise
        finally:
            session.close()
    
    def get_session(self):
        """Retorna uma nova sessão."""
        if not self.Session:
            self.create_session_factory()
        return self.Session()
    
    def close(self):
        """Fecha conexões com banco de dados."""
        if self.Session:
            self.Session.remove()
        if self.engine:
            self.engine.dispose()
        logger.info("Conexões com banco de dados fechadas")


# Instância global do gerenciador (para uso comum)
db_manager = DatabaseManager()


# Funções de conveniência
def get_engine(use_sqlite: bool = False):
    """Retorna engine do banco de dados."""
    global db_manager
    if use_sqlite and not db_manager.use_sqlite:
        db_manager = DatabaseManager(use_sqlite=True)
    if not db_manager.engine:
        db_manager.create_engine()
    return db_manager.engine


def get_session():
    """Retorna uma nova sessão do banco de dados."""
    global db_manager
    return db_manager.get_session()


def init_database(use_sqlite: bool = False):
    """Inicializa banco de dados."""
    global db_manager
    if use_sqlite:
        db_manager = DatabaseManager(use_sqlite=True)
    db_manager.init_database()


@contextmanager
def session_scope():
    """Context manager para sessões."""
    global db_manager
    with db_manager.session_scope() as session:
        yield session

