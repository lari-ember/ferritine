using UnityEngine;
using UnityEngine.InputSystem;

namespace Voxel {
    /// <summary>
    /// Sistema de raycasting otimizado para mundos de voxels.
    /// Implementa o algoritmo DDA (Digital Differential Analyzer) para atravessar
    /// o grid de voxels de forma eficiente, similar ao que jogos como Minecraft usam.
    /// 
    /// Conceito: O ray é um "laser invisível" que viaja pelo mundo 3D, verificando
    /// cada voxel que atravessa até encontrar um sólido.
    /// </summary>
    public static class VoxelRaycast {
        
        /// <summary>
        /// Resultado de um raycast de voxel.
        /// </summary>
        public struct VoxelHitInfo {
            /// <summary>Se o raycast atingiu um voxel sólido</summary>
            public bool Hit;
            
            /// <summary>Coordenadas do voxel atingido (em voxels, não metros)</summary>
            public Vector3Int VoxelPosition;
            
            /// <summary>Ponto exato de impacto no espaço do mundo (em metros)</summary>
            public Vector3 WorldHitPoint;
            
            /// <summary>Normal da face atingida (direção para onde a face aponta)</summary>
            public Vector3 Normal;
            
            /// <summary>Distância do ponto de origem até o hit (em metros)</summary>
            public float Distance;
            
            /// <summary>Tipo do bloco atingido</summary>
            public BlockType BlockType;
            
            /// <summary>Coordenadas do chunk que contém o voxel</summary>
            public Vector2Int ChunkPosition;
            
            /// <summary>
            /// Calcula a posição do voxel adjacente (o voxel "vazio" antes do sólido).
            /// Útil para operações como colocar um bloco adjacente ao clicado.
            /// </summary>
            public Vector3Int GetAdjacentVoxel() {
                return VoxelPosition + Vector3Int.RoundToInt(Normal);
            }
        }

        /// <summary>
        /// Executa um raycast no mundo de voxels usando o algoritmo DDA.
        /// 
        /// Analogia: Imagine um dardo sendo lançado - esta função calcula cada célula
        /// do grid 3D que o dardo atravessa até atingir algo sólido.
        /// 
        /// O algoritmo DDA é O(n) onde n é o número de voxels atravessados,
        /// muito mais eficiente que verificar cada voxel em um volume.
        /// </summary>
        /// <param name="world">Referência ao TerrainWorld para consulta de voxels</param>
        /// <param name="rayOrigin">Ponto de origem do ray (em coordenadas de mundo)</param>
        /// <param name="rayDirection">Direção do ray (normalizada)</param>
        /// <param name="maxDistance">Distância máxima do raycast em metros</param>
        /// <param name="voxelScale">Tamanho de cada voxel em metros</param>
        /// <returns>Informação do hit ou miss</returns>
        public static VoxelHitInfo Raycast(TerrainWorld world, Vector3 rayOrigin, Vector3 rayDirection, float maxDistance, float voxelScale) {
            VoxelHitInfo result = new VoxelHitInfo { Hit = false };
            
            if (world == null || rayDirection.sqrMagnitude < 0.001f) {
                return result;
            }
            
            // Normaliza a direção
            rayDirection = rayDirection.normalized;
            
            // Converte origem do ray para coordenadas de voxel
            Vector3 voxelOrigin = rayOrigin / voxelScale;
            
            // Posição atual no grid de voxels (arredondado para baixo)
            int x = Mathf.FloorToInt(voxelOrigin.x);
            int y = Mathf.FloorToInt(voxelOrigin.y);
            int z = Mathf.FloorToInt(voxelOrigin.z);
            
            // Direção de passo para cada eixo (-1 ou +1)
            int stepX = rayDirection.x >= 0 ? 1 : -1;
            int stepY = rayDirection.y >= 0 ? 1 : -1;
            int stepZ = rayDirection.z >= 0 ? 1 : -1;
            
            // Calcula tMax: distância até a próxima fronteira de voxel em cada eixo
            // e tDelta: quanto adicionar a tMax para atravessar um voxel inteiro
            float tMaxX, tMaxY, tMaxZ;
            float tDeltaX, tDeltaY, tDeltaZ;
            
            // Para X
            if (Mathf.Abs(rayDirection.x) < 0.0001f) {
                tMaxX = float.MaxValue;
                tDeltaX = float.MaxValue;
            } else {
                float nextX = stepX > 0 ? (x + 1) : x;
                tMaxX = (nextX - voxelOrigin.x) / rayDirection.x;
                tDeltaX = Mathf.Abs(1.0f / rayDirection.x);
            }
            
            // Para Y
            if (Mathf.Abs(rayDirection.y) < 0.0001f) {
                tMaxY = float.MaxValue;
                tDeltaY = float.MaxValue;
            } else {
                float nextY = stepY > 0 ? (y + 1) : y;
                tMaxY = (nextY - voxelOrigin.y) / rayDirection.y;
                tDeltaY = Mathf.Abs(1.0f / rayDirection.y);
            }
            
            // Para Z
            if (Mathf.Abs(rayDirection.z) < 0.0001f) {
                tMaxZ = float.MaxValue;
                tDeltaZ = float.MaxValue;
            } else {
                float nextZ = stepZ > 0 ? (z + 1) : z;
                tMaxZ = (nextZ - voxelOrigin.z) / rayDirection.z;
                tDeltaZ = Mathf.Abs(1.0f / rayDirection.z);
            }
            
            // Distância máxima em unidades de voxel
            float maxDistanceVoxels = maxDistance / voxelScale;
            float currentT = 0f;
            
            // Rastreia a última normal (face atravessada)
            Vector3 lastNormal = Vector3.zero;
            
            // Limite de iterações para segurança
            int maxIterations = Mathf.CeilToInt(maxDistanceVoxels * 1.75f) + 100;
            int iterations = 0;
            
            // Loop principal do DDA
            while (currentT < maxDistanceVoxels && iterations < maxIterations) {
                iterations++;
                
                // Verifica se as coordenadas são válidas (não negativas para Y)
                if (y >= 0) {
                    // Consulta o voxel no mundo
                    byte voxelValue = world.GetVoxelAt(x, y, z);
                    
                    // Se encontrou um voxel sólido (não-ar)
                    if (voxelValue != (byte)BlockType.Ar) {
                        result.Hit = true;
                        result.VoxelPosition = new Vector3Int(x, y, z);
                        result.BlockType = (BlockType)voxelValue;
                        result.Normal = lastNormal;
                        result.Distance = currentT * voxelScale;
                        result.WorldHitPoint = rayOrigin + rayDirection * result.Distance;
                        
                        // Calcula posição do chunk
                        result.ChunkPosition = new Vector2Int(
                            Mathf.FloorToInt((float)x / ChunkData.Largura),
                            Mathf.FloorToInt((float)z / ChunkData.Largura)
                        );
                        
                        return result;
                    }
                }
                
                // Avança para o próximo voxel
                // Escolhe o eixo com menor tMax (próxima fronteira mais perto)
                if (tMaxX < tMaxY) {
                    if (tMaxX < tMaxZ) {
                        // Avança em X
                        currentT = tMaxX;
                        x += stepX;
                        tMaxX += tDeltaX;
                        lastNormal = new Vector3(-stepX, 0, 0);
                    } else {
                        // Avança em Z
                        currentT = tMaxZ;
                        z += stepZ;
                        tMaxZ += tDeltaZ;
                        lastNormal = new Vector3(0, 0, -stepZ);
                    }
                } else {
                    if (tMaxY < tMaxZ) {
                        // Avança em Y
                        currentT = tMaxY;
                        y += stepY;
                        tMaxY += tDeltaY;
                        lastNormal = new Vector3(0, -stepY, 0);
                    } else {
                        // Avança em Z
                        currentT = tMaxZ;
                        z += stepZ;
                        tMaxZ += tDeltaZ;
                        lastNormal = new Vector3(0, 0, -stepZ);
                    }
                }
            }
            
            return result;
        }

