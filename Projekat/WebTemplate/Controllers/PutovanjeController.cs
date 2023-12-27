namespace WebTemplate.Controllers;

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
                return BadRequest("Nije uspelo dodavanje putovanja");
        }
    }

    [HttpDelete("ObrisiPutovanje/{id}")]
    public async Task<ActionResult> ObrisiPutovanje(int id)
    {
        var stari = await Context.Putovanja.FindAsync(id);

        if (stari != null)
        {
            var naziv=stari.Destinacija;
            Context.Putovanja.Remove(stari);
            await Context.SaveChangesAsync();
            return Ok($"Izbrisano je putovanje na destinaciji: {naziv}");
        }
        else
        {
            return BadRequest("Neuspelo!");
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
            stari.DatumPolaska = putovanje.DatumPolaska;
            stari.DatumPovratka=putovanje.DatumPovratka;

            Context.Putovanja.Update(stari);
            await Context.SaveChangesAsync();
            return Ok($"Azurirano je putovanje sa ID = {stari.Id}");
        }
        else
        {
            return BadRequest("Neuspelo!");
        }
    }

   
}
