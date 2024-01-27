namespace WebTemplate.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Threading.Tasks;

[ApiController]
[Route("[controller]")]
public class RecenzijaController : ControllerBase
{
    public Context Context { get; set; }

    public RecenzijaController(Context context)
    {
        Context = context;
    }

    [HttpPost("DodajRecenzijuUPutovanje/{id}")]
    public async Task<ActionResult> DodajRecenzijuUPutovanje(int id, Recenzija recenzija)
    {
        var putovanje = await Context.Putovanja
            .Include(a => a.Recenzije)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (recenzija == null || string.IsNullOrWhiteSpace(recenzija.Korisnik) || string.IsNullOrWhiteSpace(recenzija.Komentar) || recenzija.Ocena <= 0 || recenzija.Ocena >= 10)
        {
            return BadRequest("Nisu uneti svi obavezni podaci.");
        }

        if (putovanje == null)
        {
            return NotFound("Putovanje nije pronađeno.");
        }
        putovanje.Recenzije!.Add(recenzija);
        
        try
        {
            await Context.SaveChangesAsync();
            return Ok("Uspesno dodata recenzija za putovaje!");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Greška prilikom dodavanja recenzije putovanju: {ex.Message}");
        }
    }

    [HttpGet("PreuzmiRecenzijeNaPutovanju/{id}")]
    public async Task<ActionResult> PreuzmiRecenzijeNaPutovanju(int id)
    {
        var putovanje = await Context.Putovanja
            .Include(a => a.Recenzije) 
            .FirstOrDefaultAsync(a => a.Id == id);

        if (putovanje == null)
        {
            return NotFound("Putovanje nije pronađeno");
        }

        var recenzija = putovanje!.Recenzije!.ToList();
        return Ok(recenzija);
    }

    [HttpDelete("ObrisiRecenziju/{id}")]
    public async Task<ActionResult> ObrisiRecenziju(int id)
    {
        var stari = await Context.Recenzije.FindAsync(id);

        if (stari != null)
        {
            var korisnik = stari.Korisnik;
            var komentar = stari.Komentar;
            var ocena = stari.Ocena;

            Context.Recenzije.Remove(stari);
            await Context.SaveChangesAsync();
            return Ok($"Izbrisana je recenzija korisnika: {korisnik}");
        }
        else
        {
            return BadRequest("Nije uspeslo brisanje recenzije!");
        }
    }

    [HttpPut("AzurirajRecenziju/{id}")]
    public async Task<ActionResult> AzurirajRecenziju(int id, [FromBody]Recenzija recenzija)
    {
        var stari = await Context.Recenzije.FindAsync(id);

        if (stari != null)
        {
            stari.Korisnik = recenzija.Korisnik;
            stari.Komentar = recenzija.Komentar;
            stari.Ocena = recenzija.Ocena;

            Context.Recenzije.Update(stari);
            await Context.SaveChangesAsync();
            return Ok($"Azurirani su podaci o recenziji sa ID = {stari.Id}");
        }
        else
        {
            return BadRequest("Nije uspelo azuriranje recenzije!");
        }
    }
}
