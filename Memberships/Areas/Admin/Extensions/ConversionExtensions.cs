using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Threading.Tasks;
using Memberships.Entities;
using Memberships.Models;
using Memberships.Areas.Admin.Models;
using System.Transactions;

namespace Memberships.Areas.Admin.Extensions
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

        public static async Task<List<ProductItemModel>> Convert(this IQueryable<ProductItem> productItems, ApplicationDbContext db)
        {
            if(productItems.Count().Equals(0))
            {
                return new List<ProductItemModel>();
            }

            var model = await (
                from pi in db.ProductItems
                select new ProductItemModel
                {
                    ItemId = pi.ItemId,
                    ProductId = pi.ProductId,
                    ItemTitle = db.Items.FirstOrDefault(i => i.Id == pi.ItemId).Title,
                    ProductTitle = db.Products.FirstOrDefault(p => p.Id == pi.ProductId).Title
                }).ToListAsync();
            return model;
        }

        public static async Task<ProductItemModel> Convert(this ProductItem productItem, ApplicationDbContext db, bool addListData = true)
        {
            var model = new ProductItemModel
            {
                ItemId = productItem.ItemId,
                ProductId = productItem.ProductId,
                Items = addListData ? await db.Items.ToListAsync(): null,
                Products = addListData ? await db.Products.ToListAsync() : null, 
                ItemTitle = (await db.Items.FirstOrDefaultAsync(i => i.Id.Equals(productItem.ItemId))).Title,
                ProductTitle = (await db.Products.FirstOrDefaultAsync(p => p.Id.Equals(productItem.ProductId))).Title
            };

            return model;
        }

        public static async Task<bool> CanChange(this ProductItem productItem, ApplicationDbContext db)
        {
            var oldPI = await db.ProductItems.CountAsync(pi => pi.ItemId.Equals(productItem.OldItemId) && pi.ProductId.Equals(productItem.OldProductId));
            var newPI = await db.ProductItems.CountAsync(pi => pi.ItemId.Equals(productItem.ItemId) && pi.ProductId.Equals(productItem.ProductId));

            return oldPI.Equals(1) && newPI.Equals(0);
        }

        public static async Task Change(this ProductItem productItem, ApplicationDbContext db)
        {
            var oldProductItem = await db.ProductItems.FirstOrDefaultAsync(pi => pi.ItemId.Equals(productItem.OldItemId) && pi.ProductId.Equals(productItem.OldProductId));
            var newProductItem = await db.ProductItems.FirstOrDefaultAsync(pi => pi.ItemId.Equals(productItem.ItemId) && pi.ProductId.Equals(productItem.ProductId));
            if(oldProductItem != null && newProductItem == null)
            {
                newProductItem = new ProductItem
                {
                    ItemId = productItem.ItemId,
                    ProductId = productItem.ProductId
                };

                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled)) {
                    try
                    {
                        db.ProductItems.Remove(oldProductItem);
                        db.ProductItems.Add(newProductItem);
                        await db.SaveChangesAsync();
                        transaction.Complete();
                    } catch
                    {
                        transaction.Dispose();
                    }
                }
                
            }
        }

        public static async Task<List<SubscriptionProductModel>> Convert(this IQueryable<SubscriptionProduct> subscriptionProducts, ApplicationDbContext db)
        {
            if (subscriptionProducts.Count().Equals(0))
            {
                return new List<SubscriptionProductModel>();
            }

            var model = await (
                from pi in db.SubscriptionProducts
                select new SubscriptionProductModel
                {
                    SubscriptionId = pi.SubscriptionId,
                    ProductId = pi.ProductId,
                    SubscriptionTitle = db.Subscriptions.FirstOrDefault(i => i.Id == pi.SubscriptionId).Title,
                    ProductTitle = db.Products.FirstOrDefault(p => p.Id == pi.ProductId).Title
                }).ToListAsync();
            return model;
        }

        public static async Task<SubscriptionProductModel> Convert(this SubscriptionProduct productItem, ApplicationDbContext db, bool addListData = true)
        {
            var model = new SubscriptionProductModel
            {
                SubscriptionId = productItem.SubscriptionId,
                ProductId = productItem.ProductId,
                Subscriptions = addListData ? await db.Subscriptions.ToListAsync() : null,
                Products = addListData ? await db.Products.ToListAsync() : null,
                SubscriptionTitle = (await db.Subscriptions.FirstOrDefaultAsync(i => i.Id.Equals(productItem.SubscriptionId))).Title,
                ProductTitle = (await db.Products.FirstOrDefaultAsync(p => p.Id.Equals(productItem.ProductId))).Title
            };

            return model;
        }

        public static async Task<bool> CanChange(this SubscriptionProduct subscriptionProduct, ApplicationDbContext db)
        {
            var oldPI = await db.SubscriptionProducts.CountAsync(sp => sp.SubscriptionId.Equals(subscriptionProduct.OldSubscriptionId) && sp.ProductId.Equals(subscriptionProduct.OldProductId));
            var newPI = await db.SubscriptionProducts.CountAsync(sp => sp.SubscriptionId.Equals(subscriptionProduct.SubscriptionId) && sp.ProductId.Equals(subscriptionProduct.ProductId));

            return oldPI.Equals(1) && newPI.Equals(0);
        }

        public static async Task Change(this SubscriptionProduct subscriptionProduct, ApplicationDbContext db)
        {
            var oldProductItem = await db.SubscriptionProducts.FirstOrDefaultAsync(sp => sp.SubscriptionId.Equals(subscriptionProduct.OldSubscriptionId) && sp.ProductId.Equals(subscriptionProduct.OldProductId));
            var newProductItem = await db.SubscriptionProducts.FirstOrDefaultAsync(sp => sp.SubscriptionId.Equals(subscriptionProduct.SubscriptionId) && sp.ProductId.Equals(subscriptionProduct.ProductId));
            if (oldProductItem != null && newProductItem == null)
            {
                newProductItem = new SubscriptionProduct
                {
                    SubscriptionId = subscriptionProduct.SubscriptionId,
                    ProductId = subscriptionProduct.ProductId
                };

                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        db.SubscriptionProducts.Remove(oldProductItem);
                        db.SubscriptionProducts.Add(newProductItem);
                        await db.SaveChangesAsync();
                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                    }
                }

            }
        }
    }
}