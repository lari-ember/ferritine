# 🎵 Planejamento do Sistema de Áudio - Roadmap Completo

> **Data de Criação**: 2025-12-24  
> **Status**: Documento de Planejamento  
> **Versão**: 1.0

## 📋 Visão Geral

Este documento organiza o desenvolvimento do sistema de áudio do projeto Ferritine em 3 níveis progressivos de complexidade e funcionalidade.

---

## 🎯 Estado Atual (NÍVEL 1 - IMPLEMENTADO)

### ✅ O Que Já Temos

O arquivo `ferritineVU/Assets/Scripts/Audio/AudioManager.cs` implementa:

1. **Singleton Pattern** com DontDestroyOnLoad
2. **Pool de AudioSource** (5 por canal, expansível)
3. **Canais Separados**:
   - UI (sons de interface)
   - SFX (efeitos sonoros do mundo)
   - Music (música de fundo)
   - Ambient (sons ambientes)
4. **Volume por Categoria** (básico):
   - SetMasterVolume()
   - SetUIVolume()
   - SetSFXVolume()
   - SetMusicVolume()
5. **Persistência Básica**: PlayerPrefs para salvar volumes
6. **Áudio Espacial Básico**: Suporte a spatialBlend e posição 3D
7. **API Simples**:
   ```csharp
   AudioManager.PlayUISound("button_click");
   AudioManager.PlaySFX("explosion", position);
   AudioManager.PlayMusic("background_music");
   ```

### 💪 Pontos Fortes da Implementação Atual

- Código limpo e bem documentado (em português)
- Arquitetura extensível
- Performance adequada para uso básico
- Sistema de pooling funcional

### 🔍 Limitações Conhecidas

- Sem AudioMixer configurado (apenas código)
- Sem UI para controle de volume
- Sem sistema de zonas de áudio
- Sem reverb ou efeitos avançados
- Sem sistema de prioridade de sons
- Sem oclusão de áudio
- Sem otimizações avançadas de performance

---

## 🚫 O QUE **NÃO** FAZER AGORA

Conforme especificado no problema original, as seguintes funcionalidades **NÃO devem ser implementadas agora**:

❌ **Proibido implementar neste momento:**
- AudioMixer avançado
- Reverb e efeitos de áudio
- Áudio 3D avançado (HRTF, doppler)
- Música de fundo com sistema completo
- Pool de AudioSource otimizado (atual é suficiente)
- UI de configurações de áudio
- Sistema de zonas
- Oclusão
- Sistema de prioridade

✅ **Pode continuar usando:**
- AudioManager.cs atual
- Funções básicas (PlayUISound, PlaySFX, PlayMusic)
- Pool existente
- Volume básico por categoria

---

## 📊 Roadmap de Desenvolvimento

### 🔮 NÍVEL 2: Futuro Próximo (3-4 semanas)

**Documento**: [AUDIO_LEVEL_2_FUTURE.md](./AUDIO_LEVEL_2_FUTURE.md)

#### Issue 1: AudioMixer Completo e Configurável
- Estrutura hierárquica de grupos
- Efeitos de áudio (reverb, filters)
- Snapshots do mixer
- ScriptableObject para configuração
- **Estimativa**: 3-5 dias

#### Issue 2: Interface de Controle de Volume
- UI com sliders para cada categoria
- Botões de mute
- VU meters (medidores visuais)
- Preview de sons
- Persistência melhorada
- **Estimativa**: 4-6 dias

#### Issue 3: Sistema de Som Ambiente por Zona
- Audio Zones com triggers
- Transição suave entre zonas
- Camadas de som (base, detail, event)
- Variação temporal (dia/noite)
- **Estimativa**: 5-7 dias

**Prioridade**: Média a Alta  
**Tempo Total**: ~3-4 semanas  
**Dependências**: Nenhuma (independentes entre si)

---

### 🔮 NÍVEL 3: Bem Depois (2-3 meses)

**Documento**: [AUDIO_LEVEL_3_FUTURE.md](./AUDIO_LEVEL_3_FUTURE.md)

#### Issue 1: Sistema de Áudio 3D Real
- HRTF e binaural audio
- Doppler effect
- Curvas de atenuação customizáveis
- Sons direcionais
- Integração com spatializer plugins
- **Estimativa**: 2-3 semanas

#### Issue 2: Sistema de Oclusão e Obstruction
- Raycasting para detecção de obstáculos
- Material-based filtering
- Filtros dinâmicos (lowpass, highpass)
- Portal system (som através de portas)
- **Estimativa**: 3-4 semanas

#### Issue 3: Sistema de Prioridade e Culling
- Priority levels (Critical, High, Medium, Low)
- Distance-based culling
- Voice stealing
- Limite de AudioSources simultâneas
- Performance monitoring dashboard
- **Estimativa**: 2-3 semanas

#### Issue 4: Performance Tuning e Otimização
- Audio streaming (música)
- Compressão e formatos otimizados
- Memory management
- CPU optimization
- Platform-specific optimizations
- **Estimativa**: 2-3 semanas

**Prioridade**: Baixa (polish, não essencial)  
**Tempo Total**: ~2-3 meses  
**Dependências**: Requer Nível 2 completo

---

## 🗂️ Estrutura de Arquivos Recomendada

