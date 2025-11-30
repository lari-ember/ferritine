"""
API FastAPI para Ferritine - Integração com Unity/C#
Fornece endpoints REST para consumo do estado da simulação.
"""

from fastapi import FastAPI, HTTPException
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel
from typing import List, Optional, Dict, Any
from datetime import datetime
from decimal import Decimal
import uvicorn

# Importações do banco de dados
from backend.database.connection import get_session
from backend.database.models import (
    Agent, Vehicle, Station, Route, TransportOperator,
    Ticket, Schedule, AgentStatus, VehicleStatus, StationType
)
from sqlalchemy import func

# Inicializar FastAPI
app = FastAPI(
    title="Ferritine API",
    description="API de simulação de transporte para integração com Unity",
    version="0.2.0"
)

# Configurar CORS para Unity poder consumir
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  # Em produção: restringir
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# ==================== MODELOS PYDANTIC (DTOs) ====================

class AgentDTO(BaseModel):
    id: str
    name: str
    status: str
    location_type: Optional[str]
    location_id: Optional[str]
    energy_level: int
    wallet: float
    
    class Config:
        from_attributes = True

class VehicleDTO(BaseModel):
    id: str
    name: str
    vehicle_type: str
    passengers: int
    capacity: int
    status: str
    current_station_id: Optional[str]
    current_route_id: Optional[str]
    fuel_level: Optional[float]
    
    class Config:
        from_attributes = True

class StationDTO(BaseModel):
    id: str
    name: str
    station_type: str
    x: int
    y: int
    queue_length: int
    max_queue: int
    is_operational: bool
    
    class Config:
        from_attributes = True

class RouteDTO(BaseModel):
    id: str
    name: str
    code: str
    route_type: str
    fare: float
    frequency: int
    is_active: bool
    
    class Config:
        from_attributes = True

class OperatorDTO(BaseModel):
    id: str
    name: str
    operator_type: str
    revenue: float
    costs: float
    profit: float
    
    class Config:
        from_attributes = True

class MetricsDTO(BaseModel):
    total_passengers_waiting: int
    total_passengers_in_vehicles: int
    total_vehicles: int
    total_stations: int
    total_routes: int
    total_revenue: float
    avg_queue_length: float

class WorldStateDTO(BaseModel):
    timestamp: str
    simulation_time: Optional[str]
    agents: List[AgentDTO]
    vehicles: List[VehicleDTO]
    stations: List[StationDTO]
    routes: List[RouteDTO]
    operators: List[OperatorDTO]
    metrics: MetricsDTO

# ==================== HELPERS ====================

def to_str(uuid_obj) -> str:
    """Converte UUID para string."""
    return str(uuid_obj) if uuid_obj else None

def to_float(decimal_obj) -> float:
    """Converte Decimal para float."""
    return float(decimal_obj) if decimal_obj else 0.0

# ==================== ENDPOINTS ====================

@app.get("/")
def root():
    """Endpoint raiz - status da API."""
    return {
        "name": "Ferritine API",
        "version": "0.2.0",
        "status": "online",
        "docs": "/docs",
        "endpoints": {
            "world_state": "/api/world/state",
            "agents": "/api/agents",
            "vehicles": "/api/vehicles",
            "stations": "/api/stations",
            "routes": "/api/routes",
            "operators": "/api/operators",
            "metrics": "/api/metrics"
        }
    }

@app.get("/health")
def health_check():
    """Health check para monitoramento."""
    session = get_session()
    try:
        # Testar conexão com banco
        session.execute("SELECT 1")
        return {"status": "healthy", "database": "connected"}
    except Exception as e:
        return {"status": "unhealthy", "error": str(e)}
    finally:
        session.close()

