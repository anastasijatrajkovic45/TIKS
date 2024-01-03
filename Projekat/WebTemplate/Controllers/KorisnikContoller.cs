namespace WebTemplate.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Threading.Tasks;

[ApiController]
[Route("[controller]")]
public class KorisnikContoller : ControllerBase
{
    public Context Context { get; set; }

    public KorisnikContoller(Context context)
    {
        Context = context;
    }

    [HttpPost]
    [Route("DodajKorisnika")]
    public async Task<ActionResult> DodajKorisnika([FromBody]Korisnik korisnik)
    {
        try
        {
            await Context.Korisnici.AddAsync(korisnik);
            await Context.SaveChangesAsync();
            return Ok($"ID novog korisnika je = {korisnik.Id}");
        }
        catch
        {
                return BadRequest("Nije uspelo dodavanje korisnika!");
        }
    }

    [HttpDelete("ObrisiKorisnika/{id}")]
    public async Task<ActionResult> ObrisiKorisnika(int id)
    {
        var stari = await Context.Korisnici.FindAsync(id);

        if (stari != null)
        {
            var ime = stari.Ime;
            var prezime = stari.Prezime;
            
            Context.Korisnici.Remove(stari);
            await Context.SaveChangesAsync();
            return Ok($"Izbrisan je korisnik: {ime} {prezime}");
        }
        else
        {
            return BadRequest("Neuspelo!");
        }
    }

    [HttpGet("PrezumiKorisnike")]
    public async Task<ActionResult> PrezumiKorisnike()
    {
        try
        {
            return Ok(await Context.Korisnici.ToListAsync());
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("AzurirajKorisnika/{id}")]
    public async Task<ActionResult> AzurirajKorisnika(int id, [FromBody]Korisnik korisnik)
    {
        var stari = await Context.Korisnici.FindAsync(id);

        if (stari != null)
        {
            stari.Ime = korisnik.Ime;
            stari.Prezime = korisnik.Prezime;
            stari.Adresa = korisnik.Adresa;
            stari.BrojTelefona = korisnik.BrojTelefona;

            Context.Korisnici.Update(stari);
            await Context.SaveChangesAsync();
            return Ok($"Azurirani su podaci o korisniku sa ID = {stari.Id}");
        }
        else
        {
            return BadRequest("Nije uspelo azuriranje korisnika!");
        }
    }
}
