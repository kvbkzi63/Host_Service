using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HIS2_Host_Service_Formal.Service;
using Newtonsoft.Json;

namespace Host_Service.Controller
{
    [RoutePrefix("api/ICCard")]
    public class ICCardController : ApiController
    {
        [Route("CardRead")]
        [HttpGet]
        public HttpResponseMessage ICCardRead()
        {
            try
            {
                Dictionary<string, string> ptInfo = new ICCardService().ReadICCardData();
                LOGDelegate.Instance.dbDebugMsg(JsonConvert.SerializeObject(ptInfo), "ICCardRead");
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(ptInfo), Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                LOGDelegate.Instance.dbDebugMsg(ex.ToString(), "ICCardRead"); 
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed, new { Message = "讀卡失敗，請檢查IC卡讀卡機是否正常。" }, Configuration.Formatters.JsonFormatter);

            }
        }

        [HttpPost]
        [Route("CardWrite")] 
        public HttpResponseMessage ICCardWrite(WriteData data)
        {
            try
            { 
                LOGDelegate.Instance.dbDebugMsg(JsonConvert.SerializeObject(data),"ICCardWrite");
                var result = new ICCardService().WriteICCardData(data.PK_ID, data.Idno, data.PID, data.IMPORT_MODE);
                return Request.CreateResponse(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                LOGDelegate.Instance.dbDebugMsg(ex.ToString(), "ICCardWrite"); 
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed, new { Message = "寫入IC卡失敗，請檢查IC卡讀卡機是否正常。" }, Configuration.Formatters.JsonFormatter);

            }
        }

        public class WriteData
        {
            public string Idno { get; set; }
            public string PID { get; set; }
            public string IMPORT_MODE { get; set; }
            public List<string> PK_ID { get; set; } 
        }
    }
}
