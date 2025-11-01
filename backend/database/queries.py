"""
Queries e operações comuns do banco de dados.
"""

from sqlalchemy.orm import Session
from sqlalchemy import func, and_, or_
from typing import List, Optional, Dict, Any
from datetime import datetime
import uuid

from backend.database.models import (
    Agent, Building, Vehicle, Event, EconomicStat, 
    Profession, Routine, NamePool,
    CreatedBy, HealthStatus, AgentStatus, Gender
)


class AgentQueries:
    """Queries relacionadas a agentes."""
    
    def __init__(self, session: Session):
        self.session = session
    
    def get_by_id(self, agent_id: uuid.UUID) -> Optional[Agent]:
        """Busca agente por ID."""
        return self.session.query(Agent).filter(Agent.id == agent_id).first()
    
    def get_all(self, include_deleted: bool = False) -> List[Agent]:
        """Retorna todos os agentes."""
        query = self.session.query(Agent)
        if not include_deleted:
            query = query.filter(Agent.is_deleted == False)
        return query.all()
    
    def get_by_name(self, name: str) -> List[Agent]:
        """Busca agentes por nome (case-insensitive)."""
        return self.session.query(Agent).filter(
            Agent.name.ilike(f'%{name}%'),
            Agent.is_deleted == False
        ).all()
    
    def get_at_location(self, location_type: str, location_id: uuid.UUID) -> List[Agent]:
        """Retorna agentes em uma localização específica."""
        return self.session.query(Agent).filter(
            Agent.current_location_type == location_type,
            Agent.current_location_id == location_id,
            Agent.is_deleted == False
        ).all()
    
    def get_by_profession(self, profession_id: uuid.UUID) -> List[Agent]:
        """Retorna agentes de uma profissão específica."""
        return self.session.query(Agent).filter(
            Agent.profession_id == profession_id,
            Agent.is_deleted == False
        ).all()
    
    def get_by_status(self, status: AgentStatus) -> List[Agent]:
        """Retorna agentes com status específico."""
        return self.session.query(Agent).filter(
            Agent.current_status == status,
            Agent.is_deleted == False
        ).all()
    
    def get_by_health_status(self, health_status: HealthStatus) -> List[Agent]:
        """Retorna agentes com status de saúde específico."""
        return self.session.query(Agent).filter(
            Agent.health_status == health_status,
            Agent.is_deleted == False
        ).all()
    
    def get_by_building(self, building_id: uuid.UUID, 
                       location_type: str = 'home') -> List[Agent]:
        """Retorna agentes associados a um edifício."""
        if location_type == 'home':
            return self.session.query(Agent).filter(
                Agent.home_building_id == building_id,
                Agent.is_deleted == False
            ).all()
        elif location_type == 'work':
            return self.session.query(Agent).filter(
                Agent.work_building_id == building_id,
                Agent.is_deleted == False
            ).all()
        return []
    
    def get_wealthy_agents(self, min_amount: float = 10000.00) -> List[Agent]:
        """Retorna agentes com carteira acima de um valor."""
        return self.session.query(Agent).filter(
            Agent.wallet >= min_amount,
            Agent.is_deleted == False
        ).all()
    
    def get_poor_agents(self, max_amount: float = 100.00) -> List[Agent]:
        """Retorna agentes com carteira abaixo de um valor."""
        return self.session.query(Agent).filter(
            Agent.wallet <= max_amount,
            Agent.is_deleted == False
        ).all()
    
    def create(self, **kwargs) -> Agent:
        """Cria um novo agente."""
        agent = Agent(**kwargs)
        self.session.add(agent)
        self.session.flush()
        return agent
    
    def update(self, agent_id: uuid.UUID, **kwargs) -> Optional[Agent]:
        """Atualiza um agente."""
        agent = self.get_by_id(agent_id)
        if agent:
            for key, value in kwargs.items():
                setattr(agent, key, value)
            self.session.flush()
        return agent
    
    def soft_delete(self, agent_id: uuid.UUID) -> bool:
        """Faz soft delete de um agente."""
        agent = self.get_by_id(agent_id)
        if agent:
            agent.is_deleted = True
            self.session.flush()
            return True
        return False
    
    def get_statistics(self) -> Dict[str, Any]:
        """Retorna estatísticas sobre agentes."""
        total = self.session.query(func.count(Agent.id)).filter(
            Agent.is_deleted == False
        ).scalar()
        
        avg_wallet = self.session.query(func.avg(Agent.wallet)).filter(
            Agent.is_deleted == False
        ).scalar()
        
        by_status = self.session.query(
            Agent.current_status, 
            func.count(Agent.id)
        ).filter(
            Agent.is_deleted == False
        ).group_by(Agent.current_status).all()
        
        by_health = self.session.query(
            Agent.health_status,
            func.count(Agent.id)
        ).filter(
            Agent.is_deleted == False
        ).group_by(Agent.health_status).all()
        
        return {
            'total': total,
            'average_wallet': float(avg_wallet) if avg_wallet else 0.0,
            'by_status': {status: count for status, count in by_status},
            'by_health': {health: count for health, count in by_health}
        }


