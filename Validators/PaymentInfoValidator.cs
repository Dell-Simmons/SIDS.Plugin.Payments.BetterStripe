using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using SIDS.Plugin.Payments.BetterStripe.Models;
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SIDS.Plugin.Payments.BetterStripe.Validators
{
	public class PaymentInfoValidator : BaseNopValidator<PaymentInfoModel>
	{
		public PaymentInfoValidator(ILocalizationService localizationService)
		{
			DefaultValidatorOptions.WithMessage<PaymentInfoModel, string>(DefaultValidatorExtensions.NotEmpty<PaymentInfoModel, string>(base.RuleFor<string>((PaymentInfoModel x) => x.CardholderName)), localizationService.GetResourceAsync("Payment.CardholderName.Required").get_Result());
			DefaultValidatorOptions.WithMessage<PaymentInfoModel, string>(ValidatorExtensions.IsCreditCard<PaymentInfoModel>(base.RuleFor<string>((PaymentInfoModel x) => x.CardNumber)), localizationService.GetResourceAsync("Payment.CardNumber.Wrong").get_Result());
			DefaultValidatorOptions.WithMessage<PaymentInfoModel, string>(DefaultValidatorExtensions.Matches<PaymentInfoModel>(base.RuleFor<string>((PaymentInfoModel x) => x.CardCode), "^[0-9]{3,4}$"), localizationService.GetResourceAsync("Payment.CardCode.Wrong").get_Result());
			DefaultValidatorOptions.WithMessage<PaymentInfoModel, string>(DefaultValidatorExtensions.NotEmpty<PaymentInfoModel, string>(base.RuleFor<string>((PaymentInfoModel x) => x.ExpireMonth)), localizationService.GetResourceAsync("Payment.ExpireMonth.Required").get_Result());
			DefaultValidatorOptions.WithMessage<PaymentInfoModel, string>(DefaultValidatorExtensions.NotEmpty<PaymentInfoModel, string>(base.RuleFor<string>((PaymentInfoModel x) => x.ExpireYear)), localizationService.GetResourceAsync("Payment.ExpireYear.Required").get_Result());
		}
	}
}