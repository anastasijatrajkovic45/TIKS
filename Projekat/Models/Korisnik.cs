namespace Models;
public class Korisnik
{
    public int Id { get; set; }
    public string Ime { get; set; }
    public string Prezime { get; set; }
    public string BrojTelefona { get; set; }
    public string Adresa { get; set; }
    public string Email { get; set; }
    public Agencija Agencija { get; set; }
    public Recenzija Recenzija { get; set; }
}