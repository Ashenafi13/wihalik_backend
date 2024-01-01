using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using wihalik_backend.Models;

namespace wihalik_backend.Controllers
{
    public class TimeTrackerController : ApiController
    {
       
        [Route("api/reg/time/tracker")]
        [HttpGet]
        public dynamic GetTime()
        {
            using(var db = new Wiha_likiEntities())
            {
                var time = db.reg_time_tracker.FirstOrDefault();
                return time;
            }
        }

        [Route("api/update/time")]
        [HttpPut]
        public int updateTime(reg_time_tracker data)
        {
            using (var db = new Wiha_likiEntities())
            {
                int id = checkDataExist();
                if (id != 0)
                {
                    var time = db.reg_time_tracker.Where(x => x.id == id).FirstOrDefault();
                    if (time != null)
                    {
                        time.second = data.second;
                        time.min = data.min;
                        db.SaveChanges();
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    var time = new reg_time_tracker
                    {
                        min = data.min,
                        second = data.second
                    };
                    db.reg_time_tracker.Add(time);
                    db.SaveChanges();
                    return 1;

                }
            }

        }

        private int checkDataExist()
        {
            using (var db = new Wiha_likiEntities())
            {
                var time = db.reg_time_tracker.FirstOrDefault();
                if(time != null)
                {
                    return time.id;
                }
                else
                {
                    return 0;
                }
            }
        }



        [Route("api/question/time/tracker")]
        [HttpGet]
        public dynamic GetQuestionTime()
        {
            using (var db = new Wiha_likiEntities())
            {
                var time = db.question_time_tracker.FirstOrDefault();
                return time;
            }
        }

        [Route("api/update/question/time")]
        [HttpPut]
        public int updateQuestionTime(question_time_tracker data)
        {
            using (var db = new Wiha_likiEntities())
            {
                int id = checkQuestionTimeExist();
                if (id != 0)
                {
                    var time = db.question_time_tracker.Where(x => x.id == id).FirstOrDefault();
                    if (time != null)
                    {
                        time.q_percent = data.q_percent;
                        time.question = data.question;
                        time.second = data.second;
                        
                        db.SaveChanges();
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    var time = new question_time_tracker
                    {
                        q_percent= data.q_percent,
                        question =data.question,
                        second = data.second
                };
                    db.question_time_tracker.Add(time);
                    db.SaveChanges();
                    return 1;

                }
            }

        }

        private int checkQuestionTimeExist()
        {
            using (var db = new Wiha_likiEntities())
            {
                var time = db.question_time_tracker.FirstOrDefault();
                if (time != null)
                {
                    return time.id;
                }
                else
                {
                    return 0;
                }
            }
        }

    }
}