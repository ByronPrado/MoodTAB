public class PlanSeguroModel
{
    public List<string> SenalesAlerta { get; set; } = new();
    public List<string> EstrategiasInternas { get; set; } = new();
    public List<PersonaContacto> PersonasDistraerme { get; set; } = new();
    public List<PersonaContacto> PersonasPedirAyuda { get; set; } = new();
    public List<PersonaContacto> Profesionales { get; set; } = new();
    public List<string> AmbienteSeguro { get; set; } = new();
}

public class PersonaContacto
{
    public string Nombre { get; set; }
    public string Fono { get; set; }
}