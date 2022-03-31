using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http; 

namespace Host_Service.Controller
{
    [RoutePrefix("api/Print")]
    public class PrintController : ApiController
    {
        [Route("rpt")]
        [HttpGet]
        public HttpResponseMessage Print()
        {
            try
            {
                Dictionary<string, string> parameters = Request.GetQueryNameValuePairs().Cast<KeyValuePair<string, string>>().ToDictionary(k => k.Key, v => HttpUtility.UrlDecode(v.Value));
                //new ReportPrinter(parameters).print();
                return Request.CreateResponse(HttpStatusCode.OK, "OK", Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                LOGDelegate.Instance.dbErrorMsg(ex, "Report", ex.ToString());
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed, new { Message = ex.ToString() }, Configuration.Formatters.JsonFormatter);
            }
        }
    }
}
