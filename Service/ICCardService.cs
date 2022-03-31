using System;
using System.Collections.Generic;
using System.Linq; 
using Newtonsoft.Json;

namespace Host_Service.Service
{
    public class ICCardService
    {
        private NHIICCBase icBase = new NHIICCBase();
        private ICCard_DllLibrary_Utility _icWriteReader = new ICCard_DllLibrary_Utility();

        public Dictionary<string, string> ReadICCardData()
        {
            try
            {
                CFormatICCardOutput icOuputR = new CFormatICCardOutput(); // 參考Reference

                icOuputR.CsGetVersionEx(1);
                var chk=_icWriteReader.NHIICCardStatusCheck(HIS2_CoreBase.Help.EnumUtility.ICCardType.HC);
                LOGDelegate.Instance.dbDebugMsg(JsonConvert.SerializeObject(chk), "ICCardRead");
                LOGDelegate.Instance.dbDebugMsg(icOuputR.DLL路徑, "ICCardRead");
                icOuputR = icBase.HisGetBasicData_CallFunc(); // 先呼叫 1.1
                icBase.CsCloseCom();
                Dictionary<string, string> keyValues = new Dictionary<string, string>();
                keyValues.Add("IDNO", icOuputR.身分證號或身分證明文件號碼);
                keyValues.Add("BIRTHDAY", icOuputR.出生日期);
                return keyValues;
            }
            catch (Exception ex)
            {
                throw ex;
            }
 
        }

        public Dictionary<bool, string> WriteICCardData(List<string> PK_ID,string IDNO,string PID,string IMPORT_MODE)
        {
            try
            {
                var WriteResult = new Dictionary<bool, string>();
                List<CURE_REC> orderList = (List<CURE_REC>)new DBhelp().QueryAsyncwithTimeoutTime<CURE_REC>("SELECT * FROM HIS2USER2.CURE_REC WHERE PK_ID=:PK_ID ", new { PK_ID = PK_ID }, timeoutSecs: 60).Result;
                List<QueryNHIPatientData> Querydata = new List<QueryNHIPatientData>();
                {
                    foreach (CURE_REC item in orderList)
                    {
                        Querydata.Add(new QueryNHIPatientData
                        {
                            ORDERCODE = item.CUREID,
                            FREQNO = item.FREQ,
                            SPOT = item.SPOT,
                            DAYS = item.DAYCOUNT != null ? (decimal.TryParse(item.DAYCOUNT?.ToString(), out var dayCount) ? dayCount : 0M) : 0M,
                            SALECOUNT = item.TOTALCOUNT != null ? (decimal.TryParse(item.TOTALCOUNT?.ToString(), out var totalCount) ? totalCount : 0M) : 0M,
                            ISSLOW = false,
                            IDNO = IDNO,
                            ISRXOUT = "Y",
                            StatusCheck_ComPort = false,
                            StatusCheck_DoctorCardPIN = false,
                            StatusCheck_NHIICCard = false,
                            ImportMode = (IMPORT_MODE == "1") ? QueryNHIPatientData.IMPORT_MODE_ENUM.WRITE : QueryNHIPatientData.IMPORT_MODE_ENUM.DELETE
                        });
                    }
                }

                QueryNHIPatientData QueryData = new QueryNHIPatientData()
                {
                    RID = orderList.First().RID,
                    MEDNO = PID,
                    IDNO = IDNO,
                    ReCardNote = 1,
                    ShowALertBeforeWriteCard = false,
                    USERID = orderList.First().DIAGDOCT, 
                    StatusCheck_ComPort = false,
                    StatusCheck_DoctorCardPIN = false,
                    StatusCheck_NHIICCard = false
                };
                var GetSeq256N1 = _icWriteReader.NHICCW_hisGetSeqNumber256N1(QueryData);
                if (!GetSeq256N1.Keys.FirstOrDefault())
                {
                    if (GetSeq256N1.Values.FirstOrDefault().Contains("不能重複產生就醫識別碼和就醫序號"))
                    {
                        WriteResult.Add(false, "不能重複產生就醫識別碼和就醫序號");
                        return WriteResult;
                    }
                    else
                    {
                        WriteResult.Add(false, $"就醫識別碼和就醫序號產生失敗!\r\n錯誤訊息:{GetSeq256N1.Values.FirstOrDefault()}");
                        return WriteResult;
                    }
                } 
                WriteResult = _icWriteReader.NHICCW_hisWriteMultiPrescriptionSign(Querydata); 
                return WriteResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
