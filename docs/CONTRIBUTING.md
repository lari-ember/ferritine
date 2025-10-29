# 🤝 Guia de Contribuição - Ferritine

Obrigado por considerar contribuir com o Ferritine! Este documento fornece diretrizes para contribuir com o projeto.

## 📋 Índice

- [Código de Conduta](#código-de-conduta)
- [Como Posso Contribuir?](#como-posso-contribuir)
- [Processo de Desenvolvimento](#processo-de-desenvolvimento)
- [Diretrizes de Código](#diretrizes-de-código)
- [Convenções de Commit](#convenções-de-commit)
- [Processo de Review](#processo-de-review)

## 🌟 Código de Conduta

Este projeto segue um código de conduta baseado no respeito mútuo. Esperamos que todos os contribuidores:

- Sejam respeitosos e inclusivos
- Aceitem críticas construtivas graciosamente
- Foquem no que é melhor para a comunidade
- Demonstrem empatia com outros membros da comunidade

## 🚀 Como Posso Contribuir?

### Reportando Bugs

Antes de criar um bug report:

1. **Verifique a documentação** - talvez já exista uma solução
2. **Procure por issues existentes** - alguém pode já ter reportado
3. **Use o template de bug report** - isso ajuda a entender o problema

Quando reportar um bug, inclua:

- Descrição clara e detalhada
- Passos específicos para reproduzir
- Comportamento esperado vs. atual
- Screenshots (se aplicável)
- Informações do ambiente (SO, versão do Python, etc.)

### Sugerindo Melhorias

Para sugerir novas funcionalidades:

1. **Verifique o roadmap** - pode já estar planejado
2. **Procure por feature requests similares**
3. **Use o template de feature request**

Descreva:

- O problema que a funcionalidade resolve
- Como você imagina a solução
- Alternativas que considerou

### Contribuindo com Código

1. **Fork o repositório**
2. **Clone seu fork localmente**
3. **Configure o ambiente de desenvolvimento**
4. **Crie uma branch para sua feature**
5. **Faça suas alterações**
6. **Execute os testes**
7. **Commit com mensagens descritivas**
8. **Push para seu fork**
9. **Abra um Pull Request**

## 💻 Processo de Desenvolvimento

### 1. Configuração do Ambiente

```bash
# Clone seu fork
git clone https://github.com/seu-usuario/ferritine.git
cd ferritine

# Adicione o repositório upstream
git remote add upstream https://github.com/ferritine/ferritine.git

# Crie um ambiente virtual
python -m venv .venv
source .venv/bin/activate  # Linux/macOS
# ou .venv\Scripts\Activate.ps1  # Windows

# Instale dependências de desenvolvimento
pip install -r requirements.txt
pip install pytest pytest-cov black flake8 mypy

# Instale o pacote em modo desenvolvimento (recomendado)
pip install -e .
```

### 2. Criando uma Branch

```bash
# Atualize sua branch main
git checkout main
git pull upstream main

# Crie uma branch descritiva
git checkout -b feature/nome-da-feature
# ou
git checkout -b fix/descricao-do-bug
```

### 3. Fazendo Alterações

- Escreva código limpo e legível
- Siga as diretrizes de estilo (PEP 8)
- Adicione testes para novas funcionalidades
- Atualize a documentação se necessário
- Mantenha commits pequenos e focados

### 4. Executando Testes

```bash
# Execute todos os testes
python -m pytest -v

# Execute com cobertura
python -m pytest --cov=app --cov-report=html

# Execute testes específicos
python -m pytest app/tests/test_sim.py -v

# Verifique o estilo do código
flake8 app/ --max-line-length=100

# Formate o código (opcional)
black app/
```

### 5. Commit e Push

```bash
# Adicione as mudanças
git add .

# Faça commit com mensagem descritiva
git commit -m "feat: adiciona funcionalidade X"

# Push para seu fork
git push origin feature/nome-da-feature
```

### 6. Abrindo um Pull Request

1. Vá para o repositório original no GitHub
2. Clique em "New Pull Request"
3. Selecione sua branch
4. Preencha o template de PR:
   - Descreva suas mudanças
   - Mencione issues relacionadas
   - Adicione screenshots se aplicável
   - Marque os checkboxes relevantes

## 📝 Diretrizes de Código

### Estilo Python (PEP 8)

```python
# ✅ BOM
def calcular_media(valores: list[float]) -> float:
    """
    Calcula a média de uma lista de valores.
    
    Args:
        valores: Lista de números para calcular a média.
        
    Returns:
        A média dos valores fornecidos.
    """
    return sum(valores) / len(valores)


# ❌ RUIM
def calc(v):
    return sum(v)/len(v)
```

### Docstrings

Todas as funções, classes e métodos públicos devem ter docstrings:

```python
class Agente:
    """
    Representa um habitante da cidade com rotinas diárias.
    
    Attributes:
        nome: Nome do agente.
        casa: Local onde o agente mora.
        trabalho: Local onde o agente trabalha.
    """
    
    def step(self, hora: int) -> None:
        """
        Atualiza o local do agente baseado na hora.
        
        Args:
            hora: Hora atual (0-23).
        """
        pass
```

### Type Hints

Use type hints sempre que possível:

```python
from typing import List, Dict, Optional

def processar_agentes(
    agentes: List[Agente],
    cidade: Cidade,
    config: Optional[Dict[str, any]] = None
) -> Dict[str, str]:
    """Processa agentes e retorna um snapshot."""
    pass
```

### Testes

Cada funcionalidade deve incluir testes:

```python
def test_agente_move_para_trabalho():
    """Testa se o agente se move corretamente para o trabalho."""
    # Arrange
    agente = Agente("Test", "Casa", "Trabalho")
    
    # Act
    agente.step(10)
    
    # Assert
    assert agente.local == "Trabalho"
```

## 📌 Convenções de Commit

Use [Conventional Commits](https://www.conventionalcommits.org/pt-br/):

### Tipos de Commit

- **feat**: Nova funcionalidade
- **fix**: Correção de bug
- **docs**: Mudanças na documentação
- **style**: Formatação, ponto e vírgula, etc (sem mudança de código)
- **refactor**: Refatoração de código
- **perf**: Melhoria de performance
- **test**: Adição ou correção de testes
- **chore**: Manutenção, dependências, etc

### Exemplos

```bash
feat: adiciona sistema de transporte público
fix: corrige bug na rotina de agentes
docs: atualiza README com exemplos de uso
test: adiciona testes para classe Cidade
refactor: simplifica lógica de movimento dos agentes
chore: atualiza dependências do projeto
```

### Mensagens Detalhadas

Para commits complexos, adicione corpo e rodapé:

```bash
git commit -m "feat: adiciona sistema de eventos

Implementa um sistema de eventos aleatórios que podem
afetar os agentes durante a simulação.

- Adiciona classe Event
- Implementa EventManager
- Atualiza Cidade para processar eventos

Closes #123"
```

## 🔍 Processo de Review

### Para Contribuidores

Após abrir um PR:

1. **Aguarde o review** - mantenedores irão revisar seu código
2. **Responda aos comentários** - seja receptivo a sugestões
3. **Faça as alterações solicitadas** - commit na mesma branch
4. **Aguarde aprovação** - podem ser necessárias várias iterações

### Para Reviewers

Ao revisar PRs:

1. **Seja construtivo** - critique o código, não a pessoa
2. **Seja específico** - dê exemplos de como melhorar
3. **Seja empático** - lembre-se que alguém dedicou tempo
4. **Reconheça o esforço** - agradeça pela contribuição

## ✅ Checklist Antes de Submeter

- [ ] O código segue as diretrizes de estilo (PEP 8)
- [ ] Adicionei docstrings para novas funções/classes
- [ ] Adicionei type hints quando apropriado
- [ ] Escrevi testes para novas funcionalidades
- [ ] Todos os testes passam localmente
- [ ] Atualizei a documentação (se necessário)
- [ ] Usei mensagens de commit descritivas
- [ ] Revisei meu próprio código antes de submeter

## 🆘 Precisa de Ajuda?

Se tiver dúvidas:

- Abra uma [issue](https://github.com/ferritine/ferritine/issues) com a tag "question"
- Participe das [discussões](https://github.com/ferritine/ferritine/discussions)
- Leia a [documentação](README.md)

## 🙏 Agradecimentos

Obrigado por contribuir com o Ferritine! Sua ajuda torna este projeto melhor para todos.

---

**Lembre-se:** Toda contribuição, grande ou pequena, é valiosa e apreciada! ❤️

