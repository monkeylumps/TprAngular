namespace Test
{
    using Microsoft.AspNet.Mvc;

    public class HomeController : Controller
    {
        public ViewResult Index()
        {
            return this.View("Index");
        }
    }
}
