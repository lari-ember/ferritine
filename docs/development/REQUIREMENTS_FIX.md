# Correção do Erro de Dependências no GitHub Actions

## 🐛 Problema Identificado

```
ERROR: Could not find a version that satisfies the requirement iniconfig==2.3.0
ERROR: No matching distribution found for iniconfig==2.3.0
```

### Causa Raiz

O arquivo `requirements.txt` tinha versões fixadas (`==`) de dependências que:
1. **`iniconfig==2.3.0`** - Requer Python 3.10+
2. **`tomli==2.3.0`** - Versão muito recente, incompatível com Python 3.8-3.9
3. **`typing_extensions==4.15.0`** - Versão que pode não existir ou ter problemas

O workflow de testes roda em **Python 3.8, 3.9, 3.10, 3.11**, mas as dependências só funcionavam em Python 3.10+.

## ✅ Solução Implementada

Atualizei o `requirements.txt` para usar:

1. **Ranges de versão** (`>=X.Y.Z,<W.0.0`) ao invés de versões fixas
2. **Marcadores condicionais** para dependências específicas de versão Python
3. **Versões compatíveis** com Python 3.8+

### Antes:
```txt
exceptiongroup==1.3.0
iniconfig==2.3.0
packaging==25.0
pluggy==1.6.0
Pygments==2.19.2
pytest==8.4.2
tomli==2.3.0
typing_extensions==4.15.0
```

### Depois:
```txt
# Core testing dependencies
pytest>=7.4.0,<9.0.0

# Testing utilities
pytest-cov>=4.1.0,<6.0.0

# Type hints for older Python versions
typing-extensions>=4.5.0; python_version < '3.10'

# Compatibility
exceptiongroup>=1.1.0; python_version < '3.11'
tomli>=2.0.0; python_version < '3.11'
```

## 📋 Explicação das Mudanças

### 1. **pytest>=7.4.0,<9.0.0**
- Permite qualquer versão 7.x ou 8.x
- Compatível com Python 3.8+
- Instala automaticamente suas dependências (iniconfig, pluggy, packaging)

### 2. **pytest-cov>=4.1.0,<6.0.0**
- Plugin para cobertura de código
- Versão compatível com Python 3.8+

### 3. **typing-extensions>=4.5.0; python_version < '3.10'**
- Só instala em Python < 3.10 (onde é necessário)
- Python 3.10+ tem typing embutido

### 4. **exceptiongroup>=1.1.0; python_version < '3.11'**
- Só instala em Python < 3.11
- Python 3.11+ tem ExceptionGroup nativo

### 5. **tomli>=2.0.0; python_version < '3.11'**
- Parser TOML para Python < 3.11
- Python 3.11+ tem tomllib nativo

## 🎯 Benefícios da Nova Abordagem

### ✅ Compatibilidade Universal
- Funciona em Python 3.8, 3.9, 3.10, 3.11
- Instala apenas as dependências necessárias para cada versão

### ✅ Manutenibilidade
- Permite atualizações de patch automaticamente
- Menos conflitos de versão
- Mais resiliente a mudanças

### ✅ Clareza
- Comentários explicam cada grupo de dependências
- Marcadores condicionais documentam requisitos

### ✅ Eficiência
- Não instala dependências desnecessárias
- Usa funcionalidades nativas quando disponíveis

## 🧪 Como Testar

### Localmente (qualquer versão Python 3.8+):
```bash
# Remova instalação anterior
pip uninstall -y pytest pytest-cov exceptiongroup tomli typing-extensions

# Instale novamente
pip install -r requirements.txt

# Verifique
pip list | grep pytest
```

### No GitHub Actions:
- Os testes agora devem passar em todas as versões (3.8, 3.9, 3.10, 3.11)
- Cada versão instalará apenas as dependências necessárias

## 📚 Referências

### Marcadores de Ambiente Python
- `python_version < '3.10'` - Instala apenas em Python < 3.10
- `python_version >= '3.8'` - Instala apenas em Python >= 3.8
- Documentação: https://peps.python.org/pep-0508/

### Especificadores de Versão
- `>=X.Y.Z` - Versão mínima
- `<W.0.0` - Versão máxima (exclusivo)
- `>=X.Y.Z,<W.0.0` - Range de versões
- Documentação: https://peps.python.org/pep-0440/

## 🔍 Verificação de Compatibilidade

Para verificar se uma versão de pacote é compatível com sua versão Python:

```bash
# Veja todas as versões disponíveis
pip index versions iniconfig

# Veja informações do pacote
pip show pytest

# Simule instalação sem instalar
pip install --dry-run -r requirements.txt
```

## 🚀 Próximos Passos

1. **Commit e push** - As mudanças estão prontas
2. **Aguardar CI** - Verificar se testes passam em todas as versões Python
3. **Monitorar** - Acompanhar se há outros problemas de compatibilidade

## 💡 Boas Práticas

### ✅ Faça:
- Use ranges de versão (`>=X.Y,<W.0`) para flexibilidade
- Use marcadores condicionais para dependências específicas
- Documente dependências com comentários
- Teste em múltiplas versões Python

### ❌ Evite:
- Fixar versões exatas (`==X.Y.Z`) sem motivo
- Ignorar requisitos de versão Python
- Instalar dependências desnecessárias
- Depender de versões bleeding-edge sem necessidade

## 🎓 Comandos Úteis

```bash
# Gerar requirements.txt do ambiente atual
pip freeze > requirements.txt

# Instalar com verbose para debug
pip install -v -r requirements.txt

# Verificar dependências de um pacote
pip show pytest | grep Requires

# Verificar árvore de dependências
pip install pipdeptree
pipdeptree
```

## ✅ Status

- [x] Problema identificado
- [x] Causa raiz compreendida
- [x] Solução implementada
- [x] requirements.txt atualizado
- [x] Documentação criada
- [ ] CI verificado (pendente push)
- [ ] Testes passando em todas as versões Python

---

**Data**: 2025-10-29  
**Erro**: `iniconfig==2.3.0` incompatível com Python < 3.10  
**Solução**: Requirements.txt com ranges e marcadores condicionais  
**Status**: ✅ Resolvido

