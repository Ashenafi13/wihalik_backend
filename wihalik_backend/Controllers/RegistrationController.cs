﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using wihalik_backend.Models;
namespace wihalik_backend.Controllers
{
    public class RegistrationController : ApiController
    {

        [Route("api/update/start/end/date")]
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        public int updateEpisode()
        {
            using(var db = new Wiha_likiEntities())
            {
                var ep = db.episodes.Where(x => x.status == 1).FirstOrDefault();
                if(ep != null)
                {
                    DateTime startTime = DateTime.Now;
                    DateTime endTime = startTime.AddMinutes(Convert.ToDouble(ep.startTime));
                    ep.startedDateTime = startTime;
                    ep.endedDateTime = endTime;
                    ep.timer_status = 1;
                    db.SaveChanges();
                    return 1;

                }
                else
                {
                    return 0;
                }
            }
        }

        [Route("api/add/registor")]
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        public int registor()
        {
            try
            {
                DateTime? startTime = GetStartDate();
                DateTime? endTime = GetEndDate();
                using (var db = new EBCSMSEntities())
                {
                    var smsQuery = db.Database.SqlQuery<ozekimessagein>("SELECT * FROM [EBCSMS].[dbo].[ozekimessagein] WHERE receiver='+800' AND receivedtime BETWEEN '" + startTime + "' AND '" + endTime + "'").ToList();
                    if (smsQuery.Count > 0)
                    {
                        smsQuery.ForEach((sms) =>
                        {
                            Addregistor(sms.sender);
                        });

                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        [Route("api/get/sms/all")]
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        public IEnumerable<dynamic> GetSMS()
        {
            using(var db = new EBCSMSEntities())
            {
                var ebcsms = db.ozekimessageins.ToList();
                return ebcsms;
            }
        }
        public DateTime? GetStartDate()
        {
            using (var db = new Wiha_likiEntities())
            {
                var ep = db.episodes.Where(x => x.status == 1).FirstOrDefault();
                if (ep != null)
                {
                    return ep.startedDateTime;
                }
                else
                {
                    return null;
                }
            }
        }
        public DateTime? GetEndDate()
        {
            using (var db = new Wiha_likiEntities())
            {
                var ep = db.episodes.Where(x => x.status == 1).FirstOrDefault();
                if (ep!= null)
                {
                    return ep.endedDateTime;
                }
                else
                {
                    return null;
                }
            }
        }

        

        public void Addregistor(string phone)
        {
            int season = GetActiveSeason();
            int episode = GetActivEpisodes();
            using (var db = new Wiha_likiEntities())
            {
                try
                {
                    var req = new register
                    {
                        phone = phone,
                        totalAnswered = 0,
                        episode_id = episode,
                        season_id = season
                    };

                    int isExist = checkExist(phone);
                    if (isExist == 0)
                    {
                        db.registers.Add(req);
                        db.SaveChanges();
                    }
                }catch(Exception e)
                {

                }
               
            }
        }
        public int GetActiveSeason()
        {
            using (var db = new Wiha_likiEntities())
            {
                var query = db.seasons.Where(x => x.status == 1).FirstOrDefault();

                if (query != null)
                {
                    return query.id;
                }
                else
                {
                    return 0;
                }
            }
        }

        public int GetActivEpisodes()
        {
            using (var db = new Wiha_likiEntities())
            {
                var query = db.episodes.Where(x => x.status == 1).FirstOrDefault();
                if (query != null)
                {
                    return query.id;
                }
                else
                {
                    return 0;
                }
            }
        }
        public int checkExist(string phone)
        {
            int season = GetActiveSeason();
            int episode = GetActivEpisodes();
            using (var db = new Wiha_likiEntities())
            {
                var reg = db.registers.Where(x => x.phone.Equals(phone) && x.season_id == season && x.episode_id == episode).FirstOrDefault();
                if(reg != null)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }


        [Route("api/get/registers")]
        [System.Web.Http.AcceptVerbs("Get")]
        [System.Web.Http.HttpGet]
        public IEnumerable<register> GetRegisters()
        {
            int season = GetActiveSeason();
            int episode = GetActivEpisodes();
            using (var db = new Wiha_likiEntities())
            {
                var req = db.registers.Where(x=>x.episode_id == episode && x.season_id == season).ToList();
                return req;
            }
        }


    }
}