using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using NopStation.Plugin.Misc.Core.Controllers;
using NopStation.Plugin.Misc.Core.Filters;
using SIDS.Plugin.Payments.BetterStripe.Models;
using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SIDS.Plugin.Payments.BetterStripe.Controllers
{
	public class PaymentStripeController : NopStationAdminController
	{
		private readonly IWorkContext _workContext;

		private readonly IStoreService _storeService;

		private readonly ISettingService _settingService;

		private readonly ILocalizationService _localizationService;

		private readonly IStoreContext _storeContext;

		private readonly INotificationService _notificationService;

		private readonly IPermissionService _permissionService;

		public PaymentStripeController(IWorkContext workContext, IStoreService storeService, ISettingService settingService, ILocalizationService localizationService, IStoreContext storeContext, INotificationService notificationService, IPermissionService permissionService)
		{
			this._workContext = workContext;
			this._storeService = storeService;
			this._settingService = settingService;
			this._localizationService = localizationService;
			this._storeContext = storeContext;
			this._notificationService = notificationService;
			this._permissionService = permissionService;
		}

		public async Task<IActionResult> Configure()
		{
			IActionResult actionResult;
            StripePaymentSettings stripePaymentSetting;
            ConfigurationModel configurationModel;
			if (await this._permissionService.AuthorizeAsync(StripePaymentPermissionProvider.ManageConfiguration))
			{
				int activeStoreScopeConfigurationAsync = await this._storeContext.GetActiveStoreScopeConfigurationAsync();
				stripePaymentSetting = await this._settingService.LoadSettingAsync<StripePaymentSettings>(activeStoreScopeConfigurationAsync);
                ConfigurationModel apiKey = new ConfigurationModel()
				{
					TransactionModeId = Convert.ToInt32(stripePaymentSetting.TransactionMode),
					AdditionalFee = stripePaymentSetting.AdditionalFee,
					AdditionalFeePercentage = stripePaymentSetting.AdditionalFeePercentage
				};
                ConfigurationModel configurationModel1 = apiKey;
				SelectList selectListAsync = await Extensions.ToSelectListAsync<TransactionMode>(stripePaymentSetting.TransactionMode, true, null, true);
				configurationModel1.TransactionModeValues = selectListAsync;
				apiKey.ApiKey = stripePaymentSetting.ApiKey;
				apiKey.ActiveStoreScopeConfiguration = activeStoreScopeConfigurationAsync;
				configurationModel = apiKey;
				configurationModel1 = null;
				apiKey = null;
				if (activeStoreScopeConfigurationAsync > 0)
				{
					apiKey = configurationModel;
					ISettingService settingService = this._settingService;
                    StripePaymentSettings stripePaymentSetting1 = stripePaymentSetting;
					bool flag = await settingService.SettingExistsAsync<StripePaymentSettings, TransactionMode>(stripePaymentSetting1, (StripePaymentSettings x) => x.TransactionMode, activeStoreScopeConfigurationAsync);
					apiKey.TransactionModeId_OverrideForStore = flag;
					apiKey = null;
					apiKey = configurationModel;
					ISettingService settingService1 = this._settingService;
                    StripePaymentSettings stripePaymentSetting2 = stripePaymentSetting;
					flag = await settingService1.SettingExistsAsync<StripePaymentSettings, decimal>(stripePaymentSetting2, (StripePaymentSettings x) => x.AdditionalFee, activeStoreScopeConfigurationAsync);
					apiKey.AdditionalFee_OverrideForStore = flag;
					apiKey = null;
					apiKey = configurationModel;
					ISettingService settingService2 = this._settingService;
                    StripePaymentSettings stripePaymentSetting3 = stripePaymentSetting;
					flag = await settingService2.SettingExistsAsync<StripePaymentSettings, bool>(stripePaymentSetting3, (StripePaymentSettings x) => x.AdditionalFeePercentage, activeStoreScopeConfigurationAsync);
					apiKey.AdditionalFeePercentage_OverrideForStore = flag;
					apiKey = null;
					apiKey = configurationModel;
					ISettingService settingService3 = this._settingService;
                    StripePaymentSettings stripePaymentSetting4 = stripePaymentSetting;
					flag = await settingService3.SettingExistsAsync<StripePaymentSettings, string>(stripePaymentSetting4, (StripePaymentSettings x) => x.ApiKey, activeStoreScopeConfigurationAsync);
					apiKey.ApiKey_OverrideForStore = flag;
					apiKey = null;
				}
				actionResult = this.View("~/Plugins/NopStation.Plugin.Payments.Stripe/Views/Configure.cshtml", configurationModel);
			}
			else
			{
				actionResult = this.AccessDeniedView();
			}
			stripePaymentSetting = null;
			configurationModel = null;
			return actionResult;
		}

		[EditAccess(false)]
		[HttpPost]
		public async Task<IActionResult> Configure(ConfigurationModel model)
		{
			IActionResult action;
            StripePaymentSettings transactionModeId;
			if (await this._permissionService.AuthorizeAsync(StripePaymentPermissionProvider.ManageConfiguration))
			{
				int activeStoreScopeConfigurationAsync = await this._storeContext.GetActiveStoreScopeConfigurationAsync();
				transactionModeId = await this._settingService.LoadSettingAsync<StripePaymentSettings>(activeStoreScopeConfigurationAsync);
				transactionModeId.TransactionMode = model.TransactionModeId;
				transactionModeId.AdditionalFee = model.AdditionalFee;
				transactionModeId.AdditionalFeePercentage = model.AdditionalFeePercentage;
				transactionModeId.ApiKey = model.ApiKey;
				if (model.TransactionModeId_OverrideForStore || activeStoreScopeConfigurationAsync == 0)
				{
					ISettingService settingService = this._settingService;
                    StripePaymentSettings stripePaymentSetting = transactionModeId;
					await settingService.SaveSettingAsync<StripePaymentSettings, TransactionMode>(stripePaymentSetting, (StripePaymentSettings x) => x.TransactionMode, activeStoreScopeConfigurationAsync, false);
				}
				else if (activeStoreScopeConfigurationAsync > 0)
				{
					ISettingService settingService1 = this._settingService;
                    StripePaymentSettings stripePaymentSetting1 = transactionModeId;
					await settingService1.DeleteSettingAsync<StripePaymentSettings, TransactionMode>(stripePaymentSetting1, (StripePaymentSettings x) => x.TransactionMode, activeStoreScopeConfigurationAsync);
				}
				if (model.AdditionalFee_OverrideForStore || activeStoreScopeConfigurationAsync == 0)
				{
					ISettingService settingService2 = this._settingService;
                    StripePaymentSettings stripePaymentSetting2 = transactionModeId;
					await settingService2.SaveSettingAsync<StripePaymentSettings, decimal>(stripePaymentSetting2, (StripePaymentSettings x) => x.AdditionalFee, activeStoreScopeConfigurationAsync, false);
				}
				else if (activeStoreScopeConfigurationAsync > 0)
				{
					ISettingService settingService3 = this._settingService;
                    StripePaymentSettings stripePaymentSetting3 = transactionModeId;
					await settingService3.DeleteSettingAsync<StripePaymentSettings, decimal>(stripePaymentSetting3, (StripePaymentSettings x) => x.AdditionalFee, activeStoreScopeConfigurationAsync);
				}
				if (model.AdditionalFeePercentage_OverrideForStore || activeStoreScopeConfigurationAsync == 0)
				{
					ISettingService settingService4 = this._settingService;
                    StripePaymentSettings stripePaymentSetting4 = transactionModeId;
					await settingService4.SaveSettingAsync<StripePaymentSettings, bool>(stripePaymentSetting4, (StripePaymentSettings x) => x.AdditionalFeePercentage, activeStoreScopeConfigurationAsync, false);
				}
				else if (activeStoreScopeConfigurationAsync > 0)
				{
					ISettingService settingService5 = this._settingService;
                    StripePaymentSettings stripePaymentSetting5 = transactionModeId;
					await settingService5.DeleteSettingAsync<StripePaymentSettings, bool>(stripePaymentSetting5, (StripePaymentSettings x) => x.AdditionalFeePercentage, activeStoreScopeConfigurationAsync);
				}
				if (model.ApiKey_OverrideForStore || activeStoreScopeConfigurationAsync == 0)
				{
					ISettingService settingService6 = this._settingService;
                    StripePaymentSettings stripePaymentSetting6 = transactionModeId;
					await settingService6.SaveSettingAsync<StripePaymentSettings, string>(stripePaymentSetting6, (StripePaymentSettings x) => x.ApiKey, activeStoreScopeConfigurationAsync, false);
				}
				else if (activeStoreScopeConfigurationAsync > 0)
				{
					ISettingService settingService7 = this._settingService;
                    StripePaymentSettings stripePaymentSetting7 = transactionModeId;
					await settingService7.DeleteSettingAsync<StripePaymentSettings, string>(stripePaymentSetting7, (StripePaymentSettings x) => x.ApiKey, activeStoreScopeConfigurationAsync);
				}
				await this._settingService.ClearCacheAsync();
				INotificationService notificationService = this._notificationService;
				string resourceAsync = await this._localizationService.GetResourceAsync("Admin.Configuration.Updated");
				notificationService.SuccessNotification(resourceAsync, true);
				notificationService = null;
				action = this.RedirectToAction("Configure");
			}
			else
			{
				action = this.AccessDeniedView();
			}
			transactionModeId = null;
			return action;
		}
	}
}