# README - Carpeta Controllers

Esta carpeta contiene los controladores que gestionan la lógica de la aplicación y la interacción con el cliente. Está dividida en dos partes principales:

- **API Controllers:** Controladores que exponen endpoints REST para ser consumidos por clientes externos o frontend SPA.
- **MVC Controllers:** Controladores que manejan las vistas Razor para la interfaz web tradicional.

---

## Estructura

Controllers/
- Api/ # Controladores API (Web API)
- Mvc/ # Controladores MVC para vistas Razor


---

## API Controllers

Los controladores API usan el atributo `[ApiController]` y rutas bajo `/api/*`. Su propósito principal es proveer datos y operaciones mediante JSON, con respuestas HTTP estándar.

### Lista de Controllers API

| Controller                 | Ruta base           | Funcionalidad principal                                         |
|----------------------------|---------------------|----------------------------------------------------------------|
| **ApiFormularioController** | `/api/formulario`     | Obtiene formularios asignados a pacientes, con detalles.       |
| **ApiPreguntaController**   | `/api/preguntas`      | Devuelve todas las preguntas registradas.                      |
| **ApiTestController**       | `/api/pacientes`      | Controlador de prueba, responde con mensaje de conexión.       |

---

## MVC Controllers

Los controladores MVC heredan de `Controller` y gestionan la interacción con vistas Razor, mostrando interfaces web y gestionando formularios.

### Lista de Controllers MVC

| Controller                     | Funcionalidad principal                                                                                   |
|-------------------------------|----------------------------------------------------------------------------------------------------------|
| **FormularioPreguntasController** | Asigna y desasigna preguntas a formularios, mostrando vistas para la selección.                        |
| **FormulariosAsignadosController** | Lista, asigna y elimina formularios asignados a pacientes.                                             |
| **FormulariosController**           | CRUD completo para formularios, incluyendo validación y carga de datos auxiliares (psiquiatras).      |
| **HomeController**                  | Controla páginas generales: Inicio, Privacidad y Error.                                                |
| **PacientesController**             | CRUD para pacientes, con control de sesión para psiquiatras autenticados.                             |
| **PreguntasController**             | CRUD para preguntas, con validación y manejo de errores.                                              |
| **PsiquiatrasController**           | CRUD para psiquiatras y manejo de login con sesión.                                                  |

---

## Descripción detallada de cada Controller

### API Controllers

#### ApiFormularioController

- Obtiene el formulario asignado más reciente para un paciente, con todas sus preguntas y opciones.
- Endpoint principal: `GET /api/formulario/{pacienteId}`.

#### ApiPreguntaController

- Devuelve todas las preguntas disponibles.
- Endpoint principal: `GET /api/preguntas`.

#### ApiTestController

- Controlador de prueba para verificar la conexión al API.
- Endpoint principal: `GET /api/pacientes` devuelve un mensaje de confirmación.

---

### MVC Controllers

#### FormularioPreguntasController

- Gestiona la asignación/desasignación de preguntas a formularios.
- Acciones para mostrar preguntas disponibles, asignar y desasignar.

#### FormulariosAsignadosController

- Lista y gestiona las asignaciones de formularios a pacientes.
- Permite crear nuevas asignaciones y eliminar existentes.

#### FormulariosController

- Maneja todas las operaciones CRUD para formularios.
- Incluye validación y carga dinámica de datos relacionados (psiquiatras).

#### HomeController

- Controla las páginas de inicio, privacidad y errores.
- Usa logging para registrar eventos.

#### PacientesController

- Maneja CRUD para pacientes.
- Controla sesión y autenticación mediante filtro personalizado y sesiones HTTP.

#### PreguntasController

- CRUD para preguntas.
- Valida entradas y muestra errores de validación.

#### PsiquiatrasController

- CRUD para psiquiatras.
- Incluye manejo de login con sesión y validación de credenciales.

---

## Convenciones generales

- Los controladores API usan `IActionResult` para retornar respuestas HTTP con JSON.
- Los controladores MVC retornan `View()` para renderizar páginas Razor.
- Inyección de dependencias se usa ampliamente para desacoplar lógica y facilitar pruebas.
- Validación de modelos se maneja con `ModelState`.
- Uso de atributos como `[ValidateAntiForgeryToken]` para proteger formularios contra CSRF.
- Manejo de sesiones para controlar acceso y seguridad.

---

## Cómo contribuir y mantener la documentación

- Cada nuevo controller debe incluir documentación XML para métodos públicos.
- Mantener actualizados los README.md por carpeta con descripción clara.
- Usar rutas REST claras y consistentes en los API Controllers.
- Seguir la arquitectura y separación lógica entre API y MVC.

---


