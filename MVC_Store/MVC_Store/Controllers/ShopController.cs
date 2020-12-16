using MVC_Store.Models.Data;
using MVC_Store.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_Store.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Pages");
        }
        // GET: Shop/
        public ActionResult CategoryMenuPartial()
        {
            List<CategoryVM> categoryVMs;

            using (Db db = new Db())
            {
                categoryVMs = db.Categories.ToArray()
                            .OrderBy(x => x.Sorting)
                            .Select(x => new CategoryVM(x)).ToList();
            }

            return PartialView("_CategoryMenuPartial", categoryVMs);
        }

        // GET: Shop/Category/Name
        public ActionResult Category(string name)
        {
            List<ProductVM> productVMs;

            using (Db db = new Db())
            {
                var catDTO = db.Categories.FirstOrDefault(x => x.Slug == name);
                int catId = catDTO.Id;

                productVMs = db.Products.ToArray()
                    .Where(x => x.CategoryId == catId)
                    .Select(x => new ProductVM(x)).ToList();

                var productCat = db.Products.FirstOrDefault(x => x.CategoryId == catId);

                if (productCat == null)
                {
                    var catName = db.Categories
                            .Where(x => x.Slug == name)
                            .Select(x => x.Name)
                            .FirstOrDefault();

                    ViewBag.CategorieName = catName;
                }
                else
                {
                    ViewBag.CategorieName = productCat.Name ;
                }


                return View(productVMs);
            }


        }

        // GET: Shop/product-details/Name
        [ActionName("product-details")]
        public ActionResult ProductDetails(string name)
        {
            ProductDTO dTO;
            ProductVM model;

            int id=0;

            using (Db db = new Db())
            {
                if (!db.Products.Any(x => x.Slug.Equals(name)))
                    return RedirectToAction("Index", "Shop");

                dTO = db.Products.FirstOrDefault(x => x.Slug == name);

                id = dTO.Id;

                model = new ProductVM(dTO);
            }

            model.GaleryImages = Directory.EnumerateFiles(Server.MapPath($"~/Images/Uploads/Products/{id}/Gallery/Thumbs/"))
                    .Select(fn => Path.GetFileName(fn));


            return View("ProductDetails", model);
        }
    }
}