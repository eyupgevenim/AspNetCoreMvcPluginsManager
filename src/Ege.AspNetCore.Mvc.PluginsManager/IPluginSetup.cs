using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Ege.AspNetCore.Mvc.PluginsManager
{
    public interface IPluginSetup
    {
        int Order { get; }
        ViewsType ViewsType => ViewsType.Internal;
        ControllerType ControllerType => ControllerType.Internal;
        void ConfigureServices(IServiceCollection services);
        void Configure(IApplicationBuilder app, IHostEnvironment env);
    }
}
