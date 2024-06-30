using Invoice_generator.DTOs.Invoice;
using Invoice_generator.DTOs.Item;
using Invoice_generator.Entities;
using Invoice_generator.Interfaces;
using Invoice_generator.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Invoice_generator.Tests
{
    public class InvoiceServiceTests
    {
        private readonly InvoiceService _invoiceService;
        private readonly Mock<ICountriesService> _mockCountriesService;

        private readonly DbContextOptions<DBContext> _dbContextOptions;
        private readonly DBContext _dbContext;


        public InvoiceServiceTests()
        {
            var dbContext = new Mock<DBContext>();
            _mockCountriesService = new Mock<ICountriesService>();

            _dbContextOptions = new DbContextOptionsBuilder<DBContext>()
                .UseInMemoryDatabase(databaseName: "InvoiceDatabase")
                .Options;

            _dbContext = new DBContext(_dbContextOptions);

            _invoiceService = new InvoiceService(_dbContext, _mockCountriesService.Object);
        }

        [Fact]
        public async Task CalculatePrice_SellerNotVATPayer_ReturnsTotalPriceWithoutVAT()
        {
            // Arrange
            var items = new List<InvoicePurchases>
            {
                new InvoicePurchases { Item = new Item { Id = Guid.NewGuid(), Name = "I1", Code = "I1", Price = 100 }, Amount = 2 },
                new InvoicePurchases { Item = new Item { Id = Guid.NewGuid(), Name = "I2", Code = "I2", Price = 50 }, Amount = 1 }
            };

            var seller = new Seller 
            { 
                VATPayer = false,
                Id = Guid.NewGuid(),
                Name = "S",
                Address = "S",
                CompanyCode = "1",
                VATPayerCode = "1",
                CountryCode = "LT",
            };

            var client = new BaseClient 
            { 
                CountryCode = "US",
                Name = "C",
                Address = "C",
                ClientType = Enums.ClientType.Company,
            };

            // Act
            var result = await _invoiceService.CalculatePrice(items, seller, client);

            // Assert
            Assert.Equal((250, 250, 0), result);
        }

        [Fact]
        public async Task CalculatePrice_ClientNotInEU_ReturnsTotalPriceWithoutVAT()
        {
            // Arrange
            var items = new List<InvoicePurchases>
            {
                new InvoicePurchases { Item = new Item { Id = Guid.NewGuid(), Name = "I1", Code = "I1", Price = 100 }, Amount = 2 },
                new InvoicePurchases { Item = new Item { Id = Guid.NewGuid(), Name = "I2", Code = "I2", Price = 50 }, Amount = 1 }
            };
            var seller = new Seller
            {
                VATPayer = true,
                Id = Guid.NewGuid(),
                Name = "S",
                Address = "S",
                CompanyCode = "1",
                VATPayerCode = "1",
                CountryCode = "LT",
            };

            var client = new BaseClient
            {
                CountryCode = "US",
                Name = "C",
                Address = "C",
                ClientType = Enums.ClientType.Company,
            };

            _mockCountriesService.Setup(cs => cs.IsCountryInEU(client.CountryCode)).Returns(false);

            // Act
            var result = await _invoiceService.CalculatePrice(items, seller, client);

            // Assert
            Assert.Equal((250, 250, 0), result);
        }

        [Fact]
        public async Task CalculatePrice_ClientInEU_ReturnsTotalPriceWithVAT()
        {
            // Arrange
            var items = new List<InvoicePurchases>
            {
                new InvoicePurchases { Item = new Item { Id = Guid.NewGuid(), Name = "I1", Code = "I1", Price = 100 }, Amount = 2 },
                new InvoicePurchases { Item = new Item { Id = Guid.NewGuid(), Name = "I2", Code = "I2", Price = 50 }, Amount = 1 }
            };

            var seller = new Seller
            {
                VATPayer = true,
                Id = Guid.NewGuid(),
                Name = "S",
                Address = "S",
                CompanyCode = "1",
                VATPayerCode = "1",
                CountryCode = "LT",
            };

            var client = new BaseClient
            {
                CountryCode = "DE",
                Name = "C",
                Address = "C",
                ClientType = Enums.ClientType.Company,
            };

            _mockCountriesService.Setup(cs => cs.IsCountryInEU(client.CountryCode)).Returns(true);
            _mockCountriesService.Setup(cs => cs.GetCountriesVAT(client.CountryCode)).ReturnsAsync(19);

            // Act
            var result = await _invoiceService.CalculatePrice(items, seller, client);

            // Assert
            Assert.Equal((250, 297.5m, 19), result);
        }

        [Fact]
        public async Task CalculatePrice_InvalidVAT_ThrowsException()
        {
            // Arrange
            var items = new List<InvoicePurchases>
            {
                new InvoicePurchases { Item = new Item { Id = Guid.NewGuid(), Name = "I1", Code = "I1", Price = 100 }, Amount = 2 },
                new InvoicePurchases { Item = new Item { Id = Guid.NewGuid(), Name = "I2", Code = "I2", Price = 50 }, Amount = 1 }
            };

            var seller = new Seller
            {
                VATPayer = true,
                Id = Guid.NewGuid(),
                Name = "S",
                Address = "S",
                CompanyCode = "1",
                VATPayerCode = "1",
                CountryCode = "LT",
            };

            var client = new BaseClient
            {
                CountryCode = "DE",
                Name = "C",
                Address = "C",
                ClientType = Enums.ClientType.Company,
            };

            _mockCountriesService.Setup(cs => cs.IsCountryInEU(client.CountryCode)).Returns(true);
            _mockCountriesService.Setup(cs => cs.GetCountriesVAT(client.CountryCode)).ReturnsAsync(-1);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () =>
                await _invoiceService.CalculatePrice(items, seller, client));

            Assert.Equal("Invalid VAT", exception.Message);
        }


        [Fact]
        public async Task CreateInvoice_InvalidItem_ThrowsException()
        {
            // Arrange
            var request = new CreateInvoiceDto
            {
                ItemPurchaseDtos = new List<ItemPurchaseDto>
                {
                    new ItemPurchaseDto { ItemId = Guid.NewGuid(), Amount = 1 }
                },
                SellerId = Guid.NewGuid(),
                BuyerId = Guid.NewGuid()
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await _invoiceService.CreateInvoice(request));
            Assert.Equal("Invalid item", exception.Message);
        }

        [Fact]
        public async Task CreateInvoice_InvalidItemAmount_ThrowsException()
        {
            // Arrange
            var dbContext = new DBContext(_dbContextOptions);
            var item = new Item { Id = Guid.NewGuid(), Name = "I1", Code = "I1", Price = 100 };
            dbContext.Items.Add(item);
            dbContext.SaveChanges();

            var request = new CreateInvoiceDto
            {
                ItemPurchaseDtos = new List<ItemPurchaseDto>
                {
                    new ItemPurchaseDto { ItemId = item.Id, Amount = 0 }
                },
                SellerId = Guid.NewGuid(),
                BuyerId = Guid.NewGuid()
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await _invoiceService.CreateInvoice(request));
            Assert.Equal("Invalid item amount", exception.Message);
        }

        [Fact]
        public async Task CreateInvoice_InvalidSeller_ThrowsException()
        {
            // Arrange
            var dbContext = new DBContext(_dbContextOptions);
            var item = new Item { Id = Guid.NewGuid(), Name = "I1", Code = "I1", Price = 100 };
            dbContext.Items.Add(item);
            dbContext.SaveChanges();

            var request = new CreateInvoiceDto
            {
                ItemPurchaseDtos = new List<ItemPurchaseDto>
                {
                    new ItemPurchaseDto { ItemId = item.Id, Amount = 1 }
                },
                SellerId = Guid.NewGuid(),
                BuyerId = Guid.NewGuid()
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await _invoiceService.CreateInvoice(request));
            Assert.Equal("Invalid seller", exception.Message);
        }

        [Fact]
        public async Task CreateInvoice_InvalidClient_ThrowsException()
        {
            // Arrange
            var dbContext = new DBContext(_dbContextOptions);
            var item = new Item { Id = Guid.NewGuid(), Name = "I1", Code = "I1", Price = 100 };
            dbContext.Items.Add(item);

            var seller = new Seller
            {
                VATPayer = true,
                Id = Guid.NewGuid(),
                Name = "S",
                Address = "S",
                CompanyCode = "1",
                VATPayerCode = "1",
                CountryCode = "LT",
            };

            dbContext.Sellers.Add(seller);
            dbContext.SaveChanges();

            var request = new CreateInvoiceDto
            {
                ItemPurchaseDtos = new List<ItemPurchaseDto>
                {
                    new ItemPurchaseDto { ItemId = item.Id, Amount = 1 }
                },
                SellerId = seller.Id,
                BuyerId = Guid.NewGuid()
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await _invoiceService.CreateInvoice(request));
            Assert.Equal("Invalid client", exception.Message);
        }

        [Fact]
        public async Task CreateInvoice_ValidInputs_ReturnsInvoice()
        {
            // Arrange
            var dbContext = new DBContext(_dbContextOptions);
            var item = new Item { Id = Guid.NewGuid(), Name = "I1", Code = "I1", Price = 100 };

            var seller = new Seller
            {
                VATPayer = true,
                Id = Guid.NewGuid(),
                Name = "S",
                Address = "S",
                CompanyCode = "1",
                VATPayerCode = "1",
                CountryCode = "LT",
            };

            var client = new BaseClient
            {
                CountryCode = "DE",
                Name = "C",
                Address = "C",
                ClientType = Enums.ClientType.Company,
            };

            dbContext.Items.Add(item);
            dbContext.Sellers.Add(seller);
            dbContext.BaseClients.Add(client);
            dbContext.SaveChanges();

            var request = new CreateInvoiceDto
            {
                ItemPurchaseDtos = new List<ItemPurchaseDto>
                {
                    new ItemPurchaseDto { ItemId = item.Id, Amount = 1 }
                },
                SellerId = seller.Id,
                BuyerId = client.Id
            };

            _mockCountriesService.Setup(cs => cs.IsCountryInEU(client.CountryCode)).Returns(true);
            _mockCountriesService.Setup(cs => cs.GetCountriesVAT(client.CountryCode)).ReturnsAsync(19);

            // Act
            var invoice = await _invoiceService.CreateInvoice(request);

            // Assert
            Assert.NotNull(invoice);
            Assert.Equal(1, invoice.Items.Count);
            Assert.Equal(item.Id, invoice.Items.First().Item.Id);
            Assert.Equal(100, invoice.TotalPricePreVAT);
            Assert.Equal(119, invoice.TotalPriceVAT);
            Assert.Equal(19, invoice.VAT);
        }
    }
}
