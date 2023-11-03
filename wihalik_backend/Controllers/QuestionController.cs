﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using wihalik_backend.Models;

namespace wihalik_backend.Controllers
{
    public class QuestionController : ApiController
    {
        [Route("api/seasons")]
        [System.Web.Http.AcceptVerbs("GET")]
        [System.Web.Http.HttpGet]
        public IEnumerable<season> GetSeasons()
        {
            using(var db = new Wiha_likiEntities())
            {
                var se = db.seasons.ToList();
                return se;
            }
        }

        [Route("api/episodes")]
        [System.Web.Http.AcceptVerbs("GET")]
        [System.Web.Http.HttpGet]
        public IEnumerable<episode> GetEpisodes()
        {
            using (var db = new Wiha_likiEntities())
            {
                var ep = db.episodes.ToList();
                return ep;
            }
        }

        [Route("api/episode/questions")]
        [System.Web.Http.AcceptVerbs("GET")]
        [System.Web.Http.HttpGet]
        public IEnumerable<dynamic> GetEpisodeQuestions()
        {
            using (var db = new Wiha_likiEntities())
            {
                int season = GetActiveSeason();
                int episode = GetActivEpisodes();
                var query = (from qts in db.questions
                               join cho in db.choices on qts.id equals cho.question_id 
                               where qts.season_id == season && qts.episode_id == episode
                               select new { qts, cho}
                              ).ToList();
               
                return query;
            }
        }

        [Route("api/questions/status")]
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        public int updateQuestionsStatus(int QId)
        {
            using (var db = new Wiha_likiEntities())
            {
                var qu = db.questions.Where(x => x.id == QId).FirstOrDefault();
                if(qu != null)
                {
                    qu.status = 1;
                    db.SaveChanges();
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }


        [Route("api/active/season")]
        [System.Web.Http.AcceptVerbs("GET")]
        [System.Web.Http.HttpGet]
        public active GetActive()
        {
            using (var db = new Wiha_likiEntities())
            {
                var query = db.seasons.Where(x => x.status == 1).FirstOrDefault();
                var query2 = db.episodes.Where(x => x.status == 1).FirstOrDefault();
                if (query != null && query2 != null)
                {
                    return new active { season = query.name , episode = query2.name} ;
                }
                else
                {
                    return null;
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
        [Route("api/questions")]
        [System.Web.Http.AcceptVerbs("GET")]
        [System.Web.Http.HttpGet]
        public IEnumerable<question> GetQuestions()
        {
            using (var db = new Wiha_likiEntities())
            {
                var quest = db.questions.ToList();
                return quest;
            }
        }

        [Route("api/update/questions")]
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        public int UpdateQuestions(question data, int id)
        {
            using (var db = new Wiha_likiEntities())
            {
                var quest = db.questions.Where(x => x.id == id).FirstOrDefault();
                if(quest != null)
                {
                    quest.question_label = data.question_label;
                    quest.time = data.time;
                    db.SaveChanges();
                    return 1;
                }
                else
                {
                    return 0;
                }
               
            }
        }

        [Route("api/delete/questions")]
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        public int DeleteQuestions(int id)
        {
            using (var db = new Wiha_likiEntities())
            {
                var quest = db.questions.Where(x => x.id == id).FirstOrDefault();
                if (quest != null)
                {
                    db.questions.Remove(quest);
                    DeleteChoices(id);
                    db.SaveChanges();
                    return 1;
                }
                else
                {
                    return 0;
                }

            }
        }

        [Route("api/choices")]
        [System.Web.Http.AcceptVerbs("GET")]
        [System.Web.Http.HttpGet]
        public IEnumerable<choice> GetChoices(int QId)
        {
            using (var db = new Wiha_likiEntities())
            {
                var query = db.choices.Where(x=>x.question_id==QId).ToList();
                return query;
            }
        }

        [Route("api/delete/choices")]
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        public int DeleteChoices(int id)
        {
            using (var db = new Wiha_likiEntities())
            {
                var ch = db.choices.Where(x => x.id == id).FirstOrDefault();
                if (ch != null)
                {
                    db.choices.Remove(ch);
                    db.SaveChanges();
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }


        [Route("api/update/choices")]
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        public int UpdateChoices(choice data,int id)
        {
            using (var db = new Wiha_likiEntities())
            {
                var ch = db.choices.Where(x => x.id == id).FirstOrDefault();
                if(ch != null)
                {
                    ch.choice_label = data.choice_label;
                    ch.isAnswer = data.isAnswer;
                    db.SaveChanges();
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

            [Route("api/add/choices")]
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        public int AddChoices(choice data)
        {
            using (var db = new Wiha_likiEntities())
            {
                var ch = new choice
                {
                    choice_label = data.choice_label,
                    isAnswer = data.isAnswer,
                    question_id = data.question_id
                };
                try
                {
                    db.choices.Add(ch);
                    db.SaveChanges();
                    return 1;
                }
                catch (Exception e)
                {
                    return 0;
                }
            }
        }

            [Route("api/add/questions")]
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        public int AddQuestions(question data)
        {
            using (var db = new Wiha_likiEntities())
            {
                var questions = new question
                {
                    episode_id = data.episode_id,
                    question_label = data.question_label,
                    season_id = data.season_id,
                    time = data.time
                };

                try
                {
                    db.questions.Add(questions);
                    db.SaveChanges();
                    return 1;

                }catch(Exception e)
                {
                    return 0;
                }



            }
        }
    }
}