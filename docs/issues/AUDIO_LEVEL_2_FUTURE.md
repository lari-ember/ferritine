# 🔮 Sistema de Áudio - NÍVEL 2 (Futuro Próximo)

> **⚠️ ATENÇÃO**: Estas funcionalidades **NÃO devem ser implementadas agora**. Este documento serve como planejamento para futuras melhorias do sistema de áudio.

## Status Atual

O `AudioManager.cs` já possui uma implementação básica funcional com:
- ✅ Singleton pattern
- ✅ Pool de AudioSource básico
- ✅ Canais separados (UI, SFX, Music, Ambient)
- ✅ Volume por categoria (básico)
- ✅ Suporte a áudio espacial (básico)

## 🎯 Objetivo do Nível 2

Expandir o sistema de áudio atual para incluir controles avançados de mixagem, interface de usuário para configurações de áudio, e sons ambientes contextuais por zona.

---

## Issue 1: AudioMixer Completo e Configurável

### Descrição
Implementar um sistema completo de AudioMixer no Unity com grupos hierárquicos, efeitos de áudio, e configuração via ScriptableObject.

### Funcionalidades Requeridas

#### 1.1 Estrutura Hierárquica de Mixer Groups
```
Master
├── UI
│   ├── UI_Buttons
│   ├── UI_Notifications
│   └── UI_Dialogs
├── World
│   ├── SFX_Vehicles
│   ├── SFX_Environment
│   ├── SFX_Characters
│   └── SFX_Buildings
├── Music
│   ├── Music_Background
│   └── Music_Events
└── Ambient
    ├── Ambient_Urban
    ├── Ambient_Industrial
    └── Ambient_Residential
```

#### 1.2 Efeitos de Áudio (Audio Effects)
- **Reverb**: Para criar sensação de espaço
  - Presets: Small Room, Large Hall, Cathedral, Outdoor
  - Parâmetros ajustáveis: Decay Time, Diffusion, Dry/Wet Mix
- **Lowpass/Highpass Filters**: Para oclusão e distância
- **Echo**: Para efeitos especiais
- **Compressor**: Para balancear volumes dinâmicos

#### 1.3 Snapshots do Mixer
- **Default**: Estado normal do jogo
- **Paused**: Quando o jogo está pausado (abaixar música/sfx)
- **Combat**: Durante situações tensas
- **Cutscene**: Para cinemáticas (priorizar diálogos)
- **Menu**: No menu principal

### Configuração Técnica

```csharp
[CreateAssetMenu(fileName = "AudioMixerConfig", menuName = "Audio/Mixer Configuration")]
public class AudioMixerConfiguration : ScriptableObject
{
    [System.Serializable]
    public class MixerGroupConfig
    {
        public string groupName;
        public AudioMixerGroup mixerGroup;
        public float defaultVolume = 1f;
        public bool allowMute = true;
    }
    
    [System.Serializable]
    public class MixerSnapshot
    {
        public string snapshotName;
        public AudioMixerSnapshot snapshot;
        public float transitionTime = 1f;
    }
    
    public AudioMixer mainMixer;
    public MixerGroupConfig[] mixerGroups;
    public MixerSnapshot[] snapshots;
}
```

### Critérios de Aceitação
- [ ] AudioMixer asset criado no Unity com estrutura hierárquica completa
- [ ] Todos os grupos têm controle de volume individual
- [ ] Pelo menos 3 snapshots diferentes funcionando
- [ ] Efeitos de reverb aplicados em grupos relevantes
- [ ] ScriptableObject para configuração implementado
- [ ] Testes de transição entre snapshots funcionando

### Prioridade
**Média** - Importante para imersão, mas não crítico para funcionalidade básica

### Dependências
- Nenhuma (pode ser implementado independentemente)

### Estimativa
3-5 dias de desenvolvimento + 2 dias de testes

---

## Issue 2: Interface de Controle de Volume (Mute e Sliders)

### Descrição
Criar uma interface de usuário completa para controle de volume com sliders, botões de mute, e persistência de configurações.

### Funcionalidades Requeridas

#### 2.1 UI de Configurações de Áudio
- **Painel de Settings**: Menu acessível no jogo
- **Sliders de Volume**:
  - Master Volume (0-100%)
  - UI Volume (0-100%)
  - SFX/World Volume (0-100%)
  - Music Volume (0-100%)
  - Ambient Volume (0-100%)
- **Botões de Mute**: Toggle individual para cada categoria
- **Preview de Sons**: Tocar um som de exemplo ao ajustar volume
- **Reset to Defaults**: Botão para restaurar configurações padrão

#### 2.2 Visualização em Tempo Real
- Medidor VU (Volume Unit) para cada canal
- Indicador visual quando um som está tocando em cada canal
- Contador de AudioSources ativas por canal

