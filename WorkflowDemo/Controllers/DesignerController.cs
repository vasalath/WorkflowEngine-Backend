using System.Collections.Specialized;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using OptimaJet.Workflow;
using WorkflowLib;

namespace WorkflowDemo.Controllers
{
    public class DesignerController : Controller
    {
        public IActionResult API()
        {
            Stream filestream = null;
            var isPost = Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase);
            if (isPost && Request.Form.Files != null && Request.Form.Files.Count > 0)
                filestream = Request.Form.Files[0].OpenReadStream();

            var pars = new NameValueCollection();
            foreach (var q in Request.Query)
            {
                pars.Add(q.Key, q.Value.First());
            }

            if (isPost)
            {
                var parsKeys = pars.AllKeys;

                foreach (var key in Request.Form.Keys)
                {
                    if (!parsKeys.Contains(key))
                    {
                        pars.Add(key, Request.Form[key]);
                    }
                }
            }

            var res = WorkflowInit.Runtime.DesignerAPI(pars, out bool hasError, filestream, true);

            if (pars["operation"].ToLower() == "downloadscheme" && !hasError)
                return File(Encoding.UTF8.GetBytes(res), "text/xml");

            if (pars["operation"].ToLower() == "downloadschemebpmn" && !hasError)
                return File(Encoding.UTF8.GetBytes(res), "text/xml");

            return Content(res);
        }
    }
}
