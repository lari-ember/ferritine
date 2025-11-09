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
    Profession, Routine, NamePool, Station,
    CreatedBy, HealthStatus, AgentStatus, Gender, StationType, StationStatus
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

    def get_docked_at_station(self, station_id: uuid.UUID) -> List[Vehicle]:
        """Retorna veículos acoplados em uma estação.

        Args:
            station_id: UUID da estação

        Returns:
            Lista de veículos acoplados na estação
        """
        return self.session.query(Vehicle).filter(
            Vehicle.current_station_id == station_id,
            Vehicle.is_docked == True
        ).all()

    def dock_vehicle(self, vehicle_id: uuid.UUID, station_id: uuid.UUID) -> Optional[Vehicle]:
        """Acopla um veículo em uma estação.

        Args:
            vehicle_id: UUID do veículo
            station_id: UUID da estação

        Returns:
            Veículo atualizado ou None se não encontrado
        """
        vehicle = self.get_by_id(vehicle_id)
        if vehicle:
            vehicle.current_station_id = station_id
            vehicle.is_docked = True
            vehicle.is_moving = False
            vehicle.speed = 0.0
            self.session.flush()
        return vehicle

    def undock_vehicle(self, vehicle_id: uuid.UUID) -> Optional[Vehicle]:
        """Desacopla um veículo de uma estação.

        Args:
            vehicle_id: UUID do veículo

        Returns:
            Veículo atualizado ou None se não encontrado
        """
        vehicle = self.get_by_id(vehicle_id)
        if vehicle:
            vehicle.current_station_id = None
            vehicle.is_docked = False
            self.session.flush()
        return vehicle

    def get_all_docked(self) -> List[Vehicle]:
        """Retorna todos os veículos acoplados em estações.

        Returns:
            Lista de veículos acoplados
        """
        return self.session.query(Vehicle).filter(
            Vehicle.is_docked == True
        ).all()

    def get_by_assigned_route(self, route_id: uuid.UUID) -> List[Vehicle]:
        """Retorna veículos atribuídos a uma rota.

        Args:
            route_id: UUID da rota

        Returns:
            Lista de veículos
        """
        return self.session.query(Vehicle).filter(
            Vehicle.assigned_route_id == route_id
        ).all()


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


