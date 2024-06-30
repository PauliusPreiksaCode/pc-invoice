using Invoice_generator.Interfaces;
using Invoice_generator.Services;
using Microsoft.AspNetCore.Mvc;

namespace Invoice_generator.Controllers
{
    [Route("Country")]
    public class CountriesController(ICountriesService contriesService) : Controller
    {
        private readonly ICountriesService _contriesService = contriesService;

        [HttpGet]
        [Route("Codes")]
        public async Task<IActionResult> GetAllCountriesCodes()
        {
            try
            {
                var codes = await _contriesService.GetAllCountriesCodes();
                return Ok(codes);
            }
            catch (Exception ex)
            {
                return ex.Message switch
                {
                    "Country codes cannot be parsed" => BadRequest("Country codes cannot be parsed"),
                    _ => StatusCode(500, "Error occured while getting countries data")
                };
            }
            
        }
    }
}
