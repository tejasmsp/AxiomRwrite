using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Axiom.Web.Controllers
{
    public class ClientsController : Controller
    {
        // GET: Client
        [Route("Clients")]
        [Route("Client")]
        public ActionResult Index()
        {
            return Redirect("https://www.axiomcopyonline.com");
        }
    }
}