using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using static System.Net.Mime.MediaTypeNames;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class PutovanjaTests : PageTest
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
    public async Task DodajPutovanjeTest()
    {
        var agencijaId = "1";
        await page.GotoAsync($"http://localhost:3000/Agencije/{agencijaId}");

        await page.ClickAsync("#dodajPutovanje");

        await page.FillAsync("#mesto", "Barselona");
        await page.FillAsync("#slika", "https://ticketshop.barcelona/images/sights-barcelona.jpg");
        await page.FillAsync("#brojNocenja", "10");
        await page.FillAsync("#cena", "500");
        await page.FillAsync("#prevoz", "avio");

        await page.ClickAsync("#dodaj");

        await Task.Delay(TimeSpan.FromSeconds(5));

        var karticePutovanja = await page.QuerySelectorAllAsync("#listaPutovanja");

        Assert.IsTrue(karticePutovanja.Count > 0);

        bool putovanjeNadjeno = false;
        foreach (var kartica in karticePutovanja)
        {
            var tekstKarticePutovanja = await kartica.InnerTextAsync();
            if (tekstKarticePutovanja.Contains("Barselona")
                && tekstKarticePutovanja.Contains("10")
                && tekstKarticePutovanja.Contains("500")
                && tekstKarticePutovanja.Contains("avio"))
            {
                putovanjeNadjeno = true;
                break;
            }
        }
        Assert.IsTrue(putovanjeNadjeno);
    }

    [Test]
    public async Task IzmeniPutovanjeTest()
    {
        var agencijaId = "1";
        await page.GotoAsync($"http://localhost:3000/Agencije/{agencijaId}");

        await page.ClickAsync("#izmeni");

        await page.FillAsync("#mestoIzmeni", "Egipat");
        await page.FillAsync("#slikaIzmeni", "https://www.mediteraneo.rs/pic/bliski_istok_egipatmediteraneo_001_800x457m.jpg");
        await page.FillAsync("#prevozIzmeni", "avio");
        await page.FillAsync("#brojNocenjaIzmeni", "9");
        await page.FillAsync("#cenaIzmeni", "950");

        await page.ClickAsync("#sacuvajIzmene");

        await Task.Delay(TimeSpan.FromSeconds(2));

        var karticePutovanja = await page.QuerySelectorAllAsync("#listaPutovanja");

        Assert.IsTrue(karticePutovanja.Count > 0);

        bool putovanjeNadjeno = false;
        foreach (var kartica in karticePutovanja)
        {
            var tekstKarticePutovanja = await kartica.InnerTextAsync();
            if (tekstKarticePutovanja.Contains("Egipat")
                && tekstKarticePutovanja.Contains("9")
                && tekstKarticePutovanja.Contains("950")
                && tekstKarticePutovanja.Contains("avio"))
            {
                putovanjeNadjeno = true;
                break;
            }
        }
        Assert.IsTrue(putovanjeNadjeno);
    }

    [Test]
    public async Task ObrisiPutovanjeTest()
    {
        var agencijaId = "22";
        await page.GotoAsync($"http://localhost:3000/Agencije/{agencijaId}");

        var karticePutovanja = await page.QuerySelectorAllAsync("#listaPutovanja");

        Assert.IsTrue(karticePutovanja.Count > 0);

        var prvaKartica = karticePutovanja[0];
        var dugmeZaBrisanje = await prvaKartica.QuerySelectorAsync("#obrisi");
        await dugmeZaBrisanje.ClickAsync();

        await Task.Delay(TimeSpan.FromSeconds(2));

        karticePutovanja = await page.QuerySelectorAllAsync("#listaPutovanja");

        Assert.IsFalse(karticePutovanja.Contains(prvaKartica));
    }

    [Test]
    public async Task PreuzmiPutovanjaAgencijeTest()
    {
        var agencijaId = "1";
        await page.GotoAsync($"http://localhost:3000/Agencije/{agencijaId}");

        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var karticePutovanja = await page.QuerySelectorAllAsync("#listaPutovanja");

        Assert.IsTrue(karticePutovanja.Count > 0);

        foreach (var kartica in karticePutovanja)
        {
            var mesto = await kartica.InnerTextAsync();
            var slika = await kartica.InnerTextAsync();
            var brojNocenja = await kartica.InnerTextAsync();
            var prevoz = await kartica.InnerTextAsync();
            var cena = await kartica.InnerTextAsync();

            Assert.IsFalse(string.IsNullOrEmpty(mesto));
            Assert.IsFalse(string.IsNullOrEmpty(slika));
            Assert.IsFalse(string.IsNullOrEmpty(brojNocenja));
            Assert.IsFalse(string.IsNullOrEmpty(prevoz));
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
