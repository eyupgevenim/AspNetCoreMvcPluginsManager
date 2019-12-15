using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Ege.AspNetCore.Mvc.PluginsManager
{
    public static class PluginManager
    {
        private static Dictionary<string, IPluginSetup> _Plugins;
        internal static Dictionary<string, IPluginSetup> Plugins => _Plugins ?? (_Plugins = new Dictionary<string, IPluginSetup>());

        public static void AddPlugins(this IServiceCollection services, string pluginsRootFolder)
        {
            LoadPlugins(pluginsRootFolder);

            foreach (var plugin in Plugins)
            {
                services.LoadViews(plugin);
                services.LoadControllers(plugin);

                foreach (var pluginSetup in Plugins.Select(x => x.Value))
                    pluginSetup.ConfigureServices(services);
            }
        }

        private static void LoadPlugins(string pluginsRootFolder)
        {
            var pluginInfos = GetPluginInfos(pluginsRootFolder);
            foreach (var pluginInfo in pluginInfos)
            {
                var dllPath = pluginInfo.Key.Replace("PluginInfo.json", pluginInfo.Value.File);
                var assembly = Assembly.LoadFile(dllPath);
                TypeInfo typeInfo;
                try
                {
                    typeInfo = assembly.DefinedTypes.First((dt) => dt.ImplementedInterfaces.Any(ii => ii == typeof(IPluginSetup)));
                }
                catch (Exception ex)
                {
                    throw new TypeLoadException($"{nameof(LoadPlugins)}: {dllPath} not contains a definition for {typeof(IPluginSetup).Name}", ex);
                }

                if (typeInfo == null)
                    throw new TypeUnloadedException($"{nameof(LoadPlugins)}: {dllPath} not contains a definition for {typeof(IPluginSetup).Name}");

                var Plugin = (IPluginSetup)assembly.CreateInstance(typeInfo.FullName);
                Plugins.Add(dllPath, Plugin);
            }
        }

        private static Dictionary<string, PluginInfo> GetPluginInfos(string pluginsRootFolder)
        {
            if (!Directory.Exists(pluginsRootFolder))
                throw new DirectoryNotFoundException($"{nameof(GetPluginInfos)}: no Plugins folder found");

            var pluginInfos = new Dictionary<string, PluginInfo>();
            var jsonPluginInfoPaths = Directory.GetFiles(pluginsRootFolder, "*PluginInfo.json", SearchOption.AllDirectories);
            foreach (var jsonPath in jsonPluginInfoPaths)
            {
                var pluginInfo = JsonConvert.DeserializeObject<PluginInfo>(File.ReadAllText(jsonPath));
                pluginInfos.Add(jsonPath, pluginInfo);
            }

            return pluginInfos;
        }

        private static void LoadViews(this IServiceCollection services, KeyValuePair<string, IPluginSetup> plugin)
        {
            switch (plugin.Value.ViewsType)
            {
                case ViewsType.Internal:
                    {
                        string viewAssembypath = plugin.Key.Replace(".dll", ".Views.dll");
                        if (File.Exists(viewAssembypath))
                        {
                            services.AddMvc().ConfigureApplicationPartManager(apm =>
                            {
                                var compiledRazorAssemblyApplicationParts = new CompiledRazorAssemblyApplicationPartFactory().GetApplicationParts(AssemblyLoadContext.Default.LoadFromAssemblyPath(viewAssembypath));
                                foreach (var craapf in compiledRazorAssemblyApplicationParts)
                                    apm.ApplicationParts.Add(craapf);
                            });
                        }
                        else
                            throw new FileNotFoundException($"{nameof(AddPlugins)}: no Plugin View Assembly found: {viewAssembypath}"); 
                    }
                    break;
                case ViewsType.External:
                    {
                        var embeddedFileProvider = new EmbeddedFileProvider(AssemblyLoadContext.Default.LoadFromAssemblyPath(plugin.Key));
                        services.Configure<RazorViewEngineOptions>(options =>
                        {
                            options.FileProviders.Add(embeddedFileProvider);
                        }); 
                    }
                    break;
                default:
                    break;
            }
        }

        private static void LoadControllers(this IServiceCollection services, KeyValuePair<string, IPluginSetup> plugin)
        {
            switch (plugin.Value.ControllerType)
            {
                case ControllerType.Internal:
                    {
                        services
                            .AddMvc()
                            .ConfigureApplicationPartManager(apm => apm
                                .ApplicationParts.Add(new AssemblyPart(AssemblyLoadContext.Default.LoadFromAssemblyPath(plugin.Key)))
                            ); 
                    }
                    break;
                default:
                    break;
            }
        }

        public static IApplicationBuilder UsePluginsConfigure(this IApplicationBuilder app, IHostEnvironment env)
        {
            foreach (var plugin in Plugins.OrderBy(x => x.Value.Order).Select(x => x.Value)) 
                plugin.Configure(app, env);

            return app;
        }
    }
}
