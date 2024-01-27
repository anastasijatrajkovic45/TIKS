namespace WebTemplate.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Threading.Tasks;

[ApiController]
[Route("[controller]")]
public class AktivnostiController : ControllerBase
{
    public Context Context { get; set; }

    public AktivnostiController(Context context)
    {
        Context = context;
    }

    [HttpPost("DodajAktivnostUPutovanje/{id}")]
    public async Task<ActionResult> DodajAktivnostUPutovanje(int id, Aktivnost aktivnost)
    {
        var putovanje = await Context.Putovanja
            .Include(a => a.Aktivnosti)
            .FirstOrDefaultAsync(a => a.Id == id);
        if (aktivnost == null || string.IsNullOrWhiteSpace(aktivnost.Naziv) || aktivnost.Cena <= 0)
        {
            return BadRequest("Nisu uneti svi obavezni podaci.");
        }

        if (putovanje == null)
        {
            return NotFound("Putovanje nije pronađeno.");
        }
        putovanje.Aktivnosti!.Add(aktivnost);
        
        try
        {
            await Context.SaveChangesAsync();
            return Ok("Uspesno dodata aktivnost za putovaje!");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Greška prilikom dodavanja aktivnosti putovanju: {ex.Message}");
        }
    }

    [HttpGet("PreuzmiAktivnostiNaPutovanju/{id}")]
    public async Task<ActionResult> PreuzmiAktivnostiNaPutovanju(int id)
    {
        var putovanje = await Context.Putovanja
            .Include(a => a.Aktivnosti) 
            .FirstOrDefaultAsync(a => a.Id == id);

        if (putovanje == null)
        {
            return NotFound("Putovanje nije pronađeno");
        }

        var aktivnost = putovanje!.Aktivnosti!.ToList();
        return Ok(aktivnost);
    }

    [HttpDelete("ObrisiAktivnost/{id}")]
    public async Task<ActionResult> ObrisiAktivnost(int id)
    {
        var stari = await Context.Aktivnosti.FindAsync(id);

        if (stari != null)
        {
            var naziv = stari.Naziv;
            var cena = stari.Cena;

            Context.Aktivnosti.Remove(stari);
            await Context.SaveChangesAsync();
            return Ok($"Izbrisana je aktivnost: {naziv} {cena}");
        }
        else
        {
            return BadRequest("Nije uspeslo brisanje aktivnosti!");
        }
    }

    [HttpPut("AzurirajAktivnost/{id}")]
    public async Task<ActionResult> AzurirajAktivnost(int id, [FromBody]Aktivnost aktivnost)
    {
        var stari = await Context.Aktivnosti.FindAsync(id);

        if (stari != null)
        {
            stari.Naziv = aktivnost.Naziv;
            stari.Cena = aktivnost.Cena;

            Context.Aktivnosti.Update(stari);
            await Context.SaveChangesAsync();
            return Ok($"Azurirani su podaci o aktivnosti sa ID = {stari.Id}");
        }
        else
        {
            return BadRequest("Nije uspelo azuriranje aktivnosti!");
        }
    }
}
