#!/usr/bin/env python3
"""
Script de inicialização e gerenciamento do banco de dados.
"""

import argparse
import sys
from pathlib import Path

# Adicionar diretório raiz ao path
sys.path.insert(0, str(Path(__file__).parent))

from backend.database.connection import DatabaseManager, DatabaseConfig
from backend.database.queries import DatabaseQueries
from backend.database.models import (
    Profession, NamePool, Gender,
    CreatedBy, Agent, Building
)
from datetime import datetime
from decimal import Decimal
import logging

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)


def init_database(use_sqlite=False):
    """Inicializa o banco de dados criando todas as tabelas."""
    logger.info("Inicializando banco de dados...")
    db_manager = DatabaseManager(use_sqlite=use_sqlite)
    db_manager.init_database()
    logger.info("✓ Banco de dados inicializado com sucesso!")
    return db_manager


def seed_professions(db_manager):
    """Popula banco com profissões iniciais."""
    logger.info("Populando profissões...")
    
    professions = [
        {
            "name": "Médico",
            "description": "Profissional de saúde",
            "base_salary": Decimal("8000.00"),
            "work_sector": "public",
            "required_skills": ["medicine", "empathy"]
        },
        {
            "name": "Professor",
            "description": "Educador",
            "base_salary": Decimal("4000.00"),
            "work_sector": "public",
            "required_skills": ["teaching", "patience"]
        },
        {
            "name": "Engenheiro",
            "description": "Engenheiro civil",
            "base_salary": Decimal("7000.00"),
            "work_sector": "industrial",
            "required_skills": ["engineering", "mathematics"]
        },
        {
            "name": "Vendedor",
            "description": "Profissional de vendas",
            "base_salary": Decimal("2500.00"),
            "work_sector": "commercial",
            "required_skills": ["communication", "persuasion"]
        },
        {
            "name": "Programador",
            "description": "Desenvolvedor de software",
            "base_salary": Decimal("6000.00"),
            "work_sector": "commercial",
            "required_skills": ["programming", "logic"]
        },
    ]
    
    with db_manager.session_scope() as session:
        queries = DatabaseQueries(session)
        for prof_data in professions:
            # Verificar se já existe
            existing = queries.professions.get_by_name(prof_data["name"])
            if not existing:
                queries.professions.create(**prof_data)
                logger.info(f"  + Profissão criada: {prof_data['name']}")
            else:
                logger.info(f"  - Profissão já existe: {prof_data['name']}")
    
    logger.info("✓ Profissões populadas!")


def seed_names(db_manager):
    """Popula banco com pool de nomes."""
    logger.info("Populando pool de nomes...")
    
    first_names_male = [
        ("João", 1.0), ("Pedro", 0.9), ("Lucas", 0.8), ("Gabriel", 0.7),
        ("Rafael", 0.6), ("Miguel", 0.5), ("Carlos", 0.4), ("André", 0.3)
    ]
    
    first_names_female = [
        ("Maria", 1.0), ("Ana", 0.9), ("Julia", 0.8), ("Beatriz", 0.7),
        ("Letícia", 0.6), ("Laura", 0.5), ("Camila", 0.4), ("Fernanda", 0.3)
    ]
    
    middle_names = [
        ("Silva", 0.2), ("Luiz", 0.15), ("Eduardo", 0.1), ("Paula", 0.1)
    ]
    
    last_names = [
        ("Silva", 1.0), ("Santos", 0.9), ("Oliveira", 0.8), ("Souza", 0.7),
        ("Lima", 0.6), ("Costa", 0.5), ("Pereira", 0.4), ("Rodrigues", 0.3),
        ("Almeida", 0.2), ("Nascimento", 0.15)
    ]
    
    with db_manager.session_scope() as session:
        queries = DatabaseQueries(session)
        
        # Nomes masculinos
        for name, rarity in first_names_male:
            queries.names.create(
                name=name,
                name_type="first",
                gender=Gender.MALE,
                rarity=rarity,
                origin="brasileiro"
            )
        
        # Nomes femininos
        for name, rarity in first_names_female:
            queries.names.create(
                name=name,
                name_type="first",
                gender=Gender.FEMALE,
                rarity=rarity,
                origin="brasileiro"
            )
        
        # Nomes do meio
        for name, rarity in middle_names:
            queries.names.create(
                name=name,
                name_type="middle",
                rarity=rarity,
                origin="brasileiro"
            )
        
        # Sobrenomes
        for name, rarity in last_names:
            queries.names.create(
                name=name,
                name_type="last",
                rarity=rarity,
                origin="brasileiro"
            )
        
        logger.info(f"  + {len(first_names_male)} nomes masculinos")
        logger.info(f"  + {len(first_names_female)} nomes femininos")
        logger.info(f"  + {len(middle_names)} nomes do meio")
        logger.info(f"  + {len(last_names)} sobrenomes")
    
    logger.info("✓ Pool de nomes populado!")


