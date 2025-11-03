# Guia de Uso: Building Model

## 📋 Índice
- [Visão Geral](#visão-geral)
- [Enums Disponíveis](#enums-disponíveis)
- [Criando Edifícios](#criando-edifícios)
- [Consultas e Filtros](#consultas-e-filtros)
- [Relacionamentos com Agent](#relacionamentos-com-agent)
- [Métodos Úteis](#métodos-úteis)
- [Exemplos Práticos](#exemplos-práticos)

---

## 🎯 Visão Geral

O modelo `Building` foi expandido para suportar **150+ tipos de edifícios** com atributos complexos incluindo:

- **Identificação e Localização**: Nome, tipo, coordenadas, endereço, zoneamento
- **Status e Condição**: 50+ estados operacionais, condição física (0-100)
- **Propriedade**: Dono (agente), tipo de propriedade (privado, público, religioso, etc)
- **Arquitetura**: Estilo por era, ano de construção, dimensões detalhadas
- **Economia**: Custos operacionais, receitas, valor de mercado, ROI
- **Histórico**: Linha do tempo, eventos importantes, reformas
- **Sustentabilidade**: Consumo energético, emissões, certificações
- **Segurança**: Sistemas de alarme, inspeções, resistência estrutural
- **IoT**: Integração com Arduino (LED, sensores)

---

## 🏗️ Enums Disponíveis

### BuildingType (150+ tipos)

```python
from backend.database.models import BuildingType

# Residenciais (10 tipos)
BuildingType.RESIDENTIAL_HOUSE_SMALL
BuildingType.RESIDENTIAL_HOUSE_MEDIUM
BuildingType.RESIDENTIAL_APARTMENT_HIGH
BuildingType.RESIDENTIAL_VILLA

# Comerciais (18 tipos)
BuildingType.COMMERCIAL_SUPERMARKET
BuildingType.COMMERCIAL_RESTAURANT
BuildingType.COMMERCIAL_BANK
BuildingType.COMMERCIAL_OFFICE_TOWER

# Industriais (15 tipos)
BuildingType.INDUSTRIAL_FACTORY_TEXTILE
BuildingType.INDUSTRIAL_BREWERY
BuildingType.INDUSTRIAL_POWER_PLANT_HYDRO

# Públicos (20 tipos)
BuildingType.PUBLIC_SCHOOL_ELEMENTARY
BuildingType.PUBLIC_HOSPITAL_LARGE
BuildingType.PUBLIC_MUSEUM
BuildingType.PUBLIC_FIRE_STATION

# Transporte (15 tipos)
BuildingType.TRANSPORT_TRAIN_STATION_CENTRAL
BuildingType.TRANSPORT_BUS_TERMINAL
BuildingType.TRANSPORT_AIRPORT

# Lazer (16 tipos)
BuildingType.LEISURE_PARK_LARGE
BuildingType.LEISURE_STADIUM
BuildingType.LEISURE_CINEMA

# Infraestrutura (11 tipos)
BuildingType.INFRASTRUCTURE_POWER_SUBSTATION
BuildingType.INFRASTRUCTURE_WATER_TOWER

# Especiais (10 tipos)
BuildingType.SPECIAL_RUINS
BuildingType.SPECIAL_CONSTRUCTION_SITE
BuildingType.SPECIAL_LANDMARK
```

### BuildingStatus (50+ status)

```python
from backend.database.models import BuildingStatus

# Planejamento
BuildingStatus.PLANNING_PROPOSED
BuildingStatus.PLANNING_APPROVED

# Construção
BuildingStatus.CONSTRUCTION_FOUNDATION  # 0-25%
BuildingStatus.CONSTRUCTION_STRUCTURE   # 25-50%
BuildingStatus.CONSTRUCTION_WALLS       # 50-75%
BuildingStatus.CONSTRUCTION_FINISHING   # 75-99%

# Operação
BuildingStatus.OPERATIONAL_ACTIVE
BuildingStatus.OPERATIONAL_BUSY
BuildingStatus.OPERATIONAL_SLOW

# Manutenção
BuildingStatus.MAINTENANCE_RENOVATION
BuildingStatus.MAINTENANCE_EMERGENCY

# Problemas
BuildingStatus.DAMAGED_FIRE
BuildingStatus.DAMAGED_FLOOD
BuildingStatus.DAMAGED_SEVERE

# Desativado
BuildingStatus.ABANDONED_OLD
BuildingStatus.CONDEMNED

# Eventos especiais
BuildingStatus.EVENT_HOSTING
BuildingStatus.QUARANTINED
BuildingStatus.STRIKE
```

### BuildingCondition

```python
from backend.database.models import BuildingCondition

BuildingCondition.EXCELLENT   # 90-100
BuildingCondition.GOOD        # 70-89
BuildingCondition.FAIR        # 50-69
BuildingCondition.POOR        # 30-49
BuildingCondition.VERY_POOR   # 10-29
BuildingCondition.RUINOUS     # 0-9
```

### BuildingArchitectureStyle (por era)

```python
from backend.database.models import BuildingArchitectureStyle

# Era 1 (1860-1920)
BuildingArchitectureStyle.COLONIAL_PORTUGUESE
BuildingArchitectureStyle.VICTORIAN
BuildingArchitectureStyle.NEOCLASSICAL

# Era 2 (1920-1960)
BuildingArchitectureStyle.ART_DECO
BuildingArchitectureStyle.MODERNIST

# Era 3 (1960-2000)
BuildingArchitectureStyle.BRUTALIST
BuildingArchitectureStyle.POSTMODERN

# Era 4 (2000+)
BuildingArchitectureStyle.CONTEMPORARY
BuildingArchitectureStyle.ECO_SUSTAINABLE
```

---

## 🏢 Criando Edifícios

### Exemplo Básico

```python
from backend.database.models import Building, BuildingType, BuildingStatus
from backend.database.connection import SessionLocal
from decimal import Decimal

session = SessionLocal()

# Criar casa simples
house = Building(
    name="Casa da Família Silva",
    building_type=BuildingType.RESIDENTIAL_HOUSE_SMALL,
    x=10,
    y=20,
    address="Rua das Flores, 123",
    neighborhood="Centro",
    construction_year=1920,
    era=2,
    max_occupancy=4,
    current_market_value=Decimal("250000.00")
)

session.add(house)
session.commit()
```

### Exemplo Completo: Fábrica Têxtil

```python
from backend.database.models import (
    Building, BuildingType, BuildingStatus, 
    BuildingArchitectureStyle, BuildingOwnershipType
)
from decimal import Decimal

factory = Building(
    # Identificação
    name="Fábrica de Tecidos Silva & Cia",
    building_type=BuildingType.INDUSTRIAL_FACTORY_TEXTILE,
    
    # Localização
    x=50,
    y=30,
    address="Av. Industrial, 500",
    neighborhood="Distrito Industrial",
    postal_code="12345-678",
    
    # Dimensões
    width=50.0,
    length=80.0,
    height=10.0,
    floors=2,
    
    # Status
    status=BuildingStatus.OPERATIONAL_ACTIVE,
    condition_value=75,
    
    # Propriedade
    owner_type=BuildingOwnershipType.PRIVATE_COMPANY,
    
    # Arquitetura
    architecture_style=BuildingArchitectureStyle.ART_DECO,
    construction_year=1925,
    era=2,
    
    # Capacidade
    max_occupancy=150,
    current_occupancy=120,
    parking_spaces=20,
    
    # Construção
    foundation_type="concrete",
    structure_type="steel",
    roof_type="tile",
    
    # Utilidades
    has_electricity=True,
    has_water=True,
    has_sewage=True,
    has_generator=True,
    
    # Economia
    land_value=Decimal("500000.00"),
    construction_cost=Decimal("2000000.00"),
    current_market_value=Decimal("3500000.00"),
    maintenance_cost=Decimal("5000.00"),
    utility_costs=Decimal("8000.00"),
    business_revenue=Decimal("150000.00"),
    jobs_created=120,
    
    # Meio Ambiente
    energy_consumption_kwh_month=15000.0,
    water_consumption_m3_month=500.0,
    co2_emissions_kg_year=180000.0,
    noise_level_db=75.0,
    
    # Segurança
    has_fire_alarm=True,
    has_fire_extinguishers=True,
    has_emergency_exits=4,
    
    # IoT
    has_led=True,
    led_pin=5
)

session.add(factory)
session.commit()
```

---

## 🔍 Consultas e Filtros

### Por Tipo

```python
from sqlalchemy import select

# Todas as casas
stmt = select(Building).where(
    Building.building_type.in_([
        BuildingType.RESIDENTIAL_HOUSE_SMALL,
        BuildingType.RESIDENTIAL_HOUSE_MEDIUM,
        BuildingType.RESIDENTIAL_HOUSE_LARGE
    ])
)
houses = session.execute(stmt).scalars().all()

# Todas as fábricas
stmt = select(Building).where(
    Building.building_type.like("industrial_factory_%")
)
factories = session.execute(stmt).scalars().all()
```

### Por Status

```python
# Edifícios em construção
stmt = select(Building).where(
    Building.status.in_([
        BuildingStatus.CONSTRUCTION_FOUNDATION,
        BuildingStatus.CONSTRUCTION_STRUCTURE,
        BuildingStatus.CONSTRUCTION_WALLS,
        BuildingStatus.CONSTRUCTION_FINISHING
    ])
)
under_construction = session.execute(stmt).scalars().all()

# Edifícios danificados
stmt = select(Building).where(
    Building.condition_value < 50
)
damaged = session.execute(stmt).scalars().all()
```

### Por Localização

```python
# Edifícios em uma área específica (grid 10x10 a 20x20)
stmt = select(Building).where(
    Building.x.between(10, 20),
    Building.y.between(10, 20)
)
area_buildings = session.execute(stmt).scalars().all()
```

### Por Economia

```python
# Edifícios lucrativos
stmt = select(Building).where(
    (Building.rental_income + Building.business_revenue) >
    (Building.maintenance_cost + Building.utility_costs + Building.tax_property)
)
profitable = session.execute(stmt).scalars().all()

# Top 10 mais valiosos
stmt = select(Building).order_by(
    Building.current_market_value.desc()
).limit(10)
most_valuable = session.execute(stmt).scalars().all()
```

---

## 👥 Relacionamentos com Agent

### Atribuir Proprietário

```python
from backend.database.models import Agent, Building

# Buscar agente e edifício
agent = session.query(Agent).filter_by(name="João Silva").first()
building = session.query(Building).filter_by(name="Casa Silva").first()

# Definir proprietário
building.owner_id = agent.id
building.owner = agent  # Ou usar o objeto diretamente
session.commit()

# Acessar edifícios de um agente
print(f"{agent.name} possui {len(agent.owned_buildings)} edifícios:")
for b in agent.owned_buildings:
    print(f"  - {b.name} (R$ {b.current_market_value})")
```

### Atribuir Residentes

```python
# Definir casa do agente
agent.home_building_id = house.id
agent.home = house
session.commit()

# Atualizar ocupação
house.current_occupancy = len(house.residents)
session.commit()

# Listar moradores
print(f"Moradores de {house.name}:")
for resident in house.residents:
    print(f"  - {resident.name}, {resident.age} anos")
```

### Atribuir Trabalhadores

```python
# Definir local de trabalho
agent.work_building_id = factory.id
agent.workplace = factory
session.commit()

# Atualizar ocupação
factory.current_occupancy = len(factory.workers)
factory.jobs_created = len(factory.workers)
session.commit()

# Listar funcionários
print(f"Funcionários da {factory.name}:")
for worker in factory.workers:
    print(f"  - {worker.name} - {worker.profession.name}")
```

---

## 🛠️ Métodos Úteis

### Cálculos Econômicos

```python
building = session.query(Building).first()

# Custos mensais totais
total_costs = building.calculate_monthly_costs()
print(f"Custos: R$ {total_costs:.2f}")

# Receita mensal total
total_income = building.calculate_monthly_income()
print(f"Receita: R$ {total_income:.2f}")

# Verificar lucratividade
if building.is_profitable():
    profit = total_income - total_costs
    print(f"Lucro mensal: R$ {profit:.2f}")
else:
    loss = total_costs - total_income
    print(f"Prejuízo mensal: R$ {loss:.2f}")
```

### Ocupação

```python
# Taxa de ocupação
rate = building.get_occupancy_rate()
print(f"Ocupação: {rate*100:.1f}%")

# Verificar se pode acomodar mais pessoas
if building.can_accommodate(5):
    print("Há espaço para mais 5 pessoas")
else:
    print("Lotado!")
```

### Idade e Condição

```python
# Idade do edifício
print(f"Construído em {building.construction_year}")
print(f"Idade: {building.age} anos")

# Verificar operacionalidade
if building.is_operational():
    print("Edifício operacional")
else:
    print("Fora de operação")
```

---

## 💡 Exemplos Práticos

### Cenário 1: Construindo uma Casa Nova

```python
from datetime import datetime, timedelta

# Fase 1: Proposta
new_house = Building(
    name="Casa Nova - Rua das Palmeiras",
    building_type=BuildingType.RESIDENTIAL_HOUSE_MEDIUM,
    x=35,
    y=42,
    status=BuildingStatus.PLANNING_PROPOSED,
    construction_progress=0,
    construction_year=2025,
    max_occupancy=6
)
session.add(new_house)
session.commit()

# Fase 2: Aprovação e início
new_house.status = BuildingStatus.PLANNING_APPROVED
new_house.construction_started = datetime.utcnow()
new_house.estimated_completion_date = datetime.utcnow() + timedelta(days=180)
session.commit()

# Fase 3: Fundação
new_house.status = BuildingStatus.CONSTRUCTION_FOUNDATION
new_house.construction_progress = 25
session.commit()

# Fase 4: Estrutura
new_house.status = BuildingStatus.CONSTRUCTION_STRUCTURE
new_house.construction_progress = 50
session.commit()

# Fase 5: Paredes
new_house.status = BuildingStatus.CONSTRUCTION_WALLS
new_house.construction_progress = 75
session.commit()

# Fase 6: Acabamento
new_house.status = BuildingStatus.CONSTRUCTION_FINISHING
new_house.construction_progress = 95
session.commit()

# Fase 7: Inauguração
new_house.status = BuildingStatus.OPERATIONAL_NEW
new_house.construction_progress = 100
new_house.construction_completed = datetime.utcnow()
new_house.inauguration_date = datetime.utcnow()
new_house.condition_value = 100
new_house.condition = BuildingCondition.EXCELLENT
session.commit()
```

### Cenário 2: Evento de Incêndio

```python
import json
from datetime import datetime

building = session.query(Building).filter_by(name="Fábrica Silva").first()

# Registrar evento
event = {
    "date": datetime.utcnow().isoformat(),
    "type": "fire",
    "severity": 70,
    "condition_before": building.condition_value,
    "description": "Incêndio causado por curto-circuito no 2º andar"
}

# Atualizar condição
building.condition_value = max(0, building.condition_value - 70)
building.status = BuildingStatus.DAMAGED_FIRE

# Salvar evento no histórico
if not building.major_events:
    building.major_events = []
building.major_events.append(event)

# Atualizar valor de mercado
building.current_market_value *= Decimal("0.4")  # Perde 60% do valor

session.commit()

print(f"Edifício danificado: condição {building.condition_value}/100")
```

### Cenário 3: Reforma e Modernização

```python
from decimal import Decimal
from datetime import datetime

# Edifício antigo
old_building = session.query(Building).filter_by(
    construction_year=1920
).first()

# Registrar início da reforma
renovation_event = {
    "date": datetime.utcnow().isoformat(),
    "type": "renovation",
    "cost": 150000.00,
    "description": "Reforma completa: fachada, telhado, instalações"
}

old_building.status = BuildingStatus.MAINTENANCE_RENOVATION
old_building.last_renovation = datetime.utcnow()

# Melhorias
old_building.condition_value = min(100, old_building.condition_value + 40)
old_building.has_electricity = True
old_building.has_water = True
old_building.has_sewage = True
old_building.wheelchair_accessible = True

# Atualizar economia
investment = Decimal("150000.00")
old_building.total_invested += investment
old_building.current_market_value += investment * Decimal("0.7")

# Modernização arquitetônica
old_building.architecture_style = BuildingArchitectureStyle.CONTEMPORARY

# Salvar no histórico
if not old_building.renovations:
    old_building.renovations = []
old_building.renovations.append(renovation_event)

# Retornar à operação
old_building.status = BuildingStatus.OPERATIONAL_ACTIVE

session.commit()

print(f"Reforma concluída! Nova condição: {old_building.condition_value}/100")
print(f"Novo valor: R$ {old_building.current_market_value}")
```

### Cenário 4: Análise de Sustentabilidade

```python
# Buscar edifícios sustentáveis
stmt = select(Building).where(
    Building.has_solar_panels == True,
    Building.energy_efficiency_rating.in_(["A+", "A", "B"])
)
sustainable = session.execute(stmt).scalars().all()

print(f"Edifícios sustentáveis: {len(sustainable)}")

# Calcular emissões totais da cidade
total_co2 = session.query(
    func.sum(Building.co2_emissions_kg_year)
).scalar() or 0

print(f"Emissões totais: {total_co2:,.0f} kg CO2/ano")

# Top 5 maiores poluidores
stmt = select(Building).order_by(
    Building.co2_emissions_kg_year.desc()
).limit(5)
polluters = session.execute(stmt).scalars().all()

print("\nMaiores poluidores:")
for b in polluters:
    print(f"  {b.name}: {b.co2_emissions_kg_year:,.0f} kg/ano")
```

### Cenário 5: Sistema de Alertas

```python
from sqlalchemy import and_, or_

# Edifícios que precisam de manutenção urgente
stmt = select(Building).where(
    or_(
        Building.condition_value < 30,
        Building.safety_violations > 2,
        and_(
            Building.last_inspection.isnot(None),
            Building.last_inspection < datetime.utcnow() - timedelta(days=365)
        )
    )
)
needs_attention = session.execute(stmt).scalars().all()

print(f"⚠️  ALERTAS: {len(needs_attention)} edifícios precisam de atenção\n")

for building in needs_attention:
    print(f"🏢 {building.name}")
    print(f"   Condição: {building.condition_value}/100")
    print(f"   Violações: {building.safety_violations}")
    if building.last_inspection:
        days = (datetime.utcnow() - building.last_inspection).days
        print(f"   Última inspeção: há {days} dias")
    print()
```

---

## 📊 Estatísticas da Cidade

```python
from sqlalchemy import func

# Total de edifícios por tipo
type_stats = session.query(
    Building.building_type,
    func.count(Building.id).label('count')
).group_by(Building.building_type).all()

print("Edifícios por tipo:")
for building_type, count in type_stats:
    print(f"  {building_type.value}: {count}")

# Valor total da cidade
total_value = session.query(
    func.sum(Building.current_market_value)
).scalar() or Decimal("0")

print(f"\nValor total: R$ {total_value:,.2f}")

# Edifício mais antigo
oldest = session.query(Building).order_by(
    Building.construction_year.asc()
).first()

print(f"\nEdifício mais antigo: {oldest.name} ({oldest.construction_year})")

# Edifício mais alto
tallest = session.query(Building).order_by(
    Building.height.desc()
).first()

print(f"Edifício mais alto: {tallest.name} ({tallest.height}m)")
```

---

## 🎯 Próximos Passos

1. ✅ **Modelos completos**: Agent e Building implementados
2. ⏭️ **Migrations**: Criar com Alembic
3. ⏭️ **Testes unitários**: Validar relacionamentos e métodos
4. ⏭️ **Seeders**: Popular banco com dados iniciais
5. ⏭️ **API REST**: Endpoints para CRUD de buildings

---

**Documentação criada em**: 2025-11-02  
**Versão do projeto**: Ferritine v0.1.0  
**Issue relacionada**: #4 - Configurar banco de dados PostgreSQL

