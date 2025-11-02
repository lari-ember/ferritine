"""
Tests for vehicle transport system.
"""

import pytest
from datetime import datetime, timedelta

from backend.simulation.models.vehicle import (
    Vehicle, Train, Bus, Tram, Taxi, Truck,
    VehicleStatus, FuelType
)
from backend.database.vehicle_db import VehicleDatabase


class MockDB:
    """Mock database for testing vehicles."""
    
    def __init__(self):
        self.vehicles = {}
        self.trips = []
        self.tickets = []
        self.maintenance_records = []
        self.incidents = []
        self.routes = {
            1: {
                'id': 1,
                'name': 'Route 1',
                'total_distance_km': 10.0
            }
        }
    
    def create_vehicle(self, vehicle_data):
        vehicle_id = len(self.vehicles) + 1
        self.vehicles[vehicle_id] = vehicle_data
        return vehicle_id
    
    def get_vehicle(self, vehicle_id):
        return self.vehicles.get(vehicle_id)
    
    def update_vehicle(self, vehicle_id, vehicle_data):
        if vehicle_id in self.vehicles:
            self.vehicles[vehicle_id].update(vehicle_data)
    
    def create_trip(self, **trip_data):
        trip_id = len(self.trips) + 1
        self.trips.append({**trip_data, 'id': trip_id})
        return trip_id
    
    def complete_trip(self, vehicle_id):
        for trip in self.trips:
            if trip.get('vehicle_id') == vehicle_id and 'ended_at' not in trip:
                trip['ended_at'] = datetime.now()
    
    def create_ticket(self, **ticket_data):
        ticket_id = len(self.tickets) + 1
        self.tickets.append({**ticket_data, 'id': ticket_id})
        return ticket_id
    
    def complete_ticket(self, agent_id, vehicle_id, alighting_station_id):
        for ticket in self.tickets:
            if (ticket.get('agent_id') == agent_id and 
                ticket.get('vehicle_id') == vehicle_id and
                'alighting_station_id' not in ticket):
                ticket['alighting_station_id'] = alighting_station_id
    
    def create_maintenance_record(self, **maintenance_data):
        record_id = len(self.maintenance_records) + 1
        self.maintenance_records.append({**maintenance_data, 'id': record_id})
        return record_id
    
    def create_incident(self, **incident_data):
        incident_id = len(self.incidents) + 1
        self.incidents.append({**incident_data, 'id': incident_id})
        return incident_id
    
    def get_route(self, route_id):
        return self.routes.get(route_id)
    
    def get_vehicle_stats(self, vehicle_id, date):
        return {
            'total_revenue': 100.0,
            'total_fuel_cost': 30.0,
            'total_maintenance_cost': 20.0,
            'profit': 50.0
        }


@pytest.fixture
def mock_db():
    """Provides a mock database for tests."""
    return MockDB()


class TestVehicleBasics:
    """Tests for basic vehicle functionality."""
    
    def test_create_vehicle(self, mock_db):
        """Test creating a basic vehicle."""
        vehicle = Vehicle(
            mock_db,
            name="Test Vehicle",
            type="generic",
            max_passengers=50
        )
        
        assert vehicle.name == "Test Vehicle"
        assert vehicle.type == "generic"
        assert vehicle.max_passengers == 50
        assert vehicle.current_passengers == 0
        assert vehicle.status == VehicleStatus.IDLE
        assert vehicle.is_active is True
    
    def test_vehicle_status_enum(self, mock_db):
        """Test vehicle status enum."""
        vehicle = Vehicle(mock_db, status='moving')
        assert vehicle.status == VehicleStatus.MOVING
        
        vehicle.status = VehicleStatus.MAINTENANCE
        assert vehicle.status == VehicleStatus.MAINTENANCE
    
    def test_fuel_type_enum(self, mock_db):
        """Test fuel type enum."""
        vehicle = Vehicle(mock_db, fuel_type='diesel')
        assert vehicle.fuel_type == FuelType.DIESEL
        
        coal_vehicle = Vehicle(mock_db, fuel_type='coal')
        assert coal_vehicle.fuel_type == FuelType.COAL


