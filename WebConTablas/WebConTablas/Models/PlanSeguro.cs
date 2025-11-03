// En una nueva carpeta "ViewModels" o similar
public class PlanSeguridadViewModel
{
    public List<string> SenalesAlerta { get; set; } = new List<string>();
    public List<string> EstrategiasInternas { get; set; } = new List<string>();
    public List<ContactoPlan> PersonasDistraerme { get; set; } = new List<ContactoPlan>();
    public List<ContactoPlan> PersonasPedirAyuda { get; set; } = new List<ContactoPlan>();
    public List<ContactoPlan> Profesionales { get; set; } = new List<ContactoPlan>();
    public List<string> AmbienteSeguro { get; set; } = new List<string>();
}

public class ContactoPlan
{
    public string Nombre { get; set; } = "";
    public string Fono { get; set; } = "";
}