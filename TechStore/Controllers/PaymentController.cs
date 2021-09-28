using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PayPal.Api;

namespace TechStore.Controllers
{
    public class PaymentController : Controller
    {
        // GET: Payment
        public ActionResult PaymentWithPaypal()
        {
            APIContext apicntx = PaypalConfiguration.GetAPIContext();
            try
            {
                string PayerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(PayerId))
                {
                    string baseUri = Request.Url.Scheme + "://" + Request.Url.Authority + 
                        "PaymentWithPaypal/PaymentWithPaypal?";
                    var Guid = Convert.ToString((new Random()).Next(100000000));
                    var createPayment = this.CreatePayment(apicntx, baseUri + "guid="+ Guid);
                    var links = createPayment.links.GetEnumerator();
                    string paypalRedirectURL = null;
                    while (links.MoveNext())
                    {
                        Links lk = links.Current;
                        if (lk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            paypalRedirectURL = lk.href;
                        }
                    }
                }
                else
                {
                    var guid = Request.Params["guid"];
                    var exePayment = ExecutePayment(apicntx, PayerId, Session[guid] as string);
                    if (exePayment.ToString().ToLower() != "approved")
                    {
                        return View("FailureView");
                    }
                }
            }
            catch(Exception)
            {
                return View("FailureView");
            }
            return View("SuccessView");
        }

        private object ExecutePayment(APIContext apicntx, string payerId, string PaymentId)
        {
            var paymentExecution = new PaymentExecution() { payer_id=payerId};
            this.payment = new Payment() { id= PaymentId};
            return this.payment.Execute(apicntx, paymentExecution);
        }

        private PayPal.Api.Payment payment;
        private Payment CreatePayment(APIContext apicntx, string redirectURL)
        {
            var itemList = new ItemList() { items= new List<Item>()};
            if (Session["cart"] != "")
            {
                List<Models.Home.Item> cart = (List<Models.Home.Item>)(Session["cart"]);
                foreach (var item in cart)
                {
                    itemList.items.Add(new Item()
                    {
                        name = item.products.ProductName.ToString(),
                        currency = "CAD",
                        price = item.products.Price.ToString(),
                        quantity = item.products.Quantity.ToString(),
                        sku = "sku"
                    });
                }
                var payer = new Payer() { payment_method = "paypal" };
                var rediUrl = new RedirectUrls()
                {
                    cancel_url = redirectURL + "&Cancel=true",
                    return_url = redirectURL
                };
                var details = new Details()
                {
                    tax = "1",
                    shipping = "1",
                    subtotal = "1"
                };
                var amount = new Amount()
                {
                    currency = "CAD",
                    total = Session["SesTotal"].ToString(),
                    details = details
                };
                var transactionList = new List<Transaction>();
                transactionList.Add(new Transaction()
                {
                    description = "Tansaction Description",
                    invoice_number = "#100000",
                    amount = amount,
                    item_list= itemList
                });
                this.payment = new Payment()
                {
                    intent="sale",
                    payer=payer,
                    transactions=transactionList,
                    redirect_urls=rediUrl
                };
            }
            return this.payment.Create(apicntx);
        }
    }
}