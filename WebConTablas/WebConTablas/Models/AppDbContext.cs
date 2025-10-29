using System;
using Microsoft.EntityFrameworkCore;
using WebConTablas.Models;

public class AppDbContext : DbContext
{
    public DbSet<Psiquiatra> Psiquiatras { get; set; }
    public DbSet<Paciente> Pacientes { get; set; }
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<Formulario> Formularios { get; set; }
    public DbSet<FormularioPregunta> FormularioPreguntas { get; set; }
    public DbSet<FormularioAsignado> FormulariosAsignados { get; set; }
    public DbSet<Respuesta> Respuestas { get; set; }
    public DbSet<DiarioEmocional> DiariosEmocionales { get; set; }
    public DbSet<UsuarioExterno> UsuariosExternos { get; set; }
    public DbSet<ComentariosExternos> ComentariosExternos { get; set; }
    public DbSet<Alertas> Alertas { get; set; }
    public DbSet<RecordatoriosPsiquiatra> RecordatoriosPsiquiatra { get; set; }
    public DbSet<Logs> Logs { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Relación: UsuarioExterno 1 - N ComentariosExternos
        modelBuilder.Entity<ComentariosExternos>()
            .HasOne(c => c.UsuarioExterno)
            .WithMany(ue => ue.Comentarios)
            .HasForeignKey(c => c.IdUsuarioExterno)
            .OnDelete(DeleteBehavior.Cascade);
        // Claves primarias explícitas
        modelBuilder.Entity<Psiquiatra>().HasKey(p => p.ID_Psiquiatra);
        modelBuilder.Entity<Paciente>().HasKey(p => p.ID_Paciente);
        modelBuilder.Entity<Pregunta>().HasKey(p => p.ID_Pregunta);
        modelBuilder.Entity<Formulario>().HasKey(f => f.ID_Formulario);
        modelBuilder.Entity<FormularioPregunta>().HasKey(fp => new { fp.ID_Formulario, fp.ID_Pregunta });
        modelBuilder.Entity<FormularioAsignado>().HasKey(fa => fa.ID_Asignacion);
        modelBuilder.Entity<Respuesta>().HasKey(r => r.ID_Respuesta);
        modelBuilder.Entity<DiarioEmocional>().HasKey(d => d.ID_Diario);
        modelBuilder.Entity<UsuarioExterno>().HasKey(ue => ue.IdUsuarioExterno);
        modelBuilder.Entity<Alertas>().HasKey(a => a.ID_Alerta);
        modelBuilder.Entity<RecordatoriosPsiquiatra>().HasKey(rps => rps.ID_RecordatorioPsiquiatra);
        modelBuilder.Entity<Logs>().HasKey(l => l.ID_Log);

        modelBuilder.Entity<UsuarioExterno>()
            .HasOne<Paciente>()
            .WithMany(p => p.UsuariosExternos)
            .HasForeignKey(ue => ue.ID_Paciente)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UsuarioExterno>()
            .HasOne(ue => ue.Psiquiatra)
            .WithMany()
            .HasForeignKey(ue => ue.ID_Psiquiatra)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Paciente>()
            .HasOne(p => p.Psiquiatra)
            .WithMany(q => q.Pacientes)
            .HasForeignKey(p => p.ID_Psiquiatra)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Formulario>()
            .HasOne(f => f.Psiquiatra)
            .WithMany(q => q.Formularios)
            .HasForeignKey(f => f.ID_Psiquiatra)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FormularioPregunta>()
            .HasOne(fp => fp.Formulario)
            .WithMany(f => f.Preguntas)
            .HasForeignKey(fp => fp.ID_Formulario);

        modelBuilder.Entity<FormularioPregunta>()
            .HasOne(fp => fp.Pregunta)
            .WithMany(p => p.Formularios)
            .HasForeignKey(fp => fp.ID_Pregunta);

        modelBuilder.Entity<FormularioAsignado>()
            .HasOne(fa => fa.Formulario)
            .WithMany(f => f.FormulariosAsignados)
            .HasForeignKey(fa => fa.ID_Formulario);

        modelBuilder.Entity<FormularioAsignado>()
            .HasOne(fa => fa.Paciente)
            .WithMany(p => p.FormulariosAsignados)
            .HasForeignKey(fa => fa.ID_Paciente);

        modelBuilder.Entity<Respuesta>()
            .HasOne(r => r.FormularioAsignado)
            .WithMany(fa => fa.Respuestas)
            .HasForeignKey(r => r.ID_Asignacion);

        modelBuilder.Entity<Respuesta>()
            .HasOne(r => r.Pregunta)
            .WithMany(p => p.Respuestas)
            .HasForeignKey(r => r.ID_Pregunta);

        modelBuilder.Entity<Respuesta>()
            .HasIndex(r => new { r.ID_Asignacion, r.ID_Pregunta })
            .IsUnique();

        modelBuilder.Entity<DiarioEmocional>()
            .HasOne(d => d.Paciente)
            .WithMany(p => p.DiariosEmocionales)
            .HasForeignKey(d => d.ID_Paciente);

        modelBuilder.Entity<DiarioEmocional>()
            .HasIndex(d => new { d.ID_Paciente, d.Fecha })
            .IsUnique();

        modelBuilder.Entity<Alertas>()
            .HasOne(a => a.Paciente)
            .WithMany(p => p.Alertas)
            .HasForeignKey(a => a.ID_Paciente);
            
        modelBuilder.Entity<RecordatoriosPsiquiatra>()
            .HasOne(rps => rps.Psiquiatra)
            .WithMany(p => p.RecordatorioPsiquiatra)
            .HasForeignKey(rps => rps.ID_Psiquiatra);

        modelBuilder.Entity<Logs>()
            .HasOne(l => l.Paciente)
            .WithMany(p => p.Logs)
            .HasForeignKey(l => l.ID_Paciente);

        modelBuilder.Entity<Logs>()
            .HasOne(l => l.Psiquiatra)
            .WithMany(p => p.Logs)
            .HasForeignKey(l => l.ID_Psiquiatra);

        // ---------------------------
        // SEED DATA (Datos de ejemplo)
        // ---------------------------

        // Psiquiatra
        modelBuilder.Entity<Psiquiatra>().HasData(
            new Psiquiatra
            {
                ID_Psiquiatra = 1,
                Nombre = "Dr. Juan Pérez",
                Contrasena = "1234",
                Email = "juan@ejemplo.com",
                Telefono = "555-1234"
            }
        );

        // Paciente
        modelBuilder.Entity<Paciente>().HasData(
            new Paciente
            {
                ID_Paciente = 1,
                Nombre = "Ana Gómez",
                Diagnostico = "Ansiedad",
                Edad = 30,
                Sexo = "F",
                Email = "ana@mail.com",
                Telefono = "555-5678",
                ID_Psiquiatra = 1,
                Contrasena = "1234"
            }
        );

        // Pregunta
        modelBuilder.Entity<Pregunta>().HasData(
            new Pregunta
            {
                ID_Pregunta = 1,
                Contenido = "¿Cómo te has sentido hoy?",
                Tipo = "texto",
                Created_at = new DateTime(2024, 6, 16, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        // Formulario
        modelBuilder.Entity<Formulario>().HasData(
            new Formulario
            {
                ID_Formulario = 1,
                ID_Psiquiatra = 1,
                Titulo = "Evaluación inicial",
                Descripcion = "Formulario para evaluar estado inicial del paciente",
                Created_at = new DateTime(2024, 6, 16, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        // FormularioPregunta
        modelBuilder.Entity<FormularioPregunta>().HasData(
            new FormularioPregunta
            {
                ID_Formulario = 1,
                ID_Pregunta = 1,
                Orden = 1
            }
        );

        // FormularioAsignado
        modelBuilder.Entity<FormularioAsignado>().HasData(
            new FormularioAsignado
            {
                ID_Asignacion = 1,
                ID_Formulario = 1,
                ID_Paciente = 1,
                Fecha_Asignacion = new DateTime(2024, 6, 16, 0, 0, 0, DateTimeKind.Utc),
                Fecha_Limite = new DateTime(2024, 6, 23, 0, 0, 0, DateTimeKind.Utc),
                Estado = "pendiente"
            }
        );

        // Alerta
        modelBuilder.Entity<Alertas>().HasData(
            new Alertas
            {
                ID_Alerta = 1,
                ID_Paciente = 1,
                Contenido = "Desvio diario emocional",
                Tipo = "Desvio",
                Estado = "No Visto", // No Visto / Visto / Ignorado
                Created_at = new DateTime(2025, 9, 18, 0, 0, 0, DateTimeKind.Utc),
            }
        );

        // DiarioEmocional
        // Corrección para AppDbContext.cs (Línea 200)
        modelBuilder.Entity<DiarioEmocional>().HasData(
            new DiarioEmocional
            {
                ID_Diario = 1,
                ID_Paciente = 1,
                Fecha = new DateTime(2024, 6, 16, 0, 0, 0, DateTimeKind.Utc),
                Emociones = "{\"feliz\":0,\"triste\":1}",
                Descripcion = "Tuve un día difícil",
                Pasos = 3000,
                Horas_celular = 4,
                Horas_redes = 2,
                
                Hora_dormida = 7.5f, // Ejemplo: 7 horas y media
                
                Estado = "inhibido"
            }
        );
    }
}