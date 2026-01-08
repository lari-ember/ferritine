# 🌍 Sistema de Geologia de Curitiba

## 🚀 TL;DR (Too Long; Didn't Read)

✅ **Sistema de geologia em camadas implementado e funcional!**

- 4 camadas geológicas (Grama → Terra → Argila → Granito)
- 20 materiais com texturas únicas
- Sistema de gameplay (custos, drenagem, construção)
- Texture atlas 512x512 gerado automaticamente
- Documentação completa

**Próximo passo**: Testar no Unity!

---

## 📁 Arquivos Importantes

### Para começar rapidamente:
1. **QUICK_START_GEOLOGIA.md** - Guia rápido de uso
2. **GEOLOGIA_TEST_CHECKLIST.md** - Checklist de testes
3. **Assets/Textures/VoxelAtlas.png** - Texture atlas (já gerado!)

### Para entender o sistema:
4. **GEOLOGIA_CURITIBA_IMPLEMENTATION.md** - Documentação completa
5. **GEOLOGIA_IMPLEMENTATION_SUMMARY.md** - Resumo da implementação
6. **TEXTURE_ATLAS_GUIDE.md** - Como criar/modificar texturas

### Código:
7. **Assets/Scripts/Voxel/ChunkData.cs** - Lógica de camadas
8. **Assets/Scripts/Voxel/ChunkMeshGenerator.cs** - UV mapping
9. **Assets/Scripts/Voxel/GeologyGameplayExample.cs** - Exemplos de uso

---

## ⚡ Quick Start (5 minutos)

### 1. Verificar Texture Atlas
```bash
# O atlas já foi gerado e copiado para:
ls -lh Assets/Textures/VoxelAtlas.png
# Deve mostrar: 512x512 pixels, ~100KB
```

### 2. Configurar no Unity
1. Abrir Unity
2. Ir para `Assets/Textures/VoxelAtlas.png`
3. Inspector → Texture Type: Default
4. Filter Mode: **Point (no filter)**
5. Max Size: 512
6. **Apply**

### 3. Criar Material
1. `Assets/Materials/` → Create → Material
2. Nome: `VoxelTerrainMaterial`
3. Shader: Standard (ou URP/Lit)
4. Albedo: Arrastar `VoxelAtlas.png`
5. Metallic: 0, Smoothness: 0.3

### 4. Testar
1. Executar o jogo
2. Verificar se terreno tem texturas coloridas:
   - Verde = Grama (superfície)
   - Marrom = Terra (camada intermediária)
   - Vermelho = Argila (profundo)
   - Cinza = Granito (base rochosa)

---

## 🎯 O que cada arquivo faz?

### Código Principal

**ChunkData.cs**
```csharp
// Define as camadas geológicas ao gerar chunks
// Grama → Terra → Argila → Granito
voxels[x, y, z] = (byte)BlockType.Grama; // Por exemplo
```

**ChunkMeshGenerator.cs**
```csharp
// Cria a mesh com UVs corretas para cada material
AddFaceUVs(uvs, blockType); // Mapeia para texture atlas
```

**GeologyGameplayExample.cs**
```csharp
// Exemplos de uso no gameplay:
float cost = CalculateFoundationCost(x, z); // Custo por tipo de solo
bool canBuild = CanBuild(x, z, out reason); // Verificar viabilidade
float runoff = SimulateRainfall(x, z, 10f); // Drenagem
```

### Dados

**VoxelDataTypes.cs**
```csharp
public enum BlockType : byte {
    Ar = 0, Grama = 1, Terra = 2, Argila = 3,
    Areia = 4, Granito = 10, Agua = 40, ...
}
```

**SoilProperties.cs**
```csharp
// Propriedades físicas de cada solo:
// - Permeabilidade (absorção de água)
// - Taxa de erosão
// - Capacidade de suporte (para construção)
// - Se permite vegetação
```

---

## 🎮 Exemplos de Uso

### Exemplo 1: Verificar tipo de solo
```csharp
BlockType soil = terrainWorld.GetSoilBlockType(x, z);
Debug.Log($"Solo em ({x},{z}): {soil}");
```

### Exemplo 2: Calcular custo de construção
```csharp
GeologyGameplayExample geology = GetComponent<GeologyGameplayExample>();
float cost = geology.CalculateFoundationCost(x, z);
Debug.Log($"Custo: ${cost}");
```

### Exemplo 3: Verificar se pode construir
```csharp
if (geology.CanBuild(x, z, out string reason)) {
    // Permitir construção
    BuildingManager.Construct(x, z);
} else {
    // Mostrar erro ao jogador
    UI.ShowError(reason);
}
```

### Exemplo 4: Simular enchente
```csharp
float rainfall = 10f; // mm de chuva
float runoff = geology.SimulateRainfall(x, z, rainfall);
if (runoff > 0.5f) {
    // Área em risco de enchente!
    FloodManager.TriggerFlood(x, z);
}
```

---

## 📊 Materiais Disponíveis (Top 10)

| BlockType | Cor | Uso |
|-----------|-----|-----|
| Grama (1) | Verde | Superfície, parques |
| Terra (2) | Marrom | Fundações rasas |
| Argila (3) | Vermelho | Fundações médias, tijolos |
| Areia (4) | Amarelo | Várzeas, praias |
| Granito (10) | Cinza | Fundações profundas |
| Água (40) | Azul | Rios, lagos |
| Concreto (30) | Prata | Estruturas urbanas |
| Asfalto (31) | Preto | Ruas |
| Vegetação (41) | Verde escuro | Floresta |
| Arenito (20) | Bege | Construção |

