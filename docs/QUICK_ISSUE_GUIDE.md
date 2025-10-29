# 🚀 Guia Rápido para Criação de Issues

Este guia fornece instruções práticas para criar as issues, milestones e labels no GitHub baseadas no documento `ISSUES_MILESTONES_TAGS.md`.

## 📋 Pré-requisitos

- Acesso de administrador ao repositório GitHub
- Familiaridade com interface do GitHub

---

## 1️⃣ CRIAR LABELS (TAGS)

Vá em: **Settings** → **Labels** → **New label**

### Labels por Tipo (cor azul #0366d6)
```
Nome: feat
Descrição: Nova funcionalidade
Cor: #0366d6
```

```
Nome: bug
Descrição: Correção de bug
Cor: #d73a4a
```

```
Nome: docs
Descrição: Documentação
Cor: #0075ca
```

```
Nome: test
Descrição: Testes
Cor: #d4c5f9
```

```
Nome: refactor
Descrição: Refatoração
Cor: #fbca04
```

```
Nome: chore
Descrição: Manutenção/tarefas auxiliares
Cor: #fef2c0
```

```
Nome: hardware
Descrição: Relacionado a hardware/eletrônica
Cor: #e99695
```

```
Nome: simulation
Descrição: Sistema de simulação
Cor: #1d76db
```

```
Nome: iot
Descrição: Internet das Coisas
Cor: #5319e7
```

### Labels por Prioridade (cor vermelha #d73a4a)
```
Nome: priority: critical
Descrição: Crítico, bloqueia outras funcionalidades
Cor: #b60205
```

```
Nome: priority: high
Descrição: Alta prioridade
Cor: #d93f0b
```

```
Nome: priority: medium
Descrição: Prioridade média
Cor: #fbca04
```

```
Nome: priority: low
Descrição: Baixa prioridade
Cor: #0e8a16
```

### Labels por Fase (cor roxa #5319e7)
```
Nome: phase-0: fundamentals
Descrição: Fase 0: Fundamentos
Cor: #5319e7
```

```
Nome: phase-1: digital
Descrição: Fase 1: Simulação Digital
Cor: #7057ff
```

```
Nome: phase-2: basic-hardware
Descrição: Fase 2: Hardware Básico
Cor: #8b72ff
```

```
Nome: phase-3: physical-model
Descrição: Fase 3: Maquete Física
Cor: #a18dff
```

```
Nome: phase-4: expansion
Descrição: Fase 4: Expansão
Cor: #b8a9ff
```

### Labels por Área (cor verde #0e8a16)
```
Nome: area: agents
Descrição: Sistema de agentes
Cor: #0e8a16
```

```
Nome: area: economy
Descrição: Sistema econômico
Cor: #1a7f37
```

```
Nome: area: transport
Descrição: Sistema de transporte
Cor: #22863a
```

```
Nome: area: politics
Descrição: Sistema político
Cor: #2ea44f
```

```
Nome: area: construction
Descrição: Sistema de construção
Cor: #2da44e
```

```
Nome: area: ui
Descrição: Interface de usuário
Cor: #38b44a
```

```
Nome: area: database
Descrição: Banco de dados
Cor: #46c85c
```

```
Nome: area: api
Descrição: API REST/WebSocket
Cor: #57d769
```

```
Nome: area: world
Descrição: Mundo/Cidade
Cor: #6ae07f
```

### Labels por Complexidade (cor amarela #fbca04)
```
Nome: complexity: beginner
Descrição: Fácil, bom para iniciantes
Cor: #c5def5
```

```
Nome: complexity: intermediate
Descrição: Complexidade média
Cor: #fbca04
```

```
Nome: complexity: advanced
Descrição: Avançado, requer experiência
Cor: #d93f0b
```

### Labels Especiais
```
Nome: good first issue
Descrição: Boa primeira issue para novos contribuidores
Cor: #7057ff
```

```
Nome: help wanted
Descrição: Precisa de ajuda da comunidade
Cor: #008672
```

```
Nome: blocked
Descrição: Bloqueada por outra issue
Cor: #e99695
```

```
Nome: wip
Descrição: Work in Progress (em andamento)
Cor: #d4c5f9
```

```
Nome: research
Descrição: Pesquisa necessária
Cor: #c5def5
```

```
Nome: ai
Descrição: Inteligência Artificial / Machine Learning
Cor: #5319e7
```

---

## 2️⃣ CRIAR MILESTONES

Vá em: **Issues** → **Milestones** → **New milestone**

### Milestone 0: Fundamentos e Infraestrutura
```
Título: Milestone 0: Fundamentos e Infraestrutura
Data de Vencimento: [2 meses a partir de hoje]
Descrição:
Estabelecer fundamentos do projeto, documentação básica e aprendizado inicial.

Objetivos:
- Reorganizar estrutura do projeto
- Configurar logging, configuração e banco de dados
- Criar documentação técnica
- Documentar currículos de aprendizado (eletrônica, IoT, simulação)
```

