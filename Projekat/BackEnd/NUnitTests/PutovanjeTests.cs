using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Win32.SafeHandles;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTemplate.Controllers;

namespace NUnitTests
{
    public class PutovanjeTests
    {
        private WebTemplate.Controllers.PutovanjeController _putovanjeController;
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

            _putovanjeController = new WebTemplate.Controllers.PutovanjeController(_context);
        }

        //dodavanje
        [Test]
        public async Task DodajPutovanje_UspesnoDodavanje_VracaOkStatus()
        {
            Putovanje novoPutovanje = new Putovanje
            {
                Mesto = "Nova Destinacija",
                Slika = "putanja/do/slike",
                Prevoz = "Novi Prevoz",
                BrojNocenja=2,
                Cena=250
            };
            var rezultat = await _putovanjeController.DodajPutovanje(novoPutovanje);

            Assert.IsInstanceOf<OkObjectResult>(rezultat);
            var okRezultat = rezultat as OkObjectResult;

            Assert.IsNotNull(novoPutovanje, "novoPutovanje nije instancirano.");

            Assert.IsNotNull(novoPutovanje.Id, "Id nije postavljen nakon dodavanja putovanja.");
            Assert.AreEqual($"ID novog putovanja je = {novoPutovanje.Id}", okRezultat?.Value);
        }

        [Test]
        public async Task DodajPutovanje_NeuspesnoDodavanje_VracaBadRequest()
        {
            Putovanje putovanjeSaGreskom = new Putovanje
            {
                Mesto = "Novo mesto",
                Prevoz = "Novi prevoz",
                Slika = "Nova Slika",
                BrojNocenja = 0,
                Cena = 240
            };

            var result = await _putovanjeController.DodajPutovanje(putovanjeSaGreskom);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task DodajPutovanje_SaIspravnimPodacima_VracaOkStatus()
        {
            Putovanje postojecePutovanje = new Putovanje
            {
                Mesto = "VecPostojecaDestinacija",
                Slika = "Test Slika",
                Prevoz = "Test Prevoz",
                BrojNocenja = 3,
                Cena = 400
            };

            await _context.Putovanja.AddAsync(postojecePutovanje);
            await _context.SaveChangesAsync();

            Putovanje novoPutovanje = new Putovanje
            {
                Mesto = "Nova Destinacija",
                Slika = "Nova Slika",
                Prevoz = "Novi Prevoz",
                BrojNocenja = 5,
                Cena = 600
            };

            var result = await _putovanjeController.DodajPutovanje(novoPutovanje);

            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(novoPutovanje.Id, "Id nije postavljen nakon dodavanja putovanja.");
            Assert.AreEqual($"ID novog putovanja je = {novoPutovanje.Id}", okResult?.Value);
        }

        //dodavanje putovanja u agenciju
        [Test]
        public async Task DodajPutovanjeAgenciji_AgencijaNePostoji()
        {
           
            var dbContextOptions = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new Context(dbContextOptions))
            {
                // Kontekst je prazan, ne postoji agencija sa Id
            }

            using (var context = new Context(dbContextOptions))
            {
                var controller = new PutovanjeController(context);

                var putovanje = new Putovanje { Mesto = "Destinacija", BrojNocenja = 7, Prevoz ="Prevoz", Slika="SLika", Cena = 500 };

                var result = await controller.DodajPutovanjeAgenciji(putovanje, 1);

                Assert.IsInstanceOf<NotFoundObjectResult>(result);
                var notFoundResult = (NotFoundObjectResult)result;
                Assert.AreEqual("Agencija nije pronađena", notFoundResult.Value);
            }
        }

        [Test]
        public async Task DodajPutovanjeAgenciji_NeispravneVrednosti_VracaBadRequest()
        {
            Putovanje putovanjeNepopunjeno = new Putovanje
            {
                BrojNocenja=0,
                Cena=0,
                Mesto="Mesto",
                Slika="Slika",
                Prevoz="Prevoz"
            };

            Agencija agencija = new Agencija
            { 
                Naziv="Naziv",
                Adresa="Adresa",
                Grad="Grad",
                Email="Email",
                BrojTelefona="Brojtf"
            };
            int agencijaId = agencija.Id;
            await _context.Agencije.AddAsync(agencija);
            await _context.SaveChangesAsync();

            var result = await _putovanjeController.DodajPutovanjeAgenciji(putovanjeNepopunjeno, agencijaId);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            Assert.AreEqual("Nisu uneti svi obavezni podaci.", (result as BadRequestObjectResult)?.Value);
        }

