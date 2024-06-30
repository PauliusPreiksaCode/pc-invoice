using Invoice_generator.Entities;
using Invoice_generator.Enums;

namespace Invoice_generator.Seeders
{
    public class ItemSeeder
    {
        public static void Seed(DBContext context)
        {
            var Items = new List<Item>
            {
                new Item
                {
                    Id = Guid.Parse("9bf3c81f-f728-402a-9eaf-0f4a33865bce"),
                    Name = "Item1",
                    Code = "1ASD467",
                    Price = 21.15M
                },
                new Item
                {
                    Id = Guid.Parse("9cf0baa7-969e-4f3f-908f-4a99ddefe614"),
                    Name = "Item2",
                    Code = "AZ16579",
                    Price = 16.18M
                },
                new Item
                {
                    Id = Guid.Parse("583203d8-0e17-47ec-90ec-0165a954389c"),
                    Name = "Item3",
                    Code = "TASDASDSA",
                    Price = 1.16M
                },
                new Item
                {
                    Id = Guid.Parse("01f0b198-be82-4124-96e5-17db60dfdad8"),
                    Name = "Item4",
                    Code = "QERS1567",
                    Price = 7.81M
                },
            };

            context.Items.AddRange(Items);
            context.SaveChanges();
        }
    }
}
