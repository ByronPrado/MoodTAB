using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Psiquiatras> Psiquiatras { get; set; }
    public DbSet<Paciente> Pacientes { get; set; }
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<DiarioPaciente> DiariosPacientes { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);

        // Relación uno a muchos: un paciente tiene muchas preguntas
        modelBuilder.Entity<Pregunta>()
            .HasOne(p => p.Paciente)
            .WithMany(pac => pac.Preguntas)
            .HasForeignKey(p => p.IdPaciente);
        modelBuilder.Entity<DiarioPaciente>()
            .HasOne(d => d.Paciente)
            .WithMany()
            .HasForeignKey(d => d.IdPaciente);
    }
}
