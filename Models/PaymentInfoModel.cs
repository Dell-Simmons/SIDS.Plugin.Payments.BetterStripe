using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace SIDS.Plugin.Payments.BetterStripe.Models
{
	public record PaymentInfoModel : BaseNopModel, IEquatable<PaymentInfoModel>
	{
		[NopResourceDisplayName("NopStation.Stripe.CardCode")]
		public string CardCode
		{
			get;
			set;
		}

		[NopResourceDisplayName("NopStation.Stripe.CardholderName")]
		public string CardholderName
		{
			get;
			set;
		}

		[NopResourceDisplayName("NopStation.Stripe.CardNumber")]
		public string CardNumber
		{
			get;
			set;
		}

		[NopResourceDisplayName("NopStation.Stripe.CreditCardType")]
		public string CreditCardType
		{
			get;
			set;
		}

		public IList<SelectListItem> CreditCardTypes
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
				return typeof(PaymentInfoModel);
			}
		}

		[NopResourceDisplayName("NopStation.Stripe.ExpirationDate")]
		public string ExpireMonth
		{
			get;
			set;
		}

		public IList<SelectListItem> ExpireMonths
		{
			get;
			set;
		}

		[NopResourceDisplayName("NopStation.Stripe.ExpirationDate")]
		public string ExpireYear
		{
			get;
			set;
		}

		public IList<SelectListItem> ExpireYears
		{
			get;
			set;
		}

		public PaymentInfoModel()
		{
			this.CreditCardTypes = new List<SelectListItem>();
			this.ExpireMonths = new List<SelectListItem>();
			this.ExpireYears = new List<SelectListItem>();
		}
	}
}