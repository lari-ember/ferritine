namespace Voxel {
    // ZonaHelper: utilitários para agrupar categorias do enum ZonaTipo.
    // Permite ao DetailWorld e outras camadas decidirem comportamento sem depender
    // de nomes específicos do enum espalhados pelo projeto.
    public static class ZonaHelper {
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
    }
}
