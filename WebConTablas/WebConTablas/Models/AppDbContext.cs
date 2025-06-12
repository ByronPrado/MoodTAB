using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Psiquiatras> Psiquiatras { get; set; }
    public DbSet<Paciente> Pacientes { get; set; }
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<PacientePregunta> PacientePreguntas { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Clave primaria compuesta
        modelBuilder.Entity<PacientePregunta>()
            .HasKey(pp => new { pp.PacienteId, pp.PreguntaId });

        // Relaciones
        modelBuilder.Entity<PacientePregunta>()
            .HasOne(pp => pp.Paciente)
            .WithMany(p => p.PacientePreguntas)
            .HasForeignKey(pp => pp.PacienteId);

        modelBuilder.Entity<PacientePregunta>()
            .HasOne(pp => pp.Pregunta)
            .WithMany(p => p.PacientePreguntas)
            .HasForeignKey(pp => pp.PreguntaId);
    }
}
