# Workflows do GitHub Actions - Guia Completo

Este documento explica todos os workflows automatizados do projeto Ferritine e como usá-los.

## 📋 Visão Geral dos Workflows

O projeto possui 4 workflows principais:

1. **Tests** (`tests.yml`) - Executa testes automatizados
2. **Release Drafter** (`release-drafter.yml`) - Mantém draft de releases atualizado
3. **Release on Tag** (`release-on-tag.yml`) - Publica release quando tag é criada
4. **Release (bump & create)** (`release.yml`) - Cria release automaticamente (legado/alternativo)

## 🧪 1. Tests Workflow

**Arquivo**: `.github/workflows/tests.yml`

### Quando executa:
- Push para `main` ou `develop`
- Pull Requests para `main` ou `develop`

### O que faz:
- Testa o código em múltiplas versões do Python (3.8, 3.9, 3.10, 3.11)
- Gera relatório de cobertura de código
- Envia cobertura para Codecov

### Correção aplicada:
O workflow agora define corretamente o `PYTHONPATH` para evitar `ModuleNotFoundError`:

```yaml
env:
  PYTHONPATH: ${{ github.workspace }}
```

Isso garante que o pytest encontre o módulo `app` mesmo no ambiente limpo do GitHub Actions.

## 📝 2. Release Drafter Workflow (RECOMENDADO)

**Arquivo**: `.github/workflows/release-drafter.yml`  
**Configuração**: `.github/release-drafter.yml`

### Quando executa:
- Automaticamente quando um PR é merged para `main`
- Pode ser executado manualmente via Actions

### O que faz:
1. Lê os PRs merged desde o último release
2. Categoriza mudanças baseado nos labels dos PRs
3. Atualiza o **draft release** no GitHub
4. Sugere a próxima versão (major/minor/patch) baseado nos labels

### Como usar:

#### Passo 1: Label seus PRs
Ao criar um PR, adicione um label apropriado:

- **Features**: `feature`, `enhancement`
- **Bugs**: `bug`, `fix`
- **Docs**: `documentation`, `docs`
- **Manutenção**: `chore`, `maintenance`, `refactor`
- **Performance**: `performance`, `perf`
- **Testes**: `test`, `tests`
- **Dependências**: `dependencies`, `deps`

#### Passo 2: Merge o PR
Quando o PR é merged para `main`, o Release Drafter:
- Adiciona o PR ao draft da próxima release
- Organiza por categoria
- Atualiza a versão sugerida

#### Passo 3: Revise o Draft
1. Vá em **Releases** no GitHub
2. Verá um **Draft release** com todas as mudanças
3. Revise o conteúdo, edite se necessário

#### Passo 4: Publique (quando pronto)
**Opção A - Manual**:
1. Clique em "Edit" no draft release
2. Confirme a versão (ex: `v0.2.0`)
3. Clique em "Publish release"

**Opção B - Com Tag** (RECOMENDADO):
1. Crie a tag localmente:
   ```bash
   git tag v0.2.0
   git push origin v0.2.0
   ```
2. O workflow `release-on-tag.yml` publica automaticamente

### Categorias do Changelog

O Release Drafter organiza mudanças assim:

```markdown
## 🚀 Features
- Nova funcionalidade X @usuario (#123)

## 🐛 Bug Fixes
- Correção do bug Y @usuario (#124)

## 📚 Documentation
- Melhoria na documentação @usuario (#125)

## 🧹 Maintenance
- Refatoração do código Z @usuario (#126)
```

### Versionamento Automático

O Release Drafter sugere a versão baseado nos labels:

- **Major** (`v2.0.0`): PRs com label `major` ou `breaking`
- **Minor** (`v1.1.0`): PRs com label `minor`, `feature`, `enhancement`
- **Patch** (`v1.0.1`): PRs com label `patch`, `bug`, `fix`, `chore`, `dependencies`

## 🏷️ 3. Release on Tag Workflow (RECOMENDADO)

**Arquivo**: `.github/workflows/release-on-tag.yml`

### Quando executa:
- Quando uma tag `v*` é criada (ex: `v1.0.0`, `v0.2.1`)

### O que faz:
1. Detecta a nova tag
2. Gera changelog desde a tag anterior
3. Publica o release no GitHub (ou atualiza draft existente)
4. Marca como pre-release se contém `-alpha`, `-beta`, `-rc`

### Como usar:

```bash
# 1. Certifique-se que está em main e atualizado
git checkout main
git pull origin main

# 2. Crie a tag com a versão desejada
git tag v0.2.0 -m "Release v0.2.0"

# 3. Faça push da tag
git push origin v0.2.0

# 4. O workflow publica a release automaticamente!
```

### Pre-releases:
Para criar uma pre-release:
```bash
git tag v1.0.0-beta.1 -m "Beta release"
git push origin v1.0.0-beta.1
```

O workflow detecta automaticamente e marca como pre-release.

## 🔄 4. Release (bump & create) - Alternativo

**Arquivo**: `.github/workflows/release.yml`

Este é o workflow legado que pode funcionar de duas formas:

### Modo 1: Automático (push para main)
- Dispara automaticamente quando há push para `main`
- Incrementa versão como `patch` por padrão
- Cria tag e release automaticamente

**⚠️ Atenção**: Este modo pode criar releases em todo merge para main. Útil para projetos que querem release a cada merge.

### Modo 2: Manual (workflow_dispatch)
1. Vá em **Actions** no GitHub
2. Selecione "Release (bump & create)"
3. Clique em "Run workflow"
4. Escolha o nível: `patch`, `minor` ou `major`
5. Confirme

