# ✅ Checklist de Testes - Geologia de Curitiba

## 📋 Testes de Implementação

### Fase 1: Verificação de Código ✅
- [x] ChunkData.cs modificado com lógica de camadas
- [x] ChunkMeshGenerator.cs com UV mapping
- [x] GeologyGameplayExample.cs criado
- [x] VoxelDataTypes.cs com BlockType enum
- [x] SoilProperties.cs com propriedades físicas
- [x] Sem erros de compilação
- [x] Texture atlas gerado (VoxelAtlas.png)

### Fase 2: Configuração no Unity 🔧
- [ ] VoxelAtlas.png copiado para Assets/Textures/
- [ ] Import settings configuradas:
  - [ ] Texture Type: Default
  - [ ] Filter Mode: Point (no filter)
  - [ ] Max Size: 512
  - [ ] sRGB: Ativado
- [ ] Material criado (VoxelTerrainMaterial)
  - [ ] Shader: Standard ou URP/Lit
  - [ ] Albedo: VoxelAtlas.png
  - [ ] Metallic: 0
  - [ ] Smoothness: 0.3
- [ ] Material atribuído ao prefab/gameobject do chunk

### Fase 3: Teste Visual 👀

#### 3.1 Verificação de Texturas
Execute o jogo e verifique:

- [ ] **Superfície aparece VERDE** (grama)
- [ ] **Áreas baixas aparecem AMARELAS** (areia)
- [ ] **Áreas muito baixas aparecem AZUIS** (água)

#### 3.2 Teste de Profundidade
Se tiver ferramenta de escavação/debug, remova blocos e verifique:

- [ ] **2-5 blocos abaixo**: Aparece MARROM (terra)
- [ ] **6-12 blocos abaixo**: Aparece VERMELHO (argila)
- [ ] **Mais de 12 blocos**: Aparece CINZA (granito)

#### 3.3 Teste de Iluminação
- [ ] Adicionar luz direcional (se não tiver)
- [ ] Verificar se cores são visíveis
- [ ] Ajustar intensidade se necessário

### Fase 4: Teste de Gameplay 🎮

#### 4.1 Sistema de Custos
Adicione `GeologyGameplayExample.cs` a um GameObject e teste:

```csharp
// No Console, você deve ver algo como:
=== Análise de Terreno em (100, 100) ===
Tipo de solo: Grama
Permeabilidade: 0.50
Capacidade de suporte: 80 kPa
Taxa de erosão: 0.20
Amigável para vegetação: True
✅ Construção permitida
💰 Custo estimado: $100.00
⏱️ Tempo de escavação (10 blocos): 5.5 horas
```

Checklist:
- [ ] Script executa sem erros
- [ ] Logs aparecem no Console
- [ ] Tipo de solo está correto
- [ ] Custos fazem sentido
- [ ] Tempo de escavação varia por profundidade

#### 4.2 Teste em Diferentes Terrenos
Teste em várias posições:

**Posição Alta** (ex: x=100, z=100):
- [ ] Tipo: Grama
- [ ] Custo: ~$100
- [ ] Permite construção

**Posição Baixa** (ex: próximo a rios):
- [ ] Tipo: Areia ou Água
- [ ] Custo: Maior ou impossível
- [ ] Aviso de enchente (se água)

**Posição Média**:
- [ ] Tipo: Terra
- [ ] Custo: ~$120
- [ ] Permite construção

#### 4.3 Sistema de Drenagem
Teste com diferentes tipos de solo:

```csharp
// Adicionar ao GeologyGameplayExample.cs:
void TestDrainage() {
    Debug.Log("=== TESTE DE DRENAGEM ===");
    
    // Teste em grama
    float runoff1 = SimulateRainfall(100, 100, 10f);
    Debug.Log($"Grama - Escoamento: {runoff1}mm (esperado: ~5mm)");
    
    // Teste em argila
    float runoff2 = SimulateRainfall(50, 50, 10f);
    Debug.Log($"Argila - Escoamento: {runoff2}mm (esperado: ~9.5mm)");
}
```

Checklist:
- [ ] Grama absorve mais água (menor escoamento)
- [ ] Argila absorve menos água (maior escoamento)
- [ ] Granito tem escoamento muito alto
- [ ] Água tem escoamento 100%

### Fase 5: Teste de Performance ⚡

#### 5.1 FPS Baseline
Execute o jogo e monitore:

- [ ] FPS sem chunks: _____
- [ ] FPS com 1 chunk: _____
- [ ] FPS com 4 chunks (2x2): _____
- [ ] FPS com 9 chunks (3x3): _____
- [ ] FPS com 16 chunks (4x4): _____

**Meta**: FPS > 30 com pelo menos 9 chunks visíveis

#### 5.2 Memory Usage
Verificar no Profiler:

- [ ] Uso de memória por chunk: ~_____ MB
- [ ] Crescimento de memória ao adicionar chunks: Linear / Exponencial
- [ ] Vazamento de memória ao destruir chunks: Sim / Não

