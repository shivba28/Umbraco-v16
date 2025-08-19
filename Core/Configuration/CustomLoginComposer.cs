using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace UmbracoV16.Core.Configuration
{
    public class CustomLoginComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            // Register any additional services if needed
            builder.Services.Configure<Microsoft.AspNetCore.Routing.RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
            });
        }
    }
}
