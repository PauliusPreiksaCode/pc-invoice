using Invoice_generator.DTOs.Client;
using Invoice_generator.DTOs.Seller;
using Invoice_generator.Interfaces;
using Invoice_generator.Services;
using Microsoft.AspNetCore.Mvc;

namespace Invoice_generator.Controllers
{
    [Route("BusinessEntity")]
    public class BusinessEntityController(IBusinessEntityService businessEntityService) : Controller
    {
        private readonly IBusinessEntityService _businessEntityService = businessEntityService;

        [HttpGet]
        [Route("SellersList")]
        public async Task<IActionResult> GetAllSellers()
        {
            try
            {
                var sellers = await _businessEntityService.GetAllSellers();
                return Ok(sellers);
            }
            catch (Exception ex)
            {
                return ex.Message switch
                {
                    _ => StatusCode(500, "Error occured while getting sellers")
                };
            }
        }

        [HttpGet]
        [Route("Seller")]
        public async Task<IActionResult> GetSellerById([FromQuery] Guid id)
        {
            try
            {
                var seller = _businessEntityService.GetSellerById(id);
                return Ok(seller);
            }
            catch (Exception e)
            {
                return e.Message switch
                {
                    "Seller not found" => NotFound("Seller not found"),
                    _ => StatusCode(500, "Error occured while getting seller")
                };
            }
        }

        [HttpPost]
        [Route("Seller")]
        public async Task<IActionResult> AddSeller([FromBody] AddSellerDto request)
        {
            try
            {
                await _businessEntityService.AddSeller(request);
                return Ok();
            }
            catch (Exception e)
            {
                return e.Message switch
                {
                    _ => StatusCode(500, "Error occured while adding seller")
                };
            }
        }

        [HttpGet]
        [Route("CompaniesList")]
        public async Task<IActionResult> GetAllCompanies()
        {
            try
            {
                var companies = await _businessEntityService.GetAllCompanies();
                return Ok(companies);
            }
            catch (Exception ex)
            {
                return ex.Message switch
                {
                    _ => StatusCode(500, "Error occured while getting companies")
                };
            }
        }

        [HttpGet]
        [Route("Company")]
        public async Task<IActionResult> GetCompanyById([FromQuery] Guid id)
        {
            try
            {
                var company = _businessEntityService.GetCompanyById(id);
                return Ok(company);
            }
            catch (Exception e)
            {
                return e.Message switch
                {
                    "Company not found" => NotFound("Company not found"),
                    _ => StatusCode(500, "Error occured while getting company")
                };
            }
        }

        [HttpPost]
        [Route("Company")]
        public async Task<IActionResult> AddCompany([FromBody] AddCompanyDto request)
        {
            try
            {
                await _businessEntityService.AddCompany(request);
                return Ok();
            }
            catch (Exception e)
            {
                return e.Message switch
                {
                    _ => StatusCode(500, "Error occured while adding company")
                };
            }
        }

        [HttpGet]
        [Route("PeopleList")]
        public async Task<IActionResult> GetAllPeople()
        {
            try
            {
                var people = await _businessEntityService.GetAllPeople();
                return Ok(people);
            }
            catch (Exception ex)
            {
                return ex.Message switch
                {
                    _ => StatusCode(500, "Error occured while getting people")
                };
            }
        }

        [HttpGet]
        [Route("Person")]
        public async Task<IActionResult> GetPersonById([FromQuery] Guid id)
        {
            try
            {
                var person = _businessEntityService.GetPersonById(id);
                return Ok(person);
            }
            catch (Exception e)
            {
                return e.Message switch
                {
                    "Person not found" => NotFound("Person not found"),
                    _ => StatusCode(500, "Error occured while getting person")
                };
            }
        }

        [HttpPost]
        [Route("Person")]
        public async Task<IActionResult> AddPerson([FromBody] AddPersonDto request)
        {
            try
            {
                await _businessEntityService.AddPerson(request);
                return Ok();
            }
            catch (Exception e)
            {
                return e.Message switch
                {
                    _ => StatusCode(500, "Error occured while adding person")
                };
            }
        }
    }
}
