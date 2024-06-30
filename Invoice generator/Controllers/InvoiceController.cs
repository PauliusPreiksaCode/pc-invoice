using Invoice_generator.DTOs.Invoice;
using Invoice_generator.Interfaces;
using Invoice_generator.Services;
using Microsoft.AspNetCore.Mvc;

namespace Invoice_generator.Controllers
{
    [Route("Invoice")]
    public class InvoiceController(IInvoiceService invoiceService) : Controller
    {
        private readonly IInvoiceService _invoiceService = invoiceService;

        [HttpPost]
        [Route("Generate")]
        public async Task<IActionResult> GenerateInvoice([FromBody] CreateInvoiceDto request)
        {
            try
            {
                var fileResult = await _invoiceService.GenerateInvoice(request);
                return fileResult;
            }
            catch (Exception ex)
            {
                return ex.Message switch
                {
                    "Invalid item" => BadRequest("Invalid item"),
                    "Invalid item amount" => BadRequest("Invalid item amount"),
                    "Invalid seller" => BadRequest("Invalid seller"),
                    "Invalid client" => BadRequest("Invalid client"),
                    "Unable to create invoice" => StatusCode(500, "Unable to create invoice"),
                    "Invalid VAT" => StatusCode(500, "Invalid VAT"),
                    "Country code is empty" => BadRequest("Country code is empty"),
                    "No country found by this code" => NotFound("No country found by this code"),
                    "No standard VAT found" => NotFound("No standard VAT found"),
                    "Error occured while getting countries standard VAT" => StatusCode(500, "Error occured while getting countries standard VAT"),
                    _ => StatusCode(500, "Unable to create invoice")
                };
            }
            
        }
    }
}
