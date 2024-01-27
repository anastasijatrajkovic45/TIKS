namespace WebTemplate.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Threading.Tasks;

[ApiController]
[Route("[controller]")]
public class RezervacijaController : ControllerBase
{
    public Context Context { get; set; }

    public RezervacijaController(Context context)
    {
        Context = context;
    }

    [HttpPost]
    [Route("DodajRezervaciju")]
    public async Task<ActionResult> DodajRezervaciju([FromBody]Rezervacija rezervacija)
    {
        try
        {
            await Context.Rezervacije.AddAsync(rezervacija);
            await Context.SaveChangesAsync();
            return Ok($"ID nove rezervacije je = {rezervacija.Id}");
        }
        catch
        {
                return BadRequest("Nije uspelo dodavanje rezervacije!");
        }
    }

    [HttpPost("DodajRezervacijuPutovanja/{id}")]
    public async Task<ActionResult> DodajRezervacijuPutovanja(int id, Rezervacija rezervacija)
    {
        var putovanje = await Context.Putovanja
            .Include(a => a.Rezervacije)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (putovanje == null)
        {
            return NotFound("Putovanje nije pronađena");
        }
        putovanje.Rezervacije!.Add(rezervacija);
        
        try
        {
            await Context.SaveChangesAsync();
            return Ok("Rezervacija za putovanje je uspesno dodata.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Greška prilikom dodavanja rezervacije putovanja: {ex.Message}");
        }
    }

    [HttpGet("PreuzmiRezervacijePutovanja/{id}")]
    public async Task<ActionResult> PreuzmiRezervacijePutovanja(int id)
    {
        var putovanje = await Context.Putovanja
            .Include(a => a.Rezervacije) 
            .FirstOrDefaultAsync(a => a.Id == id);

        if (putovanje == null)
        {
            return NotFound("Putovanje nije pronađeno");
        }

        var rezervacija = putovanje!.Rezervacije!.ToList();
        return Ok(rezervacija);
    }

    [HttpGet("PreuzmiRezervacije")]
    public async Task<ActionResult> PreuzmiRezervacije()
    {
        try
        {
            return Ok(await Context.Putovanja.ToListAsync());
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("ObrisiRezervaicju/{id}")]
    public async Task<ActionResult> ObrisiRezervaciju(int id)
    {
        var stari = await Context.Rezervacije.FindAsync(id);

        if (stari != null)
        {
            var ime = stari.Ime;
            var prezime = stari.Prezime;

            Context.Rezervacije.Remove(stari);
            await Context.SaveChangesAsync();
            return Ok($"Izbrisana je rezervacija korisnika: {ime} {prezime}");
        }
        else
        {
            return BadRequest("Nije uspelo brisanje rezervacije!");
        }
    }

    [HttpPut("AzurirajRezervaciju/{id}")]
    public async Task<ActionResult> AzurirajRezervaciju(int id, [FromBody]Rezervacija rezervacija)
    {
        var stari = await Context.Rezervacije.FindAsync(id);

        if (stari != null)
        {
            stari.Ime = rezervacija.Ime;
            stari.Prezime = rezervacija.Prezime;
            stari.Adresa = rezervacija.Adresa;
            stari.Grad = rezervacija.Grad;
            stari.BrojTelefona = rezervacija.BrojTelefona;
            stari.Email = rezervacija.Email;
            stari.BrojOsoba = rezervacija.BrojOsoba;

            Context.Rezervacije.Update(stari);
            await Context.SaveChangesAsync();
            return Ok($"Azurirani su podaci o rezervaciji sa ID = {stari.Id}");
        }
        else
        {
            return BadRequest("Nije uspelo azuriranje rezervacije!");
        }
    }
}
