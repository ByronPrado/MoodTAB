public static class SessionHelper
{
    public static int? ObtenerIdPsiquiatra(HttpContext context) =>
        context.Session.GetInt32("PsiquiatraId");

    public static void GuardarIdPsiquiatra(HttpContext context, int id) =>
        context.Session.SetInt32("PsiquiatraId", id);

    public static void LimpiarSesion(HttpContext context) =>
        context.Session.Clear();
}
