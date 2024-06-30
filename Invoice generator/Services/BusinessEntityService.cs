using Invoice_generator.DTOs.Client;
using Invoice_generator.DTOs.Seller;
using Invoice_generator.Entities;
using Invoice_generator.Enums;
using Invoice_generator.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Invoice_generator.Services
{
    public class BusinessEntityService(DBContext dbContext) : IBusinessEntityService
    {
        private readonly DBContext _dbContext = dbContext;

        public async Task AddSeller(AddSellerDto request)
        {
            var seller = new Seller
            {
                Name = request.Name,
                Address = request.Address,
                VATPayer = request.VATPayer,
                VATPayerCode = request.VATPayerCode,
                CompanyCode = request.CompanyCode,
                CountryCode = request.CountryCode,
            };

            await _dbContext.Sellers.AddAsync(seller);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Seller>> GetAllSellers()
        {
            return await _dbContext.Sellers.ToListAsync();
        }

        public Seller GetSellerById(Guid id)
        {
            var seller = _dbContext.Sellers.FirstOrDefault(x => x.Id.Equals(id));

            if (seller is null)
            {
                throw new Exception("Seller not found");
            }

            return seller;
        }

        public async Task AddCompany(AddCompanyDto request)
        {
            var company = new Company
            {
                Name = request.Name,
                Address = request.Address,
                ClientType = ClientType.Company,
                CountryCode = request.CountryCode,
                CompanyCode = request.CompanyCode,
                VATPayerCode = request.VATPayerCode,
            };

            await _dbContext.Companies.AddAsync(company);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Company>> GetAllCompanies()
        {
            return await _dbContext.Companies.ToListAsync();
        }

        public Company GetCompanyById(Guid id)
        {
            var company = _dbContext.Companies.FirstOrDefault(x => x.Id.Equals(id));

            if (company is null)
            {
                throw new Exception("Company not found");
            }

            return company;
        }

        public async Task AddPerson(AddPersonDto request)
        {
            var person = new Person
            {
                Name = request.Name,
                Address = request.Address,
                ClientType = ClientType.Person,
                CountryCode = request.CountryCode,
                Identification = request.Identification
            };

            await _dbContext.People.AddAsync(person);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Person>> GetAllPeople()
        {
            return await _dbContext.People.ToListAsync();
        }

        public Person GetPersonById(Guid id)
        {
            var person = _dbContext.People.FirstOrDefault(x => x.Id.Equals(id));

            if (person is null)
            {
                throw new Exception("Person not found");
            }

            return person;
        }
    }
}
