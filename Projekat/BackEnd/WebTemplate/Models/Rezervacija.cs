namespace Models;

public class Rezervacija
{
    [Key]
    public int Id { get; set; }
   public required string Ime { get; set; }
    public required string Prezime { get; set; }
    public required string BrojTelefona { get; set; }
    public required string Adresa { get; set; }
    public required string Grad { get; set; }
    public required string Email { get; set; }
    public int BrojOsoba { get; set; }


    public Putovanje? Putovanje { get; set; }
}