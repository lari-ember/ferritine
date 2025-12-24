# 🎮 Documentação Unity

Documentação completa para integração e desenvolvimento Unity do Ferritine.

## 📚 Guias de Integração

### Guias Principais

- **[UNITY_INTEGRATION_GUIDE.md](UNITY_INTEGRATION_GUIDE.md)** - 📖 Guia completo de integração
  - Setup inicial
  - Comunicação com backend
  - Código C# pronto para uso
  
- **[UNITY_VOXEL_INTEGRATION.md](UNITY_VOXEL_INTEGRATION.md)** - Integração com sistema voxel
- **[SCRIPTS_README.md](SCRIPTS_README.md)** - Documentação dos scripts Unity
- **[API_ENDPOINTS.md](API_ENDPOINTS.md)** - Endpoints da API para Unity

## 🎨 Funcionalidades

### Sistema de Animações

- **[AGENT_ANIMATION_IMPLEMENTATION.md](AGENT_ANIMATION_IMPLEMENTATION.md)** - Implementação completa
- **[CHECKLIST_AGENT_ANIMATION.md](CHECKLIST_AGENT_ANIMATION.md)** - Checklist de implementação
- **[QUICK_SETUP_AGENT_ANIMATION.md](QUICK_SETUP_AGENT_ANIMATION.md)** - Setup rápido
- **[README_AGENT_ANIMATION.md](README_AGENT_ANIMATION.md)** - README de animações

### Sistema de Seleção e UI

- **[FINAL_SELECTION_TEST.md](FINAL_SELECTION_TEST.md)** - Testes do sistema de seleção
- **[UI_MANAGER_CENTRALIZATION.md](UI_MANAGER_CENTRALIZATION.md)** - Gerenciamento centralizado de UI

## 🚀 Quick Start

1. **Backend rodando**:
   ```bash
   python main.py
   ```

2. **No Unity**:
   - Abra o projeto em `ferritineVU/`
   - Configure o endpoint da API em `http://localhost:5000`
   - Execute a cena principal

3. **Testar conexão**:
   ```
   GET http://localhost:5000/api/world/state
   ```

## 🔌 Endpoints Principais

- `GET /api/world/state` - Estado completo do mundo
- `GET /api/stations` - Lista de estações
- `GET /api/vehicles` - Lista de veículos
- `GET /api/agents` - Lista de agentes
- `GET /api/metrics` - Métricas da simulação

## 📖 Estrutura do Projeto Unity

```
ferritineVU/
├── Assets/
│   ├── Scripts/
│   │   ├── API/           # Comunicação com backend
│   │   ├── Controllers/   # Controllers de gameplay
│   │   ├── Managers/      # Managers de sistemas
│   │   └── Models/        # Modelos de dados
│   ├── Scenes/            # Cenas Unity
│   └── Prefabs/           # Prefabs
```

## 💡 Dicas

- Mantenha o backend rodando em um terminal separado
- Use o modo Play do Unity para testar em tempo real
- Consulte os logs para debug de comunicação
- Veja exemplos de código nos guias

---

[⬅️ Voltar ao índice principal](../README.md)
