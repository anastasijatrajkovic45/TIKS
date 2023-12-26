namespace Models;
public class Agencija
{
    public int Id { get; set; }
    public string Naziv { get; set; }
    public string Adresa { get; set; }
    public string Email { get; set; }
    public string BrojaTelefona { get; set; }
    public Putovanje Putovanje { get; set; }
    public List<Korisnik>? Korisnici { get; set; }

}