class BuildingQueries:
    """Queries relacionadas a edifícios."""
    
    def __init__(self, session: Session):
        self.session = session
    
    def get_by_id(self, building_id: uuid.UUID) -> Optional[Building]:
        """Busca edifício por ID."""
        return self.session.query(Building).filter(
            Building.id == building_id
        ).first()
    
    def get_all(self, active_only: bool = True) -> List[Building]:
        """Retorna todos os edifícios."""
        query = self.session.query(Building)
        if active_only:
            query = query.filter(Building.is_active == True)
        return query.all()
    
    def get_by_type(self, building_type: str) -> List[Building]:
        """Retorna edifícios por tipo."""
        return self.session.query(Building).filter(
            Building.building_type == building_type,
            Building.is_active == True
        ).all()
    
    def get_at_position(self, x: int, y: int) -> List[Building]:
        """Retorna edifícios em uma posição."""
        return self.session.query(Building).filter(
            Building.x == x,
            Building.y == y,
            Building.is_active == True
        ).all()
    
    def get_with_vacancy(self, building_type: str = None) -> List[Building]:
        """Retorna edifícios com vagas disponíveis."""
        query = self.session.query(Building).filter(
            Building.current_occupancy < Building.capacity,
            Building.is_active == True
        )
        if building_type:
            query = query.filter(Building.building_type == building_type)
        return query.all()
    
    def create(self, **kwargs) -> Building:
        """Cria um novo edifício."""
        building = Building(**kwargs)
        self.session.add(building)
        self.session.flush()
        return building
    
    def update_occupancy(self, building_id: uuid.UUID, change: int) -> Optional[Building]:
        """Atualiza ocupação de um edifício."""
        building = self.get_by_id(building_id)
        if building:
            building.current_occupancy = max(0, building.current_occupancy + change)
            self.session.flush()
        return building


class VehicleQueries:
    """Queries relacionadas a veículos."""
    
    def __init__(self, session: Session):
        self.session = session
    
    def get_by_id(self, vehicle_id: uuid.UUID) -> Optional[Vehicle]:
        """Busca veículo por ID."""
        return self.session.query(Vehicle).filter(
            Vehicle.id == vehicle_id
        ).first()
    
    def get_all(self) -> List[Vehicle]:
        """Retorna todos os veículos."""
        return self.session.query(Vehicle).all()
    
    def get_by_type(self, vehicle_type: str) -> List[Vehicle]:
        """Retorna veículos por tipo."""
        return self.session.query(Vehicle).filter(
            Vehicle.vehicle_type == vehicle_type
        ).all()
    
    def get_available(self, min_capacity: int = 1) -> List[Vehicle]:
        """Retorna veículos com capacidade disponível."""
        return self.session.query(Vehicle).filter(
            Vehicle.current_passengers < Vehicle.passenger_capacity,
            Vehicle.passenger_capacity >= min_capacity
        ).all()
    
    def create(self, **kwargs) -> Vehicle:
        """Cria um novo veículo."""
        vehicle = Vehicle(**kwargs)
        self.session.add(vehicle)
        self.session.flush()
        return vehicle