class TestVehicleMovement:
    """Tests for vehicle movement functionality."""
    
    def test_start_trip(self, mock_db):
        """Test starting a trip."""
        vehicle = Vehicle(
            mock_db,
            name="Bus 1",
            condition_percent=80,
            current_fuel=50
        )
        
        trip_id = vehicle.start_trip(route_id=1, station_id=1)
        
        assert trip_id is not None
        assert vehicle.current_route_id == 1
        assert vehicle.next_station_id == 1
        assert vehicle.status == VehicleStatus.MOVING
        assert len(mock_db.trips) == 1
    
    def test_cannot_start_trip_when_broken(self, mock_db):
        """Test that broken vehicles cannot start trips."""
        vehicle = Vehicle(
            mock_db,
            name="Broken Bus",
            status='broken',
            condition_percent=10
        )
        
        result = vehicle.start_trip(route_id=1)
        
        assert result is False
        assert vehicle.status == VehicleStatus.BROKEN
    
    def test_move_updates_position(self, mock_db):
        """Test that moving updates vehicle position."""
        vehicle = Vehicle(
            mock_db,
            name="Train 1",
            speed_kmh=60.0,
            current_fuel=100
        )
        
        vehicle.start_trip(route_id=1)
        initial_km = vehicle.total_km_traveled
        
        # Move for 0.5 hours at 60 km/h = 30 km
        vehicle.move(delta_time_hours=0.5)
        
        assert vehicle.total_km_traveled > initial_km
        assert vehicle.current_fuel < 100  # Fuel consumed
    
    def test_complete_trip(self, mock_db):
        """Test completing a trip."""
        vehicle = Vehicle(mock_db, name="Bus 2")
        vehicle.current_passengers = 20
        
        vehicle.start_trip(route_id=1)
        vehicle.complete_trip()
        
        assert vehicle.status == VehicleStatus.IDLE
        assert vehicle.current_route_id is None
        assert vehicle.current_passengers == 0
        assert vehicle.speed_kmh == 0.0


class TestPassengersAndCargo:
    """Tests for passenger and cargo management."""
    
    def test_board_passenger(self, mock_db):
        """Test boarding a passenger."""
        vehicle = Vehicle(
            mock_db,
            name="Bus 1",
            max_passengers=40
        )
        
        result = vehicle.board_passenger(agent_id=1, boarding_station_id=1)
        
        assert result is True
        assert vehicle.current_passengers == 1
        assert len(mock_db.tickets) == 1
    
    def test_cannot_board_when_full(self, mock_db):
        """Test that passengers cannot board when vehicle is full."""
        vehicle = Vehicle(
            mock_db,
            name="Full Bus",
            max_passengers=2,
            current_passengers=2
        )
        
        result = vehicle.board_passenger(agent_id=1, boarding_station_id=1)
        
        assert result is False
        assert vehicle.current_passengers == 2
    
    def test_alight_passenger(self, mock_db):
        """Test passenger alighting."""
        vehicle = Vehicle(
            mock_db,
            name="Bus 1",
            current_passengers=5
        )
        
        vehicle.alight_passenger(agent_id=1, alighting_station_id=2)
        
        assert vehicle.current_passengers == 4
    
    def test_load_cargo(self, mock_db):
        """Test loading cargo."""
        vehicle = Vehicle(
            mock_db,
            name="Truck 1",
            max_cargo_kg=10000
        )
        
        result = vehicle.load_cargo(5000)
        
        assert result is True
        assert vehicle.current_cargo_kg == 5000
    
    def test_cannot_overload_cargo(self, mock_db):
        """Test that cargo cannot exceed capacity."""
        vehicle = Vehicle(
            mock_db,
            name="Truck 1",
            max_cargo_kg=10000,
            current_cargo_kg=8000
        )
        
        result = vehicle.load_cargo(5000)
        
        assert result is False
        assert vehicle.current_cargo_kg == 8000


class TestMaintenance:
    """Tests for vehicle maintenance system."""
    
    def test_wear_reduces_condition(self, mock_db):
        """Test that movement causes wear."""
        vehicle = Vehicle(
            mock_db,
            name="Train 1",
            condition_percent=100,
            wear_rate=0.1
        )
        
        initial_condition = vehicle.condition_percent
        vehicle._apply_wear(distance_km=100)
        
        assert vehicle.condition_percent < initial_condition
    
    def test_schedule_maintenance(self, mock_db):
        """Test scheduling maintenance."""
        vehicle = Vehicle(mock_db, name="Bus 1")
        
        vehicle.schedule_maintenance(days_ahead=7)
        
        assert vehicle.next_maintenance_due is not None
        assert vehicle.next_maintenance_due > datetime.now()
    
    def test_perform_maintenance_restores_condition(self, mock_db):
        """Test that maintenance restores vehicle condition."""
        vehicle = Vehicle(
            mock_db,
            name="Train 1",
            condition_percent=50,
            id=1
        )
        
        cost = vehicle.perform_maintenance()
        
        assert vehicle.condition_percent == 100
        assert vehicle.status == VehicleStatus.IDLE
        assert vehicle.repairs_count == 1
        assert cost > 0
        assert len(mock_db.maintenance_records) == 1
    
    def test_auto_schedule_maintenance_when_low_condition(self, mock_db):
        """Test that maintenance is auto-scheduled when condition is low."""
        vehicle = Vehicle(
            mock_db,
            name="Bus 1",
            condition_percent=60
        )
        
        # Apply wear to bring condition below 50%
        vehicle._apply_wear(distance_km=200)
        
        assert vehicle.next_maintenance_due is not None


