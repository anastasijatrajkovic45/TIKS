using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebTemplate.Controllers;
using Models;

namespace WebTemplate.NUnitTestovi
{
    [TestFixture]
    public class AgencijaTests
    {
        [SetUp]
        public void Setup()
        {
            Console.WriteLine("Setup");
        }

        [Test]
        public async Task DodajAgenciju_ValidAgencija_ReturnsOkResult()
        {
            var contextMock = new Mock<Context>();
            var controller = new AgencijaContoller(contextMock.Object);

            var agencija = new Agencija
            {
                Naziv = "SunTravel",
                Adresa = "Bulevar Nemanjica",
                Grad = "Nis",
                Email = "suntravel@gmail.com",
                BrojTelefona = "06158585"
            };

            var result = await controller.DodajAgenciju(agencija);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)result;
            Assert.That(okResult.Value, Is.EqualTo($"ID nove agencije je = {agencija.Id}"));
        }

        //[Test]
        //public async Task DodajAgenciju_FailsToSave_ReturnsBadRequest()
        //{
        //    // Aranžman (Arrange)
        //    var contextMock = new Mock<Context>();
        //    contextMock.Setup(c => c.Agencije.AddAsync(It.IsAny<Agencija>()))
        //               .Throws(new Exception("Simulirana greška prilikom čuvanja u bazi"));

        //    var controller = new AgencijaController(contextMock.Object);

        //    var agencija = new Agencija
        //    {
        //        // Postavite agenciju sa validnim podacima
        //        // ...

        //    };

        //    // Akt (Act)
        //    var result = await controller.DodajAgenciju(agencija);

        //    // Asertacija (Assert)
        //    Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        //}

        //[Test]
        //public async Task DodajAgenciju_InvalidAgencija_ReturnsBadRequest()
        //{
        //    // Aranžman (Arrange)
        //    var contextMock = new Mock<Context>();
        //    var controller = new AgencijaController(contextMock.Object);

        //    // Kreirajte agenciju sa nevalidnim podacima (npr. bez naziva)
        //    var agencija = new Agencija
        //    {
        //        // Postavite agenciju sa nevalidnim podacima (npr. bez naziva)
        //        // ...

        //    };

        //    // Akt (Act)
        //    var result = await controller.DodajAgenciju(agencija);

        //    // Asertacija (Assert)
        //    Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        //}

        [TearDown]
        public void TearDown()
        {
            Console.WriteLine("Tear down");
        }
    }
}
