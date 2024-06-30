using Invoice_generator.DTOs.Item;
using Invoice_generator.Entities;
using Invoice_generator.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Invoice_generator.Services
{
    public class ItemService(DBContext dbContext) : IItemService
    {
        private readonly DBContext _dbContext = dbContext;

        public async Task<List<Item>> GetAllItems()
        {
            return await _dbContext.Items.ToListAsync();
        } 

        public Item GetItemById(Guid id) 
        {
            var item = _dbContext.Items.FirstOrDefault(x => x.Id.Equals(id));

            if(item is null)
            {
                throw new Exception("Item not found");
            }

            return item;
        }

        public async Task AddItem(AddItemDto request)
        {
            var item = new Item
            {
                Name = request.Name,
                Code = request.Code,
                Price = request.Price
            };

            await _dbContext.Items.AddAsync(item);
            await _dbContext.SaveChangesAsync();
        }
    }
}
