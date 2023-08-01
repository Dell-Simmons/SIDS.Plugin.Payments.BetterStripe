using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NopStation.Plugin.Misc.Core.Components;
using SIDS.Plugin.Payments.BetterStripe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SIDS.Plugin.Payments.BetterStripe.Components
{
	public class StripePaymentViewComponent : NopStationViewComponent
	{
		public StripePaymentViewComponent()
		{
		}

		public IViewComponentResult Invoke()
		{
            StripePaymentViewComponent.<>c__DisplayClass0_0 variable = null;
            PaymentInfoModel paymentInfoModel = new PaymentInfoModel();
			IList<SelectListItem> creditCardTypes = paymentInfoModel.CreditCardTypes;
			SelectListItem selectListItem = new SelectListItem();
			selectListItem.set_Text("Visa");
			selectListItem.set_Value("Visa");
			creditCardTypes.Add(selectListItem);
			IList<SelectListItem> list = paymentInfoModel.CreditCardTypes;
			SelectListItem selectListItem1 = new SelectListItem();
			selectListItem1.set_Text("Master card");
			selectListItem1.set_Value("MasterCard");
			list.Add(selectListItem1);
			IList<SelectListItem> creditCardTypes1 = paymentInfoModel.CreditCardTypes;
			SelectListItem selectListItem2 = new SelectListItem();
			selectListItem2.set_Text("Amex");
			selectListItem2.set_Value("Amex");
			creditCardTypes1.Add(selectListItem2);
			for (int i = 0; i < 15; i++)
			{
				DateTime now = DateTime.get_Now();
				string str = Convert.ToString(now.get_Year() + i);
				IList<SelectListItem> expireYears = paymentInfoModel.ExpireYears;
				SelectListItem selectListItem3 = new SelectListItem();
				selectListItem3.set_Text(str);
				selectListItem3.set_Value(str);
				expireYears.Add(selectListItem3);
			}
			for (int j = 1; j <= 12; j++)
			{
				string str1 = (j < 10 ? string.Concat("0", j.ToString()) : j.ToString());
				IList<SelectListItem> expireMonths = paymentInfoModel.ExpireMonths;
				SelectListItem selectListItem4 = new SelectListItem();
				selectListItem4.set_Text(str1);
				selectListItem4.set_Value(j.ToString());
				expireMonths.Add(selectListItem4);
			}
			if (base.get_Request().get_Method() != "GET")
			{
				IFormCollection form = base.get_Request().get_Form();
				paymentInfoModel.CardholderName = form.get_Item("CardholderName");
				paymentInfoModel.CardNumber = form.get_Item("CardNumber");
				paymentInfoModel.CardCode = form.get_Item("CardCode");
				SelectListItem selectListItem5 = Enumerable.FirstOrDefault<SelectListItem>(paymentInfoModel.CreditCardTypes, new Func<SelectListItem, bool>(variable, (SelectListItem x) => x.get_Value().Equals(this.form.get_Item("CreditCardType"), 3)));
				if (selectListItem5 != null)
				{
					selectListItem5.set_Selected(true);
				}
				SelectListItem selectListItem6 = Enumerable.FirstOrDefault<SelectListItem>(paymentInfoModel.ExpireMonths, new Func<SelectListItem, bool>(variable, (SelectListItem x) => x.get_Value().Equals(this.form.get_Item("ExpireMonth"), 3)));
				if (selectListItem6 != null)
				{
					selectListItem6.set_Selected(true);
				}
				SelectListItem selectListItem7 = Enumerable.FirstOrDefault<SelectListItem>(paymentInfoModel.ExpireYears, new Func<SelectListItem, bool>(variable, (SelectListItem x) => x.get_Value().Equals(this.form.get_Item("ExpireYear"), 3)));
				if (selectListItem7 != null)
				{
					selectListItem7.set_Selected(true);
				}
			}
			return base.View<PaymentInfoModel>("~/Plugins/NopStation.Plugin.Payments.Stripe/Views/PaymentInfo.cshtml", paymentInfoModel);
		}
	}
}