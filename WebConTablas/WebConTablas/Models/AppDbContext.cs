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

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Claves primarias explícitas
        modelBuilder.Entity<Psiquiatra>().HasKey(p => p.ID_Psiquiatra);
        modelBuilder.Entity<Paciente>().HasKey(p => p.ID_Paciente);
        modelBuilder.Entity<Pregunta>().HasKey(p => p.ID_Pregunta);
        modelBuilder.Entity<Formulario>().HasKey(f => f.ID_Formulario);
        modelBuilder.Entity<FormularioPregunta>().HasKey(fp => new { fp.ID_Formulario, fp.ID_Pregunta });
        modelBuilder.Entity<FormularioAsignado>().HasKey(fa => fa.ID_Asignacion);
        modelBuilder.Entity<Respuesta>().HasKey(r => r.ID_Respuesta);
        modelBuilder.Entity<DiarioEmocional>().HasKey(d => d.ID_Diario);

        // Relaciones (igual que antes)
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
                ID_Psiquiatra = 1
            }
        );

        // Pregunta
        modelBuilder.Entity<Pregunta>().HasData(
            new Pregunta
            {
                ID_Pregunta = 1,
                Contenido = "¿Cómo te has sentido hoy?",
                Tipo = "texto",
                Created_at = new DateTime(2024, 6, 16)
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
                Created_at = new DateTime(2024, 6, 16)
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
                Fecha_Asignacion = new DateTime(2024, 6, 16),
                Fecha_Limite = new DateTime(2024, 6, 23),
                Estado = "pendiente"
            }
        );

        // DiarioEmocional
        modelBuilder.Entity<DiarioEmocional>().HasData(
            new DiarioEmocional
            {
                ID_Diario = 1,
                ID_Paciente = 1,
                Fecha = new DateTime(2024, 6, 16),
                Emociones = "{\"feliz\":0,\"triste\":1}",
                Descripcion = "Tuve un día difícil",
                Pasos = 3000,
                Horas_celular = 4,
                Horas_redes = 2,
                Hora_dormida = "23:00",
                Estado = "inhibido" // <-- NUEVO, pon el valor que corresponda
            }
        );
    }
}