namespace Models;
public class Korisnik
{
    public int Id { get; set; }
    public required string Ime { get; set; }
    public required string Prezime { get; set; }
    public required string BrojTelefona { get; set; }
    public required string Adresa { get; set; }
    public required string Email { get; set; }
    public Agencija? Agencija { get; set; }
    public List<Recenzija>? Recenzija { get; set; }

}