using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Invoice_generator.Entities
{
    public class Seller
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public required string Address { get; set; }

        public bool VATPayer { get; set; }

        public required string CompanyCode { get; set; }

        public required string VATPayerCode { get; set; }

        public required string CountryCode { get; set; }
    }
}
