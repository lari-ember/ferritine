Shader "Ferritine/VoxelZoneOverlay"
{
    // ============================================================
    // VoxelZoneOverlay: Shader para feedback visual de zonas
    // 
    // Fase 2: "O Construtor" - Feedback Imediato
    // 
    // Este shader pinta o topo dos voxels com cores representando
    // as zonas do CityLayer. É muito mais performático que criar
    // centenas de GameObjects de "chão colorido".
    // 
    // Conceito: O shader lê os dados de zona de uma textura ou
    // buffer e aplica a cor correspondente ao topo de cada voxel.
    // ============================================================
    
    Properties
    {
        _MainTex ("Texture Atlas", 2D) = "white" {}
        _VoxelSize ("Voxel Size", Float) = 0.5
        
        [Header(Zone Overlay)]
        _ZoneOverlayStrength ("Zone Overlay Strength", Range(0, 1)) = 0.6
        _ZoneOverlayBlend ("Zone Blend Mode", Range(0, 1)) = 0.5
        
        [Header(Zone Colors)]
        _ResidencialLowColor ("Residencial Baixa", Color) = (0.4, 0.8, 0.4, 1)
        _ResidencialMedColor ("Residencial Média", Color) = (0.2, 0.7, 0.2, 1)
        _ResidencialHighColor ("Residencial Alta", Color) = (0.1, 0.5, 0.1, 1)
        _ComercialLocalColor ("Comercial Local", Color) = (0.4, 0.6, 0.9, 1)
        _ComercialCentralColor ("Comercial Central", Color) = (0.2, 0.4, 0.8, 1)
        _IndustrialLeveColor ("Industrial Leve", Color) = (0.9, 0.9, 0.4, 1)
        _IndustrialPesadaColor ("Industrial Pesada", Color) = (0.9, 0.7, 0.2, 1)
        _MistoColor ("Misto", Color) = (0.7, 0.5, 0.8, 1)
        _ParqueColor ("Parque", Color) = (0.2, 0.9, 0.5, 1)
        _ViaColor ("Via", Color) = (0.5, 0.5, 0.5, 1)
        
        [Header(Animation)]
        _PulseSpeed ("Pulse Speed", Float) = 2.0
        _PulseIntensity ("Pulse Intensity", Range(0, 0.3)) = 0.1
        
        [Header(Grid Overlay)]
        _GridColor ("Grid Color", Color) = (1, 1, 1, 0.2)
        _GridThickness ("Grid Thickness", Range(0, 0.1)) = 0.02
        _ShowGrid ("Show Grid", Range(0, 1)) = 1.0
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 200
        
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows vertex:vert
        #pragma target 3.0
        
        sampler2D _MainTex;
        float _VoxelSize;
        
        // Zone overlay
        float _ZoneOverlayStrength;
        float _ZoneOverlayBlend;
        
        // Zone colors
        float4 _ResidencialLowColor;
        float4 _ResidencialMedColor;
        float4 _ResidencialHighColor;
        float4 _ComercialLocalColor;
        float4 _ComercialCentralColor;
        float4 _IndustrialLeveColor;
        float4 _IndustrialPesadaColor;
        float4 _MistoColor;
        float4 _ParqueColor;
        float4 _ViaColor;
        
        // Animation
        float _PulseSpeed;
        float _PulseIntensity;
        
        // Grid
        float4 _GridColor;
        float _GridThickness;
        float _ShowGrid;
        
        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
            float3 worldNormal;
            float4 zoneData; // x=zoneType, y=isValid, z=isPainted, w=reserved
        };
        
        // Vertex data para passar informações de zona
        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            
            // Passa dados de zona através das cores do vértice
            // R = tipo de zona (0-255)
            // G = validação (0=inválido, 1=válido)
            // B = está pintado (0=não, 1=sim)
            // A = reservado
            o.zoneData = v.color;
        }
        
        // Função para obter cor da zona baseado no índice
        float4 GetZoneColor(float zoneIndex)
        {
            // Convertendo float para int aproximado
            int zone = (int)(zoneIndex * 255.0);
            
            // Mapeamento de ZonaTipo enum para cores
            // 0 = Nenhuma, 1 = ResidencialBaixa, 2 = ResidencialMedia, etc.
            if (zone == 0) return float4(0, 0, 0, 0); // Nenhuma - transparente
            if (zone == 1) return _ResidencialLowColor;
            if (zone == 2) return _ResidencialMedColor;
            if (zone == 3) return _ResidencialHighColor;
            if (zone == 4) return _ComercialLocalColor;
            if (zone == 5) return _ComercialCentralColor;
            if (zone == 6) return _IndustrialLeveColor;
            if (zone == 7) return _IndustrialPesadaColor;
            if (zone == 8) return _MistoColor;
            if (zone == 9) return float4(0.6, 0.5, 0.3, 1); // Rural
            if (zone == 10) return float4(0.8, 0.9, 0.3, 1); // Agricultura
            if (zone == 11) return float4(0.8, 0.6, 0.6, 1); // Especial
            if (zone == 12) return float4(0.8, 0.6, 0.6, 1); // Institucional
            if (zone == 13) return float4(0.7, 0.7, 0.7, 1); // Infraestrutura
            if (zone == 14) return _ParqueColor;
            if (zone == 15) return _ViaColor;
            
            return float4(0.5, 0.5, 0.5, 0.3); // Fallback
        }
        
        // Função para desenhar grid
        float DrawGrid(float3 worldPos)
        {
            if (_ShowGrid < 0.5) return 0;
            
            float2 gridPos = worldPos.xz / _VoxelSize;
            float2 gridFrac = frac(gridPos);
            
            float gridX = step(gridFrac.x, _GridThickness) + step(1 - _GridThickness, gridFrac.x);
            float gridZ = step(gridFrac.y, _GridThickness) + step(1 - _GridThickness, gridFrac.y);
            
            return saturate(gridX + gridZ);
        }
        
        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            // Textura base do voxel
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            
            // Detecta se é face superior (normal apontando para cima)
            float isTopFace = step(0.5, IN.worldNormal.y);
            
            // Obtém dados da zona do vértice
            float zoneType = IN.zoneData.x;
            float isValid = IN.zoneData.y;
            float isPainted = IN.zoneData.z;
            
            // Cor da zona
            float4 zoneColor = GetZoneColor(zoneType);
            
            // Aplica overlay apenas na face superior e se tiver zona pintada
            if (isTopFace > 0.5 && isPainted > 0.5 && zoneColor.a > 0.01)
            {
                // Efeito de pulso para zonas recém-pintadas ou inválidas
                float pulse = 1.0;
                if (isValid < 0.5)
                {
                    // Zonas inválidas pulsam em vermelho
                    pulse = 1.0 + sin(_Time.y * _PulseSpeed * 3.0) * _PulseIntensity * 2.0;
                    zoneColor = lerp(zoneColor, float4(1, 0.2, 0.2, 1), 0.5);
                }
                else
                {
                    pulse = 1.0 + sin(_Time.y * _PulseSpeed) * _PulseIntensity;
                }
                
                // Blend entre textura base e cor da zona
                c.rgb = lerp(c.rgb, zoneColor.rgb * pulse, _ZoneOverlayStrength * zoneColor.a);
            }
            
            // Adiciona grid
            float grid = DrawGrid(IN.worldPos);
            c.rgb = lerp(c.rgb, _GridColor.rgb, grid * _GridColor.a * isTopFace);
            
            o.Albedo = c.rgb;
            o.Alpha = c.a;
            o.Metallic = 0;
            o.Smoothness = 0.1;
        }
        ENDCG
    }
    
    FallBack "Diffuse"
}

