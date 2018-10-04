using System;
using System.Threading;
using EazeCrawler.Common.Interfaces;
using EazeCrawler.Extensions;
using EazeCrawler.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ninject;
using Ninject.Activation;
using Ninject.Infrastructure.Disposal;

namespace EazeCrawler
{
    public class Startup
    {
        private readonly AsyncLocal<Scope> _scopeProvider = new AsyncLocal<Scope>();
        private IKernel Kernel { get; set; }

        private object Resolve(Type type) => Kernel.Get(type);
        private object RequestScope(IContext context) => _scopeProvider.Value;

        private sealed class Scope : DisposableObject
        {
        }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddRequestScopingMiddleware(() => _scopeProvider.Value = new Scope());
            services.AddCustomControllerActivation(Resolve);
            services.AddCustomViewComponentActivation(Resolve);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            Kernel = RegisterApplicationComponents(app);

            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            else app.UseHsts();

            app.UseHttpsRedirection();
            app.UseMvc();
        }

        private IKernel RegisterApplicationComponents(IApplicationBuilder app)
        {
            // IKernelConfiguration config = new KernelConfiguration();
            var kernel = new StandardKernel();

            // Register application services
            foreach (var ctrlType in app.GetControllerTypes())
                kernel.Bind(ctrlType).ToSelf().InScope(RequestScope);

            // This is where our bindings are configurated
            kernel.Bind<ICrawler>().To<Crawler>().InScope(RequestScope);
            kernel.Bind<IEventManager>().To<EventManager>().InSingletonScope();
            kernel.Bind<IScheduler>().To<Scheduler>().InSingletonScope();
                
            // Cross-wire required framework services
            kernel.BindToMethod(app.GetRequestService<IViewBufferScope>);

            return kernel;
        }
    }
}