using UmbracoV16.Core.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace UmbracoV16.Core.Components
{
    public class BrocadeCoreSeriesViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(BrocadeSwitchViewModel model)
        {
            return View(model);
        }
    }
}