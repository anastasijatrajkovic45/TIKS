namespace Models;

public class Rezervacija
{
    [Key]
    public int Id { get; set; }
    public required string Smestaj { get; set; }
    public DateTime DatumOd { get; set; }
    public DateTime DatumDo { get; set; }
    public int BrojOsoba { get; set; }

    public List<Korisnik>? Korisnik { get; set; }
    public List<Putovanje>? Putovanje { get; set; }
}