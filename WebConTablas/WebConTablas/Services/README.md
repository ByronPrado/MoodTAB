# README - Carpeta Services

La carpeta **Services** contiene la lógica de negocio central de la aplicación. Cada servicio encapsula operaciones específicas relacionadas con los modelos de datos y el acceso a la base de datos usando Entity Framework Core. Esto permite mantener un código modular, desacoplado y fácil de mantener.

---

## Estructura general

- Cada servicio implementa una interfaz que define su contrato (métodos públicos).
- Se inyecta el `AppDbContext` para interactuar con la base de datos.
- Los métodos usan `async/await` para operaciones asíncronas.
- Servicios manejan operaciones CRUD y consultas con carga de relaciones (`Include`).
- La lógica de negocio está separada de los controladores, facilitando pruebas y mantenimiento.

---

## Servicios y sus responsabilidades

### 1. **FormularioAsignadoService**

- Gestiona asignaciones de formularios a pacientes.
- Métodos principales:
  - `ObtenerTodosAsync()`: obtiene todas las asignaciones con detalles.
  - `AsignarFormularioAsync(...)`: crea una nueva asignación.
  - `EliminarAsync(id)`: elimina una asignación por ID.

---

### 2. **FormularioPreguntaService**

- Gestiona la relación de preguntas asignadas a formularios.
- Métodos principales:
  - `ObtenerFormularioConPreguntas(id)`: devuelve un formulario con sus preguntas cargadas.
  - `ObtenerPreguntasNoAsignadas(formularioId)`: devuelve preguntas no asignadas al formulario.
  - `AsignarPreguntasAsync(formularioId, preguntasIds)`: asigna preguntas a un formulario.
  - `DesasignarPreguntaAsync(formularioId, preguntaId)`: elimina una asignación pregunta-formulario.

---

### 3. **FormularioService**

- CRUD para formularios, incluyendo carga de psiquiatra y preguntas relacionadas.
- Métodos principales:
  - `ObtenerTodosAsync()`: lista todos los formularios con psiquiatras.
  - `ObtenerDetallesAsync(id)`: detalles de formulario con relaciones.
  - `CrearAsync(formulario)`, `EditarAsync(formulario)`, `EliminarAsync(id)` para gestión de formularios.

---

### 4. **PacienteService**

- Gestiona pacientes asociados a un psiquiatra.
- Métodos principales:
  - `ObtenerPacientesPorPsiquiatraAsync(idPsiquiatra)`: pacientes con datos relacionados.
  - `ObtenerPacientePorIdAsync(id, idPsiquiatra)`: paciente específico validando psiquiatra.
  - CRUD básico: `CrearPacienteAsync`, `EditarPacienteAsync`, `EliminarPacienteAsync`.

---

### 5. **PreguntaService**

- CRUD para preguntas.
- Métodos principales:
  - `ObtenerTodasAsync()`: lista todas las preguntas.
  - `ObtenerPorIdAsync(id)`: obtiene pregunta por ID.
  - `CrearAsync`, `EditarAsync`, `EliminarAsync` para gestionar preguntas.

---

### 6. **PsiquiatraService**

- CRUD para psiquiatras y búsqueda por credenciales (login).
- Métodos principales:
  - `ObtenerTodosAsync()`: lista todos los psiquiatras.
  - `BuscarPorCredencialesAsync(nombre, email)`: login por nombre y correo.
  - `ObtenerPorIdAsync(id)`, `CrearAsync`, `EditarAsync`, `EliminarAsync`.

---

### 7. **SesionService**

- Servicio simple para obtener datos de sesión HTTP.
- Método principal:
  - `ObtenerPsiquiatraId()`: retorna el ID del psiquiatra en sesión (si existe).

---

### 8. **VistaDatosService**

- Servicio para obtener listas básicas usadas en vistas (datos auxiliares).
- Métodos principales:
  - `ObtenerPsiquiatrasAsync()`
  - `ObtenerPacientesAsync()`
  - `ObtenerFormulariosAsync()`

---

## Buenas prácticas observadas

- Uso de interfaces para facilitar pruebas unitarias y mantenimiento.
- Métodos asíncronos para operaciones de base de datos.
- Inclusión explícita de relaciones mediante `.Include()` y `.ThenInclude()`.
- Validación y manejo de entidades antes de operaciones de eliminación.
- Código limpio y modular para fácil extensión.

---

## Recomendaciones

- Documentar con XML comentarios cada método para mejorar mantenibilidad.
- Manejar excepciones y errores en los servicios para robustez.
- Considerar patrones de diseño como Unit of Work o Repository si el proyecto escala.
- Escribir pruebas unitarias para los servicios clave.

---