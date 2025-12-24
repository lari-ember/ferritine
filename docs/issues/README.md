# 📋 Planejamento de Issues Futuras

Este diretório contém documentação detalhada de issues e features planejadas para o futuro, mas que **NÃO devem ser implementadas agora**.

## 📁 Conteúdo

### 🎵 Sistema de Áudio

Planejamento completo do sistema de áudio em 3 níveis:

| Arquivo | Descrição | Tempo Estimado | Prioridade |
|---------|-----------|----------------|------------|
| **[AUDIO_SUMMARY.md](./AUDIO_SUMMARY.md)** | ⚡ Resumo executivo - comece aqui! | 5 min leitura | ⭐⭐⭐ |
| **[AUDIO_ROADMAP.md](./AUDIO_ROADMAP.md)** | 🗺️ Roadmap completo com timeline | 15 min leitura | ⭐⭐⭐ |
| **[AUDIO_LEVEL_2_FUTURE.md](./AUDIO_LEVEL_2_FUTURE.md)** | 🔮 Nível 2: Futuro próximo (3-4 semanas) | 30 min leitura | ⭐⭐ |
| **[AUDIO_LEVEL_3_FUTURE.md](./AUDIO_LEVEL_3_FUTURE.md)** | 🔮 Nível 3: Avançado (2-3 meses) | 45 min leitura | ⭐ |

---

## 🎵 Sistema de Áudio - Visão Geral

### Estado Atual (Nível 1) ✅
**Implementado em**: `ferritineVU/Assets/Scripts/Audio/AudioManager.cs`

- Singleton pattern
- Pool de AudioSource
- 4 canais (UI, SFX, Music, Ambient)
- Volume básico por categoria
- Áudio espacial básico

### Nível 2 - Futuro Próximo 🔮
**Quando**: 3-4 semanas de dev  
**Prioridade**: Média a Alta

1. **AudioMixer Completo** (3-5 dias)
   - Hierarquia de grupos
   - Efeitos: Reverb, Filters, Echo
   - Snapshots
   
2. **UI de Controle** (4-6 dias)
   - Sliders de volume
   - Botões de mute
   - VU meters
   
3. **Som Ambiente por Zona** (5-7 dias)
   - Zonas com triggers
   - Transições suaves
   - Variação dia/noite

### Nível 3 - Avançado 🔮
**Quando**: 2-3 meses de dev  
**Prioridade**: Baixa (polish final)

1. **Áudio 3D Real** (2-3 semanas)
   - HRTF e binaural
   - Doppler effect
   
2. **Oclusão** (3-4 semanas)
   - Raycasting
   - Filtros por material
   
3. **Prioridade e Culling** (2-3 semanas)
   - Sistema de prioridades
   - Voice stealing
   
4. **Performance Tuning** (2-3 semanas)
   - Streaming
   - Otimização de memória

---

## 🚦 Como Usar Esta Documentação

### Para Desenvolvedores

#### Se você vai implementar áudio AGORA:
1. ❌ **NÃO** leia os documentos deste diretório
2. ✅ Use o `AudioManager.cs` atual
3. ✅ Ele é suficiente para desenvolvimento básico

#### Se você está planejando o futuro:
1. ✅ Comece com [AUDIO_SUMMARY.md](./AUDIO_SUMMARY.md) (5 min)
2. ✅ Leia [AUDIO_ROADMAP.md](./AUDIO_ROADMAP.md) para visão geral
3. ✅ Aprofunde no nível que vai implementar:
   - Nível 2: [AUDIO_LEVEL_2_FUTURE.md](./AUDIO_LEVEL_2_FUTURE.md)
   - Nível 3: [AUDIO_LEVEL_3_FUTURE.md](./AUDIO_LEVEL_3_FUTURE.md)

### Para Product Owners / Managers

#### Pergunta: "Quando adicionar feature X de áudio?"

**Consulte**: [AUDIO_ROADMAP.md](./AUDIO_ROADMAP.md) - Seção "Roadmap de Desenvolvimento"

