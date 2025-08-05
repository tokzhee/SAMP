using System.Web.Mvc;

namespace App.Web.Controllers
{
    [RoutePrefix("error")]
    public class ErrorController : BaseController
    {
        #region ActionResult

        [Route("not-found")]
        public ActionResult NotFound()
        {
            return View();
        }
        [Route("internal-server-error")]
        public ActionResult InternalServerError()
        {
            return View();
        }
        [Route("unauthorised-access")]
        public ActionResult Unauthorised()
        {
            return View();
        }

        #endregion
    }
}