# 📋 Issues, Milestones e Tags - Ferritine

Este documento detalha todas as issues, milestones e tags que devem ser criadas no projeto Ferritine, baseadas no GDD (Game Design Document) e no estado atual do projeto.

## 📊 Visão Geral

O projeto Ferritine está dividido em **5 fases principais**:
- **Fase 0**: Fundamentos (Mês 1-2) - Aprendizado básico
- **Fase 1**: Simulação Digital Básica (Mês 3-4) - Motor de simulação
- **Fase 2**: Hardware Básico (Mês 5-7) - Primeiros componentes físicos
- **Fase 3**: Maquete Física Inicial (Mês 8-12) - Maquete 1m² funcional
- **Fase 4**: Expansão e Refinamento (Ano 2+) - Expansões futuras

---

## 🏷️ TAGS/LABELS

### Por Tipo
- `feat` - Nova funcionalidade
- `bug` - Correção de bug
- `docs` - Documentação
- `test` - Testes
- `refactor` - Refatoração
- `chore` - Manutenção/tarefas auxiliares
- `hardware` - Relacionado a hardware/eletrônica
- `simulation` - Sistema de simulação
- `iot` - Internet das Coisas

### Por Prioridade
- `priority: critical` - Crítico, bloqueia outras funcionalidades
- `priority: high` - Alta prioridade
- `priority: medium` - Prioridade média
- `priority: low` - Baixa prioridade

### Por Fase
- `phase-0: fundamentals` - Fase 0: Fundamentos
- `phase-1: digital` - Fase 1: Simulação Digital
- `phase-2: basic-hardware` - Fase 2: Hardware Básico
- `phase-3: physical-model` - Fase 3: Maquete Física
- `phase-4: expansion` - Fase 4: Expansão

### Por Área
- `area: agents` - Sistema de agentes
- `area: economy` - Sistema econômico
- `area: transport` - Sistema de transporte
- `area: politics` - Sistema político
- `area: construction` - Sistema de construção
- `area: ui` - Interface de usuário
- `area: database` - Banco de dados
- `area: api` - API REST/WebSocket

### Por Complexidade
- `complexity: beginner` - Fácil, bom para iniciantes
- `complexity: intermediate` - Complexidade média
- `complexity: advanced` - Avançado, requer experiência

### Outros
- `good first issue` - Boa primeira issue para novos contribuidores
- `help wanted` - Precisa de ajuda da comunidade
- `blocked` - Bloqueada por outra issue
- `wip` - Work in Progress (em andamento)
- `research` - Pesquisa necessária

---

## 🎯 MILESTONES

### Milestone 0: Fundamentos e Infraestrutura
**Descrição**: Estabelecer fundamentos do projeto, documentação básica e aprendizado inicial  
**Data Prevista**: 2 meses  
**Issues**: 15-20 issues  

### Milestone 1.1: Mundo Estático
**Descrição**: Criar estrutura básica da cidade com grid 2D, edifícios e ruas  
**Data Prevista**: 2 semanas  
**Issues**: 8-10 issues  

### Milestone 1.2: Agentes Simples
**Descrição**: Implementar agentes com rotinas básicas e atributos  
**Data Prevista**: 2 semanas  
**Issues**: 6-8 issues  

### Milestone 1.3: Economia Básica
**Descrição**: Sistema de salários, gastos e produção simples  
**Data Prevista**: 2 semanas  
**Issues**: 5-7 issues  

### Milestone 1.4: Transporte Ferroviário Virtual
**Descrição**: Trens virtuais funcionando na simulação  
**Data Prevista**: 2 semanas  
**Issues**: 6-8 issues  

### Milestone 2.1: Circuito de Iluminação
**Descrição**: Arduino controlando LEDs via Python  
**Data Prevista**: 3 semanas  
**Issues**: 5-7 issues  

### Milestone 2.2: Sensor de Trem
**Descrição**: Detecção de trem com reed switch  
**Data Prevista**: 2 semanas  
**Issues**: 4-6 issues  

### Milestone 2.3: Controle de Desvio
**Descrição**: Servomotor controlando desvios de trilho  
**Data Prevista**: 2 semanas  
**Issues**: 4-6 issues  

### Milestone 3.1: Base e Topografia
**Descrição**: Construir base física MDF 100x100cm com relevo  
**Data Prevista**: 4 semanas  
**Issues**: 6-8 issues  

### Milestone 3.2: Trilhos e Primeiro Trem
**Descrição**: Sistema ferroviário físico funcionando  
**Data Prevista**: 4 semanas  
**Issues**: 7-9 issues  

### Milestone 3.3: Primeiros Edifícios
**Descrição**: 3-5 prédios construídos e instalados  
**Data Prevista**: 4 semanas  
**Issues**: 5-7 issues  

### Milestone 3.4: Integração Física-Digital
**Descrição**: Sincronização completa entre maquete física e simulação  
**Data Prevista**: 4 semanas  
**Issues**: 8-10 issues  

---

## 📝 ISSUES DETALHADAS

## FASE 0: FUNDAMENTOS E INFRAESTRUTURA

### Issue #1: Configurar estrutura de projeto Python
**Labels**: `feat`, `phase-0: fundamentals`, `priority: critical`, `complexity: beginner`  
**Milestone**: Milestone 0: Fundamentos e Infraestrutura  

**Descrição**:
Reorganizar estrutura do projeto seguindo arquitetura definida no GDD.

**Tarefas**:
- [ ] Criar diretórios: `backend/`, `frontend/`, `visualization/`, `hardware/`, `data/`, `docs/`, `tests/`
- [ ] Mover código existente (`app/`) para `backend/simulation/`
- [ ] Criar arquivos `__init__.py` em todos os pacotes
- [ ] Atualizar imports em todos os arquivos
- [ ] Criar `backend/config.py` para configurações centralizadas
- [ ] Atualizar `requirements.txt` com dependências organizadas por categoria
- [ ] Atualizar documentação com nova estrutura

**Critérios de Aceitação**:
- Estrutura de diretórios segue o GDD
- Todos os testes existentes continuam passando
- Imports funcionam corretamente
- README.md atualizado com nova estrutura

---

### Issue #2: Configurar sistema de logging
**Labels**: `feat`, `phase-0: fundamentals`, `priority: high`, `complexity: beginner`  
**Milestone**: Milestone 0: Fundamentos e Infraestrutura  

**Descrição**:
Implementar sistema de logging profissional para debug e monitoramento.

**Tarefas**:
- [ ] Criar `backend/utils/logger.py`
- [ ] Configurar diferentes níveis de log (DEBUG, INFO, WARNING, ERROR)
- [ ] Implementar rotação de logs
- [ ] Adicionar logs em módulos críticos (simulação, economia, transporte)
- [ ] Criar arquivo de configuração para logs
- [ ] Documentar como usar o sistema de logging

