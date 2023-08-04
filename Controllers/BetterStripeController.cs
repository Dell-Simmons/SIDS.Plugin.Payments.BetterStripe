using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using SIDS.Plugin.Payments.BetterStripe.Models;

namespace SIDS.Plugin.Payments.BetterStripe.Controllers
{
    [Area(AreaNames.Admin)]
    [HttpsRequirement]
    [AutoValidateAntiforgeryToken]
    [ValidateIpAddress]
    [AuthorizeAdmin]
    [ValidateVendor]
    public class BetterStripeController : BasePaymentController
    {
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;

        public BetterStripeController(ILocalizationService localizationService,
                                        INotificationService notificationService,
                                        IPermissionService permissionService,
                                        ISettingService settingService,
                                        IStoreContext storeContext)
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _storeContext = storeContext;
        }

        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var betterStripePaymentSettings = await _settingService.LoadSettingAsync<BetterStripePaymentSettings>(storeScope);

            var model = new ConfigurationModel
            {
                ActiveStoreScopeConfiguration = storeScope,
                UseSandbox = betterStripePaymentSettings.UseSandbox,
                PublicKey = betterStripePaymentSettings.PublicKey,
                SecretKey = betterStripePaymentSettings.SecretKey,
                Use3DS = betterStripePaymentSettings.Use3DS
            };

          

            return View("~/Plugins/Payments.BetterStripe/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return await Configure();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var betterStripePaymentSettings = await _settingService.LoadSettingAsync<BetterStripePaymentSettings>(storeScope);

            //save settings
            betterStripePaymentSettings.UseSandbox = model.UseSandbox;
            betterStripePaymentSettings.PublicKey = model.PublicKey;
            betterStripePaymentSettings.SecretKey = model.SecretKey;
            betterStripePaymentSettings.Use3DS = model.Use3DS;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
         
            //now clear settings cache
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }
    }
}
