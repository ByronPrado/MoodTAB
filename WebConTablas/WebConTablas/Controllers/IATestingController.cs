using Microsoft.AspNetCore.Mvc;

// Este controlador solo se encarga de mostrar la interfaz del formulario.
// La lógica de POST (guardar en BD y ML) seguirá en DiarioEmocionalController.cs (el API Controller).

public class IATestingController : Controller
{
    // GET: /IATesting/Crear
    [HttpGet]
    public IActionResult Crear()
    {
        // Esto buscará y renderizará la vista en /Views/IATestinga/Crear.cshtml
        return View();
    }
}