class TestFuel:
    """Tests for fuel consumption system."""
    
    def test_fuel_consumption(self, mock_db):
        """Test fuel consumption during movement."""
        vehicle = Vehicle(
            mock_db,
            name="Bus 1",
            current_fuel=100,
            fuel_consumption_rate=10.0  # 10L/100km
        )
        
        vehicle._consume_fuel(distance_km=100)
        
        assert vehicle.current_fuel < 100
        assert vehicle.current_fuel == 90  # 100 - 10
    
    def test_refuel(self, mock_db):
        """Test refueling."""
        vehicle = Vehicle(
            mock_db,
            name="Bus 1",
            current_fuel=30,
            fuel_capacity=100
        )
        
        cost = vehicle.refuel(amount=50)
        
        assert vehicle.current_fuel == 80
        assert cost > 0
    
    def test_refuel_full_tank(self, mock_db):
        """Test refueling to full capacity."""
        vehicle = Vehicle(
            mock_db,
            name="Bus 1",
            current_fuel=30,
            fuel_capacity=100
        )
        
        cost = vehicle.refuel()  # No amount = fill tank
        
        assert vehicle.current_fuel == 100
        assert cost > 0


class TestAccidents:
    """Tests for accident simulation."""
    
    def test_trigger_accident(self, mock_db):
        """Test accident simulation."""
        vehicle = Vehicle(
            mock_db,
            name="Bus 1",
            condition_percent=100,
            current_passengers=20,
            id=1
        )
        
        initial_condition = vehicle.condition_percent
        vehicle.trigger_accident(severity='minor')
        
        assert vehicle.status == VehicleStatus.BROKEN
        assert vehicle.condition_percent < initial_condition
        assert vehicle.accidents_count == 1
        assert len(mock_db.incidents) == 1
    
    def test_severe_accident_has_casualties(self, mock_db):
        """Test that severe accidents may have casualties."""
        vehicle = Vehicle(
            mock_db,
            name="Train 1",
            current_passengers=100,
            id=1
        )
        
        vehicle.trigger_accident(severity='severe')
        
        assert len(mock_db.incidents) == 1
        incident = mock_db.incidents[0]
        # Severe accidents with passengers may have casualties
        # (randomized, so just check structure)
        assert 'injuries_count' in incident
        assert 'fatalities_count' in incident


class TestVehicleTypes:
    """Tests for specialized vehicle types."""
    
    def test_create_train(self, mock_db):
        """Test creating a train."""
        train = Train(
            mock_db,
            name="Steam Train 1"
        )
        
        assert train.type == 'train'
        assert train.max_passengers == 200
        assert train.fuel_type == FuelType.COAL
        assert train.wagon_count == 4
    
    def test_train_couple_wagon(self, mock_db):
        """Test coupling wagons to train."""
        train = Train(mock_db, name="Train 1")
        
        initial_wagons = train.wagon_count
        initial_capacity = train.max_passengers
        
        train.couple_wagon()
        
        assert train.wagon_count == initial_wagons + 1
        assert train.max_passengers > initial_capacity
    
    def test_create_bus(self, mock_db):
        """Test creating a bus."""
        bus = Bus(mock_db, name="City Bus 1")
        
        assert bus.type == 'bus'
        assert bus.max_passengers == 40
        assert bus.fuel_type == FuelType.DIESEL
    
    def test_bus_express_mode(self, mock_db):
        """Test bus express mode."""
        bus = Bus(mock_db, name="Express Bus")
        bus.speed_kmh = 50.0
        
        initial_speed = bus.speed_kmh
        bus.enable_express_mode()
        
        assert bus.speed_kmh > initial_speed
    
    def test_create_tram(self, mock_db):
        """Test creating a tram."""
        tram = Tram(mock_db, name="Tram 1")
        
        assert tram.type == 'tram'
        assert tram.fuel_type == FuelType.ELECTRICITY
    
    def test_create_taxi(self, mock_db):
        """Test creating a taxi."""
        taxi = Taxi(mock_db, name="Taxi 1")
        
        assert taxi.type == 'taxi'
        assert taxi.max_passengers == 4
        assert taxi.is_available is True
    
    def test_taxi_accept_ride(self, mock_db):
        """Test taxi accepting a ride."""
        taxi = Taxi(mock_db, name="Taxi 1")
        
        result = taxi.accept_ride(agent_id=1, destination_id=5)
        
        assert result is True
        assert taxi.is_available is False
        assert taxi.status == VehicleStatus.MOVING
    
    def test_taxi_complete_ride(self, mock_db):
        """Test taxi completing a ride."""
        taxi = Taxi(
            mock_db,
            name="Taxi 1",
            meter_rate_per_km=3.0
        )
        
        taxi.accept_ride(agent_id=1, destination_id=5)
        taxi.current_route_id = 1  # Set route for fare calculation
        fare = taxi.complete_ride()
        
        assert taxi.is_available is True
        assert taxi.status == VehicleStatus.IDLE
        assert fare > 0
    
    def test_create_truck(self, mock_db):
        """Test creating a truck."""
        truck = Truck(mock_db, name="Truck 1")
        
        assert truck.type == 'truck'
        assert truck.max_cargo_kg == 10000
    
    def test_truck_attach_trailer(self, mock_db):
        """Test attaching trailer to truck."""
        truck = Truck(mock_db, name="Truck 1")
        
        initial_capacity = truck.max_cargo_kg
        truck.attach_trailer()
        
        assert truck.trailer_attached is True
        assert truck.max_cargo_kg > initial_capacity


