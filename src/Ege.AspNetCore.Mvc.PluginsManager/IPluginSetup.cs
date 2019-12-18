using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Ege.AspNetCore.Mvc.PluginsManager
{
    /// <summary>
    /// ASP.NET Core apps use a Startup class, which is named Startup by convention. 
    /// Like uses plugin apps IPluginSetup class.
    /// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/startup
    /// </summary>
    public interface IPluginSetup
    {
        /// <summary>
        /// Middleware order:
        /// The order that middleware components are added in the Startup.
        /// Configure method defines the order in which the middleware components are invoked on requests 
        /// and the reverse order for the response. The order is critical 
        /// for security, performance, and functionality.
        /// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware
        /// </summary>
        int Order { get; }
        /// <summary>
        /// To access views internal or external plugin or not view
        /// </summary>
        [Obsolete("ViewsType will be removed in the next version.")]
        ViewsType ViewsType => ViewsType.Internal;
        /// <summary>
        /// To use internal controllers or not controller
        /// </summary>
        [Obsolete("ControllerType will be removed in the next version.")]
        ControllerType ControllerType => ControllerType.Internal;
        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container for plugin
        /// </summary>
        /// <param name="services">Specifies the contract for a collection of service descriptors.</param>
        void ConfigureServices(IServiceCollection services);
        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline for plugin
        /// </summary>
        /// <param name="app">Defines a class that provides the mechanisms to configure an application's request pipeline.</param>
        /// <param name="env">Provides information about the hosting environment an application is running in</param>
        void Configure(IApplicationBuilder app, IHostEnvironment env);
    }
}
