# Congelar Decisões (OBRIGATÓRIA)

Documento que registra as decisões arquiteturais que estão congeladas para a Fase 0 do projeto. Essas decisões não devem ser alteradas durante esta fase sem revisão formal.

### Arquitetura Geral
- [ ] Mundo majoritariamente estático
- [ ] CityLayer decide regras, não gera visual
- [ ] TerrainWorld é autoridade de altura e solo
- [ ] DetailWorld é visual e localizado
- [ ] Chunk é a unidade de mundo, LOD e preload
- [ ] Nenhuma dependência circular entre sistemas

### Representação do Mundo
- [ ] Terreno base usa voxels macro
- [ ] Detalhes não fazem parte do terreno base
- [ ] Escala variável existe, mas **não é usada ainda**
- [ ] Cada chunk possui uma única escala de voxel

### Performance & Streaming
- [ ] LOD será feito por chunk, não por voxel
- [ ] Preload baseado em distância da câmera
- [ ] Preload considera velocidade da câmera
- [ ] Nenhuma lógica de streaming nesta fase

### Documentação
- [ ] Arquivo `.md` criado para decisões de arquitetura
- [ ] Decisões acima escritas e salvas
- [ ] Compromisso de não mudar arquitetura nesta fase

**FASE 0 CONCLUÍDA QUANDO:**  
✔ Todas as caixas acima estão marcadas

---

Observações:
- Este arquivo registra a intenção e o estado atual das decisões arquiteturais para a Fase 0.
- Para alterar qualquer item desta lista durante a Fase 0 é necessário abrir um pedido formal de revisão e registrar a justificativa.
- Local do arquivo: `docs/architecture/DECISOES_ARQUITETURA_FASE_0.md`
- Autor: equipe de arquitetura
- Data de criação: 2025-12-29


