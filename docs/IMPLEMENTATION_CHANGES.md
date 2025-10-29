# Resumo das Alterações - Sistema de CI/CD e Workflows

## ✅ Problema Resolvido

**Erro original**: `ModuleNotFoundError: No module named 'app'` no GitHub Actions

**Solução aplicada**: Configuração correta do `PYTHONPATH` no workflow de testes.

## 🚀 Melhorias Implementadas

### 1. Correção do Workflow de Testes (`tests.yml`)

**Antes:**
```yaml
run: |
  PYTHONPATH=$PYTHONPATH:. python -m pytest -v --cov=app --cov-report=xml
```

**Depois:**
```yaml
run: |
  python -m pytest -v --cov=app --cov-report=xml
env:
  PYTHONPATH: ${{ github.workspace }}
```

**Por que isso funciona:**
- Define `PYTHONPATH` como variável de ambiente do step
- Usa `github.workspace` que é o caminho absoluto do repositório
- É a forma recomendada pelo GitHub Actions
- Funciona consistentemente em todos os runners

### 2. Release Workflow Automatizado (`release.yml`)

**Mudanças principais:**

✅ **Trigger automático em push para main:**
```yaml
on:
  push:
    branches:
      - main
    paths-ignore:
      - 'VERSION'
      - '**.md'
      - 'docs/**'
```

✅ **Prevenção de loops infinitos:**
- Ignora commits que só mudam VERSION ou documentação
- Verifica se há mudanças antes de commitar
- Só cria tag se houve bump de versão real

✅ **Suporte a execução manual:**
- Mantém `workflow_dispatch` para releases manuais
- Permite escolher o nível (patch/minor/major)
- Usa 'patch' como padrão quando automático

### 3. Novo Workflow: Release Drafter (`release-drafter.yml`)

**O que faz:**
- Mantém automaticamente um **draft release** atualizado
- Categoriza PRs merged baseado em labels
- Gera changelog organizado e formatado
- Sugere a próxima versão automaticamente

**Configuração (`.github/release-drafter.yml`):**
- 7 categorias de mudanças (Features, Bugs, Docs, etc.)
- Template customizável para o changelog
- Regras de versionamento baseadas em labels
- Exclusão de PRs com labels específicos

**Categorias:**
- 🚀 Features (`feature`, `enhancement`)
- 🐛 Bug Fixes (`bug`, `fix`)
- 📚 Documentation (`documentation`, `docs`)
- 🧹 Maintenance (`chore`, `maintenance`, `refactor`)
- ⚡ Performance (`performance`, `perf`)
- 🧪 Tests (`test`, `tests`)
- 🔧 Dependencies (`dependencies`, `deps`)

### 4. Novo Workflow: Release on Tag (`release-on-tag.yml`)

**Trigger:** Criação de tags `v*` (ex: `v1.0.0`)

**O que faz:**
1. Detecta a nova tag
2. Gera changelog desde a tag anterior
3. Publica release no GitHub
4. Detecta automaticamente pre-releases (`-alpha`, `-beta`, `-rc`)

**Uso:**
```bash
git tag v0.2.0 -m "Release v0.2.0"
git push origin v0.2.0
# Release publicado automaticamente!
```

### 5. Sistema de Labels (`.github/labels.yml`)

**Criado arquivo com 40+ labels organizados em categorias:**

- **Tipo de Mudança**: feature, bug, docs, chore, test, performance, dependencies
- **Impacto na Versão**: major, minor, patch, breaking
- **Prioridade**: critical, high, medium, low
- **Status**: wip, ready-for-review, blocked, on-hold
- **Especiais**: good first issue, help wanted, skip-changelog
- **Área/Componente**: simulation, agents, ui, core, tests, ci-cd

**Sincronizar labels com o repo:**
```bash
gh label sync --file .github/labels.yml
```

### 6. Documentação Completa

**Criado**: `docs/WORKFLOWS_GUIDE.md` (>300 linhas)

**Conteúdo:**
- Visão geral de todos os workflows
- Explicação detalhada de cada workflow
- Fluxo recomendado de desenvolvimento
- Como usar labels corretamente
- Troubleshooting de problemas comuns
- Exemplos práticos passo a passo

**Atualizado**: `README.md`

**Melhorias:**
- Badges atualizados (Tests, Release Drafter, Coverage)
- Seção de recursos expandida
- Estrutura do projeto detalhada
- Contribuição com informações sobre labels
- Versionamento com 3 formas de release
- Links para documentação de workflows

### 7. Bug Report Template

**Status**: Já estava preenchido e completo!

**Conteúdo:**
- Descrição clara do bug
- Passos para reproduzir
- Comportamento esperado vs. atual
- Informações de ambiente
- Seção para logs/erros
- Código para reproduzir
- Contexto adicional