class TestVehicleOperational:
    """Tests for operational status checks."""
    
    def test_operational_vehicle(self, mock_db):
        """Test that healthy vehicle is operational."""
        vehicle = Vehicle(
            mock_db,
            name="Good Bus",
            condition_percent=80,
            current_fuel=50
        )
        
        assert vehicle.is_operational() is True
    
    def test_broken_vehicle_not_operational(self, mock_db):
        """Test that broken vehicle is not operational."""
        vehicle = Vehicle(
            mock_db,
            name="Broken Bus",
            status='broken'
        )
        
        assert vehicle.is_operational() is False
    
    def test_low_condition_not_operational(self, mock_db):
        """Test that vehicle with low condition is not operational."""
        vehicle = Vehicle(
            mock_db,
            name="Worn Bus",
            condition_percent=20
        )
        
        assert vehicle.is_operational() is False
    
    def test_low_fuel_not_operational(self, mock_db):
        """Test that vehicle with low fuel is not operational."""
        vehicle = Vehicle(
            mock_db,
            name="Empty Bus",
            current_fuel=5
        )
        
        assert vehicle.is_operational() is False
    
    def test_get_occupancy_percent(self, mock_db):
        """Test occupancy percentage calculation."""
        vehicle = Vehicle(
            mock_db,
            name="Bus 1",
            max_passengers=50,
            current_passengers=25
        )
        
        assert vehicle.get_occupancy_percent() == 50.0
    
    def test_get_status_summary(self, mock_db):
        """Test status summary generation."""
        vehicle = Vehicle(
            mock_db,
            name="Bus 1",
            model="City Bus 2000"
        )
        
        summary = vehicle.get_status_summary()
        
        assert "Bus 1" in summary
        assert "City Bus 2000" in summary
        assert "Status:" in summary


class TestVehicleFinancial:
    """Tests for financial calculations."""
    
    def test_calculate_daily_profit(self, mock_db):
        """Test daily profit calculation."""
        vehicle = Vehicle(
            mock_db,
            name="Bus 1",
            id=1
        )
        
        profit = vehicle.calculate_daily_profit()
        
        assert 'revenue' in profit
        assert 'fuel_cost' in profit
        assert 'maintenance_cost' in profit
        assert 'profit' in profit
    
    def test_depreciate(self, mock_db):
        """Test vehicle depreciation."""
        vehicle = Vehicle(
            mock_db,
            name="Bus 1",
            current_value=100000.0,
            depreciation_rate=0.05
        )
        
        initial_value = vehicle.current_value
        vehicle.depreciate()
        
        assert vehicle.current_value < initial_value
    
    def test_retire_old_vehicle(self, mock_db):
        """Test retiring old vehicle."""
        vehicle = Vehicle(
            mock_db,
            name="Old Bus",
            year_built=1980,
            condition_percent=20
        )
        
        vehicle.retire()
        
        assert vehicle.status == VehicleStatus.RETIRED
        assert vehicle.is_active is False
        assert vehicle.retired_at is not None