        [Test]
        public async Task DodajPutovanjeAgenciji_NepopunjenaPolja_VracaBadRequest()
        {
            Putovanje putovanjeNepopunjeno = new Putovanje
            {
                BrojNocenja = 2,
                Cena = 200,
                Mesto = "",
                Slika = "Slika",
                Prevoz = "Prevoz"
            };

            Agencija agencija = new Agencija
            {
                Naziv = "Naziv",
                Adresa = "Adresa",
                Grad = "Grad",
                Email = "Email",
                BrojTelefona = "Brojtf"
            };
            int agencijaId = agencija.Id;
            await _context.Agencije.AddAsync(agencija);
            await _context.SaveChangesAsync();

            var result = await _putovanjeController.DodajPutovanjeAgenciji(putovanjeNepopunjeno, agencijaId);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            Assert.AreEqual("Nisu uneti svi obavezni podaci.", (result as BadRequestObjectResult)?.Value);
        }


        //brisanje
        [Test]
        public async Task ObrisiPutovanje_UspesnoBrisanje_VracaOkStatus()
        {
            var putovanje = new Putovanje
            {
                Mesto = "Test Mesto",
                BrojNocenja = 5,
                Cena = 1000,
                Prevoz = "Autobus",
                Slika = "urlSlike"
            };

            await _context.Putovanja.AddAsync(putovanje);
            await _context.SaveChangesAsync();

            var result = await _putovanjeController.ObrisiPutovanje(putovanje.Id);

            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual($"Izbrisano je putovanje na mestu: {putovanje.Mesto}", okResult.Value);
        }

        //2.Test: Brisanje nepostojeceg putovanja
        [Test]
        public async Task ObrisiPutovanje_NepostojecegPutovanja_BadRequest()
        {
            var nepostojeciId = 9999;

            var result = await _putovanjeController.ObrisiPutovanje(nepostojeciId);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Nije uspelo brisanje putovanja!", badRequestResult.Value);
        }

        //3.Test: da li se obrisao iz baze
        [Test]
        public async Task ObrisiPutovanje_ProveraBrisanjaIzBaze()
        {
            var putovanje = new Putovanje
            {
                Mesto = "Test Mesto",
                BrojNocenja = 5,
                Cena = 1000,
                Prevoz = "Autobus",
                Slika = "urlSlike"
            };

            await _context.Putovanja.AddAsync(putovanje);
            await _context.SaveChangesAsync();

            await _putovanjeController.ObrisiPutovanje(putovanje.Id);
            var putovanjeIzBaze = await _context.Putovanja.FindAsync(putovanje.Id);

            Assert.IsNull(putovanjeIzBaze, "Putovanje nije obrisana iz baze.");
        }

        //preuzimanje
        [Test]
        public async Task PreuzmiPutovanja_Uspesno()
        {
            var dbContextOptions = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new Context(dbContextOptions))
            {
                var putovanje1 = new Putovanje { Mesto = "Destinacija1", BrojNocenja = 7, Cena = 500, Slika="slika1", Prevoz="prevoz1"};
                var putovanje2 = new Putovanje { Mesto = "Destinacija2", BrojNocenja = 5, Cena = 700, Slika = "slika1", Prevoz = "prevoz1" };

                context.Putovanja.AddRange(putovanje1, putovanje2);
                await context.SaveChangesAsync();
            }

