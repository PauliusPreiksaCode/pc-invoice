namespace Invoice_generator.DTOs.Seller
{
    public class AddSellerDto
    {
        public required string Name { get; set; }

        public required string Address { get; set; }

        public bool VATPayer { get; set; }

        public required string CompanyCode { get; set; }

        public required string VATPayerCode { get; set; }

        public required string CountryCode { get; set; }
    }
}
