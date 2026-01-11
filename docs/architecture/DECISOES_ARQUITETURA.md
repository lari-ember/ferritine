# Decisões de Arquitetura (Congeladas)

Documento que registra as decisões arquiteturais que estão congeladas para a Fase 0 do projeto. Essas decisões não devem ser alteradas durante esta fase sem revisão formal.

### Arquitetura Geral
- [x] Mundo majoritariamente estático
- [x] CityLayer decide regras, não gera visual
- [x] TerrainWorld é autoridade de altura e solo
- [x] DetailWorld é visual e localizado
- [x] Chunk é a unidade de mundo, LOD e preload
- [x] Nenhuma dependência circular entre sistemas

### Representação do Mundo
- [x] Terreno base usa voxels macro
- [x] Detalhes não fazem parte do terreno base
- [x] Escala variável existe, mas **não é usada ainda**
- [x] Cada chunk possui uma única escala de voxel

### Performance & Streaming
- [x] LOD será feito por chunk, não por voxel
- [x] Preload baseado em distância da câmera
- [x] Preload considera velocidade da câmera
- [x] Nenhuma lógica de streaming nesta fase

### Documentação
- [x] Arquivo `.md` criado para decisões de arquitetura
- [x] Decisões acima escritas e salvas
- [x] Compromisso de não mudar arquitetura nesta fase

**FASE 0 CONCLUÍDA QUANDO:**  
✔ Todas as caixas acima estão marcadas

---

Observações:
- Este arquivo registra a intenção e o estado atual das decisões arquiteturais para a Fase 0.
- Para alterar qualquer item desta lista durante a Fase 0 é necessário abrir um pedido formal de revisão e registrar a justificativa.
- Local do arquivo: `docs/architecture/DECISOES_ARQUITETURA.md`
- Autor: equipe de arquitetura
- Data de criação: 2025-12-29

