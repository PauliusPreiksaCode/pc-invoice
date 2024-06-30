
using Invoice_generator.Interfaces;
using Newtonsoft.Json.Linq;

namespace Invoice_generator.Services
{
    public class CountriesService(HttpClient httpClient) : ICountriesService
    {
        private readonly HttpClient _httpClient = httpClient;

        private static readonly HashSet<string> EU_COUNTRY_CODES = new HashSet<string>
        {
            "AT", // Austria
            "BE", // Belgium
            "BG", // Bulgaria
            "HR", // Croatia
            "CY", // Cyprus
            "CZ", // Czech Republic
            "DK", // Denmark
            "EE", // Estonia
            "FI", // Finland
            "FR", // France
            "DE", // Germany
            "GR", // Greece
            "HU", // Hungary
            "IE", // Ireland
            "IT", // Italy
            "LV", // Latvia
            "LT", // Lithuania
            "LU", // Luxembourg
            "MT", // Malta
            "NL", // Netherlands
            "PL", // Poland
            "PT", // Portugal
            "RO", // Romania
            "SK", // Slovakia
            "SI", // Slovenia
            "ES", // Spain
            "SE"  // Sweden
        };

        public async Task<List<string>> GetAllCountriesCodes()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync("https://api.first.org/data/v1/countries?limit=249");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                var jsonObject = JObject.Parse(responseBody);
                var data = jsonObject["data"] as JObject;

                if(data is null)
                {
                    throw new Exception("Country codes cannot be parsed");
                }

                List<string> codes = new List<string>();

                foreach (var property in data.Properties())
                {
                    codes.Add(property.Name);
                }

                return codes;
            }
            catch (Exception)
            {
                throw new Exception("Error occured while getting countries data");
            }
        }

        public bool IsCountryInEU(string countryCode)
        {
            if (countryCode is null)
                return false;

            countryCode = countryCode.ToUpper();

            return EU_COUNTRY_CODES.Contains(countryCode);
        }

        public async Task<int> GetCountriesVAT(string countryCode)
        {
            if(countryCode is null)
                throw new Exception("Country code is empty");

            countryCode = countryCode.ToUpper();

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"https://api.vatlookup.eu/rates/{countryCode}");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                var jsonObject = JObject.Parse(responseBody);
                var ratesArray = jsonObject["rates"] as JArray;

                if (ratesArray is null)
                    throw new Exception("No country found by this code");

                foreach (var rateObj in ratesArray)
                {
                    var rateName = rateObj["name"]?.ToString();
                    if (rateName == "Standard")
                    {
                        var standardRates = rateObj["rates"] as JArray;
                        if (standardRates != null && standardRates.Count > 0)
                        {
                            return (int)standardRates[0];
                        }
                    }
                }

                throw new Exception("No standard VAT found");
            }
            catch(Exception)
            {
                throw new Exception("Error occured while getting countries standard VAT");
            }
        }
    }
}
