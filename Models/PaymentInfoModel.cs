using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;

namespace SIDS.Plugin.Payments.BetterStripe.Models
{
    public class PaymentInfoModel
    {
        public string AccountCountryCode { get; set;}
        public int Amount { get; set;}
        public Address BillingAddress { get; set;}
        public Country BillingAddressCountry { get; set;}
        public StateProvince BillingAddressStateProvince { get; set;}
        public string CardHolderName { get; set;}
        public string Currency { get; set; }
        public IList<string> Items_Purchased { get; set;}
        public bool HidePostalCode { get; set;}
        public string PublishableKey { get; set;}
        public string StripeErrors { get; set;}
        public string StripePaymentIntentId { get; set; }
        public IList<string> Warnings { get; set; }

    }
}