**Critérios de Aceitação**:
- Sistema de logging funcional
- Logs salvos em `data/logs/`
- Diferentes níveis de log configuráveis
- Rotação automática de arquivos de log

---

### Issue #3: Implementar sistema de configuração com YAML
**Labels**: `feat`, `phase-0: fundamentals`, `priority: high`, `complexity: intermediate`  
**Milestone**: Milestone 0: Fundamentos e Infraestrutura  

**Descrição**:
Criar sistema de configuração centralizado usando arquivos YAML.

**Tarefas**:
- [ ] Criar `data/config.yaml` com configurações padrão
- [ ] Implementar `backend/utils/config_loader.py`
- [ ] Adicionar validação de configurações
- [ ] Criar exemplo de arquivo de configuração (`config.example.yaml`)
- [ ] Documentar todas as opções de configuração
- [ ] Atualizar código para usar configurações centralizadas

**Configurações a incluir**:
- Parâmetros de simulação (velocidade, tempo inicial)
- Configurações de banco de dados
- Configurações de IoT (porta serial, broker MQTT)
- Parâmetros econômicos (salários, preços)
- Configurações de visualização

---

### Issue #4: Configurar banco de dados SQLite
**Labels**: `feat`, `phase-0: fundamentals`, `priority: critical`, `area: database`, `complexity: intermediate`  
**Milestone**: Milestone 0: Fundamentos e Infraestrutura  

**Descrição**:
Implementar persistência de dados usando SQLite e SQLAlchemy.

**Tarefas**:
- [ ] Instalar SQLAlchemy (`pip install sqlalchemy`)
- [ ] Criar `backend/database/models.py` com modelos:
  - Agente (Agent)
  - Edifício (Building)
  - Veículo (Vehicle)
  - Evento (Event)
  - Estatística Econômica (EconomicStat)
- [ ] Implementar `backend/database/queries.py` com consultas comuns
- [ ] Criar sistema de migrations (Alembic)
- [ ] Implementar funções de save/load da simulação
- [ ] Adicionar testes para modelos de banco de dados

**Critérios de Aceitação**:
- Banco de dados criado em `data/city.db`
- Modelos SQLAlchemy funcionais
- Operações CRUD funcionando
- Migrations configuradas

---

### Issue #5: Criar documentação técnica de arquitetura
**Labels**: `docs`, `phase-0: fundamentals`, `priority: medium`, `complexity: beginner`  
**Milestone**: Milestone 0: Fundamentos e Infraestrutura  

**Descrição**:
Documentar a arquitetura do sistema detalhadamente.

**Tarefas**:
- [ ] Criar `docs/architecture.md`
- [ ] Documentar diagrama de componentes
- [ ] Explicar cada camada (apresentação, lógica, dados, hardware)
- [ ] Documentar fluxo de dados
- [ ] Adicionar diagramas (usar Mermaid ou PlantUML)
- [ ] Documentar padrões de design utilizados

---

### Issue #6: Configurar ambiente de desenvolvimento
**Labels**: `docs`, `chore`, `phase-0: fundamentals`, `priority: high`, `complexity: beginner`, `good first issue`  
**Milestone**: Milestone 0: Fundamentos e Infraestrutura  

**Descrição**:
Facilitar configuração do ambiente para novos desenvolvedores.

**Tarefas**:
- [ ] Criar `docs/setup_guide.md`
- [ ] Documentar instalação de Python e dependências
- [ ] Criar script de setup automático (`scripts/setup.sh` / `setup.bat`)
- [ ] Configurar pre-commit hooks
- [ ] Adicionar linter (flake8 ou pylint)
- [ ] Configurar formatador de código (black)
- [ ] Atualizar `.gitignore`

---

### Issue #7: Implementar testes de integração
**Labels**: `test`, `phase-0: fundamentals`, `priority: medium`, `complexity: intermediate`  
**Milestone**: Milestone 0: Fundamentos e Infraestrutura  

**Descrição**:
Expandir cobertura de testes além dos testes unitários existentes.

**Tarefas**:
- [ ] Criar `tests/integration/` para testes de integração
- [ ] Adicionar testes de integração para simulação completa
- [ ] Configurar pytest-cov para cobertura de código
- [ ] Criar CI para rodar testes automaticamente (GitHub Actions)
- [ ] Adicionar badge de cobertura no README
- [ ] Meta: atingir 70%+ de cobertura

---

### Issue #8: Criar guia de aprendizado de eletrônica
**Labels**: `docs`, `phase-0: fundamentals`, `priority: medium`, `complexity: beginner`, `good first issue`  
**Milestone**: Milestone 0: Fundamentos e Infraestrutura  

**Descrição**:
Documentar currículo de eletrônica conforme especificado no GDD.

**Tarefas**:
- [ ] Criar `docs/learning/electronics_curriculum.md`
- [ ] Documentar Semana 1-2: Fundamentos (Lei de Ohm, componentes básicos)
- [ ] Documentar Semana 3-4: Componentes (resistores, capacitores, LEDs)
- [ ] Documentar Semana 5-6: Arduino (programação básica)
- [ ] Documentar Semana 7-8: Sensores e Atuadores
- [ ] Adicionar links para recursos (vídeos, tutoriais)
- [ ] Criar projetos práticos para cada semana

---

### Issue #9: Criar guia de aprendizado de IoT
**Labels**: `docs`, `phase-0: fundamentals`, `iot`, `priority: medium`, `complexity: beginner`  
**Milestone**: Milestone 0: Fundamentos e Infraestrutura  

**Descrição**:
Documentar currículo de IoT conforme especificado no GDD.

**Tarefas**:
- [ ] Criar `docs/learning/iot_curriculum.md`
- [ ] Documentar Semana 1-2: Comunicação Serial (pyserial)
- [ ] Documentar Semana 3-4: ESP32 e WiFi
- [ ] Documentar Semana 5-6: MQTT (Mosquitto, broker)
- [ ] Adicionar exemplos práticos
- [ ] Criar projeto final: Sensor de temperatura com gráfico real-time

---

### Issue #10: Criar guia de aprendizado de simulação
**Labels**: `docs`, `phase-0: fundamentals`, `simulation`, `priority: medium`, `complexity: intermediate`  
**Milestone**: Milestone 0: Fundamentos e Infraestrutura  

**Descrição**:
Documentar currículo de simulação e programação conforme GDD.

