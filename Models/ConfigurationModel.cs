using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace SIDS.Plugin.Payments.BetterStripe.Models
{
    /// <summary>
    /// Represents a configuration model
    /// </summary>
    public record ConfigurationModel : BaseSearchModel
    {
        #region Properties
        public int ActiveStoreScopeConfiguration { get; set; }


        [NopResourceDisplayName("Plugins.Payments.BetterStripe.Fields.PublicKey")]
        public string PublicKey { get; set; }

        [NopResourceDisplayName("Plugins.Payments.BetterStripe.Fields.SecretKey")]
        [DataType(DataType.Password)]
        public string SecretKey { get; set; }

        [NopResourceDisplayName("Plugins.Payments.BetterStripe.Fields.Use3DS")]
        public bool Use3DS { get; set; }

        [NopResourceDisplayName("Plugins.Payments.BetterStripe.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }
        #endregion
    }
}