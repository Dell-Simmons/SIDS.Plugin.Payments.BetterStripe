using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIDS.Plugin.Payments.BetterStripe
{
    /// <summary>
    /// Rename this file and change to the correct type
    /// </summary>
    public class BetterStripePaymentProcessor : BasePlugin, IPaymentMethod
    {
        private readonly BetterStripePaymentSettings _betterStripePaymentSettings;
        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;

        public BetterStripePaymentProcessor(BetterStripePaymentSettings betterStripePaymentSettings, 
                                            IAddressService addressService,
                                            ICountryService countryService,
                                            ICurrencyService currencyService,
                                            ICustomerService customerService,
                                            ILocalizationService localizationService,
                                            IOrderTotalCalculationService orderTotalCalculationService,
                                            ISettingService settingService,
                                            IWebHelper webHelper,
                                            IWorkContext workContext)
        {
            _betterStripePaymentSettings = betterStripePaymentSettings;
            _addressService = addressService;
            _countryService = countryService;
            _currencyService = currencyService;
            _customerService = customerService;
            _localizationService = localizationService;
            _orderTotalCalculationService = orderTotalCalculationService;
            _settingService = settingService;
            _webHelper = webHelper;
            _workContext = workContext;
        }
        #region Properties
        public PaymentMethodType PaymentMethodType => PaymentMethodType.Standard;

        public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.NotSupported;

        public bool SkipPaymentInfo => false;

        public bool SupportCapture => false;

        public bool SupportPartiallyRefund => false;

        public bool SupportRefund => false;

        public bool SupportVoid => false;
        #endregion

        #region Methods
        public Task<CancelRecurringPaymentResult> CancelRecurringPaymentAsync(
            CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            var result = new CancelRecurringPaymentResult();
            result.AddError("Recurring payment not supported");

            return Task.FromResult(result);
        }

        public Task<bool> CanRePostProcessPaymentAsync(Order order) { return Task.FromResult(false); }

        public Task<CapturePaymentResult> CaptureAsync(CapturePaymentRequest capturePaymentRequest)
        {
            var result = new CapturePaymentResult();
            result.AddError("Capture method not supported");

            return Task.FromResult(result);
        }

        public Task<decimal> GetAdditionalHandlingFeeAsync(IList<ShoppingCartItem> cart)
        {
            var result = 0m;
            return Task.FromResult(result);
        }

        public Task<ProcessPaymentRequest> GetPaymentInfoAsync(IFormCollection form)
        { throw new NotImplementedException(); }

        public Task<string> GetPaymentMethodDescriptionAsync() { throw new NotImplementedException(); }

        public Type GetPublicViewComponent() { throw new NotImplementedException(); }

        public Task<bool> HidePaymentMethodAsync(IList<ShoppingCartItem> cart) { throw new NotImplementedException(); }

        public Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest)
        { throw new NotImplementedException(); }

        public Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        { throw new NotImplementedException(); }

        public Task<ProcessPaymentResult> ProcessRecurringPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        { throw new NotImplementedException(); }

        public Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest refundPaymentRequest)
        { throw new NotImplementedException(); }

        public Task<IList<string>> ValidatePaymentFormAsync(IFormCollection form)
        { throw new NotImplementedException(); }

        public Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest voidPaymentRequest)
        { throw new NotImplementedException(); }
        #endregion
    }
}
