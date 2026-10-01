using Microsoft.EntityFrameworkCore;
using replay_app;

public class AcademyDbContext : DbContext
{
    public DbSet<Student> Students { get; set; }
    public AcademyDbContext(DbContextOptions<AcademyDbContext> options) 
        : base(options) { }
}