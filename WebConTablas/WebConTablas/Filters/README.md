# README - Carpeta Filters

La carpeta **Filters** contiene filtros personalizados para la aplicación, que permiten controlar el comportamiento de las acciones de los controladores antes o después de su ejecución.

---

## Filtros en el proyecto

### RequiereSesionPsiquiatraAttribute

- Es un filtro de acción que hereda de `ActionFilterAttribute`.
- Su función es validar que exista una sesión activa con un psiquiatra autenticado antes de ejecutar la acción del controlador.
- Si no hay un `PsiquiatraId` en la sesión HTTP, redirige automáticamente a la acción `Login` del controlador `Psiquiatras`.
- Esto protege rutas que requieren autenticación para psiquiatras.

---

## Uso

Se aplica el filtro usando el atributo `[RequiereSesionPsiquiatra]` en controladores o acciones para requerir que el usuario tenga sesión activa.

```csharp
[RequiereSesionPsiquiatra]
public class PacientesController : Controller
{
    // Acciones protegidas...
}
```

## Beneficios
- Control centralizado del acceso basado en sesión.
- Evita la duplicación de lógica de verificación en cada acción.
- Mejora la seguridad y la experiencia de usuario al forzar login si no hay sesión.

## Recomendaciones
- Para proyectos más complejos, considerar usar el middleware de autenticación y autorización de ASP.NET Core.
- Agregar filtros para manejo de errores, logging u otras políticas transversales según necesidad.