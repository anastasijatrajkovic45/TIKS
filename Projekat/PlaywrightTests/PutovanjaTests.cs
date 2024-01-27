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

    //[Test]
    //public async Task DodajPutovanjeTest()
    //{
    //    var agencyId = "2";
    //    await page.GotoAsync($"http://localhost:3000/Agencije/{agencyId}");

    //    var mesto = "TestMesto";
    //    var slika = "https://example.com/test.jpg";
    //    var brojNocenja = 7;
    //    var cena = 500;
    //    var prevoz = "TestPrevoz";

    //    await page.ClickAsync("#dodajPutovanje-2");

    //    await page.TypeAsync("#mestoPutovanja-1", mesto);
    //    await page.TypeAsync("#slikaPutovanja-1", slika);
    //    await page.TypeAsync("#brojNocenjaPutovanja-1", brojNocenja.ToString());
    //    await page.TypeAsync("#cenaPutovanja-1", cena.ToString());
    //    await page.TypeAsync("#prevozPutovanja-1", prevoz);

    //    await page.ClickAsync("#dodajPutovanje-1");

    //    //var successMessage = await page.WaitForSelectorAsync("#successMessage");
    //    //Assert.NotNull(successMessage);
    //    await page.ScreenshotAsync(new() { Path = "../../../Slike/dodajPutovanje.png" });
    //}

    [Test]
    public async Task PrikazPutovanja()
    {
        var agencyId = "1";
        await page.GotoAsync($"http://localhost:3000/Agencije/{agencyId}");

        var putovanja = await page.QuerySelectorAllAsync("#karticaPutovanja");

        Assert.True(putovanja.Count > 0);
    }
}
