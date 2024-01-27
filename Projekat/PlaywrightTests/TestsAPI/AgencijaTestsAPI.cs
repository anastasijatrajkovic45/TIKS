using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Intrinsics.X86;

namespace PlaywrightTests
{
    [TestFixture]
    internal class AgencijaTestsAPI : PlaywrightTest
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
        public async Task PreuzmiAgencije()
        {
            await using var response = await Request.GetAsync("/AgencijaContoller/PrezumiAgencije");

            Assert.That(response.Status, Is.EqualTo(200));
            var jsonResponse = await response.JsonAsync();
            Assert.That(jsonResponse, Is.Not.Null);
        }

        [Test]
        public async Task PrezumiAgenciju_ValidId_ReturnsOk()
        {
            int agencijaId = 1; 
            await using var response = await Request.GetAsync($"/AgencijaContoller/PrezumiAgenciju/{agencijaId}");

            Assert.That(response.Status, Is.EqualTo(200));
            var jsonResponse = await response.JsonAsync();
            Assert.That(jsonResponse, Is.Not.Null);
        }
        [Test]
        public async Task ObrisiAgenciju_ReturnsOk()
        {
            int agencijaId = 2;

            await using var response = await Request.DeleteAsync($"/AgencijaContoller/ObrisiAgenciju/{agencijaId}");

            Assert.That(response.Status, Is.EqualTo(400));
            var textResponse = await response.TextAsync();
            Assert.That(textResponse, Does.Contain("Nije uspelo brisanje agencije!"));
        }

        [Test]
        public async Task DodajAgenciju_ReturnsOk()
        {
            var novaAgencija = new //nikako isti naziv, promeni naziv
            {
                naziv = "Puzzzlee",
                adresa = "NekaAdresa",
                grad = "Nis",
                email = "nova@agencija.com",
                brojTelefona = "123456789"
            };

            await using var response = await Request.PostAsync("/AgencijaContoller/DodajAgenciju", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                },
                DataObject = novaAgencija
            });

            Assert.That(response.Status, Is.EqualTo(200));
            var textResponse = await response.TextAsync();
            Assert.That(textResponse, Does.Contain("ID nove agencije je = "));
        }
        [Test]
        public async Task AzurirajAgenciju_ValidanId_VracaOk()
        {
            int validanId = 22; // postojeci id agencije
            var azuriranaAgencija = new
            {
                naziv = "Rapsody",
                adresa = "Vojvode Gojka 12",
                grad = "Nis",
                email = "rapsody@gmail.com",
                brojTelefona = "123456789"
            };

            await using var response = await Request.PutAsync($"/AgencijaContoller/AzurirajAgenciju/{validanId}", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
        {
            { "Content-Type", "application/json" }
        },
                DataObject = azuriranaAgencija
            });

            Assert.That(response.Status, Is.EqualTo(200));
            var textResponse = await response.TextAsync();
            Assert.That(textResponse, Does.Contain($"Azurirana je agencija sa ID = {validanId}"));
        }
    }


}




