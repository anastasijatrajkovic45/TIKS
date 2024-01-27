namespace WebTemplate.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Threading.Tasks;

[ApiController]
[Route("[controller]")]
public class AgencijaContoller : ControllerBase
{
    public Context Context { get; set; }

    public AgencijaContoller(Context context)
    {
        Context = context;
    }

    [HttpPost]
    [Route("DodajAgenciju")]
    public async Task<ActionResult> DodajAgenciju([FromBody]Agencija agencija)
    {
        if (agencija == null  ||  string.IsNullOrWhiteSpace(agencija.Email) || string.IsNullOrWhiteSpace(agencija.BrojTelefona))
        {
            return BadRequest("Nisu uneti svi obavezni podaci.");
        }

        if (Context.Agencije.Any(a => a.Naziv == agencija.Naziv))
        {
            return BadRequest("Agencija sa istim nazivom već postoji.");
        }

        try
        {
            await Context.Agencije.AddAsync(agencija);
            await Context.SaveChangesAsync();
            return Ok($"ID nove agencije je = {agencija.Id}");
        }
        catch
        {
            return BadRequest("Nije uspelo dodavanje agencije!");
        }
    }

    [HttpDelete("ObrisiAgenciju/{id}")]
    public async Task<ActionResult> ObrisiAgenciju(int id)
    {
        var stari = await Context.Agencije.FindAsync(id);

        if (stari != null)
        {
            var naziv = stari.Naziv;
            
            Context.Agencije.Remove(stari);
            await Context.SaveChangesAsync();
            return Ok($"Izbrisana je agencija: {naziv}");
        }
        else
        {
            return BadRequest("Nije uspelo brisanje agencije!");
        }
    }

    [HttpGet("PrezumiAgencije")]
    public async Task<ActionResult> PrezumiAgencije()
    {
        try
        {
            return Ok(await Context.Agencije.ToListAsync());
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("PrezumiAgenciju/{id}")]
    public async Task<ActionResult> PrezumiAgenciju(int id)
    {
        try
        {
            var agencija = await Context.Agencije.FindAsync(id);

            if (agencija == null)
            {
                return NotFound();
            }

            return Ok(agencija);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }


    [HttpPut("AzurirajAgenciju/{id}")]
    public async Task<ActionResult> AzurirajAgenciju(int id, [FromBody]Agencija agencija)
    {
        var stari = await Context.Agencije.FindAsync(id);

        if (stari != null)
        {
            stari.Naziv = agencija.Naziv;
            stari.Adresa = agencija.Adresa;
            stari.BrojTelefona = agencija.BrojTelefona;
            stari.Email = agencija.Email;
            stari.Grad = agencija.Grad;

            Context.Agencije.Update(stari);
            await Context.SaveChangesAsync();
            return Ok($"Azurirana je agencija sa ID = {stari.Id}");
        }
        else
        {
            return BadRequest("Nije uspelo azuriranje agencije!");
        }
    }
}
