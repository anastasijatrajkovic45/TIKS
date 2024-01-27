using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Intrinsics.X86;

namespace PlaywrightTests.TestsAPI
{
    internal class RecenzijeTestsAPI : PlaywrightTest
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
        public async Task ObrisiRecenziju_NepostojeciId_ReturnsBadRequest()
        {
            int nonExistingId = -1; //nevažeći ID-em

            await using var response = await Request.DeleteAsync($"/Recenzija/ObrisiRecenziju/{nonExistingId}");

            Assert.That(response.Status, Is.EqualTo(400));
            var tekstOdgovora = await response.TextAsync();
            Assert.That(tekstOdgovora, Does.Contain(""));
        }

        [Test]
        public async Task PreuzmiRecenzijePutovanja_PostojeciId_ReturnsOk()
        {
            int putovanjeId = 1; //id postojeceg putovanja

            await using var response = await Request.GetAsync($"/Recenzija/PreuzmiRecenzijeNaPutovanju/{putovanjeId}");

            Assert.That(response.Status, Is.EqualTo(200));
            var jsonResponse = await response.JsonAsync();
            Assert.That(jsonResponse, Is.Not.Null);
        }
        [Test]
        public async Task DodajaRecenzijuPutovanja_NeispravanId_VracaNotFound()
        {
            var nevalidniId = -1;

            var novaRecenzija = new
            {
                korisnik = "Korisnik",
                ocena = 4,
                komentar = "Komentar"
            };

            await using var response = await Request.PostAsync($"/Recenzija/DodajRecenzijuUPutovanje/{nevalidniId}", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                },
                DataObject = novaRecenzija
            });

            Assert.That(response.Status, Is.EqualTo(404));
            var textResponse = await response.TextAsync();
            Assert.That(textResponse, Does.Contain("Putovanje nije pronađeno."));
        }
        [Test]
        public async Task AzurirajRecenzijuPutovanja_ValidanId_VracaOk()
        {
            int validanId = 3; // postojeci id putovanja
            var azuriranaRecenzija = new
            {
                korisnik = "Korisnik1",
                ocena = 3,
                komentar = "Komentar1"
            };

            await using var response = await Request.PutAsync($"/Recenzija/AzurirajRecenziju/{validanId}", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
        {
            { "Content-Type", "application/json" }
        },
                DataObject = azuriranaRecenzija
            });

            Assert.That(response.Status, Is.EqualTo(200));
            var textResponse = await response.TextAsync();
            Assert.That(textResponse, Does.Contain($"Azurirani su podaci o recenziji sa ID = {validanId}"));
        }
    }
}
