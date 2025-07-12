## 🛠 Helpers usados en WebConTablas

Para facilitar la reutilización de lógica común en los controladores y mejorar la organización, el proyecto cuenta con varios **helpers estáticos** que manejan tareas transversales como validación de sesión, validación de modelos y redirecciones.

### ControllerHelper

- **Funciones principales:**  
  - Obtener el ID del psiquiatra desde la sesión HTTP.  
  - Validar que el modelo sea válido y mostrar errores en consola.  
  - Validar si la sesión está activa o redirigir a login.

- **Uso típico en controladores:**  
  ```csharp
  var idPsiquiatra = ControllerHelper.ObtenerIdPsiquiatraSesionConValidacion(this, HttpContext);
  if (idPsiquiatra == null)
      return RedirectToAction("Login", "Psiquiatras");

  if (!ControllerHelper.ValidarModeloYMostrarErrores(ModelState, this))
      return View(model);
  ```
### RedirectHelper
**Funciones principales:** 
- Centralizar redirecciones hacia la página de login con mensajes de error opcionales.

- **Uso típico:** 

  ```csharp
    return RedirectHelper.RedirigirLogin(this, "Sesión expirada, por favor ingresa nuevamente");
  ```

### SessionHelper
- **Funciones principales:** 
  - Lectura y escritura del ID de psiquiatra en la sesión HTTP.
  - Limpieza de sesión.

- **Uso típico:**

  ```csharp
    SessionHelper.GuardarIdPsiquiatra(HttpContext, psiquiatra.ID_Psiquiatra);
    var id = SessionHelper.ObtenerIdPsiquiatra(HttpContext);
    SessionHelper.LimpiarSesion(HttpContext);
  ```
### Integración en los Controladores
- Todos los controladores que requieran validación de sesión y validación de modelos integran estos helpers para mantener un código limpio y coherente. Por ejemplo, en PacientesController:

  ```csharp
    var idPsiquiatra = ControllerHelper.ObtenerIdPsiquiatraSesionConValidacion(this, _httpContextAccessor.HttpContext!);
    if (idPsiquiatra == null)
        return RedirectHelper.RedirigirLogin(this);

    if (!ControllerHelper.ValidarModeloYMostrarErrores(ModelState, this))
        return View(paciente);
  ```