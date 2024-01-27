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
    public class AktivnostiTests
    {
        private WebTemplate.Controllers.AktivnostiController _aktivnostiController;
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

            _aktivnostiController = new WebTemplate.Controllers.AktivnostiController(_context);
        }

        //dodavanje aktivnosti u putovanje
        [Test]
        public async Task DodajAktivnostPutovanju_PutovanjeNePostoji()
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
                var controller = new AktivnostiController(context);

                var aktivnost = new Aktivnost {Naziv="Aktivnost", Cena=1000 };

                var result = await controller.DodajAktivnostUPutovanje(1, aktivnost);

                Assert.IsInstanceOf<NotFoundObjectResult>(result);
                var notFoundResult = (NotFoundObjectResult)result;
                Assert.AreEqual("Putovanje nije pronađeno.", notFoundResult.Value);
            }
        }

        [Test]
        public async Task DodajAktivnostPutovanju_NeispravneVrednosti_VracaBadRequest()
        {
            Putovanje putovanje = new Putovanje
            {
                BrojNocenja = 3,
                Cena = 100,
                Mesto = "Mesto",
                Slika = "Slika",
                Prevoz = "Prevoz"
            };

            Aktivnost aktivnost = new Aktivnost
            {
                Naziv = "Naziv",
                Cena=0
            };
            int putovanjeId = putovanje.Id;
            await _context.Putovanja.AddAsync(putovanje);
            await _context.SaveChangesAsync();

            var result = await _aktivnostiController.DodajAktivnostUPutovanje(putovanjeId, aktivnost);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            Assert.AreEqual("Nisu uneti svi obavezni podaci.", (result as BadRequestObjectResult)?.Value);
        }
        [Test]
        public async Task DodajAktivnostPutovanju_NepopunjenaPolja_VracaBadRequest()
        {
            Putovanje putovanje = new Putovanje
            {
                BrojNocenja = 2,
                Cena = 200,
                Mesto = "Mesto",
                Slika = "Slika",
                Prevoz = "Prevoz"
            };

            Aktivnost aktivnost = new Aktivnost
            {
                Naziv = "",
                Cena = 200
            };
            int putovanjeId = putovanje.Id;
            await _context.Putovanja.AddAsync(putovanje);
            await _context.SaveChangesAsync();

            var result = await _aktivnostiController.DodajAktivnostUPutovanje(putovanjeId, aktivnost);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            Assert.AreEqual("Nisu uneti svi obavezni podaci.", (result as BadRequestObjectResult)?.Value);
        }

        //brisanje
        [Test]
        public async Task ObrisiAktivnost_UspesnoBrisanje_VracaOkStatus()
        {
            Aktivnost aktivnost = new Aktivnost
            {
                Naziv = "Naziv",
                Cena = 1000
            };

            await _context.Aktivnosti.AddAsync(aktivnost);
            await _context.SaveChangesAsync();

            var result = await _aktivnostiController.ObrisiAktivnost(aktivnost.Id);

            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual($"Izbrisana je aktivnost: {aktivnost.Naziv} {aktivnost.Cena}", okResult.Value);
        }

        [Test]
        public async Task ObrisiAktivnost_NepostojeciId_BadRequest()
        {
            var nepostojeciId = 9999;

            var result = await _aktivnostiController.ObrisiAktivnost(nepostojeciId);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Nije uspeslo brisanje aktivnosti!", badRequestResult.Value);
        }

        [Test]
        public async Task ObrisiAktivnost_ProveraBrisanjaIzBaze()
        {
            Aktivnost aktivnost = new Aktivnost
            {
                Naziv = "Naziv",
                Cena = 1000
            };

            await _context.Aktivnosti.AddAsync(aktivnost);
            await _context.SaveChangesAsync();

            await _aktivnostiController.ObrisiAktivnost(aktivnost.Id);
            var aktivnostIzBaze = await _context.Putovanja.FindAsync(aktivnost.Id);

            Assert.IsNull(aktivnostIzBaze,null);
        }

        //preuzimanje aktivnosti iz putovanja
        [Test]
        public async Task PreuzmiAktivnostiPutovanja_NepostojeciId_VracaNotFound()
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
                var controller = new AktivnostiController(context);
                int nepostojeciId = 999;

                var result = await controller.PreuzmiAktivnostiNaPutovanju(nepostojeciId);

                Assert.IsInstanceOf<NotFoundObjectResult>(result);
                Assert.AreEqual("Putovanje nije pronađeno", (result as NotFoundObjectResult)?.Value);

            }
        }
        [Test]
        public async Task PreuzmiAktivnostiPutovanja_VracaPraznuListu()
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
                var controller = new AktivnostiController(context);

                var result = await controller.PreuzmiAktivnostiNaPutovanju(putovanjeId);
                Assert.IsInstanceOf<OkObjectResult>(result);
                Assert.IsNotNull((result as OkObjectResult)?.Value);
                var aktivnost = (result as OkObjectResult)?.Value as List<Aktivnost>;
                Assert.IsNotNull(aktivnost);
                Assert.IsEmpty(aktivnost);
            }
        }
        [Test]
        public async Task PreuzmiAktivnostiPutovanja_ValidId_ReturnsOkWithPutovanjaList()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: "Putovanje")
                .Options;
            int putovanjeId;

            using (var context = new Context(options))
            {
                var putovanje = new Putovanje { Mesto = "Destinacija1", BrojNocenja = 7, Cena = 500, Slika = "slika1", Prevoz = "prevoz1" };
                putovanje.Aktivnosti = new List<Aktivnost>
                {
                    new Aktivnost {Naziv="Aktivnost1", Cena=1000},
                    new Aktivnost {Naziv="Aktivnost2", Cena=2000}
                };
                context.Putovanja.Add(putovanje);
                context.SaveChanges();
                putovanjeId = putovanje.Id;
            }

            using (var context = new Context(options))
            {
                var controller = new AktivnostiController(context);

                var result = await controller.PreuzmiAktivnostiNaPutovanju(putovanjeId);

                Assert.IsInstanceOf<OkObjectResult>(result);
                Assert.IsNotNull((result as OkObjectResult)?.Value);
                var aktivnosti = (result as OkObjectResult)?.Value as List<Aktivnost>;
                Assert.IsNotNull(aktivnosti);
                Assert.IsNotEmpty(aktivnosti);
            }
        }
        //azuriranje
        [Test]
        public async Task AzurirajAktivnost_VracaOkSaPorukom()
        {
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: "Putovanje")
                .Options;
            int aktivnostId;

            using (var context = new Context(options))
            {
                Aktivnost aktivnost = new Aktivnost { Naziv = "Aktivnost1", Cena = 1000 };
                context.Aktivnosti.Add(aktivnost);
                context.SaveChanges();
                aktivnostId = aktivnost.Id;
            }

            using (var context = new Context(options))
            {
                var controller = new AktivnostiController(context);
                var updatedAktivnost = new Aktivnost { Naziv = "Aktivnost2", Cena = 1000 };

                var result = await controller.AzurirajAktivnost(aktivnostId, updatedAktivnost);

                Assert.IsInstanceOf<OkObjectResult>(result);
                Assert.AreEqual($"Azurirani su podaci o aktivnosti sa ID = {aktivnostId}", (result as OkObjectResult)?.Value);
            }
        }

        [Test]
        public async Task AzurirajAktivnost_NepostojeciId_VracaBadRequest()
        {
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: "Putovanje")
                .Options;

            using (var context = new Context(options))
            {
                var controller = new AktivnostiController(context);
                var updatedAktivnost = new Aktivnost { Naziv = "Aktivnost2", Cena = 1000 };

                var result = await controller.AzurirajAktivnost(999, updatedAktivnost);

                Assert.IsInstanceOf<BadRequestObjectResult>(result);
                Assert.AreEqual("Nije uspelo azuriranje aktivnosti!", (result as BadRequestObjectResult)?.Value);
            }
        }

        [Test]
        public async Task AzurirajAktivnost_ValidIdAndValidAktivnost_VracaOkSaPorukom()
        {
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: "Putovanje")
                .Options;
            int aktivnostId;
            using (var context = new Context(options))
            {
                var aktivnost = new Aktivnost { Naziv = "Aktivnost", Cena = 1000 };
                context.Aktivnosti.Add(aktivnost);
                context.SaveChanges();

                aktivnostId = aktivnost.Id;
            }

            using (var context = new Context(options))
            {
                var controller = new AktivnostiController(context);
                var updatedAktivnost = new Aktivnost { Naziv = "Aktivnost3", Cena = 2000 };

                var result = await controller.AzurirajAktivnost(aktivnostId, updatedAktivnost);

                Assert.IsInstanceOf<OkObjectResult>(result);
                Assert.AreEqual($"Azurirani su podaci o aktivnosti sa ID = {aktivnostId}", (result as OkObjectResult)?.Value);
            }
        }

    }
}
