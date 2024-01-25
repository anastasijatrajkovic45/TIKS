namespace Models;

public class Agencija
{
    [Key]
    public int Id { get; set; }
    public required string Naziv { get; set; }
    public required string Adresa { get; set; }
    public required string Grad { get; set; }
    public required string Email { get; set; }
    public required string BrojTelefona { get; set; }
    [JsonIgnore]
    public List<Putovanje>? Putovanje { get; set; }
}