### Configuração atual:
- Ignora mudanças em `VERSION`, `*.md`, `docs/**` para evitar loops
- Só cria release se houver mudanças reais

## 🎯 Fluxo Recomendado (Best Practice)

Para o melhor resultado, use este fluxo:

### Desenvolvimento Diário:

1. **Crie uma branch** para sua feature/fix:
   ```bash
   git checkout -b feature/nova-funcionalidade
   ```

2. **Desenvolva e commite**:
   ```bash
   git add .
   git commit -m "feat: adiciona nova funcionalidade"
   git push origin feature/nova-funcionalidade
   ```

3. **Abra um PR**:
   - No GitHub, abra PR da sua branch para `main`
   - **Adicione labels apropriados** (`feature`, `bug`, etc.)
   - Aguarde os testes passarem
   - Peça review se necessário

4. **Merge o PR**:
   - Quando aprovado, faça merge do PR
   - Release Drafter automaticamente atualiza o draft

### Quando Pronto para Release:

5. **Revise o Draft**:
   - Vá em **Releases** no GitHub
   - Veja o draft com todas as mudanças desde último release
   - Edite se necessário (título, descrição, etc.)

6. **Crie a Tag**:
   ```bash
   git checkout main
   git pull origin main
   git tag v0.2.0 -m "Release v0.2.0"
   git push origin v0.2.0
   ```

7. **Release Publicado**:
   - O workflow `release-on-tag.yml` publica automaticamente
   - A release fica visível para todos
   - Changelog organizado e formatado

## 🏷️ Labels Disponíveis

Para sincronizar os labels com o repositório:

```bash
# Instale o GitHub CLI se não tiver
# https://cli.github.com/

# Sincronize os labels
gh label sync --file .github/labels.yml
```

### Labels de Tipo:
- `feature`, `enhancement` - Novas funcionalidades
- `bug`, `fix` - Correções
- `documentation`, `docs` - Documentação
- `chore`, `maintenance`, `refactor` - Manutenção
- `test` - Testes
- `performance`, `perf` - Performance
- `dependencies` - Dependências

### Labels de Versão:
- `breaking`, `major` - Mudanças breaking (v2.0.0)
- `minor` - Nova funcionalidade (v1.1.0)
- `patch` - Correção (v1.0.1)

### Labels de Prioridade:
- `priority:critical` - Urgente
- `priority:high` - Alta
- `priority:medium` - Média
- `priority:low` - Baixa

### Labels de Status:
- `wip` - Work in progress
- `ready-for-review` - Pronto para revisão
- `blocked` - Bloqueado
- `on-hold` - Em espera

### Labels Especiais:
- `good first issue` - Para iniciantes
- `help wanted` - Precisa de ajuda
- `skip-changelog` - Não incluir no changelog

## 🔧 Troubleshooting

### Testes falhando com `ModuleNotFoundError: No module named 'app'`

**Solução**: Já corrigido! O workflow agora define `PYTHONPATH` corretamente.

Se ainda tiver problemas, verifique:
```yaml
env:
  PYTHONPATH: ${{ github.workspace }}
```

### Loop infinito de releases

**Causa**: O workflow de release faz commit e push, que dispara o workflow novamente.

**Solução**: O workflow agora:
- Ignora mudanças em `VERSION` com `paths-ignore`
- Verifica se há mudanças antes de commitar
- Só cria tag se versão foi alterada

### Release Drafter não atualiza

**Verifique**:
1. O PR tem labels?
2. O PR foi merged para `main`?
3. O workflow `release-drafter.yml` executou? (veja Actions)
4. Permissões estão corretas? (precisa `contents: write`)

### Tag não publica release

**Verifique**:
1. Tag começa com `v`? (ex: `v1.0.0`, não `1.0.0`)
2. O workflow `release-on-tag.yml` existe?
3. Veja logs em Actions para erros

## 📚 Referências

- [Release Drafter Documentation](https://github.com/release-drafter/release-drafter)
- [Semantic Versioning](https://semver.org/)
- [GitHub Actions Documentation](https://docs.github.com/actions)
- [Conventional Commits](https://www.conventionalcommits.org/)

## 💡 Dicas

1. **Use labels consistentemente** - Isso garante changelogs organizados
2. **Escreva boas descrições de PR** - Elas aparecem no changelog
3. **Revise drafts antes de publicar** - Você pode editar manualmente
4. **Use Conventional Commits** - Facilita entender as mudanças
5. **Teste em branches** - Todos os PRs rodam testes automaticamente

## 🎓 Exemplos Práticos

### Exemplo 1: Adicionar uma feature

```bash
# Crie branch
git checkout -b feature/add-visualization

# Desenvolva
# ... código ...

# Commit
git commit -m "feat: add data visualization module"

# Push e abra PR
git push origin feature/add-visualization
```

No PR, adicione label `feature`. Quando merged, aparece em "🚀 Features".

### Exemplo 2: Corrigir um bug

```bash
git checkout -b fix/agent-movement-bug
# ... correção ...
git commit -m "fix: correct agent movement calculation"
git push origin fix/agent-movement-bug
```

No PR, adicione label `bug`. Quando merged, aparece em "🐛 Bug Fixes".

### Exemplo 3: Publicar release

```bash
# Depois de vários PRs merged
git checkout main
git pull

# Veja o draft em https://github.com/USER/REPO/releases
# Quando satisfeito:

git tag v0.3.0 -m "Release v0.3.0"
git push origin v0.3.0

# Release publicado automaticamente! 🎉
```

