namespace WebTemplate.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("[controller]")]
public class PutovanjeController : ControllerBase
{
    public Context Context { get; set; }
    //private readonly ILogger<PutovanjeController> _logger;

    //public PutovanjeController(Context context, ILogger<PutovanjeController> logger)
    //{
    //    Context = context;
    //    _logger = logger;
    //}
    public PutovanjeController (Context context)
    {
        Context = context;
    }

    [HttpPost]
    [Route("DodajPutovanje")]
    public async Task<ActionResult> DodajPutovanje([FromBody]Putovanje putovanje)
    {
        if (putovanje == null || string.IsNullOrWhiteSpace(putovanje.Mesto) || string.IsNullOrWhiteSpace(putovanje.Slika) || string.IsNullOrWhiteSpace(putovanje.Prevoz) || putovanje.BrojNocenja <= 0 || putovanje.Cena <= 0)
        {
            return BadRequest("Nisu uneti svi obavezni podaci.");
        }
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
        var agencija = await Context.Agencije
            .Include(a => a.Putovanje)
            .FirstOrDefaultAsync(a => a.Id == id);
        if (putovanje == null || string.IsNullOrWhiteSpace(putovanje.Mesto) || string.IsNullOrWhiteSpace(putovanje.Slika) || string.IsNullOrWhiteSpace(putovanje.Prevoz) || putovanje.BrojNocenja <= 0 || putovanje.Cena <= 0)
        {
            return BadRequest("Nisu uneti svi obavezni podaci.");
        }

        if (agencija == null)
        {
            return NotFound("Agencija nije pronađena");
        }
        agencija.Putovanje!.Add(putovanje);

        try
        {
            await Context.SaveChangesAsync();
            return Ok(new { Message = "Putovanje za agenciju je uspesno dodato." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Greška prilikom dodavanja putovanja agenciji: {ex.Message}");
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
    
    [HttpGet("PreuzmiPutovanjaAgencije/{id}")]
    public async Task<ActionResult> PreuzmiPutovanjaAgencije(int id)
    {
        var agencija = await Context.Agencije
            .Include(a => a.Putovanje) 
            .FirstOrDefaultAsync(a => a.Id == id);

        if (agencija == null)
        {
            return NotFound("Agencija nije pronađena.");
        }

        var putovanja = agencija!.Putovanje!.ToList();
        return Ok(putovanja);
    }

    [HttpPut("AzurirajPutovanje/{id}")]
    public async Task<ActionResult> AzurirajPutovanje(int id, [FromBody]Putovanje putovanje)
    {
        var stari = await Context.Putovanja.FindAsync(id);

        if (stari != null)
        {
            stari.Slika = putovanje.Slika;
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
            return BadRequest("Nije uspelo azuriranje putovanja!");
        }
    }
}
