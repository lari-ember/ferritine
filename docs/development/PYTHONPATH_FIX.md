# 🔧 Solução: ModuleNotFoundError no GitHub Actions

## Problema

Ao executar testes no GitHub Actions, ocorria o erro:
```
ModuleNotFoundError: No module named 'app'
```

Isso acontecia porque o GitHub Actions não adiciona automaticamente o diretório raiz ao `PYTHONPATH`, diferente de alguns ambientes locais.

## Soluções Implementadas

### 1. ✅ Configuração do pytest (`pytest.ini`)

Criamos o arquivo `pytest.ini` que configura o pytest para adicionar o diretório raiz ao PYTHONPATH:

```ini
[pytest]
pythonpath = .
testpaths = app/tests
```

**Vantagens:**
- Funciona automaticamente em qualquer ambiente
- Não requer configuração manual
- É a solução mais limpa e profissional

### 2. ✅ Workflow do GitHub Actions (`tests.yml`)

Adicionamos `PYTHONPATH` explicitamente no workflow:

```yaml
- name: Executar testes com pytest
  run: |
    PYTHONPATH=$PYTHONPATH:. python -m pytest -v --cov=app
```

**Vantagens:**
- Garante que funciona mesmo sem pytest.ini
- Backup caso a configuração falhe
- Explícito e fácil de debugar

### 3. ✅ Setup.py (Instalação em Modo Desenvolvimento)

Criamos `setup.py` para permitir instalação do pacote:

```bash
pip install -e .
```

**Vantagens:**
- Instala o pacote localmente
- Permite imports de qualquer lugar
- Facilita distribuição futura
- Recomendado para desenvolvedores

## Como Usar

### Opção 1: pytest.ini (Automático) ⭐ Recomendado

Apenas execute os testes normalmente:
```bash
python -m pytest -v
```

O `pytest.ini` já configura tudo automaticamente.

### Opção 2: Instalação em Modo Desenvolvimento

```bash
# Instalar o pacote em modo desenvolvimento
pip install -e .

# Agora pode importar de qualquer lugar
python -m pytest -v
```

### Opção 3: PYTHONPATH Manual

```bash
# Linux/macOS
PYTHONPATH=. python -m pytest -v

# Windows (PowerShell)
$env:PYTHONPATH="."; python -m pytest -v

# Windows (CMD)
set PYTHONPATH=. && python -m pytest -v
```

## Verificação

Para verificar se está funcionando:

```bash
# Teste localmente
python -m pytest -v

# Verifique o PYTHONPATH atual
python -c "import sys; print('\n'.join(sys.path))"

# Teste o import manualmente
python -c "from app.models.agente import Agente; print('✓ Import OK')"
```

## Por Que Isso Acontece?

### Localmente
- IDEs como PyCharm/VSCode adicionam o diretório do projeto ao PYTHONPATH automaticamente
- Você pode não perceber que depende disso

### GitHub Actions
- Ambiente limpo sem configurações automáticas
- `sys.path` não inclui o diretório atual
- Precisa de configuração explícita

## Arquivos Criados/Modificados

- ✅ `pytest.ini` - Configuração do pytest
- ✅ `setup.py` - Setup para instalação
- ✅ `.github/workflows/tests.yml` - Workflow com PYTHONPATH
- ✅ `.env.example` - Template de variáveis de ambiente
- ✅ `README.md` - Documentação atualizada
- ✅ `CONTRIBUTING.md` - Guia atualizado

## Testes

Todos os testes passando:
```
✓ test_agente_move_para_trabalho
✓ test_cidade_snapshot
✓ test_agente_horarios_limites
✓ test_cidade_multiplos_agentes

4 passed in 0.01s
```

## Para Novos Contribuidores

Siga estas etapas após clonar o repositório:

```bash
# 1. Clone
git clone https://github.com/ferritine/ferritine.git
cd ferritine

# 2. Ambiente virtual
python -m venv .venv
source .venv/bin/activate  # Linux/macOS

# 3. Instale dependências
pip install -r requirements.txt

# 4. Instale em modo desenvolvimento (recomendado)
pip install -e .

# 5. Execute testes
python -m pytest -v
```

## Referências

- [pytest pythonpath configuration](https://docs.pytest.org/en/stable/reference/customize.html#pythonpath)
- [setuptools editable installs](https://setuptools.pypa.io/en/latest/userguide/development_mode.html)
- [Python sys.path](https://docs.python.org/3/library/sys.html#sys.path)

---

**Problema resolvido! ✅**

Os testes agora funcionam tanto localmente quanto no GitHub Actions.