class EventQueries:
    """Queries relacionadas a eventos."""
    
    def __init__(self, session: Session):
        self.session = session
    
    def get_by_id(self, event_id: uuid.UUID) -> Optional[Event]:
        """Busca evento por ID."""
        return self.session.query(Event).filter(Event.id == event_id).first()
    
    def get_recent(self, limit: int = 100) -> List[Event]:
        """Retorna eventos recentes."""
        return self.session.query(Event).order_by(
            Event.occurred_at.desc()
        ).limit(limit).all()
    
    def get_by_type(self, event_type: str, limit: int = 100) -> List[Event]:
        """Retorna eventos por tipo."""
        return self.session.query(Event).filter(
            Event.event_type == event_type
        ).order_by(Event.occurred_at.desc()).limit(limit).all()
    
    def get_by_agent(self, agent_id: uuid.UUID, limit: int = 100) -> List[Event]:
        """Retorna eventos de um agente."""
        return self.session.query(Event).filter(
            Event.agent_id == agent_id
        ).order_by(Event.occurred_at.desc()).limit(limit).all()
    
    def create(self, **kwargs) -> Event:
        """Cria um novo evento."""
        event = Event(**kwargs)
        self.session.add(event)
        self.session.flush()
        return event


class EconomicStatQueries:
    """Queries relacionadas a estatísticas econômicas."""
    
    def __init__(self, session: Session):
        self.session = session
    
    def get_latest(self) -> Optional[EconomicStat]:
        """Retorna estatística mais recente."""
        return self.session.query(EconomicStat).order_by(
            EconomicStat.simulation_time.desc()
        ).first()
    
    def get_by_time_range(self, start_time: int, end_time: int) -> List[EconomicStat]:
        """Retorna estatísticas em um período."""
        return self.session.query(EconomicStat).filter(
            EconomicStat.simulation_time >= start_time,
            EconomicStat.simulation_time <= end_time
        ).order_by(EconomicStat.simulation_time).all()
    
    def create(self, **kwargs) -> EconomicStat:
        """Cria nova estatística."""
        stat = EconomicStat(**kwargs)
        self.session.add(stat)
        self.session.flush()
        return stat


class ProfessionQueries:
    """Queries relacionadas a profissões."""
    
    def __init__(self, session: Session):
        self.session = session
    
    def get_by_id(self, profession_id: uuid.UUID) -> Optional[Profession]:
        """Busca profissão por ID."""
        return self.session.query(Profession).filter(
            Profession.id == profession_id
        ).first()
    
    def get_all(self) -> List[Profession]:
        """Retorna todas as profissões."""
        return self.session.query(Profession).all()
    
    def get_by_name(self, name: str) -> Optional[Profession]:
        """Busca profissão por nome."""
        return self.session.query(Profession).filter(
            Profession.name == name
        ).first()
    
    def get_by_sector(self, sector: str) -> List[Profession]:
        """Retorna profissões por setor."""
        return self.session.query(Profession).filter(
            Profession.work_sector == sector
        ).all()
    
    def create(self, **kwargs) -> Profession:
        """Cria uma nova profissão."""
        profession = Profession(**kwargs)
        self.session.add(profession)
        self.session.flush()
        return profession


class NamePoolQueries:
    """Queries para pool de nomes."""
    
    def __init__(self, session: Session):
        self.session = session
    
    def get_random_name(self, name_type: str, gender: Gender = None) -> Optional[NamePool]:
        """Retorna nome aleatório considerando raridade."""
        query = self.session.query(NamePool).filter(
            NamePool.name_type == name_type
        )
        if gender:
            query = query.filter(
                or_(NamePool.gender == gender, NamePool.gender == None)
            )
        
        # TODO: Implementar seleção ponderada por raridade
        return query.order_by(func.random()).first()
    
    def create(self, **kwargs) -> NamePool:
        """Adiciona nome ao pool."""
        name = NamePool(**kwargs)
        self.session.add(name)
        self.session.flush()
        return name


class DatabaseQueries:
    """Classe principal que agrupa todas as queries."""
    
    def __init__(self, session: Session):
        self.session = session
        self.agents = AgentQueries(session)
        self.buildings = BuildingQueries(session)
        self.vehicles = VehicleQueries(session)
        self.events = EventQueries(session)
        self.economic_stats = EconomicStatQueries(session)
        self.professions = ProfessionQueries(session)
        self.names = NamePoolQueries(session)
    
    def commit(self):
        """Commit das alterações."""
        self.session.commit()
    
    def rollback(self):
        """Rollback das alterações."""
        self.session.rollback()

