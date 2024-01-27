using Microsoft.Playwright;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollector.InProcDataCollector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaywrightTests.TestsAPI
{
    internal class AktivnostiTestsAPI : PlaywrightTest
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
        public async Task ObrisiAktivnost_NonExistingId_ReturnsBadRequest()
        {
            int nonExistingId = -1; //nevažeći ID-em

            await using var response = await Request.DeleteAsync($"/AktivnostContoller/ObrisiAktivnost/{nonExistingId}");

            Assert.That(response.Status, Is.EqualTo(404));
            var tekstOdgovora = await response.TextAsync();
            Assert.That(tekstOdgovora, Does.Contain(""));
        }
    }
}