### Milestone 1.1: Mundo Estático
```
Título: Milestone 1.1: Mundo Estático
Data de Vencimento: [2 semanas após Milestone 0]
Descrição:
Criar estrutura básica da cidade com grid 2D, edifícios e ruas.

Objetivos:
- Grid 2D implementado
- Sistema de edifícios funcionando
- Ruas e trilhos no grid
- Visualização 2D com Pygame
```

### Milestone 1.2: Agentes Simples
```
Título: Milestone 1.2: Agentes Simples
Data de Vencimento: [2 semanas após Milestone 1.1]
Descrição:
Implementar agentes com rotinas básicas e atributos.

Objetivos:
- Agentes com atributos físicos/mentais
- Máquina de estados implementada
- Rotinas diárias funcionando
- Visualização de agentes no mapa
```

### Milestone 1.3: Economia Básica
```
Título: Milestone 1.3: Economia Básica
Data de Vencimento: [2 semanas após Milestone 1.2]
Descrição:
Sistema de salários, gastos e produção simples.

Objetivos:
- Sistema econômico básico
- Cadeia produtiva implementada
- Dashboard de estatísticas
```

### Milestone 1.4: Transporte Ferroviário Virtual
```
Título: Milestone 1.4: Transporte Ferroviário Virtual
Data de Vencimento: [2 semanas após Milestone 1.3]
Descrição:
Trens virtuais funcionando na simulação.

Objetivos:
- Classe Train implementada
- Rotas e horários funcionando
- Embarque/desembarque de passageiros
- Transporte de carga
- Visualização de trens
```

### Milestone 2.1: Circuito de Iluminação
```
Título: Milestone 2.1: Circuito de Iluminação
Data de Vencimento: [3 semanas após Milestone 1.4]
Descrição:
Arduino controlando LEDs via Python.

Objetivos:
- Comunicação Serial Python-Arduino
- Firmware de controle de LEDs
- Iluminação sincronizada com dia/noite
```

### Milestone 2.2: Sensor de Trem
```
Título: Milestone 2.2: Sensor de Trem
Data de Vencimento: [2 semanas após Milestone 2.1]
Descrição:
Detecção de trem com reed switch.

Objetivos:
- Firmware de sensor de trem
- Integração com simulação
```

### Milestone 2.3: Controle de Desvio
```
Título: Milestone 2.3: Controle de Desvio
Data de Vencimento: [2 semanas após Milestone 2.2]
Descrição:
Servomotor controlando desvios de trilho.

Objetivos:
- Firmware de controle de servo
- Integração com simulação
```

### Milestone 3.1: Base e Topografia
```
Título: Milestone 3.1: Base e Topografia
Data de Vencimento: [4 semanas após Milestone 2.3]
Descrição:
Construir base física MDF 100x100cm com relevo.

Objetivos:
- Projeto da base documentado
- Guia de construção de relevo
```

### Milestone 3.2: Trilhos e Primeiro Trem
```
Título: Milestone 3.2: Trilhos e Primeiro Trem
Data de Vencimento: [4 semanas após Milestone 3.1]
Descrição:
Sistema ferroviário físico funcionando.

Objetivos:
- Guia de instalação de trilhos
- Controle DCC integrado
```

### Milestone 3.3: Primeiros Edifícios
```
Título: Milestone 3.3: Primeiros Edifícios
Data de Vencimento: [4 semanas após Milestone 3.2]
Descrição:
3-5 prédios construídos e instalados.

Objetivos:
- Modelos 3D criados
- Técnicas de construção documentadas
```

### Milestone 3.4: Integração Física-Digital
```
Título: Milestone 3.4: Integração Física-Digital
Data de Vencimento: [4 semanas após Milestone 3.3]
Descrição:
Sincronização completa entre maquete física e simulação.

Objetivos:
- Sincronização física-digital funcionando
- Dashboard web implementado
```

---

## 3️⃣ CRIAR ISSUES

Vá em: **Issues** → **New issue**

### Template de Issue

```markdown
**Título**: [Copiar da issue correspondente no ISSUES_MILESTONES_TAGS.md]

**Labels**: [Selecionar labels apropriados]

**Milestone**: [Selecionar milestone apropriado]

**Assignees**: [Atribuir a você mesmo ou deixar em branco]

**Descrição**:
[Copiar descrição da issue do ISSUES_MILESTONES_TAGS.md]

**Tarefas**:
[Copiar checklist de tarefas]

**Critérios de Aceitação**:
[Copiar critérios de aceitação]

**Notas Adicionais**:
[Adicionar quaisquer notas relevantes]
```

### Exemplo: Issue #1

```markdown
**Título**: Configurar estrutura de projeto Python

**Labels**: feat, phase-0: fundamentals, priority: critical, complexity: beginner

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
```

---

## 4️⃣ ORDEM RECOMENDADA DE CRIAÇÃO

### Prioridade 1 (Começar Imediatamente)
1. **Criar todos os labels** (15-20 minutos)
2. **Criar Milestone 0** (2 minutos)
3. **Criar Issues #1-#5** (Fase 0 - Infraestrutura crítica)

