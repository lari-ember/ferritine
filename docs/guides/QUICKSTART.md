# 🚀 Quick Start - Ferritine

Guia rápido para começar a usar o Ferritine em 5 minutos!

## ⚡ Instalação Rápida

```bash
# 1. Clone o repositório
git clone https://github.com/ferritine/ferritine.git
cd ferritine

# 2. Crie ambiente virtual
python -m venv .venv
source .venv/bin/activate  # Linux/macOS
# ou .venv\Scripts\Activate.ps1  # Windows

# 3. Instale dependências
pip install -r requirements.txt

# 4. Execute!
python main.py
```

## 🎮 Uso Básico

### Executar Demo
```bash
python main.py
```

### Executar Testes
```bash
python -m pytest -v
```

### Ver Cobertura
```bash
python -m pytest --cov=app --cov-report=html
# Abra htmlcov/index.html no navegador
```

## 🔧 Para Desenvolvedores

### Atualizar Versão
```bash
# Patch: 0.1.0 -> 0.1.1
python .github/scripts/bump_version.py --level patch

# Minor: 0.1.0 -> 0.2.0
python .github/scripts/bump_version.py --level minor

# Major: 0.1.0 -> 1.0.0
python .github/scripts/bump_version.py --level major
```

### Criar Nova Funcionalidade
```bash
# 1. Crie uma branch
git checkout -b feature/minha-feature

# 2. Faça suas alterações
# ... edite os arquivos ...

# 3. Adicione testes
# ... edite app/tests/test_*.py ...

# 4. Execute testes
python -m pytest -v

# 5. Commit
git add .
git commit -m "feat: adiciona minha nova feature"

# 6. Push
git push origin feature/minha-feature

# 7. Abra PR no GitHub
```

## 📝 Exemplo de Código

```python
from app.models.agente import Agente
from app.models.cidade import Cidade

# Criar cidade
cidade = Cidade()

# Adicionar agentes
cidade.add_agente(Agente("Ana", "Casa1", "Escola"))
cidade.add_agente(Agente("João", "Casa2", "Trabalho"))

# Simular
for hora in range(24):
    cidade.step(hora)
    print(f"{hora:02d}h -> {cidade.snapshot()}")
```

## 🆘 Ajuda Rápida

| Preciso... | Faço... |
|------------|---------|
| Reportar bug | Issues → Bug Report |
| Sugerir feature | Issues → Feature Request |
| Contribuir | Ver CONTRIBUTING.md |
| Ver documentação | Ver README.md |
| Atualizar versão | `bump_version.py --level patch` |
| Executar testes | `python -m pytest -v` |

## 🔗 Links Úteis

- [README Completo](README.md)
- [Guia de Contribuição](CONTRIBUTING.md)
- [Changelog](CHANGELOG.md)
- [Licença](LICENSE)

## 💡 Dicas

1. **Sempre ative o ambiente virtual** antes de trabalhar
2. **Execute os testes** antes de fazer commit
3. **Use Conventional Commits** nas mensagens
4. **Leia o CONTRIBUTING.md** antes de contribuir
5. **Mantenha o código PEP 8** compatível

---

**Pronto para começar! 🚀**

Para mais detalhes, veja o [README.md](README.md) completo.

