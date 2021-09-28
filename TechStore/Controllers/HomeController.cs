using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TechStore.DAL;
using TechStore.Models.Home;

namespace TechStore.Controllers
{
    public class HomeController : Controller
    {
        TechStoreEntities cntx = new TechStoreEntities();
        public ActionResult Index(string search, int? page)
        {
            HomeIndexViewModel model = new HomeIndexViewModel();
            return View(model.CreateModel(search, 4, page));
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Location()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult AddToCart(int productId)
        {
            if (Session["cart"] == null) { 
            List<Item> cart = new List<Item>();
            var pd = cntx.Product.Find(productId);
            cart.Add(new Item() {
                products = pd,
                Quantity = 1
            });
            Session["cart"] = cart;
        }
        else{
                List<Item> cart = (List<Item>)Session["cart"];
                var pd = cntx.Product.Find(productId);
                foreach(var item in cart)
                {
                    if (item.products.ProductId == productId)
                    {
                        int pqty = item.Quantity;
                        cart.Remove(item);
                        cart.Add(new Item()
                        {
                            products = pd,
                            Quantity = pqty+1
                        });                        
                    }
                    else
                    {
                        cart.Add(new Item()
                        {
                            products = pd,
                            Quantity = 1
                        });                        
                    }
                    break;
                }
                Session["cart"] = cart;
            }
            return Redirect("Index");
        } 

        public ActionResult RemoveFromCart(int productId)
        {
            List<Item> cart = (List<Item>)Session["cart"];
            foreach(var item in cart)
            {
                if (item.products.ProductId == productId)
                {
                    cart.Remove(item);
                    break;
                }
            }
            Session["cart"] = cart;

            return Redirect("Index");
        }

        public ActionResult Checkout()
        {
            return View();
        }

        public ActionResult CheckoutDetails()
        {
            return View();
        }

        public ActionResult DecreaseQty(int productId)
        {
            if(Session["cart"] != null)
            {
                List<Item> cart = (List<Item>)Session["cart"];
                var pd = cntx.Product.Find(productId);
                foreach(var item in cart)
                {
                    if (item.products.ProductId == productId)
                    {
                        int pqty = item.Quantity;
                        if(pqty > 0)
                        {
                            cart.Remove(item);
                            cart.Add(new Item()
                            {
                                products=pd,
                                Quantity=pqty-1
                            });
                        }
                        break;
                    }
                }
                Session["cart"] = cart;
            }
            return Redirect("Checkout");
        }
    }
}