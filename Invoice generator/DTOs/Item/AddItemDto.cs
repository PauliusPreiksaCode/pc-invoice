namespace Invoice_generator.DTOs.Item
{
    public class AddItemDto
    {
        public required string Name { get; set; }

        public required string Code { get; set; }
        public decimal Price { get; set; } = decimal.Zero;
    }
}
