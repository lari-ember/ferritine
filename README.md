# 🏙️ Ferritine

[![Python Version](https://img.shields.io/badge/python-3.8%2B-blue.svg)](https://www.python.org/downloads/release/python-380/)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)
[![Tests](https://img.shields.io/github/actions/workflow/status/ferritine/tests.yml?branch=main)]

**Ferritine** é um projeto de maquete híbrida físico-digital que combina agentes inteligentes, simulação temporal e integração com hardware para criar um ambiente urbano interativo e dinâmico.

O projeto simula uma cidade com agentes que possuem rotinas diárias realistas, movendo-se entre casa e trabalho de acordo com horários programados. É ideal para pesquisa em sistemas multiagentes, simulação urbana e IoT.

## ✨ Recursos

- 🤖 **Simulação de Agentes**: Agentes autônomos com rotinas diárias (casa ↔ trabalho)
- 🏗️ **Arquitetura Modular**: Código organizado e fácil de estender
- ⏰ **Simulação Temporal**: Sistema de tempo discreto (horas do dia)
- 🧪 **Testes Automatizados**: Cobertura de testes unitários com pytest
- 📊 **Snapshots de Estado**: Visualização do estado da cidade a qualquer momento
- 🔄 **Sistema de Versionamento**: Versionamento semântico automatizado
- 🚀 **CI/CD**: Workflows automatizados para releases e testes

## 📋 Pré-requisitos

Antes de começar, certifique-se de ter instalado:

- **Python 3.8 ou superior** ([Download](https://www.python.org/downloads/))
- **pip** (gerenciador de pacotes do Python)
- **git** (para clonar o repositório)
- **Sistema operacional**: Linux, macOS ou Windows com WSL

### Dependências do Projeto

As dependências são gerenciadas através do `requirements.txt`:
- `pytest>=8.4.2` - Framework de testes
- Outras dependências serão listadas conforme o projeto evolui

## 🚀 Instalação e Uso

### 1. Clone o Repositório
```bash
git clone https://github.com/ferritine/ferritine.git
cd ferritine
```

### 2. Configure o Ambiente Virtual
```bash
# Criar ambiente virtual
python -m venv .venv

# Ativar ambiente virtual
# No Linux/macOS:
source .venv/bin/activate
# No Windows (PowerShell):
# .venv\Scripts\Activate.ps1
```

### 3. Instale as Dependências
```bash
pip install -r requirements.txt
```

### 4. Execute o Programa
```bash
python main.py
```

### 5. Execute os Testes
```bash
# Executar todos os testes
pytest

# Executar com verbose
pytest -v

# Executar com cobertura
pytest --cov=app
```

## 📁 Estrutura do Projeto

```
ferritine/
├── .github/                      # Configurações do GitHub
│   ├── ISSUE_TEMPLATE/           # Templates para issues
│   │   ├── bug_report.md         # Template para reportar bugs
│   │   └── feature_request.md    # Template para solicitar funcionalidades
│   ├── scripts/                  # Scripts de automação
│   │   ├── bump_version.py       # Script para atualizar versão
│   │   └── generate_changelog.sh # Script para gerar changelog
│   └── workflows/                # GitHub Actions workflows
│       └── release.yml           # Workflow de release automatizado
├── app/                          # Código principal da aplicação
│   ├── __init__.py               # Inicialização do pacote
│   ├── models/                   # Modelos de domínio
│   │   ├── __init__.py
│   │   ├── agente.py             # Classe Agente (habitantes)
│   │   └── cidade.py             # Classe Cidade (mundo da simulação)
│   └── tests/                    # Testes automatizados
│       └── test_sim.py           # Testes de simulação
├── scripts/                      # Scripts auxiliares
│   └── bump_version.py           # Script local de versionamento
├── main.py                       # Ponto de entrada da aplicação
├── requirements.txt              # Dependências do projeto
├── VERSION                       # Versão atual (semantic versioning)
├── README.md                     # Documentação principal
└── LICENSE                       # Licença do projeto
```

### Descrição dos Componentes

- **`app/models/agente.py`**: Define a classe `Agente` que representa habitantes da cidade com rotinas diárias
- **`app/models/cidade.py`**: Define a classe `Cidade` que gerencia os agentes e a simulação
- **`app/tests/`**: Contém testes unitários para validar o comportamento do sistema
- **`main.py`**: Script de demonstração que executa uma simulação de 24 horas
- **`VERSION`**: Arquivo que armazena a versão atual usando versionamento semântico (MAJOR.MINOR.PATCH)
- **`.github/`**: Configurações e automações do GitHub (CI/CD, templates)

## 💡 Exemplos de Uso

### Executando a Demo

Ao executar `python main.py`, você verá uma simulação de 24 horas:

```
00h -> {'Ana': 'CasaA', 'Beto': 'CasaB', 'Clara': 'CasaC'}
01h -> {'Ana': 'CasaA', 'Beto': 'CasaB', 'Clara': 'CasaC'}
...
07h -> {'Ana': 'Fábrica', 'Beto': 'Loja', 'Clara': 'Escola'}
...
17h -> {'Ana': 'CasaA', 'Beto': 'CasaB', 'Clara': 'CasaC'}
```

### Criando Seus Próprios Agentes

```python
from app.models.agente import Agente
from app.models.cidade import Cidade

# Criar uma cidade
cidade = Cidade()

# Adicionar agentes com diferentes rotinas
cidade.add_agente(Agente("João", "CasaA", "Hospital"))
cidade.add_agente(Agente("Maria", "CasaB", "Universidade"))

# Simular um dia
for hora in range(24):
    cidade.step(hora)
    print(f"{hora:02d}h -> {cidade.snapshot()}")
```

### Personalizando Horários

Você pode estender a classe `Agente` para customizar horários:

```python
class AgentePersonalizado(Agente):
    def step(self, hora: int):
        # Trabalha das 9h às 18h
        if 9 <= hora < 18:
            self.local = self.trabalho
        else:
            self.local = self.casa
```

## 🤝 Contribuição

Contribuições são muito bem-vindas! Este projeto segue boas práticas de desenvolvimento e code review.

### Como Contribuir

1. **Fork o Repositório**
   ```bash
   # Clique em "Fork" no GitHub e depois clone seu fork
   git clone https://github.com/seu-usuario/ferritine.git
   cd ferritine
   ```

2. **Crie uma Branch para sua Feature**
   ```bash
   git checkout -b feature/minha-nova-feature
   # ou
   git checkout -b fix/correcao-de-bug
   ```

3. **Faça suas Alterações**
   - Escreva código limpo e bem documentado
   - Siga o padrão PEP 8 (Python)
   - Adicione testes para novas funcionalidades
   - Adicione docstrings em português para funções e classes

4. **Execute os Testes**
   ```bash
   pytest -v
   ```

5. **Commit suas Mudanças**
   ```bash
   git add .
   git commit -m "feat: adiciona nova funcionalidade X"
   ```
   
   **Padrão de Commits** (Conventional Commits):
   - `feat:` - Nova funcionalidade
   - `fix:` - Correção de bug
   - `docs:` - Mudanças na documentação
   - `test:` - Adição ou modificação de testes
   - `refactor:` - Refatoração de código
   - `chore:` - Tarefas de manutenção

6. **Push para seu Fork**
   ```bash
   git push origin feature/minha-nova-feature
   ```

7. **Abra um Pull Request**
   - Vá para o repositório original no GitHub
   - Clique em "New Pull Request"
   - Descreva suas mudanças detalhadamente

### Reportando Bugs

Use o [template de bug report](.github/ISSUE_TEMPLATE/bug_report.md) para reportar bugs. Inclua:
- Descrição clara do problema
- Passos para reproduzir
- Comportamento esperado vs. atual
- Informações do ambiente (SO, versão do Python, etc.)

### Solicitando Funcionalidades

Use o [template de feature request](.github/ISSUE_TEMPLATE/feature_request.md) para sugerir novas funcionalidades.

### Diretrizes de Código

- **Estilo**: Siga PEP 8
- **Documentação**: Todas as classes e funções públicas devem ter docstrings
- **Testes**: Novas funcionalidades devem incluir testes
- **Idioma**: Código e comentários em português
- **Type Hints**: Use type hints quando possível

## 🔖 Versionamento

Este projeto usa [Versionamento Semântico (SemVer)](https://semver.org/lang/pt-BR/):

- **MAJOR** (X.0.0): Mudanças incompatíveis na API
- **MINOR** (0.X.0): Novas funcionalidades compatíveis com versões anteriores
- **PATCH** (0.0.X): Correções de bugs compatíveis com versões anteriores

A versão atual está no arquivo `VERSION` na raiz do projeto.

### Atualizando a Versão Localmente

Use o script de bump de versão:

```bash
# Incrementar patch version (0.1.0 -> 0.1.1)
python .github/scripts/bump_version.py --level patch

# Incrementar minor version (0.1.0 -> 0.2.0)
python .github/scripts/bump_version.py --level minor

# Incrementar major version (0.1.0 -> 1.0.0)
python .github/scripts/bump_version.py --level major
```

### Criando Releases

O projeto possui um workflow automatizado para criar releases:

1. Acesse a aba **Actions** no GitHub
2. Selecione o workflow **"Release (bump & create)"**
3. Clique em **"Run workflow"**
4. Escolha o nível de incremento (patch/minor/major)
5. Confirme a execução

O workflow irá:
- ✅ Incrementar a versão no arquivo `VERSION`
- ✅ Fazer commit e push da nova versão
- ✅ Criar uma tag Git (`vX.Y.Z`)
- ✅ Gerar changelog automaticamente
- ✅ Criar uma Release no GitHub com o changelog

## 📝 Changelog

O changelog é gerado automaticamente durante o processo de release, baseado nos commits entre as tags. Para melhores resultados, use [Conventional Commits](https://www.conventionalcommits.org/pt-br/) nas mensagens de commit.

## 📄 Licença
Este projeto está licenciado sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

## 🗺️ Roadmap

Funcionalidades planejadas para futuras versões:

- [ ] Interface gráfica para visualização da simulação
- [ ] Integração com hardware (Arduino/Raspberry Pi)
- [ ] Múltiplos tipos de agentes (estudantes, trabalhadores, etc.)
- [ ] Sistema de eventos aleatórios
- [ ] Persistência de estado (salvar/carregar simulações)
- [ ] API REST para controle remoto
- [ ] Dashboard web em tempo real
- [ ] Suporte a múltiplas cidades
- [ ] Sistema de transporte público

## 🙏 Agradecimentos

Este projeto foi desenvolvido como parte de uma maquete híbrida físico-digital, combinando simulação computacional com elementos físicos para criar uma experiência educacional e interativa.

## 📞 Contato

Se você tiver dúvidas, sugestões ou quiser contribuir, sinta-se à vontade para:

- Abrir uma [issue](https://github.com/ferritine/ferritine/issues)
- Enviar um [pull request](https://github.com/ferritine/ferritine/pulls)
- Entrar em contato através do GitHub

---

<div align="center">

**Feito com ❤️ para simulação urbana e IoT**

[![Stars](https://img.shields.io/github/stars/ferritine/ferritine?style=social)](https://github.com/ferritine/ferritine/stargazers)
[![Forks](https://img.shields.io/github/forks/ferritine/ferritine?style=social)](https://github.com/ferritine/ferritine/network/members)

</div>

