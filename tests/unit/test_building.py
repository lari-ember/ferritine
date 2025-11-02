"""
Testes unitários para o modelo Building.
Valida criação, relacionamentos e métodos de negócio.
"""

import pytest
from datetime import datetime
from decimal import Decimal
from sqlalchemy import create_engine
from sqlalchemy.orm import sessionmaker

from backend.database.models import (
    Base, Building, Agent, BuildingType, BuildingStatus,
    BuildingCondition, BuildingArchitectureStyle,
    BuildingOwnershipType, BuildingZoning
)


# Fixture: banco de dados em memória para testes
@pytest.fixture(scope="function")
def db_session():
    """Cria sessão de banco em memória para cada teste."""
    engine = create_engine("sqlite:///:memory:")
    Base.metadata.create_all(engine)
    Session = sessionmaker(bind=engine)
    session = Session()
    
    yield session
    
    session.close()
    Base.metadata.drop_all(engine)


class TestBuildingCreation:
    """Testes de criação de edifícios."""
    
    def test_create_basic_building(self, db_session):
        """Testa criação de edifício básico."""
        building = Building(
            name="Casa Teste",
            building_type=BuildingType.RESIDENTIAL_HOUSE_SMALL,
            x=10,
            y=20
        )
        
        db_session.add(building)
        db_session.commit()
        
        assert building.id is not None
        assert building.name == "Casa Teste"
        assert building.building_type == BuildingType.RESIDENTIAL_HOUSE_SMALL
        assert building.x == 10
        assert building.y == 20
    
    def test_create_building_with_defaults(self, db_session):
        """Testa valores padrão ao criar edifício."""
        building = Building(
            name="Casa Padrão",
            building_type=BuildingType.RESIDENTIAL_HOUSE_MEDIUM,
            x=0,
            y=0
        )
        
        db_session.add(building)
        db_session.commit()
        
        # Verificar defaults
        assert building.status == BuildingStatus.OPERATIONAL_ACTIVE
        assert building.condition == BuildingCondition.GOOD
        assert building.condition_value == 80
        assert building.max_occupancy == 10
        assert building.current_occupancy == 0
        assert building.construction_year == 1900
        assert building.era == 1
    
    def test_create_factory(self, db_session):
        """Testa criação de fábrica completa."""
        factory = Building(
            name="Fábrica Têxtil Silva",
            building_type=BuildingType.INDUSTRIAL_FACTORY_TEXTILE,
            x=50,
            y=30,
            address="Av. Industrial, 500",
            neighborhood="Distrito Industrial",
            width=50.0,
            length=80.0,
            height=10.0,
            floors=2,
            max_occupancy=150,
            architecture_style=BuildingArchitectureStyle.ART_DECO,
            construction_year=1925,
            era=2,
            land_value=Decimal("500000.00"),
            construction_cost=Decimal("2000000.00"),
            current_market_value=Decimal("3500000.00"),
            has_led=True,
            led_pin=5
        )
        
        db_session.add(factory)
        db_session.commit()
        
        assert factory.id is not None
        assert factory.building_type == BuildingType.INDUSTRIAL_FACTORY_TEXTILE
        assert factory.architecture_style == BuildingArchitectureStyle.ART_DECO
        assert factory.max_occupancy == 150
        assert factory.has_led is True
        assert factory.led_pin == 5


