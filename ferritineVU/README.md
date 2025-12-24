# 🎮 FerritineVU - Unity Project

Este é o projeto Unity do Ferritine, uma visualização 3D interativa da simulação de cidade.

## 📖 Documentação

Toda a documentação do Unity foi movida para melhor organização. Consulte:

**[📚 Documentação Unity Completa](../docs/unity/)**

### Guias Principais

- [Guia de Integração Unity](../docs/unity/UNITY_INTEGRATION_GUIDE.md) - Setup e integração completa
- [Sistema de Animações](../docs/unity/AGENT_ANIMATION_IMPLEMENTATION.md) - Animações de agentes
- [API Endpoints](../docs/unity/API_ENDPOINTS.md) - Endpoints disponíveis
- [Scripts README](../docs/unity/SCRIPTS_README.md) - Documentação dos scripts

## 🚀 Quick Start

1. **Inicie o backend**:
   ```bash
   cd ..
   python main.py
   ```

2. **Abra o projeto no Unity**:
   - Unity 2021.3 LTS ou superior
   - Abra a pasta `ferritineVU` no Unity Hub

3. **Configure a conexão**:
   - Endpoint: `http://localhost:5000`
   - Configuração em `Assets/Scripts/API/`

4. **Execute a cena principal**:
   - Abra `Assets/Scenes/MainScene.unity`
   - Pressione Play

## 📁 Estrutura do Projeto

```
ferritineVU/
├── Assets/
│   ├── Scenes/          # Cenas Unity
│   ├── Scripts/         # Scripts C#
│   │   ├── API/         # Comunicação com backend
│   │   ├── Controllers/ # Controllers
│   │   ├── Managers/    # Managers
│   │   └── Models/      # Modelos de dados
│   ├── Prefabs/         # Prefabs
│   ├── Materials/       # Materiais
│   └── Resources/       # Recursos
├── Packages/            # Packages Unity
└── ProjectSettings/     # Configurações do projeto
```

## 🔌 Conexão com Backend

O projeto Unity se comunica com o backend Python através de uma API REST:

- **Base URL**: `http://localhost:5000`
- **Estado do mundo**: `GET /api/world/state`
- **Agentes**: `GET /api/agents`
- **Veículos**: `GET /api/vehicles`
- **Estações**: `GET /api/stations`

Consulte [API_ENDPOINTS.md](../docs/unity/API_ENDPOINTS.md) para detalhes.

## 🎯 Funcionalidades

- ✅ Visualização 3D da cidade
- ✅ Agentes animados com rotinas
- ✅ Sistema de seleção de entidades
- ✅ UI Manager centralizado
- ✅ Comunicação em tempo real com backend
- ✅ Sistema de câmera interativa

## 🐛 Solução de Problemas

**Backend não conecta?**
- Verifique se o backend está rodando
- Confirme o endpoint em `Assets/Scripts/API/`
- Veja logs no Console do Unity

**Erros de compilação?**
- Reimporte todos os assets
- Verifique a versão do Unity
- Limpe o cache do projeto

## 📚 Documentação Adicional

Para mais informações, consulte a [documentação completa do Unity](../docs/unity/).

---

**Versão Unity**: 2021.3 LTS ou superior  
**Plataforma**: Windows, macOS, Linux
