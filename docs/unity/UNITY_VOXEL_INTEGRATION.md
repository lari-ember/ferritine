# Unity Engine Integration - Technical Specification

## Overview

Ferritine will use **Unity Engine** for 3D visualization with **voxel-style textures** to create a unique aesthetic that combines historical authenticity with modern rendering.

## Engine Selection

### Unity Engine
- **Version**: Unity 2022 LTS or later
- **Rendering Pipeline**: Universal Render Pipeline (URP) for optimal performance
- **Platform Support**: Desktop (Windows, Linux, macOS) with potential for mobile/AR expansion

### Why Unity?
1. **C# Integration**: Strong scripting capabilities with .NET
2. **Asset Pipeline**: Excellent support for procedural generation and voxel-based assets
3. **Physics Engine**: Built-in physics for vehicle simulation
4. **AR Foundation**: Future-proof for augmented reality features
5. **Community**: Large ecosystem of voxel art tools and assets

## Voxel Texture System

### Visual Style
- **Aesthetic**: Minecraft/Teardown-inspired voxel art
- **Resolution**: Low-poly voxel models with high-quality textures
- **Color Palette**: Historical color schemes matching each era (1860-2000)

### Asset Creation Pipeline

#### Buildings
```
Historical Research → Voxel Modeling (MagicaVoxel) → Unity Import → Material Assignment
```

**Tools**:
- **MagicaVoxel**: Primary modeling tool for voxel structures
- **Qubicle**: Alternative for complex buildings
- **Unity Voxel Importer**: Custom scripts for .vox file import

#### Vehicles
```
Reference Photos → Voxel Design → Animation Rig → Unity Prefab
```

