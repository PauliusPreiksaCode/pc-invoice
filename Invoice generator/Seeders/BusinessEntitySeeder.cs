using Invoice_generator.Entities;
using Invoice_generator.Enums;

namespace Invoice_generator.Seeders
{
    public class BusinessEntitySeeder
    {
        public static void Seed(DBContext context)
        {
            var Sellers = new List<Seller>
            {
                new Seller
                {
                    Id = Guid.Parse("0ef3d559-7499-4fa0-9154-507d4b2f4496"),
                    Name = "Seller1",
                    Address = "Studentu str. 50 Kaunas, Lithuania",
                    VATPayer = true,
                    CompanyCode = "31215601",
                    VATPayerCode = "165798913",
                    CountryCode = "LT"
                },
                new Seller
                {
                    Id = Guid.Parse("e1d6d52a-40ba-4288-9c89-192ebc3f0574"),
                    Name = "Seller2",
                    Address = "Ryga str. 70 Ryga, Latvia",
                    VATPayer = false,
                    CompanyCode = "123546",
                    VATPayerCode = "7896345",
                    CountryCode = "LV"
                }
            };

            var Companies = new List<Company>
            {
                new Company
                {
                    Id = Guid.Parse("1e4b9bd1-0512-4f66-a7ca-21ad2d5c6150"),
                    Name = "Company1",
                    Address = "Studentu str. 51 Kaunas, Lithuania",
                    ClientType = ClientType.Company,
                    CountryCode = "LT",
                    CompanyCode = "165467",
                    VATPayerCode = "12347",
                },
                new Company
                {
                    Id = Guid.Parse("0ea3cb31-586a-4e5b-a6ca-972fd40d0aeb"),
                    Name = "Company2",
                    Address = "Ryga str. 71 Ryga, Latvia",
                    ClientType = ClientType.Company,
                    CountryCode = "LV",
                    CompanyCode = "1657865467",
                    VATPayerCode = "129874347",
                },
            };

            var People = new List<Person>
            {
                new Person
                {
                    Id = Guid.Parse("76afba99-366d-4ec8-b2f9-ae6459c43311"),
                    Name = "Person1",
                    Address = "Studentu str. 52 Kaunas, Lithuania",
                    ClientType = ClientType.Person,
                    CountryCode = "LT",
                    Identification = "56179813214"
                },
                new Person
                {
                    Id = Guid.Parse("c47cdb95-1d08-40ce-b9ac-1ab7a86edcae"),
                    Name = "Person2",
                    Address = "Luanda str. 11 Luanda, Angola",
                    ClientType = ClientType.Person,
                    CountryCode = "AO",
                    Identification = "465132415646"
                },
            };

            context.Sellers.AddRange(Sellers);
            context.Companies.AddRange(Companies);
            context.People.AddRange(People);

            context.SaveChanges();
        }
    }
}
