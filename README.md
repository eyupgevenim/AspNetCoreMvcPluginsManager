 Asp.Net Core Mvc Plugins Manager
==================================


[![build status](https://img.shields.io/badge/buid-passing-green)](https://github.com/eyupgevenim/AspNetCoreMvcPluginsManager/tree/master/src/Ege.AspNetCore.Mvc.PluginsManager)
[![latest version](https://img.shields.io/nuget/v/Ege.AspNetCore.Mvc.PluginsManager)](https://www.nuget.org/packages/Ege.AspNetCore.Mvc.PluginsManager)
[![latest version](https://img.shields.io/badge/license-MIT-green)](LICENSE)

That is an open-source plugin(modular) and multi-tenant application framework built with ASP.NET Core, 
and a content management system (CMS) built on top of that application framework. Include a main project without referencing it using mvc structure in .dll projects as a plugin

### Installation
.Net Core Mvc Plugins Manager is available on [NuGet](https://www.nuget.org/packages/Ege.AspNetCore.Mvc.PluginsManager)

```sh
dotnet add package Ege.AspNetCore.Mvc.PluginsManager
```
or
```sh
PM>Install-Package Ege.AspNetCore.Mvc.PluginsManager
```

### Usage

Create Project as ``` Sdk="Microsoft.NET.Sdk.Razor" ``` or ``` Sdk="Microsoft.NET.Sdk.Web" ```
Choose build output path

Sample ``` Plugin.Tax``` plugin project in ``` Plugin.Tax.csproj``` :
```xml
<Project Sdk="Microsoft.NET.Sdk.Razor">
  <PropertyGroup>
    <TargetFramework>netcoreapp3.0</TargetFramework>
    <AddRazorSupportForMvc>true</AddRazorSupportForMvc>
  </PropertyGroup>
    ...
  <PropertyGroup Condition="'$(Configuration)|$(Platform)'=='Debug|AnyCPU'">
    <OutputPath>..\..\Applications\Application.WebMvc\Plugins\Plugin.Tax</OutputPath>
    <OutDir>$(OutputPath)</OutDir>
  </PropertyGroup>
</Project>
```

Add plugin info as json file ``` PluginInfo.json``` :

```json
{
  "Name": "Plugin.Widget",
  "File": "Plugin.Widget.dll",
  "Description": "Plugin Widget"
}
```
set File key as assembly name


Add ``` Setup.cs``` like Asp.net core project for plugin:
 ```csharp
using Ege.AspNetCore.Mvc.PluginsManager;
...
    internal class PluginSetup :IPluginSetup 
    {
        //for sorting between plugins middleware
        public int Order => 10;

        //your plugin ConfigureServices method
        public void ConfigureServices(IServiceCollection services)
        {
            //TODO:
        }
        //your plugin Configure method
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            //TODO:
        }
    }
...
```


Add .Net Core MVC Main Project and some general Configurations for plugins

on ``` Application.WebMvc.csproj``` include plugins dll in main project
```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
...
  <ItemGroup>
    <Folder Include="Plugins\" />
    <Content Remove="Plugins\**\Properties\*.json" />
    <Content Include="Plugins\**\*.dll">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </Content>
  </ItemGroup>
...
  <Target Name="CreatePluginFolder" AfterTargets="AfterPublish">
    <MakeDir Directories="$(PublishDir)Plugins" Condition="!Exists('$(PublishDir)Plugins')" />
  </Target>
...
</Project>
```

And configure ``` Startup.cs``` to load plugins
```csharp
using Ege.AspNetCore.Mvc.PluginsManager;
...

...
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            ...
            //your plugin root directory
            var pluginRootPath = Path.Combine(AppContext.BaseDirectory, "Plugins");
            //load plugins
            services.AddPlugins(pluginRootPath);
            ...
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            ...
            app.UsePluginsConfigure(env);
            ...
        }
...
```










