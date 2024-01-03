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

    [HttpPost]
    [Route("DodajRecenziju")]
    public async Task<ActionResult> DodajRecenziju([FromBody]Recenzija recenzija)
    {
        try
        {
            await Context.Recenzije.AddAsync(recenzija);
            await Context.SaveChangesAsync();
            return Ok($"ID nove recenzije je = {recenzija.Id}");
        }
        catch
        {
                return BadRequest("Nije uspelo dodavanje recenzije!");
        }
    }

    [HttpDelete("ObrisiRecenziju/{id}")]
    public async Task<ActionResult> ObrisiRecenziju(int id)
    {
        var stari = await Context.Recenzije.FindAsync(id);

        if (stari != null)
        {
            Context.Recenzije.Remove(stari);
            await Context.SaveChangesAsync();
            return Ok($"Izbrisana je recenzija sa id: {stari.Id}");
        }
        else
        {
            return BadRequest("Nije uspelo brisanje recenzije!");
        }
    }

    [HttpGet("PrezumiRecenzije")]
    public async Task<ActionResult> PrezumiRecenzije()
    {
        try
        {
            return Ok(await Context.Recenzije.ToListAsync());
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("AzurirajRecenziju/{id}")]
    public async Task<ActionResult> AzurirajRecenziju(int id, [FromBody]Recenzija recenzija)
    {
        var stari = await Context.Recenzije.FindAsync(id);

        if (stari != null)
        {
            stari.Komentar = recenzija.Komentar;

            Context.Recenzije.Update(stari);
            await Context.SaveChangesAsync();
            return Ok($"Azurirana je recenzija sa ID = {stari.Id}");
        }
        else
        {
            return BadRequest("Nije uspelo azuriranje recenzije!");
        }
    }
}