class TestBuildingMethods:
    """Testes de métodos do Building."""
    
    def test_calculate_monthly_costs(self, db_session):
        """Testa cálculo de custos mensais."""
        building = Building(
            name="Prédio Comercial",
            building_type=BuildingType.COMMERCIAL_OFFICE_SMALL,
            x=0,
            y=0,
            maintenance_cost=Decimal("1000.00"),
            utility_costs=Decimal("500.00"),
            tax_property=Decimal("300.00"),
            insurance_cost=Decimal("200.00")
        )
        
        db_session.add(building)
        db_session.commit()
        
        total_costs = building.calculate_monthly_costs()
        assert total_costs == 2000.00
    
    def test_calculate_monthly_income(self, db_session):
        """Testa cálculo de receita mensal."""
        building = Building(
            name="Apartamento Alugado",
            building_type=BuildingType.RESIDENTIAL_APARTMENT_MID,
            x=0,
            y=0,
            rental_income=Decimal("3000.00"),
            business_revenue=Decimal("0.00")
        )
        
        db_session.add(building)
        db_session.commit()
        
        total_income = building.calculate_monthly_income()
        assert total_income == 3000.00
    
    def test_is_profitable(self, db_session):
        """Testa verificação de lucratividade."""
        # Edifício lucrativo
        profitable = Building(
            name="Loja Lucrativa",
            building_type=BuildingType.COMMERCIAL_STORE_MEDIUM,
            x=0,
            y=0,
            rental_income=Decimal("5000.00"),
            business_revenue=Decimal("10000.00"),
            maintenance_cost=Decimal("2000.00"),
            utility_costs=Decimal("1000.00")
        )
        
        db_session.add(profitable)
        db_session.commit()
        
        assert profitable.is_profitable() is True
        
        # Edifício não lucrativo
        unprofitable = Building(
            name="Casa Cara",
            building_type=BuildingType.RESIDENTIAL_HOUSE_LARGE,
            x=0,
            y=0,
            rental_income=Decimal("1000.00"),
            maintenance_cost=Decimal("5000.00")
        )
        
        db_session.add(unprofitable)
        db_session.commit()
        
        assert unprofitable.is_profitable() is False
    
    def test_get_occupancy_rate(self, db_session):
        """Testa cálculo da taxa de ocupação."""
        building = Building(
            name="Prédio",
            building_type=BuildingType.RESIDENTIAL_APARTMENT_MID,
            x=0,
            y=0,
            max_occupancy=100,
            current_occupancy=75
        )
        
        db_session.add(building)
        db_session.commit()
        
        rate = building.get_occupancy_rate()
        assert rate == 0.75
        
        # Teste com ocupação zero
        building.current_occupancy = 0
        db_session.commit()
        assert building.get_occupancy_rate() == 0.0
        
        # Teste com lotação máxima
        building.current_occupancy = 100
        db_session.commit()
        assert building.get_occupancy_rate() == 1.0
    
    def test_can_accommodate(self, db_session):
        """Testa verificação de espaço disponível."""
        building = Building(
            name="Hotel",
            building_type=BuildingType.COMMERCIAL_HOTEL_SMALL,
            x=0,
            y=0,
            max_occupancy=50,
            current_occupancy=45
        )
        
        db_session.add(building)
        db_session.commit()
        
        # Pode acomodar 5 pessoas
        assert building.can_accommodate(5) is True
        
        # Não pode acomodar 10 pessoas
        assert building.can_accommodate(10) is False
    
    def test_is_operational(self, db_session):
        """Testa verificação se edifício está operacional."""
        # Operacional
        operational = Building(
            name="Loja Ativa",
            building_type=BuildingType.COMMERCIAL_STORE_SMALL,
            x=0,
            y=0,
            status=BuildingStatus.OPERATIONAL_ACTIVE,
            condition_value=80
        )
        
        db_session.add(operational)
        db_session.commit()
        
        assert operational.is_operational() is True
        
        # Não operacional (condição ruim)
        damaged = Building(
            name="Prédio Danificado",
            building_type=BuildingType.RESIDENTIAL_HOUSE_SMALL,
            x=0,
            y=0,
            status=BuildingStatus.OPERATIONAL_ACTIVE,
            condition_value=15
        )
        
        db_session.add(damaged)
        db_session.commit()
        
        assert damaged.is_operational() is False
        
        # Não operacional (status)
        closed = Building(
            name="Loja Fechada",
            building_type=BuildingType.COMMERCIAL_STORE_SMALL,
            x=0,
            y=0,
            status=BuildingStatus.ABANDONED_OLD,
            condition_value=80
        )
        
        db_session.add(closed)
        db_session.commit()
        
        assert closed.is_operational() is False
    
    def test_age_property(self, db_session):
        """Testa cálculo de idade do edifício."""
        current_year = datetime.utcnow().year
        
        building = Building(
            name="Edifício Antigo",
            building_type=BuildingType.PUBLIC_MUSEUM,
            x=0,
            y=0,
            construction_year=1920
        )
        
        db_session.add(building)
        db_session.commit()
        
        expected_age = current_year - 1920
        assert building.age == expected_age


