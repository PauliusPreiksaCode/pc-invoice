using Invoice_generator.DTOs.Item;
using Invoice_generator.Entities;

namespace Invoice_generator.Interfaces
{
    public interface IItemService
    {
        Task<List<Item>> GetAllItems();

        public Item GetItemById(Guid id);

        Task AddItem(AddItemDto request);
    }
}
