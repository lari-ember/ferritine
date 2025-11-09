"""
Testes unitários para funcionalidades de Vehicle relacionadas a Station.
"""
import pytest
from decimal import Decimal
from backend.database.models import (
    Vehicle, Station, StationType, StationStatus,
    VehicleStatus, FuelType, MaintenanceStatus, Route
)
from backend.database.queries import DatabaseQueries


class TestVehicleDocking:
    """Testes para funcionalidades de docking de veículos."""

    @pytest.fixture
    def sample_station(self, db_session):
        """Cria uma estação de exemplo."""
        station = Station(
            name="Estação Central",
            station_type=StationType.METRO_STATION,
            code="M01",
            x=100,
            y=200,
            max_queue_length=150,
            is_operational=True,
            status=StationStatus.ACTIVE
        )
        db_session.add(station)
        db_session.commit()
        return station

    @pytest.fixture
    def sample_route(self, db_session):
        """Cria uma rota de exemplo."""
        route = Route(
            name="Linha 1 - Metrô",
            route_type="metro",
            is_active=True
        )
        db_session.add(route)
        db_session.commit()
        return route

    @pytest.fixture
    def sample_vehicles(self, db_session, sample_route):
        """Cria veículos de exemplo."""
        vehicles = []
        
        # Veículo 1: Metrô
        v1 = Vehicle(
            name="Metrô 001",
            vehicle_type="metro",
            license_plate="MTR-001",
            passenger_capacity=200,
            current_passengers=0,
            fuel_type=FuelType.ELECTRIC,
            status=VehicleStatus.ACTIVE,
            assigned_route_id=sample_route.id
        )
        
        # Veículo 2: Ônibus
        v2 = Vehicle(
            name="Ônibus 101",
            vehicle_type="bus",
            license_plate="BUS-101",
            passenger_capacity=80,
            current_passengers=0,
            fuel_type=FuelType.DIESEL,
            status=VehicleStatus.ACTIVE
        )
        
        # Veículo 3: Trem
        v3 = Vehicle(
            name="Trem 501",
            vehicle_type="train",
            license_plate="TRN-501",
            passenger_capacity=300,
            current_passengers=0,
            fuel_type=FuelType.DIESEL,
            status=VehicleStatus.ACTIVE
        )
        
        vehicles = [v1, v2, v3]
        db_session.add_all(vehicles)
        db_session.commit()
        
        return vehicles

    def test_vehicle_has_docking_fields(self, db_session, sample_vehicles):
        """Testa que o veículo tem os novos campos."""
        vehicle = sample_vehicles[0]
        
        assert hasattr(vehicle, 'current_station_id')
        assert hasattr(vehicle, 'is_docked')
        assert hasattr(vehicle, 'assigned_route_id')
        
        # Valores padrão
        assert vehicle.current_station_id is None
        assert vehicle.is_docked is False
        assert vehicle.assigned_route_id is not None

    def test_dock_vehicle(self, db_session, sample_vehicles, sample_station):
        """Testa acoplamento de veículo em uma estação."""
        db = DatabaseQueries(db_session)
        vehicle = sample_vehicles[0]
        
        # Acoplar veículo
        updated = db.vehicles.dock_vehicle(vehicle.id, sample_station.id)
        
        assert updated is not None
        assert updated.current_station_id == sample_station.id
        assert updated.is_docked is True
        assert updated.is_moving is False
        assert updated.speed == 0.0

    def test_undock_vehicle(self, db_session, sample_vehicles, sample_station):
        """Testa desacoplamento de veículo."""
        db = DatabaseQueries(db_session)
        vehicle = sample_vehicles[0]
        
        # Acoplar primeiro
        db.vehicles.dock_vehicle(vehicle.id, sample_station.id)
        
        # Desacoplar
        updated = db.vehicles.undock_vehicle(vehicle.id)
        
        assert updated is not None
        assert updated.current_station_id is None
        assert updated.is_docked is False

    def test_get_docked_at_station(self, db_session, sample_vehicles, sample_station):
        """Testa busca de veículos acoplados em uma estação."""
        db = DatabaseQueries(db_session)
        
        # Acoplar 2 veículos
        db.vehicles.dock_vehicle(sample_vehicles[0].id, sample_station.id)
        db.vehicles.dock_vehicle(sample_vehicles[1].id, sample_station.id)
        
        # Buscar veículos acoplados
        docked = db.vehicles.get_docked_at_station(sample_station.id)
        
        assert len(docked) == 2
        assert all(v.is_docked for v in docked)
        assert all(v.current_station_id == sample_station.id for v in docked)

    def test_get_all_docked(self, db_session, sample_vehicles, sample_station):
        """Testa busca de todos os veículos acoplados."""
        db = DatabaseQueries(db_session)
        
        # Criar outra estação
        station2 = Station(
            name="Estação Sul",
            station_type=StationType.METRO_STATION,
            code="M02",
            x=200,
            y=200,
            max_queue_length=150,
            is_operational=True,
            status=StationStatus.ACTIVE
        )
        db_session.add(station2)
        db_session.commit()
        
        # Acoplar veículos em diferentes estações
        db.vehicles.dock_vehicle(sample_vehicles[0].id, sample_station.id)
        db.vehicles.dock_vehicle(sample_vehicles[1].id, station2.id)
        
        # Buscar todos acoplados
        all_docked = db.vehicles.get_all_docked()
        
        assert len(all_docked) == 2
        assert all(v.is_docked for v in all_docked)

    def test_vehicle_station_relationship(self, db_session, sample_vehicles, sample_station):
        """Testa relacionamento bidirecional Vehicle-Station."""
        db = DatabaseQueries(db_session)
        vehicle = sample_vehicles[0]
        
        # Acoplar veículo
        db.vehicles.dock_vehicle(vehicle.id, sample_station.id)
        db_session.refresh(vehicle)
        db_session.refresh(sample_station)
        
        # Testar relacionamento direto
        assert vehicle.current_station is not None
        assert vehicle.current_station.id == sample_station.id
        
        # Testar relacionamento reverso
        assert len(sample_station.docked_vehicles) > 0
        assert vehicle in sample_station.docked_vehicles

    def test_multiple_vehicles_same_station(self, db_session, sample_vehicles, sample_station):
        """Testa múltiplos veículos na mesma estação."""
        db = DatabaseQueries(db_session)
        
        # Acoplar todos os veículos na mesma estação
        for vehicle in sample_vehicles:
            db.vehicles.dock_vehicle(vehicle.id, sample_station.id)
        
        db_session.refresh(sample_station)
        
        # Verificar que todos estão acoplados
        assert len(sample_station.docked_vehicles) == 3
        
        docked = db.vehicles.get_docked_at_station(sample_station.id)
        assert len(docked) == 3

    def test_dock_undock_cycle(self, db_session, sample_vehicles, sample_station):
        """Testa ciclo completo de dock/undock."""
        db = DatabaseQueries(db_session)
        vehicle = sample_vehicles[0]
        
        # Estado inicial
        assert vehicle.is_docked is False
        assert vehicle.current_station_id is None
        
        # Acoplar
        db.vehicles.dock_vehicle(vehicle.id, sample_station.id)
        db_session.refresh(vehicle)
        assert vehicle.is_docked is True
        assert vehicle.current_station_id == sample_station.id
        
        # Desacoplar
        db.vehicles.undock_vehicle(vehicle.id)
        db_session.refresh(vehicle)
        assert vehicle.is_docked is False
        assert vehicle.current_station_id is None
        
        # Acoplar novamente
        db.vehicles.dock_vehicle(vehicle.id, sample_station.id)
        db_session.refresh(vehicle)
        assert vehicle.is_docked is True

    def test_dock_sets_vehicle_stationary(self, db_session, sample_vehicles, sample_station):
        """Testa que docking deixa o veículo parado."""
        db = DatabaseQueries(db_session)
        vehicle = sample_vehicles[0]
        
        # Simular veículo em movimento
        vehicle.is_moving = True
        vehicle.speed = 60.0
        db_session.commit()
        
        # Acoplar
        db.vehicles.dock_vehicle(vehicle.id, sample_station.id)
        db_session.refresh(vehicle)
        
        # Verificar que está parado
        assert vehicle.is_moving is False
        assert vehicle.speed == 0.0

    def test_get_by_assigned_route(self, db_session, sample_vehicles, sample_route):
        """Testa busca de veículos por rota atribuída."""
        db = DatabaseQueries(db_session)
        
        # Veículo 1 já tem assigned_route_id
        vehicles = db.vehicles.get_by_assigned_route(sample_route.id)
        
        assert len(vehicles) >= 1
        assert all(v.assigned_route_id == sample_route.id for v in vehicles)

    def test_assign_route_to_vehicle(self, db_session, sample_vehicles, sample_route):
        """Testa atribuição de rota a um veículo."""
        vehicle = sample_vehicles[1]  # Ônibus sem rota
        
        # Atribuir rota
        vehicle.assigned_route_id = sample_route.id
        db_session.commit()
        
        # Verificar relacionamento
        db_session.refresh(vehicle)
        assert vehicle.assigned_route_id == sample_route.id
        assert vehicle.assigned_route is not None
        assert vehicle.assigned_route.id == sample_route.id

    def test_dock_vehicle_not_found(self, db_session, sample_station):
        """Testa dock de veículo inexistente."""
        import uuid
        db = DatabaseQueries(db_session)
        
        result = db.vehicles.dock_vehicle(uuid.uuid4(), sample_station.id)
        assert result is None

    def test_undock_vehicle_not_found(self, db_session):
        """Testa undock de veículo inexistente."""
        import uuid
        db = DatabaseQueries(db_session)
        
        result = db.vehicles.undock_vehicle(uuid.uuid4())
        assert result is None

    def test_docked_vehicles_empty_station(self, db_session, sample_station):
        """Testa busca em estação sem veículos."""
        db = DatabaseQueries(db_session)
        
        docked = db.vehicles.get_docked_at_station(sample_station.id)
        assert len(docked) == 0

    def test_vehicle_docking_with_passengers(self, db_session, sample_vehicles, sample_station):
        """Testa docking de veículo com passageiros."""
        db = DatabaseQueries(db_session)
        vehicle = sample_vehicles[0]
        
        # Adicionar passageiros
        vehicle.current_passengers = 50
        db_session.commit()
        
        # Acoplar
        db.vehicles.dock_vehicle(vehicle.id, sample_station.id)
        db_session.refresh(vehicle)
        
        # Passageiros devem ser mantidos
        assert vehicle.current_passengers == 50
        assert vehicle.is_docked is True

    def test_station_capacity_tracking(self, db_session, sample_vehicles, sample_station):
        """Testa rastreamento de capacidade da estação."""
        db = DatabaseQueries(db_session)
        
        # Verificar capacidade inicial
        initial_count = len(sample_station.docked_vehicles)
        
        # Acoplar veículo
        db.vehicles.dock_vehicle(sample_vehicles[0].id, sample_station.id)
        db_session.refresh(sample_station)
        
        # Verificar que aumentou
        assert len(sample_station.docked_vehicles) == initial_count + 1

    def test_multiple_stations_different_vehicles(self, db_session, sample_vehicles):
        """Testa múltiplas estações com veículos diferentes."""
        db = DatabaseQueries(db_session)
        
        # Criar múltiplas estações
        stations = []
        for i in range(3):
            station = Station(
                name=f"Estação {i}",
                station_type=StationType.METRO_STATION,
                code=f"M0{i}",
                x=100 + i * 50,
                y=200,
                max_queue_length=150,
                is_operational=True,
                status=StationStatus.ACTIVE
            )
            stations.append(station)
        
        db_session.add_all(stations)
        db_session.commit()
        
        # Distribuir veículos pelas estações
        for i, vehicle in enumerate(sample_vehicles):
            db.vehicles.dock_vehicle(vehicle.id, stations[i].id)
        
        # Verificar distribuição
        for i, station in enumerate(stations):
            db_session.refresh(station)
            assert len(station.docked_vehicles) == 1
            assert station.docked_vehicles[0].id == sample_vehicles[i].id

    def test_vehicle_change_station(self, db_session, sample_vehicles, sample_station):
        """Testa mudança de veículo entre estações."""
        db = DatabaseQueries(db_session)
        vehicle = sample_vehicles[0]
        
        # Criar segunda estação
        station2 = Station(
            name="Estação Norte",
            station_type=StationType.METRO_STATION,
            code="M03",
            x=300,
            y=300,
            max_queue_length=150,
            is_operational=True,
            status=StationStatus.ACTIVE
        )
        db_session.add(station2)
        db_session.commit()
        
        # Acoplar na primeira estação
        db.vehicles.dock_vehicle(vehicle.id, sample_station.id)
        db_session.refresh(sample_station)
        assert len(sample_station.docked_vehicles) == 1
        
        # Desacoplar e acoplar na segunda
        db.vehicles.undock_vehicle(vehicle.id)
        db.vehicles.dock_vehicle(vehicle.id, station2.id)
        
        # Verificar mudança
        db_session.refresh(sample_station)
        db_session.refresh(station2)
        assert len(sample_station.docked_vehicles) == 0
        assert len(station2.docked_vehicles) == 1

    def test_docking_integration_complete(self, db_session, sample_vehicles, sample_station, sample_route):
        """Teste de integração completo do sistema de docking."""
        db = DatabaseQueries(db_session)
        vehicle = sample_vehicles[0]
        
        # Estado inicial
        assert vehicle.is_docked is False
        assert vehicle.assigned_route_id == sample_route.id
        
        # 1. Acoplar na estação
        db.vehicles.dock_vehicle(vehicle.id, sample_station.id)
        db_session.refresh(vehicle)
        db_session.refresh(sample_station)
        
        # Verificações
        assert vehicle.is_docked is True
        assert vehicle.current_station_id == sample_station.id
        assert vehicle in sample_station.docked_vehicles
        
        # 2. Embarcar passageiros
        vehicle.current_passengers = 100
        db_session.commit()
        
        # 3. Preparar para partir
        db.vehicles.undock_vehicle(vehicle.id)
        db_session.refresh(vehicle)
        
        assert vehicle.is_docked is False
        assert vehicle.current_station_id is None
        assert vehicle.current_passengers == 100  # Passageiros mantidos
        
        # 4. Verificar estação
        db_session.refresh(sample_station)
        assert len(sample_station.docked_vehicles) == 0

