using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using SIDS.Plugin.Payments.BetterStripe.Models;

namespace SIDS.Plugin.Payments.BetterStripe.Validators
{
    /// <summary>
    /// Represents configuration model validator
    /// </summary>
    public class ConfigurationValidator : BaseNopValidator<ConfigurationModel>
    {
        #region Ctor

        public ConfigurationValidator(ILocalizationService localizationService)
        {
            RuleFor(model => model.SecretKey)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.BetterStripe.Fields.SecretKey.Required"))
                .When(model => !model.UseSandbox);

            RuleFor(model => model.PublicKey)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.BetterStripe.Fields.PublicKey.Required"))
                .When(model => !model.UseSandbox);
        }

        #endregion
    }
}