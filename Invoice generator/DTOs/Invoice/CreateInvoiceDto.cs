using Invoice_generator.DTOs.Item;

namespace Invoice_generator.DTOs.Invoice
{
    public class CreateInvoiceDto
    {
        public Guid BuyerId { get; set; }

        public Guid SellerId { get; set; }

        public required List<ItemPurchaseDto> ItemPurchaseDtos { get; set; }
    }
}
