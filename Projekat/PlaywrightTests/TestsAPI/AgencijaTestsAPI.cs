using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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







    }



}