**Resposta Rápida**:
- AudioMixer/UI/Zonas → Nível 2 (3-4 semanas)
- HRTF/Oclusão/Prioridade → Nível 3 (2-3 meses)

#### Pergunta: "Qual a prioridade?"

**Consulte**: [AUDIO_SUMMARY.md](./AUDIO_SUMMARY.md) - Tabela de comparação

**Resposta Rápida**:
- Nível 2: Média a Alta (melhora UX)
- Nível 3: Baixa (polish final)

---

## 📝 Como Criar Issues no GitHub

Quando for hora de implementar, use este template:

### Template para Nível 2

```markdown
---
name: Audio System - Level 2 - Issue X
about: Implementar feature X do sistema de áudio (Nível 2)
title: '[AUDIO-L2-X] Nome da Feature'
labels: enhancement, audio, level-2, future
---

## Referência
Baseado em: `docs/issues/AUDIO_LEVEL_2_FUTURE.md` - Issue X

## Descrição
[Copiar descrição do documento]

## Critérios de Aceitação
[Copiar do documento]

## Estimativa
[Do documento]

## Dependências
[Do documento]

## Documentação
- [ ] Atualizar AUDIO_ROADMAP.md com status
- [ ] Documentar código implementado
- [ ] Atualizar README se necessário
```

### Template para Nível 3

Similar ao Nível 2, mas usar `level-3` na label.

---

## ⚠️ IMPORTANTE: O Que NÃO Fazer

### 🚫 NÃO Implementar Agora

Estes documentos são **planejamento futuro**. As features descritas **NÃO devem ser implementadas agora** porque:

1. O sistema atual é suficiente para desenvolvimento básico
2. Essas features exigem tempo significativo (semanas/meses)
3. Há prioridades mais importantes no projeto
4. Podem adicionar complexidade desnecessária

### ✅ O Que Fazer Agora

1. Use o `AudioManager.cs` atual
2. Adicione sons básicos ao jogo
3. Foque em gameplay e features core
4. **Quando** chegar a hora de melhorar áudio:
   - Consulte estes documentos
   - Crie issues específicas
   - Implemente incrementalmente

---

## 🔗 Links Úteis

### Documentação do Projeto
- [Documentação Geral](../README.md)
- [AudioManager.cs atual](../../ferritineVU/Assets/Scripts/Audio/AudioManager.cs)
- [Unity Integration Guide](../unity/UNITY_INTEGRATION_GUIDE.md)

### Referências Externas
- [Unity Audio Manual](https://docs.unity3d.com/Manual/Audio.html)
- [Unity Audio Mixer](https://docs.unity3d.com/Manual/AudioMixer.html)
- [Steam Audio Plugin](https://valvesoftware.github.io/steam-audio/)

---

## 📊 Status dos Documentos

| Documento | Status | Última Atualização | Versão |
|-----------|--------|-------------------|--------|
| AUDIO_SUMMARY.md | ✅ Completo | 2025-12-24 | 1.0 |
| AUDIO_ROADMAP.md | ✅ Completo | 2025-12-24 | 1.0 |
| AUDIO_LEVEL_2_FUTURE.md | ✅ Completo | 2025-12-24 | 1.0 |
| AUDIO_LEVEL_3_FUTURE.md | ✅ Completo | 2025-12-24 | 1.0 |

---

## 🤝 Contribuindo

Se você identificar:
- Informações desatualizadas
- Features faltando
- Melhores abordagens
- Erros técnicos

Por favor:
1. Abra uma issue descrevendo o problema
2. Ou envie um PR com a correção
3. Marque com label `documentation`

---

## 📞 Dúvidas?

- Consulte [AUDIO_SUMMARY.md](./AUDIO_SUMMARY.md) primeiro
- Se ainda tiver dúvidas, abra uma [Discussion no GitHub](https://github.com/lari-ember/ferritine/discussions)
- Para bugs no código atual, abra uma [Issue](https://github.com/lari-ember/ferritine/issues)

---

**Mantido por**: Equipe de Áudio  
**Criado em**: 2025-12-24  
**Versão**: 1.0
