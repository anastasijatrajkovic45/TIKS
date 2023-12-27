namespace Models;

public class Context : DbContext
{
    public Context(DbContextOptions options) : base(options)
    {
        
    }
    public required DbSet<Agencija> Agencije { get; set; }
    public required DbSet<Korisnik> Korisnici { get; set; }
    public required DbSet<Recenzija> Recenzije { get; set; }
    public required DbSet<Putovanje> Putovanja { get; set; }

}