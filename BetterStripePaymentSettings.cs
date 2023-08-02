using Nop.Core.Configuration;

namespace SIDS.Plugin.Payments.BetterStripe
{
    /// <summary>
    /// Represents plugin settings
    /// </summary>
    public class BetterStripePaymentSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether to use sandbox environment
        /// </summary>
        public bool UseSandbox { get; set; }

       
        /// <summary>
        /// Gets or sets a public key
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        /// Gets or sets a private key
        /// </summary>
        public string SecretKey { get; set; }

    

        /// <summary>
        /// Gets or sets a value indicating whether to use Strong Customer Authentication (SCA) with 3-D secure implementation
        /// </summary>
        public bool Use3DS { get; set; }
    }
}