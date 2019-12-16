namespace Ege.AspNetCore.Mvc.PluginsManager
{
    /// <summary>
    /// Plugin views access types
    /// </summary>
    public enum ViewsType
    {
        /// <summary>
        /// Not inculede .cshtml view in plugin
        /// </summary>
        None = 0,
        /// <summary>
        /// Include .cshtml views in plugin to access
        /// </summary>
        Internal = 1,
        /// <summary>
        /// Include .cshtml views outsite plugin to access
        /// </summary>
        External = 2,
    }
}
