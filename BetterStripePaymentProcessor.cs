using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Nop.Core.Domain.Orders;
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
            var result = await _orderTotalCalculationService.CalculatePaymentAdditionalFeeAsync(cart,
       _braintreePaymentSettings.AdditionalFee, _braintreePaymentSettings.AdditionalFeePercentage);

            return result;
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