**Tarefas**:
- [ ] Criar `docs/learning/simulation_curriculum.md`
- [ ] Documentar Semana 1-2: POO em Python
- [ ] Documentar Semana 3-4: Estruturas de Dados (grafos, filas)
- [ ] Documentar Semana 5-6: Simulação de Eventos (simpy)
- [ ] Documentar Semana 7-8: Agentes Inteligentes
- [ ] Documentar Semana 9-10: Economia Simulada
- [ ] Documentar Semana 11-12: Integração completa

---

## FASE 1: SIMULAÇÃO DIGITAL BÁSICA

### Issue #11: [Milestone 1.1] Implementar classe World/Cidade expandida
**Labels**: `feat`, `phase-1: digital`, `simulation`, `priority: critical`, `area: world`, `complexity: intermediate`  
**Milestone**: Milestone 1.1: Mundo Estático  

**Descrição**:
Expandir classe Cidade existente com grid 2D e funcionalidades avançadas.

**Tarefas**:
- [ ] Renomear e mover `app/models/cidade.py` para `backend/simulation/world.py`
- [ ] Implementar grid 2D (matriz de células)
- [ ] Adicionar sistema de coordenadas (x, y)
- [ ] Implementar mapa de ocupação (quais células têm edifícios, ruas, trilhos)
- [ ] Criar método `get_neighbors(x, y)` para navegação
- [ ] Adicionar propriedades: tamanho, nome da cidade, data de fundação
- [ ] Implementar método `save_to_db()` e `load_from_db()`
- [ ] Adicionar testes unitários

**Critérios de Aceitação**:
- Grid 2D funcional (mínimo 50x50)
- Sistema de coordenadas implementado
- Persistência em banco de dados
- Testes passando

---

### Issue #12: [Milestone 1.1] Implementar classe Building (Edifício)
**Labels**: `feat`, `phase-1: digital`, `simulation`, `priority: critical`, `area: construction`, `complexity: intermediate`  
**Milestone**: Milestone 1.1: Mundo Estático  

**Descrição**:
Criar sistema de edifícios com diferentes tipos e funcionalidades.

**Tarefas**:
- [ ] Criar `backend/simulation/building.py`
- [ ] Implementar classe base `Building` com atributos:
  - ID, nome, tipo, posição (x, y)
  - Tamanho (largura, altura)
  - Proprietário (agente ou governo)
  - Capacidade (pessoas ou carga)
- [ ] Implementar subclasses:
  - `ResidentialBuilding` (casa, apartamento)
  - `CommercialBuilding` (loja, restaurante)
  - `IndustrialBuilding` (fábrica, armazém)
  - `PublicBuilding` (estação, escola, hospital)
- [ ] Adicionar propriedades específicas por tipo
- [ ] Implementar método `can_accommodate(agent)` para residências
- [ ] Adicionar persistência em BD
- [ ] Criar testes

**Critérios de Aceitação**:
- Tipos de edifício implementados
- Edifícios podem ser colocados no grid
- Propriedades específicas funcionando
- Testes passando

---

### Issue #13: [Milestone 1.1] Implementar sistema de ruas e trilhos
**Labels**: `feat`, `phase-1: digital`, `simulation`, `priority: high`, `area: transport`, `complexity: intermediate`  
**Milestone**: Milestone 1.1: Mundo Estático  

**Descrição**:
Criar infraestrutura de transporte (ruas e trilhos) no grid.

**Tarefas**:
- [ ] Criar `backend/simulation/infrastructure.py`
- [ ] Implementar classe `Road` (rua)
- [ ] Implementar classe `Rail` (trilho)
- [ ] Adicionar sistema de conexões entre células
- [ ] Implementar validação de caminho (pathfinding básico)
- [ ] Criar método `add_road(start, end)` e `add_rail(start, end)`
- [ ] Visualizar ruas e trilhos no grid
- [ ] Adicionar testes

---

### Issue #14: [Milestone 1.1] Criar visualização 2D com Pygame
**Labels**: `feat`, `phase-1: digital`, `ui`, `priority: high`, `complexity: intermediate`  
**Milestone**: Milestone 1.1: Mundo Estático  

**Descrição**:
Implementar renderização 2D da cidade usando Pygame.

**Tarefas**:
- [ ] Instalar Pygame (`pip install pygame`)
- [ ] Criar `visualization/pygame_renderer.py`
- [ ] Implementar janela básica 800x600
- [ ] Renderizar grid da cidade
- [ ] Renderizar edifícios (retângulos coloridos por tipo)
- [ ] Renderizar ruas (linhas cinzas)
- [ ] Renderizar trilhos (linhas marrons/pretas)
- [ ] Adicionar zoom e pan (arrastar mouse)
- [ ] Adicionar legenda de cores
- [ ] Criar loop de renderização

**Critérios de Aceitação**:
- Janela Pygame abre
- Cidade visível em 2D
- Zoom e pan funcionando
- 30+ FPS

---

### Issue #15: [Milestone 1.2] Expandir classe Agent com atributos detalhados
**Labels**: `feat`, `phase-1: digital`, `simulation`, `priority: critical`, `area: agents`, `complexity: intermediate`  
**Milestone**: Milestone 1.2: Agentes Simples  

**Descrição**:
Expandir agentes existentes com atributos e comportamentos do GDD.

**Tarefas**:
- [ ] Mover `app/models/agente.py` para `backend/simulation/agent.py`
- [ ] Adicionar atributos físicos/mentais:
  - Saúde (health): 0-100
  - Energia (energy): 0-100
  - Felicidade (happiness): 0-100
  - Fome (hunger): 0-100
  - Conhecimento (knowledge): 0-100
  - Força (strength): 0-100
  - Atenção (attention): 0-100
- [ ] Adicionar traços de personalidade:
  - Preguiça (laziness): 0-100
  - Ambição (ambition): 0-100
- [ ] Implementar método `update_needs(time_delta)` para atualizar necessidades
- [ ] Adicionar família: lista de IDs de familiares
- [ ] Implementar histórico de eventos
- [ ] Adicionar persistência em BD
- [ ] Criar testes

---

### Issue #16: [Milestone 1.2] Implementar máquina de estados para agentes
**Labels**: `feat`, `phase-1: digital`, `simulation`, `priority: high`, `area: agents`, `complexity: advanced`  
**Milestone**: Milestone 1.2: Agentes Simples  

**Descrição**:
Criar sistema de estados para rotinas de agentes.

**Tarefas**:
- [ ] Criar enum `AgentState` (SLEEPING, AT_HOME, COMMUTING, WORKING, LEISURE, SHOPPING)
- [ ] Implementar método `update(world, current_time)` principal
- [ ] Implementar transições de estado baseadas em horário
- [ ] Implementar método `_decide_activity()` para tomada de decisão
- [ ] Criar método `_sleep()` - recuperar energia
- [ ] Criar método `_work()` - trabalhar e ganhar dinheiro
- [ ] Criar método `_commute()` - deslocar-se
- [ ] Criar método `_leisure()` - atividades de lazer
- [ ] Criar método `_shop()` - comprar itens
- [ ] Criar método `_eat()` - reduzir fome
- [ ] Adicionar testes para cada estado

