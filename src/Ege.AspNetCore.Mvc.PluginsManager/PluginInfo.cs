namespace Ege.AspNetCore.Mvc.PluginsManager
{
    /// <summary>
    /// Plugin PluginInfo.json file convert to PluginInfo class object
    /// </summary>
    public class PluginInfo
    {
        /// <summary>
        /// Plugin name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Plugin assembly name 
        /// Example: Plugin.Widget.dll
        /// </summary>
        public string File { get; set; }
        /// <summary>
        /// Plugin description
        /// </summary>
        public string Description { get; set; }
    }
}
