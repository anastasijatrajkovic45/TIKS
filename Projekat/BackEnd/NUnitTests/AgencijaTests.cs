using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Models;
using NUnit.Framework;
using System.IO;
using System.Threading.Tasks;
using WebTemplate.Controllers;

namespace NUnitTests
{
    public class AgencijaTests
    {
        private WebTemplate.Controllers.AgencijaContoller _agencijaController;
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

            _agencijaController = new WebTemplate.Controllers.AgencijaContoller(_context);
        }

        //DodajAgencija
        //treba da se doda agencija koja nema ovaj isti naziv koja vec postoji!!!!!
        [Test]
        public async Task DodajAgenciju_UspesnoDodavanje_VracaOkStatus()
        {
            Agencija novaAgencija = new Agencija
            {
                Naziv = "Agencija",
                Adresa = "Adresa",
                Grad = "Grad",
                Email = "nova@email.com",
                BrojTelefona = "123456789"
            };

            var result = await _agencijaController.DodajAgenciju(novaAgencija);

            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(novaAgencija, "novaAgencija nije instancirana.");

            Assert.IsNotNull(novaAgencija.Id, "Id nije postavljen nakon dodavanja agencije.");
            Assert.AreEqual($"ID nove agencije je = {novaAgencija.Id}", okResult?.Value);
        }

        [Test]
        public async Task DodajAgenciju_NeuspesnoDodavanje_VracaBadRequest()
        {
            Agencija agencijaSaGreskom = new Agencija
            {
                Naziv = "Nova agencija",
                Adresa = "Nova Adresa",
                Grad = "Novi Grad"
            };

            var result = await _agencijaController.DodajAgenciju(agencijaSaGreskom);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        //3.Test: BadRequest za Duplikate Naziva
        [Test]
        public async Task DodajAgenciju_DuplikatNaziva_BadRequest()
        {
            var postojećaAgencija = new Agencija
            {
                Naziv = "VecPostojecaAgencija",
                Adresa = "Test Adresa",
                Grad = "Test Grad",
                Email = "test@example.com",
                BrojTelefona = "123456789"
            };

            await _context.Agencije.AddAsync(postojećaAgencija);
            await _context.SaveChangesAsync();

            var novaAgencija = new Agencija
            {
                Naziv = "VecPostojecaAgencija",
                Adresa = "Nova Adresa",
                Grad = "Novi Grad",
                Email = "nova@test.com",
                BrojTelefona = "987654321"
            };

            var result = await _agencijaController.DodajAgenciju(novaAgencija);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Agencija sa istim nazivom već postoji.", badRequestResult.Value);
        }

        //ObrisiAgenciju
        //1.Test: Uspesno brisanje
        [Test]
        public async Task ObrisiAgenciju_UspesnoBrisanje_VracaOkStatus()
        {
            var agencija = new Agencija
            {
                Naziv = "Test Agencija",
                Adresa = "Test Adresa",
                Grad = "Test Grad",
                Email = "test@example.com",
                BrojTelefona = "123456789"
            };

            await _context.Agencije.AddAsync(agencija);
            await _context.SaveChangesAsync();

            var result = await _agencijaController.ObrisiAgenciju(agencija.Id);

            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual($"Izbrisana je agencija: {agencija.Naziv}", okResult.Value);
        }

        //2.Test: Brisanje nepostojece agencije
        [Test]
        public async Task ObrisiAgenciju_NepostojecaAgencija_BadRequest()
        {
            var nepostojeciId = 9999;

            var result = await _agencijaController.ObrisiAgenciju(nepostojeciId);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Nije uspelo brisanje agencije!", badRequestResult.Value);
        }

        //3.Test: da li se obrisao iz baze
        [Test]
        public async Task ObrisiAgenciju_ProveraBrisanjaIzBaze()
        {
            var agencija = new Agencija
            {
                Naziv = "Test Agencija",
                Adresa = "Test Adresa",
                Grad = "Test Grad",
                Email = "test@example.com",
                BrojTelefona = "123456789"
            };

            await _context.Agencije.AddAsync(agencija);
            await _context.SaveChangesAsync();

            await _agencijaController.ObrisiAgenciju(agencija.Id);
            var agencijaIzBaze = await _context.Agencije.FindAsync(agencija.Id);

            Assert.IsNull(agencijaIzBaze, "Agencija nije obrisana iz baze.");
        }

        //PreuzmiAgencije
        [Test]
        public async Task PrezumiAgencije_UspesnoPreuzimanje_VracaOKStatus()
        {
            var agencije = new List<Agencija>
            {
                new Agencija { Naziv = "Agencija1", Adresa = "Adresa1", Grad = "Grad1", Email = "email1@example.com", BrojTelefona = "123456789" },
                new Agencija { Naziv = "Agencija2", Adresa = "Adresa2", Grad = "Grad2", Email = "email2@example.com", BrojTelefona = "987654321" }
            };

            await _context.Agencije.AddRangeAsync(agencije);
            await _context.SaveChangesAsync();

            var result = await _agencijaController.PrezumiAgencije() as OkObjectResult;

            Assert.IsNotNull(result);
            var preuzeteAgencije = result.Value as List<Agencija>;
            Assert.IsNotNull(preuzeteAgencije);

            Assert.AreEqual(_context.Agencije.Count(), preuzeteAgencije.Count);
        }
        [Test]
        public async Task PreuzmiAgencije_PraznaLista()
        {
            var dbContextOptions = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new Context(dbContextOptions))
            {
                var controller = new AgencijaContoller(context);
                var result = await controller.PrezumiAgencije();

                Assert.IsInstanceOf<OkObjectResult>(result);
                var okObjectResult = (OkObjectResult)result;
                var agencije = (List<Agencija>)okObjectResult.Value;
                Assert.IsEmpty(agencije);
            }
        }