class TestBuildingRelationships:
    """Testes de relacionamentos entre Building e Agent."""
    
    def test_building_owner(self, db_session):
        """Testa relacionamento de propriedade."""
        from backend.database.models import Agent, Gender, CreatedBy
        
        # Criar agente
        agent = Agent(
            name="João Silva",
            birth_date=datetime(1980, 5, 15),
            gender=Gender.CIS_MALE,
            created_by=CreatedBy.ADMIN,
            version="0.1.0"
        )
        
        # Criar edifício
        building = Building(
            name="Casa do João",
            building_type=BuildingType.RESIDENTIAL_HOUSE_MEDIUM,
            x=10,
            y=20,
            owner_type=BuildingOwnershipType.PRIVATE_INDIVIDUAL
        )
        
        db_session.add(agent)
        db_session.add(building)
        db_session.commit()
        
        # Atribuir propriedade
        building.owner_id = agent.id
        db_session.commit()
        
        # Verificar relacionamento
        assert building.owner.name == "João Silva"
        assert agent.owned_buildings[0].name == "Casa do João"
    
    def test_building_residents(self, db_session):
        """Testa relacionamento de moradores."""
        from backend.database.models import Agent, Gender, CreatedBy
        
        # Criar edifício
        building = Building(
            name="Prédio Residencial",
            building_type=BuildingType.RESIDENTIAL_APARTMENT_MID,
            x=5,
            y=5,
            max_occupancy=20
        )
        
        db_session.add(building)
        db_session.commit()
        
        # Criar moradores
        residents_data = [
            ("Maria Santos", datetime(1990, 3, 10)),
            ("Pedro Oliveira", datetime(1985, 7, 22)),
            ("Ana Costa", datetime(2000, 12, 5))
        ]
        
        for name, birth_date in residents_data:
            resident = Agent(
                name=name,
                birth_date=birth_date,
                gender=Gender.CIS_FEMALE if "Maria" in name or "Ana" in name else Gender.CIS_MALE,
                created_by=CreatedBy.ADMIN,
                version="0.1.0",
                home_building_id=building.id
            )
            db_session.add(resident)
        
        db_session.commit()
        
        # Verificar moradores
        assert len(building.residents) == 3
        assert building.residents[0].name == "Maria Santos"
        
        # Atualizar ocupação
        building.current_occupancy = len(building.residents)
        db_session.commit()
        
        assert building.current_occupancy == 3
        assert building.get_occupancy_rate() == 0.15  # 3/20
    
    def test_building_workers(self, db_session):
        """Testa relacionamento de trabalhadores."""
        from backend.database.models import Agent, Gender, CreatedBy
        
        # Criar fábrica
        factory = Building(
            name="Fábrica XYZ",
            building_type=BuildingType.INDUSTRIAL_FACTORY_METAL,
            x=50,
            y=50,
            max_occupancy=100
        )
        
        db_session.add(factory)
        db_session.commit()
        
        # Criar trabalhadores
        for i in range(10):
            worker = Agent(
                name=f"Trabalhador {i+1}",
                birth_date=datetime(1990, 1, 1),
                gender=Gender.CIS_MALE,
                created_by=CreatedBy.IA,
                version="0.1.0",
                work_building_id=factory.id
            )
            db_session.add(worker)
        
        db_session.commit()
        
        # Verificar trabalhadores
        assert len(factory.workers) == 10
        
        # Atualizar dados de emprego
        factory.current_occupancy = len(factory.workers)
        factory.jobs_created = len(factory.workers)
        db_session.commit()
        
        assert factory.jobs_created == 10


