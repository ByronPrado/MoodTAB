using System;
using System.ComponentModel.DataAnnotations;

public class DiarioPaciente
{
    public int Id { get; set; }

    [Required]
    public int IdPaciente { get; set; }
    public Paciente? Paciente { get; set; }

    [Required]
    public DateTime Fecha { get; set; }

    [Required]
    public string Texto { get; set; }

    public string Tags { get; set; } // Ejemplo: "Feliz,Triste"

    public string Pasos { get; set; }

    public double HorasCelular { get; set; }

    public int DesbloqueosCelular { get; set; }

    public double HorasRedesSociales { get; set; }

    public double HorasSueno { get; set; }
}