**Critérios de Aceitação**:
- Estados implementados
- Transições funcionando corretamente
- Agentes seguem rotina diária realista
- Testes passando

---

### Issue #17: [Milestone 1.2] Implementar rotinas diárias dinâmicas
**Labels**: `feat`, `phase-1: digital`, `simulation`, `priority: high`, `area: agents`, `complexity: intermediate`  
**Milestone**: Milestone 1.2: Agentes Simples  

**Descrição**:
Agentes seguem rotinas baseadas em horários e necessidades.

**Tarefas**:
- [ ] Implementar rotina de dia típico (GDD):
  - 06:00 - Acordar
  - 07:00 - Deslocamento para trabalho
  - 08:00-12:00 - Trabalho
  - 12:00-13:00 - Almoço
  - 13:00-17:00 - Trabalho
  - 17:00-19:00 - Lazer opcional
  - 19:00 - Retorno para casa
  - 22:00 - Dormir
- [ ] Adicionar variações para finais de semana
- [ ] Implementar decisões baseadas em necessidades (fome > trabalho)
- [ ] Adicionar chance de faltar trabalho (preguiça)
- [ ] Criar visualização de estado dos agentes

---

### Issue #18: [Milestone 1.2] Visualizar agentes no mapa Pygame
**Labels**: `feat`, `phase-1: digital`, `ui`, `priority: medium`, `complexity: beginner`  
**Milestone**: Milestone 1.2: Agentes Simples  

**Descrição**:
Renderizar agentes como pontos ou sprites no mapa 2D.

**Tarefas**:
- [ ] Adicionar renderização de agentes (círculos coloridos)
- [ ] Cores diferentes por estado (dormindo=azul, trabalhando=verde, etc.)
- [ ] Mostrar tooltip ao passar mouse sobre agente (nome, estado, local)
- [ ] Adicionar opção de mostrar/ocultar agentes
- [ ] Implementar filtros (mostrar só agentes trabalhando, etc.)

---

### Issue #19: [Milestone 1.3] Implementar sistema de economia básico
**Labels**: `feat`, `phase-1: digital`, `simulation`, `priority: critical`, `area: economy`, `complexity: advanced`  
**Milestone**: Milestone 1.3: Economia Básica  

**Descrição**:
Criar sistema econômico com salários, gastos e produção.

**Tarefas**:
- [ ] Criar `backend/simulation/economy.py`
- [ ] Implementar classe `Economy` com:
  - PIB (soma de produção)
  - Taxa de desemprego
  - Inflação
  - Preços de bens (alimentos, roupas, etc.)
- [ ] Implementar mercado de trabalho:
  - Agentes procuram emprego
  - Empresas contratam
- [ ] Implementar salários:
  - Agentes recebem salário mensal
  - Salário varia por tipo de trabalho
- [ ] Implementar gastos:
  - Agentes gastam em comida, aluguel
  - Calcular custo de vida
- [ ] Adicionar estatísticas econômicas ao BD
- [ ] Criar testes

---

### Issue #20: [Milestone 1.3] Implementar cadeia produtiva básica
**Labels**: `feat`, `phase-1: digital`, `simulation`, `priority: high`, `area: economy`, `complexity: advanced`  
**Milestone**: Milestone 1.3: Economia Básica  

**Descrição**:
Criar fluxo de produção conforme GDD (fazenda → moinho → padaria).

**Tarefas**:
- [ ] Criar classe `Good` (bem/mercadoria) com tipos:
  - Matéria-prima: grãos, carvão, madeira
  - Processado: farinha, energia, tábuas
  - Final: pão, roupas, móveis
- [ ] Implementar produção em fábricas:
  - Fábrica consome matéria-prima
  - Fábrica produz bens processados
  - Trabalhadores necessários
- [ ] Implementar armazenamento:
  - Edifícios têm estoque de bens
  - Limites de capacidade
- [ ] Implementar comércio básico:
  - Lojas compram de fábricas
  - Agentes compram de lojas
- [ ] Adicionar transporte de bens (preparação para trens)
- [ ] Criar testes

---

### Issue #21: [Milestone 1.3] Criar dashboard de estatísticas econômicas
**Labels**: `feat`, `phase-1: digital`, `ui`, `priority: medium`, `area: economy`, `complexity: intermediate`  
**Milestone**: Milestone 1.3: Economia Básica  

**Descrição**:
Visualizar KPIs econômicos em tempo real.

**Tarefas**:
- [ ] Instalar Matplotlib (`pip install matplotlib`)
- [ ] Criar painel de estatísticas na janela Pygame
- [ ] Mostrar:
  - PIB atual
  - Taxa de desemprego
  - Inflação
  - População total
  - Felicidade média
- [ ] Criar gráficos de linha para histórico
- [ ] Atualizar em tempo real

---

### Issue #22: [Milestone 1.4] Implementar classe Train (Trem)
**Labels**: `feat`, `phase-1: digital`, `simulation`, `priority: critical`, `area: transport`, `complexity: intermediate`  
**Milestone**: Milestone 1.4: Transporte Ferroviário Virtual  

**Descrição**:
Criar sistema de trens virtuais funcionando na simulação.

**Tarefas**:
- [ ] Criar `backend/simulation/vehicle.py`
- [ ] Implementar classe base `Vehicle`
- [ ] Implementar classe `Train` com:
  - ID, nome, modelo
  - Posição atual (célula do grid)
  - Velocidade
  - Capacidade (passageiros e/ou carga)
  - Rota (lista de estações)
  - Carga atual (lista de bens ou passageiros)
- [ ] Implementar tipos de trem:
  - Maria Fumaça (era 1)
  - Diesel (era 2-3)
  - Elétrico (era 4)
- [ ] Adicionar persistência em BD
- [ ] Criar testes

---

### Issue #23: [Milestone 1.4] Implementar sistema de rotas e horários de trem
**Labels**: `feat`, `phase-1: digital`, `simulation`, `priority: high`, `area: transport`, `complexity: advanced`  
**Milestone**: Milestone 1.4: Transporte Ferroviário Virtual  

**Descrição**:
Trens seguem rotas programadas com horários definidos.

**Tarefas**:
- [ ] Criar classe `Route` com lista de estações
- [ ] Implementar cálculo de distância entre estações
- [ ] Implementar movimento do trem ao longo da rota
- [ ] Adicionar paradas em estações
- [ ] Implementar horário de saída/chegada
- [ ] Criar sistema de atrasos (falhas mecânicas, etc.)
- [ ] Adicionar testes

