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

    [HttpPost]
    [Route("DodajAktivnost")]
    public async Task<ActionResult> DodajRezervaciju([FromBody]Aktivnost aktivnost)
    {
        try
        {
            await Context.Aktivnosti.AddAsync(aktivnost);
            await Context.SaveChangesAsync();
            return Ok($"ID nove aktivnosti je = {aktivnost.Id}");
        }
        catch
        {
                return BadRequest("Nije uspeslo dodavanje aktivnosti!");
        }
    }

    [HttpGet("PreuzmiAktivnosti")]
    public async Task<ActionResult> PreuzmiAktivnosti()
    {
        try
        {
            return Ok(await Context.Aktivnosti.ToListAsync());
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
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
