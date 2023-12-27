namespace Models;

public class Agencija
{
    public int Id { get; set; }
    public required string Naziv { get; set; }
    public required string Adresa { get; set; }
    public required string Email { get; set; }
    public required string BrojaTelefona { get; set; }
    public List<Putovanje>? Putovanje { get; set; }
    public List<Korisnik>? Korisnici { get; set; }
    //public required DbSet<Agencija> Agencije { get; set; }

}