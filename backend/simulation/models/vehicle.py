"""
Sistema completo de veículos para Ferritine.
Implementa gerenciamento de frotas, manutenção, combustível e operações.
"""

from datetime import datetime, timedelta
from typing import Optional, Dict
from enum import Enum
import random


# ===== ENUMS =====


class VehicleStatus(Enum):
    """Status operacional do veículo"""

    IDLE = "idle"  # Parado
    MOVING = "moving"  # Em movimento
    BOARDING = "boarding"  # Embarcando passageiros
    MAINTENANCE = "maintenance"  # Em manutenção
    BROKEN = "broken"  # Quebrado/acidentado
    RETIRED = "retired"  # Aposentado


class FuelType(Enum):
    """Tipos de combustível por era"""

    COAL = "coal"  # Carvão (era do vapor)
    DIESEL = "diesel"  # Diesel
    ELECTRICITY = "electricity"  # Elétrico


# ===== CLASSE PRINCIPAL: VEHICLE =====


class Vehicle:
    """
    Classe base para todos os veículos.
    Gerencia movimento, passageiros, carga, manutenção, combustível.
    """

    def __init__(self, db, **kwargs):
        self.db = db

        # Identificação
        self.id = kwargs.get("id")
        self.name = kwargs.get("name", "Veículo Sem Nome")
        self.type = kwargs.get("type", "generic")
        self.model = kwargs.get("model", "Modelo Padrão")
        self.year_built = kwargs.get("year_built", 1900)

        # Localização e movimento
        self.current_x = kwargs.get("current_x", 0)
        self.current_y = kwargs.get("current_y", 0)
        self.speed_kmh = kwargs.get("speed_kmh", 0.0)
        self.position_on_route = kwargs.get("position_on_route", 0.0)  # 0.0 a 1.0
        self.current_route_id = kwargs.get("current_route_id")
        self.next_station_id = kwargs.get("next_station_id")

        # Capacidades
        self.max_passengers = kwargs.get("max_passengers", 0)
        self.current_passengers = kwargs.get("current_passengers", 0)
        self.max_cargo_kg = kwargs.get("max_cargo_kg", 0)
        self.current_cargo_kg = kwargs.get("current_cargo_kg", 0)

        # Tripulação
        self.crew_size = kwargs.get("crew_size", 1)
        self.driver_id = kwargs.get("driver_id")

        # Status operacional
        self.status = VehicleStatus(kwargs.get("status", VehicleStatus.IDLE.value))
        self.is_active = kwargs.get("is_active", True)

        # Combustível
        self.fuel_type = FuelType(kwargs.get("fuel_type", FuelType.DIESEL.value))
        self.fuel_capacity = kwargs.get("fuel_capacity", 100.0)
        self.current_fuel = kwargs.get("current_fuel", 100.0)
        self.fuel_consumption_rate = kwargs.get("fuel_consumption_rate", 10.0)  # L/100km

        # Condição e manutenção
        self.condition_percent = kwargs.get("condition_percent", 100.0)
        self.wear_rate = kwargs.get("wear_rate", 0.1)  # % por km
        self.last_maintenance_date = kwargs.get("last_maintenance_date")
        self.next_maintenance_due = kwargs.get("next_maintenance_due")
        self.repairs_count = kwargs.get("repairs_count", 0)

        # Estatísticas
        self.total_km_traveled = kwargs.get("total_km_traveled", 0.0)
        self.accidents_count = kwargs.get("accidents_count", 0)

        # Financeiro
        self.purchase_price = kwargs.get("purchase_price", 100000.0)
        self.current_value = kwargs.get("current_value", 100000.0)
        self.depreciation_rate = kwargs.get("depreciation_rate", 0.05)  # 5% ao ano

        # Metadata
        self.created_at = kwargs.get("created_at", datetime.now())
        self.retired_at = kwargs.get("retired_at")

    # ===== MOVIMENTO E NAVEGAÇÃO =====

    def start_trip(self, route_id: int, station_id: int = None):
        """
        Inicia uma viagem em uma rota
        """
        if not self.is_operational():
            print(f"⚠️ {self.name} não está operacional para iniciar viagem")
            return False

        self.current_route_id = route_id
        self.next_station_id = station_id
        self.position_on_route = 0.0
        self.status = VehicleStatus.MOVING

        # Cria registro de viagem no banco
        trip_id = self.db.create_trip(vehicle_id=self.id, route_id=route_id, started_at=datetime.now())

        return trip_id

    def move(self, delta_time_hours: float):
        """
        Atualiza posição do veículo

        Args:
            delta_time_hours: Tempo decorrido em horas
        """
        if self.status != VehicleStatus.MOVING:
            return

        # Calcula distância percorrida
        distance_km = self.speed_kmh * delta_time_hours

        # Atualiza estatísticas
        self.total_km_traveled += distance_km

        # Consome combustível
        self._consume_fuel(distance_km)

        # Aplica desgaste
        self._apply_wear(distance_km)

        # Atualiza posição na rota (simplificado)
        route = self.get_route()
        if route:
            total_distance = route["total_distance_km"]
            if total_distance > 0:
                self.position_on_route = min(1.0, self.position_on_route + (distance_km / total_distance))

    def arrive_at_station(self, station_id: int):
        """
        Chega em uma estação
        """
        self.status = VehicleStatus.BOARDING
        self.next_station_id = station_id
        self.speed_kmh = 0.0

        print(f"🚏 {self.name} chegou na estação {station_id}")

    def complete_trip(self):
        """
        Finaliza viagem atual
        """
        if self.current_route_id:
            self.db.complete_trip(self.id)

        self.status = VehicleStatus.IDLE
        self.current_route_id = None
        self.position_on_route = 0.0
        self.speed_kmh = 0.0

        # Desembarca todos os passageiros
        self.current_passengers = 0

    # ===== PASSAGEIROS E CARGA =====

    def board_passenger(self, agent_id: int, boarding_station_id: int, fare: float = 5.0):
        """
        Embarca um passageiro
        """
        if self.current_passengers >= self.max_passengers:
            return False

        self.current_passengers += 1

        # Cria ticket
        self.db.create_ticket(
            agent_id=agent_id, vehicle_id=self.id, boarding_station_id=boarding_station_id, fare_paid=fare
        )

        return True

    def alight_passenger(self, agent_id: int, alighting_station_id: int):
        """
        Desembarca um passageiro
        """
        if self.current_passengers > 0:
            self.current_passengers -= 1

            # Completa ticket
            self.db.complete_ticket(agent_id, self.id, alighting_station_id)

    def load_cargo(self, weight_kg: float):
        """
        Carrega mercadoria
        """
        if self.current_cargo_kg + weight_kg <= self.max_cargo_kg:
            self.current_cargo_kg += weight_kg
            return True
        return False

    def unload_cargo(self, weight_kg: float):
        """
        Descarrega mercadoria
        """
        if weight_kg <= self.current_cargo_kg:
            self.current_cargo_kg -= weight_kg
            return True
        return False

    # ===== MANUTENÇÃO =====

    def _apply_wear(self, distance_km: float):
        """
        Aplica desgaste baseado na distância percorrida
        """
        wear_amount = distance_km * self.wear_rate
        self.condition_percent = max(0, self.condition_percent - wear_amount)

        # Verifica se precisa manutenção
        if self.condition_percent < 50 and not self.next_maintenance_due:
            self.schedule_maintenance()

    def schedule_maintenance(self, days_ahead: int = 7):
        """
        Agenda manutenção preventiva
        """
        self.next_maintenance_due = datetime.now() + timedelta(days=days_ahead)
        print(f"{self.name}: Manutenção agendada para {self.next_maintenance_due.date()}")

    def perform_maintenance(self, maintenance_type: str = "preventive"):
        """
        Realiza manutenção
        """
        self.status = VehicleStatus.MAINTENANCE

        # Calcula custos
        parts_cost = (100 - self.condition_percent) * 50  # R$ 50 por % de condição perdida
        labor_cost = 200 + (self.crew_size * 100)

        # Registra manutenção
        self.db.create_maintenance_record(
            vehicle_id=self.id,
            type=maintenance_type,
            parts_cost=parts_cost,
            labor_cost=labor_cost,
            started_at=datetime.now(),
        )

        # Restaura condição
        self.condition_percent = 100
        self.last_maintenance_date = datetime.now()
        self.next_maintenance_due = None
        self.repairs_count += 1

        # Retorna ao serviço após algumas horas
        self.status = VehicleStatus.IDLE

        return parts_cost + labor_cost

    # ===== COMBUSTÍVEL =====

    def _consume_fuel(self, distance_km: float):
        """
        Consome combustível baseado na distância
        """
        consumption = (distance_km / 100) * self.fuel_consumption_rate
        self.current_fuel = max(0, self.current_fuel - consumption)

        # Combustível baixo
        if self.current_fuel < 20 and self.status == VehicleStatus.MOVING:
            print(f"⚠️ {self.name}: Combustível baixo ({self.current_fuel:.1f}%)")

    def refuel(self, amount: float = None):
        """
        Abastece o veículo

        Args:
            amount: Quantidade a abastecer (None = encher tanque)
        """
        if amount is None:
            amount = self.fuel_capacity - self.current_fuel

        self.current_fuel = min(self.fuel_capacity, self.current_fuel + amount)

        # Custo do combustível
        fuel_prices = {
            FuelType.COAL: 0.5,  # R$ 0.50/kg
            FuelType.DIESEL: 5.0,  # R$ 5.00/L
            FuelType.ELECTRICITY: 0.8,  # R$ 0.80/kWh
        }

        cost = amount * fuel_prices[self.fuel_type]
        return cost

    # ===== ACIDENTES =====

    def trigger_accident(self, severity: str = "minor", cause: str = "unknown"):
        """
        Registra um acidente
        """
        self.status = VehicleStatus.BROKEN
        self.accidents_count += 1

        # Danos
        damage_ranges = {"minor": (5, 15), "moderate": (15, 40), "severe": (40, 70), "fatal": (70, 100)}

        damage_percent = random.randint(*damage_ranges[severity])
        self.condition_percent = max(0, self.condition_percent - damage_percent)

        # Vítimas (se houver passageiros)
        injuries = 0
        fatalities = 0

        if self.current_passengers > 0:
            if severity in ["severe", "fatal"]:
                fatalities = random.randint(0, self.current_passengers // 3)
                injuries = random.randint(0, self.current_passengers // 2)

        # Registra incidente
        self.db.create_incident(
            vehicle_id=self.id,
            type="accident",
            severity=severity,
            cause=cause,
            vehicle_damage_percent=damage_percent,
            injuries_count=injuries,
            fatalities_count=fatalities,
            occurred_at=datetime.now(),
        )

        print(f"🚨 ACIDENTE: {self.name} - Severidade: {severity}")
        if fatalities > 0:
            print(f"   💀 Fatalidades: {fatalities}")
        if injuries > 0:
            print(f"   🤕 Feridos: {injuries}")

    # ===== FINANCEIRO =====

    def calculate_daily_profit(self) -> Dict[str, float]:
        """
        Calcula lucro/prejuízo diário
        """
        stats = self.db.get_vehicle_stats(self.id, datetime.now().date())

        return {
            "revenue": stats["total_revenue"],
            "fuel_cost": stats["total_fuel_cost"],
            "maintenance_cost": stats["total_maintenance_cost"],
            "profit": stats["profit"],
        }

    def depreciate(self):
        """
        Aplica depreciação anual ao valor do veículo
        """
        self.current_value *= 1 - self.depreciation_rate

        # Veículo muito antigo pode ser aposentado
        age = datetime.now().year - self.year_built
        if age > 40 and self.condition_percent < 30:
            self.retire()

    def retire(self):
        """
        Aposenta o veículo (remove de operação)
        """
        self.status = VehicleStatus.RETIRED
        self.retired_at = datetime.now()
        self.is_active = False

        print(f"🛑 {self.name} foi aposentado após {self.total_km_traveled:.0f}km rodados")

    # ===== HELPERS =====

    def get_route(self) -> Optional[Dict]:
        """
        Retorna informações da rota atual
        """
        if not self.current_route_id:
            return None
        return self.db.get_route(self.current_route_id)

    def get_occupancy_percent(self) -> float:
        """
        Retorna ocupação percentual
        """
        if self.max_passengers == 0:
            return 0.0
        return (self.current_passengers / self.max_passengers) * 100

    def is_operational(self) -> bool:
        """
        Verifica se o veículo está em condições de operar
        """
        return (
            self.is_active
            and self.status not in [VehicleStatus.BROKEN, VehicleStatus.RETIRED]
            and self.condition_percent >= 30
            and self.current_fuel >= 10
        )

    def get_status_summary(self) -> str:
        """
        Retorna resumo do status atual
        """
        return f"""
🚂 {self.name} ({self.model})
━━━━━━━━━━━━━━━━━━━━━━━━━
Status: {self.status.value}
Posição: Rota {self.current_route_id}, {self.position_on_route*100:.1f}%
Velocidade: {self.speed_kmh:.1f} km/h
Passageiros: {self.current_passengers}/{self.max_passengers} ({self.get_occupancy_percent():.1f}%)
Condição: {self.condition_percent}%
Combustível: {self.current_fuel:.1f}/{self.fuel_capacity} ({self.current_fuel/self.fuel_capacity*100:.1f}%)
Quilometragem: {self.total_km_traveled:.0f} km
Acidentes: {self.accidents_count}
        """.strip()

    # ===== PERSISTÊNCIA =====

    def save(self):
        """
        Salva veículo no banco de dados
        """
        if self.id is None:
            # Criar novo registro
            self.id = self.db.create_vehicle(self.__dict__)
        else:
            # Atualizar existente
            self.db.update_vehicle(self.id, self.__dict__)

    @classmethod
    def load(cls, db, vehicle_id: int):
        """
        Carrega veículo do banco de dados
        """
        data = db.get_vehicle(vehicle_id)
        if data:
            return cls(db, **data)
        return None


# ===== CLASSES ESPECIALIZADAS =====


class Train(Vehicle):
    """
    Trem - Maior capacidade, trilhos fixos
    """

    def __init__(self, db, **kwargs):
        kwargs.setdefault("type", "train")
        kwargs.setdefault("max_passengers", 200)
        kwargs.setdefault("max_cargo_kg", 5000)
        kwargs.setdefault("fuel_type", "coal")  # Era do vapor
        kwargs.setdefault("crew_size", 2)  # Maquinista + Foguista
        super().__init__(db, **kwargs)

        # Atributos específicos de trem
        self.wagon_count = kwargs.get("wagon_count", 4)
        self.has_dining_car = kwargs.get("has_dining_car", False)
        self.track_gauge = kwargs.get("track_gauge", "standard")  # bitola

    def couple_wagon(self):
        """
        Acopla vagão adicional
        """
        self.wagon_count += 1
        self.max_passengers += 50
        self.max_cargo_kg += 1000
        print(f"{self.name}: Vagão acoplado (total: {self.wagon_count})")

    def decouple_wagon(self):
        """
        Desacopla vagão
        """
        if self.wagon_count > 1:
            self.wagon_count -= 1
            self.max_passengers -= 50
            self.max_cargo_kg -= 1000
            print(f"{self.name}: Vagão desacoplado (total: {self.wagon_count})")


class Bus(Vehicle):
    """
    Ônibus - Flexível, ruas urbanas
    """

    def __init__(self, db, **kwargs):
        kwargs.setdefault("type", "bus")
        kwargs.setdefault("max_passengers", 40)
        kwargs.setdefault("fuel_type", "diesel")
        kwargs.setdefault("crew_size", 2)  # Motorista + Cobrador
        super().__init__(db, **kwargs)

        # Atributos específicos
        self.has_air_conditioning = kwargs.get("has_air_conditioning", False)
        self.is_articulated = kwargs.get("is_articulated", False)
        self.accessibility_features = kwargs.get("accessibility_features", [])

    def enable_express_mode(self):
        """
        Modo expresso (menos paradas)
        """
        self.speed_kmh *= 1.3
        print(f"{self.name}: Modo expresso ativado")


class Tram(Vehicle):
    """
    Bonde - Elétrico, trilhos urbanos
    """

    def __init__(self, db, **kwargs):
        kwargs.setdefault("type", "tram")
        kwargs.setdefault("max_passengers", 60)
        kwargs.setdefault("fuel_type", "electricity")
        kwargs.setdefault("crew_size", 1)
        super().__init__(db, **kwargs)

        self.pantograph_type = kwargs.get("pantograph_type", "overhead")


class Taxi(Vehicle):
    """
    Táxi - Individual, sob demanda
    """

    def __init__(self, db, **kwargs):
        kwargs.setdefault("type", "taxi")
        kwargs.setdefault("max_passengers", 4)
        kwargs.setdefault("fuel_type", "diesel")
        kwargs.setdefault("crew_size", 1)
        super().__init__(db, **kwargs)

        self.is_available = kwargs.get("is_available", True)
        self.current_fare = kwargs.get("current_fare", 0.0)
        self.meter_rate_per_km = kwargs.get("meter_rate_per_km", 3.0)

    def accept_ride(self, agent_id: int, destination_id: int):
        """
        Aceita corrida
        """
        if not self.is_available:
            return False

        self.is_available = False
        self.driver_id = agent_id
        self.next_station_id = destination_id
        self.status = VehicleStatus.MOVING
        return True

    def complete_ride(self) -> float:
        """
        Finaliza corrida e calcula tarifa
        """
        route = self.get_route()
        if route:
            distance = route["total_distance_km"]
            self.current_fare = distance * self.meter_rate_per_km + 5.0  # R$ 5 bandeirada

        self.is_available = True
        self.status = VehicleStatus.IDLE
        return self.current_fare


class Truck(Vehicle):
    """
    Caminhão - Carga pesada
    """

    def __init__(self, db, **kwargs):
        kwargs.setdefault("type", "truck")
        kwargs.setdefault("max_cargo_kg", 10000)
        kwargs.setdefault("fuel_type", "diesel")
        super().__init__(db, **kwargs)

        self.trailer_attached = kwargs.get("trailer_attached", False)

    def attach_trailer(self):
        """
        Acopla reboque
        """
        if not self.trailer_attached:
            self.trailer_attached = True
            self.max_cargo_kg *= 2
            self.fuel_consumption_rate *= 1.5
            print(f"{self.name}: Reboque acoplado")
