namespace Models;
public class Putovanje
{
    [Key]
    public int Id { get; set; }
    public required string Mesto { get; set; }
    public required int BrojNocenja { get; set; }
    public required string Prevoz { get; set; }
    public required int Cena { get; set; }

    public Agencija? Agencija { get; set; }
    public List<Aktivnost>? Aktivnosti { get; set; }
}