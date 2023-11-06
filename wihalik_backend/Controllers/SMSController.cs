using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using wihalik_backend.Models;
namespace wihalik_backend.Controllers
{
    public class SMSController : ApiController
    {

        [Route("api/add/Competitor")]
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        public int AddCompetitor(int QId)
        {
            DateTime? startDate = GetStartDate(QId);
            DateTime? endDate = GetEndDate(QId);

            int result = 0;
            using(var db = new EBCSMSEntities())
            {
                try
                {
                    var query = (from d in db.ozekimessageins
                                 where d.receivedtime >= startDate && d.receivedtime <= endDate
                                 select d).ToList();

                    query.ForEach((sms) =>
                    {
                        //bool isCompetitorRegistered = checkCompetitorRegistered(sms.sender);
                        bool isCompetitorSendSMSOnce = checkIfCompetitorSendSMSOnce(startDate, endDate, sms.sender);
                        bool isAnswerCorrect = checkAnswer(sms.msg, QId);

                        //if (isCompetitorRegistered)
                        //{
                            if (isCompetitorSendSMSOnce)
                            {
                                if (isAnswerCorrect)
                                {
                                    insertCompetitor(1, sms.msg, sms.sender, QId);
                                    updateCompetitorTotalAnswerd(sms.sender);



                                }
                                else
                                {
                                    insertCompetitor(0, sms.msg, sms.sender, QId);
                                   
                                }
                            }
                        //}


                    });
                    result = 1;

                }
                catch(Exception e)
                {
                    result = 0;
                }

                return result;
            }
        }

        public void updateCompetitorTotalAnswerd(string phone)
        {
            int? totalAnswerdByCompetitor = GetTotalAnswerdByCompetitor(phone);
            using (var db = new Wiha_likiEntities())
            {
                var comp = db.registers.Where(x => x.phone.Equals(phone)).FirstOrDefault();
                if (comp != null)
                {
                    comp.totalAnswered = totalAnswerdByCompetitor + 1;
                    db.SaveChanges();
                }
            }
        }

        public int? GetTotalAnswerdByCompetitor(string phone)
        {
            using(var db = new Wiha_likiEntities())
            {
                var comp = db.registers.Where(x => x.phone.Equals(phone)).FirstOrDefault();
                if(comp != null)
                {
                    if(comp.totalAnswered != null)
                    {
                        return comp.totalAnswered;
                    }
                    else
                    {
                        return 0;
                    }
                    
                }
                else
                {
                    return 0;
                }
            }
        }

        public DateTime? GetStartDate(int QId)
        {
            using(var db = new Wiha_likiEntities())
            {
                var q = db.questions.Where(x => x.id == QId).FirstOrDefault();
                if(q != null)
                {
                    return q.startDate;
                }
                else
                {
                    return null;
                }
            }
        }
        public DateTime? GetEndDate(int QId)
        {
            using (var db = new Wiha_likiEntities())
            {
                var q = db.questions.Where(x => x.id == QId).FirstOrDefault();
                if (q != null)
                {
                    return q.endDate;
                }
                else
                {
                    return null;
                }
            }
        }


        [Route("api/send/sms")]
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        public void SendSMS(ozekimessagein data)
        {
            try
            {
                using (var db = new EBCSMSEntities())
                {

                    var obj = new ozekimessagein
                    {
                        receiver = "+800",
                        sender = data.sender,
                        msg = data.msg,
                        senttime = DateTime.Now,
                        receivedtime = DateTime.Now,
                        @operator = "ETH-MTN",
                        msgtype = "SMS:TEXT"

                    };

                    db.ozekimessageins.Add(obj);
                    db.SaveChanges();


                }
            }catch(Exception w)
            {

            }

        }




        public bool checkIfCompetitorSendSMSOnce(DateTime? startDate, DateTime? endDate, string phone)
        {
            using (var db = new EBCSMSEntities())
            {
                var query = (from d in db.ozekimessageins
                             where d.receivedtime >= startDate && d.receivedtime <= endDate
                             select d).Where(x=>x.sender.Equals(phone)).ToList();
                if(query.Count() > 1)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public int insertCompetitor(int answer,string msg,string phone,int QId)
        {
            using (var db = new Wiha_likiEntities())
            {
                var data = new answer_sms_mapping
                {
                    answer = answer,
                    msg = msg,
                    phone = phone,
                    qu_Id = QId,
                    regDate = DateTime.Now
                };

                db.answer_sms_mapping.Add(data);
                db.SaveChanges();
                return 1;
            }

        }


        public bool checkCompetitorRegistered(string phone)
        {
            using (var db = new Wiha_likiEntities())
            {
                var query = db.registers.Where(x => x.phone.Equals(phone)).FirstOrDefault();
                if(query != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        public bool checkAnswer(string answer, int QId)
        {
            using(var db = new Wiha_likiEntities())
            {
                var query = db.choices.Where(x => x.question_id == QId && x.isAnswer == 1).FirstOrDefault();
                if(query != null)
                {
                    if(answer.Equals(query.alphabet, StringComparison.Ordinal))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

                
            }

        }


        [Route("api/Competitor")]
        [System.Web.Http.AcceptVerbs("GET")]
        [System.Web.Http.HttpGet]
        public IEnumerable<answer_sms_mapping> GetAnswer_Sms(int QId)
        {

            using(var db = new Wiha_likiEntities())
            {
                var list = db.answer_sms_mapping.Where(x => x.qu_Id == QId).ToList();
                return list;
            }
        }
        [Route("api/Competitor/rank")]
        [System.Web.Http.AcceptVerbs("GET")]
        [System.Web.Http.HttpGet]
        public IEnumerable<ranks> GetRank()
        {
            using (var db = new Wiha_likiEntities())
            {
                var rank = db.Database.SqlQuery<ranks>("SELECT c.*, DENSE_RANK() OVER (ORDER BY totalAnswered DESC) AS Rank FROM register  c").ToList();
                return rank;
            }
        }
    }
}