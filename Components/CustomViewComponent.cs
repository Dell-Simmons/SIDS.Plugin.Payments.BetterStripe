using System;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace SIDS.Plugin.Misc.SEOCleaner.Components
{
    [ViewComponent(Name = "Custom")]
    public class CustomViewComponent : NopViewComponent
    {
        public CustomViewComponent()
        {

        }

        public IViewComponentResult Invoke(int productId)
        {
            throw new NotImplementedException();
        }
    }
}
