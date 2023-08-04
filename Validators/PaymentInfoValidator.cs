using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using SIDS.Plugin.Payments.BetterStripe.Models;

namespace SIDS.Plugin.Payments.BetterStripe.Validators
{
    /// <summary>
    /// Represents payment info model validator
    /// </summary>
    public class PaymentInfoValidator : BaseNopValidator<PaymentInfoModel>
    {
        #region Ctor

        public PaymentInfoValidator(BetterStripePaymentSettings betterStripePaymentSettings, ILocalizationService localizationService)
        {
            if (betterStripePaymentSettings.Use3DS)
                return;

            RuleFor(model => model.CardHolderName)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Payment.CardholderName.Required"));

            //RuleFor(model => model.CardNumber)
            //    .IsCreditCard()
            //    .WithMessageAwait(localizationService.GetResourceAsync("Payment.CardNumber.Wrong"));

            //RuleFor(model => model.CardCode)
            //    .Matches(@"^[0-9]{3,4}$")
            //    .WithMessageAwait(localizationService.GetResourceAsync("Payment.CardCode.Wrong"));

            //RuleFor(model => model.ExpireMonth)
            //    .NotEmpty()
            //    .WithMessageAwait(localizationService.GetResourceAsync("Payment.ExpireMonth.Required"));

            //RuleFor(model => model.ExpireYear)
            //    .NotEmpty()
            //    .WithMessageAwait(localizationService.GetResourceAsync("Payment.ExpireYear.Required"));
        }

        #endregion
    }
}