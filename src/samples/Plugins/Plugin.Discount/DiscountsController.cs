using Microsoft.AspNetCore.Mvc;

namespace Plugin.Discount
{
    public class DiscountsController : Controller
    {
        public IActionResult Index()
        {
            return Content(
                @"this Discounts Index 
                <br/>    
                <p>
                    <a href=""/Home/Index""> Home Page </a>
                </p>"
             ,"text/html");
        }
    }
}
