using Nop.Core.Configuration;
using System;
using System.Runtime.CompilerServices;

namespace SIDS.Plugin.Payments.BetterStripe
{
	public class StripePaymentSettings : ISettings
	{
		public decimal AdditionalFee
		{
			get;
			set;
		}

		public bool AdditionalFeePercentage
		{
			get;
			set;
		}

		public string ApiKey
		{
			get;
			set;
		}

		public TransactionMode TransactionMode
		{
			get;
			set;
		}

		public StripePaymentSettings()
		{
		}
	}
}