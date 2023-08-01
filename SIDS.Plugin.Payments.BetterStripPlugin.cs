using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework.Menu;
using NopStation.Plugin.Misc.Core;
using NopStation.Plugin.Misc.Core.Services;
using SIDS.Plugin.Payments.BetterStripe.Components;
using SIDS.Plugin.Payments.BetterStripe.Models;
using SIDS.Plugin.Payments.BetterStripe.Validators;
using Stripe;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SIDS.Plugin.Payments.BetterStripe
{
	public class StripePaymentProcessor : BasePlugin, IPaymentMethod, IPlugin, INopStationPlugin, IAdminMenuPlugin
	{
		private readonly StripePaymentSettings _stripePaymentSettings;

		private readonly ISettingService _settingService;

		private readonly ICurrencyService _currencyService;

		private readonly ILocalizationService _localizationService;

		private readonly IWebHelper _webHelper;

		private readonly IWorkContext _workContext;

	//	private readonly INopStationCoreService _nopStationCoreService;

		private readonly IPermissionService _permissionService;

		private readonly ICustomerService _customerService;

		private readonly IOrderTotalCalculationService _orderTotalCalculationService;

		private readonly CurrencySettings _currencySettings;

		public Nop.Services.Payments.PaymentMethodType PaymentMethodType
		{
			get
			{
				return 10;
			}
		}

		public Nop.Services.Payments.RecurringPaymentType RecurringPaymentType
		{
			get
			{
				return 0;
			}
		}

		public bool SkipPaymentInfo
		{
			get
			{
				return false;
			}
		}

		public bool SupportCapture
		{
			get
			{
				return true;
			}
		}

		public bool SupportPartiallyRefund
		{
			get
			{
				return true;
			}
		}

		public bool SupportRefund
		{
			get
			{
				return true;
			}
		}

		public bool SupportVoid
		{
			get
			{
				return true;
			}
		}

		public StripePaymentProcessor(StripePaymentSettings stripePaymentSettings, 
                                        ISettingService settingService, 
                                        ICurrencyService currencyService, 
                                        ILocalizationService localizationService, 
                                        IWebHelper webHelper, 
                                        IWorkContext workContext, 
                                        //INopStationCoreService nopStationCoreService,
                                        IPermissionService permissionService, 
                                        ICustomerService customerService, 
                                        IOrderTotalCalculationService orderTotalCalculationService, 
                                        CurrencySettings currencySettings)
		{
			_stripePaymentSettings = stripePaymentSettings;
			_settingService = settingService;
			_currencyService = currencyService;
			_localizationService = localizationService;
			_webHelper = webHelper;
			_workContext = workContext;
		//	this._nopStationCoreService = nopStationCoreService;
			_permissionService = permissionService;
			_customerService = customerService;
			_orderTotalCalculationService = orderTotalCalculationService;
			_currencySettings = currencySettings;
		}

		public Task<CancelRecurringPaymentResult> CancelRecurringPaymentAsync(CancelRecurringPaymentRequest cancelPaymentRequest)
		{
			return Task.FromResult<CancelRecurringPaymentResult>(new CancelRecurringPaymentResult());
		}

		public Task<bool> CanRePostProcessPaymentAsync(Nop.Core.Domain.Orders.Order order)
		{
			return Task.FromResult<bool>(false);
		}

		public Task<CapturePaymentResult> CaptureAsync(CapturePaymentRequest capturePaymentRequest)
		{
			CapturePaymentResult capturePaymentResult = new CapturePaymentResult();
			ChargeCaptureOptions chargeCaptureOption = new ChargeCaptureOptions()
			{
				Amount = new long?((long)((int)(capturePaymentRequest.get_Order().get_OrderTotal() * new decimal(100))))
			};
			Charge charge = (new ChargeService(new StripeClient(this._stripePaymentSettings.ApiKey, null, null, null, null, null))).Capture(capturePaymentRequest.get_Order().get_AuthorizationTransactionId(), chargeCaptureOption, null);
			if (!charge.Captured || !(charge.Status == "succeeded"))
			{
				capturePaymentResult.AddError(this._localizationService.GetResourceAsync("NopStation.Stripe.CaptureFailed").get_Result());
			}
			else
			{
				capturePaymentResult.set_NewPaymentStatus(30);
				capturePaymentResult.set_CaptureTransactionId(charge.Id);
			}
			return Task.FromResult<CapturePaymentResult>(capturePaymentResult);
		}

		public async Task<decimal> GetAdditionalHandlingFeeAsync(IList<ShoppingCartItem> cart)
		{
			decimal num = await this._orderTotalCalculationService.CalculatePaymentAdditionalFeeAsync(cart, this._stripePaymentSettings.AdditionalFee, this._stripePaymentSettings.AdditionalFeePercentage);
			return num;
		}

		public override string GetConfigurationPageUrl()
		{
			bool? nullable = null;
			return string.Concat(this._webHelper.GetStoreLocation(nullable), "Admin/PaymentStripe/Configure");
		}

		public Task<ProcessPaymentRequest> GetPaymentInfoAsync(IFormCollection form)
		{
			ProcessPaymentRequest processPaymentRequest = new ProcessPaymentRequest();
			processPaymentRequest.CreditCardType=form["CreditCardType"];
			processPaymentRequest.set_CreditCardName(form.get_Item("CardholderName"));
			processPaymentRequest.set_CreditCardNumber(form.get_Item("CardNumber"));
			processPaymentRequest.set_CreditCardExpireMonth(int.Parse(form.get_Item("ExpireMonth")));
			processPaymentRequest.set_CreditCardExpireYear(int.Parse(form.get_Item("ExpireYear")));
			processPaymentRequest.set_CreditCardCvv2(form.get_Item("CardCode"));
			return Task.FromResult<ProcessPaymentRequest>(processPaymentRequest);
		}

		public async Task<string> GetPaymentMethodDescriptionAsync()
		{
			return await this._localizationService.GetResourceAsync("Admin.NopStation.Stripe.Configuration.Fields.PaymentMethodDescription");
		}

		public Type GetPublicViewComponent()
		{
			return typeof(StripePaymentViewComponent);
		}

		public Task<bool> HidePaymentMethodAsync(IList<ShoppingCartItem> cart)
		{
			return Task.FromResult<bool>(false);
		}

		public override async Task InstallAsync()
		{
			StripePaymentSettings stripePaymentSetting = new StripePaymentSettings()
			{
				TransactionMode = TransactionMode.AuthorizeAndCapture
			};
			await this._settingService.SaveSettingAsync<StripePaymentSettings>(stripePaymentSetting, 0);
			await NopStationHelpers.InstallPluginAsync<StripePaymentProcessor>(this, new StripePaymentPermissionProvider(), true);
			await this.<>n__0();
		}

		public async Task ManageSiteMapAsync(SiteMapNode rootNode)
		{
			SiteMapNode siteMapNode = new SiteMapNode();
			SiteMapNode siteMapNode1 = siteMapNode;
			string resourceAsync = await this._localizationService.GetResourceAsync("Admin.NopStation.Stripe.Menu.StripePayment");
			siteMapNode1.set_Title(resourceAsync);
			siteMapNode.set_Visible(true);
			siteMapNode.set_IconClass("far fa-dot-circle");
			SiteMapNode siteMapNode2 = siteMapNode;
			siteMapNode1 = null;
			siteMapNode = null;
			if (await this._permissionService.AuthorizeAsync(StripePaymentPermissionProvider.ManageConfiguration))
			{
				siteMapNode1 = new SiteMapNode();
				siteMapNode = siteMapNode1;
				resourceAsync = await this._localizationService.GetResourceAsync("Admin.NopStation.Stripe.Menu.Configuration");
				siteMapNode.set_Title(resourceAsync);
				bool? nullable = null;
				siteMapNode1.set_Url(string.Concat(this._webHelper.GetStoreLocation(nullable), "Admin/PaymentStripe/Configure"));
				siteMapNode1.set_Visible(true);
				siteMapNode1.set_IconClass("far fa-circle");
				siteMapNode1.set_SystemName("StripePayment.Configuration");
				SiteMapNode siteMapNode3 = siteMapNode1;
				siteMapNode = null;
				siteMapNode1 = null;
				siteMapNode2.get_ChildNodes().Add(siteMapNode3);
			}
			if (await this._permissionService.AuthorizeAsync(CorePermissionProvider.ShowDocumentations))
			{
				siteMapNode = new SiteMapNode();
				siteMapNode1 = siteMapNode;
				resourceAsync = await this._localizationService.GetResourceAsync("Admin.NopStation.Common.Menu.Documentation");
				siteMapNode1.set_Title(resourceAsync);
				siteMapNode.set_Url("https://www.nop-station.com/stripe-payment-documentation?utm_source=admin-panel&utm_medium=products&utm_campaign=stripe-payment");
				siteMapNode.set_Visible(true);
				siteMapNode.set_IconClass("far fa-circle");
				siteMapNode.set_OpenUrlInNewTab(true);
				SiteMapNode siteMapNode4 = siteMapNode;
				siteMapNode1 = null;
				siteMapNode = null;
				siteMapNode2.get_ChildNodes().Add(siteMapNode4);
			}
			await this._nopStationCoreService.ManageSiteMapAsync(rootNode, siteMapNode2, 1);
			siteMapNode2 = null;
		}

		public List<KeyValuePair<string, string>> PluginResouces()
		{
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			list.Add(new KeyValuePair<string, string>("Admin.NopStation.Stripe.Configuration.Fields.AdditionalFee", "Additional fee"));
			list.Add(new KeyValuePair<string, string>("Admin.NopStation.Stripe.Configuration.Fields.AdditionalFee.Hint", "Enter additional fee to charge your customers."));
			list.Add(new KeyValuePair<string, string>("Admin.NopStation.Stripe.Configuration.Fields.AdditionalFeePercentage", "Additional fee. Use percentage"));
			list.Add(new KeyValuePair<string, string>("Admin.NopStation.Stripe.Configuration.Fields.AdditionalFeePercentage.Hint", "Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used."));
			list.Add(new KeyValuePair<string, string>("Admin.NopStation.Stripe.Configuration.Fields.ApiKey", "Secret key"));
			list.Add(new KeyValuePair<string, string>("Admin.NopStation.Stripe.Configuration.Fields.ApiKey.Hint", "The secret key."));
			list.Add(new KeyValuePair<string, string>("Admin.NopStation.Stripe.Configuration.Fields.TransactionMode", "Transaction mode"));
			list.Add(new KeyValuePair<string, string>("Admin.NopStation.Stripe.Configuration.Fields.TransactionMode.Hint", "Select transaction mode."));
			list.Add(new KeyValuePair<string, string>("Admin.NopStation.Stripe.Configuration.Fields.PaymentMethodDescription", "Pay by credit / debit card"));
			list.Add(new KeyValuePair<string, string>("Admin.NopStation.Stripe.Menu.StripePayment", "Stripe payment"));
			list.Add(new KeyValuePair<string, string>("Admin.NopStation.Stripe.Menu.Configuration", "Configuration"));
			list.Add(new KeyValuePair<string, string>("Admin.NopStation.Stripe.Configuration", "Stripe payment settings"));
			list.Add(new KeyValuePair<string, string>("NopStation.Stripe.CreditCardType", "Card Type"));
			list.Add(new KeyValuePair<string, string>("NopStation.Stripe.CardholderName", "Cardholder Name"));
			list.Add(new KeyValuePair<string, string>("NopStation.Stripe.CardNumber", "Card Number"));
			list.Add(new KeyValuePair<string, string>("NopStation.Stripe.ExpirationDate", "Exp. Date"));
			list.Add(new KeyValuePair<string, string>("NopStation.Stripe.CardCode", "Card Code"));
			list.Add(new KeyValuePair<string, string>("NopStation.Stripe.CustomerNotFound", "Customer is not found"));
			list.Add(new KeyValuePair<string, string>("NopStation.Stripe.NotSupportedTransaction", "Not supported transaction type"));
			list.Add(new KeyValuePair<string, string>("NopStation.Stripe.VoidFailed", "Void request failed"));
			list.Add(new KeyValuePair<string, string>("NopStation.Stripe.RefundFailed", "Refund request failed"));
			list.Add(new KeyValuePair<string, string>("NopStation.Stripe.CaptureFailed", "Capture request failed"));
			list.Add(new KeyValuePair<string, string>("NopStation.Stripe.CurrencyNotLoaded", "Currency cannot be loaded"));
			list.Add(new KeyValuePair<string, string>("NopStation.Stripe.TransactionFailed", "Transaction failed"));
			return list;
		}

		public Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest)
		{
			return Task.get_CompletedTask();
		}

		public async Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest processPaymentRequest)
		{
			Currency workingCurrencyAsync;
			int num;
			ProcessPaymentResult processPaymentResult = new ProcessPaymentResult();
			processPaymentResult.set_AllowStoringCreditCardNumber(false);
			ProcessPaymentResult processPaymentResult1 = processPaymentResult;
			TokenCreateOptions tokenCreateOption = new TokenCreateOptions();
			TokenCardOptions tokenCardOption = new TokenCardOptions()
			{
				Number = processPaymentRequest.get_CreditCardNumber()
			};
			int creditCardExpireYear = processPaymentRequest.get_CreditCardExpireYear();
			tokenCardOption.ExpYear = new long?(long.Parse(creditCardExpireYear.ToString()));
			creditCardExpireYear = processPaymentRequest.get_CreditCardExpireMonth();
			tokenCardOption.ExpMonth = new long?(long.Parse(creditCardExpireYear.ToString()));
			tokenCardOption.Cvc = processPaymentRequest.get_CreditCardCvv2();
			creditCardExpireYear = processPaymentRequest.get_CustomerId();
			tokenCardOption.Name = creditCardExpireYear.ToString();
			tokenCreateOption.Card = tokenCardOption;
			TokenCreateOptions tokenCreateOption1 = tokenCreateOption;
			TokenService tokenService = new TokenService(new StripeClient(this._stripePaymentSettings.ApiKey, null, null, null, null, null));
			Token token = tokenService.Create(tokenCreateOption1, null);
			Nop.Core.Domain.Customers.Customer customerByIdAsync = await this._customerService.GetCustomerByIdAsync(processPaymentRequest.get_CustomerId());
			if (customerByIdAsync == null)
			{
				throw new NopException(this._localizationService.GetResourceAsync("NopStation.Stripe.CustomerNotFound").get_Result());
			}
			ICurrencyService currencyService = this._currencyService;
			int? currencyId = customerByIdAsync.get_CurrencyId();
			num = (currencyId.get_HasValue() ? currencyId.GetValueOrDefault() : this._currencySettings.get_PrimaryStoreCurrencyId());
			Currency currencyByIdAsync = await currencyService.GetCurrencyByIdAsync(num);
			if (currencyByIdAsync == null || !currencyByIdAsync.get_Published())
			{
				workingCurrencyAsync = await this._workContext.GetWorkingCurrencyAsync();
			}
			else
			{
				workingCurrencyAsync = currencyByIdAsync;
			}
			Currency currency = workingCurrencyAsync;
			if (currency == null)
			{
				throw new NopException(this._localizationService.GetResourceAsync("NopStation.Stripe.CurrencyNotLoaded").get_Result());
			}
			ChargeCreateOptions chargeCreateOption = new ChargeCreateOptions()
			{
				Amount = new long?((long)((int)(processPaymentRequest.get_OrderTotal() * new decimal(100)))),
				Currency = currency.get_CurrencyCode(),
				Description = processPaymentRequest.get_OrderGuid().ToString(),
				Source = token.Id
			};
			ChargeCreateOptions nullable = chargeCreateOption;
			nullable.Capture = new bool?(this._stripePaymentSettings.TransactionMode != TransactionMode.Authorize);
			Charge charge = (new ChargeService(new StripeClient(this._stripePaymentSettings.ApiKey, null, null, null, null, null))).Create(nullable, null);
			if (charge.Status == "succeeded")
			{
				if (!charge.Captured || this._stripePaymentSettings.TransactionMode != TransactionMode.AuthorizeAndCapture)
				{
					processPaymentResult1.set_NewPaymentStatus(20);
					processPaymentResult1.set_AuthorizationTransactionId(charge.Id);
					processPaymentResult1.set_AuthorizationTransactionResult(charge.Status);
				}
				else
				{
					processPaymentResult1.set_NewPaymentStatus(30);
					processPaymentResult1.set_CaptureTransactionId(charge.Id);
					processPaymentResult1.set_CaptureTransactionResult(charge.Status);
				}
			}
			else if (charge.Status != "pending")
			{
				processPaymentResult1.AddError(this._localizationService.GetResourceAsync("NopStation.Stripe.TransactionFailed").get_Result());
			}
			else
			{
				processPaymentResult1.set_NewPaymentStatus(10);
				processPaymentResult1.set_CaptureTransactionId(charge.Id);
				processPaymentResult1.set_CaptureTransactionResult(charge.Status);
			}
			ProcessPaymentResult processPaymentResult2 = processPaymentResult1;
			processPaymentResult1 = null;
			token = null;
			return processPaymentResult2;
		}

		public Task<ProcessPaymentResult> ProcessRecurringPaymentAsync(ProcessPaymentRequest processPaymentRequest)
		{
			ProcessPaymentResult processPaymentResult = new ProcessPaymentResult();
			TransactionMode transactionMode = this._stripePaymentSettings.TransactionMode;
			if (transactionMode == TransactionMode.Authorize)
			{
				processPaymentResult.NewPaymentStatus=20;
			}
			else if (transactionMode == TransactionMode.AuthorizeAndCapture)
			{
				processPaymentResult.set_NewPaymentStatus(30);
			}
			else
			{
				processPaymentResult.AddError(this._localizationService.GetResourceAsync("NopStation.Stripe.NotSupportedTransaction").get_Result());
			}
			return Task.FromResult<ProcessPaymentResult>(processPaymentResult);
		}

		public Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest refundPaymentRequest)
		{
			RefundPaymentResult refundPaymentResult = new RefundPaymentResult();
			decimal num = (refundPaymentRequest.get_IsPartialRefund() ? refundPaymentRequest.get_AmountToRefund() : refundPaymentRequest.get_Order().get_OrderTotal());
			RefundCreateOptions refundCreateOption = new RefundCreateOptions()
			{
				Amount = new long?((long)(num * new decimal(100))),
				Reason = "requested_by_customer",
				Charge = refundPaymentRequest.get_Order().get_CaptureTransactionId()
			};
			if ((new RefundService(new StripeClient(this._stripePaymentSettings.ApiKey, null, null, null, null, null))).Create(refundCreateOption, null).Status != "succeeded")
			{
				refundPaymentResult.AddError(this._localizationService.GetResourceAsync("NopStation.Stripe.RefundFailed").get_Result());
			}
			else
			{
				refundPaymentResult.set_NewPaymentStatus((refundPaymentRequest.get_IsPartialRefund() ? 35 : 40));
			}
			return Task.FromResult<RefundPaymentResult>(refundPaymentResult);
		}

		public override async Task UninstallAsync()
		{
			await this._settingService.DeleteSettingAsync<StripePaymentSettings>();
			await NopStationHelpers.UninstallPluginAsync<StripePaymentProcessor>(this, new StripePaymentPermissionProvider());
			await this.<>n__1();
		}

		public Task<IList<string>> ValidatePaymentFormAsync(IFormCollection form)
		{
			List<string> list = new List<string>();
            PaymentInfoValidator paymentInfoValidator = new PaymentInfoValidator(this._localizationService);
            PaymentInfoModel paymentInfoModel = new PaymentInfoModel()
			{
				CardholderName = form.get_Item("CardholderName"),
				CardNumber = form.get_Item("CardNumber"),
				CardCode = form.get_Item("CardCode"),
				ExpireMonth = form.get_Item("ExpireMonth"),
				ExpireYear = form.get_Item("ExpireYear")
			};
			ValidationResult validationResult = ((AbstractValidator<PaymentInfoModel>)paymentInfoValidator).Validate(paymentInfoModel);
			if (!validationResult.get_IsValid())
			{
				foreach (ValidationFailure error in validationResult.get_Errors())
				{
					list.Add(error.get_ErrorMessage());
				}
			}
			return Task.FromResult<IList<string>>(list);
		}

		public Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest voidPaymentRequest)
		{
			VoidPaymentResult voidPaymentResult = new VoidPaymentResult();
			RefundCreateOptions refundCreateOption = new RefundCreateOptions()
			{
				Amount = new long?((long)(voidPaymentRequest.get_Order().get_OrderTotal() * new decimal(100))),
				Reason = "requested_by_customer",
				Charge = voidPaymentRequest.get_Order().get_AuthorizationTransactionId()
			};
			if ((new RefundService(new StripeClient(this._stripePaymentSettings.ApiKey, null, null, null, null, null))).Create(refundCreateOption, null).Status != "succeeded")
			{
				voidPaymentResult.AddError(this._localizationService.GetResourceAsync("NopStation.Stripe.VoidFailed").get_Result());
			}
			else
			{
				voidPaymentResult.set_NewPaymentStatus(50);
			}
			return Task.FromResult<VoidPaymentResult>(voidPaymentResult);
		}
	}
}