        /// <summary>
        /// Versão conveniente que aceita um Ray do Unity diretamente.
        /// </summary>
        public static VoxelHitInfo Raycast(TerrainWorld world, Ray ray, float maxDistance, float voxelScale) {
            return Raycast(world, ray.origin, ray.direction, maxDistance, voxelScale);
        }

        /// <summary>
        /// Executa raycast a partir da posição do mouse na tela.
        /// Converte coordenadas 2D do ecrã para um ray 3D usando a câmara.
        /// </summary>
        /// <param name="camera">Câmera principal</param>
        /// <param name="screenPosition">Posição do mouse no ecrã (pixels)</param>
        /// <param name="world">Referência ao TerrainWorld</param>
        /// <param name="maxDistance">Distância máxima em metros</param>
        /// <returns>Informação do hit</returns>
        public static VoxelHitInfo RaycastFromScreen(Camera camera, Vector2 screenPosition, TerrainWorld world, float maxDistance) {
            if (camera == null || world == null) {
                return new VoxelHitInfo { Hit = false };
            }
            
            Ray ray = camera.ScreenPointToRay(screenPosition);
            return Raycast(world, ray, maxDistance, world.escalaVoxel);
        }

        /// <summary>
        /// Executa raycast a partir da posição atual do mouse usando o novo Input System.
        /// Este é o método recomendado para uso com o novo sistema de input do Unity.
        /// 
        /// Exemplo de uso:
        /// <code>
        /// // No Update() ou callback de Input Action:
        /// var hit = VoxelRaycast.RaycastFromMouse(Camera.main, terrainWorld, 100f);
        /// if (hit.Hit) {
        ///     Debug.Log($"Voxel atingido: {hit.VoxelPosition}");
        /// }
        /// </code>
        /// </summary>
        /// <param name="camera">Câmera principal</param>
        /// <param name="world">Referência ao TerrainWorld</param>
        /// <param name="maxDistance">Distância máxima em metros</param>
        /// <returns>Informação do hit</returns>
        public static VoxelHitInfo RaycastFromMouse(Camera camera, TerrainWorld world, float maxDistance) {
            if (camera == null || world == null) {
                return new VoxelHitInfo { Hit = false };
            }
            
            // Usa o novo Input System para obter a posição do mouse
            if (Mouse.current == null) {
                Debug.LogWarning("[VoxelRaycast] Mouse.current é null - Input System pode não estar configurado corretamente");
                return new VoxelHitInfo { Hit = false };
            }
            
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            return RaycastFromScreen(camera, mousePosition, world, maxDistance);
        }

