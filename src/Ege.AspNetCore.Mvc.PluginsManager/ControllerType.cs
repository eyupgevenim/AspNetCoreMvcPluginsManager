using System;

namespace Ege.AspNetCore.Mvc.PluginsManager
{
    /// <summary>
    /// Plugin controller types
    /// </summary>
    [Obsolete("ControllerType will be removed in the next version.")]
    public enum ControllerType
    {
        /// <summary>
        /// Not include plugin controller
        /// </summary>
        None = 0,
        /// <summary>
        /// Include plugin controllers
        /// </summary>
        Internal = 1,
    }
}
