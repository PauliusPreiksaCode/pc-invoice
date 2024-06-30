namespace Invoice_generator.Interfaces
{
    public interface ICountriesService
    {
        Task<List<string>> GetAllCountriesCodes();

        bool IsCountryInEU(string countryCode);

        Task<int> GetCountriesVAT(string countryCode);
    }
}