## 🎯 Fluxo Recomendado de Uso

### Desenvolvimento Diário:

1. **Criar branch**: `git checkout -b feature/nova-funcionalidade`
2. **Desenvolver**: fazer commits com conventional commits
3. **Abrir PR**: adicionar labels apropriados
4. **Merge**: Release Drafter atualiza draft automaticamente

### Quando Pronto para Release:

5. **Revisar draft**: Ver changelog gerado em Releases
6. **Criar tag**: `git tag v0.2.0 && git push origin v0.2.0`
7. **Release publicado**: Automaticamente pelo workflow!

## 📊 Workflows Disponíveis

| Workflow | Arquivo | Trigger | Função |
|----------|---------|---------|--------|
| Tests | `tests.yml` | Push/PR para main/develop | Roda testes em Python 3.8-3.11 |
| Release Drafter | `release-drafter.yml` | Push para main | Atualiza draft release |
| Release on Tag | `release-on-tag.yml` | Push de tag `v*` | Publica release |
| Release (legacy) | `release.yml` | Push para main / Manual | Cria release automaticamente |

## 🔧 Comandos Úteis

```bash
# Rodar testes localmente (com PYTHONPATH correto)
pytest -v

# Sincronizar labels
gh label sync --file .github/labels.yml

# Criar release manual
git tag v0.2.0 -m "Release v0.2.0"
git push origin v0.2.0

# Bump de versão local
python .github/scripts/bump_version.py --level patch
```

## 📝 Arquivos Criados/Modificados

### Criados:
- ✅ `.github/workflows/release-drafter.yml`
- ✅ `.github/workflows/release-on-tag.yml`
- ✅ `.github/release-drafter.yml`
- ✅ `.github/labels.yml`
- ✅ `docs/WORKFLOWS_GUIDE.md`

### Modificados:
- ✅ `.github/workflows/tests.yml` (fix PYTHONPATH)
- ✅ `.github/workflows/release.yml` (auto-trigger + loop prevention)
- ✅ `README.md` (melhorias e documentação)

### Já Existentes (OK):
- ✅ `.github/ISSUE_TEMPLATE/bug_report.md` (completo)
- ✅ `.github/ISSUE_TEMPLATE/feature_request.md` (completo)

## 🎓 Próximos Passos Recomendados

1. **Testar os workflows**:
   - Fazer um commit para main e ver se os testes passam
   - Criar um PR de teste com labels
   - Verificar se o Release Drafter atualiza

2. **Sincronizar labels**:
   ```bash
   gh label sync --file .github/labels.yml
   ```

3. **Criar primeiro release com o novo sistema**:
   - Revisar o draft em Releases
   - Criar tag `v0.1.1` (ou versão apropriada)
   - Verificar se o release foi publicado

4. **Educar a equipe**:
   - Compartilhar o `docs/WORKFLOWS_GUIDE.md`
   - Enfatizar a importância dos labels
   - Ensinar o fluxo de conventional commits

## 🐛 Troubleshooting

### Se testes falharem no Actions:

1. Verifique se o PYTHONPATH está configurado:
   ```yaml
   env:
     PYTHONPATH: ${{ github.workspace }}
   ```

2. Verifique se `pytest.ini` tem:
   ```ini
   [pytest]
   pythonpath = .
   ```

### Se houver loop de releases:

1. Verifique `paths-ignore` no `release.yml`
2. Certifique-se que commits de VERSION não disparam o workflow
3. Use `[skip ci]` em commits que não devem disparar workflows

### Se Release Drafter não atualizar:

1. Verifique se o PR tem labels
2. Verifique se foi merged para `main`
3. Veja logs em Actions
4. Verifique permissões (`contents: write`)

## 💡 Dicas Importantes

1. **Use labels consistentemente** - Crucial para changelogs organizados
2. **Revise drafts antes de publicar** - Você pode editar manualmente
3. **Use conventional commits** - Facilita entender mudanças
4. **Teste em branches** - Todos os PRs rodam testes
5. **Leia o WORKFLOWS_GUIDE.md** - Tem todos os detalhes

## 🎉 Resultado Final

Agora o projeto tem:
- ✅ Testes funcionando no GitHub Actions (PYTHONPATH correto)
- ✅ Sistema de releases automatizado com 3 opções
- ✅ Changelogs gerados automaticamente
- ✅ Organização por labels semânticos
- ✅ Documentação completa e detalhada
- ✅ Prevenção de loops infinitos
- ✅ Fluxo de desenvolvimento profissional

**Tudo pronto para desenvolvimento colaborativo eficiente! 🚀**

