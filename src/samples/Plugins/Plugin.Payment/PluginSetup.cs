using Ege.AspNetCore.Mvc.PluginsManager;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Plugin.Payment
{
    internal class PluginSetup : IPluginSetup
    {
        public int Order => 20;
        //public ViewsType ViewsType => ViewsType.Internal;
        //public ControllerType ControllerType => Controller.Internal;

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