#### 2.3 Persistência de Configurações
- Salvar em PlayerPrefs (desenvolvimento)
- Preparar estrutura para salvar em JSON (produção)
- Carregar automaticamente ao iniciar o jogo

### Implementação Técnica

```csharp
public class AudioSettingsUI : MonoBehaviour
{
    [Header("UI References")]
    public Slider masterVolumeSlider;
    public Slider uiVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider ambientVolumeSlider;
    
    [Header("Mute Buttons")]
    public Toggle masterMuteToggle;
    public Toggle uiMuteToggle;
    public Toggle sfxMuteToggle;
    public Toggle musicMuteToggle;
    public Toggle ambientMuteToggle;
    
    [Header("Visual Feedback")]
    public VUMeter masterVUMeter;
    public VUMeter[] channelVUMeters;
    
    [Header("Preview Sounds")]
    public AudioClip uiPreviewSound;
    public AudioClip sfxPreviewSound;
    
    // Métodos de callback para os sliders
    public void OnMasterVolumeChanged(float value) { }
    public void OnUIVolumeChanged(float value) { }
    // ... etc
    
    // Métodos para mute toggles
    public void OnMasterMuteToggled(bool muted) { }
    // ... etc
    
    // Método para preview
    public void PlayPreviewSound(string channel) { }
    
    // Save/Load
    public void SaveSettings() { }
    public void LoadSettings() { }
    public void ResetToDefaults() { }
}
```

### Design da UI
- **Localização**: Acessível via menu de pausa (ESC)
- **Tab/Seção**: "Audio Settings" ou "Configurações de Áudio"
- **Layout**: Vertical com categorias claramente separadas
- **Estilo**: Minimalista, seguindo o design system do projeto

### Critérios de Aceitação
- [ ] UI funcional com todos os sliders e botões
- [ ] Mudanças de volume aplicadas em tempo real
- [ ] Mute funciona para cada categoria independentemente
- [ ] Configurações persistem entre sessões do jogo
- [ ] Preview de som funciona ao ajustar volume
- [ ] VU meters mostram níveis de áudio visualmente
- [ ] UI responsiva e intuitiva

### Prioridade
**Alta** - Essencial para acessibilidade e experiência do usuário

### Dependências
- Issue 1 (AudioMixer completo) deve estar concluída

### Estimativa
4-6 dias de desenvolvimento + 2 dias de polish UI

---

## Issue 3: Sistema de Som Ambiente por Zona

### Descrição
Implementar um sistema que reproduz sons ambientes diferentes dependendo da zona/área do mapa onde o jogador ou câmera está localizada.

### Funcionalidades Requeridas

#### 3.1 Definição de Zonas de Áudio
- **Trigger Zones**: Colliders invisíveis que definem áreas
- **Audio Zone Component**: Script que configura o som ambiente da zona
- **Tipos de Zona**:
  - Residencial: Pássaros, vozes distantes, carros ocasionais
  - Industrial: Máquinas, metal, vapor
  - Comercial: Multidão, tráfego, lojas
  - Rural/Parque: Natureza, vento, água
  - Noturna: Grilos, silêncio, sons distantes

#### 3.2 Transição Suave entre Zonas
- Crossfade entre ambientes (2-3 segundos)
- Múltiplas camadas de som ambiente
- Priorização quando em múltiplas zonas (zona menor tem prioridade)

#### 3.3 Sistema de Camadas (Layered Ambient)
- **Base Layer**: Som contínuo (ex: vento constante)
- **Detail Layer**: Sons ocasionais (ex: pássaros, carros)
- **Event Layer**: Sons raros/especiais (ex: sirene ao longe)

#### 3.4 Variação Temporal
- Sons diferentes para dia/noite
- Sons diferentes para clima (chuva, tempestade)
- Intensidade baseada em eventos (ex: rush hour)

### Implementação Técnica

```csharp
public enum ZoneType
{
    Residential,
    Industrial,
    Commercial,
    Rural,
    Night
}

[System.Serializable]
public class AmbientSoundLayer
{
    public AudioClip[] clips;
    public float volume = 1f;
    public bool loop = true;
    public float minInterval = 5f;
    public float maxInterval = 15f;
}

public class AudioZone : MonoBehaviour
{
    [Header("Zone Configuration")]
    public ZoneType zoneType;
    public string zoneName;
    public int priority = 0; // Maior = mais importante
    
    [Header("Ambient Layers")]
    public AmbientSoundLayer baseLayer;
    public AmbientSoundLayer detailLayer;
    public AmbientSoundLayer eventLayer;
    
    [Header("Transition")]
    public float fadeInDuration = 2f;
    public float fadeOutDuration = 2f;
    
    [Header("Time/Weather Conditions")]
    public bool affectedByTimeOfDay = true;
    public AmbientSoundLayer nightOverride;
    public bool affectedByWeather = true;
    public AmbientSoundLayer rainOverride;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("MainCamera"))
        {
            AudioManager.EnterAudioZone(this);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("MainCamera"))
        {
            AudioManager.ExitAudioZone(this);
        }
    }
}
```

