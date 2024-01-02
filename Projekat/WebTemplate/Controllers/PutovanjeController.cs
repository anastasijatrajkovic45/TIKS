namespace WebTemplate.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Threading.Tasks;

[ApiController]
[Route("[controller]")]
public class PutovanjeController : ControllerBase
{
    public Context Context { get; set; }

    public PutovanjeController(Context context)
    {
        Context = context;
    }

    [HttpPost]
    [Route("DodajPutovanje")]
    public async Task<ActionResult> DodajPutovanje([FromBody]Putovanje putovanje)
    {
        try
        {
            await Context.Putovanja.AddAsync(putovanje);
            await Context.SaveChangesAsync();
            return Ok($"ID novog putovanja je = {putovanje.Id}");
        }
        catch
        {
                return BadRequest("Nije uspelo dodavanje putovanja!");
        }
    }
    [HttpPost]
    [Route("DodajPutovanjeAgenciji/{id}")]
    public async Task<ActionResult> DodajPutovanjeAgenciji([FromBody] Putovanje putovanje, int id)
    {
        var agencija = await Context.Agencije.FindAsync(id);
        try
        {
            putovanje.Agencija=agencija;
            agencija.Putovanje.Add(putovanje);
            await Context.Putovanja.AddAsync(putovanje);
            await Context.SaveChangesAsync();
            return Ok($"ID novog putovanja je = {putovanje.Id}");
        }
        catch
        {
            return BadRequest("Nije uspelo dodavanje putovanja!");
        }
    }

    [HttpDelete("ObrisiPutovanje/{id}")]
    public async Task<ActionResult> ObrisiPutovanje(int id)
    {
        var stari = await Context.Putovanja.FindAsync(id);

        if (stari != null)
        {
            var mesto = stari.Mesto;

            Context.Putovanja.Remove(stari);
            await Context.SaveChangesAsync();
            return Ok($"Izbrisano je putovanje na mestu: {mesto}");
        }
        else
        {
            return BadRequest("Nije uspelo brisanje putovanja!");
        }
    }

    [HttpGet("PreuzmiPutovanja")]
    public async Task<ActionResult> PreuzmiPutovanja()
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

    [HttpPut("AzurirajPutovanje/{id}")]
    public async Task<ActionResult> AzurirajPutovanje(int id, [FromBody]Putovanje putovanje)
    {
        var stari = await Context.Putovanja.FindAsync(id);

        if (stari != null)
        {
            stari.Mesto = putovanje.Mesto;
            stari.BrojNocenja = putovanje.BrojNocenja;
            stari.Prevoz = putovanje.Prevoz;
            stari.Cena = putovanje.Cena;

            Context.Putovanja.Update(stari);
            await Context.SaveChangesAsync();
            return Ok($"Azurirano je putovanje sa ID = {stari.Id}");
        }
        else
        {
            return BadRequest("Nije uspeslo azuriranje putovanja!");
        }
    }
}