```
ferritineVU/Assets/
├── Audio/
│   ├── Clips/
│   │   ├── UI/
│   │   ├── SFX/
│   │   ├── Music/
│   │   └── Ambient/
│   ├── Mixers/
│   │   └── MainAudioMixer.mixer      # Nível 2
│   └── Configs/
│       ├── AudioMixerConfig.asset     # Nível 2
│       └── AudioPerformanceSettings.asset  # Nível 3
├── Scripts/
│   └── Audio/
│       ├── AudioManager.cs            # ✅ ATUAL
│       ├── AudioSettingsUI.cs         # Nível 2
│       ├── AudioZone.cs               # Nível 2
│       ├── Audio3DEmitter.cs          # Nível 3
│       ├── AudioOcclusionSystem.cs    # Nível 3
│       ├── AudioPrioritySystem.cs     # Nível 3
│       └── AudioPerformanceManager.cs # Nível 3
└── Prefabs/
    └── Audio/
        ├── AudioManager.prefab
        └── AudioSettingsUI.prefab
```

---

## 📈 Métricas de Sucesso

### Nível 2
- [ ] AudioMixer configurado e funcional
- [ ] UI de configurações acessível e responsiva
- [ ] Pelo menos 3 zonas de áudio implementadas
- [ ] Transições suaves entre zonas
- [ ] Configurações persistem entre sessões
- [ ] Feedback positivo de playtesters

### Nível 3
- [ ] Performance de áudio < 5% CPU
- [ ] Memória de áudio dentro do budget
- [ ] Oclusão funcionando corretamente
- [ ] Sistema de prioridade sem falhas
- [ ] Zero crashes relacionados a áudio
- [ ] Testes em múltiplas plataformas bem-sucedidos

---

## 🎓 Recursos de Aprendizado

### Para Nível 2
- [Unity Audio Mixer Documentation](https://docs.unity3d.com/Manual/AudioMixer.html)
- [Unity UI Toolkit](https://docs.unity3d.com/Manual/UIElements.html)
- Tutorial: "Audio Manager in Unity" (Brackeys)

### Para Nível 3
- [Steam Audio Plugin](https://valvesoftware.github.io/steam-audio/)
- GDC Talk: "Audio for Games"
- Unity Learn: Advanced Audio Techniques
- Paper: "Real-Time Sound Synthesis for Interactive Applications"

---

## 🔄 Processo de Implementação Recomendado

### Antes de Começar Qualquer Nível

1. **Revisar código atual**: Entender completamente o AudioManager.cs
2. **Definir requisitos**: Listar funcionalidades específicas necessárias
3. **Criar branch**: `feature/audio-level-X`
4. **Planejar testes**: Como validar cada feature

### Durante Implementação

1. **Desenvolver incrementalmente**: Uma feature por vez
2. **Testar constantemente**: Não acumular código não testado
3. **Documentar código**: Manter padrão de comentários em português
4. **Otimizar cedo**: Não deixar problemas de performance para depois

### Após Implementação

1. **Code review**: Revisar com time
2. **Playtest**: Testar com usuários reais
3. **Profile**: Usar Unity Profiler para verificar performance
4. **Documentar**: Atualizar documentação do projeto

---

## ⚠️ Avisos Importantes

### Performance
- **Sempre profile** após adicionar novas features de áudio
- **Mantenha limite** de AudioSources simultâneas (32-64 máximo)
- **Cuidado com efeitos**: Reverb e filters são caros em CPU

### Compatibilidade
- **WebGL tem limitações**: Poucos AudioSources, sem streaming
- **Mobile é limitado**: Reduzir qualidade e quantidade
- **Testes em plataforma alvo**: Não assumir que funciona igual em todos

### Manutenibilidade
- **Não complexifique desnecessariamente**: Simplicidade > Features
- **Documente decisões**: Por que escolheu fazer de uma forma
- **Mantenha código limpo**: Refatore quando necessário

---

## 📞 Suporte e Dúvidas

### Documentos Relacionados
- [AudioManager.cs atual](../../ferritineVU/Assets/Scripts/Audio/AudioManager.cs)
- [AUDIO_LEVEL_2_FUTURE.md](./AUDIO_LEVEL_2_FUTURE.md) - Detalhes do Nível 2
- [AUDIO_LEVEL_3_FUTURE.md](./AUDIO_LEVEL_3_FUTURE.md) - Detalhes do Nível 3

### Para Criar Issues no GitHub
Use o template de feature request:
```
Título: [AUDIO-L2-1] Implementar AudioMixer Completo
Ou: [AUDIO-L3-2] Implementar Sistema de Oclusão

Labels: enhancement, audio, future
Milestone: Audio System Level 2 (ou 3)
```

### Discussão Técnica
Para discutir decisões de design ou implementação, abra uma Discussion no GitHub na categoria "Architecture" ou "Ideas".

---

## 📅 Linha do Tempo Sugerida

```
Agora (Nível 1) -------- 1 mês -------- 2 meses ------- 3 meses ---- 6 meses
     |                      |               |              |            |
  [AudioManager        [Iniciar        [Completar    [Iniciar    [Completar
   básico OK]           Nível 2]        Nível 2]     Nível 3]    Nível 3]
                                                                       |
                                                                   [Sistema de
                                                                    áudio AAA
                                                                    completo!]
```

**Nota**: Esta é apenas uma sugestão. Ajuste baseado em prioridades do projeto.

---

## 🎉 Conclusão

Este roadmap fornece um caminho claro para evoluir o sistema de áudio do Ferritine de um estado funcional básico para um sistema de áudio de qualidade AAA.

**Lembre-se**:
- ✅ Nível 1 (atual) é **suficiente** para desenvolvimento inicial
- 🔮 Nível 2 deve ser implementado quando **UX de áudio** for prioridade
- 🔮 Nível 3 deve ser implementado apenas no **polishing final**

**Priorize sempre**:
1. Gameplay funcional
2. Performance estável
3. Features de áudio avançadas

---

**Documento Mantido Por**: Equipe de Áudio  
**Última Revisão**: 2025-12-24  
**Próxima Revisão**: Após completar Nível 2  
**Versão**: 1.0
