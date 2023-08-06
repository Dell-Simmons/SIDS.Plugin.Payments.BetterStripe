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
using SIDS.Plugin.Payments.BetterStripe.Components;
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

        public async Task<string> GetPaymentMethodDescriptionAsync() 
        { 
            return await _localizationService.GetResourceAsync("Plugins.Payments.BetterStripe.PaymentMethodDescription"); 
        }

        public Type GetPublicViewComponent() 
        {
            return typeof(PaymentInfoViewComponent);
        }

        public Task<bool> HidePaymentMethodAsync(IList<ShoppingCartItem> cart)
        { 
        
            //you can put any logic here
            //for example, hide this payment method if all products in the cart are downloadable
            //or hide this payment method if current customer is from certain country

            return Task.FromResult(false);
        }

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

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        //! HERE HERE HERE
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/BetterStripe/Configure";
        }
        /// <summary>
        /// Install plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {
            //settings
            await _settingService.SaveSettingAsync(new BetterStripePaymentSettings
            {
                UseSandbox = true
            });

            //locales
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
               ["Plugins.Payments.BetterStripe.Errors.3DSecureFailed"] = "The 3D Secure authentication is failed",
                ["Plugins.Payments.BetterStripe.Errors.ErrorProcessingPayment"] = "Error processing payment.",
                 ["Plugins.Payments.BetterStripe.Fields.PrivateKey.Hint"] = "Enter Secret key",
                ["Plugins.Payments.BetterStripe.Fields.SecretKey.Required"] = "Secret key is required",
                ["Plugins.Payments.BetterStripe.Fields.SecretKey"] = "Secret Key",
                ["Plugins.Payments.BetterStripe.Fields.PublicKey.Hint"] = "Enter Public key",
                ["Plugins.Payments.BetterStripe.Fields.PublicKey.Required"] = "Public key is required",
                ["Plugins.Payments.BetterStripe.Fields.PublicKey"] = "Public Key",
                ["Plugins.Payments.BetterStripe.Fields.Use3DS"] = "Use the 3D secure",
                ["Plugins.Payments.BetterStripe.Fields.Use3DS.Hint"] = "Check to enable the 3D secure integration",
                ["Plugins.Payments.BetterStripe.Fields.UseSandbox.Hint"] = "Check to enable Sandbox (testing environment).",
                ["Plugins.Payments.BetterStripe.Fields.UseSandbox"] = "Use Sandbox",
                ["Plugins.Payments.BetterStripe.PaymentMethodDescription"] = "Pay by credit / debit card"
            });

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {
            //settings
            await _settingService.DeleteSettingAsync<BetterStripePaymentSettings>();

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Payments.BetterStripe");

            await base.UninstallAsync();
        }
        #endregion
    }
}
