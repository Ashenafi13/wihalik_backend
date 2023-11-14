using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using wihalik_backend.Models;
namespace wihalik_backend.Controllers
{
    public class SeasonsController : ApiController
    {
        [Route("api/list/season/episodes")]
        [System.Web.Http.AcceptVerbs("GET")]
        [System.Web.Http.HttpGet]
        public IEnumerable<episode> GetAllEpisodes(int season_Id)
        {
            using(var db = new Wiha_likiEntities())
            {
                var eps = db.episodes.Where(x=>x.season_Id == season_Id).ToList();
                return eps;
            }
        }



        [Route("api/all/season")]
        [System.Web.Http.AcceptVerbs("GET")]
        [System.Web.Http.HttpGet]
        public ArrayList GetSeasons()
        {
            var list = new ArrayList();
            using (var db = new Wiha_likiEntities())
            {
                var sp = db.seasons.ToList();
                sp.ForEach((s) =>
                {
                    double numberOfEpisodes = TotalNumberOfEpisodes(s.id);
                    list.Add(new season_settings {
                         season = s.name,
                         SId = s.id,
                          status = s.status,
                         numberOfEpisodes = numberOfEpisodes
                    });
                });

                return list;
            }
        }

        [Route("api/season/add")]
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        public int AddSeason(season data)
        {
            using(var db = new Wiha_likiEntities())
            {
                var sp = new season
                {
                     name = data.name,
                     status = 0
                };
                int exist = checkSeasonExist(data.name);
                if (exist == 0)
                {
                    db.seasons.Add(sp);
                    db.SaveChanges();
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }



        public int checkSeasonExist(string name)
        {
            using (var db = new Wiha_likiEntities())
            {
                var sp = db.seasons.Where(x => x.name.Equals(name)).FirstOrDefault();
                if(sp != null)
                {
                    return 1;

                }
                else
                {
                    return 0;
                }
            }
        }

        [Route("api/episode/add")]
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        public int AddEpisodes(episode data)
        {
            using (var db = new Wiha_likiEntities())
            {
                var ep = new episode
                {
                    name = data.name,
                    startTime = data.startTime,
                    season_Id = data.season_Id,
                    status = 0,
                    timer_status = 0
                };
                int exist = checkEpisodes(data.name, data.season_Id);
                if (exist == 0)
                {

                    db.episodes.Add(ep);
                    db.SaveChanges();
                    return 1;
                }
                else
                {
                    return 0;
                }

            }
        }

        [Route("api/episode/update")]
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        public int updateEpisode(int Id,episode data)
        {
            using (var db = new Wiha_likiEntities())
            {
                var ep = db.episodes.Where(x => x.id == Id).FirstOrDefault();
                if(ep != null)
                {
                    if (!ep.name.Equals(data.name))
                    {
                        int exist = checkEpisodes(data.name, data.season_Id);
                        if (exist == 0)
                        {
                            ep.name = data.name;
                            ep.startTime = data.startTime;
                            db.SaveChanges();
                            return 1;
                        }
                        else
                        {
                            return 2;
                        }
                    }
                    else
                    {
                        ep.name = data.name;
                        ep.startTime = data.startTime;
                        db.SaveChanges();
                        return 1;
                    }
                }
                else
                {
                    return 0;
                }
            }

        }


        [Route("api/season/update")]
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        public int updateSeason(int season_Id, season data)
        {
            using (var db = new Wiha_likiEntities())
            {
              
                var sp = db.seasons.Where(x => x.id == season_Id).FirstOrDefault();
                if(sp != null)
                {
                    if (!sp.name.Equals(data.name))
                    {
                        int checkName = checkSeasonName(data.name);
                        if (checkName == 0)
                        {
                            sp.name = data.name;
                            db.SaveChanges();
                            return 1;
                        }
                        else
                        {
                            return 2;
                        }
                    }
                    else
                    {
                        sp.name = data.name;
                        db.SaveChanges();
                        return 1;
                    }
                }
                else
                {
                    return 0;
                }
            }
        }

        public int checkSeasonName(string name)
        {
            using (var db = new Wiha_likiEntities())
            {
                var sp = db.seasons.Where(x => x.name.Equals(name)).FirstOrDefault();
                if (sp != null)
                {

                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        [Route("api/season/episode/activate")]
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        public int ActivateSeason(int episode_Id, int season_id)
        {
            using (var db = new Wiha_likiEntities())
            {
                int deactiveEp = deactiveEpisode();
                if(deactiveEp == 1)
                {
                    var ep = db.episodes.Where(x => x.id == episode_Id).FirstOrDefault();
                    if(ep != null)
                    {
                        ep.status = 1;
                        activaeSeason(season_id);
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
                    return 0;
                }
               
            }
        }
        public void activaeSeason(int season_Id)
        {
            using (var db = new Wiha_likiEntities())
            {
                int deactive = deactiveSeason();
                if (deactive == 1)
                {
                    var sp = db.seasons.Where(x => x.id == season_Id).FirstOrDefault();
                    if (sp != null)
                    {
                        sp.status = 1;
                        db.SaveChanges();

                    }
                }
            }
        }

        public int deactiveSeason()
        {
            using (var db = new Wiha_likiEntities())
            {
                var sp = db.seasons.Where(x => x.status == 1).FirstOrDefault();
                if(sp != null)
                {
                    sp.status = 1;
                    db.SaveChanges();
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }
        public int deactiveEpisode()
        {
            using (var db = new Wiha_likiEntities())
            {
                var ep = db.episodes.Where(x => x.status == 1).FirstOrDefault();
                if(ep != null)
                {
                    ep.status = 0;
                    db.SaveChanges();
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        public int checkEpisodes(string name, int? season_Id)
        {
            using (var db = new Wiha_likiEntities())
            {
                var ep = db.episodes.Where(x => x.name == name && x.season_Id == season_Id).FirstOrDefault();
                if(ep != null)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        [Route("api/season/numbers")]
        [System.Web.Http.AcceptVerbs("GET")]
        [System.Web.Http.HttpGet]
        public double TotalNumberOfEpisodes(int season_Id)
        {
            using (var db = new Wiha_likiEntities())
            {
                var sp = db.episodes.Where(x=>x.season_Id == season_Id).ToList();
                if (sp != null)
                {
                    return sp.Count();
                }
                else
                {
                    return 0;
                }
            }
        }

    }
}
