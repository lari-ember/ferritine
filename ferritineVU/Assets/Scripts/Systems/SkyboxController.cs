using UnityEngine;

namespace Systems
{

/// <summary>
/// Controla a transição do skybox entre dia e noite.
/// Trabalha em conjunto com TimeManager para alterar cores e intensidades.
/// </summary>
public class SkyboxController : MonoBehaviour
{
    [Header("Skybox Material")]
    [SerializeField] private Material skyboxMaterial;
    
    [Header("Day/Night Colors")]
    [SerializeField] private Color dayColor = Color.white;
    [SerializeField] private Color nightColor = new Color(0.1f, 0.1f, 0.2f); // Azul escuro noturno
    [SerializeField] private Color sunriseColor = new Color(1f, 0.6f, 0.3f); // Laranja/vermelho
    [SerializeField] private Color sunsetColor = new Color(1f, 0.4f, 0.2f); // Laranja/vermelho
    
    [Header("Lighting")]
    [SerializeField] private Light mainLight; // Luz direcional (sol)
    [SerializeField] private float dayLightIntensity = 1.2f;
    [SerializeField] private float nightLightIntensity = 0.2f;
    
    [Header("Ambient Light")]
    [SerializeField] private Color ambientDayColor = new Color(0.8f, 0.8f, 0.8f);
    [SerializeField] private Color ambientNightColor = new Color(0.2f, 0.2f, 0.3f);
    
    private TimeManager _timeManager;
    private static readonly int TintColor = Shader.PropertyToID("_Tint");
    
    void Awake()
    {
        // Auto-find skybox material if not assigned
        if (skyboxMaterial == null)
        {
            skyboxMaterial = RenderSettings.skybox;
        }
        
        // Auto-find main light
        if (mainLight == null)
        {
            mainLight = FindFirstObjectByType<Light>();
        }
    }
    
    void Start()
    {
        _timeManager = TimeManager.Instance;
        
        if (_timeManager != null)
        {
            _timeManager.OnTimeChanged += UpdateSkybox;
            Debug.Log("[SkyboxController] Conectado ao TimeManager");
        }
        else
        {
            Debug.LogError("[SkyboxController] TimeManager não encontrado na cena!");
        }
        
        // Atualizar skybox inicial
        UpdateSkybox(_timeManager?.CurrentTimeOfDay ?? 12f);
    }
    
    /// <summary>
    /// Atualiza o skybox baseado no tempo do dia.
    /// Chamado pelo TimeManager sempre que o tempo muda.
    /// </summary>
    void UpdateSkybox(float timeOfDay)
    {
        // Determinar cor baseada no tempo
        Color targetColor = GetColorForTime(timeOfDay);
        
        // Aplicar color ao skybox material
        if (skyboxMaterial != null)
        {
            skyboxMaterial.SetColor(TintColor, targetColor);
        }
        
        // Atualizar luz direcional
        if (mainLight != null)
        {
            mainLight.intensity = _timeManager.GetLightIntensity() * dayLightIntensity;
            
            // Rotacionar luz para acompanhar o sol durante o dia
            float sunAngle = (timeOfDay / 24f) * 360f - 90f;
            mainLight.transform.rotation = Quaternion.Euler(sunAngle, 0, 0);
        }
        
        // Atualizar luz ambiente
        float dayNightFactor = _timeManager.IsDaytime() ? 1f : 0f;
        Color ambientColor = Color.Lerp(ambientNightColor, ambientDayColor, dayNightFactor);
        RenderSettings.ambientLight = ambientColor;
    }
    
    /// <summary>
    /// Calcula a cor do skybox para um determinado tempo do dia.
    /// Suavemente transiciona entre cores de dia, nascer do sol, pôr do sol e noite.
    /// </summary>
    Color GetColorForTime(float timeOfDay)
    {
        // Nascer do sol: 5h-6h
        if (timeOfDay >= 5f && timeOfDay < 6f)
        {
            float t = (timeOfDay - 5f) / 1f; // 0-1
            return Color.Lerp(nightColor, sunriseColor, t);
        }
        
        // Dia pleno: 6h-18h
        if (timeOfDay >= 6f && timeOfDay < 12f)
        {
            float t = (timeOfDay - 6f) / 6f; // 0-1 (sunrise -> noon)
            return Color.Lerp(sunriseColor, dayColor, t);
        }
        
        if (timeOfDay >= 12f && timeOfDay < 18f)
        {
            float t = (timeOfDay - 12f) / 6f; // 0-1 (noon -> sunset)
            return Color.Lerp(dayColor, sunsetColor, t);
        }
        
        // Pôr do sol: 18h-20h
        if (timeOfDay >= 18f && timeOfDay < 20f)
        {
            float t = (timeOfDay - 18f) / 2f; // 0-1
            return Color.Lerp(sunsetColor, nightColor, t);
        }
        
        // Noite: 20h-5h (próximo dia)
        return nightColor;
    }
    
    void OnDestroy()
    {
        if (_timeManager != null)
        {
            _timeManager.OnTimeChanged -= UpdateSkybox;
        }
    }
}

}  // namespace Systems

