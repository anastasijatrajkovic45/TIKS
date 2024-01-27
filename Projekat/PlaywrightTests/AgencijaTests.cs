using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class AgencijaTests : PageTest
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
    public async Task DodajAgencijuTest()
    {
        await page.GotoAsync($"http://localhost:3000/Agencije");

        await page.ClickAsync("#dodajAgenciju");

        await page.FillAsync("#naziv", "GoToTravell");
        await page.FillAsync("#adresa", "Igmanska");
        await page.FillAsync("#grad", "Novi Sad");
        await page.FillAsync("#brojTelefona", "0645454");
        await page.FillAsync("#email", "goto@gmail.com");

        await page.ClickAsync("#sacuvaj");

        await Task.Delay(TimeSpan.FromSeconds(2));

        var karticeAgencija = await page.QuerySelectorAllAsync("#agencije");

        Assert.IsTrue(karticeAgencija.Count > 0);

        bool agencijaNadjena = false;
        foreach (var kartica in karticeAgencija)
        {
            var tekstKarticeAgencija = await kartica.InnerTextAsync();
            if (tekstKarticeAgencija.Contains("GoToTravell")
                && tekstKarticeAgencija.Contains("Igmanska")
                    && tekstKarticeAgencija.Contains("Novi Sad")
                        && tekstKarticeAgencija.Contains("0645454")
                            && tekstKarticeAgencija.Contains("goto@gmail.com"))
            {
                agencijaNadjena = true;
                break;
            }
        }

        Assert.IsTrue(agencijaNadjena);
    }

    [Test]
    public async Task IzmeniAgencijuTest()
    {
        await page.GotoAsync($"http://localhost:3000/Agencije");

        await page.ClickAsync("#izmeni");

        await page.FillAsync("#nazivIzmeni", "GoToTravell");
        await page.FillAsync("#adresaIzmeni", "Djerdapska 50");
        await page.FillAsync("#gradIzmeni", "Nis");
        await page.FillAsync("#brojTelefonaIzmeni", "0645454");
        await page.FillAsync("#emailIzmeni", "goto@gmail.com");

        await page.ClickAsync("#sacuvajIzmene");

        await Task.Delay(TimeSpan.FromSeconds(2));

        var karticeAgencija = await page.QuerySelectorAllAsync("#agencije");

        Assert.IsTrue(karticeAgencija.Count > 0);

        bool agencijaNadjena = false;
        foreach (var kartica in karticeAgencija)
        {
            var tekstKarticeAgencija = await kartica.InnerTextAsync();
            if (tekstKarticeAgencija.Contains("GoToTravell")
                && tekstKarticeAgencija.Contains("Djerdapska 50")
                    && tekstKarticeAgencija.Contains("Nis")
                        && tekstKarticeAgencija.Contains("0645454")
                            && tekstKarticeAgencija.Contains("goto@gmail.com"))
            {
                agencijaNadjena = true;
                break;
            }
        }

        Assert.IsTrue(agencijaNadjena);
    }

    [Test]
    public async Task ObrisiAgencijuTest()
    {
        await page.GotoAsync($"http://localhost:3000/Agencije");

        var karticeAgencije = await page.QuerySelectorAllAsync("#agencije");

        Assert.IsTrue(karticeAgencije.Count > 0);

        var prvaKartica = karticeAgencije[0];
        var dugmeZaBrisanje = await prvaKartica.QuerySelectorAsync("#obrisi");
        await dugmeZaBrisanje.ClickAsync();

        await Task.Delay(TimeSpan.FromSeconds(2));

        karticeAgencije = await page.QuerySelectorAllAsync("#agencije");

        Assert.IsFalse(karticeAgencije.Contains(prvaKartica));
    }

    [Test]
    public async Task PreuzmiAktivnostiNaPutovanjuTest()
    {
        await page.GotoAsync($"http://localhost:3000/Agencije");

        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var karticeAgencije = await page.QuerySelectorAllAsync("#agencije");

        Assert.IsTrue(karticeAgencije.Count > 0);

        foreach (var kartica in karticeAgencije)
        {
            var naziv = await kartica.InnerTextAsync();
            var adresa = await kartica.InnerTextAsync();
            var grad = await kartica.InnerTextAsync();
            var brojTelefona = await kartica.InnerTextAsync();
            var email = await kartica.InnerTextAsync();

            Assert.IsFalse(string.IsNullOrEmpty(naziv));
            Assert.IsFalse(string.IsNullOrEmpty(adresa));
            Assert.IsFalse(string.IsNullOrEmpty(grad));
            Assert.IsFalse(string.IsNullOrEmpty(brojTelefona));
            Assert.IsFalse(string.IsNullOrEmpty(email));
        }
    }

    [TearDown]
    public async Task Teardown()
    {
        await page.CloseAsync();
        await browser.DisposeAsync();
    }
}