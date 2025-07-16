using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebConTablas.Services;
using WebConTablas.Models;
using Microsoft.AspNetCore.Http;
using WebConTablas.Helpers;
using WebConTablas.ViewModels;
[RequiereSesionPsiquiatra]
public class PacientesController : Controller
{
    private readonly IPacienteService _pacienteService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PacientesController(IPacienteService pacienteService, IHttpContextAccessor httpContextAccessor)
    {
        _pacienteService = pacienteService;
        _httpContextAccessor = httpContextAccessor;
    }

    private int? ObtenerIdPsiquiatraSesion() =>
        ControllerHelper.ObtenerIdPsiquiatraSesionConValidacion(this, _httpContextAccessor.HttpContext!);

    public async Task<IActionResult> Index()
    {
        var idPsiquiatra = ObtenerIdPsiquiatraSesion();
        if (idPsiquiatra == null)
            return RedirectHelper.RedirigirLogin(this);

        var pacientes = await _pacienteService.ObtenerPacientesPorPsiquiatraAsync(idPsiquiatra.Value);
        return View(pacientes);
    }

    public IActionResult Create() => View();

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var idPsiquiatra = ObtenerIdPsiquiatraSesion();
        if (idPsiquiatra == null)
            return RedirectHelper.RedirigirLogin(this);

        var paciente = await _pacienteService.ObtenerPacientePorIdAsync(id, idPsiquiatra.Value);
        if (paciente == null)
            return NotFound();

        return View(paciente);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Paciente paciente)
    {
        var idPsiquiatra = ObtenerIdPsiquiatraSesion();
        if (idPsiquiatra == null)
            return RedirectHelper.RedirigirLogin(this);

        paciente.ID_Psiquiatra = idPsiquiatra.Value;

        if (!ControllerHelper.ValidarModeloYMostrarErrores(ModelState, this))
            return View(paciente);

        await _pacienteService.CrearPacienteAsync(paciente);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Paciente paciente)
    {
        var idPsiquiatra = ObtenerIdPsiquiatraSesion();
        if (idPsiquiatra == null)
            return RedirectHelper.RedirigirLogin(this);

        var pacienteExistente = await _pacienteService.ObtenerPacientePorIdAsync(paciente.ID_Paciente, idPsiquiatra.Value);
        if (pacienteExistente == null)
            return Unauthorized();

        if (!ControllerHelper.ValidarModeloYMostrarErrores(ModelState, this))
            return View(paciente);

        // Actualiza campos
        pacienteExistente.Nombre = paciente.Nombre;
        pacienteExistente.Diagnostico = paciente.Diagnostico;
        pacienteExistente.Edad = paciente.Edad;
        pacienteExistente.Sexo = paciente.Sexo;
        pacienteExistente.Email = paciente.Email;
        pacienteExistente.Telefono = paciente.Telefono;

        await _pacienteService.EditarPacienteAsync(pacienteExistente);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var idPsiquiatra = ObtenerIdPsiquiatraSesion();
        if (idPsiquiatra == null)
            return RedirectHelper.RedirigirLogin(this);

        await _pacienteService.EliminarPacienteAsync(id, idPsiquiatra.Value);
        return RedirectToAction(nameof(Index));
    }
    // Función para mapear paciente entidad a DTO (puede estar en algún helper o en el mismo controlador)
    PacienteDTO MapPacienteToDTO(Paciente pacienteEntidad)
    {
        return new PacienteDTO
        {
            ID_Paciente = pacienteEntidad.ID_Paciente,
            Nombre = pacienteEntidad.Nombre,
            Diagnostico = pacienteEntidad.Diagnostico,
            Edad = pacienteEntidad.Edad,
            Sexo = pacienteEntidad.Sexo,
            Email = pacienteEntidad.Email,
            Telefono = pacienteEntidad.Telefono,
            DiariosEmocionales = pacienteEntidad.DiariosEmocionales.Select(d => new DiarioEmocionalDTO
            {
                ID_Diario = d.ID_Diario,
                Fecha = d.Fecha,
                Emociones = d.Emociones,
                Pasos = d.Pasos,
                Horas_celular = d.Horas_celular,
                Horas_redes = d.Horas_redes,
                Descripcion = d.Descripcion,
                Estado = d.Estado
            }).ToList()
        };
    }

    public async Task<IActionResult> Details(int id)
    {
        var idPsiquiatra = ObtenerIdPsiquiatraSesion();
        if (idPsiquiatra == null)
            return RedirectToAction("Login", "Psiquiatras");

        var paciente = await _pacienteService.ObtenerPacientePorIdAsync(id, idPsiquiatra.Value);
        if (paciente == null)
            return NotFound();

        // Mapea la entidad Paciente a DTO
        var pacienteDto = MapPacienteToDTO(paciente);

        var desdeFechaTendencia = DateTime.Today.AddDays(-14);

        // Diarios recientes en DTO
        List<DiarioEmocionalDTO> diariosRecientesDto = pacienteDto.DiariosEmocionales
            .Where(d => d.Fecha >= desdeFechaTendencia)
            .OrderBy(d => d.Fecha)
            .ToList();

        // Mapea de DTO a entidad para usar en helpers
        var diariosEntidad = diariosRecientesDto.Select(d => new DiarioEmocional
        {
            ID_Diario = d.ID_Diario,
            Fecha = d.Fecha,
            Emociones = d.Emociones,
            Pasos = d.Pasos,
            Horas_celular = d.Horas_celular.HasValue ? (int?)Convert.ToInt32(d.Horas_celular.Value) : null,
            Horas_redes = d.Horas_redes.HasValue ? (int?)Convert.ToInt32(d.Horas_redes.Value) : null,
            Estado = d.Estado,
            Descripcion = d.Descripcion,
            Hora_dormida = d.Hora_dormida 
        }).ToList();


        var emocionesSet = PacienteReporteHelper.ObtenerEmocionesSet(diariosEntidad);
        var conteo = PacienteReporteHelper.ContarEmociones(diariosEntidad, emocionesSet);
        var emocionesRaw = PacienteReporteHelper.ObtenerEmocionesRaw(diariosEntidad, emocionesSet);
        var emocionesSuavizadas = PacienteReporteHelper.ObtenerEmocionesSuavizadas(emocionesRaw);
        var fechasTendencia = diariosRecientesDto.Select(d => d.Fecha.ToString("dd/MM")).Distinct().ToList();

        var inhibidoPasos = pacienteDto.DiariosEmocionales
            .Where(d => d.Estado == "inhibido" && d.Pasos.HasValue)
            .Select(d => d.Pasos.Value);

        var exaltadoPasos = pacienteDto.DiariosEmocionales
            .Where(d => d.Estado == "exaltado" && d.Pasos.HasValue)
            .Select(d => d.Pasos.Value);

        double mediaInhibido = inhibidoPasos.Any() ? inhibidoPasos.Average() : 0;
        double mediaExaltado = exaltadoPasos.Any() ? exaltadoPasos.Average() : 0;

        var viewModel = new PacienteDetalleViewModel
        {
            Paciente = pacienteDto,
            DiariosRecientes = diariosRecientesDto,
            EmocionesSet = emocionesSet,
            ConteoEmociones = conteo,
            EmocionesSuavizadas = emocionesSuavizadas,
            FechasTendencia = fechasTendencia,
            MediaPasosInhibido = mediaInhibido,
            MediaPasosExaltado = mediaExaltado
        };

        return View(viewModel);
    }




}