        //preuzima jednu agenciju
        [Test]
        public async Task PrezumiAgenciju_Uspeh_VracaOkStatusSaTacnimPodacima()
        {
            var novaAgencija = new Agencija
            {
                Naziv = "NovaAgencija",
                Adresa = "Test Adresa",
                Grad = "Novi Grad",
                BrojTelefona = "123456789",
                Email = "test@example.com"
            };

            await _context.Agencije.AddAsync(novaAgencija);
            await _context.SaveChangesAsync();

            var result = await _agencijaController.PrezumiAgenciju(novaAgencija.Id) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsNotNull(result.Value);

            var preuzetaAgencija = result.Value as Agencija;
            Assert.IsNotNull(preuzetaAgencija);
            Assert.AreEqual(novaAgencija.Naziv, preuzetaAgencija.Naziv);
            Assert.AreEqual(novaAgencija.Adresa, preuzetaAgencija.Adresa);
        }

        [Test]
        public async Task PrezumiAgenciju_NePostojiAgencijaSaZadatimIdem_VracaNotFoundResult()
        {
            var nepostojeciId = 999;
            var result = await _agencijaController.PrezumiAgenciju(nepostojeciId);
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task PrezumiAgenciju_DrugaAgencijaSaValidnimIdem_VracaOkStatusSaTacnimPodacima()
        {
            var prvaAgencija = new Agencija
            {
                Naziv = "PrvaAgencija",
                Adresa = "Adresa1",
                Grad = "Grad1",
                BrojTelefona = "123456789",
                Email = "email1@example.com"
            };

            var drugaAgencija = new Agencija
            {
                Naziv = "DrugaAgencija",
                Adresa = "Adresa2",
                Grad = "Grad2",
                BrojTelefona = "987654321",
                Email = "email2@example.com"
            };

            await _context.Agencije.AddRangeAsync(prvaAgencija, drugaAgencija);
            await _context.SaveChangesAsync();

            var result = await _agencijaController.PrezumiAgenciju(drugaAgencija.Id) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsNotNull(result.Value);

            var preuzetaAgencija = result.Value as Agencija;
            Assert.IsNotNull(preuzetaAgencija);
            Assert.AreEqual(drugaAgencija.Naziv, preuzetaAgencija.Naziv);
            Assert.AreEqual(drugaAgencija.Adresa, preuzetaAgencija.Adresa);
            Assert.AreEqual(drugaAgencija.Grad, preuzetaAgencija.Grad);
            Assert.AreEqual(drugaAgencija.BrojTelefona, preuzetaAgencija.BrojTelefona);
            Assert.AreEqual(drugaAgencija.Email, preuzetaAgencija.Email);

        }

        //azuriranje
        [Test]
        public async Task AzurirajAgenciju_VracaOkSaPorukom()
        {
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: "Putovanje")
                .Options;
            int agencijaId;

            using (var context = new Context(options))
            {
                Agencija agencija = new Agencija { Naziv = "Test Agencija", Adresa = "Test Adresa", Grad = "Test Grad", BrojTelefona = "4552", Email = "email@gmail.com" };
                context.Agencije.Add(agencija);
                context.SaveChanges();
                agencijaId = agencija.Id;
            }

            using (var context = new Context(options))
            {
                var controller = new AgencijaContoller(context);
                var updatedAgenciju = new Agencija { Naziv = "Test Agencija1", Adresa = "Test Adresa1", Grad = "Test Grad1", BrojTelefona = "45521", Email = "email1@gmail.com" };

                var result = await controller.AzurirajAgenciju(agencijaId, updatedAgenciju);

                Assert.IsInstanceOf<OkObjectResult>(result);
                Assert.AreEqual($"Azurirana je agencija sa ID = {agencijaId}", (result as OkObjectResult)?.Value);
            }
        }

        [Test]
        public async Task AzurirajAgenciju_NepostojeciId_VracaBadRequest()
        {
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: "Putovanje")
                .Options;

            using (var context = new Context(options))
            {
                var controller = new AgencijaContoller(context);
                var updatedAgenciju = new Agencija { Naziv = "Test Agencija1", Adresa = "Test Adresa1", Grad = "Test Grad1", BrojTelefona = "45521", Email = "email1@gmail.com" };

                var result = await controller.AzurirajAgenciju(999, updatedAgenciju);

                Assert.IsInstanceOf<BadRequestObjectResult>(result);
                Assert.AreEqual("Nije uspelo azuriranje agencije!", (result as BadRequestObjectResult)?.Value);
            }
        }

        [Test]
        public async Task AzurirajAgenciju_ValidIdAndValidPutovanje_VracaOkSaPorukom()
        {
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: "Putovanje")
                .Options;
            int agencijaId;
            using (var context = new Context(options))
            {
                var agencija = new Agencija { Naziv = "Test Agencija1", Adresa = "Test Adresa1", Grad = "Test Grad1", BrojTelefona = "45521", Email = "email1@gmail.com" };
                context.Agencije.Add(agencija);
                context.SaveChanges();

                agencijaId = agencija.Id;
            }

            using (var context = new Context(options))
            {
                var controller = new AgencijaContoller(context);
                var updatedAgencija = new Agencija { Naziv = "Test Agencija", Adresa = "Test Adresa", Grad = "Test Grad", BrojTelefona = "4552", Email = "email@gmail.com" };

                var result = await controller.AzurirajAgenciju(agencijaId, updatedAgencija);

                Assert.IsInstanceOf<OkObjectResult>(result);
                Assert.AreEqual($"Azurirana je agencija sa ID = {agencijaId}", (result as OkObjectResult)?.Value);
            }
        }
    }
}
