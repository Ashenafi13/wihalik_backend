using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using wihalik_backend.Models;

namespace wihalik_backend.Controllers
{
    public class ReportsController : ApiController
    {
        
        [Route("api/report/Competitor")]
        [HttpGet]
        public dynamic GetCompetitor(int season,int episode)
        {
            using(var db = new Wiha_likiEntities())
            {
                var competitor = db.registers.Where(x => x.season_id == season && x.episode_id == episode).ToList();
                return competitor;
            }
        }
      

        [Route("api/report/Competitor/details")]
        [HttpGet]
        public dynamic GetCompetitorDetails(int season, int episode, string phone)
        {
            using (var db = new Wiha_likiEntities())
            {
                string[] words = phone.Split(' ');
                string phoneNumber = "+"+words[1];
                var competitor = (from mapped in db.answer_sms_mapping
                                  join q in db.questions on mapped.qu_Id equals q.id                      
                                  where mapped.phone.Equals(phoneNumber) && q.season_id == season && q.episode_id == episode
                                  select new { mapped, q}
                                  ).ToList();
                return competitor;
            }
        }

        [Route("api/report/questions/choices")]
        [HttpGet]
        public dynamic Getchoices()
        {
            using (var db = new Wiha_likiEntities())
            {
                var ch = db.choices.ToList();
                return ch;
            }
        }
    }
}