class StationQueries:
    """Queries relacionadas a estações de transporte."""

    def __init__(self, session: Session):
        self.session = session

    def get_by_id(self, station_id: uuid.UUID) -> Optional[Station]:
        """Busca estação por ID.

        Args:
            station_id: UUID da estação

        Returns:
            Station ou None se não encontrada
        """
        return self.session.query(Station).filter(
            Station.id == station_id
        ).first()

    def get_all(self, active_only: bool = True) -> List[Station]:
        """Retorna todas as estações.

        Args:
            active_only: Se True, retorna apenas estações ativas

        Returns:
            Lista de estações
        """
        query = self.session.query(Station)
        if active_only:
            query = query.filter(
                Station.status == StationStatus.ACTIVE,
                Station.is_operational == True
            )
        return query.all()

    def get_by_type(self, station_type: StationType) -> List[Station]:
        """Filtra estações por tipo.

        Args:
            station_type: Tipo de estação (ex: METRO_STATION, BUS_STOP_TUBE)

        Returns:
            Lista de estações do tipo especificado
        """
        return self.session.query(Station).filter(
            Station.station_type == station_type,
            Station.is_operational == True
        ).all()

    def get_by_building(self, building_id: uuid.UUID) -> List[Station]:
        """Retorna estações de um edifício.

        Args:
            building_id: UUID do edifício

        Returns:
            Lista de estações dentro/anexas ao edifício
        """
        return self.session.query(Station).filter(
            Station.building_id == building_id
        ).all()

    def get_nearest_station(self, x: int, y: int,
                          station_type: StationType = None,
                          max_distance: int = None) -> Optional[Station]:
        """Encontra a estação mais próxima de uma coordenada.

        Args:
            x: Coordenada X
            y: Coordenada Y
            station_type: Tipo de estação (opcional)
            max_distance: Distância máxima em tiles (opcional)

        Returns:
            Estação mais próxima ou None
        """
        query = self.session.query(
            Station,
            func.sqrt(
                func.pow(Station.x - x, 2) +
                func.pow(Station.y - y, 2)
            ).label('distance')
        ).filter(
            Station.is_operational == True,
            Station.status == StationStatus.ACTIVE
        )

        if station_type:
            query = query.filter(Station.station_type == station_type)

        if max_distance:
            query = query.filter(
                func.sqrt(
                    func.pow(Station.x - x, 2) +
                    func.pow(Station.y - y, 2)
                ) <= max_distance
            )

        result = query.order_by('distance').first()
        return result[0] if result else None

    def get_available_for_docking(self, vehicle_type: str = None,
                                  min_capacity: int = 1) -> List[Station]:
        """Retorna estações com capacidade disponível para embarque.

        Args:
            vehicle_type: Tipo de veículo (opcional, para filtrar estações compatíveis)
            min_capacity: Capacidade mínima livre necessária

        Returns:
            Lista de estações disponíveis
        """
        # Mapeamento de tipos de veículos para tipos de estações compatíveis
        vehicle_station_map = {
            'train': [StationType.TRAIN_PLATFORM, StationType.TRAIN_STOP_LOCAL,
                     StationType.TRAIN_STOP_EXPRESS],
            'bus': [StationType.BUS_STOP_SIMPLE, StationType.BUS_STOP_SHELTER,
                   StationType.BUS_STOP_TUBE, StationType.BUS_STATION_LOCAL,
                   StationType.BRT_STATION],
            'metro': [StationType.METRO_STATION, StationType.METRO_PLATFORM],
            'tram': [StationType.TRAM_STOP, StationType.TRAM_PLATFORM]
        }

        query = self.session.query(Station).filter(
            Station.is_operational == True,
            Station.status == StationStatus.ACTIVE,
            Station.serves_passengers == True,
            (Station.max_queue_length - Station.current_queue_length) >= min_capacity
        )

        if vehicle_type and vehicle_type in vehicle_station_map:
            query = query.filter(
                Station.station_type.in_(vehicle_station_map[vehicle_type])
            )

        return query.all()

    def get_waiting_passengers_count(self, station_id: uuid.UUID) -> int:
        """Conta passageiros esperando em uma estação.

        Args:
            station_id: UUID da estação

        Returns:
            Número de passageiros aguardando
        """
        station = self.get_by_id(station_id)
        return station.current_queue_length if station else 0

    def update_condition(self, station_id: uuid.UUID, condition: int) -> Optional[Station]:
        """Atualiza a condição de uma estação.

        Args:
            station_id: UUID da estação
            condition: Valor de condição (0-100)

        Returns:
            Estação atualizada ou None
        """
        station = self.get_by_id(station_id)
        if station:
            # A validação já está no modelo
            station.condition_value = condition

            # Atualizar status se necessário
            if condition < 20:
                station.status = StationStatus.MAINTENANCE
                station.is_operational = False
            elif condition >= 50:
                if station.status == StationStatus.MAINTENANCE:
                    station.status = StationStatus.ACTIVE
                    station.is_operational = True

            self.session.flush()
        return station

    def create(self, **kwargs) -> Station:
        """Cria uma nova estação.

        Args:
            **kwargs: Atributos da estação

        Returns:
            Estação criada
        """
        station = Station(**kwargs)
        self.session.add(station)
        self.session.flush()
        return station

    def update(self, station_id: uuid.UUID, **kwargs) -> Optional[Station]:
        """Atualiza uma estação.

        Args:
            station_id: UUID da estação
            **kwargs: Atributos a atualizar

        Returns:
            Estação atualizada ou None
        """
        station = self.get_by_id(station_id)
        if station:
            for key, value in kwargs.items():
                if hasattr(station, key):
                    setattr(station, key, value)
            self.session.flush()
        return station

    def get_overcrowded_stations(self, threshold: float = 0.9) -> List[Station]:
        """Retorna estações superlotadas.

        Args:
            threshold: Percentual de ocupação considerado superlotado (0-1)

        Returns:
            Lista de estações superlotadas
        """
        return self.session.query(Station).filter(
            Station.is_operational == True,
            Station.max_queue_length > 0,
            (Station.current_queue_length / Station.max_queue_length) >= threshold
        ).all()

    def get_by_operator(self, company_id: uuid.UUID) -> List[Station]:
        """Retorna estações operadas por uma empresa.

        Args:
            company_id: UUID da empresa operadora

        Returns:
            Lista de estações
        """
        return self.session.query(Station).filter(
            Station.operator_company_id == company_id
        ).all()

    def get_multimodal_stations(self) -> List[Station]:
        """Retorna estações multimodais (conectadas a outras estações).

        Returns:
            Lista de estações multimodais
        """
        return self.session.query(Station).filter(
            Station.connects_to_stations != None,
            func.json_array_length(Station.connects_to_stations) > 0
        ).all()

    def get_needing_maintenance(self) -> List[Station]:
        """Retorna estações que precisam de manutenção.

        Returns:
            Lista de estações
        """
        return self.session.query(Station).filter(
            or_(
                Station.condition_value < 50,
                and_(
                    Station.next_maintenance_at != None,
                    Station.next_maintenance_at <= datetime.utcnow()
                )
            )
        ).all()

    def get_statistics(self) -> Dict[str, Any]:
        """Retorna estatísticas sobre estações.

        Returns:
            Dicionário com estatísticas
        """
        total = self.session.query(func.count(Station.id)).scalar()

        operational = self.session.query(func.count(Station.id)).filter(
            Station.is_operational == True
        ).scalar()

        avg_condition = self.session.query(func.avg(Station.condition_value)).scalar()

        total_passengers_waiting = self.session.query(
            func.sum(Station.current_queue_length)
        ).scalar()

        by_type = self.session.query(
            Station.station_type,
            func.count(Station.id)
        ).group_by(Station.station_type).all()

        by_status = self.session.query(
            Station.status,
            func.count(Station.id)
        ).group_by(Station.status).all()

        return {
            'total': total or 0,
            'operational': operational or 0,
            'average_condition': float(avg_condition) if avg_condition else 0.0,
            'total_passengers_waiting': int(total_passengers_waiting) if total_passengers_waiting else 0,
            'by_type': {str(stype.value): count for stype, count in by_type},
            'by_status': {str(status.value): count for status, count in by_status}
        }


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
        self.stations = StationQueries(session)

    def commit(self):
        """Commit das alterações."""
        self.session.commit()
    
    def rollback(self):
        """Rollback das alterações."""
        self.session.rollback()

