# TODO: Issue #1 - Configurar estrutura de projeto Python
# Labels: feat, phase-0: fundamentals, priority: critical, complexity: beginner
# Milestone: Milestone 0: Fundamentos e Infraestrutura

"""
Reorganizar estrutura do projeto seguindo arquitetura definida no GDD.

Tarefas:
- [ ] Criar diretórios: backend/, frontend/, visualization/, hardware/, data/, docs/, tests/
- [ ] Mover código existente (app/) para backend/simulation/
- [ ] Criar arquivos __init__.py em todos os pacotes
- [ ] Atualizar imports em todos os arquivos
- [ ] Criar backend/config.py para configurações centralizadas
- [ ] Atualizar requirements.txt com dependências organizadas por categoria
- [ ] Atualizar documentação com nova estrutura

Critérios de Aceitação:
- Estrutura de diretórios segue o GDD
- Todos os testes existentes continuam passando
- Imports funcionam corretamente
- README.md atualizado com nova estrutura
"""

# Nova estrutura esperada:
# ferritine/
# ├── backend/           # Lógica de backend
# │   ├── simulation/    # Motor de simulação (mover app/ para aqui)
# │   ├── database/      # Modelos e queries do banco
# │   ├── api/           # API REST/WebSocket
# │   └── utils/         # Utilitários (logger, config)
# ├── frontend/          # Interface web (futuro)
# ├── visualization/     # Visualização 2D/3D
# ├── hardware/          # Código para Arduino/ESP32
# ├── data/              # Banco de dados, logs, configs
# │   ├── logs/
# │   ├── db/
# │   └── config/
# ├── docs/              # Documentação (já existe)
# └── tests/             # Testes (expandir)
#     ├── unit/
#     └── integration/

# TODO: Executar migração
# 1. Criar estrutura de diretórios
# 2. Mover arquivos existentes
# 3. Atualizar imports
# 4. Testar que tudo funciona

