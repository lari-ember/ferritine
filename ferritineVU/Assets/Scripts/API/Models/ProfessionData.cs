using System;

[Serializable]
public class ProfessionData
{
    // UUID serializado como string no JSON, convertido para Guid
    public string id;
    public string name;
    public string category;
    public float base_salary;
    public string[] required_skills;
    public int education_level;
    public string description;
    
    // Propriedade helper para trabalhar com Guid (safe parsing)
    public Guid GetIdAsGuid() => Guid.TryParse(id, out Guid result) ? result : Guid.Empty;
    
    // Validação de UUID
    public bool HasValidId() => Guid.TryParse(id, out _);
}