---

### Issue #24: [Milestone 1.4] Implementar embarque/desembarque de passageiros
**Labels**: `feat`, `phase-1: digital`, `simulation`, `priority: high`, `area: transport`, `area: agents`, `complexity: intermediate`  
**Milestone**: Milestone 1.4: Transporte Ferroviário Virtual  

**Descrição**:
Agentes podem pegar trem para ir ao trabalho.

**Tarefas**:
- [ ] Agentes decidem pegar trem baseado em distância
- [ ] Implementar fila de espera em estações
- [ ] Agentes embarcam quando trem chega
- [ ] Agentes desembarcam na estação de destino
- [ ] Cobrar tarifa de passagem
- [ ] Atualizar estado do agente (COMMUTING)
- [ ] Visualizar agentes dentro do trem
- [ ] Criar testes

---

### Issue #25: [Milestone 1.4] Implementar transporte de carga por trem
**Labels**: `feat`, `phase-1: digital`, `simulation`, `priority: high`, `area: transport`, `area: economy`, `complexity: intermediate`  
**Milestone**: Milestone 1.4: Transporte Ferroviário Virtual  

**Descrição**:
Trens transportam bens entre fábricas e cidades.

**Tarefas**:
- [ ] Vagões de carga têm capacidade (toneladas)
- [ ] Bens podem ser carregados/descarregados em estações
- [ ] Implementar logística:
  - Fazenda → Trem → Cidade
  - Mina → Trem → Fábrica
- [ ] Calcular receita de frete
- [ ] Visualizar carga nos vagões
- [ ] Criar testes

---

### Issue #26: [Milestone 1.4] Visualizar trens no mapa Pygame
**Labels**: `feat`, `phase-1: digital`, `ui`, `priority: medium`, `complexity: beginner`  
**Milestone**: Milestone 1.4: Transporte Ferroviário Virtual  

**Descrição**:
Renderizar trens se movendo nos trilhos.

**Tarefas**:
- [ ] Renderizar trens como retângulos nos trilhos
- [ ] Animar movimento suave entre células
- [ ] Mostrar direção (seta)
- [ ] Tooltip com informações (rota, carga, passageiros)
- [ ] Cores diferentes por tipo de trem
- [ ] Adicionar fumaça/partículas para marias fumaça

---

## FASE 2: HARDWARE BÁSICO

### Issue #27: [Milestone 2.1] Configurar comunicação Serial Python-Arduino
**Labels**: `feat`, `phase-2: basic-hardware`, `iot`, `priority: critical`, `complexity: intermediate`  
**Milestone**: Milestone 2.1: Circuito de Iluminação  

**Descrição**:
Estabelecer comunicação bidirecional entre Python e Arduino.

**Tarefas**:
- [ ] Instalar pyserial (`pip install pyserial`)
- [ ] Criar `backend/iot/serial_handler.py`
- [ ] Implementar classe `SerialConnection`
- [ ] Detectar automaticamente porta COM/USB
- [ ] Implementar envio de comandos (formato JSON ou simples)
- [ ] Implementar leitura de dados do Arduino
- [ ] Adicionar tratamento de erros (desconexão, timeout)
- [ ] Criar testes (mock do serial)

---

### Issue #28: [Milestone 2.1] Criar firmware Arduino para controle de LEDs
**Labels**: `feat`, `phase-2: basic-hardware`, `hardware`, `priority: critical`, `complexity: beginner`  
**Milestone**: Milestone 2.1: Circuito de Iluminação  

**Descrição**:
Programar Arduino para controlar 5-10 LEDs via comandos seriais.

**Tarefas**:
- [ ] Criar `hardware/arduino/lighting_control/lighting_control.ino`
- [ ] Configurar 10 pinos digitais para LEDs
- [ ] Implementar protocolo de comunicação serial
- [ ] Comandos:
  - `LED:1:ON` - Ligar LED 1
  - `LED:1:OFF` - Desligar LED 1
  - `LED:ALL:ON` - Ligar todos
  - `LED:1:PWM:128` - Brilho do LED 1 (PWM)
- [ ] Adicionar resposta de confirmação
- [ ] Documentar esquema de ligação
- [ ] Criar diagrama Fritzing

---

### Issue #29: [Milestone 2.1] Integrar iluminação com simulação (dia/noite)
**Labels**: `feat`, `phase-2: basic-hardware`, `iot`, `simulation`, `priority: high`, `complexity: intermediate`  
**Milestone**: Milestone 2.1: Circuito de Iluminação  

**Descrição**:
LEDs acendem/apagam baseado na hora do dia simulado.

**Tarefas**:
- [ ] Adicionar sistema de tempo na simulação
- [ ] Implementar ciclo dia/noite (0h-23h)
- [ ] LEDs de postes acendem após 18h, apagam às 6h
- [ ] LEDs de prédios acendem se agentes estão em casa
- [ ] Brilho varia (PWM) conforme horário
- [ ] Sincronizar com Arduino via serial
- [ ] Criar testes

---

### Issue #30: [Milestone 2.2] Criar firmware Arduino para sensor de trem
**Labels**: `feat`, `phase-2: basic-hardware`, `hardware`, `priority: critical`, `complexity: beginner`  
**Milestone**: Milestone 2.2: Sensor de Trem  

**Descrição**:
Programar Arduino para detectar trem com reed switch.

**Tarefas**:
- [ ] Criar `hardware/arduino/train_sensor/train_sensor.ino`
- [ ] Configurar pino digital para reed switch (INPUT_PULLUP)
- [ ] Detectar mudança de estado (trem passando)
- [ ] Enviar mensagem via serial: `TRAIN_DETECTED:SENSOR_1`
- [ ] Implementar debounce (evitar múltiplas leituras)
- [ ] Documentar esquema de ligação (reed switch + ímã)
- [ ] Criar diagrama Fritzing

---

### Issue #31: [Milestone 2.2] Integrar sensor de trem com simulação
**Labels**: `feat`, `phase-2: basic-hardware`, `iot`, `simulation`, `priority: high`, `complexity: intermediate`  
**Milestone**: Milestone 2.2: Sensor de Trem  

**Descrição**:
Python recebe dados do sensor e atualiza posição do trem na simulação.

**Tarefas**:
- [ ] Criar listener de eventos serial
- [ ] Ao receber `TRAIN_DETECTED:SENSOR_1`:
  - Atualizar posição do trem na simulação
  - Registrar no log
  - Atualizar visualização
- [ ] Implementar mapeamento sensor → posição no grid
- [ ] Adicionar validação (trem só pode estar em um lugar)
- [ ] Criar testes

