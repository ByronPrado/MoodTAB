using Microsoft.AspNetCore.Mvc;


public static class RedirectHelper
{
    public static IActionResult RedirigirLogin(Controller controller, string mensajeError = null)
    {
        if (mensajeError != null)
            controller.TempData["Error"] = mensajeError;
        return controller.RedirectToAction("Login", "Psiquiatras");
    }
}