### Expansão do AudioManager

```csharp
// Adicionar ao AudioManager.cs existente:
public class AudioManager : MonoBehaviour
{
    // ... código existente ...
    
    private List<AudioZone> activeZones = new List<AudioZone>();
    private AudioZone currentPrimaryZone;
    
    public static void EnterAudioZone(AudioZone zone)
    {
        if (Instance == null) return;
        Instance.HandleZoneEnter(zone);
    }
    
    public static void ExitAudioZone(AudioZone zone)
    {
        if (Instance == null) return;
        Instance.HandleZoneExit(zone);
    }
    
    private void HandleZoneEnter(AudioZone zone)
    {
        activeZones.Add(zone);
        UpdatePrimaryZone();
    }
    
    private void HandleZoneExit(AudioZone zone)
    {
        activeZones.Remove(zone);
        UpdatePrimaryZone();
    }
    
    private void UpdatePrimaryZone()
    {
        // Determina zona com maior prioridade
        AudioZone newPrimary = activeZones
            .OrderByDescending(z => z.priority)
            .FirstOrDefault();
        
        if (newPrimary != currentPrimaryZone)
        {
            TransitionToZone(newPrimary);
            currentPrimaryZone = newPrimary;
        }
    }
    
    private void TransitionToZone(AudioZone zone)
    {
        // Crossfade logic
    }
}
```

### Critérios de Aceitação
- [ ] AudioZone component funcional
- [ ] Pelo menos 3 tipos de zona implementados com sons
- [ ] Transições suaves entre zonas funcionando
- [ ] Sistema de prioridade de zonas funcional
- [ ] Camadas de som ambiente (base, detail, event) implementadas
- [ ] Sons variam com hora do dia
- [ ] Performance otimizada (sem travamentos)

### Prioridade
**Média** - Importante para imersão, mas não crítico

### Dependências
- Issue 1 (AudioMixer) deve estar concluída
- Requer criação/obtenção de assets de áudio

### Estimativa
5-7 dias de desenvolvimento + 3 dias de testes e ajustes

---

## 📋 Resumo do Nível 2

### Ordem de Implementação Recomendada
1. Issue 1: AudioMixer Completo → Base para tudo
2. Issue 2: UI de Controle → Essencial para UX
3. Issue 3: Som Ambiente por Zona → Feature de imersão

### Recursos Necessários
- **AudioClips**: ~50-100 sons ambiente diferentes
- **Designer de Som**: Para criar/selecionar sons adequados
- **UI/UX Designer**: Para design do menu de configurações
- **Tempo Total Estimado**: 3-4 semanas de desenvolvimento

### Testes Necessários
- Testes de performance com múltiplas zonas ativas
- Testes de transição entre zonas
- Testes de UI em diferentes resoluções
- Testes de persistência de configurações
- Testes de memória (vazamentos de AudioSource)

---

## ⚠️ O QUE **NÃO** FAZER AGORA

❌ **Não implementar neste momento:**
- Reverb avançado
- Efeitos de áudio complexos
- UI de configurações completa
- Sistema de zonas
- Variação temporal de sons
- Sistema de snapshots

✅ **Pode usar do código atual:**
- AudioManager singleton básico
- Pool de AudioSource existente
- Funções PlayUISound, PlaySFX, PlayMusic
- Volume básico por categoria

---

## 🔗 Referências e Recursos

### Unity Documentation
- [AudioMixer Manual](https://docs.unity3d.com/Manual/AudioMixer.html)
- [Audio Effects](https://docs.unity3d.com/Manual/class-AudioEffect.html)
- [AudioMixer Snapshots](https://docs.unity3d.com/Manual/AudioMixerSnapshots.html)

### Tutoriais Recomendados
- Unity Learn: Audio Mixing
- Brackeys: Audio Manager Tutorial
- Game Dev Guide: Advanced Audio Systems

### Assets de Áudio Gratuitos
- Freesound.org
- Unity Asset Store (Free Audio)
- OpenGameArt.org

---

## 📝 Notas Adicionais

- Considerar acessibilidade (subtitles para sons importantes)
- Documentar todos os parâmetros de áudio para sound designers
- Criar ferramentas de debug para visualizar zonas de áudio ativas
- Manter compatibilidade com WebGL (limitações de áudio)

---

**Última Atualização**: 2025-12-24  
**Status**: 📋 Planejamento  
**Versão do Documento**: 1.0