### Prioridade 2 (Primeira Semana)
1. **Criar Issues #6-#10** (Fase 0 - Documentação de aprendizado)
2. **Criar Milestone 1.1**
3. **Criar Issues #11-#14** (Fase 1.1 - Mundo Estático)

### Prioridade 3 (Primeira Quinzena)
1. **Criar Milestones 1.2, 1.3, 1.4**
2. **Criar Issues #15-#26** (Fase 1 completa)

### Prioridade 4 (Primeiro Mês)
1. **Criar Milestones 2.1, 2.2, 2.3**
2. **Criar Issues #27-#34** (Fase 2)

### Prioridade 5 (Conforme Necessário)
1. **Criar Milestones 3.x**
2. **Criar Issues #35-#55** (Fases 3 e 4)

---

## 5️⃣ DICAS E BOAS PRÁTICAS

### Para Labels
- ✅ Use cores consistentes por categoria
- ✅ Mantenha nomes curtos e descritivos
- ✅ Não crie muitos labels (dificulta organização)

### Para Milestones
- ✅ Defina datas realistas
- ✅ Não tenha mais de 3-4 milestones ativos simultaneamente
- ✅ Feche milestones quando todos os issues estiverem completos

### Para Issues
- ✅ Use títulos claros e acionáveis (comece com verbo)
- ✅ Detalhe critérios de aceitação
- ✅ Referencie documentação relevante
- ✅ Adicione links para issues relacionadas/bloqueadoras
- ✅ Atualize o status regularmente

### Organização
- ✅ Use **Projects** do GitHub para visualizar progresso (Kanban)
- ✅ Adicione issues aos Projects automaticamente
- ✅ Revise milestones semanalmente
- ✅ Feche issues concluídas prontamente

---

## 6️⃣ AUTOMAÇÃO OPCIONAL

### GitHub CLI (gh)
Se você tem `gh` instalado, pode criar issues via linha de comando:

```bash
# Criar issue
gh issue create \
  --title "Configurar estrutura de projeto Python" \
  --body "$(cat issue_template.md)" \
  --label "feat,phase-0: fundamentals,priority: critical" \
  --milestone "Milestone 0"

# Criar milestone
gh api repos/:owner/:repo/milestones \
  -f title="Milestone 0: Fundamentos" \
  -f description="Estabelecer fundamentos do projeto" \
  -f due_on="2024-12-31T23:59:59Z"

# Criar label
gh label create "phase-0: fundamentals" \
  --description "Fase 0: Fundamentos" \
  --color "5319e7"
```

### Script Python
Você pode criar um script para automatizar criação em massa:

```python
# scripts/create_issues.py
import requests
import json

GITHUB_TOKEN = "seu_token_aqui"
REPO_OWNER = "lari-ember"
REPO_NAME = "ferritine"

def create_label(name, description, color):
    url = f"https://api.github.com/repos/{REPO_OWNER}/{REPO_NAME}/labels"
    headers = {
        "Authorization": f"token {GITHUB_TOKEN}",
        "Accept": "application/vnd.github.v3+json"
    }
    data = {
        "name": name,
        "description": description,
        "color": color
    }
    response = requests.post(url, headers=headers, json=data)
    return response.json()

def create_issue(title, body, labels, milestone):
    url = f"https://api.github.com/repos/{REPO_OWNER}/{REPO_NAME}/issues"
    headers = {
        "Authorization": f"token {GITHUB_TOKEN}",
        "Accept": "application/vnd.github.v3+json"
    }
    data = {
        "title": title,
        "body": body,
        "labels": labels,
        "milestone": milestone
    }
    response = requests.post(url, headers=headers, json=data)
    return response.json()

# Uso
# create_label("feat", "Nova funcionalidade", "0366d6")
# create_issue("Issue Title", "Issue body", ["feat", "priority: high"], 1)
```

---

## 7️⃣ CHECKLIST FINAL

Antes de considerar completo:

- [ ] Todos os labels criados (30+ labels)
- [ ] Todos os milestones criados (12 milestones)
- [ ] Issues da Fase 0 criadas (#1-#10)
- [ ] Issues da Fase 1 criadas (#11-#26)
- [ ] Issues críticas priorizadas
- [ ] Project Board criado (opcional)
- [ ] README.md atualizado com link para issues
- [ ] Documentação `ISSUES_MILESTONES_TAGS.md` no repositório

---

## 📚 Recursos Adicionais

- [Documentação de Issues do GitHub](https://docs.github.com/en/issues)
- [Documentação de Labels](https://docs.github.com/en/issues/using-labels-and-milestones-to-track-work/managing-labels)
- [Documentação de Milestones](https://docs.github.com/en/issues/using-labels-and-milestones-to-track-work/about-milestones)
- [GitHub Projects](https://docs.github.com/en/issues/planning-and-tracking-with-projects)
- [GitHub CLI](https://cli.github.com/)

---

**Boa organização!** 🎉