            using (var context = new Context(dbContextOptions))
            {
                var controller = new PutovanjeController(context);
                var result = await controller.PreuzmiPutovanja();

                Assert.IsInstanceOf<OkObjectResult>(result);
                var okObjectResult = (OkObjectResult)result;
                var putovanja = (List<Putovanje>)okObjectResult.Value;
                Assert.AreEqual(2, putovanja.Count);
            }
        }

        [Test]
        public async Task PreuzmiPutovanja_PraznaLista()
        {
            var dbContextOptions = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new Context(dbContextOptions))
            {
                var controller = new PutovanjeController(context);
                var result = await controller.PreuzmiPutovanja();

                Assert.IsInstanceOf<OkObjectResult>(result);
                var okObjectResult = (OkObjectResult)result;
                var putovanja = (List<Putovanje>)okObjectResult.Value;
                Assert.IsEmpty(putovanja);
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
                context.Agencije.Add(new Agencija {Naziv = "Test Agencija", Adresa = "Test Adresa", Grad = "Test Grad", BrojTelefona = "4552", Email = "email@gmail.com"});
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
        [Test]
        public async Task PreuzmiPutovanjaAgencije_VracaPraznuListu()
        {
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: "Putovanje")
                .Options;

            int agencijaId;

            using (var context = new Context(options))
            {
                var novaAgencija = new Agencija
                {
                    Naziv = "Test Agencija",
                    Adresa = "Test Adresa",
                    Grad = "Test Grad",
                    BrojTelefona = "4552",
                    Email = "email@gmail.com"
                };

                context.Agencije.Add(novaAgencija);
                context.SaveChanges();
                agencijaId = novaAgencija.Id;
            }

            using (var context = new Context(options))
            {
                var controller = new PutovanjeController(context);

                var result = await controller.PreuzmiPutovanjaAgencije(agencijaId);
                Assert.IsInstanceOf<OkObjectResult>(result);
                Assert.IsNotNull((result as OkObjectResult)?.Value);
                var putovanja = (result as OkObjectResult)?.Value as List<Putovanje>;
                Assert.IsNotNull(putovanja);
                Assert.IsEmpty(putovanja);
            }
        }
        [Test]
        public async Task PreuzmiPutovanjaAgencije_ValidId_ReturnsOkWithPutovanjaList()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: "Putovanje")
                .Options;
            int agencijaId;

            using (var context = new Context(options))
            {
                var agencija = new Agencija { Naziv = "Test Agencija", Adresa = "Test Adresa", Grad = "Test Grad", BrojTelefona = "4552", Email = "email@gmail.com" };
                agencija.Putovanje = new List<Putovanje>
                {
                    new Putovanje { Mesto = "Destinacija1", BrojNocenja = 7, Cena = 500, Slika="slika1", Prevoz="prevoz1"},
                    new Putovanje { Mesto = "Destinacija2", BrojNocenja = 5, Cena = 700, Slika = "slika1", Prevoz = "prevoz1" }
                };
                context.Agencije.Add(agencija);
                context.SaveChanges();
                agencijaId = agencija.Id;
            }

            using (var context = new Context(options))
            {
                var controller = new PutovanjeController(context);

                var result = await controller.PreuzmiPutovanjaAgencije(agencijaId);

                Assert.IsInstanceOf<OkObjectResult>(result);
                Assert.IsNotNull((result as OkObjectResult)?.Value);
                var putovanja = (result as OkObjectResult)?.Value as List<Putovanje>;
                Assert.IsNotNull(putovanja);
                Assert.IsNotEmpty(putovanja);
            }
        }
        //azuriranje
        [Test]
        public async Task AzurirajPutovanje_VracaOkSaPorukom()
        {
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: "Putovanje")
                .Options;
            int putovanjeId;

            using (var context = new Context(options))
            {
                Putovanje putovanje = new Putovanje { Mesto = "Destinacija1", BrojNocenja = 7, Cena = 500, Slika = "slika1", Prevoz = "prevoz1" };
                context.Putovanja.Add(putovanje);
                context.SaveChanges();
                putovanjeId = putovanje.Id;
            }

            using (var context = new Context(options))
            {
                var controller = new PutovanjeController(context);
                var updatedPutovanje = new Putovanje { Mesto = "Destinacija2", BrojNocenja = 5, Cena = 600, Slika = "slika2", Prevoz = "prevoz2" };

                var result = await controller.AzurirajPutovanje(putovanjeId, updatedPutovanje);

                Assert.IsInstanceOf<OkObjectResult>(result);
                Assert.AreEqual($"Azurirano je putovanje sa ID = {putovanjeId}", (result as OkObjectResult)?.Value);
            }
        }

        [Test]
        public async Task AzurirajPutovanje_NepostojeciId_VracaBadRequest()
        {
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: "Putovanje")
                .Options;

            using (var context = new Context(options))
            {
                var controller = new PutovanjeController(context);
                var updatedPutovanje = new Putovanje { Mesto = "Destinacija2", BrojNocenja = 5, Cena = 600, Slika = "slika2", Prevoz = "prevoz2" };
              
                var result = await controller.AzurirajPutovanje(999, updatedPutovanje);

                Assert.IsInstanceOf<BadRequestObjectResult>(result);
                Assert.AreEqual("Nije uspelo azuriranje putovanja!", (result as BadRequestObjectResult)?.Value);
            }
        }

        [Test]
        public async Task AzurirajPutovanje_ValidIdAndValidPutovanje_VracaOkSaPorukom()
        {
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: "Putovanje")
                .Options;
            int putovanjeId;
            using (var context = new Context(options))
            {
                var putovanje = new Putovanje { Mesto = "Destinacija1", BrojNocenja = 5, Cena = 600, Slika = "slika1", Prevoz = "prevoz1" };
                context.Putovanja.Add(putovanje);
                context.SaveChanges();

                putovanjeId=putovanje.Id; 
            }

            using (var context = new Context(options))
            {
                var controller = new PutovanjeController(context);
                var updatedPutovanje = new Putovanje { Mesto = "Destinacija2", BrojNocenja = 5, Cena = 600, Slika = "slika2", Prevoz = "prevoz2" };

                var result = await controller.AzurirajPutovanje(putovanjeId, updatedPutovanje);

                Assert.IsInstanceOf<OkObjectResult>(result);
                Assert.AreEqual($"Azurirano je putovanje sa ID = {putovanjeId}", (result as OkObjectResult)?.Value);
            }
        }

    }
}
