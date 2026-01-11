using UnityEngine;

namespace Voxel {
    // ZonaHelper: utilitários para agrupar categorias do enum ZonaTipo.
    // Permite ao DetailWorld e outras camadas decidirem comportamento sem depender
    // de nomes específicos do enum espalhados pelo projeto.
    // 
    // Fase 2 Expansion: Adicionados métodos para densidade, compatibilidade,
    // e mapeamento de cores para o sistema de pintura de zonas.
    public static class ZonaHelper {
        
        #region Categorias Básicas
        
        public static bool IsResidential(ZonaTipo z) {
            switch (z) {
                case ZonaTipo.ResidencialBaixaDensidade:
                case ZonaTipo.ResidencialMediaDensidade:
                case ZonaTipo.ResidencialAltaDensidade:
                    return true;
                default: return false;
            }
        }

        public static bool IsCommercial(ZonaTipo z) {
            switch (z) {
                case ZonaTipo.ComercialLocal:
                case ZonaTipo.ComercialCentral:
                    return true;
                default: return false;
            }
        }

        public static bool IsIndustrial(ZonaTipo z) {
            switch (z) {
                case ZonaTipo.IndustrialLeve:
                case ZonaTipo.IndustrialPesada:
                    return true;
                default: return false;
            }
        }
        
        public static bool IsMixed(ZonaTipo z) {
            return z == ZonaTipo.Misto;
        }
        
        public static bool IsRural(ZonaTipo z) {
            switch (z) {
                case ZonaTipo.Rural:
                case ZonaTipo.Agricultura:
                    return true;
                default: return false;
            }
        }
        
        public static bool IsPublicSpace(ZonaTipo z) {
            switch (z) {
                case ZonaTipo.Parque:
                case ZonaTipo.Institucional:
                case ZonaTipo.Infraestrutura:
                case ZonaTipo.Via:
                    return true;
                default: return false;
            }
        }
        
        public static bool IsSpecial(ZonaTipo z) {
            switch (z) {
                case ZonaTipo.Especial:
                case ZonaTipo.Historico:
                case ZonaTipo.Turismo:
                    return true;
                default: return false;
            }
        }
        
        #endregion
        
        #region Densidade
        
        /// <summary>
        /// Retorna o nível de densidade da zona (0-3).
        /// 0 = Sem densidade (vazio, via, parque)
        /// 1 = Baixa densidade
        /// 2 = Média densidade
        /// 3 = Alta densidade
        /// </summary>
        public static int GetDensityLevel(ZonaTipo z) {
            switch (z) {
                // Baixa densidade
                case ZonaTipo.ResidencialBaixaDensidade:
                case ZonaTipo.ComercialLocal:
                case ZonaTipo.IndustrialLeve:
                case ZonaTipo.Rural:
                case ZonaTipo.Agricultura:
                    return 1;
                
                // Média densidade
                case ZonaTipo.ResidencialMediaDensidade:
                case ZonaTipo.Misto:
                    return 2;
                
                // Alta densidade
                case ZonaTipo.ResidencialAltaDensidade:
                case ZonaTipo.ComercialCentral:
                case ZonaTipo.IndustrialPesada:
                    return 3;
                
                // Sem densidade (espaços públicos, vias, etc.)
                default:
                    return 0;
            }
        }
        
        /// <summary>
        /// Verifica se a zona permite construção de edifícios.
        /// </summary>
        public static bool AllowsBuildings(ZonaTipo z) {
            switch (z) {
                case ZonaTipo.Nenhuma:
                case ZonaTipo.Via:
                case ZonaTipo.Parque:
                    return false;
                default:
                    return true;
            }
        }
        
        /// <summary>
        /// Retorna a altura máxima permitida (em andares) para a zona.
        /// </summary>
        public static int GetMaxFloors(ZonaTipo z) {
            switch (z) {
                case ZonaTipo.ResidencialBaixaDensidade:
                    return 2;
                case ZonaTipo.ResidencialMediaDensidade:
                    return 6;
                case ZonaTipo.ResidencialAltaDensidade:
                    return 30;
                case ZonaTipo.ComercialLocal:
                    return 3;
                case ZonaTipo.ComercialCentral:
                    return 50;
                case ZonaTipo.IndustrialLeve:
                    return 3;
                case ZonaTipo.IndustrialPesada:
                    return 5;
                case ZonaTipo.Misto:
                    return 12;
                case ZonaTipo.Institucional:
                    return 10;
                case ZonaTipo.Historico:
                    return 4;
                case ZonaTipo.Rural:
                case ZonaTipo.Agricultura:
                    return 2;
                default:
                    return 0;
            }
        }
        
