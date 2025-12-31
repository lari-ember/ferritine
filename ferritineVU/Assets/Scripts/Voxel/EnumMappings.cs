using System;

namespace Voxel
{
    // Este helper centraliza a conversão entre representações legadas (strings do backend
    // ou nomes antigos) e o enum local `ZonaTipo`. Mantê-lo evita espalhar conversões
    // pela base de código e facilita mudanças futuras na modelagem de zona.
    public static class EnumMappings
    {
        // Mapear strings legadas do backend para ZonaTipo local
        // Ex: response da API backend -> ZonaTipo usado pelo CityLayer/DetailWorld
        public static ZonaTipo FromBackendBuildingZoning(string backend)
        {
            if (string.IsNullOrEmpty(backend)) return ZonaTipo.Nenhuma;
            switch (backend.ToLowerInvariant())
            {
                case "residential_low_density": return ZonaTipo.ResidencialBaixaDensidade;
                case "residential_medium_density": return ZonaTipo.ResidencialMediaDensidade;
                case "residential_high_density": return ZonaTipo.ResidencialAltaDensidade;
                case "commercial_local": return ZonaTipo.ComercialLocal;
                case "commercial_central": return ZonaTipo.ComercialCentral;
                case "industrial_light": return ZonaTipo.IndustrialLeve;
                case "industrial_heavy": return ZonaTipo.IndustrialPesada;
                case "mixed_use": return ZonaTipo.Misto;
                case "rural": return ZonaTipo.Rural;
                case "agriculture": return ZonaTipo.Agricultura;
                default: return ZonaTipo.Nenhuma;
            }
        }
    }
}