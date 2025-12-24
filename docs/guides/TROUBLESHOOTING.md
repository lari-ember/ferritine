# 🔍 Troubleshooting - Ferritine

Guia para resolver problemas comuns no projeto Ferritine.

## 🐍 Problemas de Import

### ❌ ModuleNotFoundError: No module named 'app'

**Sintomas:**
```python
from app.models.agente import Agente
ModuleNotFoundError: No module named 'app'
```

**Soluções:**

1. **Verifique se está usando `python -m pytest`** (não apenas `pytest`):
   ```bash
   python -m pytest -v  # ✓ Correto
   pytest -v            # ✗ Pode falhar
   ```

2. **Instale em modo desenvolvimento:**
   ```bash
   pip install -e .
   ```

3. **Verifique se pytest.ini existe:**
   ```bash
   ls -la pytest.ini
   cat pytest.ini  # Deve ter pythonpath = .
   ```

4. **Defina PYTHONPATH manualmente:**
   ```bash
   # Linux/macOS
   PYTHONPATH=. python -m pytest -v
   
   # Windows PowerShell
   $env:PYTHONPATH="."; python -m pytest -v
   ```

---

## 🧪 Problemas com Testes

### ❌ Testes não são encontrados

**Sintomas:**
```
collected 0 items
```

**Soluções:**

1. **Verifique a estrutura de diretórios:**
   ```bash
   app/
   └── tests/
       └── test_sim.py  # Deve começar com test_
   ```

2. **Verifique se __init__.py existe:**
   ```bash
   ls app/__init__.py
   ls app/tests/__init__.py  # Opcional mas recomendado
   ```

3. **Execute do diretório raiz:**
   ```bash
   cd /caminho/para/ferritine
   python -m pytest -v
   ```

### ❌ Import Error em testes

**Sintomas:**
```python
ImportError while importing test module
```

**Soluções:**

1. **Verifique se está no ambiente virtual:**
   ```bash
   which python  # Deve apontar para .venv/bin/python
   source .venv/bin/activate  # Se não estiver ativo
   ```

2. **Reinstale dependências:**
   ```bash
   pip install -r requirements.txt
   pip install -e .
   ```

---

## 📦 Problemas com Dependências

### ❌ pip install falha

**Sintomas:**
```
ERROR: Could not find a version that satisfies the requirement...
```

**Soluções:**

1. **Atualize pip:**
   ```bash
   python -m pip install --upgrade pip
   ```

2. **Verifique versão do Python:**
   ```bash
   python --version  # Deve ser 3.8+
   ```

3. **Limpe cache do pip:**
   ```bash
   pip cache purge
   pip install -r requirements.txt
   ```

---

## 🔧 Problemas com Ambiente Virtual

### ❌ Ambiente virtual não ativa

**Linux/macOS:**
```bash
source .venv/bin/activate
```

**Windows PowerShell:**
```powershell
.venv\Scripts\Activate.ps1

# Se houver erro de política de execução:
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

**Windows CMD:**
```cmd
.venv\Scripts\activate.bat
```

### ❌ Comando não encontrado após ativar venv

**Sintomas:**
```
python: command not found
```

**Soluções:**

1. **Verifique se o venv foi criado corretamente:**
   ```bash
   python -m venv .venv
   ```

2. **Use python3 explicitamente:**
   ```bash
   python3 -m venv .venv
   source .venv/bin/activate
   ```

---

## 🐙 Problemas com Git/GitHub

### ❌ Workflow do GitHub Actions falha

**Sintomas:**
```yaml
Error: Process completed with exit code 1
```

**Soluções:**

1. **Verifique logs no GitHub:**
   - Acesse Actions tab
   - Clique no workflow falhado
   - Expanda os logs para ver erro específico

2. **Verifique pytest.ini no repositório:**
   ```bash
   git ls-files | grep pytest.ini
   ```

3. **Verifique se requirements.txt está no repo:**
   ```bash
   git ls-files | grep requirements.txt
   ```

4. **Teste localmente primeiro:**
   ```bash
   # Simule ambiente limpo
   deactivate
   rm -rf .venv
   python -m venv .venv
   source .venv/bin/activate
   pip install -r requirements.txt
   python -m pytest -v
   ```

---

## 🔄 Problemas com Versionamento

### ❌ Script bump_version.py falha

**Sintomas:**
```
ERROR: VERSION must be MAJOR.MINOR.PATCH
```

**Soluções:**

1. **Verifique formato do VERSION:**
   ```bash
   cat VERSION  # Deve ser X.Y.Z (ex: 0.1.0)
   ```

2. **Corrija manualmente se necessário:**
   ```bash
   echo "0.1.0" > VERSION
   ```

3. **Execute com nível válido:**
   ```bash
   python .github/scripts/bump_version.py --level patch
   # Níveis válidos: patch, minor, major
   ```

---

## 💻 Problemas de Performance

### ❌ Testes muito lentos

**Soluções:**

1. **Execute em paralelo (futuro):**
   ```bash
   pip install pytest-xdist
   python -m pytest -n auto
   ```

2. **Execute apenas testes modificados:**
   ```bash
   python -m pytest tests/test_sim.py -v
   ```

3. **Desabilite cobertura:**
   ```bash
   python -m pytest -v  # Sem --cov
   ```

---

## 🔍 Debugging

### Como debugar imports

```python
# No topo do arquivo de teste
import sys
print("Python path:")
for p in sys.path:
    print(f"  - {p}")

