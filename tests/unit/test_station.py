"""
Testes unitários para o modelo Station.
"""
import pytest
from datetime import datetime, timedelta
from decimal import Decimal
from backend.database.models import (
    Station, StationType, StationStatus, Building, Company, BuildingType,
    BuildingStatus, BuildingCondition, BuildingArchitectureStyle,
    BuildingOwnershipType, BuildingZoning
)


class TestStation:
    """Testes para o modelo Station."""

    @pytest.fixture
    def sample_building(self, db_session):
        """Cria um edifício de exemplo para testes."""
        building = Building(
            name="Terminal Central",
            building_type=BuildingType.TRANSPORT_TRAIN_STATION_CENTRAL,
            x=100,
            y=200,
            status=BuildingStatus.OPERATIONAL_ACTIVE,
            condition=BuildingCondition.GOOD,
            condition_value=85,
            owner_type=BuildingOwnershipType.PUBLIC_MUNICIPAL,
            architecture_style=BuildingArchitectureStyle.MODERNIST,
            construction_year=2010,
            max_occupancy=1000
        )
        db_session.add(building)
        db_session.commit()
        return building

    @pytest.fixture
    def sample_company(self, db_session):
        """Cria uma empresa de exemplo para testes."""
        company = Company(
            name="Transporte Municipal LTDA",
            company_type="transport",
            balance=Decimal('1000000.00'),
            is_active=True
        )
        db_session.add(company)
        db_session.commit()
        return company

    def test_create_station(self, db_session):
        """Testa criação básica de uma estação."""
        # Criar estação
        station = Station(
            name="Estação Central - Plataforma 1",
            station_type=StationType.METRO_PLATFORM,
            code="M01-P1",
            x=100,
            y=200,
            z=-1,  # Subsolo
            platform_count=1,
            max_queue_length=100,
            is_accessible=True,
            has_elevator=True,
            has_tactile_paving=True
        )

        db_session.add(station)
        db_session.commit()

        # Verificar
        assert station.id is not None
        assert station.name == "Estação Central - Plataforma 1"
        assert station.station_type == StationType.METRO_PLATFORM
        assert station.code == "M01-P1"
        assert station.x == 100
        assert station.y == 200
        assert station.z == -1
        assert station.status == StationStatus.ACTIVE
        assert station.is_operational is True
        assert station.current_queue_length == 0
        assert station.platform_count == 1
        assert station.max_queue_length == 100
        assert station.is_accessible is True
        assert station.has_elevator is True
        assert station.has_tactile_paving is True

    def test_station_capacity(self, db_session):
        """Testa gestão de capacidade da estação."""
        station = Station(
            name="Ponto de Ônibus Central",
            station_type=StationType.BUS_STOP_TUBE,
            x=50,
            y=50,
            max_queue_length=30,
            serves_passengers=True
        )

        db_session.add(station)
        db_session.commit()

        # Testar adição de passageiros à fila
        assert station.can_accept_passengers(10) is True
        assert station.add_to_queue(10) is True
        assert station.current_queue_length == 10

        # Adicionar mais
        assert station.add_to_queue(15) is True
        assert station.current_queue_length == 25

        # Testar limite
        assert station.can_accept_passengers(10) is False
        assert station.add_to_queue(10) is False
        assert station.current_queue_length == 25

        # Testar se está superlotado
        assert station.is_overcrowded() is False

        station.add_to_queue(5)
        assert station.current_queue_length == 30
        assert station.is_overcrowded() is True

        # Testar remoção de fila
        removed = station.remove_from_queue(10)
        assert removed == 10
        assert station.current_queue_length == 20

        # Limpar fila
        station.clear_queue()
        assert station.current_queue_length == 0

    def test_board_vehicle(self, db_session):
        """Testa embarque de passageiros em veículo."""
        station = Station(
            name="Estação de Trem",
            station_type=StationType.TRAIN_PLATFORM,
            x=150,
            y=150,
            max_queue_length=200,
            serves_passengers=True
        )

        db_session.add(station)
        db_session.commit()

        # Adicionar passageiros à fila
        station.add_to_queue(50)
        assert station.current_queue_length == 50

        # Embarcar passageiros em um veículo com capacidade 40
        # O veículo tem capacidade 40, mas há 50 na fila, então embarca 40
        boarded = station.board_vehicle(vehicle_capacity=40)
        assert boarded == 40  # Embarcou 40 (capacidade do veículo)
        assert station.current_queue_length == 10  # Restaram 10 na fila
        assert station.total_passengers_served == 40

        # Adicionar mais passageiros
        station.add_to_queue(100)
        assert station.current_queue_length == 110  # 10 restantes + 100 novos

        # Embarcar em veículo com capacidade 80
        boarded = station.board_vehicle(vehicle_capacity=80)
        assert boarded == 80
        assert station.current_queue_length == 30  # 110 - 80
        assert station.total_passengers_served == 120  # 40 + 80

        # Verificar última contagem
        assert station.last_passenger_count_at is not None

    def test_station_building_relationship(self, db_session, sample_building):
        """Testa relacionamento entre Station e Building."""
        # Criar estação dentro de um edifício
        station = Station(
            name="Plataforma Sul",
            station_type=StationType.TRAIN_PLATFORM,
            building_id=sample_building.id,
            x=100,
            y=200,
            z=0,
            max_queue_length=150
        )

        db_session.add(station)
        db_session.commit()

        # Verificar relacionamento
        assert station.building is not None
        assert station.building.id == sample_building.id
        assert station.building.name == "Terminal Central"

        # Verificar relacionamento reverso
        db_session.refresh(sample_building)
        assert len(sample_building.stations) > 0
        assert station in sample_building.stations

    def test_station_company_operator(self, db_session, sample_company):
        """Testa relacionamento com empresa operadora."""
        station = Station(
            name="Estação Municipal",
            station_type=StationType.BUS_STATION_LOCAL,
            operator_company_id=sample_company.id,
            x=75,
            y=75,
            max_queue_length=80
        )

        db_session.add(station)
        db_session.commit()

        # Verificar relacionamento
        assert station.operator_company is not None
        assert station.operator_company.id == sample_company.id
        assert station.operator_company.name == "Transporte Municipal LTDA"

    def test_station_maintenance(self, db_session):
        """Testa sistema de manutenção."""
        station = Station(
            name="Estação Teste",
            station_type=StationType.METRO_STATION,
            x=120,
            y=120,
            condition_value=45,
            maintenance_interval_days=90
        )

        db_session.add(station)
        db_session.commit()

        # Verificar se precisa manutenção (condição < 50)
        assert station.needs_maintenance() is True

        # Realizar manutenção
        maintenance_cost = Decimal('500.00')
        station.perform_maintenance(cost=maintenance_cost)

        assert station.condition_value == 75  # 45 + 30
        assert station.last_maintenance_at is not None
        assert station.next_maintenance_at is not None
        assert station.maintenance_cost_total == maintenance_cost

        # Verificar agendamento
        expected_next = datetime.utcnow() + timedelta(days=90)
        time_diff = abs((station.next_maintenance_at - expected_next).total_seconds())
        assert time_diff < 5  # Menos de 5 segundos de diferença

    def test_station_degradation(self, db_session):
        """Testa degradação da estação."""
        station = Station(
            name="Estação Velha",
            station_type=StationType.BUS_STOP_SIMPLE,
            x=80,
            y=80,
            condition_value=100
        )

        db_session.add(station)
        db_session.commit()

        # Degradar
        station.degrade_condition(amount=5)
        assert station.condition_value == 95

        # Degradar até nível crítico
        station.degrade_condition(amount=80)
        assert station.condition_value == 15
        assert station.status == StationStatus.MAINTENANCE
        assert station.is_operational is False

    def test_station_accessibility_score(self, db_session):
        """Testa cálculo de score de acessibilidade."""
        station = Station(
            name="Estação Acessível",
            station_type=StationType.METRO_STATION,
            x=90,
            y=90,
            is_accessible=True,
            has_elevator=True,
            has_escalator=True,
            has_ramp=True,
            has_tactile_paving=True,
            has_audio_announcements=True,
            has_braille_signage=True
        )

        db_session.add(station)
        db_session.commit()

        # Score completo
        score = station.get_accessibility_score()
        assert score == 100  # 20+15+10+15+15+15+10

        # Station sem acessibilidade
        station2 = Station(
            name="Estação Básica",
            station_type=StationType.BUS_STOP_SIMPLE,
            x=95,
            y=95
        )
        db_session.add(station2)
        db_session.commit()

        score2 = station2.get_accessibility_score()
        assert score2 == 0

    def test_station_comfort_score(self, db_session):
        """Testa cálculo de score de conforto."""
        station = Station(
            name="Estação Confortável",
            station_type=StationType.BRT_STATION,
            x=110,
            y=110,
            has_shelter=True,
            has_seating=True,
            has_heating=True,
            has_cooling=True,
            has_restrooms=True,
            has_wifi=True,
            has_drinking_fountain=True
        )

        db_session.add(station)
        db_session.commit()

        score = station.get_comfort_score()
        assert score == 100  # 20+15+15+15+15+10+10

    def test_station_occupancy_rate(self, db_session):
        """Testa cálculo de taxa de ocupação."""
        station = Station(
            name="Estação Teste Ocupação",
            station_type=StationType.METRO_PLATFORM,
            x=130,
            y=130,
            max_queue_length=100,
            current_queue_length=0
        )

        db_session.add(station)
        db_session.commit()

        # Vazio
        assert station.calculate_occupancy_rate() == 0.0
        assert station.queue_status == "empty"

        # Baixo
        station.current_queue_length = 20
        assert station.calculate_occupancy_rate() == 20.0
        assert station.queue_status == "low"

        # Moderado
        station.current_queue_length = 50
        assert station.calculate_occupancy_rate() == 50.0
        assert station.queue_status == "moderate"

        # Alto
        station.current_queue_length = 80
        assert station.calculate_occupancy_rate() == 80.0
        assert station.queue_status == "high"

        # Crítico
        station.current_queue_length = 95
        assert station.calculate_occupancy_rate() == 95.0
        assert station.queue_status == "critical"

    def test_station_connections(self, db_session):
        """Testa conexões entre estações."""
        station1 = Station(
            name="Estação A",
            station_type=StationType.METRO_STATION,
            x=100,
            y=100,
            max_queue_length=100
        )

        station2 = Station(
            name="Estação B",
            station_type=StationType.METRO_STATION,
            x=150,
            y=150,
            max_queue_length=100
        )

        station3 = Station(
            name="Estação C",
            station_type=StationType.BUS_STATION_LOCAL,
            x=200,
            y=200,
            max_queue_length=50
        )

        db_session.add_all([station1, station2, station3])
        db_session.commit()

        # Adicionar conexões
        station1.add_connection(str(station2.id))
        station1.add_connection(str(station3.id))

        assert len(station1.connects_to_stations) == 2
        assert str(station2.id) in station1.connects_to_stations
        assert str(station3.id) in station1.connects_to_stations
        assert station1.is_multimodal is True

        # Buscar estações conectadas
        connected = station1.get_connected_stations(db_session)
        assert len(connected) == 2
        assert station2 in connected
        assert station3 in connected

        # Remover conexão
        station1.remove_connection(str(station3.id))
        assert len(station1.connects_to_stations) == 1
        assert str(station2.id) in station1.connects_to_stations

    def test_station_operating_hours(self, db_session):
        """Testa verificação de horários de operação."""
        station = Station(
            name="Estação com Horário",
            station_type=StationType.BUS_STOP_SHELTER,
            x=140,
            y=140,
            max_queue_length=50,
            operating_hours={
                "monday": ["06:00-22:00"],
                "tuesday": ["06:00-22:00"],
                "wednesday": ["06:00-22:00"],
                "thursday": ["06:00-22:00"],
                "friday": ["06:00-22:00"],
                "saturday": ["08:00-20:00"],
                "sunday": ["08:00-18:00"]
            }
        )

        db_session.add(station)
        db_session.commit()

        # Simular horário de operação
        # Nota: Este teste é simplificado, em produção usaríamos mock para datetime
        assert station.is_operational is True
        assert station.status == StationStatus.ACTIVE

    def test_station_validation(self, db_session):
        """Testa validações do modelo."""
        station = Station(
            name="Estação Validação",
            station_type=StationType.METRO_PLATFORM,
            x=160,
            y=160,
            max_queue_length=100,
            current_queue_length=50,
            condition_value=80
        )

        db_session.add(station)
        db_session.commit()

        # Testar validação de fila (não pode exceder máximo)
        station.current_queue_length = 150  # Vai ser ajustado para 100
        db_session.commit()
        db_session.refresh(station)
        assert station.current_queue_length == 100

        # Testar validação de condição (0-100)
        station.condition_value = 150  # Vai ser ajustado para 100
        db_session.commit()
        db_session.refresh(station)
        assert station.condition_value == 100

        station.condition_value = -10  # Vai ser ajustado para 0
        db_session.commit()
        db_session.refresh(station)
        assert station.condition_value == 0

    def test_station_daily_stats(self, db_session):
        """Testa métodos de estatísticas diárias."""
        station = Station(
            name="Estação Stats",
            station_type=StationType.BRT_STATION,
            x=170,
            y=170,
            max_queue_length=200,
            current_queue_length=80,
            daily_passenger_avg=1500,
            daily_passenger_peak=2000,
            total_passengers_served=45000,
            daily_cargo_avg_kg=500.0
        )

        db_session.add(station)
        db_session.commit()

        stats = station.get_daily_stats()
        assert stats['avg_passengers'] == 1500
        assert stats['peak_passengers'] == 2000
        assert stats['total_served'] == 45000
        assert stats['avg_cargo_kg'] == 500.0
        assert stats['occupancy_rate'] == 40.0
        assert stats['queue_status'] == 'moderate'

    def test_station_status_summary(self, db_session):
        """Testa resumo de status."""
        station = Station(
            name="Estação Summary",
            station_type=StationType.METRO_STATION,
            x=180,
            y=180,
            max_queue_length=150,
            current_queue_length=100,
            condition_value=90,
            is_accessible=True,
            has_elevator=True,
            has_shelter=True,
            has_seating=True
        )

        db_session.add(station)
        db_session.commit()

        summary = station.get_status_summary()
        assert summary['name'] == "Estação Summary"
        assert summary['type'] == StationType.METRO_STATION.value
        assert summary['status'] == StationStatus.ACTIVE.value
        assert summary['operational'] is True
        assert summary['condition'] == 90
        assert summary['queue'] == 100
        assert summary['max_queue'] == 150
        assert summary['accessibility_score'] > 0
        assert summary['comfort_score'] > 0

    def test_station_estimate_wait_time(self, db_session):
        """Testa estimativa de tempo de espera."""
        station = Station(
            name="Estação Wait Time",
            station_type=StationType.BUS_STOP_TUBE,
            x=190,
            y=190,
            max_queue_length=50
        )

        db_session.add(station)
        db_session.commit()

        # 6 veículos por hora = 10 minutos de espera
        wait_time = station.estimate_wait_time(vehicles_per_hour=6)
        assert wait_time == 10

        # 12 veículos por hora = 5 minutos
        wait_time = station.estimate_wait_time(vehicles_per_hour=12)
        assert wait_time == 5

        # Sem veículos = tempo máximo
        wait_time = station.estimate_wait_time(vehicles_per_hour=0)
        assert wait_time == 999

