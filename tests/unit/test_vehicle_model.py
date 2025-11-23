"""
Testes unitários para o modelo Vehicle.
"""
import pytest
from datetime import datetime, timedelta
from decimal import Decimal
from backend.database.models import (
    Vehicle, VehicleStatus, MaintenanceStatus, FuelType
)


class TestVehicleModel:
    """Testes para o modelo Vehicle."""

    def test_vehicle_creation(self):
        """Testa criação básica de um veículo."""
        vehicle = Vehicle(
            name="Trem Central",
            vehicle_type="train",
            license_plate="TR-1234",
            manufacturer="Ferroeste",
            model="Serie 2000",
            year=2020
        )
        
        assert vehicle.name == "Trem Central"
        assert vehicle.vehicle_type == "train"
        assert vehicle.status == VehicleStatus.ACTIVE
        assert vehicle.maintenance_status == MaintenanceStatus.GOOD

    def test_fuel_consumption(self):
        """Testa consumo de combustível."""
        vehicle = Vehicle(
            name="Ônibus 101",
            vehicle_type="bus",
            fuel_capacity=100.0,
            current_fuel=100.0,
            fuel_consumption_rate=0.5  # 0.5L por km
        )
        
        # Consome combustível para 10km
        result = vehicle.consume_fuel(10.0)
        
        assert result is True
        assert vehicle.current_fuel == 95.0  # 100 - (10 * 0.5)
        assert vehicle.odometer == 10.0

    def test_fuel_consumption_insufficient(self):
        """Testa consumo quando não há combustível suficiente."""
        vehicle = Vehicle(
            name="Carro 1",
            vehicle_type="car",
            fuel_capacity=50.0,
            current_fuel=5.0,
            fuel_consumption_rate=1.0
        )
        
        # Tenta consumir combustível para 10km (precisa de 10L, tem apenas 5L)
        result = vehicle.consume_fuel(10.0)
        
        assert result is False
        assert vehicle.current_fuel == 5.0  # Não mudou
        assert vehicle.odometer == 0.0  # Não se moveu

    def test_refuel(self):
        """Testa abastecimento do veículo."""
        vehicle = Vehicle(
            name="Caminhão 1",
            vehicle_type="truck",
            fuel_capacity=200.0,
            current_fuel=50.0
        )
        
        # Abastece 100L
        added = vehicle.refuel(100.0)
        
        assert added == 100.0
        assert vehicle.current_fuel == 150.0

    def test_refuel_overflow(self):
        """Testa abastecimento que excede capacidade."""
        vehicle = Vehicle(
            name="Moto 1",
            vehicle_type="motorcycle",
            fuel_capacity=15.0,
            current_fuel=10.0
        )
        
        # Tenta abastecer 10L (capacidade restante é 5L)
        added = vehicle.refuel(10.0)
        
        assert added == 5.0
        assert vehicle.current_fuel == 15.0  # Máximo

    def test_needs_maintenance_by_km(self):
        """Testa necessidade de manutenção por quilometragem."""
        vehicle = Vehicle(
            name="Van 1",
            vehicle_type="van",
            odometer=10500.0,
            next_maintenance_km=10000
        )
        
        assert vehicle.needs_maintenance() is True

    def test_needs_maintenance_by_status(self):
        """Testa necessidade de manutenção por status crítico."""
        vehicle = Vehicle(
            name="Táxi 1",
            vehicle_type="taxi",
            odometer=5000.0,
            next_maintenance_km=10000,
            maintenance_status=MaintenanceStatus.CRITICAL
        )
        
        assert vehicle.needs_maintenance() is True

    def test_depreciation_calculation(self):
        """Testa cálculo de depreciação."""
        purchase_date = datetime.utcnow() - timedelta(days=365)  # 1 ano atrás
        
        vehicle = Vehicle(
            name="Carro Usado",
            vehicle_type="car",
            purchase_date=purchase_date,
            purchase_price=Decimal("50000.00"),
            depreciation_rate=0.15  # 15% ao ano
        )
        
        new_value = vehicle.calculate_depreciation()
        
        # Após 1 ano com 15% de depreciação: 50000 * 0.85 = 42500
        assert float(new_value) == pytest.approx(42500.0, rel=0.01)

    def test_passenger_operations(self):
        """Testa embarque e desembarque de passageiros."""
        vehicle = Vehicle(
            name="Ônibus Urbano",
            vehicle_type="bus",
            passenger_capacity=50,
            current_passengers=20
        )
        
        # Embarca 15 passageiros
        assert vehicle.board_passengers(15) is True
        assert vehicle.current_passengers == 35
        
        # Tenta embarcar 20 (excederia capacidade)
        assert vehicle.board_passengers(20) is False
        assert vehicle.current_passengers == 35  # Não mudou
        
        # Desembarca 10 passageiros
        assert vehicle.disembark_passengers(10) is True
        assert vehicle.current_passengers == 25

    def test_cargo_operations(self):
        """Testa carregamento e descarregamento de carga."""
        vehicle = Vehicle(
            name="Caminhão de Carga",
            vehicle_type="truck",
            cargo_capacity=5000.0,  # 5 toneladas
            current_cargo=1000.0
        )
        
        # Carrega 2 toneladas
        assert vehicle.load_cargo(2000.0) is True
        assert vehicle.current_cargo == 3000.0
        
        # Tenta carregar 3 toneladas (excederia capacidade)
        assert vehicle.load_cargo(3000.0) is False
        assert vehicle.current_cargo == 3000.0  # Não mudou
        
        # Descarrega 1.5 toneladas
        assert vehicle.unload_cargo(1500.0) is True
        assert vehicle.current_cargo == 1500.0

    def test_can_accept_passengers(self):
        """Testa verificação de espaço para passageiros."""
        vehicle = Vehicle(
            name="Trem Lotado",
            vehicle_type="train",
            passenger_capacity=100,
            current_passengers=95
        )
        
        assert vehicle.can_accept_passengers(5) is True
        assert vehicle.can_accept_passengers(6) is False

    def test_vehicle_status_enum(self):
        """Testa enumeração de status do veículo."""
        vehicle = Vehicle(
            name="Veículo Teste",
            vehicle_type="car"
        )
        
        vehicle.status = VehicleStatus.ACTIVE
        assert vehicle.status == VehicleStatus.ACTIVE
        
        vehicle.status = VehicleStatus.MAINTENANCE
        assert vehicle.status == VehicleStatus.MAINTENANCE
        
        vehicle.status = VehicleStatus.RETIRED
        assert vehicle.status == VehicleStatus.RETIRED

    def test_fuel_type_enum(self):
        """Testa enumeração de tipos de combustível."""
        diesel_vehicle = Vehicle(
            name="Trem Diesel",
            vehicle_type="train",
            fuel_type=FuelType.DIESEL
        )
        assert diesel_vehicle.fuel_type == FuelType.DIESEL
        
        electric_vehicle = Vehicle(
            name="Trem Elétrico",
            vehicle_type="train",
            fuel_type=FuelType.ELECTRIC
        )
        assert electric_vehicle.fuel_type == FuelType.ELECTRIC

    def test_vehicle_repr(self):
        """Testa representação em string do veículo."""
        vehicle = Vehicle(
            name="Trem Expresso",
            vehicle_type="train",
            status=VehicleStatus.ACTIVE
        )
        
        repr_str = repr(vehicle)
        assert "Trem Expresso" in repr_str
        assert "train" in repr_str
        assert "active" in repr_str


