using UnityEngine;

namespace Voxel {
    /// <summary>
    /// Perfil de configuração para carregamento de chunks.
    /// Ajusta automaticamente os parâmetros baseado na escala do voxel.
    /// </summary>
    [CreateAssetMenu(fileName = "PreloadProfile", menuName = "Voxel/Preload Profile")]
    public class PreloadProfile : ScriptableObject {
        [Header("Configuração de Área")]
        [Tooltip("Raio em metros que deve ser mantido carregado ao redor da câmera")]
        public float raioDesejadoMetros = 200f;
        
        [Header("Limites de Performance")]
        [Tooltip("Número máximo de chunks em cada direção (raio). Evita explosão com voxels pequenos")]
        [Range(8, 64)] public int maxChunkRadius = 24;
        
        [Tooltip("Quantos chunks de dados manter em RAM além da área visual")]
        [Range(2, 12)] public int dadosRetencaoRadius = 4;
        
        [Tooltip("Quantos chunks descartar por frame (maior = mais agressivo)")]
        [Range(4, 32)] public int dadosRetencaoBatchPerFrame = 16;
        
        [Header("Temporização")]
        [Tooltip("Intervalo em segundos entre checagens de descarte")]
        [Range(0.1f, 2f)] public float unloadInterval = 0.5f;
        
        /// <summary>
        /// Calcula o raio efetivo em chunks baseado na escala do voxel.
        /// Mantém a mesma área em metros independente do tamanho do voxel.
        /// </summary>
        public int CalcularRaioEmChunks(float escalaVoxel) {
            // Lado de cada chunk em metros
            int chunkLadoMetros = Mathf.RoundToInt(ChunkData.Largura * escalaVoxel);
            
            // Quantos chunks necessários para cobrir o raio desejado
            int raioEmChunks = Mathf.CeilToInt(raioDesejadoMetros / chunkLadoMetros);
            
            // Aplica o limite máximo para evitar explosão de memória
            return Mathf.Min(raioEmChunks, maxChunkRadius);
        }
        
        /// <summary>
        /// Retorna informações de debug sobre os cálculos.
        /// </summary>
        public string GetDebugInfo(float escalaVoxel) {
            int chunkLado = Mathf.RoundToInt(ChunkData.Largura * escalaVoxel);
            int raioCalculado = (int)(raioDesejadoMetros / chunkLado);
            int raioEfetivo = CalcularRaioEmChunks(escalaVoxel);
            int totalChunks = (raioEfetivo * 2 + 1) * (raioEfetivo * 2 + 1);
            
            return $"Perfil de Preload:\n" +
                   $"  Escala Voxel: {escalaVoxel}m ({escalaVoxel * 100}cm)\n" +
                   $"  Chunk Lado: {chunkLado}m\n" +
                   $"  Raio Desejado: {raioDesejadoMetros}m\n" +
                   $"  Raio Calculado: {raioCalculado} chunks\n" +
                   $"  Raio Efetivo: {raioEfetivo} chunks (limitado a {maxChunkRadius})\n" +
                   $"  Total Chunks: ~{totalChunks}\n" +
                   $"  Área Efetiva: ~{raioEfetivo * chunkLado}m";
        }
    }
}

