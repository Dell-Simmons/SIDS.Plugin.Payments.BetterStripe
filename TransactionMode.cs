using System;

namespace SIDS.Plugin.Payments.BetterStripe
{
	public enum TransactionMode
	{
		Authorize = 1,
		AuthorizeAndCapture = 2
	}
}