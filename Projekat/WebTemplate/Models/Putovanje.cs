namespace Models;
public class Putovanje
{
    public int Id { get; set; }
    public required string Destinacija { get; set; }
    public DateTime DatumPolaska { get; set; }
    public DateTime DatumPovratka { get; set; }
    public Agencija? Agencija { get; set; }
    //public required DbSet<Putovanje> Putovanja { get; set; }
}