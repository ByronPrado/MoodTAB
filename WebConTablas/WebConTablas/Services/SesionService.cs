using Microsoft.AspNetCore.Http;

public class SesionService : ISesionService
{
    private readonly IHttpContextAccessor _http;
    public SesionService(IHttpContextAccessor http) => _http = http;

    public int? ObtenerPsiquiatraId()
        => _http.HttpContext?.Session.GetInt32("PsiquiatraId");
}
