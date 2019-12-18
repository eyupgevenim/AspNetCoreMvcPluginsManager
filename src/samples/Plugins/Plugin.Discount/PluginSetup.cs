using Ege.AspNetCore.Mvc.PluginsManager;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Plugin.Discount
{
    internal class PluginSetup :IPluginSetup 
    {
        public int Order => 200;
        public ViewsType ViewsType => ViewsType.None;
        public ControllerType ControllerType => ControllerType.Internal;
        
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