# Tente importar manualmente
try:
    from app.models.agente import Agente
    print("✓ Import successful")
except ImportError as e:
    print(f"✗ Import failed: {e}")
```

### Como debugar pytest

```bash
# Modo verbose com traceback completo
python -m pytest -vvv --tb=long

# Com output de print statements
python -m pytest -vv -s

# Parar no primeiro erro
python -m pytest -x

# Debugger interativo
python -m pytest --pdb
```

### Como verificar configuração pytest

```bash
# Ver configuração atual
python -m pytest --version
python -m pytest --co  # Collect-only (mostra o que seria executado)

# Ver configuração completa
python -m pytest --showlocals -v
```

---

## 📚 Recursos Úteis

### Comandos de Diagnóstico

```bash
# Verificar Python
python --version
which python

# Verificar pip
pip --version
pip list

# Verificar pytest
python -m pytest --version

# Verificar estrutura
tree -L 3  # Se tree estiver instalado
ls -R app/

# Verificar git
git status
git log --oneline -5
```

### Logs Importantes

```bash
# Salvar output de testes
python -m pytest -v > test_output.txt 2>&1

# Ver últimas linhas de erro
python -m pytest -v 2>&1 | tail -20

# Buscar por erros
python -m pytest -v 2>&1 | grep -i error
```

---

## 🆘 Ainda Com Problemas?

1. **Abra uma issue:** https://github.com/ferritine/ferritine/issues
   - Use o template de bug report
   - Inclua output completo dos comandos
   - Mencione SO, versão do Python, etc.

2. **Verifique issues existentes:** Alguém pode já ter tido o mesmo problema

3. **Consulte a documentação:**
   - [README.md](../README.md)
   - [CONTRIBUTING.md](CONTRIBUTING.md)
   - [docs/PYTHONPATH_FIX.md](PYTHONPATH_FIX.md)

4. **Último recurso - Reset completo:**
   ```bash
   # CUIDADO: Isso apaga seu ambiente virtual
   deactivate
   rm -rf .venv
   python -m venv .venv
   source .venv/bin/activate
   pip install --upgrade pip
   pip install -r requirements.txt
   pip install -e .
   python -m pytest -v
   ```

---

## ✅ Checklist de Diagnóstico

Antes de abrir uma issue, verifique:

- [ ] Estou no diretório raiz do projeto?
- [ ] Ambiente virtual está ativado?
- [ ] Dependências foram instaladas?
- [ ] pytest.ini existe?
- [ ] Estou usando `python -m pytest` (não apenas `pytest`)?
- [ ] Versão do Python é 3.8+?
- [ ] Tentei `pip install -e .`?
- [ ] Tentei recriar o ambiente virtual?
- [ ] Consultei a documentação?
- [ ] Busquei por issues similares?

---

**Se nada funcionar, não hesite em pedir ajuda! 💪**

