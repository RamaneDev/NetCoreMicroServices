using Catalog_API.Entities;
using MongoDB.Driver;

namespace Catalog_API.Data
{
    public interface ICatalogContext
    {
        IMongoCollection<Product> Products { get; }
    }
}
