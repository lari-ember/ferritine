"""
Queries e operações comuns do banco de dados.
"""

from sqlalchemy.orm import Session
from sqlalchemy import func, and_, or_, desc
from typing import List, Optional, Dict, Any
from datetime import datetime, timedelta
import uuid

from backend.database.models import (
    Agent, Building, Vehicle, Event, EconomicStat, 
    Profession, Routine, NamePool, Station,
    CreatedBy, HealthStatus, AgentStatus, Gender, StationType, StationStatus,
    Ticket, TicketStatus, TicketType, Route, Schedule
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

    def get_waiting_at_station(self, station_id: uuid.UUID) -> List[Agent]:
        """Retorna agentes aguardando em uma estação.

        Args:
            station_id: UUID da estação

        Returns:
            Lista de agentes aguardando
        """
        return self.session.query(Agent).filter(
            Agent.waiting_at_station_id == station_id,
            Agent.is_deleted == False
        ).all()

    def get_with_active_ticket(self) -> List[Agent]:
        """Retorna agentes com bilhete válido.

        Returns:
            Lista de agentes com bilhete ativo
        """
        from backend.database.models import Ticket, TicketStatus

        return self.session.query(Agent).join(
            Ticket,
            Agent.current_ticket_id == Ticket.id
        ).filter(
            Ticket.status == TicketStatus.ACTIVE,
            Agent.is_deleted == False
        ).all()


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
            Building.current_occupancy < Building.max_occupancy,
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


# ==================== TICKET QUERIES (Issue 4.8) ====================
class TicketQueries:
    """Queries relacionadas a bilhetes de transporte (Issue 4.8)."""

    def __init__(self, session: Session):
        self.session = session

    # ---------- CRUD / Básico ----------
    def get_by_id(self, ticket_id: uuid.UUID) -> Optional[Ticket]:
        """Retorna bilhete pelo ID."""
        return self.session.query(Ticket).filter(Ticket.id == ticket_id).first()

    def create_ticket(
        self,
        agent_id: uuid.UUID,
        route_id: Optional[uuid.UUID] = None,
        origin_id: Optional[uuid.UUID] = None,
        destination_id: Optional[uuid.UUID] = None,
        ticket_type: TicketType = TicketType.SINGLE,
        price: Optional[float] = None
    ) -> Ticket:
        """Cria bilhete seguindo regras de validade por tipo.

        Se price for None e route_id existir, usa fare_base da rota.
        Não realiza commit; responsabilidade do chamador.
        """
        # Inferir preço se não passado
        if price is None and route_id:
            route = self.session.query(Route).filter(Route.id == route_id).first()
            if route and hasattr(route, 'fare_base'):
                price = route.fare_base
        if price is None:
            price = 0.00

        ticket = Ticket(
            agent_id=agent_id,
            route_id=route_id,
            origin_station_id=origin_id,
            destination_station_id=destination_id,
            ticket_type=ticket_type,
            price=price,
            status=TicketStatus.ACTIVE
        )

        now = datetime.utcnow()
        ticket.valid_from = now

        # Regras de validade similares a Agent.purchase_ticket
        if ticket_type == TicketType.SINGLE:
            ticket.valid_until = now + timedelta(hours=2)
            ticket.max_validations = 1
        elif ticket_type == TicketType.RETURN:
            ticket.valid_until = now + timedelta(days=1)
            ticket.max_validations = 2
        elif ticket_type == TicketType.DAY_PASS:
            ticket.valid_until = now + timedelta(days=1)
            ticket.max_validations = 999
        elif ticket_type == TicketType.WEEK_PASS:
            ticket.valid_until = now + timedelta(days=7)
            ticket.max_validations = 999
        elif ticket_type == TicketType.MONTH_PASS:
            ticket.valid_until = now + timedelta(days=30)
            ticket.max_validations = 999
        elif ticket_type == TicketType.TRANSFER:
            ticket.valid_until = now + timedelta(minutes=30)
            ticket.max_validations = 1

        self.session.add(ticket)
        self.session.flush()  # Obter ID
        return ticket

    # ---------- Operações ----------
    def validate_ticket(self, ticket_id: uuid.UUID) -> bool:
        """Valida (usa) um bilhete pelo ID."""
        ticket = self.get_by_id(ticket_id)
        if not ticket:
            return False
        return ticket.validate()

    # ---------- Listagens ----------
    def get_active_tickets(self, agent_id: uuid.UUID = None) -> List[Ticket]:
        """Retorna bilhetes ativos (opcional filtrar por agente)."""
        query = self.session.query(Ticket).filter(Ticket.status == TicketStatus.ACTIVE)
        if agent_id:
            query = query.filter(Ticket.agent_id == agent_id)
        return query.order_by(desc(Ticket.purchased_at)).all()

    def get_by_agent(self, agent_id: uuid.UUID) -> List[Ticket]:
        """Retorna todos os bilhetes de um agente."""
        return self.session.query(Ticket).filter(Ticket.agent_id == agent_id).order_by(desc(Ticket.purchased_at)).all()

    # ---------- Métricas / Estatísticas ----------
    def get_revenue_by_period(self, start: datetime, end: datetime) -> float:
        """Retorna a soma de preços de bilhetes não cancelados no período."""
        total = self.session.query(func.sum(Ticket.price)).filter(
            Ticket.purchased_at >= start,
            Ticket.purchased_at <= end,
            Ticket.status != TicketStatus.CANCELLED
        ).scalar()
        return float(total) if total else 0.0

    def get_usage_statistics(self, period: str) -> Dict[str, Any]:
        """Retorna estatísticas de uso de bilhetes.

        period: 'day' | 'week' | 'month' | 'all'
        """
        now = datetime.utcnow()
        if period == 'day':
            start = now - timedelta(days=1)
        elif period == 'week':
            start = now - timedelta(weeks=1)
        elif period == 'month':
            start = now - timedelta(days=30)
        elif period == 'all':
            start = None
        else:
            raise ValueError("Período inválido. Use: day, week, month, all")

        query = self.session.query(Ticket)
        if start:
            query = query.filter(Ticket.purchased_at >= start)

        tickets = query.all()
        total = len(tickets)
        active = sum(1 for t in tickets if t.status == TicketStatus.ACTIVE)
        used = sum(1 for t in tickets if t.status == TicketStatus.USED)
        expired = sum(1 for t in tickets if t.status == TicketStatus.EXPIRED)
        cancelled = sum(1 for t in tickets if t.status == TicketStatus.CANCELLED)
        validations = sum(t.validation_count for t in tickets)
        revenue = sum(float(t.price) for t in tickets if t.status != TicketStatus.CANCELLED)

        return {
            'period': period,
            'total': total,
            'active': active,
            'used': used,
            'expired': expired,
            'cancelled': cancelled,
            'avg_validations': (validations / total) if total else 0.0,
            'revenue_total': revenue,
            'usage_rate': (used / total) if total else 0.0
        }


class ScheduleQueries:
    """Queries relacionadas a horários de veículos em rotas (Issue 4.9)."""

    def __init__(self, session: Session):
        self.session = session

    # ---------- CRUD Básico ----------

    def create(self, route_id: uuid.UUID, vehicle_id: uuid.UUID,
               departure_time: datetime.time, days_of_week: List[int],
               is_active: bool = True) -> Schedule:
        """
        Cria um novo horário de veículo em rota.

        Args:
            route_id: UUID da rota
            vehicle_id: UUID do veículo
            departure_time: Horário de partida (time object)
            days_of_week: Lista de dias [0=Monday, 1=Tuesday, ..., 6=Sunday]
            is_active: Se o horário está ativo

        Returns:
            Schedule criado
        """
        schedule = Schedule(
            route_id=route_id,
            vehicle_id=vehicle_id,
            departure_time=departure_time,
            days_of_week=days_of_week,
            is_active=is_active
        )
        self.session.add(schedule)
        self.session.flush()
        return schedule

    def get_by_id(self, schedule_id: uuid.UUID) -> Optional[Schedule]:
        """Busca horário por ID."""
        return self.session.query(Schedule).filter(Schedule.id == schedule_id).first()

    def update(self, schedule_id: uuid.UUID, **kwargs) -> Optional[Schedule]:
        """
        Atualiza um horário.

        Args:
            schedule_id: UUID do horário
            **kwargs: Campos a atualizar

        Returns:
            Schedule atualizado ou None
        """
        schedule = self.get_by_id(schedule_id)
        if schedule:
            for key, value in kwargs.items():
                if hasattr(schedule, key):
                    setattr(schedule, key, value)
            self.session.flush()
        return schedule

    def delete(self, schedule_id: uuid.UUID) -> bool:
        """
        Remove um horário.

        Args:
            schedule_id: UUID do horário

        Returns:
            True se removido, False se não encontrado
        """
        schedule = self.get_by_id(schedule_id)
        if schedule:
            self.session.delete(schedule)
            self.session.flush()
            return True
        return False

    # ---------- Consultas Específicas ----------

    def get_by_route(self, route_id: uuid.UUID, only_active: bool = True) -> List[Schedule]:
        """
        Retorna todos os horários de uma rota.

        Args:
            route_id: UUID da rota
            only_active: Se deve retornar apenas horários ativos

        Returns:
            Lista de horários
        """
        query = self.session.query(Schedule).filter(Schedule.route_id == route_id)
        if only_active:
            query = query.filter(Schedule.is_active == True)
        return query.order_by(Schedule.departure_time).all()

    def get_by_vehicle(self, vehicle_id: uuid.UUID, only_active: bool = True) -> List[Schedule]:
        """
        Retorna todos os horários de um veículo.

        Args:
            vehicle_id: UUID do veículo
            only_active: Se deve retornar apenas horários ativos

        Returns:
            Lista de horários
        """
        query = self.session.query(Schedule).filter(Schedule.vehicle_id == vehicle_id)
        if only_active:
            query = query.filter(Schedule.is_active == True)
        return query.order_by(Schedule.departure_time).all()

    def get_active_schedules(self) -> List[Schedule]:
        """Retorna todos os horários ativos."""
        return self.session.query(Schedule).filter(
            Schedule.is_active == True
        ).order_by(Schedule.route_id, Schedule.departure_time).all()

    def get_schedules_for_day(self, weekday: int, route_id: Optional[uuid.UUID] = None) -> List[Schedule]:
        """
        Retorna horários ativos para um dia da semana específico.

        Args:
            weekday: Dia da semana (0=Monday, 6=Sunday)
            route_id: UUID da rota (opcional, para filtrar por rota)

        Returns:
            Lista de horários que operam no dia especificado
        """
        query = self.session.query(Schedule).filter(
            Schedule.is_active == True
        )

        if route_id:
            query = query.filter(Schedule.route_id == route_id)

        # Filtrar por dia da semana (PostgreSQL e SQLite suportam JSON)
        # Para SQLite, usamos json_each; para PostgreSQL, jsonb_array_elements
        all_schedules = query.all()

        # Filtrar em Python para compatibilidade
        filtered = [s for s in all_schedules if weekday in s.days_of_week]

        return sorted(filtered, key=lambda s: s.departure_time)

    def get_next_departures(self, route_id: uuid.UUID,
                           from_datetime: Optional[datetime] = None,
                           limit: int = 10) -> List[Dict[str, Any]]:
        """
        Retorna as próximas partidas de uma rota.

        Args:
            route_id: UUID da rota
            from_datetime: Momento de referência (default: agora)
            limit: Número máximo de partidas a retornar

        Returns:
            Lista de dicionários com schedule e próximo horário de partida
        """
        if from_datetime is None:
            from_datetime = datetime.utcnow()

        schedules = self.get_by_route(route_id, only_active=True)

        departures = []
        for schedule in schedules:
            next_departure = schedule.get_next_departure(from_datetime)
            if next_departure:
                departures.append({
                    'schedule': schedule,
                    'next_departure': next_departure,
                    'vehicle_id': schedule.vehicle_id,
                    'departure_time': schedule.departure_time
                })

        # Ordenar por próxima partida
        departures.sort(key=lambda x: x['next_departure'])

        return departures[:limit]

    def get_schedules_for_today(self, route_id: Optional[uuid.UUID] = None) -> List[Schedule]:
        """
        Retorna horários ativos para hoje.

        Args:
            route_id: UUID da rota (opcional)

        Returns:
            Lista de horários que operam hoje
        """
        today = datetime.utcnow()
        weekday = today.weekday()
        return self.get_schedules_for_day(weekday, route_id)

    def check_vehicle_availability(self, vehicle_id: uuid.UUID,
                                   departure_time: datetime.time,
                                   days_of_week: List[int]) -> bool:
        """
        Verifica se um veículo está disponível para um novo horário.

        Args:
            vehicle_id: UUID do veículo
            departure_time: Horário de partida desejado
            days_of_week: Dias da semana desejados

        Returns:
            True se disponível, False se houver conflito
        """
        existing_schedules = self.get_by_vehicle(vehicle_id, only_active=True)

        for schedule in existing_schedules:
            # Verificar se há sobreposição de dias
            overlapping_days = set(schedule.days_of_week) & set(days_of_week)
            if overlapping_days:
                # Verificar se horários são muito próximos (menos de 30 min)
                time_diff = abs(
                    (datetime.combine(datetime.today(), departure_time) -
                     datetime.combine(datetime.today(), schedule.departure_time)).total_seconds()
                )
                if time_diff < 1800:  # 30 minutos
                    return False

        return True


class TransportOperatorQueries:
    """Queries relacionadas a operadoras de transporte (Issue 4.10 e 4.11)."""

    def __init__(self, session: Session):
        self.session = session

    # ---------- CRUD Básico ----------

    def get_by_id(self, operator_id: uuid.UUID) -> Optional['TransportOperator']:
        """
        Retorna operadora pelo ID.

        Args:
            operator_id: UUID da operadora

        Returns:
            TransportOperator ou None
        """
        from backend.database.models import TransportOperator
        return self.session.query(TransportOperator).filter(
            TransportOperator.id == operator_id
        ).first()

    def get_all(self, include_inactive: bool = False) -> List['TransportOperator']:
        """
        Retorna todas as operadoras.

        Args:
            include_inactive: Se True, inclui operadoras inativas

        Returns:
            Lista de TransportOperator
        """
        from backend.database.models import TransportOperator
        query = self.session.query(TransportOperator)
        if not include_inactive:
            query = query.filter(TransportOperator.is_active == True)
        return query.order_by(TransportOperator.name).all()

    def get_by_type(self, operator_type: 'StationType') -> List['TransportOperator']:
        """
        Retorna operadoras por tipo de transporte.

        Args:
            operator_type: Tipo de estação/transporte

        Returns:
            Lista de operadoras do tipo especificado
        """
        from backend.database.models import TransportOperator
        return self.session.query(TransportOperator).filter(
            TransportOperator.operator_type == operator_type,
            TransportOperator.is_active == True
        ).order_by(TransportOperator.name).all()

    def create(self, name: str, operator_type: 'StationType',
               revenue: float = 0.0, operational_costs: float = 0.0,
               is_active: bool = True) -> 'TransportOperator':
        """
        Cria uma nova operadora de transporte.

        Args:
            name: Nome da operadora
            operator_type: Tipo de transporte operado
            revenue: Receita inicial
            operational_costs: Custos operacionais iniciais
            is_active: Se a operadora está ativa

        Returns:
            TransportOperator criada
        """
        from backend.database.models import TransportOperator
        from decimal import Decimal

        operator = TransportOperator(
            name=name,
            operator_type=operator_type,
            revenue=Decimal(str(revenue)),
            operational_costs=Decimal(str(operational_costs)),
            is_active=is_active
        )
        self.session.add(operator)
        self.session.flush()
        return operator

    def update(self, operator_id: uuid.UUID, **kwargs) -> Optional['TransportOperator']:
        """
        Atualiza uma operadora existente.

        Args:
            operator_id: UUID da operadora
            **kwargs: Campos a atualizar (name, revenue, operational_costs, is_active)

        Returns:
            TransportOperator atualizada ou None se não encontrada
        """
        from backend.database.models import TransportOperator
        from decimal import Decimal

        operator = self.get_by_id(operator_id)
        if not operator:
            return None

        # Campos permitidos para atualização
        allowed_fields = {'name', 'operator_type', 'revenue', 'operational_costs', 'is_active'}

        for key, value in kwargs.items():
            if key in allowed_fields:
                # Converter para Decimal se for campo financeiro
                if key in ('revenue', 'operational_costs') and value is not None:
                    value = Decimal(str(value))
                setattr(operator, key, value)

        self.session.flush()
        return operator

    # ---------- Operações de Negócio ----------

    def get_most_profitable(self, limit: int = 10) -> List['TransportOperator']:
        """
        Retorna as operadoras mais lucrativas.

        Args:
            limit: Número máximo de resultados

        Returns:
            Lista de operadoras ordenadas por lucro (receita - custos)
        """
        from backend.database.models import TransportOperator

        # Ordenar por (revenue - operational_costs) DESC
        operators = self.session.query(TransportOperator).filter(
            TransportOperator.is_active == True
        ).all()

        # Calcular lucro e ordenar em Python (já que é uma propriedade)
        operators_with_profit = [
            (op, op.get_profit_margin()) for op in operators
        ]
        operators_with_profit.sort(key=lambda x: x[1], reverse=True)

        return [op for op, _ in operators_with_profit[:limit]]

    def calculate_daily_revenue(self, operator_id: uuid.UUID, date: datetime) -> float:
        """
        Calcula receita diária de uma operadora.

        Args:
            operator_id: UUID da operadora
            date: Data para calcular a receita

        Returns:
            Receita do dia (float)
        """
        operator = self.get_by_id(operator_id)
        if not operator:
            return 0.0

        return operator.calculate_daily_revenue(date)

    def get_employees(self, operator_id: uuid.UUID) -> List[Agent]:
        """
        Retorna todos os funcionários de uma operadora.

        Args:
            operator_id: UUID da operadora

        Returns:
            Lista de agentes empregados pela operadora
        """
        return self.session.query(Agent).filter(
            Agent.employer_id == operator_id
        ).all()

    def assign_vehicle_to_route(self, vehicle_id: uuid.UUID,
                               route_id: uuid.UUID) -> bool:
        """
        Atribui um veículo a uma rota.

        Args:
            vehicle_id: UUID do veículo
            route_id: UUID da rota

        Returns:
            True se bem-sucedido, False caso contrário
        """
        vehicle = self.session.query(Vehicle).filter(Vehicle.id == vehicle_id).first()
        route = self.session.query(Route).filter(Route.id == route_id).first()

        if not vehicle or not route:
            return False

        # Verificar se veículo e rota pertencem à mesma operadora
        if vehicle.operator_id != route.operator_id:
            return False

        vehicle.assigned_route_id = route_id
        self.session.flush()
        return True

    # ---------- Estatísticas ----------

    def get_statistics(self, operator_id: uuid.UUID) -> Dict[str, Any]:
        """
        Retorna estatísticas completas de uma operadora.

        Args:
            operator_id: UUID da operadora

        Returns:
            Dicionário com estatísticas
        """
        operator = self.get_by_id(operator_id)
        if not operator:
            return {}

        routes = [r for r in operator.routes if r.is_active]
        vehicles = operator.vehicles
        employees = operator.employees

        return {
            'operator_id': str(operator.id),
            'name': operator.name,
            'type': operator.operator_type.value,
            'is_active': operator.is_active,
            'revenue': float(operator.revenue),
            'operational_costs': float(operator.operational_costs),
            'profit_margin': operator.get_profit_margin(),
            'is_profitable': operator.is_profitable,
            'total_routes': len(operator.routes) if operator.routes else 0,
            'active_routes': len(routes),
            'total_vehicles': operator.total_vehicles,
            'total_employees': operator.total_employees,
            'employees': [
                {
                    'id': str(emp.id),
                    'name': emp.name,
                    'profession': emp.profession.name if emp.profession else None
                } for emp in (employees[:10] if employees else [])  # Limitar a 10
            ]
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
        # Issue 4.8
        self.tickets = TicketQueries(session)
        # Issue 4.9
        self.schedules = ScheduleQueries(session)
        # Issue 4.10 e 4.11
        self.transport_operators = TransportOperatorQueries(session)

    def commit(self):
        """Commit das alterações."""
        self.session.commit()
    
    def rollback(self):
        """Rollback das alterações."""
        self.session.rollback()
