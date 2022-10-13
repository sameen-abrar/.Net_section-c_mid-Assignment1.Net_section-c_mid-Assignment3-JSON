using labtask_cart.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace labtask_cart.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        [HttpGet]
        public ActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddProduct(product p)
        {
            var db = new TableOfProductsEntities1();
            db.products.Add(p);
            db.SaveChanges();
            return RedirectToAction("AddProduct");
        }

        [HttpGet]
        public ActionResult Show()
        {
            var db = new TableOfProductsEntities1();
            var plist = db.products.ToList();

            return View(plist);
        }

        [HttpGet]
        public ActionResult edit(int id)
        {
            var db = new TableOfProductsEntities1();
            var item = (from p in db.products
                        where p.id == id
                        select p).SingleOrDefault();

            return View(item);
        }

        [HttpPost]
        public ActionResult edit(product p)
        {
            var db = new TableOfProductsEntities1();
            var item = (from pr in db.products
                        where pr.id == p.id
                        select pr).SingleOrDefault();

            item.Name = p.Name;
            item.Price = p.Price;
            item.Qty = p.Qty;

            db.SaveChanges();

            return RedirectToAction("Show");
        }

        [HttpGet]
        public ActionResult deleteProduct(int id)
        {
            var db = new TableOfProductsEntities1();
            var item = (from pr in db.products
                        where pr.id == id
                        select pr).SingleOrDefault();

            db.products.Remove(item);
            db.SaveChanges();

            return RedirectToAction("Show");
        }

        [HttpGet]
        public ActionResult customerProduct()
        {
            var db = new TableOfProductsEntities1();
            var plist = db.products.ToList();

            return View(plist);
        }

        [HttpPost]
        public ActionResult customerProduct(product p)
        {
            var db = new TableOfProductsEntities1();
            var i = (from pr in db.products
                     where pr.id == p.id
                     select pr).SingleOrDefault();

            i.Qty = i.Qty - p.Qty;
            db.SaveChanges();

            if (Session["cart"] == null)
            {
                var list = new List<product>();

                list.Add(new product()
                {
                    id = p.id,
                    Name = i.Name,
                    Price = i.Price,
                    Qty = p.Qty
                });
                string json = new JavaScriptSerializer().Serialize(list);
                Session["cart"] = json;
            }
            else
            {
                var list = new JavaScriptSerializer().Deserialize<List<product>>(Session["cart"].ToString());

                list.Add(new product()
                {
                    id = p.id,
                    Name = i.Name,
                    Price = i.Price,
                    Qty = p.Qty
                });

                string json = new JavaScriptSerializer().Serialize(list);
                Session["cart"] = json;

            }

            return RedirectToAction("ViewCart");
        }

        [HttpGet]
        public ActionResult ViewCart()
        {
            if (Session["cart"] == null)
                return View();
            else
            {
                var list = new JavaScriptSerializer().Deserialize<List<product>>(Session["cart"].ToString());
                return View(list);
            }
        }

        public ActionResult confirmOrder()
        {
            var db = new TableOfProductsEntities1();

            if (Session["cart"] == null)
                return RedirectToAction("customerProduct");
            else
            {
                var list = new JavaScriptSerializer().Deserialize<List<product>>(Session["cart"].ToString());

                foreach (var i in list)
                {
                    db.orders.Add(new order()
                    {
                        Name = i.Name,
                        Qty = i.Qty,
                        totalPrice = (i.Price * i.Qty)
                    });
                    db.SaveChanges();

                }

                TempData["oc"] = "Order Confirmed";
                return RedirectToAction("showOrders");

            }
        }

        public ActionResult showOrders()
        {
            var db = new TableOfProductsEntities1();
            var list = db.orders.ToList();

            return View(list);
        }
    }
}