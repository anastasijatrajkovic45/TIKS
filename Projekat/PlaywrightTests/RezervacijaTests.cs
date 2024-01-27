using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class RezervacijaTests : PageTest
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
    public async Task DodajRezervacijuTest()
    {
        var agencijaId = "1";
        await page.GotoAsync($"http://localhost:3000/Agencije/{agencijaId}");

        await page.ClickAsync("#rezervisi");

        await page.FillAsync("#ime", "Nikola");
        await page.FillAsync("#prezime", "Nikolic");
        await page.FillAsync("#adresa", "Strahinjica Bana");
        await page.FillAsync("#grad", "Nis");
        await page.FillAsync("#brojTelefona", "0655555");
        await page.FillAsync("#email", "nikola@gmail.com");
        await page.FillAsync("#brojOsoba", "4");

        await page.ClickAsync("#potvrdiRezervaciju");

        var putovanjeId = "1";
        await page.GotoAsync($"http://localhost:3000/Putovanje/{putovanjeId}");

        await Task.Delay(TimeSpan.FromSeconds(2));

        var karticeRezervacije = await page.QuerySelectorAllAsync("#rezervacije");

        Assert.IsTrue(karticeRezervacije.Count > 0);

        bool rezervacijaNadjena = false;

        foreach (var kartica in karticeRezervacije)
        {
            var tekstKarticeRezervacije = await kartica.InnerTextAsync();

            if (tekstKarticeRezervacije.Contains("Nikola")
                && tekstKarticeRezervacije.Contains("Nikolic")
                && tekstKarticeRezervacije.Contains("Strahinjica Bana")
                && tekstKarticeRezervacije.Contains("Nis")
                && tekstKarticeRezervacije.Contains("0655555")
                && tekstKarticeRezervacije.Contains("nikola@gmail.com")
                && tekstKarticeRezervacije.Contains("4"))
            {
                rezervacijaNadjena = true;
                break;
            }
        }
        Assert.IsTrue(rezervacijaNadjena);
    }

    [Test]
    public async Task IzmeniRezervacijuTest()
    {
        var putovanjeId = "1";
        await page.GotoAsync($"http://localhost:3000/Putovanje/{putovanjeId}");

        await page.ClickAsync("#izmeniRezervaciju");

        await page.FillAsync("#ime", "Mina");
        await page.FillAsync("#prezime", "Minic");
        await page.FillAsync("#adresa", "Vozdova");
        await page.FillAsync("#grad", "Nis");
        await page.FillAsync("#brojTelefona", "0655555");
        await page.FillAsync("#email", "mina@gmail.com");
        await page.FillAsync("#brojOsoba", "3");

        await page.ClickAsync("#sacuvajIzmeneRezervacije");

        await Task.Delay(TimeSpan.FromSeconds(2));

        var karticeRezervacije = await page.QuerySelectorAllAsync("#rezervacije");

        Assert.IsTrue(karticeRezervacije.Count > 0);

        bool rezervacijaNadjena = false;

        foreach (var kartica in karticeRezervacije)
        {
            var tekstKarticeRezervacije = await kartica.InnerTextAsync();

            if (tekstKarticeRezervacije.Contains("Mina")
                && tekstKarticeRezervacije.Contains("Minic")
                 && tekstKarticeRezervacije.Contains("Vozdova")
                  && tekstKarticeRezervacije.Contains("Nis")
                   && tekstKarticeRezervacije.Contains("0655555")
                    && tekstKarticeRezervacije.Contains("mina@gmail.com")
                     && tekstKarticeRezervacije.Contains("3"))
            {
                rezervacijaNadjena = true;
                break;
            }
        }
        Assert.IsTrue(rezervacijaNadjena);
    }

    [Test]
    public async Task ObrisiRezervacijuTest()
    {
        var putovanjeId = "1";
        await page.GotoAsync($"http://localhost:3000/Putovanje/{putovanjeId}");

        var karticeRezervacije = await page.QuerySelectorAllAsync("#rezervacije");

        Assert.IsTrue(karticeRezervacije.Count > 0);

        var prvaKartica = karticeRezervacije[0];
        var dugmeZaBrisanje = await prvaKartica.QuerySelectorAsync("#obrisiRezervaciju");
        await dugmeZaBrisanje.ClickAsync();

        await Task.Delay(TimeSpan.FromSeconds(2));

        karticeRezervacije = await page.QuerySelectorAllAsync("#rezervacije");

        Assert.IsFalse(karticeRezervacije.Contains(prvaKartica));
    }

    [Test]
    public async Task PreuzmiRezervacijaZaPutovanjeTest()
    {
        var putovanjeId = "1";
        await page.GotoAsync($"http://localhost:3000/Putovanje/{putovanjeId}");

        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var karticeRezervacije = await page.QuerySelectorAllAsync("#rezervacije");

        Assert.IsTrue(karticeRezervacije.Count > 0);

        foreach (var kartica in karticeRezervacije)
        {
            var ime = await kartica.InnerTextAsync();
            var prezime = await kartica.InnerTextAsync();
            var adresa = await kartica.InnerTextAsync();
            var grad = await kartica.InnerTextAsync();
            var brojTelefona = await kartica.InnerTextAsync();
            var email = await kartica.InnerTextAsync();
            var brojOsoba = await kartica.InnerTextAsync();

            Assert.IsFalse(string.IsNullOrEmpty(ime));
            Assert.IsFalse(string.IsNullOrEmpty(prezime));
            Assert.IsFalse(string.IsNullOrEmpty(adresa));
            Assert.IsFalse(string.IsNullOrEmpty(grad));
            Assert.IsFalse(string.IsNullOrEmpty(brojTelefona));
            Assert.IsFalse(string.IsNullOrEmpty(email));
            Assert.IsFalse(string.IsNullOrEmpty(brojOsoba));
        }
    }

    [TearDown]
    public async Task Teardown()
    {
        await page.CloseAsync();
        await browser.DisposeAsync();
    }
}