using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Linq;

public static class ControllerHelper
{
    public static int? ObtenerIdPsiquiatraSesion(HttpContext httpContext)
    {
        return httpContext.Session.GetInt32("PsiquiatraId");
    }

    // Método que devuelve el id o redirige si no está
    public static int? ObtenerIdPsiquiatraSesionConValidacion(Controller controller, HttpContext httpContext)
    {
        var idPsiquiatra = ObtenerIdPsiquiatraSesion(httpContext);
        if (idPsiquiatra == null)
            return null;
        return idPsiquiatra;
    }

    // Método para validar modelo y mostrar errores (devuelve bool)
    public static bool ValidarModeloYMostrarErrores(ModelStateDictionary modelState, Controller controller)
    {
        if (!modelState.IsValid)
        {
            foreach (var error in modelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine("Error de validación: " + error.ErrorMessage);
            }
            return false;
        }
        return true;
    }

    // Mantengo el método original por si usas en otros lugares
    public static IActionResult ValidarSesionORedirigir(Controller controller, HttpContext httpContext)
    {
        var idPsiquiatra = ObtenerIdPsiquiatraSesion(httpContext);
        if (idPsiquiatra == null)
            return controller.RedirectToAction("Login", "Psiquiatras");
        return null; // Sesión válida
    }

    public static void MostrarErroresValidacion(Controller controller)
    {
        foreach (var error in controller.ModelState.Values.SelectMany(v => v.Errors))
        {
            Console.WriteLine("Error de validación: " + error.ErrorMessage);
        }
    }
}
