using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Services.Security;
using System;
using System.Collections.Generic;

namespace SIDS.Plugin.Payments.BetterStripe
{
	public class StripePaymentPermissionProvider : IPermissionProvider
	{
		public readonly static PermissionRecord ManageConfiguration;

		static StripePaymentPermissionProvider()
		{
			PermissionRecord permissionRecord = new PermissionRecord();
			permissionRecord.set_Name("NopStation stripe payment. Manage stripe payment");
			permissionRecord.set_SystemName("ManageNopStationStripePayment");
			permissionRecord.set_Category("NopStation");
            StripePaymentPermissionProvider.ManageConfiguration = permissionRecord;
		}

		public StripePaymentPermissionProvider()
		{
		}

		[return: TupleElementNames(new string[] { "systemRoleName", "permissions" })]
		public virtual HashSet<ValueTuple<string, PermissionRecord[]>> GetDefaultPermissions()
		{
			HashSet<ValueTuple<string, PermissionRecord[]>> hashSet = new HashSet<ValueTuple<string, PermissionRecord[]>>();
			hashSet.Add(new ValueTuple<string, PermissionRecord[]>(NopCustomerDefaults.get_AdministratorsRoleName(), new PermissionRecord[] { StripePaymentPermissionProvider.ManageConfiguration }));
			return hashSet;
		}

		public virtual IEnumerable<PermissionRecord> GetPermissions()
		{
			return new PermissionRecord[] { StripePaymentPermissionProvider.ManageConfiguration };
		}
	}
}