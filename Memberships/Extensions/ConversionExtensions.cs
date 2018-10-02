using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Threading.Tasks;
using Memberships.Entities;
using Memberships.Models;
using Memberships.Areas.Admin.Models;

namespace Memberships.Extensions
{
    public static class ConversionExtensions
    {
        public static async Task<IEnumerable<ProductModel>> Convert(this IEnumerable<Product> products, ApplicationDbContext db)
        {
            if(products.Count().Equals(0))
            {
                return new List<ProductModel>();
            }

            var texts = await db.ProductLinkTexts.ToListAsync();
            var types = await db.ProductTypes.ToListAsync();

            return from p in products
                   select new ProductModel
                   {
                       Id = p.Id,
                       Title = p.Title,
                       Description = p.Description,
                       ImageUrl = p.ImageUrl,
                       ProductLinkTextId = p.ProductLinkTextId,
                       ProductLinkTexts = texts,
                       ProductTypeId = p.ProductTypeId,
                       ProductTypes = types
                   };
        }

        public static async Task<ProductModel> Convert(this Product product, ApplicationDbContext db)
        {
            var text = await db.ProductLinkTexts.FirstOrDefaultAsync(plt => plt.Id.Equals(product.ProductLinkTextId));
            var type = await db.ProductTypes.FirstOrDefaultAsync(pt => pt.Id.Equals(product.ProductTypeId));
            var texts = await db.ProductLinkTexts.ToListAsync();
            var types = await db.ProductTypes.ToListAsync();
            var model = new ProductModel
            {
                Id = product.Id,
                Title = product.Title,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                ProductLinkTextId = product.ProductLinkTextId,
                ProductTypeId = product.ProductTypeId,
                ProductLinkTexts = texts,
                ProductTypes = types
            };

            //model.ProductTypes.Add(type);
            //model.ProductLinkTexts.Add(text);

            return model;
        }
    }
}