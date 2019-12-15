using Ege.AspNetCore.Mvc.PluginsManager;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Plugin.Tax
{
    internal class PluginSetup :IPluginSetup 
    {
        public int Order => 10;
        public ViewsType ViewsType => ViewsType.Internal;
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