@app.get("/api/world/state", response_model=WorldStateDTO)
def get_world_state():
    """
    Retorna estado completo do mundo da simulação.
    Endpoint principal para Unity consumir.
    """
    session = get_session()
    
    try:
        # Buscar todos os dados
        agents = session.query(Agent).limit(100).all()
        vehicles = session.query(Vehicle).all()
        stations = session.query(Station).all()
        routes = session.query(Route).filter(Route.is_active == True).all()
        operators = session.query(TransportOperator).all()
        
        # Converter para DTOs
        agents_dto = [
            AgentDTO(
                id=to_str(a.id),
                name=a.name,
                status=a.current_status.value if a.current_status else "idle",
                location_type=a.current_location_type,
                location_id=to_str(a.current_location_id),
                energy_level=a.energy_level or 100,
                wallet=to_float(a.wallet)
            )
            for a in agents
        ]
        
        vehicles_dto = [
            VehicleDTO(
                id=to_str(v.id),
                name=v.name,
                vehicle_type=v.vehicle_type,
                passengers=v.current_passengers or 0,
                capacity=v.passenger_capacity or 0,
                status=v.status.value if v.status else "idle",
                current_station_id=to_str(v.current_station_id),
                current_route_id=to_str(v.current_route_id),
                fuel_level=float(v.current_fuel) if v.current_fuel else 0.0
            )
            for v in vehicles
        ]
        
        stations_dto = [
            StationDTO(
                id=to_str(s.id),
                name=s.name,
                station_type=s.station_type.value if s.station_type else "TRAIN_STEAM",
                x=s.x,
                y=s.y,
                queue_length=s.current_queue_length or 0,
                max_queue=s.max_queue_length or 50,
                is_operational=s.is_operational if s.is_operational is not None else True
            )
            for s in stations
        ]
        
        routes_dto = [
            RouteDTO(
                id=to_str(r.id),
                name=r.name,
                code=r.code,
                route_type=r.route_type.value if r.route_type else "TRAIN_STEAM",
                fare=to_float(r.fare_base),
                frequency=r.frequency_minutes or 10,
                is_active=r.is_active if r.is_active is not None else True
            )
            for r in routes
        ]
        
        operators_dto = [
            OperatorDTO(
                id=to_str(o.id),
                name=o.name,
                operator_type=o.operator_type.value if o.operator_type else "TRAIN_STEAM",
                revenue=to_float(o.revenue),
                costs=to_float(o.operational_costs),
                profit=o.get_profit_margin()
            )
            for o in operators
        ]
        
        # Calcular métricas
        total_queue = session.query(func.sum(Station.current_queue_length)).scalar() or 0
        total_in_vehicles = session.query(func.sum(Vehicle.current_passengers)).scalar() or 0
        avg_queue = session.query(func.avg(Station.current_queue_length)).scalar() or 0.0
        total_revenue = session.query(func.sum(TransportOperator.revenue)).scalar() or 0.0
        
        metrics_dto = MetricsDTO(
            total_passengers_waiting=int(total_queue),
            total_passengers_in_vehicles=int(total_in_vehicles),
            total_vehicles=len(vehicles),
            total_stations=len(stations),
            total_routes=len(routes),
            total_revenue=to_float(total_revenue),
            avg_queue_length=float(avg_queue)
        )
        
        # Montar resposta
        return WorldStateDTO(
            timestamp=datetime.utcnow().isoformat(),
            simulation_time=None,  # TODO: Pegar do motor de simulação
            agents=agents_dto,
            vehicles=vehicles_dto,
            stations=stations_dto,
            routes=routes_dto,
            operators=operators_dto,
            metrics=metrics_dto
        )
        
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Erro ao buscar estado: {str(e)}")
    finally:
        session.close()

@app.get("/api/agents", response_model=List[AgentDTO])
def get_agents(limit: int = 100):
    """Retorna lista de agentes."""
    session = get_session()
    try:
        agents = session.query(Agent).limit(limit).all()
        return [
            AgentDTO(
                id=to_str(a.id),
                name=a.name,
                status=a.current_status.value if a.current_status else "idle",
                location_type=a.current_location_type,
                location_id=to_str(a.current_location_id),
                energy_level=a.energy_level or 100,
                wallet=to_float(a.wallet)
            )
            for a in agents
        ]
    finally:
        session.close()

@app.get("/api/vehicles", response_model=List[VehicleDTO])
def get_vehicles():
    """Retorna lista de veículos."""
    session = get_session()
    try:
        vehicles = session.query(Vehicle).all()
        return [
            VehicleDTO(
                id=to_str(v.id),
                name=v.name,
                vehicle_type=v.vehicle_type,
                passengers=v.current_passengers or 0,
                capacity=v.passenger_capacity or 0,
                status=v.status.value if v.status else "idle",
                current_station_id=to_str(v.current_station_id),
                current_route_id=to_str(v.current_route_id),
                fuel_level=float(v.current_fuel) if v.current_fuel else 0.0
            )
            for v in vehicles
        ]
    finally:
        session.close()

@app.get("/api/stations", response_model=List[StationDTO])
def get_stations():
    """Retorna lista de estações."""
    session = get_session()
    try:
        stations = session.query(Station).all()
        return [
            StationDTO(
                id=to_str(s.id),
                name=s.name,
                station_type=s.station_type.value if s.station_type else "TRAIN_STEAM",
                x=s.x,
                y=s.y,
                queue_length=s.current_queue_length or 0,
                max_queue=s.max_queue_length or 50,
                is_operational=s.is_operational if s.is_operational is not None else True
            )
            for s in stations
        ]
    finally:
        session.close()

