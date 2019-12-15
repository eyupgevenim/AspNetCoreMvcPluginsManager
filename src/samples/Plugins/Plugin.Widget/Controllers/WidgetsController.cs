using Microsoft.AspNetCore.Mvc;
using Plugin.Widget.Models;
using System.Linq;
using System;
using System.Collections.Generic;

namespace Plugin.Widget.Controllers
{
    [Area("Plugin")]
    public class WidgetsController : Controller
    {
        private readonly List<WidgetModel> Widgets = new List<WidgetModel>
        {
            new WidgetModel{Name="a", Body="widget-a body"},
            new WidgetModel{Name="b", Body="widget-b body"},
            new WidgetModel{Name="c", Body="widget-c body"}
        };

        public IActionResult Info(string id)
        {
            var model = Widgets.FirstOrDefault(w => w.Name.Equals(id, StringComparison.OrdinalIgnoreCase));
            if (model == null)
                return Content($"not fuand {id}");

            return View(model);
            //return View("~/Views/Widgets/Info.cshtml", model);
        }
    }
}