---

### Issue #32: [Milestone 2.3] Criar firmware Arduino para controle de servo (desvio)
**Labels**: `feat`, `phase-2: basic-hardware`, `hardware`, `priority: critical`, `complexity: intermediate`  
**Milestone**: Milestone 2.3: Controle de Desvio  

**Descrição**:
Programar Arduino para controlar servomotor que aciona desvio de trilho.

**Tarefas**:
- [ ] Criar `hardware/arduino/switch_control/switch_control.ino`
- [ ] Configurar biblioteca Servo.h
- [ ] Configurar pino PWM para servo
- [ ] Implementar comandos:
  - `SWITCH:1:LEFT` - Desvio para esquerda (ângulo 45°)
  - `SWITCH:1:RIGHT` - Desvio para direita (ângulo 135°)
- [ ] Movimento suave do servo
- [ ] Adicionar confirmação
- [ ] Documentar esquema de ligação
- [ ] Criar diagrama Fritzing

---

### Issue #33: [Milestone 2.3] Integrar controle de desvio com simulação
**Labels**: `feat`, `phase-2: basic-hardware`, `iot`, `simulation`, `priority: high`, `complexity: advanced`  
**Milestone**: Milestone 2.3: Controle de Desvio  

**Descrição**:
Simulação decide rota do trem e Arduino move desvio fisicamente.

**Tarefas**:
- [ ] Implementar lógica de decisão de rota
- [ ] Antes do trem chegar ao desvio:
  - Calcular melhor caminho
  - Enviar comando ao Arduino
  - Aguardar confirmação
- [ ] Atualizar posição do trem conforme desvio
- [ ] Adicionar visualização de desvios no mapa
- [ ] Criar testes

---

### Issue #34: Documentar guia de compras Fase 2
**Labels**: `docs`, `phase-2: basic-hardware`, `hardware`, `priority: medium`, `complexity: beginner`, `good first issue`  
**Milestone**: Milestone 2.1: Circuito de Iluminação  

**Descrição**:
Criar lista detalhada de compras para Fase 2.

**Tarefas**:
- [ ] Criar `docs/hardware/phase2_shopping_list.md`
- [ ] Listar componentes necessários:
  - Arduino Uno + cabo USB
  - LEDs (10 unidades)
  - Resistores 220Ω (10 unidades)
  - Reed switches (3-5 unidades)
  - Ímãs pequenos (3-5 unidades)
  - Servomotor 9g (2 unidades)
  - Jumpers macho-macho (20 unidades)
  - Protoboard 830 pontos
- [ ] Adicionar links de lojas (MercadoLivre, Baú da Eletrônica)
- [ ] Estimar preços
- [ ] Total estimado: ~R$ 300

---

## FASE 3: MAQUETE FÍSICA INICIAL

### Issue #35: [Milestone 3.1] Documentar projeto da base MDF
**Labels**: `docs`, `phase-3: physical-model`, `hardware`, `priority: high`, `complexity: beginner`  
**Milestone**: Milestone 3.1: Base e Topografia  

**Descrição**:
Criar projeto detalhado da base física da maquete.

**Tarefas**:
- [ ] Criar `docs/hardware/base_design.md`
- [ ] Especificar dimensões: 100x100cm, espessura 15mm
- [ ] Desenhar planta baixa da cidade (papel quadriculado)
- [ ] Definir áreas:
  - Centro histórico (estação principal)
  - Distrito industrial
  - Área residencial
  - Zona rural (opcional)
- [ ] Marcar posições de trilhos
- [ ] Marcar posições de ruas
- [ ] Criar lista de materiais para base

---

### Issue #36: [Milestone 3.1] Documentar construção de relevo/topografia
**Labels**: `docs`, `phase-3: physical-model`, `hardware`, `priority: medium`, `complexity: beginner`  
**Milestone**: Milestone 3.1: Base e Topografia  

**Descrição**:
Guia passo a passo para criar relevo da maquete.

**Tarefas**:
- [ ] Criar `docs/hardware/terrain_guide.md`
- [ ] Documentar uso de isopor para elevações
- [ ] Técnicas de escultura (faca quente, lixa)
- [ ] Aplicação de gaze + cola para reforço
- [ ] Pintura de terreno (cores, técnicas)
- [ ] Adicionar fotos/ilustrações de referência
- [ ] Lista de materiais

---

### Issue #37: [Milestone 3.2] Documentar instalação de trilhos DCC
**Labels**: `docs`, `phase-3: physical-model`, `hardware`, `priority: high`, `complexity: intermediate`  
**Milestone**: Milestone 3.2: Trilhos e Primeiro Trem  

**Descrição**:
Guia para instalação do sistema ferroviário físico.

**Tarefas**:
- [ ] Criar `docs/hardware/railway_installation.md`
- [ ] Explicar diferença DCC vs DC
- [ ] Guia de instalação de trilhos:
  - Fixação de leito (EVA/cortiça)
  - Pregagem de trilhos
  - Conexões elétricas
  - Teste de continuidade
- [ ] Instalação de desvios
- [ ] Configuração de fonte DCC
- [ ] Programação de locomotiva DCC
- [ ] Solução de problemas comuns

---

### Issue #38: [Milestone 3.2] Integrar controle DCC com Python
**Labels**: `feat`, `phase-3: physical-model`, `iot`, `priority: high`, `complexity: advanced`  
**Milestone**: Milestone 3.2: Trilhos e Primeiro Trem  

**Descrição**:
Controlar locomotivas DCC via Python (se possível).

**Tarefas**:
- [ ] Pesquisar interfaces DCC (JMRI, DCC-EX)
- [ ] Escolher solução (Arduino DCC Shield ou comercial)
- [ ] Implementar `backend/iot/dcc_controller.py`
- [ ] Comandos básicos:
  - Ligar/desligar locomotiva
  - Controlar velocidade (0-127)
  - Mudar direção (frente/ré)
  - Acionar buzina/luzes
- [ ] Integrar com simulação
- [ ] Documentar setup

**Nota**: Pode ser complexo, pesquisa necessária.

---

### Issue #39: [Milestone 3.3] Criar modelos 3D de edifícios
**Labels**: `feat`, `phase-3: physical-model`, `priority: medium`, `complexity: intermediate`, `help wanted`  
**Milestone**: Milestone 3.3: Primeiros Edifícios  

**Descrição**:
Modelar 3-5 prédios para impressão 3D ou corte a laser.

**Tarefas**:
- [ ] Escolher ferramenta (TinkerCAD, Blender, FreeCAD)
- [ ] Modelar:
  - Estação ferroviária
  - Casa residencial
  - Fábrica
  - Loja/comércio
  - Escola
