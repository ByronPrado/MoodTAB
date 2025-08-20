namespace MoodTAB.Services
{
    public interface IAuthService
    {
        Task<(bool success, string log, PacienteDto user)> LoginAsync(string nombre, string email);
        Task LogoutAsync();
    }
}