#### 5.3 Otimizações Possíveis
Se performance for ruim:

- [ ] Reduzir tamanho do chunk (32x32 → 16x16)
- [ ] Implementar frustum culling
- [ ] Implementar LOD system
- [ ] Re-implementar Greedy Meshing (quando estável)

### Fase 6: Teste de Integração 🔗

#### 6.1 Integração com Sistema de Construção
- [ ] Verificar viabilidade antes de construir
- [ ] Calcular custo baseado no solo
- [ ] Mostrar mensagem de erro se inviável

#### 6.2 Integração com UI
- [ ] Mostrar tipo de solo ao selecionar terreno
- [ ] Mostrar custo estimado na UI
- [ ] Mostrar aviso de enchente se área de risco

#### 6.3 Integração com Economia
- [ ] Custo de fundação afeta orçamento
- [ ] Custo de escavação afeta tempo de construção
- [ ] Custo de drenagem afeta infraestrutura

### Fase 7: Testes de Edge Cases 🔍

#### 7.1 Bordas do Mapa
- [ ] Chunks na borda do heightmap não crasham
- [ ] Coordenadas negativas são tratadas
- [ ] Coordenadas além do mapa são tratadas

#### 7.2 Alturas Extremas
- [ ] Altitude 0 (mínima): Funciona
- [ ] Altitude 255 (máxima): Funciona
- [ ] Transição entre altitudes: Suave

#### 7.3 Dados Inválidos
- [ ] Heightmap null: Tratado
- [ ] BlockType inválido: Fallback para Terra
- [ ] Coordenadas fora do chunk: Tratado

### Fase 8: Teste de Usabilidade 👥

#### 8.1 Clareza Visual
- [ ] Texturas são distinguíveis
- [ ] Cores fazem sentido (verde=grama, marrom=terra)
- [ ] Transições entre materiais são visíveis

#### 8.2 Feedback ao Usuário
- [ ] Jogador entende por que não pode construir
- [ ] Jogador vê diferença de custo entre solos
- [ ] Jogador recebe aviso de enchente

#### 8.3 Documentação
- [ ] README explica como usar
- [ ] Tooltips no Inspector explicam parâmetros
- [ ] Exemplos de código são claros

---

## 📊 Resultados Esperados

### Sucesso Mínimo ✅
- [x] Código compila sem erros
- [ ] Texturas aparecem no terreno
- [ ] Camadas geológicas são visíveis
- [ ] Sistema de custos funciona

### Sucesso Completo 🌟
- [ ] Todas as fases de teste passam
- [ ] Performance adequada (>30 FPS)
- [ ] Integração com outros sistemas
- [ ] Usabilidade validada

### Excelência 🏆
- [ ] FPS > 60 com 16+ chunks
- [ ] Sistema de drenagem visual (água escoando)
- [ ] UI integrada e intuitiva
- [ ] Documentação completa

---

## 🐛 Problemas Comuns e Soluções

| Problema | Causa Provável | Solução |
|----------|----------------|---------|
| Texturas pretas | Material sem atlas | Atribuir VoxelAtlas.png |
| Texturas borradas | Filter Mode errado | Mudar para Point |
| FPS baixo | Muitos vértices | Reduzir chunks ou implementar LOD |
| UVs errados | Atlas não é 512x512 | Regenerar atlas |
| Cores erradas | Iluminação ruim | Adicionar luz direcional |
| Sem camadas | Lógica não executada | Verificar PopulateFromCache |
| Crash ao iniciar | Heightmap null | Atribuir heightmap no Inspector |

---

## 📝 Notas de Teste

### Teste 1: Data _____
**Testador**: _____  
**Ambiente**: Unity 2021.3 / 2022.3 / 2023.x  
**OS**: Windows / Linux / Mac  

**Resultados**:
- Fase 1: ✅ / ❌
- Fase 2: ✅ / ❌
- Fase 3: ✅ / ❌
- Fase 4: ✅ / ❌
- Fase 5: ✅ / ❌

**FPS**: _____ (com _____ chunks)  
**Problemas encontrados**: 
- _____
- _____

**Melhorias sugeridas**:
- _____
- _____

---

### Teste 2: Data _____
(repetir estrutura acima)

---

## ✅ Aprovação Final

- [ ] Todos os testes críticos passaram
- [ ] Performance aceitável
- [ ] Documentação completa
- [ ] Código revisado
- [ ] Pronto para produção

**Aprovado por**: _____  
**Data**: _____

---

## 🚀 Próximos Passos Pós-Testes

1. **Se tudo funciona**:
   - Implementar recursos avançados (drenagem visual, UI)
   - Otimizar performance (Greedy Meshing, LOD)
   - Integrar com outros sistemas

2. **Se houver problemas**:
   - Documentar bugs encontrados
   - Priorizar correções
   - Re-testar após correções

3. **Sempre**:
   - Manter documentação atualizada
   - Adicionar testes automatizados (se possível)
   - Coletar feedback de usuários

---

**Última atualização**: 2026-01-04

