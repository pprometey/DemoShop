using DemoShop.Core.Domain;
using DemoShop.Core.Infrastructure;

namespace DemoShop.Core.Services
{
    public class GoodsDbRepositories : IGoodsDbRepositories
    {
        private readonly IGoodsDbContext _context;

        private IDbRepository<Unit> _units;
        private IDbRepository<ProductsCategory> _productsCategories;
        private IDbRepository<Product> _products;

        public GoodsDbRepositories(IGoodsDbContext context)
        {
            _context = context;
        }

        public IDbRepository<Unit> Units
        {
            get
            {
                if (_units == null)
                    _units = new UnitsRepository(_context);
                return _units;
            }
        }

        public IDbRepository<ProductsCategory> ProductsCategories
        {
            get
            {
                if (_productsCategories == null)
                    _productsCategories = new ProductsCategoriesRepository(_context);
                return _productsCategories;
            }
        }

        public IDbRepository<Product> Products
        {
            get
            {
                if (_products == null)
                    _products = new ProductsRepository(_context);
                return _products;
            }
        }
    }
}