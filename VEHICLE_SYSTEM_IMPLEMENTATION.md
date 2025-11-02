# Vehicle Transport System Implementation

## Overview

This implementation adds a comprehensive vehicle transport system to Ferritine, a city simulation game set in Brazil between 1860-2000. The system includes vehicle management, AI-based route optimization, and driver fatigue simulation.

## Components Implemented

### 1. Core Vehicle System (`backend/simulation/models/vehicle.py`)

#### Vehicle Base Class
A comprehensive base class that manages all aspects of vehicle operation:

**Features:**
- **Movement & Navigation**: Trip management, route tracking, position updates
- **Passenger Management**: Boarding/alighting, capacity limits, ticketing
- **Cargo Management**: Loading/unloading with capacity constraints
- **Fuel System**: Consumption based on distance, refueling, fuel types (coal, diesel, electricity)
- **Maintenance**: Wear and tear calculation, scheduled maintenance, condition tracking
- **Accidents**: Severity levels (minor, moderate, severe, fatal), casualty simulation
- **Financial**: Depreciation, profit calculation, cost tracking
- **Lifecycle**: Creation, operation, retirement

**Status Management:**
- IDLE - Parked/waiting
- MOVING - In transit
- BOARDING - Loading passengers
- MAINTENANCE - Under repair
- BROKEN - Damaged/inoperable
- RETIRED - End of service

#### Specialized Vehicle Types

1. **Train**
   - Capacity: 200 passengers, 5000kg cargo
   - Fuel: Coal (steam era)
   - Crew: 2 (engineer + stoker)
   - Special: Wagon coupling/decoupling system

2. **Bus**
   - Capacity: 40 passengers
   - Fuel: Diesel
   - Crew: 2 (driver + conductor)
   - Special: Express mode (1.3x speed)

3. **Tram**
   - Capacity: 60 passengers
   - Fuel: Electricity
   - Crew: 1 (driver)
   - Special: Overhead pantograph system

4. **Taxi**
   - Capacity: 4 passengers
   - Fuel: Diesel
   - Crew: 1 (driver)
   - Special: On-demand service with fare calculation

5. **Truck**
   - Capacity: 10000kg cargo
   - Fuel: Diesel
   - Special: Trailer attachment (2x capacity)

### 2. AI Systems

#### Route Optimizer (`backend/ai/route_optimizer.py`)
Analyzes historical demand patterns and provides intelligent suggestions:

**Features:**
- Hourly demand analysis over configurable time windows
- Peak hour detection (>80% occupancy)
- Low-demand hour identification (<30% occupancy)
- Frequency adjustment recommendations
- Express line suggestions for high-demand routes

**Suggestions Generated:**
- Increase frequency: When occupancy >85%
- Decrease frequency: When occupancy <25%
- Add express lines: When occupancy >70% AND >150 passengers

#### Driver Fatigue System (`backend/simulation/driver_fatigue.py`)
Simulates crew tiredness and safety implications:

**Features:**
- Fatigue accumulation: 5% per hour worked
- Fatigue cap: 100%
- Rest/reset functionality
- Accident risk calculation based on fatigue level

**Risk Calculation:**
- Low fatigue (0-50%): 0-25% accident risk
- Medium fatigue (50-80%): 25-55% accident risk
- High fatigue (80-100%): 55-100% accident risk

### 3. Database Layer (`backend/database/vehicle_db.py`)

Provides persistence interface for:
- Vehicle data (CRUD operations)
- Routes and trips
- Passenger tickets
- Maintenance records
- Incident/accident logging
- Daily statistics and analytics

**Key Operations:**
- `create_trip()`: Start new journey
- `complete_trip()`: Finish journey
- `create_ticket()`: Board passenger
- `create_maintenance_record()`: Log repairs
- `create_incident()`: Record accidents
- `get_vehicle_stats()`: Retrieve performance metrics

## Testing

### Test Coverage

**Vehicle Tests** (40 tests):
- Basic vehicle creation and configuration
- Movement and trip management
- Passenger and cargo operations
- Maintenance system
- Fuel consumption and refueling
- Accident simulation
- Specialized vehicle types
- Operational status checks
- Financial calculations

**AI Module Tests** (20 tests):
- Route demand analysis
- Frequency adjustment suggestions
- Peak/low hour detection
- Driver fatigue tracking
- Accident risk calculation
- Multi-driver management

**Total: 60 tests, 100% passing**