def seed_sample_data(db_manager):
    """Popula banco com dados de exemplo."""
    logger.info("Populando dados de exemplo...")
    
    with db_manager.session_scope() as session:
        queries = DatabaseQueries(session)
        
        # Criar alguns edifícios
        buildings = [
            Building(
                name="Casa Modelo 1",
                building_type="residential",
                x=0, y=0,
                capacity=4,
                rent_cost=Decimal("800.00")
            ),
            Building(
                name="Mercado Central",
                building_type="commercial",
                x=5, y=5,
                capacity=10,
                operating_cost=Decimal("2000.00")
            ),
        ]
        for building in buildings:
            session.add(building)
        session.flush()
        
        # Criar agente de exemplo
        profession = queries.professions.get_by_name("Programador")
        if profession:
            agent = Agent(
                name="João da Silva",
                created_by=CreatedBy.ADMIN,
                birth_date=datetime(1995, 5, 15),
                gender=Gender.MALE,
                profession_id=profession.id,
                home_building_id=buildings[0].id,
                wallet=Decimal("5000.00"),
                energy_level=80,
                version="0.1.0",
                skills={"programming": 85, "communication": 60},
                personality={"openness": 0.7, "conscientiousness": 0.8}
            )
            session.add(agent)
            logger.info(f"  + Agente criado: {agent.name}")
        
        logger.info(f"  + {len(buildings)} edifícios criados")
    
    logger.info("✓ Dados de exemplo populados!")


def drop_all(db_manager):
    """Remove todas as tabelas (CUIDADO!)."""
    logger.warning("⚠️  ATENÇÃO: Isto irá remover TODAS as tabelas do banco de dados!")
    confirm = input("Digite 'CONFIRMAR' para continuar: ")
    if confirm == "CONFIRMAR":
        db_manager.drop_all()
        logger.info("✓ Todas as tabelas foram removidas")
    else:
        logger.info("Operação cancelada")


def show_stats(db_manager):
    """Mostra estatísticas do banco de dados."""
    with db_manager.session_scope() as session:
        queries = DatabaseQueries(session)
        
        logger.info("\n=== Estatísticas do Banco de Dados ===")
        
        # Agentes
        agent_stats = queries.agents.get_statistics()
        logger.info(f"\nAgentes:")
        logger.info(f"  Total: {agent_stats['total']}")
        logger.info(f"  Carteira média: R$ {agent_stats['average_wallet']:.2f}")
        logger.info(f"  Por status: {agent_stats['by_status']}")
        logger.info(f"  Por saúde: {agent_stats['by_health']}")
        
        # Edifícios
        all_buildings = queries.buildings.get_all()
        logger.info(f"\nEdifícios:")
        logger.info(f"  Total: {len(all_buildings)}")
        
        # Profissões
        all_professions = queries.professions.get_all()
        logger.info(f"\nProfissões:")
        logger.info(f"  Total: {len(all_professions)}")
        
        logger.info("\n" + "="*40 + "\n")


def main():
    """Função principal."""
    parser = argparse.ArgumentParser(description="Gerenciador de banco de dados Ferritine")
    parser.add_argument(
        '--sqlite',
        action='store_true',
        help='Usar SQLite ao invés de PostgreSQL'
    )
    parser.add_argument(
        'action',
        choices=['init', 'seed', 'drop', 'stats'],
        help='Ação a executar'
    )
    
    args = parser.parse_args()
    
    db_manager = DatabaseManager(use_sqlite=args.sqlite)
    
    if args.action == 'init':
        init_database(args.sqlite)
    elif args.action == 'seed':
        db_manager.create_session_factory()
        seed_professions(db_manager)
        seed_names(db_manager)
        seed_sample_data(db_manager)
    elif args.action == 'drop':
        drop_all(db_manager)
    elif args.action == 'stats':
        db_manager.create_session_factory()
        show_stats(db_manager)
    
    db_manager.close()


if __name__ == "__main__":
    main()

