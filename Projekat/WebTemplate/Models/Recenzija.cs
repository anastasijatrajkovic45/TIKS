namespace Models;
public class Recenzija
{
    [Key]
    public int Id { get; set; }
    public required string Komentar { get; set; }
    public int Ocena { get; set; }

    public Korisnik? Korisnik { get; set; }
}