using Microsoft.AspNetCore.Mvc;
using Plugin.Tax.Models;
using System.Collections.Generic;
using System.Linq;

namespace Plugin.Tax.Controllers
{
    public class TaxController : Controller
    {
        private readonly List<TaxModel> taxs = new List<TaxModel>
        {
            new TaxModel{Name="x", Rate=18.00M},
            new TaxModel{Name="y", Rate=18.00M},
            new TaxModel{Name="z", Rate=18.00M}
        };

        public IActionResult Index(string id)
        {
            var model = taxs.FirstOrDefault(t => t.Name == id);
            if (model == null)
                return Content($@"not found {id} tax
                                <br/>    
                                <p>
                                    <a href=""/Home/Index""> Home Page</a>
                                </p>"
                , "text/html");

            return View(model);
        }
    }
}
