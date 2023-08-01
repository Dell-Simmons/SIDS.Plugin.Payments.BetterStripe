using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace SIDS.Plugin.Payments.BetterStripe.Models
{
	public class ConfigurationModel : BaseNopModel, IEquatable<ConfigurationModel>
	{
		public int ActiveStoreScopeConfiguration
		{
			get;
			set;
		}

		[NopResourceDisplayName("Admin.NopStation.Stripe.Configuration.Fields.AdditionalFee")]
		public decimal AdditionalFee
		{
			get;
			set;
		}

		public bool AdditionalFee_OverrideForStore
		{
			get;
			set;
		}

		[NopResourceDisplayName("Admin.NopStation.Stripe.Configuration.Fields.AdditionalFeePercentage")]
		public bool AdditionalFeePercentage
		{
			get;
			set;
		}

		public bool AdditionalFeePercentage_OverrideForStore
		{
			get;
			set;
		}

		[NopResourceDisplayName("Admin.NopStation.Stripe.Configuration.Fields.ApiKey")]
		public string ApiKey
		{
			get;
			set;
		}

		public bool ApiKey_OverrideForStore
		{
			get;
			set;
		}

		[CompilerGenerated]
		[Nullable(1)]
		protected override Type EqualityContract
		{
			[NullableContext(1)]
			get
			{
				return typeof(ConfigurationModel);
			}
		}

		[NopResourceDisplayName("Admin.NopStation.Stripe.Configuration.Fields.TransactionMode")]
		public int TransactionModeId
		{
			get;
			set;
		}

		public bool TransactionModeId_OverrideForStore
		{
			get;
			set;
		}

		public SelectList TransactionModeValues
		{
			get;
			set;
		}

		public ConfigurationModel()
		{
		}
	}
}