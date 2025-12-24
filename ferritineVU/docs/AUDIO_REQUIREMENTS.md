# 🔊 Sons Necessários para Camera Controller

## Novos Sons UI Necessários

Os seguintes sons precisam ser adicionados ao array `uiClips` do `AudioManager`:

### Sons de Bookmark
- **bookmark_save** - Som de confirmação ao salvar posição (Ctrl+1-9)
- **bookmark_restore** - Som suave ao restaurar posição (1-9)

### Sons de Seleção
- **entity_select** - Som ao selecionar entidade (já deve existir)
- **entity_deselect** - Som ao deselecionar entidade com ESC

### Sons de Painel
- **panel_close** - Som ao fechar painel com ESC (já deve existir)

## Configuração no Unity

1. Crie ou obtenha os arquivos de áudio (formatos: .wav, .mp3, .ogg)
2. Coloque-os em `Assets/Audio/UI/`
3. Selecione o GameObject com `AudioManager` na cena
4. Adicione os clips ao array `uiClips` no Inspector
5. Os nomes dos clips devem corresponder exatamente aos nomes acima

## Sugestões de Sons

### bookmark_save
- Tom positivo curto
- Tipo "ding" ou "chime"
- Duração: ~0.2s

### bookmark_restore
- Tom suave "whoosh"
- Indica transição
- Duração: ~0.3s

### entity_deselect
- Tom mais grave que entity_select
- Indica "fechamento"
- Duração: ~0.15s

## Fallback

Se os sons não forem encontrados, o sistema simplesmente não tocará nada (sem erros).
O `AudioManager.PlayUISound()` já trata clips inexistentes graciosamente.
