using Ege.AspNetCore.Mvc.PluginsManager;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Plugin.Widget
{
    internal class PluginSetup :IPluginSetup
    {
        public int Order => 100;
        public void ConfigureServices(IServiceCollection services)
        {
            //TODO:
        }
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            //TODO:
        }
    }
}
