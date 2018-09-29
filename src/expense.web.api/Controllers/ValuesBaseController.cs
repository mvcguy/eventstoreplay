using Microsoft.AspNetCore.Mvc;

namespace expense.web.api.Controllers
{
    public class ValuesBaseController : ControllerBase
    {
        protected object AdaptModelState()
        {
            return null;
        }
    }
}