**Vehicle Types** (voxel models):
- Steam locomotives (Era 1: 1860-1920)
- Diesel trains (Era 2-3: 1920-1980)
- Modern electric trains (Era 4: 1980-2000)
- **BRT Biarticulado** (inspired by Curitiba's system)
- Buses (various eras)
- Trams (bondes elétricos)
- Taxis and cars

### Texture System

#### Era-Specific Palettes
**Era 1 (1860-1920) - Colonial/Industrial**:
- Weathered wood, red brick, oxidized copper
- Sepia-toned earth colors
- Steam/coal soot effects

**Era 2 (1920-1960) - Art Deco/Modernism**:
- Pastels, chrome, white marble
- Geometric patterns
- Electric lighting effects

**Era 3 (1960-1990) - Brutalist/Concrete**:
- Grey concrete, exposed aggregate
- Bold primary colors
- Neon signage

**Era 4 (1990-2000) - Contemporary**:
- Glass, steel, reflective surfaces
- Digital displays
- LED lighting

#### Material Properties
```csharp
// Example Unity shader properties for voxel materials
Shader "Ferritine/VoxelPBR"
{
    Properties
    {
        _MainTex ("Albedo", 2D) = "white" {}
        _VoxelSize ("Voxel Size", Float) = 0.1
        _Roughness ("Roughness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _EmissionColor ("Emission", Color) = (0,0,0,1)
        _WeatherIntensity ("Weather/Age", Range(0,1)) = 0.0
    }
}
```

## Python-Unity Communication

### Architecture
```
┌─────────────────┐         WebSocket/REST        ┌──────────────────┐
│  Python Backend │ ←─────────────────────────────→│  Unity Client    │
│  (Simulation)   │                                │  (Visualization) │
│                 │         JSON Messages          │                  │
│  - Agents       │ ─────────────────────────────→ │  - Scene Render  │
│  - Vehicles     │ ←───────────────────────────── │  - User Input    │
│  - Buildings    │                                │  - Camera        │
└─────────────────┘                                └──────────────────┘
```

### Message Protocol

#### Vehicle Update (Python → Unity)
```json
{
    "type": "vehicle_update",
    "timestamp": 1699000000,
    "vehicles": [
        {
            "id": 42,
            "type": "brt",
            "name": "BRT Linha Verde",
            "position": {"x": 10.5, "y": 0.0, "z": 25.3},
            "rotation": {"y": 45.0},
            "speed": 60.0,
            "passengers": 180,
            "status": "moving"
        }
    ]
}
```

#### Building State (Python → Unity)
```json
{
    "type": "building_update",
    "buildings": [
        {
            "id": 15,
            "type": "station_brt",
            "name": "Estação Tubo Central",
            "position": {"x": 10.0, "y": 0.0, "z": 25.0},
            "occupancy": 45,
            "lights_on": true,
            "era": 4
        }
    ]
}
```

### Unity C# Integration

#### WebSocket Client
```csharp
using WebSocketSharp;
using UnityEngine;
using Newtonsoft.Json;

public class SimulationConnector : MonoBehaviour
{
    private WebSocket ws;
    
    void Start()
    {
        ws = new WebSocket("ws://localhost:8765/simulation");
        ws.OnMessage += OnSimulationMessage;
        ws.Connect();
    }
    
    void OnSimulationMessage(object sender, MessageEventArgs e)
    {
        var data = JsonConvert.DeserializeObject<SimulationMessage>(e.Data);
        UpdateScene(data);
    }
}
```

#### Vehicle Controller
```csharp
public class VoxelVehicle : MonoBehaviour
{
    public int vehicleId;
    public VehicleType type;
    public float speed;
    public int passengers;
    
    // Smooth movement
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    
    void Update()
    {
        transform.position = Vector3.Lerp(
            transform.position, 
            targetPosition, 
            Time.deltaTime * 5f
        );
        
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * 5f
        );
    }
    
    public void UpdateFromSimulation(VehicleData data)
    {
        targetPosition = new Vector3(data.position.x, data.position.y, data.position.z);
        targetRotation = Quaternion.Euler(0, data.rotation.y, 0);
        speed = data.speed;
        passengers = data.passengers;
    }
}
```

## Performance Optimization

### Voxel LOD System
- **LOD 0** (Close): Full voxel detail
- **LOD 1** (Medium): Simplified voxel mesh (50% reduction)
- **LOD 2** (Far): Bounding box representation
- **Culling**: Frustum and occlusion culling for off-screen objects

### Batching Strategy
```csharp
// Instanced rendering for repeated voxel elements
Material voxelMaterial;
Mesh voxelMesh;
Matrix4x4[] instanceMatrices;

Graphics.DrawMeshInstanced(
    voxelMesh, 
    0, 
    voxelMaterial, 
    instanceMatrices
);
```

## Development Roadmap

### Phase 1: Foundation (Current)
- [x] Python simulation backend
- [ ] Unity project setup
- [ ] Basic voxel asset pipeline
- [ ] WebSocket communication

### Phase 2: Core Visualization
- [ ] Era 1 building voxel models
- [ ] Train voxel models
- [ ] **BRT biarticulado voxel model**
- [ ] Camera system and controls
- [ ] Basic lighting

### Phase 3: Advanced Features
- [ ] All 4 eras complete
- [ ] Weather effects (rain, fog)
- [ ] Day/night cycle
- [ ] Particle effects (smoke, steam)
- [ ] Sound integration

### Phase 4: Polish
- [ ] UI/HUD overlay
- [ ] Time-lapse mode
- [ ] Screenshot/video export
- [ ] AR Foundation integration (optional)

## Tools and Dependencies

### Unity Packages
```json
{
    "dependencies": {
        "com.unity.render-pipelines.universal": "12.1.0",
        "com.unity.textmeshpro": "3.0.6",
        "com.unity.cinemachine": "2.8.9",
        "com.unity.probuilder": "5.0.6",
        "com.websocket-sharp": "1.0.0",
        "newtonsoft-json": "3.0.2"
    }
}
```

### External Tools
- **MagicaVoxel** 0.99.6+ - Voxel modeling
- **Blender** (optional) - Complex mesh conversion
- **Substance Painter** (optional) - Texture authoring
- **Audacity** - Sound effects editing

## File Organization

```
UnityProject/
├── Assets/
│   ├── Models/
│   │   ├── Buildings/
│   │   │   ├── Era1/
│   │   │   ├── Era2/
│   │   │   ├── Era3/
│   │   │   └── Era4/
│   │   └── Vehicles/
│   │       ├── Trains/
│   │       ├── Buses/
│   │       ├── BRT/          # BRT biarticulado models
│   │       ├── Trams/
│   │       └── Taxis/
│   ├── Materials/
│   │   ├── VoxelShaders/
│   │   └── Textures/
│   ├── Scripts/
│   │   ├── Simulation/
│   │   │   ├── SimulationConnector.cs
│   │   │   ├── VehicleController.cs
│   │   │   └── BuildingController.cs
│   │   └── Camera/
│   ├── Prefabs/
│   └── Scenes/
├── ProjectSettings/
└── Packages/
```

## Next Steps

1. Set up Unity project with URP
2. Create MagicaVoxel templates for each era
3. Implement WebSocket bridge (Python ↔ Unity)
4. Design BRT biarticulado voxel model based on Curitiba reference
5. Test real-time vehicle movement synchronization
6. Document asset creation workflow for contributors

## References

- Unity URP Documentation: https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@latest
- MagicaVoxel: https://ephtracy.github.io/
- Curitiba BRT System: Reference photos for biarticulado design
- Historical São Paulo: Photo archives for era-accurate modeling
