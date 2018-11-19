using Memberships.Extensions;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Threading.Tasks;
using Memberships.Models;
using System.Linq;
using System;

namespace Memberships.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            var userId = Request.IsAuthenticated ? HttpContext.User.Identity.GetUserId() : null;
            var thumbnails = await new List<ThumbnailModel>().GetProductThumbnailsAsync(userId);
            var count = thumbnails.Count()/4;
            var model = new List<ThumbnailAreaModel>();
            for(int i = 0; i <= count; i++)
            {
                model.Add(new ThumbnailAreaModel
                {
                    Title = i.Equals(0) ? "My Content" : String.Empty,
                    Thumbnails = thumbnails.Skip(i * 4).Take(4)
                });
            }

            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}