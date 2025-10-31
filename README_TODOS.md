# 📋 TODOs das Issues Prioritárias - Fase 0

Este diretório contém arquivos TODO detalhados para as **Issues #1-#5 da Fase 0** (Infraestrutura Crítica).

## 📁 Arquivos TODO

### ✅ Issue #1: Configurar estrutura de projeto Python
**Arquivo**: `TODO_ISSUE_01_project_structure.py`  
**Prioridade**: CRÍTICA  
**Complexidade**: Beginner  

Reorganizar estrutura do projeto seguindo arquitetura definida no GDD.

**Próximos passos**:
1. Criar nova estrutura de diretórios
2. Migrar código existente de `app/` para `backend/simulation/`
3. Atualizar todos os imports
4. Testar que tudo funciona

---

### ✅ Issue #2: Configurar sistema de logging
**Arquivo**: `TODO_ISSUE_02_logging_system.py`  
**Prioridade**: ALTA  
**Complexidade**: Beginner  

Implementar sistema de logging profissional para debug e monitoramento.

**Próximos passos**:
1. Criar `backend/utils/logger.py` com base no código TODO
2. Configurar diferentes níveis de log
3. Adicionar rotação de logs
4. Integrar em módulos existentes

---

### ✅ Issue #3: Implementar sistema de configuração com YAML
**Arquivo**: `TODO_ISSUE_03_config_system.py`  
**Prioridade**: ALTA  
**Complexidade**: Intermediate  

Criar sistema de configuração centralizado usando arquivos YAML.

**Próximos passos**:
1. Instalar PyYAML: `pip install PyYAML`
2. Criar `backend/utils/config_loader.py` com base no código TODO
3. Criar `data/config.yaml` com configurações padrão
4. Criar `config.example.yaml` como exemplo
5. Atualizar código existente para usar configurações

---

### ✅ Issue #4: Configurar banco de dados SQLite
**Arquivo**: `TODO_ISSUE_04_database.py`  
**Prioridade**: CRÍTICA  
**Complexidade**: Intermediate  

Implementar persistência de dados usando SQLite e SQLAlchemy.

**Próximos passos**:
1. Instalar dependências: `pip install sqlalchemy alembic`
2. Criar `backend/database/models.py` com modelos do TODO
3. Criar `backend/database/queries.py` com queries
4. Configurar Alembic para migrations
5. Criar testes de banco de dados

---

### ✅ Issue #5: Criar documentação técnica de arquitetura
**Arquivo**: `TODO_ISSUE_05_architecture_docs.py`  
**Prioridade**: MÉDIA  
**Complexidade**: Beginner  

Documentar a arquitetura do sistema detalhadamente.

**Próximos passos**:
1. Criar `docs/architecture.md` com conteúdo do TODO
2. Adicionar diagramas Mermaid
3. Documentar padrões de design
4. Revisar e expandir conforme evolução do projeto

---

## 🚀 Como Usar

### 1. Revisar o TODO
Abra o arquivo TODO correspondente à issue que deseja trabalhar:

```bash
# Exemplo: Issue #1
cat TODO_ISSUE_01_project_structure.py
```

### 2. Implementar
Use o código TODO como guia/template para implementação:

```bash
# Copie seções relevantes do TODO para os arquivos reais
# Exemplo:
mkdir -p backend/utils
cp TODO_ISSUE_02_logging_system.py backend/utils/logger.py
# Depois edite e complete a implementação
```

### 3. Testar
Execute testes para verificar que funciona:

```bash
pytest -v
```

### 4. Marcar como Concluído
Quando terminar a issue:
1. Feche a issue no GitHub
2. Delete ou arquive o arquivo TODO
3. Atualize documentação

---

## 📋 Ordem Recomendada de Implementação

### Semana 1:
1. **Issue #1** - Estrutura do projeto
   - Fundamental para organização
   - Bloqueia outras issues

### Semana 2:
2. **Issue #2** - Sistema de logging
   - Importante para debug
3. **Issue #3** - Sistema de configuração
   - Necessário antes do banco

### Semana 3:
4. **Issue #4** - Banco de dados SQLite
   - Complexo, pode levar mais tempo
   - Fundamental para persistência

### Semana 4:
5. **Issue #5** - Documentação de arquitetura
   - Pode ser feita em paralelo
   - Importante para onboarding

---

## 🔗 Links Relacionados

- [ISSUES_MILESTONES_TAGS.md](ISSUES_MILESTONES_TAGS.md) - Issues completas
- [docs/QUICK_ISSUE_GUIDE.md](docs/QUICK_ISSUE_GUIDE.md) - Guia de criação de issues
- [docs/GDD_FERRITINE.md](docs/GDD_FERRITINE.md) - Game Design Document
- [docs/architecture.md](docs/architecture.md) - Arquitetura (após Issue #5)

---

## ✅ Checklist Rápida

Antes de começar uma issue:

- [ ] Leu o arquivo TODO correspondente
- [ ] Entendeu os critérios de aceitação
- [ ] Tem as dependências instaladas
- [ ] Criou branch feature/issue-X
- [ ] Testes existentes estão passando

Ao concluir uma issue:

- [ ] Implementação completa
- [ ] Testes criados/atualizados
- [ ] Documentação atualizada
- [ ] Code review aprovado
- [ ] Merge para main
- [ ] Issue fechada no GitHub

---

## 💡 Dicas

### Para Issue #1 (Estrutura):
- Faça backup antes de mover arquivos
- Use `git mv` ao invés de `mv` para manter histórico
- Atualize `.gitignore` se necessário
- Rode testes após cada mudança

### Para Issue #2 (Logging):
- Comece simples, adicione complexidade depois
- Use logging.getLogger(__name__) em módulos
- Configure rotação de logs para evitar arquivos grandes

### Para Issue #3 (Config):
- Valide configurações ao carregar
- Use valores padrão sensatos
- Documente cada opção de configuração

### Para Issue #4 (Database):
- Use migrations desde o início
- Não modifique modelos diretamente, use Alembic
- Teste com banco in-memory primeiro

### Para Issue #5 (Docs):
- Use Mermaid para diagramas (renderiza no GitHub)
- Mantenha atualizado conforme arquitetura evolui
- Explique "por quê", não apenas "o quê"

---

## 🆘 Precisa de Ajuda?

- Consulte a documentação em `docs/`
- Veja exemplos de código nos TODOs
- Abra issue no GitHub com dúvidas
- Peça review antes de mergear

---

**Última atualização**: 2025-10-29

