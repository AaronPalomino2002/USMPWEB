// ViewComponents/BreadcrumbViewComponent.cs
using Microsoft.AspNetCore.Mvc;

namespace USMPWEB.ViewComponents
{
    public class BreadcrumbViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var controller = ViewContext.RouteData.Values["controller"].ToString();
            var action = ViewContext.RouteData.Values["action"].ToString();
            
            var breadcrumb = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "INICIO", Url = "/", Active = false }
            };

            switch (controller.ToLower())
            {
                case "home":
                    breadcrumb.Add(new BreadcrumbItem { Text = "CAMPAÑAS", Active = true });
                    break;
                case "certificados":
                    breadcrumb.Add(new BreadcrumbItem { Text = "CERTIFICADOS", Active = true });
                    break;
                case "inscripciones":
                    breadcrumb.Add(new BreadcrumbItem { Text = "INSCRIPCIONES", Active = true });
                    break;
                case "contacto":
                    breadcrumb.Add(new BreadcrumbItem { Text = "CONTACTO", Active = true });
                    break;
            }

            return View(breadcrumb);
        }
    }

    public class BreadcrumbItem
    {
        public string Text { get; set; }
        public string Url { get; set; }
        public bool Active { get; set; }
    }
}