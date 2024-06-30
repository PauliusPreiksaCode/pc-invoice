using Invoice_generator.DTOs.Item;
using Invoice_generator.Interfaces;
using Invoice_generator.Services;
using Microsoft.AspNetCore.Mvc;

namespace Invoice_generator.Controllers
{
    [Route("Item")]
    public class ItemController(IItemService itemService) : Controller
    {
        private readonly IItemService _itemService = itemService;

        [HttpGet]
        [Route("list")]
        public async Task<IActionResult> GetAllItems()
        {
            try
            {
                var items = await _itemService.GetAllItems();
                return Ok(items);
            }
            catch (Exception ex)
            {
                return ex.Message switch
                {
                    _ => StatusCode(500, "Error occured while getting items")
                };
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetItemById([FromQuery] Guid id)
        {
            try
            {
                var item = _itemService.GetItemById(id);
                return Ok(item);
            }
            catch (Exception e)
            {
                return e.Message switch
                {
                    "Item not found" => NotFound("Item not found"),
                    _ => StatusCode(500, "Error occured while getting item")
                };
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddItem([FromBody] AddItemDto request)
        {
            try
            {
                await _itemService.AddItem(request);
                return Ok();
            }
            catch (Exception e)
            {
                return e.Message switch
                {
                    _ => StatusCode(500, "Error occured while adding item")
                };
            }
        }
    }
}
