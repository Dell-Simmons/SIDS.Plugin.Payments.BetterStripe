using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Orders;
using SIDS.Plugin.Payments.BetterStripe.Models;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Web.Framework.Components;

namespace SIDS.Plugin.Payments.BetterStripe.Components
{
    /// <summary>
    /// Represents the view component to display payment info in public store
    /// </summary>
    [ViewComponent(Name = "BetterStripePaymentInfo")]
    public class PaymentInfoViewComponent : NopViewComponent
    {
        #region Fields

        private readonly BetterStripePaymentSettings _betterStripePaymentSettings;
        private readonly INotificationService _notificationService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly OrderSettings _orderSettings;

        #endregion

        #region Ctor

        public PaymentInfoViewComponent(BetterStripePaymentSettings betterStripePaymentSettings,
            INotificationService notificationService,
            IOrderTotalCalculationService orderTotalCalculationService,
            IShoppingCartService shoppingCartService,
            IStoreContext storeContext,
            IWorkContext workContext,
            OrderSettings orderSettings)
        {
            _betterStripePaymentSettings = betterStripePaymentSettings;
            _notificationService = notificationService;
            _orderTotalCalculationService = orderTotalCalculationService;
            _shoppingCartService = shoppingCartService;
            _storeContext = storeContext;
            _workContext = workContext;
            _orderSettings = orderSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke view component
        /// </summary>
        /// <param name="widgetZone">Widget zone name</param>
        /// <param name="additionalData">Additional data</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the view component result
        /// </returns>
        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            var model = new PaymentInfoModel();



            //for (var i = 0; i < 15; i++)
            //{
            //    var year = Convert.ToString(DateTime.Now.Year + i);
            //    model.ExpireYears.Add(new SelectListItem { Text = year, Value = year, });
            //}

            //for (var i = 1; i <= 12; i++)
            //{
            //    var text = (i < 10) ? "0" + i : i.ToString();
            //    model.ExpireMonths.Add(new SelectListItem { Text = text, Value = i.ToString(), });
            //}

            return View("~/Plugins/Payments.BetterStripe/Views/PaymentInfo.cshtml", model);
        }

        #endregion
    }
}