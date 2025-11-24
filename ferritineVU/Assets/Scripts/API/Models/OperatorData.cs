using System;

[Serializable]
public class OperatorData
{
    // UUID serializado como string no JSON, convertido para Guid
    public string id;
    public string name;
    public string operator_type;
    public float revenue;
    public float costs;
    public float profit;
    
    // Propriedade helper para trabalhar com Guid (safe parsing)
    public Guid GetIdAsGuid() => Guid.TryParse(id, out Guid result) ? result : Guid.Empty;
    
    // Validação de UUID
    public bool HasValidId() => Guid.TryParse(id, out _);
}

