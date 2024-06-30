using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Invoice_generator.Entities
{
    public class Invoice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public required Seller Seller { get; set; }
        public required BaseClient BaseClient { get; set; }
        public required List<InvoicePurchases> Items { get; set; }
        public decimal TotalPricePreVAT { get; set; }
        public decimal TotalPriceVAT { get;  set; }
        public int VAT { get; set; }
    }
}
