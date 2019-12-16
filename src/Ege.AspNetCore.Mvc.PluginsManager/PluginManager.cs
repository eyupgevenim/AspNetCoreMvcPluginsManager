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
    /// <summary>
    /// Plugins manager extensions
    /// https://github.com/aspnet/AspNetCore.Docs/blob/master/aspnetcore/mvc/advanced/app-parts.md
    /// </summary>
    public static class PluginManager
    {
        private static Dictionary<string, IPluginSetup> _Plugins;
        /// <summary>
        /// Loaded plugin list as dictionary key:plugin assembly full path, value:plugin Setup.cs
        /// </summary>
        internal static Dictionary<string, IPluginSetup> Plugins => _Plugins ?? (_Plugins = new Dictionary<string, IPluginSetup>());
        /// <summary>
        /// Add all plugins ConfigureServices and run plugins ConfigureServices method
        /// </summary>
        /// <param name="services">Specifies the contract for a collection of service descriptors.</param>
        /// <param name="pluginsRootFolder">Plugins root path</param>
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
        /// <summary>
        /// Load all plugins assembly full path and plugins Setup.cs as dictionary
        /// </summary>
        /// <param name="pluginsRootFolder">Plugins root path</param>
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

                var pluginSetup = (IPluginSetup)assembly.CreateInstance(typeInfo.FullName);
                Plugins.Add(dllPath, pluginSetup);
            }
        }
        /// <summary>
        /// Load all plugins info as PluginInfo.json
        /// </summary>
        /// <param name="pluginsRootFolder">Plugins root path</param>
        /// <returns>plugins dictionary key:PluginInfo.json full path, value:PluginInfo.json file as PluginInfo class object</returns>
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
        /// <summary>
        /// Load pugin views
        /// </summary>
        /// <param name="services">Specifies the contract for a collection of service descriptors.</param>
        /// <param name="plugin">plugin object key:plugin full path, value:plugin Setup.cs</param>
        private static void LoadViews(this IServiceCollection services, KeyValuePair<string, IPluginSetup> plugin)
        {
            switch (plugin.Value.ViewsType)
            {
                case ViewsType.Internal:
                    {
                        string viewAssembypath = plugin.Key.Replace(".dll", ".Views.dll");
                        if (!File.Exists(viewAssembypath))
                            throw new FileNotFoundException($"{nameof(LoadViews)}: no Plugin View Assembly found: {viewAssembypath}");

                        services.AddMvc().ConfigureApplicationPartManager(apm =>
                        {
                            var compiledRazorAssemblyApplicationParts = new CompiledRazorAssemblyApplicationPartFactory().GetApplicationParts(AssemblyLoadContext.Default.LoadFromAssemblyPath(viewAssembypath));
                            foreach (var craapf in compiledRazorAssemblyApplicationParts)
                                apm.ApplicationParts.Add(craapf);
                        });
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
        /// <summary>
        /// Load plugin controllers
        /// https://docs.microsoft.com/en-us/aspnet/core/mvc/advanced/app-parts
        /// </summary>
        /// <param name="services">Specifies the contract for a collection of service descriptors.</param>
        /// <param name="plugin">plugin object key:plugin full path, value:plugin Setup.cs</param>
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
        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline for plugin
        /// </summary>
        /// <param name="app">Defines a class that provides the mechanisms to configure an application's request pipeline.</param>
        /// <param name="env">Provides information about the hosting environment an application is running in</param>
        /// <returns>Defines a class that provides the mechanisms to configure an application's request pipeline.</returns>
        public static IApplicationBuilder UsePluginsConfigure(this IApplicationBuilder app, IHostEnvironment env)
        {
            foreach (var plugin in Plugins.OrderBy(x => x.Value.Order).Select(x => x.Value)) 
                plugin.Configure(app, env);

            return app;
        }
    }
}
