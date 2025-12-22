# 🏗️ Arquitetura do Ferritine

Documentação sobre a arquitetura e planejamento do projeto.

## 📋 Documentos Principais

### Design e Conceito

- **[GDD_FERRITINE.md](GDD_FERRITINE.md)** - Game Design Document completo
  - Visão geral do projeto
  - Mecânicas e sistemas
  - Roadmap completo

- **[MAQUETE_TECH_DOCS.md](MAQUETE_TECH_DOCS.md)** - Documentação técnica da maquete física
  - Especificações de hardware
  - Integração físico-digital

### Planejamento

- **[PLANNING_INDEX.md](PLANNING_INDEX.md)** - Índice de documentos de planejamento
- **[PLANNING_STRUCTURE.md](PLANNING_STRUCTURE.md)** - Estrutura do planejamento
- **[ISSUES_MILESTONES_TAGS.md](ISSUES_MILESTONES_TAGS.md)** - Issues, milestones e sistema de tags

## 🎯 Visão Geral da Arquitetura

O Ferritine é um sistema híbrido que combina:

```
┌─────────────────────────────────────────┐
│     Camada de Apresentação              │
│  (Web UI, Unity, Dashboard)             │
└─────────────────────────────────────────┘
                  ↓
┌─────────────────────────────────────────┐
│     Camada de API                        │
│  (REST API, WebSocket, MQTT)            │
└─────────────────────────────────────────┘
                  ↓
┌─────────────────────────────────────────┐
│     Camada de Lógica                     │
│  (Motor de Simulação, IA, Economia)     │
└─────────────────────────────────────────┘
                  ↓
┌─────────────────────────────────────────┐
│     Camada de Dados                      │
│  (PostgreSQL/SQLite, Logs, Config)      │
└─────────────────────────────────────────┘
                  ↓
┌─────────────────────────────────────────┐
│     Camada de Hardware                   │
│  (Arduino, ESP32, Maquete Física)       │
└─────────────────────────────────────────┘
```

## 📚 Para Mais Informações

Consulte o [GDD_FERRITINE.md](GDD_FERRITINE.md) para documentação completa sobre:
- Fases de desenvolvimento
- Sistemas e mecânicas
- Integração hardware-software
- Roadmap detalhado

---

[⬅️ Voltar ao índice principal](../README.md)
