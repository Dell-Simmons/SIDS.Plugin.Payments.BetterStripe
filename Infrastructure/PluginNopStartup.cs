﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;

namespace SIDS.Plugin.Payments.BetterStripe.Infrastructure
{
    public class PluginNopStartup : INopStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new SIDS.Plugin.Payments..BetterStripe.Infrastructure.ViewLocationExpander());
            });

            //register services and interfaces
            //services.AddScoped<CustomModelFactory, ICustomerModelFactory>();
        }

        public void Configure(IApplicationBuilder application)
        {
        }

        public int Order => 11;
    }
}