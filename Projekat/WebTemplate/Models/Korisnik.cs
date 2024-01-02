namespace Models;
public class Korisnik
{
    [Key]
    public int Id { get; set; }
    public required string Ime { get; set; }
    public required string Prezime { get; set; }
    public required string BrojTelefona { get; set; }
    public required string Adresa { get; set; }
    public required string Grad { get; set; }
    public required string Email { get; set; }

    public Agencija? Agencija { get; set; }
    public List<Recenzija>? Recenzija { get; set; }
    public List<Rezervacija>? Rezervacija { get; set; }

}