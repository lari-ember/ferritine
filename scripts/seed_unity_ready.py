#!/usr/bin/env python3
"""
Seed de dados mínimos para testar integração Unity.
Cria um mundo funcional com 1 operadora, 1 rota, 5 estações, 3 veículos e 50 agentes.
"""

import sys
from pathlib import Path
sys.path.insert(0, str(Path(__file__).parent.parent))

from backend.database.connection import DatabaseManager, get_session
from backend.database.models import (
    TransportOperator, Route, Station, Vehicle, Agent, Schedule, RouteStation,
    StationType, VehicleStatus, AgentStatus, Gender, CreatedBy, HealthStatus
)
from datetime import datetime, time
from decimal import Decimal
import uuid

def seed_minimal_world():
    """Popula banco com dados mínimos para Unity consumir."""

    print("🌱 Iniciando seed de dados para Unity...")

    # Inicializar banco (usar SQLite para facilidade)
    print("   📁 Usando SQLite (data/db/ferritine.db)")
    db_manager = DatabaseManager(use_sqlite=True)
    db_manager.init_database()

    session = get_session()

    try:
        # Limpar dados existentes (ativa por ser execução de seed idempotente)
        # Ordem de deleção respeitando chaves estrangeiras
        print("   🧹 Limpando dados antigos (se existirem)...")
        session.query(Schedule).delete()
        session.query(RouteStation).delete()
        session.query(Vehicle).delete()
        session.query(Agent).delete()
        session.query(Station).delete()
        session.query(Route).delete()
        session.query(TransportOperator).delete()
        session.commit()
        print("   ✅ Limpeza concluída")

        # ==================== 1. CRIAR OPERADORA ====================
        print("📍 Criando operadora...")
        metro_sp = TransportOperator(
            id=uuid.uuid4(),
            name="Metrô de São Paulo",
            operator_type=StationType.METRO_STATION,
            revenue=Decimal("1000000.00"),
            operational_costs=Decimal("750000.00"),
            is_active=True
        )
        session.add(metro_sp)
        session.flush()
        print(f"   ✅ Operadora criada: {metro_sp.name}")

        # ==================== 2. CRIAR ROTA ====================
        print("📍 Criando rota...")
        route_blue = Route(
            id=uuid.uuid4(),
            name="Linha 1 - Azul",
            code="L1",
            route_type=StationType.METRO_STATION,
            operator_id=metro_sp.id,
            fare_base=Decimal("4.40"),
            frequency_minutes=5,
            operating_hours_start=time(4, 40),
            operating_hours_end=time(0, 0),
            is_active=True
        )
        session.add(route_blue)
        session.flush()
        print(f"   ✅ Rota criada: {route_blue.name}")

        # ==================== 3. CRIAR ESTAÇÕES ====================
        print("📍 Criando estações...")
        stations = [
            Station(
                id=uuid.uuid4(),
                name="Jabaquara",
                station_type=StationType.METRO_STATION,
                x=0, y=0, z=0,
                max_queue_length=100,
                current_queue_length=0,
                is_operational=True
            ),
            Station(
                id=uuid.uuid4(),
                name="Conceição",
                station_type=StationType.METRO_STATION,
                x=0, y=10, z=0,
                max_queue_length=80,
                current_queue_length=0,
                is_operational=True
            ),
            Station(
                id=uuid.uuid4(),
                name="São Judas",
                station_type=StationType.METRO_STATION,
                x=0, y=20, z=0,
                max_queue_length=80,
                current_queue_length=0,
                is_operational=True
            ),
            Station(
                id=uuid.uuid4(),
                name="Saúde",
                station_type=StationType.METRO_STATION,
                x=0, y=30, z=0,
                max_queue_length=120,
                current_queue_length=0,
                is_operational=True
            ),
            Station(
                id=uuid.uuid4(),
                name="Praça da Árvore",
                station_type=StationType.METRO_STATION,
                x=0, y=40, z=0,
                max_queue_length=100,
                current_queue_length=0,
                is_operational=True
            ),
        ]

        for station in stations:
            session.add(station)
        session.flush()
        print(f"   ✅ {len(stations)} estações criadas")

        # ==================== 4. ASSOCIAR ESTAÇÕES À ROTA ====================
        print("📍 Associando estações à rota...")
        for i, station in enumerate(stations):
            route_station = RouteStation(
                route_id=route_blue.id,
                station_id=station.id,
                sequence_order=i + 1,  # Começa de 1, não 0
                travel_time_from_previous=3 if i > 0 else 0,
                distance_from_previous_km=1.5 if i > 0 else 0.0
            )
            session.add(route_station)
        session.flush()
        print(f"   ✅ Estações associadas à rota")

        # ==================== 5. CRIAR VEÍCULOS ====================
        print("📍 Criando veículos...")
        vehicles = []
        for i in range(3):
            vehicle = Vehicle(
                id=uuid.uuid4(),
                name=f"Trem {i+1:02d}",
                vehicle_type="metro_train",
                passenger_capacity=1200,
                current_passengers=0,
                operator_id=metro_sp.id,
                current_route_id=route_blue.id,
                current_station_id=stations[i % len(stations)].id,
                status=VehicleStatus.ACTIVE,
                fuel_capacity=100.0,  # Definir ANTES de current_fuel
                current_fuel=100.0
            )
            session.add(vehicle)
            vehicles.append(vehicle)
        session.flush()
        print(f"   ✅ {len(vehicles)} veículos criados")

        # ==================== 6. CRIAR HORÁRIOS ====================
        print("📍 Criando horários...")
        departure_times = [time(5, 0), time(5, 10), time(5, 20)]
        for i, dept_time in enumerate(departure_times):
            schedule = Schedule(
                id=uuid.uuid4(),
                route_id=route_blue.id,
                vehicle_id=vehicles[i].id,
                departure_time=dept_time,
                days_of_week=[0, 1, 2, 3, 4],  # Segunda a sexta
                is_active=True
            )
            session.add(schedule)
        session.flush()
        print(f"   ✅ {len(departure_times)} horários criados")

        # ==================== 7. CRIAR AGENTES ====================
        print("📍 Criando agentes...")
        for i in range(50):
            agent = Agent(
                id=uuid.uuid4(),
                name=f"Cidadão {i+1:03d}",
                birth_date=datetime(1990, 1, 1),
                gender=Gender.CIS_MALE if i % 2 == 0 else Gender.CIS_FEMALE,
                created_by=CreatedBy.IA,
                health_status=HealthStatus.HEALTHY,
                energy_level=100,
                wallet=Decimal("100.00"),
                employer_id=metro_sp.id if i < 5 else None,  # 5 funcionários da operadora
                current_status=AgentStatus.IDLE,
                version="0.2.0"
            )
            session.add(agent)
        session.flush()
        print(f"   ✅ 50 agentes criados")

        # ==================== 8. SIMULAR FILAS INICIAIS ====================
        print("📍 Simulando filas iniciais...")
        stations[0].current_queue_length = 15  # Jabaquara
        stations[1].current_queue_length = 8   # Conceição
        stations[2].current_queue_length = 12  # São Judas
        stations[3].current_queue_length = 20  # Saúde
        stations[4].current_queue_length = 10  # Praça da Árvore

        # Simular passageiros em veículos
        vehicles[0].current_passengers = 450
        vehicles[1].current_passengers = 680
        vehicles[2].current_passengers = 320

        print(f"   ✅ Filas e passageiros simulados")

        # ==================== COMMIT ====================
        session.commit()

        # Salvar dados antes de fechar sessão (para evitar problema de reload de enums)
        operator_name = "Metrô de São Paulo"
        route_name = "Linha 1 - Azul"
        num_stations = len(stations)
        num_vehicles = len(vehicles)
        num_schedules = len(departure_times)
        total_queue = sum(s.current_queue_length for s in stations)
        total_in_vehicles = sum(v.current_passengers for v in vehicles)

        print("\n✅ SEED COMPLETO!")
        print("\n📊 RESUMO:")
        print(f"   - 1 Operadora: {operator_name}")
        print(f"   - 1 Rota: {route_name}")
        print(f"   - {num_stations} Estações")
        print(f"   - {num_vehicles} Veículos")
        print(f"   - 50 Agentes")
        print(f"   - {num_schedules} Horários")
        print(f"   - {total_queue} Passageiros em filas")
        print(f"   - {total_in_vehicles} Passageiros em veículos")

        print("\n🚀 PRÓXIMOS PASSOS:")
        print("   1. Rodar API: python -m backend.api.main")
        print("   2. Testar: curl http://localhost:5000/api/world/state")
        print("   3. Conectar Unity ao endpoint acima")

    except Exception as e:
        session.rollback()
        print(f"\n❌ ERRO ao fazer seed: {e}")
        import traceback
        traceback.print_exc()
    finally:
        session.close()

if __name__ == "__main__":
    seed_minimal_world()