class TestVehicleConstraints:
    """Testes para constraints do modelo Vehicle."""

    def test_passenger_capacity_constraint(self):
        """Testa que current_passengers não pode exceder passenger_capacity."""
        vehicle = Vehicle(
            name="Ônibus",
            vehicle_type="bus",
            passenger_capacity=50
        )
        
        # Definir manualmente (simulando violação)
        # Nota: O constraint do banco impediria isso, mas no modelo Python
        # podemos testar a lógica dos métodos
        vehicle.current_passengers = 60
        
        # O méto do board_passengers deve prevenir isso
        vehicle.current_passengers = 50
        assert vehicle.board_passengers(1) is False

    def test_fuel_constraints(self):
        """Testa constraints de combustível."""
        vehicle = Vehicle(
            name="Carro",
            vehicle_type="car",
            fuel_capacity=60.0,
            current_fuel=60.0
        )
        
        # Refuel não deve exceder capacidade
        added = vehicle.refuel(10.0)
        assert added == 0.0
        assert vehicle.current_fuel == 60.0


class TestVehicleIntegration:
    """Testes de integração entre métodos do Vehicle."""

    def test_full_operation_cycle(self):
        """Testa um ciclo completo de operação do veículo."""
        # Cria veículo
        vehicle = Vehicle(
            name="Ônibus Linha 101",
            vehicle_type="bus",
            passenger_capacity=50,
            fuel_capacity=100.0,
            current_fuel=100.0,
            fuel_consumption_rate=0.2,
            purchase_date=datetime.utcnow(),
            purchase_price=Decimal("200000.00")
        )
        
        # Embarca passageiros
        vehicle.board_passengers(30)
        assert vehicle.current_passengers == 30
        
        # Viaja 50km
        success = vehicle.consume_fuel(50.0)
        assert success is True
        assert vehicle.current_fuel == 90.0  # 100 - (50 * 0.2)
        assert vehicle.odometer == 50.0
        
        # Desembarca alguns passageiros
        vehicle.disembark_passengers(15)
        assert vehicle.current_passengers == 15
        
        # Viaja mais 100km
        success = vehicle.consume_fuel(100.0)
        assert success is True
        assert vehicle.current_fuel == 70.0
        assert vehicle.odometer == 150.0
        
        # Reabastecer
        vehicle.refuel(30.0)
        assert vehicle.current_fuel == 100.0
        
        # Verifica status geral
        assert vehicle.status == VehicleStatus.ACTIVE
        assert not vehicle.needs_maintenance()  # Ainda longe da próxima manutenção

