namespace Models;
public class Recenzija
{
    public int Id { get; set; }
    public string Komentar { get; set; }
    public int Ocena { get; set; }
    public Korisnik Korisnik { get; set; }
    
}