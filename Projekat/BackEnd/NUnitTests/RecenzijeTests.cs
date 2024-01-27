using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTemplate.Controllers;

namespace NUnitTests
{
    public class RecenzijeTests
    {
        private WebTemplate.Controllers.RecenzijaController _recenzijeController;
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

            _recenzijeController = new WebTemplate.Controllers.RecenzijaController(_context);
        }
        //dodavanje recenzije za putovanje
        [Test]
        public async Task DodajRecenzijuPutovanja_PutovanjeNePostoji()
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
                var controller = new RecenzijaController(context);

                var recenzija = new Recenzija { Korisnik = "Korisnik", Komentar="Komentar", Ocena=5 };

                var result = await controller.DodajRecenzijuUPutovanje(1, recenzija);

                Assert.IsInstanceOf<NotFoundObjectResult>(result);
                var notFoundResult = (NotFoundObjectResult)result;
                Assert.AreEqual("Putovanje nije pronađeno.", notFoundResult.Value);
            }
        }

        [Test]
        public async Task DodajRecenzijuPutovanja_NeispravniPodaci_VracaBadRequest()
        {
            Putovanje putovanje = new Putovanje
            {
                BrojNocenja = 3,
                Cena = 100,
                Mesto = "Mesto",
                Slika = "Slika",
                Prevoz = "Prevoz"
            };

            Recenzija recenzija = new Recenzija
            {
                Korisnik = "Korisnik",
                Ocena = 0,
                Komentar="Komentar"
            };
            int putovanjeId = putovanje.Id;
            await _context.Putovanja.AddAsync(putovanje);
            await _context.SaveChangesAsync();

            var result = await _recenzijeController.DodajRecenzijuUPutovanje(putovanjeId, recenzija);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            Assert.AreEqual("Nisu uneti svi obavezni podaci.", (result as BadRequestObjectResult)?.Value);
        }
        [Test]
        public async Task DodajRecenzijuPutovanju_NepopunjenaPolja_VracaBadRequest()
        {
            Putovanje putovanje = new Putovanje
            {
                BrojNocenja = 2,
                Cena = 200,
                Mesto = "Mesto",
                Slika = "Slika",
                Prevoz = "Prevoz"
            };

            Recenzija recenzija = new Recenzija
            {
                Korisnik = "",
                Ocena = 4,
                Komentar = ""
            };
            int putovanjeId = putovanje.Id;
            await _context.Putovanja.AddAsync(putovanje);
            await _context.SaveChangesAsync();

            var result = await _recenzijeController.DodajRecenzijuUPutovanje(putovanjeId, recenzija);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            Assert.AreEqual("Nisu uneti svi obavezni podaci.", (result as BadRequestObjectResult)?.Value);
        }

        //brisanje
        [Test]
        public async Task ObrisiReceniziju_UspesnoBrisanje_VracaOkStatus()
        {
            Recenzija recenzija = new Recenzija
            {
                Korisnik = "Korisnik",
                Ocena = 4,
                Komentar = "Komentar"
            };

            await _context.Recenzije.AddAsync(recenzija);
            await _context.SaveChangesAsync();

            var result = await _recenzijeController.ObrisiRecenziju(recenzija.Id);

            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual($"Izbrisana je recenzija korisnika: {recenzija.Korisnik}", okResult.Value);
        }

        [Test]
        public async Task ObrisiRecenziju_NepostojeciId_BadRequest()
        {
            var nepostojeciId = 9999;

            var result = await _recenzijeController.ObrisiRecenziju(nepostojeciId);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Nije uspeslo brisanje recenzije!", badRequestResult.Value);
        }

        [Test]
        public async Task ObrisiRecenziju_ProveraBrisanjaIzBaze()
        {
            Recenzija recenzija = new Recenzija
            {
                Korisnik = "Korisnik",
                Ocena = 4,
                Komentar = "Komentar"
            };

            await _context.Recenzije.AddAsync(recenzija);
            await _context.SaveChangesAsync();

            await _recenzijeController.ObrisiRecenziju(recenzija.Id);
            var recenzijaIzBaze = await _context.Recenzije.FindAsync(recenzija.Id);

            Assert.IsNull(recenzijaIzBaze, null);
        }
        //preuzimanje recenzije iz putovanja
        [Test]
        public async Task PreuzmiRecenzijePutovanja_NepostojeciId_VracaNotFound()
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
                var controller = new RecenzijaController(context);
                int nepostojeciId = 999;

                var result = await controller.PreuzmiRecenzijeNaPutovanju(nepostojeciId);

                Assert.IsInstanceOf<NotFoundObjectResult>(result);
                Assert.AreEqual("Putovanje nije pronađeno", (result as NotFoundObjectResult)?.Value);

            }
        }
        [Test]
        public async Task PreuzmiRecenzijePutovanja_VracaPraznuListu()
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
                var controller = new RecenzijaController(context);

                var result = await controller.PreuzmiRecenzijeNaPutovanju(putovanjeId);
                Assert.IsInstanceOf<OkObjectResult>(result);
                Assert.IsNotNull((result as OkObjectResult)?.Value);
                var recenzija = (result as OkObjectResult)?.Value as List<Recenzija>;
                Assert.IsNotNull(recenzija);
                Assert.IsEmpty(recenzija);
            }
        }
        [Test]
        public async Task PreuzmiRecenzijePutovanja_ValidId_ReturnsOkWithPutovanjaList()
        {
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: "Putovanje")
                .Options;
            int putovanjeId;

            using (var context = new Context(options))
            {
                var putovanje = new Putovanje { Mesto = "Destinacija1", BrojNocenja = 7, Cena = 500, Slika = "slika1", Prevoz = "prevoz1" };
                putovanje.Recenzije = new List<Recenzija>
                {
                    new Recenzija {Korisnik="Korisnik", Komentar="Komentar", Ocena=2},
                    new Recenzija {Korisnik="Korisnik1", Komentar="Komentar1", Ocena=2}
                };
                context.Putovanja.Add(putovanje);
                context.SaveChanges();
                putovanjeId = putovanje.Id;
            }

            using (var context = new Context(options))
            {
                var controller = new RecenzijaController(context);

                var result = await controller.PreuzmiRecenzijeNaPutovanju(putovanjeId);

                Assert.IsInstanceOf<OkObjectResult>(result);
                Assert.IsNotNull((result as OkObjectResult)?.Value);
                var recenzije = (result as OkObjectResult)?.Value as List<Recenzija>;
                Assert.IsNotNull(recenzije);
                Assert.IsNotEmpty(recenzije);
            }
        }

        //azuriranje
        [Test]
        public async Task AzurirajRecenziju_VracaOkSaPorukom()
        {
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: "Putovanje")
                .Options;
            int recenzijaId;

            using (var context = new Context(options))
            {
                Recenzija recenzija = new Recenzija { Korisnik = "Korisnik", Komentar = "Komentar", Ocena = 2 };
                context.Recenzije.Add(recenzija);
                context.SaveChanges();
                recenzijaId = recenzija.Id;
            }

            using (var context = new Context(options))
            {
                var controller = new RecenzijaController(context);
                var updatedRecenzija = new Recenzija { Korisnik = "Korisnik1", Komentar = "Komentar1", Ocena = 5 };

                var result = await controller.AzurirajRecenziju(recenzijaId, updatedRecenzija);

                Assert.IsInstanceOf<OkObjectResult>(result);
                Assert.AreEqual($"Azurirani su podaci o recenziji sa ID = {recenzijaId}", (result as OkObjectResult)?.Value);
            }
        }

        [Test]
        public async Task AzurirajRecenziju_NepostojeciId_VracaBadRequest()
        {
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: "Putovanje")
                .Options;

            using (var context = new Context(options))
            {
                var controller = new RecenzijaController(context);
                var updatedRecenzija = new Recenzija { Korisnik = "Korisnik1", Komentar = "Komentar1", Ocena = 5 };

                var result = await controller.AzurirajRecenziju(999, updatedRecenzija);

                Assert.IsInstanceOf<BadRequestObjectResult>(result);
                Assert.AreEqual("Nije uspelo azuriranje recenzije!", (result as BadRequestObjectResult)?.Value);
            }
        }

        [Test]
        public async Task AzurirajRecenziju_ValidIdAndValidRecenzija_VracaOkSaPorukom()
        {
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: "Putovanje")
                .Options;
            int recenizijaId;
            using (var context = new Context(options))
            {
                var recenzija = new Recenzija { Korisnik = "Korisnik Korisnikic", Komentar = "Komentaric", Ocena = 5 };
                context.Recenzije.Add(recenzija);
                context.SaveChanges();

                recenizijaId = recenzija.Id;
            }

            using (var context = new Context(options))
            {
                var controller = new RecenzijaController(context);
                var updatedRecenzija = new Recenzija { Korisnik = "Anastasija Trajkvoic", Komentar = "Komentaric", Ocena = 5 };

                var result = await controller.AzurirajRecenziju(recenizijaId, updatedRecenzija);

                Assert.IsInstanceOf<OkObjectResult>(result);
                Assert.AreEqual($"Azurirani su podaci o recenziji sa ID = {recenizijaId}", (result as OkObjectResult)?.Value);
            }
        }
    }
}