- [ ] Exportar STL para impressão 3D
- [ ] Exportar SVG/DXF para corte a laser (alternativa)
- [ ] Documentar dimensões (escala HO 1:87)
- [ ] Disponibilizar arquivos no repositório (`models/`)

---

### Issue #40: [Milestone 3.3] Documentar técnicas de construção de prédios
**Labels**: `docs`, `phase-3: physical-model`, `priority: medium`, `complexity: beginner`  
**Milestone**: Milestone 3.3: Primeiros Edifícios  

**Descrição**:
Guiar construção de edifícios com diferentes técnicas.

**Tarefas**:
- [ ] Criar `docs/hardware/building_techniques.md`
- [ ] Técnica 1: Scratch building com papelão
- [ ] Técnica 2: Impressão 3D
- [ ] Técnica 3: Corte a laser em MDF
- [ ] Técnica 4: Kits comerciais
- [ ] Detalhamento (janelas, portas, texturas)
- [ ] Pintura e acabamento
- [ ] Fixação na base

---

### Issue #41: [Milestone 3.4] Criar sistema de sincronização física-digital
**Labels**: `feat`, `phase-3: physical-model`, `iot`, `simulation`, `priority: critical`, `complexity: advanced`  
**Milestone**: Milestone 3.4: Integração Física-Digital  

**Descrição**:
Sincronizar posição física do trem com simulação em tempo real.

**Tarefas**:
- [ ] Múltiplos sensores ao longo da linha (5-10 pontos)
- [ ] Triangular posição do trem entre sensores
- [ ] Atualizar simulação com posição real
- [ ] Se posição simulada ≠ posição real:
  - Ajustar velocidade do trem físico
  - OU ajustar simulação
- [ ] Implementar sincronização bidirecional
- [ ] Adicionar modo "físico" vs "simulado puro"
- [ ] Criar testes

---

### Issue #42: [Milestone 3.4] Criar dashboard web com Flask
**Labels**: `feat`, `phase-3: physical-model`, `ui`, `priority: high`, `area: api`, `complexity: intermediate`  
**Milestone**: Milestone 3.4: Integração Física-Digital  

**Descrição**:
Dashboard web para monitorar maquete remotamente.

**Tarefas**:
- [ ] Instalar Flask (`pip install flask`)
- [ ] Criar `backend/api/routes.py`
- [ ] Endpoints REST:
  - `GET /api/city/status` - Status da cidade
  - `GET /api/agents` - Lista de agentes
  - `GET /api/trains` - Posição dos trens
  - `GET /api/economy/stats` - Estatísticas econômicas
  - `POST /api/train/{id}/speed` - Controlar velocidade
- [ ] Criar `frontend/dashboard/` com HTML/CSS/JS
- [ ] Visualização em tempo real com AJAX
- [ ] Gráficos com Chart.js
- [ ] Controles interativos

---

## FASE 4: EXPANSÃO E REFINAMENTO

### Issue #43: [Fase 4] Implementar sistema de ônibus
**Labels**: `feat`, `phase-4: expansion`, `simulation`, `area: transport`, `priority: medium`, `complexity: intermediate`  

**Descrição**:
Adicionar ônibus como opção de transporte público.

**Tarefas**:
- [ ] Criar classe `Bus` herdando de `Vehicle`
- [ ] Implementar rotas de ônibus
- [ ] Pontos de parada
- [ ] Agentes escolhem entre trem e ônibus
- [ ] Tarifas diferenciadas
- [ ] Visualização no mapa

---

### Issue #44: [Fase 4] Implementar sistema político (eleições)
**Labels**: `feat`, `phase-4: expansion`, `simulation`, `area: politics`, `priority: medium`, `complexity: advanced`  

**Descrição**:
Sistema de eleições e gestão política.

**Tarefas**:
- [ ] Criar `backend/simulation/politics.py`
- [ ] Classe `Mayor` (prefeito)
- [ ] Eleições a cada 4 anos (tempo simulado)
- [ ] Agentes com alta ambição podem candidatar-se
- [ ] Votação baseada em felicidade
- [ ] Prefeito toma decisões (obras, impostos)
- [ ] Eventos políticos (protestos, escândalos)

---

### Issue #45: [Fase 4] Implementar geração de notícias com IA
**Labels**: `feat`, `phase-4: expansion`, `ai`, `priority: low`, `complexity: advanced`, `research`  

**Descrição**:
IA gera manchetes de jornal baseadas em eventos.

**Tarefas**:
- [ ] Pesquisar bibliotecas (transformers, GPT-2)
- [ ] Criar `backend/ai/news_generator.py`
- [ ] Templates de notícias
- [ ] Geração baseada em eventos da cidade
- [ ] Criar jornal "O Trilho" com notícias semanais
- [ ] Interface para ler notícias

---

### Issue #46: [Fase 4] Implementar Realidade Aumentada (AR)
**Labels**: `feat`, `phase-4: expansion`, `ui`, `priority: low`, `complexity: advanced`, `research`  

**Descrição**:
App mobile com AR para visualizar informações sobre a maquete.

**Tarefas**:
- [ ] Escolher framework (AR.js, Unity AR Foundation)
- [ ] Criar marcadores AR (QR codes na maquete)
- [ ] App mobile básico (React Native ou Flutter)
- [ ] Ao apontar câmera, mostrar:
  - Nomes de ruas/prédios
  - Estatísticas de edifícios
  - Agentes virtuais "andando"
- [ ] Documentar configuração

**Nota**: Projeto complexo, pode ser spin-off separado.

---

### Issue #47: [Fase 4] Implementar sistema de eventos aleatórios
**Labels**: `feat`, `phase-4: expansion`, `simulation`, `priority: medium`, `complexity: intermediate`  

**Descrição**:
Eventos emergentes adicionam imprevisibilidade.

**Tarefas**:
- [ ] Criar `backend/simulation/events.py`
- [ ] Classe `Event` com tipos:
  - Clima (chuva, enchente, seca)
  - Social (greve, festival, epidemia)
  - Econômico (boom, recessão)
  - Tecnológico (invenção)
  - Político (eleição surpresa)
- [ ] Probabilidade de eventos baseada em contexto
- [ ] Eventos têm duração e efeitos
- [ ] Registrar no histórico
- [ ] Visualizar eventos ativos

---

### Issue #48: [Fase 4] Implementar sistema de famílias e nascimentos
**Labels**: `feat`, `phase-4: expansion`, `simulation`, `area: agents`, `priority: medium`, `complexity: intermediate`  

**Descrição**:
Agentes formam famílias, casam e têm filhos.

