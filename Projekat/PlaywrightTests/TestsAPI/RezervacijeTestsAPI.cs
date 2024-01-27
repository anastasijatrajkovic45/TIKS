using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Intrinsics.X86;

namespace PlaywrightTests.TestsAPI
{
    internal class RezervacijeTestsAPI : PlaywrightTest
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
        public async Task ObrisiRezervaciju_NeposotjeciId_VracaBadRequest()
        {
            int nonExistingId = -1; //nevažeći ID-em

            await using var response = await Request.DeleteAsync($"/Rezervacija/ObrisiRezervaicju/{nonExistingId}");

            Assert.That(response.Status, Is.EqualTo(400));
            var tekstOdgovora = await response.TextAsync();
            Assert.That(tekstOdgovora, Does.Contain(""));
        }

        [Test]
        public async Task PreuzmiRegistracijePutovanja_PostojeciId_VracaOk()
        {
            int putovanjeId = 1; //id postojeceg putovanja

            await using var response = await Request.GetAsync($"/Rezervacija/PreuzmiRezervacijePutovanja/{putovanjeId}");

            Assert.That(response.Status, Is.EqualTo(200));
            var jsonResponse = await response.JsonAsync();
            Assert.That(jsonResponse, Is.Not.Null);
        }
        [Test]
        public async Task DodajaRezervacijuPutovanja_NeispravanId_VracaNotFound()
        {
            var nevalidniId = -1;

            var novaRezervacija = new
            {
                ime = "Ime",
                prezime = "Prezime",
                brojTelefona = "032645",
                adresa = "Adresa",
                grad = "Grad",
                email = "email",
                brojOsoba = 5
            };

            await using var response = await Request.PostAsync($"/Rezervacija/DodajRezervacijuPutovanja/{nevalidniId}", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                },
                DataObject = novaRezervacija
            });

            Assert.That(response.Status, Is.EqualTo(404));
            var textResponse = await response.TextAsync();
            Assert.That(textResponse, Does.Contain("Putovanje nije pronađena"));
        }
        [Test]
        public async Task AzurirajRezervacijuPutovanja_NeispravanId_VracaBadRequest()
        {
            int neispravanId = -1;
            var azuriranaRezervacija = new
            {
                ime = "NovoIme",
                prezime = "NovoPrezime",
                brojTelefona = "032645",
                adresa = "NovaAdresa",
                grad = "NoviGrad",
                email = "novi@email.com",
                brojOsoba = 3
            };

            await using var response = await Request.PutAsync($"/Rezervacija/AzurirajRezervaciju/{neispravanId}", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
        {
            { "Content-Type", "application/json" }
        },
                DataObject = azuriranaRezervacija
            });

            Assert.That(response.Status, Is.EqualTo(400));
            var textResponse = await response.TextAsync();
            Assert.That(textResponse, Does.Contain("Nije uspelo azuriranje rezervacije!"));
        }

    }
}
