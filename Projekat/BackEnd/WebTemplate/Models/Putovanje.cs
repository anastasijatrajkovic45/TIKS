namespace Models;
public class Putovanje
{
    [Key]
    public int Id { get; set; }
    public string Slika { get; set; }
    public string Mesto { get; set; }
    public required int BrojNocenja { get; set; }
    public string Prevoz { get; set; }
    public required int Cena { get; set; }

    [JsonIgnore]
    public Agencija? Agencija { get; set; }
    [JsonIgnore]
    public List<Aktivnost>? Aktivnosti { get; set; }
    [JsonIgnore]
    public List<Recenzija>? Recenzije { get; set; }
    [JsonIgnore]
    public List<Rezervacija>? Rezervacije { get; set; }
}