class TestBuildingQueries:
    """Testes de consultas/filtros de edifícios."""
    
    def test_filter_by_type(self, db_session):
        """Testa filtragem por tipo."""
        # Criar vários edifícios
        types = [
            BuildingType.RESIDENTIAL_HOUSE_SMALL,
            BuildingType.RESIDENTIAL_HOUSE_MEDIUM,
            BuildingType.COMMERCIAL_STORE_SMALL,
            BuildingType.INDUSTRIAL_FACTORY_TEXTILE
        ]
        
        for i, btype in enumerate(types):
            building = Building(
                name=f"Edifício {i+1}",
                building_type=btype,
                x=i,
                y=i
            )
            db_session.add(building)
        
        db_session.commit()
        
        # Buscar casas residenciais
        houses = db_session.query(Building).filter(
            Building.building_type.in_([
                BuildingType.RESIDENTIAL_HOUSE_SMALL,
                BuildingType.RESIDENTIAL_HOUSE_MEDIUM
            ])
        ).all()
        
        assert len(houses) == 2
    
    def test_filter_by_status(self, db_session):
        """Testa filtragem por status."""
        # Criar edifícios com diferentes status
        statuses = [
            BuildingStatus.OPERATIONAL_ACTIVE,
            BuildingStatus.CONSTRUCTION_FOUNDATION,
            BuildingStatus.DAMAGED_FIRE,
            BuildingStatus.ABANDONED_OLD
        ]
        
        for i, status in enumerate(statuses):
            building = Building(
                name=f"Prédio {i+1}",
                building_type=BuildingType.COMMERCIAL_OFFICE_SMALL,
                x=i,
                y=i,
                status=status
            )
            db_session.add(building)
        
        db_session.commit()
        
        # Buscar em construção
        under_construction = db_session.query(Building).filter(
            Building.status == BuildingStatus.CONSTRUCTION_FOUNDATION
        ).all()
        
        assert len(under_construction) == 1
    
    def test_filter_by_condition(self, db_session):
        """Testa filtragem por condição."""
        # Criar edifícios com diferentes condições
        conditions = [100, 80, 50, 25, 5]
        
        for i, condition in enumerate(conditions):
            building = Building(
                name=f"Casa {i+1}",
                building_type=BuildingType.RESIDENTIAL_HOUSE_SMALL,
                x=i,
                y=i,
                condition_value=condition
            )
            db_session.add(building)
        
        db_session.commit()
        
        # Buscar edifícios danificados (< 50)
        damaged = db_session.query(Building).filter(
            Building.condition_value < 50
        ).all()
        
        assert len(damaged) == 2


class TestBuildingEnums:
    """Testes dos enums de Building."""
    
    def test_building_type_values(self):
        """Testa valores dos tipos de edifício."""
        assert BuildingType.RESIDENTIAL_HOUSE_SMALL.value == "residential_house_small"
        assert BuildingType.COMMERCIAL_SUPERMARKET.value == "commercial_supermarket"
        assert BuildingType.INDUSTRIAL_FACTORY_TEXTILE.value == "industrial_factory_textile"
        assert BuildingType.PUBLIC_SCHOOL_ELEMENTARY.value == "public_school_elementary"
    
    def test_building_status_values(self):
        """Testa valores dos status."""
        assert BuildingStatus.PLANNING_PROPOSED.value == "planning_proposed"
        assert BuildingStatus.CONSTRUCTION_FOUNDATION.value == "construction_foundation"
        assert BuildingStatus.OPERATIONAL_ACTIVE.value == "operational_active"
        assert BuildingStatus.DAMAGED_FIRE.value == "damaged_fire"
    
    def test_architecture_style_values(self):
        """Testa valores dos estilos arquitetônicos."""
        assert BuildingArchitectureStyle.COLONIAL_PORTUGUESE.value == "colonial_portuguese"
        assert BuildingArchitectureStyle.ART_DECO.value == "art_deco"
        assert BuildingArchitectureStyle.BRUTALIST.value == "brutalist"
        assert BuildingArchitectureStyle.CONTEMPORARY.value == "contemporary"


class TestBuildingConstraints:
    """Testes de constraints/validações."""
    
    def test_condition_value_constraint(self, db_session):
        """Testa constraint de condition_value (0-100)."""
        # Valor válido
        valid_building = Building(
            name="Válido",
            building_type=BuildingType.RESIDENTIAL_HOUSE_SMALL,
            x=0,
            y=0,
            condition_value=50
        )
        
        db_session.add(valid_building)
        db_session.commit()
        
        assert valid_building.condition_value == 50
        
        # Valor inválido (> 100) - deve lançar exceção
        invalid_building = Building(
            name="Inválido",
            building_type=BuildingType.RESIDENTIAL_HOUSE_SMALL,
            x=0,
            y=0,
            condition_value=150  # Inválido
        )
        
        db_session.add(invalid_building)
        
        with pytest.raises(Exception):  # SQLAlchemy lançará exceção
            db_session.commit()


if __name__ == "__main__":
    pytest.main([__file__, "-v"])

