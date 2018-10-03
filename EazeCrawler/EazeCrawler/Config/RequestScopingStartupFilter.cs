using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace EazeCrawler.Config
{
    public sealed class RequestScopingStartupFilter : IStartupFilter
    {
        private readonly Func<IDisposable> _requestScopeProvider;

        public RequestScopingStartupFilter(Func<IDisposable> requestScopeProvider)
        {
            _requestScopeProvider =
                requestScopeProvider ?? throw new ArgumentNullException(nameof(requestScopeProvider));
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> nextFilter)
        {
            return builder =>
            {
                ConfigureRequestScoping(builder);
                nextFilter(builder);
            };
        }

        private void ConfigureRequestScoping(IApplicationBuilder builder)
        {
            builder.Use(async (context, next) =>
            {
                using (_requestScopeProvider())
                {
                    await next();
                }
            });
        }
    }
}