@app.get("/api/routes", response_model=List[RouteDTO])
def get_routes():
    """Retorna lista de rotas ativas."""
    session = get_session()
    try:
        routes = session.query(Route).filter(Route.is_active == True).all()
        return [
            RouteDTO(
                id=to_str(r.id),
                name=r.name,
                code=r.code,
                route_type=r.route_type.value if r.route_type else "TRAIN_STEAM",
                fare=to_float(r.fare_base),
                frequency=r.frequency_minutes or 10,
                is_active=r.is_active if r.is_active is not None else True
            )
            for r in routes
        ]
    finally:
        session.close()

@app.get("/api/operators", response_model=List[OperatorDTO])
def get_operators():
    """Retorna lista de operadoras."""
    session = get_session()
    try:
        operators = session.query(TransportOperator).all()
        return [
            OperatorDTO(
                id=to_str(o.id),
                name=o.name,
                operator_type=o.operator_type.value if o.operator_type else "TRAIN_STEAM",
                revenue=to_float(o.revenue),
                costs=to_float(o.operational_costs),
                profit=o.get_profit_margin()
            )
            for o in operators
        ]
    finally:
        session.close()

@app.get("/api/metrics", response_model=MetricsDTO)
def get_metrics():
    """Retorna métricas agregadas."""
    session = get_session()
    try:
        total_queue = session.query(func.sum(Station.current_queue_length)).scalar() or 0
        total_in_vehicles = session.query(func.sum(Vehicle.current_passengers)).scalar() or 0
        avg_queue = session.query(func.avg(Station.current_queue_length)).scalar() or 0.0
        total_revenue = session.query(func.sum(TransportOperator.revenue)).scalar() or 0.0
        
        total_vehicles = session.query(Vehicle).count()
        total_stations = session.query(Station).count()
        total_routes = session.query(Route).filter(Route.is_active == True).count()
        
        return MetricsDTO(
            total_passengers_waiting=int(total_queue),
            total_passengers_in_vehicles=int(total_in_vehicles),
            total_vehicles=total_vehicles,
            total_stations=total_stations,
            total_routes=total_routes,
            total_revenue=to_float(total_revenue),
            avg_queue_length=float(avg_queue)
        )
    finally:
        session.close()

# ==================== ENTITY CONTROL ENDPOINTS ====================

class QueueUpdate(BaseModel):
    """Request body for modifying station queue."""
    queue_length: int

class AgentTeleport(BaseModel):
    """Request body for agent teleportation."""
    location_type: str  # "station" or "building"
    location_id: str

class BuildingDTO(BaseModel):
    """Simplified building data for teleport destinations."""
    id: str
    name: str
    building_type: str
    x: int
    y: int
    is_constructed: bool
    
    class Config:
        from_attributes = True

@app.post("/api/vehicles/{vehicle_id}/pause")
def pause_vehicle(vehicle_id: str):
    """Pausa um veículo."""
    session = get_session()
    try:
        from uuid import UUID
        vehicle = session.query(Vehicle).filter(Vehicle.id == UUID(vehicle_id)).first()
        
        if not vehicle:
            raise HTTPException(status_code=404, detail=f"Veículo {vehicle_id} não encontrado")
        
        # Store previous status or use a paused flag
        # For now, we'll change status to indicate paused
        vehicle.status = VehicleStatus.MAINTENANCE  # Using maintenance as "paused" indicator
        session.commit()
        
        return {
            "success": True,
            "message": f"Veículo {vehicle.name} pausado com sucesso",
            "data": {
                "id": to_str(vehicle.id),
                "name": vehicle.name,
                "status": vehicle.status.value
            }
        }
    except ValueError:
        raise HTTPException(status_code=400, detail="ID de veículo inválido")
    except Exception as e:
        session.rollback()
        raise HTTPException(status_code=500, detail=f"Erro ao pausar veículo: {str(e)}")
    finally:
        session.close()