**Tarefas**:
- [ ] Agentes podem casar (forma família)
- [ ] Casais têm filhos (novos agentes)
- [ ] Crianças crescem e entram no mercado de trabalho
- [ ] Herança de atributos dos pais
- [ ] Sobrenomes familiares
- [ ] Árvore genealógica
- [ ] Morte de agentes por idade/doença

---

### Issue #49: [Fase 4] Implementar salvamento/carregamento de cenários
**Labels**: `feat`, `phase-4: expansion`, `priority: high`, `complexity: intermediate`  

**Descrição**:
Salvar e carregar estado completo da simulação.

**Tarefas**:
- [ ] Implementar `world.save_scenario(filename)`
- [ ] Implementar `world.load_scenario(filename)`
- [ ] Salvar em formato JSON ou SQLite
- [ ] Salvar:
  - Estado de todos agentes
  - Edifícios
  - Veículos
  - Economia
  - Histórico
- [ ] Validar integridade ao carregar
- [ ] Interface para gerenciar cenários salvos

---

### Issue #50: [Fase 4] Criar modo de jogo "História" (Campaign)
**Labels**: `feat`, `phase-4: expansion`, `priority: medium`, `complexity: advanced`  

**Descrição**:
Modo com progressão através de eras históricas.

**Tarefas**:
- [ ] Definir capítulos (conforme GDD):
  - Cap 1: Era do Vapor (1860-1920)
  - Cap 2: Industrialização (1920-1960)
  - Cap 3: Modernização (1960-2000)
  - Cap 4: Era Digital (2000+)
- [ ] Objetivos por capítulo
- [ ] Progressão desbloqueável
- [ ] Conquistas (achievements)
- [ ] Salvar progresso

---

## INFRAESTRUTURA E QUALIDADE

### Issue #51: Implementar CI/CD completo
**Labels**: `chore`, `priority: high`, `complexity: intermediate`  

**Descrição**:
Melhorar pipelines de CI/CD.

**Tarefas**:
- [ ] Expandir GitHub Actions para rodar em PRs
- [ ] Adicionar análise de código (SonarCloud)
- [ ] Automatizar deploy de docs (GitHub Pages)
- [ ] Configurar dependabot para atualizações
- [ ] Badge de status no README

---

### Issue #52: Criar documentação de API completa
**Labels**: `docs`, `priority: medium`, `area: api`, `complexity: beginner`  

**Descrição**:
Documentar todos os endpoints da API REST.

**Tarefas**:
- [ ] Criar `docs/api_reference.md`
- [ ] Usar Swagger/OpenAPI
- [ ] Documentar request/response de cada endpoint
- [ ] Exemplos de uso com curl
- [ ] Códigos de erro
- [ ] Autenticação (se implementada)

---

### Issue #53: Melhorar cobertura de testes para 80%+
**Labels**: `test`, `priority: high`, `complexity: intermediate`  

**Descrição**:
Expandir testes para cobrir 80% do código.

**Tarefas**:
- [ ] Adicionar testes para todos os módulos
- [ ] Testes de unidade para classes core
- [ ] Testes de integração para fluxos completos
- [ ] Testes de regressão
- [ ] Configurar pytest-cov
- [ ] Gerar relatórios HTML de cobertura

---

### Issue #54: Criar tutorial interativo para novos usuários
**Labels**: `docs`, `priority: medium`, `complexity: beginner`, `good first issue`  

**Descrição**:
Tutorial passo a passo para primeira experiência.

**Tarefas**:
- [ ] Criar `docs/tutorial.md`
- [ ] Passo 1: Instalação
- [ ] Passo 2: Executar primeira simulação
- [ ] Passo 3: Adicionar agente customizado
- [ ] Passo 4: Ver estatísticas
- [ ] Passo 5: Conectar Arduino (opcional)
- [ ] Capturas de tela
- [ ] Vídeo tutorial (opcional)

---

### Issue #55: Implementar modo Docker para facilitar setup
**Labels**: `feat`, `chore`, `priority: medium`, `complexity: intermediate`  

**Descrição**:
Containerizar aplicação com Docker.

**Tarefas**:
- [ ] Criar `Dockerfile`
- [ ] Criar `docker-compose.yml`
- [ ] Imagem inclui Python + dependências
- [ ] Volume para persistir banco de dados
- [ ] Documentar uso com Docker
- [ ] Publicar imagem no Docker Hub (opcional)

---

## RESUMO QUANTITATIVO

### Por Fase
- **Fase 0 (Fundamentos)**: 10 issues (#1-#10)
- **Fase 1 (Digital)**: 16 issues (#11-#26)
- **Fase 2 (Hardware)**: 8 issues (#27-#34)
- **Fase 3 (Maquete)**: 8 issues (#35-#42)
- **Fase 4 (Expansão)**: 8 issues (#43-#50)
- **Infraestrutura**: 5 issues (#51-#55)

**Total**: 55 issues detalhadas

### Por Prioridade
- **Critical**: ~12 issues
- **High**: ~18 issues
- **Medium**: ~20 issues
- **Low**: ~5 issues

### Por Complexidade
- **Beginner**: ~12 issues (good first issue)
- **Intermediate**: ~28 issues
- **Advanced**: ~15 issues

---

## 📅 CRONOGRAMA SUGERIDO

### Mês 1-2: Fase 0
- Issues #1-#10
- Foco: Infraestrutura e aprendizado

### Mês 3: Fase 1.1-1.2
- Issues #11-#18
- Foco: Mundo estático e agentes

### Mês 4: Fase 1.3-1.4
- Issues #19-#26
- Foco: Economia e transporte virtual

### Mês 5-6: Fase 2.1-2.2
- Issues #27-#31
- Foco: Arduino, iluminação e sensores

### Mês 7: Fase 2.3
- Issues #32-#34
- Foco: Controle de desvios

### Mês 8-9: Fase 3.1-3.2
- Issues #35-#38
- Foco: Base física e trilhos

### Mês 10-11: Fase 3.3-3.4
- Issues #39-#42
- Foco: Edifícios e integração completa

### Mês 12+: Fase 4
- Issues #43-#50
- Foco: Expansões conforme interesse

### Contínuo: Infraestrutura
- Issues #51-#55
- Manutenção e melhorias

---

## 🎯 PRÓXIMOS PASSOS

1. **Revisar este documento** e ajustar conforme necessário
2. **Criar labels** no GitHub conforme seção "Tags/Labels"
3. **Criar milestones** no GitHub conforme seção "Milestones"
4. **Criar issues** no GitHub usando as descrições detalhadas acima
5. **Priorizar** primeiras 5-10 issues para começar
6. **Começar desenvolvimento** pela Fase 0

---

**Observação**: Este documento é um plano vivo e pode ser atualizado conforme o projeto evolui e novas necessidades surgem.
