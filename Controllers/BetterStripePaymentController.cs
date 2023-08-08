using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Web.Framework.Controllers;
using Stripe;

namespace SIDS.Plugin.Payments.BetterStripe.Controllers
{
    //public class BetterStripePaymentController:Controller
    //{
        [Route("create-payment-intent")]
        [ApiController]
        public class PaymentIntentApiController : Controller
        {
            [HttpPost]
            public ActionResult Create(PaymentIntentCreateRequest request)
            {
            //"sk_test_1vkqbd6XNV4XEIvHfjwA9JNm";
            StripeConfiguration.ApiKey = "sk_test_1vkqbd6XNV4XEIvHfjwA9JNm";
            var paymentIntentService = new PaymentIntentService();
                var paymentIntent = paymentIntentService.Create(new PaymentIntentCreateOptions
                {
                    Amount = CalculateOrderAmount(request.Items),
                    Currency = "usd",
                    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                    {
                        Enabled = true,
                    },
                });

                return Json(new { clientSecret = paymentIntent.ClientSecret });
            }

            private int CalculateOrderAmount(Item[] items)
            {
                // Replace this constant with a calculation of the order's amount
                // Calculate the order total on the server to prevent
                // people from directly manipulating the amount on the client
                return 1400;
            }

            public class Item
            {
                [JsonProperty("id")]
                public string Id { get; set; }
            }

            public class PaymentIntentCreateRequest
            {
                [JsonProperty("items")]
                public Item[] Items { get; set; }
            }
        }
    //}
}
