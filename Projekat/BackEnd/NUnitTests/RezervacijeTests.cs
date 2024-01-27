using Microsoft.Extensions.Configuration;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTemplate.Controllers;

namespace NUnitTests
{
    public class RezervacijeTests
    {
        private WebTemplate.Controllers.RezervacijaController _rezervacijaController;
        private Context _context;

        [SetUp]
        public void Setup()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("IspitCS");

            var options = new DbContextOptionsBuilder<Context>()
                .UseSqlServer(connectionString)
            .Options;
            _context = new Context(options);

            _rezervacijaController = new WebTemplate.Controllers.RezervacijaController(_context);
        }
        //dodavanje rezervacije za putovanje
        [Test]
        public async Task DodajRezervacijuPutovanja_PutovanjeNePostoji()
        {

            var dbContextOptions = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new Context(dbContextOptions))
            {
                // Kontekst je prazan, ne postoji putovanja sa Id
            }

            using (var context = new Context(dbContextOptions))
            {
                var controller = new RezervacijaController(context);

                var rezervacija = new Rezervacija { Ime = "Ime", Prezime = "Prezime", BrojTelefona="12345680", Adresa="Adresa", Grad="Grad", Email="emali", BrojOsoba=3 };

                var result = await controller.DodajRezervacijuPutovanja(1, rezervacija);

                Assert.IsInstanceOf<NotFoundObjectResult>(result);
                var notFoundResult = (NotFoundObjectResult)result;
                Assert.AreEqual("Putovanje nije pronađena", notFoundResult.Value);
            }
        }

        [Test]
        public async Task DodajRezervacijuPutovanja_NeispravniPodaci_VracaBadRequest()
        {
            Putovanje putovanje = new Putovanje
            {
                BrojNocenja = 3,
                Cena = 100,
                Mesto = "Mesto",
                Slika = "Slika",
                Prevoz = "Prevoz"
            };

            Rezervacija rezervacija = new Rezervacija
            {
                Ime = "Ime",
                Prezime = "Prezime",
                BrojTelefona="032645",
                Adresa="Adresa",
                Grad="Grad",
                Email="email",
                BrojOsoba=0
            };
            int putovanjeId = putovanje.Id;
            await _context.Putovanja.AddAsync(putovanje);
            await _context.SaveChangesAsync();

            var result = await _rezervacijaController.DodajRezervacijuPutovanja(putovanjeId, rezervacija);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            Assert.AreEqual("Nisu uneti svi obavezni podaci.", (result as BadRequestObjectResult)?.Value);
        }

        [Test]
        public async Task DodajRezervacijuPutovanju_NepopunjenaPolja_VracaBadRequest()
        {
            Putovanje putovanje = new Putovanje
            {
                BrojNocenja = 2,
                Cena = 200,
                Mesto = "Mesto",
                Slika = "Slika",
                Prevoz = "Prevoz"
            };

            Rezervacija rezervacija = new Rezervacija
            {
                Ime = "",
                Prezime = "",
                BrojTelefona = "032645",
                Adresa = "Adresa",
                Grad = "Grad",
                Email = "email",
                BrojOsoba = 3
            };
            int putovanjeId = putovanje.Id;
            await _context.Putovanja.AddAsync(putovanje);
            await _context.SaveChangesAsync();

            var result = await _rezervacijaController.DodajRezervacijuPutovanja(putovanjeId, rezervacija);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            Assert.AreEqual("Nisu uneti svi obavezni podaci.", (result as BadRequestObjectResult)?.Value);
        }

        //brisanje
        [Test]
        public async Task ObrisiRezervaciju_UspesnoBrisanje_VracaOkStatus()
        {
            Rezervacija rezervacija = new Rezervacija
            {
                Ime = "Ime",
                Prezime = "Prezime",
                BrojTelefona = "032645",
                Adresa = "Adresa",
                Grad = "Grad",
                Email = "email",
                BrojOsoba = 0
            };

            await _context.Rezervacije.AddAsync(rezervacija);
            await _context.SaveChangesAsync();

            var result = await _rezervacijaController.ObrisiRezervaciju(rezervacija.Id);

            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual($"Izbrisana je rezervacija korisnika: {rezervacija.Ime} {rezervacija.Prezime}", okResult.Value);
        }

        [Test]
        public async Task ObrisiRezervaciju_NepostojeciId_BadRequest()
        {
            var nepostojeciId = 9999;

            var result = await _rezervacijaController.ObrisiRezervaciju(nepostojeciId);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Nije uspelo brisanje rezervacije!", badRequestResult.Value);
        }

        [Test]
        public async Task ObrisiRezervaciju_ProveraBrisanjaIzBaze()
        {
            Rezervacija rezervacija = new Rezervacija
            {
                Ime = "Ime",
                Prezime = "Prezime",
                BrojTelefona = "032645",
                Adresa = "Adresa",
                Grad = "Grad",
                Email = "email",
                BrojOsoba = 0
            };

            await _context.Rezervacije.AddAsync(rezervacija);
            await _context.SaveChangesAsync();

            await _rezervacijaController.ObrisiRezervaciju(rezervacija.Id);
            var rezervacijaIzBaze = await _context.Rezervacije.FindAsync(rezervacija.Id);

            Assert.IsNull(rezervacijaIzBaze, null);
        }

        //preuzimanje rezervacija iz putovanja
        [Test]
        public async Task PreuzmiRezervacijePutovanja_NepostojeciId_VracaNotFound()
        {
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: "Putovanje")
                .Options;

            using (var context = new Context(options))
            {
                context.Putovanja.Add(new Putovanje { Mesto = "Destinacija1", BrojNocenja = 7, Cena = 500, Slika = "slika1", Prevoz = "prevoz1" });
                context.SaveChanges();
            }

            using (var context = new Context(options))
            {
                var controller = new RezervacijaController(context);
                int nepostojeciId = 999;

                var result = await controller.PreuzmiRezervacijePutovanja(nepostojeciId);

                Assert.IsInstanceOf<NotFoundObjectResult>(result);
                Assert.AreEqual("Putovanje nije pronađeno", (result as NotFoundObjectResult)?.Value);

            }
        }
        [Test]
        public async Task PreuzmiRezervacijePutovanja_VracaPraznuListu()
        {
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: "Putovanje")
                .Options;

            int putovanjeId;

            using (var context = new Context(options))
            {
                var putovanje = new Putovanje
                {
                    Mesto = "Test Mesto",
                    BrojNocenja = 5,
                    Cena = 1000,
                    Prevoz = "Autobus",
                    Slika = "urlSlike"
                };

                context.Putovanja.Add(putovanje);
                context.SaveChanges();
                putovanjeId = putovanje.Id;
            }

            using (var context = new Context(options))
            {
                var controller = new RezervacijaController(context);

                var result = await controller.PreuzmiRezervacijePutovanja(putovanjeId);
                Assert.IsInstanceOf<OkObjectResult>(result);
                Assert.IsNotNull((result as OkObjectResult)?.Value);
                var rezervacija = (result as OkObjectResult)?.Value as List<Rezervacija>;
                Assert.IsNotNull(rezervacija);
                Assert.IsEmpty(rezervacija);
            }
        }
        [Test]
        public async Task PreuzmiRezervacijePutovanja_ValidId_ReturnsOkWithPutovanjaList()
        {
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: "Putovanje")
                .Options;
            int putovanjeId;

            using (var context = new Context(options))
            {
                var putovanje = new Putovanje { Mesto = "Destinacija1", BrojNocenja = 7, Cena = 500, Slika = "slika1", Prevoz = "prevoz1" };
                putovanje.Rezervacije = new List<Rezervacija>
                {
                    new Rezervacija { Ime = "Ime", Prezime = "Prezime", BrojTelefona="12345680", Adresa="Adresa", Grad="Grad", Email="emali", BrojOsoba=3 },
                    new Rezervacija { Ime = "Ime1", Prezime = "Prezime1", BrojTelefona="123456801", Adresa="Adresa1", Grad="Grad1", Email="emali1", BrojOsoba=3 }
            };
                context.Putovanja.Add(putovanje);
                context.SaveChanges();
                putovanjeId = putovanje.Id;
            }

            using (var context = new Context(options))
            {
                var controller = new RezervacijaController(context);

                var result = await controller.PreuzmiRezervacijePutovanja(putovanjeId);

                Assert.IsInstanceOf<OkObjectResult>(result);
                Assert.IsNotNull((result as OkObjectResult)?.Value);
                var rezervacije = (result as OkObjectResult)?.Value as List<Rezervacija>;
                Assert.IsNotNull(rezervacije);
                Assert.IsNotEmpty(rezervacije);
            }
        }

        //preuzimanje podataka iz agencije sa zadatim idijem
        [Test]
        public async Task PreuzmiPutovanjaAgencije_NepostojeciId_VracaNotFound()
        {
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: "Putovanje")
                .Options;

            using (var context = new Context(options))
            {
                context.Agencije.Add(new Agencija { Naziv = "Test Agencija", Adresa = "Test Adresa", Grad = "Test Grad", BrojTelefona = "4552", Email = "email@gmail.com" });
                context.SaveChanges();
            }

            using (var context = new Context(options))
            {
                var controller = new PutovanjeController(context);
                int nepostojeciId = 999;

                var result = await controller.PreuzmiPutovanjaAgencije(nepostojeciId);

                Assert.IsInstanceOf<NotFoundObjectResult>(result);
                Assert.AreEqual("Agencija nije pronađena.", (result as NotFoundObjectResult)?.Value);

            }
        }


        //azuriranje
        [Test]
        public async Task AzurirajRezervaciju_VracaOkSaPorukom()
        {
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: "Putovanje")
                .Options;
            int rezervacijaId;

            using (var context = new Context(options))
            {
                Rezervacija rezervacija = new Rezervacija { Ime = "Ime", Prezime = "Prezime", BrojTelefona = "12345680", Adresa = "Adresa", Grad = "Grad", Email = "emali", BrojOsoba = 3 };
                context.Rezervacije.Add(rezervacija);
                context.SaveChanges();
                rezervacijaId = rezervacija.Id;
            }

            using (var context = new Context(options))
            {
                var controller = new RezervacijaController(context);
                var updatedRezervacija= new Rezervacija { Ime = "Ime", Prezime = "Prezime", BrojTelefona = "12345680", Adresa = "Adresa", Grad = "Grad", Email = "emali", BrojOsoba = 3 };

                var result = await controller.AzurirajRezervaciju(rezervacijaId, updatedRezervacija);

                Assert.IsInstanceOf<OkObjectResult>(result);
                Assert.AreEqual($"Azurirani su podaci o rezervaciji sa ID = {rezervacijaId}", (result as OkObjectResult)?.Value);
            }
        }

        [Test]
        public async Task AzurirajRezervaciju_NepostojeciId_VracaBadRequest()
        {
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: "Putovanje")
                .Options;

            using (var context = new Context(options))
            {
                var controller = new RezervacijaController(context);
                var updatedRezervacija = new Rezervacija { Ime = "Ime", Prezime = "Prezime", BrojTelefona = "12345680", Adresa = "Adresa", Grad = "Grad", Email = "emali", BrojOsoba = 3 };
                var result = await controller.AzurirajRezervaciju(999, updatedRezervacija);

                Assert.IsInstanceOf<BadRequestObjectResult>(result);
                Assert.AreEqual("Nije uspelo azuriranje rezervacije!", (result as BadRequestObjectResult)?.Value);
            }
        }

        [Test]
        public async Task AzurirajRezervaciju_ValidIdAndValidRecenzija_VracaOkSaPorukom()
        {
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: "Putovanje")
                .Options;
            int rezervacijaId;
            using (var context = new Context(options))
            {
                var rezervacija = new Rezervacija { Ime = "Ime", Prezime = "Prezime", BrojTelefona = "12345680", Adresa = "Adresa", Grad = "Grad", Email = "emali", BrojOsoba = 3 };
                context.Rezervacije.Add(rezervacija);
                context.SaveChanges();

                rezervacijaId = rezervacija.Id;
            }

            using (var context = new Context(options))
            {
                var controller = new RezervacijaController(context);
                var updatedRezervacija = new Rezervacija { Ime = "Ime1", Prezime = "Prezime1", BrojTelefona = "123456810", Adresa = "Adresa1", Grad = "Grad1", Email = "emali1", BrojOsoba = 3 };

                var result = await controller.AzurirajRezervaciju(rezervacijaId, updatedRezervacija);

                Assert.IsInstanceOf<OkObjectResult>(result);
                Assert.AreEqual($"Azurirani su podaci o rezervaciji sa ID = {rezervacijaId}", (result as OkObjectResult)?.Value);
            }
        }
    }
}
