using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using static System.Net.Mime.MediaTypeNames;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class RecenzijaTests : PageTest
{
    IPage page;
    IBrowser browser;

    [SetUp]
    public async Task Setup()
    {
        browser = await Playwright.Chromium.LaunchAsync(new()
        {
            Headless = false,
            SlowMo = 2000
        });

        page = await browser.NewPageAsync(new()
        {
            ViewportSize = new()
            {
                Width = 1280,
                Height = 720
            },
            ScreenSize = new()
            {
                Width = 1280,
                Height = 720
            }
        });
    }

    [Test]
    public async Task DodajRecenzijuTest()
    {
        var putovanjeId = "1";
        await page.GotoAsync($"http://localhost:3000/Putovanje/{putovanjeId}/Recenzije");

        await page.ClickAsync("#dodajRecenziju");

        await page.FillAsync("#korisnik", "Jana");
        await page.FillAsync("#komentar", "Super putovanje!");
        await page.FillAsync("#ocena", "5");

        await page.ClickAsync("#sacuvaj");

        await Task.Delay(TimeSpan.FromSeconds(2));

        var karticeRecenzija = await page.QuerySelectorAllAsync("#recenzije");

        Assert.IsTrue(karticeRecenzija.Count > 0);

        bool recenzijaNadjena = false;
        foreach (var kartica in karticeRecenzija)
        {
            var tekstKarticeRecenzija = await kartica.InnerTextAsync();
            if (tekstKarticeRecenzija.Contains("Jana")
                && tekstKarticeRecenzija.Contains("Super putovanje!")
                && tekstKarticeRecenzija.Contains("5"))
            {
                recenzijaNadjena = true;
                break;
            }
        }

        Assert.IsTrue(recenzijaNadjena);
    }

    [Test]
    public async Task IzmeniRecenzijuTest()
    {
        var putovanjeId = "1";
        await page.GotoAsync($"http://localhost:3000/Putovanje/{putovanjeId}/Recenzije");

        await page.ClickAsync("#izmeniRecenziju");

        await page.FillAsync("#korisnikIzmeni", "Milorad");
        await page.FillAsync("#komentarIzmeni", "Bilo je dosadno!");
        await page.FillAsync("#ocenaIzmeni", "1");

        await page.ClickAsync("#sacuvajIzmene");

        await Task.Delay(TimeSpan.FromSeconds(2));

        var karticeRecenzija = await page.QuerySelectorAllAsync("#recenzije");

        Assert.IsTrue(karticeRecenzija.Count > 0);

        bool recenzijaNadjena = false;
        foreach (var kartica in karticeRecenzija)
        {
            var tekstKarticeRecenzija = await kartica.InnerTextAsync();
            if (tekstKarticeRecenzija.Contains("Milorad")
                && tekstKarticeRecenzija.Contains("Bilo je dosadno!")
                && tekstKarticeRecenzija.Contains("1"))
            {
                recenzijaNadjena = true;
                break;
            }
        }

        Assert.IsTrue(recenzijaNadjena);
    }

    [Test]
    public async Task ObrisiRecenzijuTest()
    {
        var putovanjeId = "1";
        await page.GotoAsync($"http://localhost:3000/Putovanje/{putovanjeId}/Recenzije");

        var karticeRecenzije = await page.QuerySelectorAllAsync("#recenzije");

        Assert.IsTrue(karticeRecenzije.Count > 0);

        var prvaKartica = karticeRecenzije[0];
        var dugmeZaBrisanje = await prvaKartica.QuerySelectorAsync("#obrisiRecenziju");
        await dugmeZaBrisanje.ClickAsync();

        await Task.Delay(TimeSpan.FromSeconds(2));

        karticeRecenzije = await page.QuerySelectorAllAsync("#aktivnosti");

        Assert.IsFalse(karticeRecenzije.Contains(prvaKartica));
    }

    [Test]
    public async Task PreuzmiRecenzijeNaPutovanjuTest()
    {
        var putovanjeId = "1";
        await page.GotoAsync($"http://localhost:3000/Putovanje/{putovanjeId}/Recenzije");

        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var karticeRecenzije = await page.QuerySelectorAllAsync("#recenzije");

        Assert.IsTrue(karticeRecenzije.Count > 0);

        foreach (var kartica in karticeRecenzije)
        {
            var korisnik = await kartica.InnerTextAsync();
            var komentar = await kartica.InnerTextAsync();
            var ocena = await kartica.InnerTextAsync();

            Assert.IsFalse(string.IsNullOrEmpty(korisnik));
            Assert.IsFalse(string.IsNullOrEmpty(komentar));
            Assert.IsFalse(string.IsNullOrEmpty(ocena));
        }
    }

    [TearDown]
    public async Task Teardown()
    {
        await page.CloseAsync();
        await browser.DisposeAsync();
    }
}
