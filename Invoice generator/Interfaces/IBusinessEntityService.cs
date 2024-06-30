using Invoice_generator.DTOs.Client;
using Invoice_generator.DTOs.Seller;
using Invoice_generator.Entities;

namespace Invoice_generator.Interfaces
{
    public interface IBusinessEntityService
    {
        Task AddSeller(AddSellerDto request);
        Task<List<Seller>> GetAllSellers();
        Seller GetSellerById(Guid id);

        Task AddCompany(AddCompanyDto request);
        Task<List<Company>> GetAllCompanies();
        Company GetCompanyById(Guid id);

        Task AddPerson(AddPersonDto request);
        Task<List<Person>> GetAllPeople();
        Person GetPersonById(Guid id);
    }
}
