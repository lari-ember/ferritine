#!/usr/bin/env python3
"""
Script para criar issues do sistema de transporte no GitHub.

Uso:
    python scripts/create_transport_issues.py --token YOUR_GITHUB_TOKEN

Requer:
    pip install requests python-dotenv
"""

import argparse
import os
import sys
from typing import Dict, List
import requests
from dotenv import load_dotenv

# Carregar variáveis de ambiente
load_dotenv()

# Configurações
REPO_OWNER = "lari-ember"
REPO_NAME = "ferritine"
GITHUB_API = "https://api.github.com"


class TransportIssuesCreator:
    """Cria issues do sistema de transporte no GitHub"""

    def __init__(self, token: str):
        self.token = token
        self.headers = {
            "Authorization": f"token {token}",
            "Accept": "application/vnd.github.v3+json"
        }
        self.base_url = f"{GITHUB_API}/repos/{REPO_OWNER}/{REPO_NAME}"

    def create_issue(self, title: str, body: str, labels: List[str], milestone: int = None) -> Dict:
        """Cria uma issue no repositório"""
        url = f"{self.base_url}/issues"
        data = {
            "title": title,
            "body": body,
            "labels": labels
        }
        
        if milestone:
            data["milestone"] = milestone
        
        response = requests.post(url, headers=self.headers, json=data)
        
        if response.status_code == 201:
            issue_number = response.json()["number"]
            print(f"✅ Issue #{issue_number} criada: {title}")
            return response.json()
        else:
            print(f"❌ Erro ao criar issue {title}: {response.status_code}")
            print(f"   {response.json()}")
            return {}

    def get_milestones(self) -> Dict[str, int]:
        """Obtém lista de milestones existentes"""
        url = f"{self.base_url}/milestones"
        response = requests.get(url, headers=self.headers)
        
        if response.status_code == 200:
            milestones = {}
            for ms in response.json():
                milestones[ms["title"]] = ms["number"]
            return milestones
        return {}

    def create_all_transport_issues(self):
        """Cria todas as issues do sistema de transporte"""
        print("\n🚂 Criando Issues do Sistema de Transporte...\n")

        # Obtém milestone (ajustar conforme necessário)
        milestones = self.get_milestones()
        milestone_number = milestones.get("Milestone 1.4: Transporte Ferroviário Virtual", None)
        
        issues = [
            {
                "title": "Issue 4.1: Enum e Modelo Base Station",
                "body": """## Objetivo
Implementar o modelo base de estações de transporte com enum de tipos.

## Dependências
Nenhuma

## Tarefas
- [ ] Criar `StationType` enum em `backend/database/models.py`
- [ ] Implementar modelo `Station` com campos básicos:
  - `id` (UUID, PK)
  - `building_id` (FK → buildings.id)
  - `name` (String)
  - `type` (StationType)
  - `platform_count` (Integer, default=1)
  - `max_vehicles_docked` (Integer, default=2)
  - `has_shelter` (Boolean)
  - `has_ticket_office` (Boolean)
  - `has_restrooms` (Boolean)
  - `condition_percent` (Integer, 0-100)
  - `last_inspection_date` (DateTime)
  - `created_at`, `updated_at`
- [ ] Adicionar relacionamento `Station.building`
- [ ] Implementar métodos:
  - `can_dock_vehicle()` - verifica capacidade
  - `get_waiting_passengers()` - conta passageiros esperando
  - `perform_inspection()` - registra inspeção
- [ ] Adicionar migrations Alembic
- [ ] Criar testes:
  - `test_create_station`
  - `test_station_capacity`
  - `test_dock_vehicle`
  - `test_station_building_relationship`

## Arquivos Afetados
- `backend/database/models.py`
- `migrations/versions/`
- `tests/unit/test_models.py`

## StationType Enum
```python
class StationType(str, Enum):
    TRAIN_STATION = "train_station"
    BUS_STOP = "bus_stop"
    BUS_TERMINAL = "bus_terminal"
    TRAM_STOP = "tram_stop"
    TAXI_STAND = "taxi_stand"
    TRUCK_DEPOT = "truck_depot"
    METRO_STATION = "metro_station"
    AIRPORT = "airport"
    PORT = "port"
```
""",
                "labels": ["feat", "area: transport", "area: database", "phase-1: digital", "priority: high"]
            },
            {
                "title": "Issue 4.2: StationQueries",
                "body": """## Objetivo
Implementar queries para operações com estações.

## Dependências
- Issue 4.1

## Tarefas
- [ ] Criar classe `StationQueries` em `backend/database/queries.py`
- [ ] Implementar métodos:
  - `get_by_id(station_id: UUID)` - busca por ID
  - `get_all(active_only: bool = True)` - retorna todas estações
  - `get_by_type(station_type: StationType)` - filtra por tipo
  - `get_by_building(building_id: UUID)` - estação de um edifício
  - `get_nearest_station(x: int, y: int, station_type: StationType = None)` - mais próxima
  - `get_available_for_docking(vehicle_type: str)` - com capacidade
  - `get_waiting_passengers_count(station_id: UUID)` - conta passageiros
  - `update_condition(station_id: UUID, condition: int)` - atualiza condição
  - `create(**kwargs)` - cria nova estação
  - `update(station_id: UUID, **kwargs)` - atualiza estação
- [ ] Adicionar `self.stations = StationQueries(session)` em `DatabaseQueries.__init__`
- [ ] Criar testes unitários para todas as queries

## Arquivos Afetados
- `backend/database/queries.py`
- `tests/unit/test_queries.py`
""",
                "labels": ["feat", "area: transport", "area: database", "phase-1: digital", "priority: high"]
            },
            {
                "title": "Issue 4.3: Modificações no Modelo Vehicle",
                "body": """## Objetivo
Adicionar campos relacionados a estações e rotas no modelo Vehicle.

## Dependências
- Issue 4.1

## Tarefas
- [ ] Adicionar campos ao modelo `Vehicle`:
  - `current_station_id` (FK → stations.id, nullable)
  - `is_docked` (Boolean, default=False)
  - `assigned_route_id` (FK → routes.id, nullable)
- [ ] Adicionar relacionamento `vehicle.current_station → Station`
- [ ] Criar migration Alembic
- [ ] Atualizar `VehicleQueries` com novos métodos:
  - `get_docked_at_station(station_id: UUID)` - veículos em uma estação
  - `dock_vehicle(vehicle_id: UUID, station_id: UUID)` - acoplar veículo
  - `undock_vehicle(vehicle_id: UUID)` - desacoplar veículo
- [ ] Criar testes:
  - `test_vehicle_docking`
  - `test_vehicle_station_relationship`
  - `test_get_docked_vehicles`

## Arquivos Afetados
- `backend/database/models.py`
- `backend/database/queries.py`
- `migrations/versions/`
- `tests/unit/test_vehicle.py`
""",
                "labels": ["feat", "area: transport", "area: database", "phase-1: digital", "priority: medium"]
            },
            {
                "title": "Issue 4.4: Modificações no Modelo Agent",
                "body": """## Objetivo
Adicionar campos para transporte público no modelo Agent.

## Dependências
- Issue 4.1

## Tarefas
- [ ] Adicionar campos ao modelo `Agent`:
  - `waiting_at_station_id` (FK → stations.id, nullable)
  - `current_ticket_id` (FK → tickets.id, nullable)
- [ ] Adicionar método `purchase_ticket(route_id, origin_id, destination_id)`
- [ ] Criar migration Alembic
- [ ] Atualizar `AgentQueries` com novos métodos:
  - `get_waiting_at_station(station_id: UUID)` - agentes em estação
  - `get_with_active_ticket()` - agentes com bilhete válido
- [ ] Criar testes:
  - `test_agent_at_station`
  - `test_agent_purchase_ticket`
  - `test_get_waiting_agents`

## Arquivos Afetados
- `backend/database/models.py`
- `backend/database/queries.py`
- `migrations/versions/`
- `tests/unit/test_agent.py`
""",
                "labels": ["feat", "area: transport", "area: agents", "area: database", "phase-1: digital", "priority: medium"]
            },
            {
                "title": "Issue 4.5: Modelo Route e RouteStation",
                "body": """## Objetivo
Implementar modelos de rotas de transporte e associação com estações.

## Dependências
- Issue 4.1

## Tarefas
- [ ] Criar modelo `Route`:
  - `id` (UUID, PK)
  - `name` (String)
  - `route_type` (StationType)
  - `operating_hours_start` (Time)
  - `operating_hours_end` (Time)
  - `frequency_minutes` (Integer)
  - `is_active` (Boolean)
  - `created_at`, `updated_at`
- [ ] Criar modelo `RouteStation` (tabela associativa):
  - `route_id` (UUID, FK)
  - `station_id` (UUID, FK)
  - `sequence_order` (Integer)
  - `estimated_stop_minutes` (Integer)
  - Constraint unique (route_id, sequence_order)
- [ ] Adicionar relacionamentos
- [ ] Criar migrations Alembic
- [ ] Criar testes:
  - `test_create_route`
  - `test_route_stations_order`
  - `test_route_active_status`
  - `test_route_schedule`

## Arquivos Afetados
- `backend/database/models.py`
- `migrations/versions/`
- `tests/unit/test_models.py`
""",
                "labels": ["feat", "area: transport", "area: database", "phase-1: digital", "priority: high"]
            },
            {
                "title": "Issue 4.6: RouteQueries",
                "body": """## Objetivo
Implementar queries para operações com rotas.

## Dependências
- Issue 4.5

## Tarefas
- [ ] Criar classe `RouteQueries` em `backend/database/queries.py`
- [ ] Implementar métodos:
  - `get_by_id(route_id: UUID)`
  - `get_all(active_only: bool = True)`
  - `get_active_routes(route_type: StationType = None)`
  - `get_by_type(route_type: StationType)`
  - `get_routes_through_station(station_id: UUID)`
  - `get_next_vehicle_arrival(station_id: UUID, route_id: UUID)`
  - `calculate_trip_duration(route_id: UUID, origin_id: UUID, destination_id: UUID)`
  - `add_station_to_route(route_id: UUID, station_id: UUID, sequence: int)`
  - `remove_station_from_route(route_id: UUID, station_id: UUID)`
  - `create(**kwargs)`
  - `update(route_id: UUID, **kwargs)`
- [ ] Adicionar `self.routes = RouteQueries(session)` em `DatabaseQueries`
- [ ] Criar testes unitários

## Arquivos Afetados
- `backend/database/queries.py`
- `tests/unit/test_queries.py`
""",
                "labels": ["feat", "area: transport", "area: database", "phase-1: digital", "priority: high"]
            },
            {
                "title": "Issue 4.7: Modelo Ticket",
                "body": """## Objetivo
Implementar sistema de bilhetes/passagens.

## Dependências
- Issue 4.5
- Issue 4.4

## Tarefas
- [ ] Criar modelo `Ticket`:
  - `id` (UUID, PK)
  - `agent_id` (UUID, FK)
  - `route_id` (UUID, FK)
  - `origin_station_id` (UUID, FK)
  - `destination_station_id` (UUID, FK)
  - `price` (Decimal)
  - `purchase_time` (DateTime)
  - `is_used` (Boolean)
  - `valid_until` (DateTime)
  - `created_at`
- [ ] Adicionar métodos:
  - `validate()` - verifica se bilhete é válido
  - `use()` - marca bilhete como usado
  - `is_expired()` - verifica expiração
- [ ] Adicionar relacionamentos
- [ ] Criar migration Alembic
- [ ] Criar testes:
  - `test_create_ticket`
  - `test_validate_ticket`
  - `test_use_ticket`
  - `test_expired_ticket`

## Arquivos Afetados
- `backend/database/models.py`
- `migrations/versions/`
- `tests/unit/test_models.py`
""",
                "labels": ["feat", "area: transport", "area: economy", "area: database", "phase-1: digital", "priority: high"]
            },
            {
                "title": "Issue 4.8: TicketQueries",
                "body": """## Objetivo
Implementar queries para operações com bilhetes.

## Dependências
- Issue 4.7

## Tarefas
- [ ] Criar classe `TicketQueries` em `backend/database/queries.py`
- [ ] Implementar métodos:
  - `get_by_id(ticket_id: UUID)`
  - `create_ticket(agent_id, route_id, origin_id, destination_id)`
  - `validate_ticket(ticket_id: UUID)`
  - `get_active_tickets(agent_id: UUID = None)`
  - `get_by_agent(agent_id: UUID)`
  - `get_revenue_by_period(start: DateTime, end: DateTime)`
  - `get_usage_statistics(period: str)`
- [ ] Adicionar `self.tickets = TicketQueries(session)` em `DatabaseQueries`
- [ ] Criar testes unitários

## Arquivos Afetados
- `backend/database/queries.py`
- `tests/unit/test_queries.py`
""",
                "labels": ["feat", "area: transport", "area: economy", "area: database", "phase-1: digital", "priority: medium"]
            },
            {
                "title": "Issue 4.9: Modelo Schedule",
                "body": """## Objetivo
Implementar sistema de horários de veículos em rotas.

## Dependências
- Issue 4.5
- Issue 4.3

## Tarefas
- [ ] Criar modelo `Schedule`:
  - `id` (UUID, PK)
  - `route_id` (UUID, FK)
  - `vehicle_id` (UUID, FK)
  - `departure_time` (Time)
  - `days_of_week` (JSON/Array)
  - `is_active` (Boolean)
  - `created_at`, `updated_at`
- [ ] Adicionar métodos:
  - `is_active_today()` - verifica se ativo no dia
  - `get_next_departure()` - próxima partida
- [ ] Adicionar relacionamentos
- [ ] Criar migration Alembic
- [ ] Criar testes

## Arquivos Afetados
- `backend/database/models.py`
- `migrations/versions/`
- `tests/unit/test_models.py`
""",
                "labels": ["feat", "area: transport", "area: database", "phase-1: digital", "priority: medium"]
            },
            {
                "title": "Issue 4.10: Modelo TransportOperator",
                "body": """## Objetivo
Implementar sistema de operadoras de transporte.

## Dependências
- Issue 4.5
- Issue 4.3

## Tarefas
- [ ] Criar modelo `TransportOperator`:
  - `id` (UUID, PK)
  - `name` (String)
  - `operator_type` (StationType)
  - `revenue` (Decimal)
  - `operational_costs` (Decimal)
  - `is_active` (Boolean)
  - `created_at`, `updated_at`
- [ ] Adicionar relacionamentos:
  - `routes` → List[Route]
  - `vehicles` → List[Vehicle]
  - `employees` → List[Agent]
- [ ] Adicionar métodos:
  - `calculate_daily_revenue(date: DateTime)`
  - `calculate_operational_costs()`
  - `get_profit_margin()`
- [ ] Criar migration Alembic
- [ ] Criar testes

## Arquivos Afetados
- `backend/database/models.py`
- `migrations/versions/`
- `tests/unit/test_models.py`
""",
                "labels": ["feat", "area: transport", "area: economy", "area: database", "phase-1: digital", "priority: medium"]
            },
            {
                "title": "Issue 4.11: TransportOperatorQueries",
                "body": """## Objetivo
Implementar queries para operadoras de transporte.

## Dependências
- Issue 4.10

## Tarefas
- [ ] Criar classe `TransportOperatorQueries`
- [ ] Implementar métodos:
  - `get_by_id(operator_id: UUID)`
  - `get_all()`
  - `get_by_type(operator_type: StationType)`
  - `get_most_profitable(limit: int = 10)`
  - `calculate_daily_revenue(operator_id: UUID, date: DateTime)`
  - `get_employees(operator_id: UUID)`
  - `assign_vehicle_to_route(vehicle_id: UUID, route_id: UUID)`
  - `create(**kwargs)`
  - `update(operator_id: UUID, **kwargs)`
- [ ] Adicionar `self.transport_operators` em `DatabaseQueries`
- [ ] Criar testes

## Arquivos Afetados
- `backend/database/queries.py`
- `tests/unit/test_queries.py`
""",
                "labels": ["feat", "area: transport", "area: economy", "area: database", "phase-1: digital", "priority: low"]
            },
            {
                "title": "Issue 4.12: Modelo PassengerQueue",
                "body": """## Objetivo
Implementar sistema de filas de passageiros em estações.

## Dependências
- Issue 4.1
- Issue 4.5
- Issue 4.4

## Tarefas
- [ ] Criar modelo `PassengerQueue`:
  - `id` (UUID, PK)
  - `station_id` (UUID, FK)
  - `route_id` (UUID, FK)
  - `agent_id` (UUID, FK)
  - `queue_position` (Integer)
  - `waiting_since` (DateTime)
  - `estimated_arrival_time` (DateTime, nullable)
  - Constraint unique (station_id, route_id, agent_id)
- [ ] Adicionar métodos:
  - `calculate_wait_time()`
  - `update_position(new_position: int)`
- [ ] Adicionar relacionamentos
- [ ] Criar migration Alembic
- [ ] Criar testes

## Arquivos Afetados
- `backend/database/models.py`
- `migrations/versions/`
- `tests/unit/test_models.py`
""",
                "labels": ["feat", "area: transport", "area: agents", "area: database", "phase-1: digital", "priority: medium"]
            },
            {
                "title": "Issue 4.13: PassengerQueueQueries",
                "body": """## Objetivo
Implementar queries para gerenciamento de filas.

## Dependências
- Issue 4.12

## Tarefas
- [ ] Criar classe `PassengerQueueQueries`
- [ ] Implementar métodos:
  - `add_to_queue(agent_id: UUID, station_id: UUID, route_id: UUID)`
  - `remove_from_queue(agent_id: UUID)`
  - `get_queue_length(station_id: UUID, route_id: UUID)`
  - `get_agent_position(agent_id: UUID)`
  - `board_passengers(vehicle_id: UUID, count: int)`
  - `get_queue_by_station_route(station_id: UUID, route_id: UUID)`
  - `get_average_wait_time(station_id: UUID)`
- [ ] Adicionar `self.passenger_queues` em `DatabaseQueries`
- [ ] Criar testes

## Arquivos Afetados
- `backend/database/queries.py`
- `tests/unit/test_queries.py`
""",
                "labels": ["feat", "area: transport", "area: database", "phase-1: digital", "priority: medium"]
            },
            {
                "title": "Issue 4.14: Modelo VehicleMaintenanceLog",
                "body": """## Objetivo
Implementar registro de manutenções de veículos.

## Dependências
- Issue 4.1
- Issue 4.3

## Tarefas
- [ ] Criar enum `MaintenanceType`:
  - `ROUTINE`, `REPAIR`, `EMERGENCY`, `INSPECTION`
- [ ] Criar modelo `VehicleMaintenanceLog`:
  - `id` (UUID, PK)
  - `vehicle_id` (UUID, FK)
  - `station_id` (UUID, FK)
  - `maintenance_type` (MaintenanceType)
  - `cost` (Decimal)
  - `duration_minutes` (Integer)
  - `description` (Text, nullable)
  - `performed_at` (DateTime)
- [ ] Adicionar relacionamentos
- [ ] Criar migration Alembic
- [ ] Criar testes

## Arquivos Afetados
- `backend/database/models.py`
- `migrations/versions/`
- `tests/unit/test_models.py`
""",
                "labels": ["feat", "area: transport", "area: database", "phase-1: digital", "priority: low"]
            },
            {
                "title": "Issue 4.15: Estatísticas de Transporte",
                "body": """## Objetivo
Implementar modelos de estatísticas de estações e rotas.

## Dependências
- Issue 4.1
- Issue 4.5

## Tarefas
- [ ] Criar modelo `StationStatistics`:
  - `id` (UUID, PK)
  - `station_id` (UUID, FK)
  - `date` (Date)
  - `total_passengers_in` (Integer)
  - `total_passengers_out` (Integer)
  - `peak_hour_start` (Time)
  - `peak_hour_passengers` (Integer)
  - `average_waiting_time` (Float)
  - `revenue_generated` (Decimal)
  - `incidents_count` (Integer)
- [ ] Criar modelo `RouteStatistics`:
  - `id` (UUID, PK)
  - `route_id` (UUID, FK)
  - `date` (Date)
  - `total_trips` (Integer)
  - `total_passengers` (Integer)
  - `on_time_percentage` (Float)
  - `average_delay_minutes` (Float)
  - `revenue` (Decimal)
  - `operational_cost` (Decimal)
  - `profit` (Decimal)
- [ ] Implementar métodos de agregação
- [ ] Criar migrations
- [ ] Criar testes

## Arquivos Afetados
- `backend/database/models.py`
- `migrations/versions/`
- `tests/unit/test_models.py`
""",
                "labels": ["feat", "area: transport", "area: database", "phase-1: digital", "priority: low"]
            },
            {
                "title": "Issue 4.16: Configurações de Transporte",
                "body": """## Objetivo
Criar sistema de configurações para transporte público.

## Dependências
- Issue 4.1

## Tarefas
- [ ] Criar arquivo `backend/config/transport_config.py`
- [ ] Implementar classe `TransportConfig`:
  - `price_per_km: Dict[StationType, Decimal]`
  - `no_ticket_fine: Decimal`
  - `default_capacities: Dict[StationType, int]`
  - `average_speeds: Dict[StationType, float]`
  - `government_subsidy_per_passenger: Decimal`
  - `maintenance_cost_per_km: Dict[str, Decimal]`
- [ ] Criar constantes:
  - `STATION_INSPECTION_INTERVAL_DAYS = 90`
  - `STATION_DEGRADATION_RATE_PER_DAY = 0.5`
  - `MAX_WAITING_TIME_MINUTES = 30`
  - `TICKET_VALIDITY_HOURS = 2`
  - `BASE_TICKET_PRICE = 3.50`
- [ ] Adicionar validação de configurações
- [ ] Criar testes

## Arquivos Afetados
- `backend/config/transport_config.py`
- `tests/unit/test_config.py`
""",
                "labels": ["feat", "area: transport", "chore", "phase-1: digital", "priority: medium"]
            },
            {
                "title": "Issue 4.17: Eventos de Transporte",
                "body": """## Objetivo
Adicionar eventos relacionados ao sistema de transporte.

## Dependências
- Issue 4.1
- Issue 4.5
- Issue 4.12

## Tarefas
- [ ] Adicionar event_types de Passageiros:
  - `passenger_board_vehicle`
  - `passenger_alight_vehicle`
  - `passenger_missed_transport`
  - `passenger_queue_timeout`
- [ ] Adicionar event_types de Operação:
  - `route_started`
  - `route_completed`
  - `vehicle_breakdown`
  - `driver_shift_start`
  - `driver_shift_end`
- [ ] Adicionar event_types Econômicos:
  - `ticket_purchased`
  - `fine_issued`
  - `operator_bankrupt`
  - `route_subsidy_paid`
- [ ] Adicionar event_types de Estação:
  - `transport_station_overcrowded`
  - `transport_station_closed`
  - `transport_accident`
  - `transport_route_modified`
  - `transport_vehicle_delayed`
- [ ] Implementar métodos auxiliares em `EventQueries`
- [ ] Criar testes

## Arquivos Afetados
- `backend/database/queries.py`
- `tests/unit/test_events.py`
""",
                "labels": ["feat", "area: transport", "area: database", "phase-1: digital", "priority: medium"]
            },
            {
                "title": "Issue 4.18: Profissões de Transporte",
                "body": """## Objetivo
Adicionar profissões relacionadas ao transporte público.

## Dependências
- Issue 4.10

## Tarefas
- [ ] Adicionar novas profissões:
  - `bus_driver` - motorista de ônibus
  - `train_conductor` - condutor de trem
  - `ticket_seller` - vendedor de bilhetes
  - `taxi_driver` - taxista
  - `metro_operator` - operador de metrô
- [ ] Configurar salários e setores para cada profissão
- [ ] Adicionar relacionamento Profession → TransportOperator
- [ ] Criar migration de dados (seed)
- [ ] Atualizar documentação de profissões
- [ ] Criar testes

## Arquivos Afetados
- `backend/database/models.py`
- `backend/database/seeds/professions_seed.py`
- `migrations/versions/`
- `tests/unit/test_professions.py`
- `docs/professions.md`
""",
                "labels": ["feat", "area: transport", "area: agents", "area: database", "phase-1: digital", "priority: low"]
            },
            {
                "title": "Issue 4.19: Integração com Sistema de Rotinas",
                "body": """## Objetivo
Integrar transporte público com o sistema de rotinas dos agentes.

## Dependências
- Issue 4.7
- Issue 4.12

## Tarefas
- [ ] Modificar modelo `Routine`:
  - Adicionar `use_public_transport` (Boolean)
  - Adicionar `preferred_transport_type` (StationType, nullable)
- [ ] Atualizar lógica de rotinas:
  - Verificar `use_public_transport` ao ir trabalhar
  - Encontrar estação mais próxima
  - Calcular tempo de espera
  - Comprar bilhete automaticamente
  - Adicionar agente à fila
- [ ] Integrar com eventos de transporte
- [ ] Criar migration
- [ ] Criar testes:
  - `test_agent_uses_public_transport`
  - `test_agent_buys_ticket`
  - `test_agent_waits_at_station`

## Arquivos Afetados
- `backend/database/models.py`
- `backend/simulation/routines.py` (criar se não existir)
- `migrations/versions/`
- `tests/unit/test_routines.py`
""",
                "labels": ["feat", "area: transport", "area: agents", "simulation", "phase-1: digital", "priority: medium"]
            },
            {
                "title": "Issue 4.20: Sistema de Decisão de Transporte",
                "body": """## Objetivo
Implementar IA para decisão de uso de transporte pelos agentes.

## Dependências
- Issue 4.7
- Issue 4.16

## Tarefas
- [ ] Criar `backend/simulation/transport_decision.py`
- [ ] Implementar classe `TransportDecisionSystem`:
  - `choose_transport_mode(agent, origin, destination)` - escolhe transporte
  - `calculate_trip_cost(mode, distance)` - calcula custo
  - `calculate_trip_time(mode, distance)` - calcula tempo
  - `evaluate_comfort_score(mode, agent)` - avalia conforto
  - `make_decision(factors: Dict)` - decide baseado em fatores
- [ ] Considerar fatores:
  - Distância ao destino
  - Custo da viagem
  - Tempo estimado
  - Conforto pessoal
  - Disponibilidade de veículo próprio
  - Condições climáticas
- [ ] Criar testes

## Arquivos Afetados
- `backend/simulation/transport_decision.py`
- `tests/unit/test_transport_decision.py`
""",
                "labels": ["feat", "area: transport", "ai", "simulation", "phase-1: digital", "priority: medium", "complexity: intermediate"]
            },
            {
                "title": "Issue 4.21: Sistema de Pathfinding Multimodal",
                "body": """## Objetivo
Implementar pathfinding que combina diferentes tipos de transporte.

## Dependências
- Issue 4.5
- Issue 4.6

## Tarefas
- [ ] Criar `backend/simulation/multimodal_pathfinding.py`
- [ ] Implementar classe `MultimodalPathfinder`:
  - `find_optimal_route(origin, destination)` - encontra melhor rota
  - `calculate_route_segments()` - divide em segmentos
  - `estimate_total_time(route)` - tempo total
  - `estimate_total_cost(route)` - custo total
  - `consider_transfers()` - considera baldeações
- [ ] Integrar com sistema de navegação existente
- [ ] Implementar algoritmo A* multimodal
- [ ] Considerar horários e frequências
- [ ] Criar testes:
  - `test_single_mode_route`
  - `test_multimodal_route`
  - `test_route_with_transfers`

## Arquivos Afetados
- `backend/simulation/multimodal_pathfinding.py`
- `tests/unit/test_pathfinding.py`
""",
                "labels": ["feat", "area: transport", "ai", "simulation", "phase-1: digital", "priority: medium", "complexity: advanced"]
            },
            {
                "title": "Issue 4.22: Sistema de Preços Dinâmicos",
                "body": """## Objetivo
Implementar sistema de preços dinâmicos para bilhetes.

## Dependências
- Issue 4.7
- Issue 4.15
- Issue 4.16

## Tarefas
- [ ] Criar `backend/simulation/dynamic_pricing.py`
- [ ] Implementar classe `DynamicPricingSystem`:
  - `calculate_ticket_price(route, time, agent)` - calcula preço
  - `get_peak_hour_multiplier(time)` - multiplicador horário de pico
  - `get_demand_multiplier(route, time)` - multiplicador de demanda
  - `get_passenger_type_discount(agent)` - desconto por tipo
- [ ] Implementar tipos de passageiro:
  - Estudante (50% desconto)
  - Idoso (gratuito)
  - Pessoa com deficiência (gratuito)
  - Normal (preço cheio)
- [ ] Definir horários de pico
- [ ] Criar testes

## Arquivos Afetados
- `backend/simulation/dynamic_pricing.py`
- `tests/unit/test_pricing.py`
""",
                "labels": ["feat", "area: transport", "area: economy", "simulation", "phase-1: digital", "priority: low", "complexity: intermediate"]
            },
            {
                "title": "Issue 4.23: Sistema de Otimização de Rotas",
                "body": """## Objetivo
Implementar sistema que otimiza rotas baseado em demanda.

## Dependências
- Issue 4.15

## Tarefas
- [ ] Criar `backend/simulation/route_optimization.py`
- [ ] Implementar classe `RouteOptimizer`:
  - `adjust_frequencies(route_id)` - ajusta frequência
  - `suggest_new_routes()` - sugere novas rotas
  - `optimize_schedule(route_id)` - otimiza horários
  - `identify_underutilized_routes()` - identifica rotas subutilizadas
  - `analyze_demand_patterns()` - analisa padrões de demanda
- [ ] Criar job periódico (diário/semanal)
- [ ] Gerar relatórios de otimização
- [ ] Integrar com sistema de estatísticas
- [ ] Criar testes

## Arquivos Afetados
- `backend/simulation/route_optimization.py`
- `tests/unit/test_route_optimization.py`
""",
                "labels": ["feat", "area: transport", "ai", "simulation", "phase-1: digital", "priority: low", "complexity: advanced"]
            },
            {
                "title": "Issue 4.24: Sistema de Degradação e Manutenção",
                "body": """## Objetivo
Implementar sistema de degradação de estações e manutenção.

## Dependências
- Issue 4.1
- Issue 4.14
- Issue 4.16

## Tarefas
- [ ] Criar `backend/simulation/station_maintenance.py`
- [ ] Implementar classe `StationMaintenanceSystem`:
  - `degrade_station_condition()` - degrada condição diariamente
  - `schedule_inspection(station_id)` - agenda inspeção
  - `perform_maintenance(station_id, type)` - executa manutenção
  - `calculate_maintenance_cost(station_id)` - calcula custo
  - `check_safety_compliance(station_id)` - verifica segurança
- [ ] Implementar degradação baseada em:
  - Uso (passageiros/dia)
  - Condições climáticas
  - Tempo desde última manutenção
- [ ] Integrar com sistema de eventos
- [ ] Criar job periódico (diário)
- [ ] Criar testes

## Arquivos Afetados
- `backend/simulation/station_maintenance.py`
- `tests/unit/test_maintenance.py`
""",
                "labels": ["feat", "area: transport", "simulation", "phase-1: digital", "priority: low"]
            },
            {
                "title": "Issue 4.25: Integração Econômica Completa",
                "body": """## Objetivo
Integrar sistema de transporte com economia principal.

## Dependências
- Issue 4.8
- Issue 4.11
- Issue 4.24

## Tarefas
- [ ] Modificar modelo `EconomicStat`:
  - Adicionar `transport_revenue` (Decimal)
  - Adicionar `transport_operational_costs` (Decimal)
  - Adicionar `transport_maintenance_costs` (Decimal)
  - Adicionar `transport_subsidy_paid` (Decimal)
- [ ] Atualizar `EconomicStatQueries`:
  - `calculate_transport_profit()`
  - `get_transport_kpis()`
  - `get_transport_balance()`
- [ ] Integrar cálculos com sistema principal
- [ ] Criar dashboard de métricas de transporte
- [ ] Criar migration
- [ ] Criar testes

## Arquivos Afetados
- `backend/database/models.py`
- `backend/database/queries.py`
- `backend/simulation/economics.py` (criar se necessário)
- `migrations/versions/`
- `tests/unit/test_economics.py`
""",
                "labels": ["feat", "area: transport", "area: economy", "area: database", "phase-1: digital", "priority: medium"]
            },
            {
                "title": "Issue 4.26: API Endpoints de Transporte",
                "body": """## Objetivo
Criar endpoints REST para sistema de transporte.

## Dependências
- Todas as issues anteriores

## Tarefas
- [ ] Criar `backend/api/routes/transport.py`
- [ ] Implementar endpoints:
  - `GET /stations` - listar estações
  - `GET /stations/{id}` - detalhes da estação
  - `POST /stations` - criar estação
  - `PUT /stations/{id}` - atualizar estação
  - `GET /routes` - listar rotas
  - `GET /routes/{id}` - detalhes da rota
  - `POST /routes` - criar rota
  - `POST /tickets` - comprar bilhete
  - `GET /agents/{id}/tickets` - bilhetes do agente
  - `GET /statistics/transport` - estatísticas gerais
  - `GET /statistics/stations/{id}` - estatísticas da estação
  - `GET /statistics/routes/{id}` - estatísticas da rota
- [ ] Adicionar validações (Pydantic schemas)
- [ ] Adicionar documentação OpenAPI/Swagger
- [ ] Adicionar autenticação/autorização
- [ ] Criar testes de integração

## Arquivos Afetados
- `backend/api/routes/transport.py`
- `backend/api/schemas/transport.py` (criar)
- `backend/api/main.py`
- `tests/integration/test_api_transport.py`
""",
                "labels": ["feat", "area: transport", "area: api", "phase-1: digital", "priority: medium"]
            },
            {
                "title": "Issue 4.27: Documentação e Seeds",
                "body": """## Objetivo
Criar documentação completa e dados iniciais do sistema de transporte.

## Dependências
- Todas as issues anteriores

## Tarefas
- [ ] Criar `docs/TRANSPORT_SYSTEM.md`:
  - Arquitetura do sistema
  - Modelos e relacionamentos
  - Fluxo de uso
  - Exemplos de código
  - Diagramas ER
- [ ] Criar `backend/database/seeds/transport_seed.py`:
  - 5-10 estações de exemplo
  - 3-5 rotas de exemplo
  - 2-3 operadoras de exemplo
  - Profissões de transporte
  - Horários de exemplo
- [ ] Atualizar `README.md`:
  - Adicionar features de transporte
  - Adicionar instruções de uso
- [ ] Criar diagramas:
  - Diagrama ER do sistema de transporte
  - Fluxograma de decisão de transporte
  - Diagrama de classes
- [ ] Criar guia de uso do sistema
- [ ] Documentar APIs

## Arquivos Afetados
- `docs/TRANSPORT_SYSTEM.md`
- `docs/diagrams/transport_er.png`
- `backend/database/seeds/transport_seed.py`
- `README.md`
- `docs/API.md`
""",
                "labels": ["docs", "area: transport", "phase-1: digital", "priority: medium", "good first issue"]
            }
        ]

        created_issues = []
        for issue_data in issues:
            result = self.create_issue(
                title=issue_data["title"],
                body=issue_data["body"],
                labels=issue_data["labels"],
                milestone=milestone_number
            )
            if result:
                created_issues.append(result)
        
        print(f"\n✅ {len(created_issues)} issues criadas com sucesso!")
        return created_issues


def main():
    """Função principal"""
    parser = argparse.ArgumentParser(
        description="Cria issues do sistema de transporte no GitHub"
    )
    parser.add_argument(
        "--token",
        help="GitHub Personal Access Token",
        default=os.getenv("GITHUB_TOKEN")
    )
    parser.add_argument(
        "--dry-run",
        action="store_true",
        help="Apenas exibe o que seria criado, sem criar"
    )

    args = parser.parse_args()

    if not args.token:
        print("❌ Erro: GitHub token não fornecido.")
        print("Use --token YOUR_TOKEN ou defina GITHUB_TOKEN no arquivo .env")
        sys.exit(1)

    print(f"\n🚀 Criando issues de transporte para {REPO_OWNER}/{REPO_NAME}\n")

    if args.dry_run:
        print("⚠️  Modo DRY RUN - Nenhuma issue será criada\n")
        print("27 issues seriam criadas (4.1 a 4.27)")
        return

    creator = TransportIssuesCreator(args.token)
    creator.create_all_transport_issues()

    print("\n🎉 Processo concluído!")
    print(f"Acesse: https://github.com/{REPO_OWNER}/{REPO_NAME}/issues")


if __name__ == "__main__":
    main()

