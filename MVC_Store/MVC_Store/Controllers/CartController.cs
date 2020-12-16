using MVC_Store.Models.Data;
using MVC_Store.Models.ViewModels.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_Store.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {
            List<CartVM> carts = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            if (carts.Count == 0 || Session["cart"] == null)
            {
                ViewBag.Message = "Your cart is empty";
                return View();
            }

            decimal total = 0m;
            total = carts.Sum(x => x.Total);

            ViewBag.GrandTotal = total;

            return View(carts);
        }

        // GET: CartPartial
        public ActionResult CartPartial()
        {
            CartVM cart = new CartVM();

            if (Session["cart"] != null)
            {
                var list = (List<CartVM>)Session["cart"];
                foreach (CartVM item in list)
                {
                    cart.Quantity += item.Quantity;
                    cart.Price += item.Quantity * item.Price;
                }
            }
            else
            {
                cart.Quantity = 0;
                cart.Price = 0m;
            }

            return PartialView("_CartPartial", cart);
        }

        // GET: AddToCartPartial
        public ActionResult AddToCartPartial(int id)
        {
            List<CartVM> carts = Session["cart"] as List<CartVM> ?? new List<CartVM>();
            CartVM model = new CartVM();

            using (Db db = new Db())
            {
                var product = db.Products.Find(id);

                var productInCart = carts.FirstOrDefault(x => x.ProductId == id);
                if (productInCart == null)
                {
                    carts.Add(new CartVM()
                    {
                        ProductId = product.Id,
                        Name = product.Name,
                        Quantity = 1,
                        Price = product.Price,
                        Image = product.ImageName
                    });
                }
                else
                {
                    productInCart.Quantity++;
                }
            }

            carts.ForEach((x) =>
            {
                model.Quantity += x.Quantity;
                model.Price += x.Quantity * x.Price;
            });


            Session["cart"] = carts;

            return PartialView("_AddToCartPartial", model);
        }


        // GET: Cart/IncrementProduct
        public JsonResult IncrementProduct(int productId)
        {
            List<CartVM> carts = Session["cart"] as List<CartVM>;

            // using (Db db = new Db())
            {
                CartVM model = carts.FirstOrDefault(x => x.ProductId == productId);
                model.Quantity++;

                var result = new { qty = model.Quantity, price = model.Price };
                // Session["cart"] = carts;

                // AddToCartPartial(model.ProductId);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Cart/DecrementProduct
        public ActionResult DecrementProduct(int productId)
        {
            List<CartVM> carts = Session["cart"] as List<CartVM>;

            CartVM model = carts.FirstOrDefault(x => x.ProductId == productId);
            if (model.Quantity > 1)
                model.Quantity--;
            else
            {
                model.Quantity = 0;
                carts.Remove(model);
            }

            var result = new { qty = model.Quantity, price = model.Price };

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        // GET: Cart/RemoveProduct
        public void RemoveProduct(int productId)
        {
            List<CartVM> carts = Session["cart"] as List<CartVM>;

            CartVM model = carts.FirstOrDefault(x => x.ProductId == productId);

            carts.Remove(model);
        }
    }
}