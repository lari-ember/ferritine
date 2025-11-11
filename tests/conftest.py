"""
Configuração de fixtures para testes do Ferritine.
"""
import pytest
from sqlalchemy import create_engine
from sqlalchemy.orm import sessionmaker
from backend.database.models import Base


@pytest.fixture(scope='function')
def db_session():
    """Cria uma sessão de banco de dados em memória para testes."""
    # Criar engine SQLite em memória
    engine = create_engine('sqlite:///:memory:', echo=False)
    
    # Criar todas as tabelas
    Base.metadata.create_all(engine)
    
    # Criar sessão
    Session = sessionmaker(bind=engine)
    session = Session()
    
    yield session
    
    # Cleanup
    session.close()
    Base.metadata.drop_all(engine)


@pytest.fixture(scope='function')
def db_session_postgresql():
    """
    Cria uma sessão de banco de dados PostgreSQL para testes.
    Requer PostgreSQL rodando localmente.
    """
    # Usar banco de teste
    engine = create_engine(
        'postgresql://postgres:postgres@localhost:5432/ferritine_test',
        echo=False
    )
    
    # Criar todas as tabelas
    Base.metadata.create_all(engine)
    
    # Criar sessão
    Session = sessionmaker(bind=engine)
    session = Session()
    
    yield session
    
    # Cleanup
    session.rollback()
    session.close()
    Base.metadata.drop_all(engine)

