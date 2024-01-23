namespace Models;

public class Aktivnost
{
    [Key]
    public int Id { get; set; }
    public required string Naziv { get; set; }
    public required int Cena { get; set; }

    public List<Putovanje>? Putovanje { get; set; }
}