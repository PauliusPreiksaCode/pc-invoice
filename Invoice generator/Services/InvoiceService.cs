using Invoice_generator.DTOs.Invoice;
using Invoice_generator.Entities;
using Invoice_generator.Enums;
using Invoice_generator.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Invoice_generator.Services
{
    public class InvoiceService(DBContext dbContext, ICountriesService countriesService) : IInvoiceService
    {
        private readonly DBContext _dbContext = dbContext;
        private readonly ICountriesService _countriesService = countriesService;

        public async Task<FileResult> GenerateInvoice(CreateInvoiceDto request)
        {
            var invoice = await CreateInvoice(request);
            await CreateInvoicePDF(invoice);

            string filepath = $"Invoices/{invoice.Id}.pdf";

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filepath, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            var bytes = await System.IO.File.ReadAllBytesAsync(filepath);

            return new FileContentResult(bytes, contentType)
            {
                FileDownloadName = Path.GetFileName(filepath)
            };
        }

        public async Task<Invoice> CreateInvoice(CreateInvoiceDto request)
        {
            var itemCartList = new List<InvoicePurchases>();

            foreach (var x in request.ItemPurchaseDtos)
            {
                var item = await _dbContext.Items.FirstOrDefaultAsync(item => item.Id.Equals(x.ItemId));
                if (item is null)
                    throw new Exception("Invalid item");

                if (x.Amount <= 0)
                    throw new Exception("Invalid item amount");

                var invoicePurchase = new InvoicePurchases
                {
                    Id = Guid.NewGuid(),
                    Item = item,
                    Amount = x.Amount
                };

                itemCartList.Add(invoicePurchase);
            }

            var seller = await _dbContext.Sellers.FirstOrDefaultAsync(x => x.Id.Equals(request.SellerId));
            var client = await _dbContext.BaseClients.FirstOrDefaultAsync(x => x.Id.Equals(request.BuyerId));

            if (seller is null)
                throw new Exception("Invalid seller");
            if (client is null)
                throw new Exception("Invalid client");

            (decimal totalPricePreVAT, decimal totalPricePostVAT, int VAT) = await CalculatePrice(itemCartList, seller, client);

            var invoice = new Invoice
            {
                Id = Guid.NewGuid(),
                Date = DateTime.Now,
                Seller = seller,
                BaseClient = client,
                Items = new List<InvoicePurchases>(),
                TotalPricePreVAT = totalPricePreVAT,
                TotalPriceVAT = totalPricePostVAT,
                VAT = VAT,
            };

            invoice.Items.AddRange(itemCartList);
            var addedInvoice = await _dbContext.Invoices.AddAsync(invoice);
            await _dbContext.SaveChangesAsync();

            return addedInvoice.Entity;
        }

        public async Task<(decimal, decimal, int)> CalculatePrice(List<InvoicePurchases> items, Seller seller, BaseClient client)
        {
            decimal totalPricePreVAT = 0;

            items.ForEach(item => totalPricePreVAT += item.Item.Price * item.Amount);

            // seller is not VAT payer
            if (!seller.VATPayer)
                return (totalPricePreVAT, totalPricePreVAT, 0);

            // client does not live in EU country
            if(!_countriesService.IsCountryInEU(client.CountryCode))
                return (totalPricePreVAT, totalPricePreVAT, 0);

            /*
             * client and seller lives in different EU countries or in the same, we take clients VAT
             * it will be different it they live in different countries (client VAT priority)
             * it will be the same if they live int the same country
             */

            int VAT = await _countriesService.GetCountriesVAT(client.CountryCode);
            if (VAT < 0)
                throw new Exception("Invalid VAT");
            
            decimal VATInDecimal = 1 + (decimal)VAT / 100;
            decimal totalPricePostVAT = Math.Round(totalPricePreVAT * VATInDecimal, 2);
            return (totalPricePreVAT, totalPricePostVAT, VAT);
        }

        public async Task CreateInvoicePDF(Invoice invoice)
        {
            dynamic fullClient = invoice.BaseClient.ClientType.Equals(ClientType.Company)
                ? _dbContext.Companies.FirstOrDefault(x => x.Id.Equals(invoice.BaseClient.Id))
                : _dbContext.People.FirstOrDefault(x => x.Id.Equals(invoice.BaseClient.Id));

            Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.DefaultTextStyle(x => x.FontSize(12));

                        page.Header().Column(column =>
                        {
                            column.Item().Row(row =>
                            {
                                row.RelativeItem().Column(column =>
                                {
                                    column.Item().Text(text =>
                                    {
                                        text.Span("Invoice number: ").SemiBold();
                                        text.Span($"{invoice.Id}");
                                    });
                                    column.Item().Text(text =>
                                    {
                                        text.Span("Invoice issue date: ").SemiBold();
                                        text.Span($"{invoice.Date.ToShortDateString()}");
                                    });
                                });
                            });

                            column.Item().PaddingBottom(20);
                        });

                        page.Content().Column(column =>
                        {
                            
                            
                            column.Item().Row(row =>
                            {
                                row.RelativeItem().Column(column =>
                                {
                                    column.Item().Text("Seller:").SemiBold();
                                    column.Item().Text($"Name: {invoice.Seller.Name}");
                                    column.Item().Text($"Address: {invoice.Seller.Address}");
                                    column.Item().Text($"Company code: {invoice.Seller.CompanyCode}");
                                    column.Item().Text($"VAT payer code: {invoice.Seller.VATPayerCode}");
                                });
                            });

                            column.Item().PaddingBottom(20);
                            
                            column.Item().Row(row =>
                            {
                                row.RelativeItem().Column(column =>
                                {
                                    column.Item().Text("Client:").SemiBold();
                                    column.Item().Text($"Name: {invoice.BaseClient.Name}");
                                    column.Item().Text($"Address: {invoice.BaseClient.Address}");
                                    if (invoice.BaseClient.ClientType == ClientType.Company)
                                    {
                                        column.Item().Text($"Company code: {fullClient.CompanyCode}");
                                        column.Item().Text($"VAT payer code: {fullClient.VATPayerCode}");
                                    }
                                    else
                                    {
                                        column.Item().Text($"Person identification: {fullClient.Identification}");
                                    }
                                });
                            });
                            
                            column.Item().PaddingBottom(20);
                            
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(25);
                                    columns.ConstantColumn(100);
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyle).Text("#");
                                    header.Cell().Element(CellStyle).Text("Name");
                                    header.Cell().Element(CellStyle).Text("Code");
                                    header.Cell().Element(CellStyle).AlignRight().Text("Quantity");
                                    header.Cell().Element(CellStyle).AlignRight().Text("Price");
                                    header.Cell().Element(CellStyle).AlignRight().Text("Total");

                                    static IContainer CellStyle(IContainer container)
                                    {
                                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5)
                                            .BorderBottom(1)
                                            .BorderColor(Colors.Black);
                                    }
                                });

                                foreach (var item in invoice.Items)
                                {
                                    table.Cell().Element(CellStyle).Text((invoice.Items.IndexOf(item) + 1).ToString());
                                    table.Cell().Element(CellStyle).Text(item.Item.Name);
                                    table.Cell().Element(CellStyle).Text(item.Item.Code);
                                    table.Cell().Element(CellStyle).AlignRight().Text(item.Amount.ToString());
                                    table.Cell().Element(CellStyle).AlignRight().Text(item.Item.Price.ToString());
                                    table.Cell().Element(CellStyle).AlignRight()
                                        .Text($"{Math.Round(item.Amount * item.Item.Price, 2).ToString()}");

                                    static IContainer CellStyle(IContainer container)
                                    {
                                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                            .PaddingVertical(5);
                                    }
                                }
                            });
                            
                            column.Item().AlignRight().Text($"Total pre VAT: {invoice.TotalPricePreVAT} Eur");
                            column.Item().AlignRight().Text($"VAT: {invoice.VAT}%");
                            column.Item().AlignRight().Text($"VAT amount: {invoice.TotalPriceVAT - invoice.TotalPricePreVAT} Eur");
                            column.Item().AlignRight().Text($"Total post VAT: {invoice.TotalPriceVAT} Eur");
                        });
                    });
                })
                .GeneratePdf($"Invoices/{invoice.Id}.pdf");
        }
    }
}
