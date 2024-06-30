using Invoice_generator.Enums;

namespace Invoice_generator.DTOs.Client
{
    public class AddCompanyDto
    {
        public required string Name { get; set; }

        public required string Address { get; set; }

        public required string CountryCode { get; set; }

        public required string CompanyCode { get; set; }

        public required string VATPayerCode { get; set; }
    }
}