        /// <summary>
        /// Executa raycast usando uma InputAction de posição do mouse.
        /// Permite integração direta com o sistema de Input Actions.
        /// 
        /// Exemplo de uso com Input Actions:
        /// <code>
        /// [SerializeField] private InputActionReference mousePositionAction;
        /// 
        /// void Update() {
        ///     if (clickAction.WasPressedThisFrame()) {
        ///         var hit = VoxelRaycast.RaycastFromInputAction(
        ///             Camera.main, 
        ///             mousePositionAction.action, 
        ///             terrainWorld, 
        ///             100f
        ///         );
        ///     }
        /// }
        /// </code>
        /// </summary>
        /// <param name="camera">Câmera principal</param>
        /// <param name="mousePositionAction">InputAction configurada para ler Vector2 da posição do mouse</param>
        /// <param name="world">Referência ao TerrainWorld</param>
        /// <param name="maxDistance">Distância máxima em metros</param>
        /// <returns>Informação do hit</returns>
        public static VoxelHitInfo RaycastFromInputAction(Camera camera, InputAction mousePositionAction, TerrainWorld world, float maxDistance) {
            if (camera == null || world == null || mousePositionAction == null) {
                return new VoxelHitInfo { Hit = false };
            }
            
            if (!mousePositionAction.enabled) {
                Debug.LogWarning("[VoxelRaycast] InputAction não está habilitada. Chame Enable() primeiro.");
                return new VoxelHitInfo { Hit = false };
            }
            
            Vector2 mousePosition = mousePositionAction.ReadValue<Vector2>();
            return RaycastFromScreen(camera, mousePosition, world, maxDistance);
        }

        /// <summary>
        /// Calcula a posição precisa do voxel a partir de um RaycastHit do Unity Physics.
        /// Usa a normal para garantir que pegamos o voxel correto e não um adjacente.
        /// 
        /// Por que é necessário? Quando clicamos na face de um cubo, o hit.point está
        /// exatamente na superfície. Devido a imprecisões de ponto flutuante, 
        /// FloorToInt pode arredondar para o bloco errado. Subtraindo um pequeno
        /// pedaço da normal, "empurramos" o ponto para dentro do voxel sólido.
        /// </summary>
        /// <param name="hit">RaycastHit do Unity Physics</param>
        /// <param name="voxelScale">Tamanho do voxel em metros</param>
        /// <returns>Coordenadas do voxel em inteiros</returns>
        public static Vector3Int GetVoxelFromPhysicsHit(RaycastHit hit, float voxelScale) {
            // Empurra o ponto ligeiramente para dentro do voxel usando a normal
            Vector3 adjustedPoint = hit.point - hit.normal * (voxelScale * 0.1f);
            
            return new Vector3Int(
                Mathf.FloorToInt(adjustedPoint.x / voxelScale),
                Mathf.FloorToInt(adjustedPoint.y / voxelScale),
                Mathf.FloorToInt(adjustedPoint.z / voxelScale)
            );
        }

        /// <summary>
        /// Calcula a posição do voxel adjacente (onde colocar um novo bloco).
        /// </summary>
        public static Vector3Int GetAdjacentVoxelFromPhysicsHit(RaycastHit hit, float voxelScale) {
            // Empurra o ponto ligeiramente para fora do voxel sólido
            Vector3 adjustedPoint = hit.point + hit.normal * (voxelScale * 0.1f);
            
            return new Vector3Int(
                Mathf.FloorToInt(adjustedPoint.x / voxelScale),
                Mathf.FloorToInt(adjustedPoint.y / voxelScale),
                Mathf.FloorToInt(adjustedPoint.z / voxelScale)
            );
        }

        /// <summary>
        /// Converte coordenadas de voxel para coordenadas de mundo (metros).
        /// </summary>
        public static Vector3 VoxelToWorld(Vector3Int voxelPos, float voxelScale) {
            return new Vector3(
                voxelPos.x * voxelScale,
                voxelPos.y * voxelScale,
                voxelPos.z * voxelScale
            );
        }

        /// <summary>
        /// Converte coordenadas de mundo (metros) para coordenadas de voxel.
        /// </summary>
        public static Vector3Int WorldToVoxel(Vector3 worldPos, float voxelScale) {
            return new Vector3Int(
                Mathf.FloorToInt(worldPos.x / voxelScale),
                Mathf.FloorToInt(worldPos.y / voxelScale),
                Mathf.FloorToInt(worldPos.z / voxelScale)
            );
        }
    }
}

