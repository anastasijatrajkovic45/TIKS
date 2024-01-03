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
            var smestaj = stari.Smestaj;

            Context.Rezervacije.Remove(stari);
            await Context.SaveChangesAsync();
            return Ok($"Izbrisana je rezervacija: {smestaj}");
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
            stari.Smestaj = rezervacija.Smestaj;
            stari.DatumOd = rezervacija.DatumOd;
            stari.DatumDo = rezervacija.DatumDo;
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
