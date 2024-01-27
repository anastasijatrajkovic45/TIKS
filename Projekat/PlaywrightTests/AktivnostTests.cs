using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using static System.Net.Mime.MediaTypeNames;
namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class AktivnostTests : PageTest
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
    public async Task DodajAktivnostTest()
    {
        var putovanjeId = "1";
        await page.GotoAsync($"http://localhost:3000/Putovanje/{putovanjeId}");

        await page.ClickAsync("#dodajAktivnost");

        await page.FillAsync("#nazivAktivnosti", "Zurka");
        await page.FillAsync("#cenaAktivnosti", "1000");

        await page.ClickAsync("#sacuvajAktivnost");

        await Task.Delay(TimeSpan.FromSeconds(2));

        var karticeAktivnosti = await page.QuerySelectorAllAsync("#aktivnosti");

        Assert.IsTrue(karticeAktivnosti.Count > 0);

        bool aktivnostNadjena = false;
        foreach (var kartica in karticeAktivnosti)
        {
            var tekstKarticeAktivnosti = await kartica.InnerTextAsync();
            if (tekstKarticeAktivnosti.Contains("Zurka") && tekstKarticeAktivnosti.Contains("1000"))
            {
                aktivnostNadjena = true;
                break;
            }
        }

        Assert.IsTrue(aktivnostNadjena);
    }

    [Test]
    public async Task IzmeniAktivnostTest()
    {
        var putovanjeId = "1";
        await page.GotoAsync($"http://localhost:3000/Putovanje/{putovanjeId}");

        await page.ClickAsync("#izmeniAktivnost");

        await page.FillAsync("#izmeniNazivAktivnosti", "Nocni obilazak grada");
        await page.FillAsync("#izmeniCenuAktivnosti", "150");

        await page.ClickAsync("#sacuvajIzmeneAktivnosti");

        await Task.Delay(TimeSpan.FromSeconds(2));

        var karticeAktivnosti = await page.QuerySelectorAllAsync("#aktivnosti");

        Assert.IsTrue(karticeAktivnosti.Count > 0);

        bool aktivnostNadjena = false;

        foreach (var kartica in karticeAktivnosti)
        {
            var tekstKarticeAktivnosti = await kartica.InnerTextAsync();

            if (tekstKarticeAktivnosti.Contains("Nocni obilazak grada") && tekstKarticeAktivnosti.Contains("150"))
            {
                aktivnostNadjena = true;
                break;
            }
        }
        Assert.IsTrue(aktivnostNadjena);
    }

    [Test]
    public async Task ObrisiAktivnostTest()
    {
        var putovanjeId = "1";
        await page.GotoAsync($"http://localhost:3000/Putovanje/{putovanjeId}");

        var karticeAktivnosti = await page.QuerySelectorAllAsync("#aktivnosti");

        Assert.IsTrue(karticeAktivnosti.Count > 0);

        var prvaKartica = karticeAktivnosti[0];
        var dugmeZaBrisanje = await prvaKartica.QuerySelectorAsync("#obrisiAktivnost");
        await dugmeZaBrisanje.ClickAsync();

        await Task.Delay(TimeSpan.FromSeconds(2));

        karticeAktivnosti = await page.QuerySelectorAllAsync("#aktivnosti");

        Assert.IsFalse(karticeAktivnosti.Contains(prvaKartica));
    }

    [Test]
    public async Task PreuzmiAktivnostiNaPutovanjuTest()
    {
        var putovanjeId = "1";
        await page.GotoAsync($"http://localhost:3000/Putovanje/{putovanjeId}");

        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var karticeAktivnosti = await page.QuerySelectorAllAsync("#aktivnosti");

        Assert.IsTrue(karticeAktivnosti.Count > 0);

        foreach (var kartica in karticeAktivnosti)
        {
            var naziv = await kartica.InnerTextAsync();
            var cena = await kartica.InnerTextAsync();

            Assert.IsFalse(string.IsNullOrEmpty(naziv));
            Assert.IsFalse(string.IsNullOrEmpty(cena));
        }
    }

    [TearDown]
    public async Task Teardown()
    {
        await page.CloseAsync();
        await browser.DisposeAsync();
    }
}