        #endregion
        
        #region Compatibilidade entre Zonas
        
        /// <summary>
        /// Verifica se duas zonas são compatíveis como vizinhas.
        /// Usado para validar zoneamento e calcular penalidades.
        /// </summary>
        public static bool AreCompatible(ZonaTipo a, ZonaTipo b) {
            // Zona vazia é sempre compatível
            if (a == ZonaTipo.Nenhuma || b == ZonaTipo.Nenhuma) return true;
            
            // Parques e vias são sempre compatíveis
            if (a == ZonaTipo.Parque || b == ZonaTipo.Parque) return true;
            if (a == ZonaTipo.Via || b == ZonaTipo.Via) return true;
            
            // Industrial pesada não é compatível com residencial
            if ((a == ZonaTipo.IndustrialPesada && IsResidential(b)) ||
                (b == ZonaTipo.IndustrialPesada && IsResidential(a))) {
                return false;
            }
            
            // Comercial central não é compatível com residencial baixa densidade
            if ((a == ZonaTipo.ComercialCentral && b == ZonaTipo.ResidencialBaixaDensidade) ||
                (b == ZonaTipo.ComercialCentral && a == ZonaTipo.ResidencialBaixaDensidade)) {
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Retorna o nível de poluição gerado pela zona (0-3).
        /// </summary>
        public static int GetPollutionLevel(ZonaTipo z) {
            switch (z) {
                case ZonaTipo.IndustrialPesada:
                    return 3;
                case ZonaTipo.IndustrialLeve:
                    return 2;
                case ZonaTipo.ComercialCentral:
                case ZonaTipo.Via:
                    return 1;
                default:
                    return 0;
            }
        }
        
        /// <summary>
        /// Retorna o nível de ruído gerado pela zona (0-3).
        /// </summary>
        public static int GetNoiseLevel(ZonaTipo z) {
            switch (z) {
                case ZonaTipo.IndustrialPesada:
                case ZonaTipo.ComercialCentral:
                    return 3;
                case ZonaTipo.IndustrialLeve:
                case ZonaTipo.Via:
                    return 2;
                case ZonaTipo.ComercialLocal:
                case ZonaTipo.Misto:
                    return 1;
                default:
                    return 0;
            }
        }
        
        #endregion
        
        #region Cores para Visualização
        
        /// <summary>
        /// Retorna a cor padrão para representar a zona na UI e no terreno.
        /// </summary>
        public static Color GetZoneColor(ZonaTipo z) {
            return ZoneBrush.GetZoneColor(z);
        }
        
        /// <summary>
        /// Retorna um nome amigável para a zona (em português).
        /// </summary>
        public static string GetZoneName(ZonaTipo z) {
            switch (z) {
                case ZonaTipo.Nenhuma: return "Vazio";
                case ZonaTipo.ResidencialBaixaDensidade: return "Residencial - Baixa Densidade";
                case ZonaTipo.ResidencialMediaDensidade: return "Residencial - Média Densidade";
                case ZonaTipo.ResidencialAltaDensidade: return "Residencial - Alta Densidade";
                case ZonaTipo.ComercialLocal: return "Comercial Local";
                case ZonaTipo.ComercialCentral: return "Centro Comercial";
                case ZonaTipo.IndustrialLeve: return "Industrial Leve";
                case ZonaTipo.IndustrialPesada: return "Industrial Pesada";
                case ZonaTipo.Misto: return "Uso Misto";
                case ZonaTipo.Rural: return "Rural";
                case ZonaTipo.Agricultura: return "Agricultura";
                case ZonaTipo.Especial: return "Zona Especial";
                case ZonaTipo.Institucional: return "Institucional";
                case ZonaTipo.Infraestrutura: return "Infraestrutura";
                case ZonaTipo.Parque: return "Parque";
                case ZonaTipo.Via: return "Via Pública";
                case ZonaTipo.Historico: return "Zona Histórica";
                case ZonaTipo.Turismo: return "Zona de Turismo";
                default: return z.ToString();
            }
        }
        
        /// <summary>
        /// Retorna uma descrição curta da zona.
        /// </summary>
        public static string GetZoneDescription(ZonaTipo z) {
            switch (z) {
                case ZonaTipo.Nenhuma: 
                    return "Área não zoneada";
                case ZonaTipo.ResidencialBaixaDensidade: 
                    return "Casas térreas e sobrados. Máx. 2 andares.";
                case ZonaTipo.ResidencialMediaDensidade: 
                    return "Prédios residenciais. Máx. 6 andares.";
                case ZonaTipo.ResidencialAltaDensidade: 
                    return "Torres residenciais. Sem limite de andares.";
                case ZonaTipo.ComercialLocal: 
                    return "Lojas e serviços de bairro. Máx. 3 andares.";
                case ZonaTipo.ComercialCentral: 
                    return "Escritórios e centros comerciais. Torres permitidas.";
                case ZonaTipo.IndustrialLeve: 
                    return "Indústrias limpas e manufatura. Baixa poluição.";
                case ZonaTipo.IndustrialPesada: 
                    return "Indústrias de grande porte. Alta poluição.";
                case ZonaTipo.Misto: 
                    return "Residencial + comercial no mesmo edifício.";
                case ZonaTipo.Parque: 
                    return "Áreas verdes e espaços de lazer.";
                case ZonaTipo.Via: 
                    return "Ruas, avenidas e calçadas.";
                default: 
                    return "";
            }
        }
        
        #endregion
        
        #region Ícones e Atalhos de Teclado
        
        /// <summary>
        /// Retorna o ícone Unicode sugerido para a zona.
        /// </summary>
        public static string GetZoneIcon(ZonaTipo z) {
            switch (z) {
                case ZonaTipo.ResidencialBaixaDensidade: return "🏠";
                case ZonaTipo.ResidencialMediaDensidade: return "🏢";
                case ZonaTipo.ResidencialAltaDensidade: return "🏙️";
                case ZonaTipo.ComercialLocal: return "🏪";
                case ZonaTipo.ComercialCentral: return "🏬";
                case ZonaTipo.IndustrialLeve: return "🏭";
                case ZonaTipo.IndustrialPesada: return "⚙️";
                case ZonaTipo.Misto: return "🔀";
                case ZonaTipo.Rural: return "🌾";
                case ZonaTipo.Agricultura: return "🚜";
                case ZonaTipo.Parque: return "🌳";
                case ZonaTipo.Via: return "🛣️";
                case ZonaTipo.Institucional: return "🏛️";
                case ZonaTipo.Historico: return "🏰";
                case ZonaTipo.Turismo: return "🎡";
                default: return "❓";
            }
        }
        
        /// <summary>
        /// Retorna o atalho de teclado sugerido para selecionar a zona.
        /// </summary>
        public static KeyCode GetZoneHotkey(ZonaTipo z) {
            switch (z) {
                case ZonaTipo.ResidencialBaixaDensidade: return KeyCode.Alpha1;
                case ZonaTipo.ResidencialMediaDensidade: return KeyCode.Alpha2;
                case ZonaTipo.ResidencialAltaDensidade: return KeyCode.Alpha3;
                case ZonaTipo.ComercialLocal: return KeyCode.Alpha4;
                case ZonaTipo.ComercialCentral: return KeyCode.Alpha5;
                case ZonaTipo.IndustrialLeve: return KeyCode.Alpha6;
                case ZonaTipo.IndustrialPesada: return KeyCode.Alpha7;
                case ZonaTipo.Parque: return KeyCode.Alpha8;
                case ZonaTipo.Via: return KeyCode.Alpha9;
                case ZonaTipo.Nenhuma: return KeyCode.Alpha0;
                default: return KeyCode.None;
            }
        }
        
        #endregion
    }
}