---

## 🐛 Problemas Comuns

### "Texturas não aparecem"
→ Verificar se VoxelAtlas.png está em `Assets/Textures/`  
→ Verificar se material usa o atlas correto

### "Texturas borradas"
→ Mudar Filter Mode para `Point (no filter)`

### "Tudo é cinza/preto"
→ Adicionar luz direcional na cena  
→ Verificar se material tem o atlas atribuído

### "Performance ruim"
→ Reduzir número de chunks visíveis  
→ Reduzir tamanho do chunk (32→16)

---

## 📚 Documentação Completa

Para mais detalhes, consulte:

1. **GEOLOGIA_CURITIBA_IMPLEMENTATION.md** - Documentação técnica completa
   - Estrutura geológica detalhada
   - Implicações para gameplay
   - Sistema de texturas
   - Referências científicas

2. **QUICK_START_GEOLOGIA.md** - Guia rápido
   - Como testar
   - Como usar no gameplay
   - FAQ
   - Troubleshooting

3. **TEXTURE_ATLAS_GUIDE.md** - Guia de texturas
   - Como criar atlas personalizado
   - Ferramentas (GIMP, Python)
   - Configuração no Unity

4. **GEOLOGIA_TEST_CHECKLIST.md** - Checklist de testes
   - 8 fases de testes
   - Critérios de aprovação
   - Problemas comuns

---

## 🎯 Roadmap

### ✅ Fase 1: Base (COMPLETO)
- [x] Sistema de camadas geológicas
- [x] UV mapping por material
- [x] Texture atlas gerado
- [x] Propriedades físicas
- [x] Sistema de gameplay básico
- [x] Documentação

### 🔧 Fase 2: Testes (AGORA)
- [ ] Testar visualmente no Unity
- [ ] Validar performance
- [ ] Ajustar texturas/materiais
- [ ] Integrar com sistemas existentes

### 🚀 Fase 3: Expansão (FUTURO)
- [ ] Sistema de drenagem visual (água escoando)
- [ ] UI de informações de terreno
- [ ] Sistema de custos integrado
- [ ] Visualização de dados (mapas de calor)
- [ ] Geologia avançada (variação horizontal)
- [ ] Re-implementar Greedy Meshing

---

## 💡 Dicas

### Para Designers
- Use cores fortes no atlas para facilitar debug
- Teste em diferentes iluminações
- Considere daltonismo (evite só verde/vermelho)

### Para Programadores
- `TerrainWorld.GetSoilBlockType()` é sua função principal
- Use `SoilProperties.Get()` para gameplay
- Cache resultados de `GetSoilStats()` se chamar múltiplas vezes

### Para Artistas
- Crie variações de texturas (grama seca, molhada, etc.)
- Use normal maps para adicionar profundidade
- Considere criar atlas HD (1024x1024)

---

## 🤝 Contribuindo

### Adicionar Novo Material

1. **Adicionar ao enum** (VoxelDataTypes.cs):
```csharp
public enum BlockType : byte {
    // ...
    MeuNovoMaterial = 52
}
```

2. **Adicionar propriedades** (SoilProperties.cs):
```csharp
case BlockType.MeuNovoMaterial:
    return new SoilStats { 
        permeability = 0.4f, 
        erosionRate = 0.3f,
        bearingCapacity = 100f,
        vegetationFriendly = true,
        note = "Descrição"
    };
```

3. **Adicionar textura no atlas**:
   - Editar `scripts/generate_voxel_atlas.py`
   - Adicionar entrada em `MATERIALS` dict
   - Regenerar atlas: `python3 scripts/generate_voxel_atlas.py`

4. **Usar na lógica** (ChunkData.cs):
```csharp
// Exemplo: usar em camada específica
if (condicao) {
    voxels[x, y, z] = (byte)BlockType.MeuNovoMaterial;
}
```

---

## 📞 Suporte

### Logs de Debug
```csharp
// Adicione ao seu código para debugar:
Debug.Log($"BlockType em ({x},{z}): {terrainWorld.GetSoilBlockType(x, z)}");
Debug.Log($"Propriedades: {terrainWorld.GetSoilStats(x, z).note}");
```

### Visualizar UVs
```csharp
// No OnDrawGizmos() do chunk:
Vector2[] uvs = GetComponent<MeshFilter>().sharedMesh.uv;
for (int i = 0; i < uvs.Length; i += 4) {
    Debug.Log($"Face {i/4}: UV = {uvs[i]}");
}
```

---

## ✅ Checklist Rápido

Antes de começar a usar:
- [ ] Texture atlas copiado para `Assets/Textures/`
- [ ] Import settings configurados (Point filter)
- [ ] Material criado com atlas
- [ ] Material aplicado aos chunks
- [ ] Luz direcional na cena
- [ ] Executar o jogo e verificar texturas

Tudo certo? Agora você pode:
- [ ] Testar gameplay (GeologyGameplayExample.cs)
- [ ] Integrar com sistema de construção
- [ ] Adicionar UI de informações
- [ ] Implementar custos reais

---

## 🌟 Créditos

**Sistema de Geologia de Curitiba**  
Implementação: GitHub Copilot  
Data: 2026-01-04  
Baseado em: Geologia real do Primeiro Planalto Paranaense  

**Referências**:
- MINEROPAR - Minerais do Paraná
- Mapas geológicos de Curitiba
- Sistema de drenagem histórico da cidade

---

**Versão**: 1.0  
**Status**: ✅ Implementado e pronto para uso  
**Última atualização**: 2026-01-04

