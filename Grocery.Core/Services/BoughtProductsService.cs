
using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    public class BoughtProductsService : IBoughtProductsService
    {
        private readonly IGroceryListItemsRepository _groceryListItemsRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IProductRepository _productRepository;
        private readonly IGroceryListRepository _groceryListRepository;
        public BoughtProductsService(IGroceryListItemsRepository groceryListItemsRepository, IGroceryListRepository groceryListRepository, IClientRepository clientRepository, IProductRepository productRepository)
        {
            _groceryListItemsRepository=groceryListItemsRepository;
            _groceryListRepository=groceryListRepository;
            _clientRepository=clientRepository;
            _productRepository=productRepository;
        }
        public List<BoughtProducts> Get(int? productId)
        {
            List<GroceryListItem> groceryListItems = _groceryListItemsRepository.GetAll();
            List<BoughtProducts> boughtProducts = new();

            //Clients die product met productid hebben gekocht met client, boodschappenlijst en product in de lijst staan die wordt geretourneerd.
            foreach (var item in groceryListItems)
            {
                if (item.ProductId == productId)
                {
                    var groceryList = _groceryListRepository.Get(item.GroceryListId);
                    if (groceryList != null)
                    {
                        var client = _clientRepository.Get(groceryList.ClientId);
                        var product = _productRepository.Get(item.ProductId);
                        if (client != null && product != null)
                        {
                            boughtProducts.Add(new BoughtProducts(client, groceryList, product));
                        }
                    }
                }
            }
            return boughtProducts;
        }
    }
}
