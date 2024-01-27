using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Intrinsics.X86;

namespace PlaywrightTests.TestsAPI
{
    internal class PutovanjeTestsAPI : PlaywrightTest
    {
        private IAPIRequestContext Request;

        [SetUp]
        public async Task SetUpAPITesting()
        {
            var headers = new Dictionary<string, string>
            {
                { "Accept", "application/json" }
            };

            Request = await Playwright.APIRequest.NewContextAsync(new()
            {
                BaseURL = "https://localhost:7193",
                ExtraHTTPHeaders = headers,
                IgnoreHTTPSErrors = true
            });
        }
        [Test]
        public async Task PreuzmiPutovanja()
        {
            await using var response = await Request.GetAsync("/Putovanje/PreuzmiPutovanja");

            Assert.That(response.Status, Is.EqualTo(200));
            var jsonResponse = await response.JsonAsync();
            Assert.That(jsonResponse, Is.Not.Null);
        }
        [Test]
        public async Task ObrisiPutovanje_NepostojeciId_ReturnsBadRequest()
        {
            int nonExistingId = -1; //nevažeći ID-em

            await using var response = await Request.DeleteAsync($"/AktivnostContoller/ObrisiAktivnost/{nonExistingId}");

            Assert.That(response.Status, Is.EqualTo(404));
            var tekstOdgovora = await response.TextAsync();
            Assert.That(tekstOdgovora, Does.Contain(""));
        }

        [Test]
        public async Task PreuzmiPutovanjaAgencije_PostojeciId_ReturnsOk()
        {
            int agencijaId = 1; //id postojeceg putovanja

            await using var response = await Request.GetAsync($"/Putovanje/PreuzmiPutovanjaAgencije/{agencijaId}");

            Assert.That(response.Status, Is.EqualTo(200));
            var jsonResponse = await response.JsonAsync();
            Assert.That(jsonResponse, Is.Not.Null);
        }
        [Test]
        public async Task DodajPutovanje_ReturnsOk()
        {
            var novoPutovanje = new //nikako isti naziv, promeni naziv
            {
                slika = "slika",
                mesto = "Mesto",
                cena = 400,
                brojNocenja = 3,
                prevoz = "Prevoz"
            };

            await using var response = await Request.PostAsync("/Putovanje/DodajPutovanje", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                },
                DataObject = novoPutovanje
            });

            Assert.That(response.Status, Is.EqualTo(200));
            var textResponse = await response.TextAsync();
            Assert.That(textResponse, Does.Contain("ID novog putovanja je = "));
        }

        [Test]
        public async Task DodajPutovanjeAgenciji_NepostojecaAgencijaId_ReturnsNotFound()
        {
            var nevalidnaAgencijaId = -1; 

            var novoPutovanje = new
            {
                mesto = "NekePlaze",
                slika = "url_slike",
                prevoz = "Avion",
                brojNocenja = 7,
                cena = 1500
            };

            await using var response = await Request.PostAsync($"/Putovanje/DodajPutovanjeAgenciji/{nevalidnaAgencijaId}", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                },
                        DataObject = novoPutovanje
                    });

            Assert.That(response.Status, Is.EqualTo(404));
            var textResponse = await response.TextAsync();
            Assert.That(textResponse, Does.Contain("Agencija nije pronađena"));
        }

        [Test]
        public async Task AzurirajPutovanje_NeispravanId_VracaBadRequest()
        {
            int neispravanId = -1;
            var azuriranoPutovanje = new
            {
                mesto = "Rim",
                slika = "url_slike",
                prevoz = "Avion",
                brojNocenja = 7,
                cena = 1500
            };

            await using var response = await Request.PutAsync($"/Putovanje/AzurirajPutovanje/{neispravanId}", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
        {
            { "Content-Type", "application/json" }
        },
                DataObject = azuriranoPutovanje
            });

            Assert.That(response.Status, Is.EqualTo(400));
            var textResponse = await response.TextAsync();
            Assert.That(textResponse, Does.Contain("Nije uspelo azuriranje putovanja!"));
        }
    }


}