@app.post("/api/vehicles/{vehicle_id}/resume")
def resume_vehicle(vehicle_id: str):
    """Retoma um veículo pausado."""
    session = get_session()
    try:
        from uuid import UUID
        vehicle = session.query(Vehicle).filter(Vehicle.id == UUID(vehicle_id)).first()
        
        if not vehicle:
            raise HTTPException(status_code=404, detail=f"Veículo {vehicle_id} não encontrado")
        
        # Resume by setting status back to active
        vehicle.status = VehicleStatus.ACTIVE
        session.commit()
        
        return {
            "success": True,
            "message": f"Veículo {vehicle.name} retomado com sucesso",
            "data": {
                "id": to_str(vehicle.id),
                "name": vehicle.name,
                "status": vehicle.status.value
            }
        }
    except ValueError:
        raise HTTPException(status_code=400, detail="ID de veículo inválido")
    except Exception as e:
        session.rollback()
        raise HTTPException(status_code=500, detail=f"Erro ao retomar veículo: {str(e)}")
    finally:
        session.close()

@app.post("/api/stations/{station_id}/queue")
def modify_station_queue(station_id: str, update: QueueUpdate):
    """Modifica a fila de uma estação."""
    session = get_session()
    try:
        from uuid import UUID
        station = session.query(Station).filter(Station.id == UUID(station_id)).first()
        
        if not station:
            raise HTTPException(status_code=404, detail=f"Estação {station_id} não encontrada")
        
        # Update queue length (no min/max validation as per requirements)
        station.current_queue_length = update.queue_length
        session.commit()
        
        return {
            "success": True,
            "message": f"Fila da estação {station.name} modificada para {update.queue_length}",
            "data": {
                "id": to_str(station.id),
                "name": station.name,
                "queue_length": station.current_queue_length,
                "max_queue": station.max_queue_length
            }
        }
    except ValueError:
        raise HTTPException(status_code=400, detail="ID de estação inválido")
    except Exception as e:
        session.rollback()
        raise HTTPException(status_code=500, detail=f"Erro ao modificar fila: {str(e)}")
    finally:
        session.close()

@app.post("/api/agents/{agent_id}/teleport")
def teleport_agent(agent_id: str, teleport: AgentTeleport):
    """Teleporta um agente para uma nova localização."""
    session = get_session()
    try:
        from uuid import UUID
        agent = session.query(Agent).filter(Agent.id == UUID(agent_id)).first()
        
        if not agent:
            raise HTTPException(status_code=404, detail=f"Agente {agent_id} não encontrado")
        
        # Validate destination exists
        destination_uuid = UUID(teleport.location_id)
        
        if teleport.location_type == "station":
            destination = session.query(Station).filter(Station.id == destination_uuid).first()
            if not destination:
                raise HTTPException(status_code=404, detail=f"Estação {teleport.location_id} não encontrada")
        elif teleport.location_type == "building":
            from backend.database.models import Building
            destination = session.query(Building).filter(Building.id == destination_uuid).first()
            if not destination:
                raise HTTPException(status_code=404, detail=f"Edifício {teleport.location_id} não encontrado")
        else:
            raise HTTPException(status_code=400, detail=f"Tipo de localização inválido: {teleport.location_type}")
        
        # Update agent location
        agent.current_location_type = teleport.location_type
        agent.current_location_id = destination_uuid
        
        # TODO: Trigger pathfinding recalculation if needed
        
        session.commit()
        
        return {
            "success": True,
            "message": f"Agente {agent.name} teleportado para {teleport.location_type}",
            "data": {
                "id": to_str(agent.id),
                "name": agent.name,
                "location_type": agent.current_location_type,
                "location_id": to_str(agent.current_location_id)
            }
        }
    except ValueError:
        raise HTTPException(status_code=400, detail="ID inválido fornecido")
    except Exception as e:
        session.rollback()
        raise HTTPException(status_code=500, detail=f"Erro ao teleportar agente: {str(e)}")
    finally:
        session.close()

@app.get("/api/buildings", response_model=List[BuildingDTO])
def get_buildings(limit: int = 100, building_type: Optional[str] = None):
    """Retorna lista de edifícios."""
    session = get_session()
    try:
        from backend.database.models import Building
        
        query = session.query(Building)
        
        if building_type:
            query = query.filter(Building.building_type == building_type)
        
        buildings = query.limit(limit).all()
        
        return [
            BuildingDTO(
                id=to_str(b.id),
                name=b.name,
                building_type=b.building_type.value if hasattr(b.building_type, 'value') else str(b.building_type),
                x=b.x,
                y=b.y,
                is_constructed=True  # Assuming all queried buildings are constructed
            )
            for b in buildings
        ]
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Erro ao buscar edifícios: {str(e)}")
    finally:
        session.close()

# ==================== MAIN ====================

if __name__ == "__main__":
    uvicorn.run(
        "backend.api.main:app",
        host="0.0.0.0",
        port=5000,
        reload=True,
        log_level="info"
    )

