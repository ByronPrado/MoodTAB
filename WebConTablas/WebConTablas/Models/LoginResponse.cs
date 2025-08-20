namespace WebConTablas.Models
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public required Paciente User { get; set; }
    } 
    public class PacienteDto
    {
        public int ID_Paciente { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
    }
}