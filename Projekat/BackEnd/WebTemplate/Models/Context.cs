namespace Models;

public class Context : DbContext
{
        public virtual DbSet<Agencija> Agencije { get; set; }
        public virtual DbSet<Aktivnost> Aktivnosti { get; set; }
        public virtual DbSet<Putovanje> Putovanja { get; set; }
        public virtual DbSet<Recenzija> Recenzije { get; set; }
        public virtual DbSet<Rezervacija> Rezervacije { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    public Context(DbContextOptions options) : base(options)
    {
        
    }
    
}