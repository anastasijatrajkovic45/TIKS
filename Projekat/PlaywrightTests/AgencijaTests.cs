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

    //[Test]
    //public async Task DodajAgenciju()
    //{
    //    await page.GotoAsync("http://localhost:3000/Agencije");

    //    await page.ClickAsync("#dodajAgenciju-1");

    //    await page.FillAsync("#nazivAgencije-2", "Nova Agencija");
    //    await page.FillAsync("#adresaAgencije-2", "Adresa 123");
    //    await page.FillAsync("#gradAgencije-2", "Grad");
    //    await page.FillAsync("#emailAgencije-2", "nova.agencija@example.com");
    //    await page.FillAsync("#brojAgencije-2", "123456789");

    //    await page.ClickAsync("#dodajAgenciju-2");
    //    await page.WaitForSelectorAsync("#dodajAgenciju-2");

    //    var novaAgencija = await page.WaitForSelectorAsync("div:has-text('Nova Agencija')");
    //    Assert.NotNull(novaAgencija);
    //}

    //[Test]
    //public async Task IzmeniAgenciju()
    //{
    //    await page.GotoAsync("http://localhost:3000/Agencije");

    //    await page.ClickAsync("#izmeniAgenciju-1");

    //    await page.WaitForSelectorAsync("#nazivAgencije-1");

    //    await page.FillAsync("#nazivAgencije-1", "Novi Naziv");
    //    await page.FillAsync("#adresaAgencije-1", "Nova Adresa");
    //    await page.FillAsync("#gradAgencije-1", "Novi Grad");
    //    await page.FillAsync("#emailAgencije-1", "novi.email@example.com");
    //    await page.FillAsync("#brojAgencije-1", "987654321");

    //    await page.ClickAsync("#izmeniAgenciju-1");

    //    await page.WaitForSelectorAsync("#izmeniAgenciju-1");

    //    var izmenjenaAgencija = await page.WaitForSelectorAsync("#karticaAgencije");
    //    Assert.NotNull(izmenjenaAgencija);
    //}


    [TearDown]
    public async Task Teardown()
    {
        await page.CloseAsync();
        await browser.DisposeAsync();
    }
}