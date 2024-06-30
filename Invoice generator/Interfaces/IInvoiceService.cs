using Invoice_generator.DTOs.Invoice;
using Invoice_generator.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Invoice_generator.Interfaces
{
    public interface IInvoiceService
    {
        Task<FileResult> GenerateInvoice(CreateInvoiceDto request);
        Task<Invoice> CreateInvoice(CreateInvoiceDto request);
        Task<(decimal, decimal, int)> CalculatePrice(List<InvoicePurchases> items, Seller seller, BaseClient client);
        Task CreateInvoicePDF(Invoice invoice);
    }
}