### Test Files
- `tests/unit/test_vehicle.py` - Vehicle system tests
- `tests/unit/test_ai_modules.py` - AI system tests

## Code Quality

- ✅ All 60 tests passing
- ✅ Flake8 linting: 0 errors
- ✅ CodeQL security scan: 0 vulnerabilities
- ✅ Type hints throughout
- ✅ Comprehensive docstrings
- ✅ Consistent with codebase style (Portuguese documentation)

## Usage Examples

### Creating and Operating a Bus
```python
from backend.simulation.models.vehicle import Bus
from backend.database.vehicle_db import VehicleDatabase

# Create database connection
db = VehicleDatabase(session)

# Create a bus
bus = Bus(
    db,
    name="Ônibus Urbano 101",
    model="Mercedes-Benz O-305",
    year_built=1980
)

# Start a trip
trip_id = bus.start_trip(route_id=5, station_id=1)

# Board passengers
bus.board_passenger(agent_id=123, boarding_station_id=1, fare=3.50)
bus.board_passenger(agent_id=456, boarding_station_id=1, fare=3.50)

# Move the bus (simulation step)
bus.move(delta_time_hours=0.25)  # 15 minutes

# Check status
print(bus.get_status_summary())

# Complete trip
bus.complete_trip()
```

### Using Route Optimizer
```python
from backend.ai.route_optimizer import RouteOptimizer

optimizer = RouteOptimizer(db)

# Analyze demand for route 5 over last 30 days
analysis = optimizer.analyze_demand(route_id=5, time_window_days=30)

print(f"Peak hours: {analysis['peak_hours']}")
print(f"Low hours: {analysis['low_hours']}")

# Get recommendations
suggestions = optimizer.suggest_frequency_adjustment(route_id=5)

for rec in suggestions['increase_frequency']:
    print(f"Hour {rec['hour']}: {rec['recommendation']}")
```

### Tracking Driver Fatigue
```python
from backend.simulation.driver_fatigue import DriverFatigueSystem

fatigue = DriverFatigueSystem(db)

# Driver works a shift
fatigue.update_fatigue(driver_id=42, hours_worked=4.0)

# Check if too tired
if fatigue.is_too_tired(driver_id=42):
    print("Driver needs rest!")
    
# Calculate accident risk
risk = fatigue.calculate_accident_risk(driver_id=42)
print(f"Accident risk: {risk*100:.1f}%")
```

## Integration Points

### With Existing Systems
- **Agents**: Passengers and drivers are agents from `backend/database/models.py`
- **Buildings**: Stations and depots reference building system
- **Events**: Vehicle operations logged to event system
- **Economics**: Revenue, costs tracked in economic system

### Future Enhancements
- Integration with SQLAlchemy models for persistence
- Connection to building-based stations
- Real-time route planning
- Weather effects on vehicle performance
- Historical era restrictions (e.g., no diesel in 1860)
- Multiplayer fleet competition
- UI for fleet management

## Performance Considerations

- Vehicle position updates: O(1)
- Passenger boarding: O(1)
- Maintenance checks: O(1) per vehicle
- Route analysis: O(n) where n = number of trips
- Fatigue tracking: O(1) per driver

## Files Added/Modified

**New Files:**
- `backend/simulation/models/vehicle.py` (675 lines)
- `backend/ai/route_optimizer.py` (86 lines)
- `backend/simulation/driver_fatigue.py` (68 lines)
- `backend/database/vehicle_db.py` (225 lines)
- `tests/unit/test_vehicle.py` (673 lines)
- `tests/unit/test_ai_modules.py` (267 lines)
- `backend/ai/__init__.py`

**Total: 1,994 lines of production code + tests**

## Security Summary

CodeQL analysis completed with **0 vulnerabilities** detected.

All code follows secure coding practices:
- No SQL injection risks (using parameterized queries)
- No hardcoded credentials
- Safe random number generation
- Proper input validation
- No unsafe deserialization

## Conclusion

This implementation provides a solid foundation for the vehicle transport system in Ferritine. The system is:
- ✅ Feature-complete per requirements
- ✅ Well-tested (60 passing tests)
- ✅ Secure (0 vulnerabilities)
- ✅ Clean code (passes linting)
- ✅ Documented (comprehensive docstrings)
- ✅ Extensible (easy to add new vehicle types)

The modular design allows for future expansion while maintaining backward compatibility.
