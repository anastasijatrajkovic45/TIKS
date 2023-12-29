namespace Models;

public class Context : DbContext
{
        public required DbSet<Agencija> Agencije { get; set; }
        public required DbSet<Aktivnost> Aktivnosti { get; set; }
        public required DbSet<Korisnik> Korisnici { get; set; }
        public required DbSet<Putovanje> Putovanja { get; set; }
        public required DbSet<Recenzija> Recenzije { get; set; }
        public required DbSet<Rezervacija> Rezervacije { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    public Context(DbContextOptions options) : base(options)
    {
        
    }
    
}