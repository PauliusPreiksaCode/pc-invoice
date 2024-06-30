namespace Invoice_generator.Entities
{
    public class Company : BaseClient
    {
        public required string CompanyCode { get; set; }

        public required string VATPayerCode { get; set; }
    }
}
