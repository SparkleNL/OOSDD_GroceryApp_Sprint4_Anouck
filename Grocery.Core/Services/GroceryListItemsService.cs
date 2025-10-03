using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    public class GroceryListItemsService : IGroceryListItemsService
    {
        private readonly IGroceryListItemsRepository _groceriesRepository;
        private readonly IProductRepository _productRepository;

        public GroceryListItemsService(IGroceryListItemsRepository groceriesRepository, IProductRepository productRepository)
        {
            _groceriesRepository = groceriesRepository;
            _productRepository = productRepository;
        }

        public List<GroceryListItem> GetAll()
        {
            List<GroceryListItem> groceryListItems = _groceriesRepository.GetAll();
            FillService(groceryListItems);
            return groceryListItems;
        }

        
        public List<GroceryListItem> GetAllOnGroceryListId(int groceryListId)
        {
            List<GroceryListItem> groceryListItems = _groceriesRepository.GetAll().Where(g => g.GroceryListId == groceryListId).ToList();
            FillService(groceryListItems);
            return groceryListItems;
        }

        public GroceryListItem Add(GroceryListItem item)
        {
            return _groceriesRepository.Add(item);
        }

        public GroceryListItem? Delete(GroceryListItem item)
        {
            throw new NotImplementedException();
        }

        public GroceryListItem? Get(int id) 
        {
            return _groceriesRepository.Get(id);
        }

        public GroceryListItem? Update(GroceryListItem item)
        {
            return _groceriesRepository.Update(item);
        }

        //Make a ranking of bestselling products based on the amount sold
        public List<BestSellingProducts> GetBestSellingProducts(int topX = 5)
        {
            //Retrieve list of all grocery list items
            List<GroceryListItem> groceryListItems = _groceriesRepository.GetAll();
            List<BestSellingProducts> result = new List<BestSellingProducts>();

            var groupedItems = groceryListItems.GroupBy(item => item.ProductId).OrderByDescending(group => group.Sum(item => item.Amount)).Take(topX);
            int ranking = 0;

            //Add bestselling products to the result list
            foreach (var group in groupedItems)
            {
                ranking++;

                int productId = group.Key;
                int amountSold = group.Sum(item => item.Amount);
                
                Product? product = _productRepository.Get(productId);
                BestSellingProducts bestSeller = new BestSellingProducts(productId, product.Name, product.Stock, amountSold, ranking);
                result.Add(bestSeller);
            }
            return result;
        }

        private void FillService(List<GroceryListItem> groceryListItems)
        {
            foreach (GroceryListItem g in groceryListItems)
            {
                g.Product = _productRepository.Get(g.ProductId) ?? new(0, "", 0);
            }
        }
    }
}
