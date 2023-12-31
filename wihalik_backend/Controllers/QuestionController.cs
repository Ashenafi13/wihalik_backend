﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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
        public IEnumerable<episode> GetEpisodes(int season_Id)
        {
            using (var db = new Wiha_likiEntities())
            {
                var ep = db.episodes.Where(x=>x.season_Id == season_Id).ToList();
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
                    return new active { season = query.name , episode = query2.name, episodeStartTime = query2.startTime, time_status = query2.timer_status} ;
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
        public IEnumerable<question> GetQuestions(int season, int episode)
        {
            using (var db = new Wiha_likiEntities())
            {
                var quest = db.questions.Where(x=>x.season_id == season && x.episode_id == episode).ToList();
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

        [Route("api/update/All/questions")]
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        public int updateQ(int QId, question data)
        {
            using(var db = new Wiha_likiEntities())
            {
                var q = db.questions.Where(x => x.id == QId).FirstOrDefault();
                if (q != null)
                {
                  if(q.status == 0)
                    {
                        q.question_label = data.question_label;
                        q.time = data.time;
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
                    return -1;
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
                    int? isQuestionAsked = checkQuestionAsked(ch.question_id);
                    int? isAnswerExist = checkAnswerExist(ch.question_id);
                    if (isQuestionAsked == 0)
                    {
                        if(data.isAnswer == 0)
                        {
                            ch.choice_label = data.choice_label;
                            ch.isAnswer = data.isAnswer;
                            ch.alphabet = data.alphabet;
                            db.SaveChanges();
                        }
                        else
                        {
                            if (isAnswerExist == 0 || isAnswerExist == id)
                            {
                                ch.choice_label = data.choice_label;
                                ch.isAnswer = data.isAnswer;
                                ch.alphabet = data.alphabet;
                                db.SaveChanges();
                            }
                            else
                            {
                                return 2;
                            }
                        }
                       
                        return 1;
                    }
                    else
                    {
                        return 3;
                    }
                   
                }
                else
                {
                    return 0;
                }
            }
        }

       

        [Route("api/add/startDate")]
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        public void updateStartDate(int QId)
        {
            using (var db = new Wiha_likiEntities())
            {
                var q = db.questions.Where(x => x.id == QId).FirstOrDefault();
                if(q != null)
                {
                    DateTime startDate = DateTime.Now;
                    DateTime endDate = DateTime.Now.AddSeconds(Convert.ToDouble(q.time));
                    q.startDate = startDate;
                    q.endDate = endDate;
                    db.SaveChanges();
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
                    question_id = data.question_id,
                    alphabet = data.alphabet
                };
                try
                {
                    int? isQuestionAsked = checkQuestionAsked(data.question_id);
                    if (isQuestionAsked == 0)
                    {
                        if(data.isAnswer == 0)
                        {
                            db.choices.Add(ch);
                            db.SaveChanges();
                            return 1;
                        }
                        else
                        {
                            int? isAnswerExist = checkAnswerExist(data.question_id);
                            if (isAnswerExist == 0)
                            {
                                db.choices.Add(ch);
                                db.SaveChanges();
                                return 1;
                            }
                            else
                            {
                                return 2;
                            }
                        }
                    
                    }
                    else
                    {
                        return 0;
                    }
                    
                }
                catch (Exception e)
                {
                    return 0;
                }
            }
        }

        public int? checkAnswerExist(int? QId)
        {
            using (var db = new Wiha_likiEntities())
            {
                var q = db.choices.Where(x => x.question_id == QId && x.isAnswer == 1).FirstOrDefault();
                if (q == null)
                {
                    return 0;
                }
                else
                {
                    return q.id;
                }
            }
        }
        public int? checkQuestionAsked(int? QId)
        {
            using(var db = new Wiha_likiEntities())
            {
                var q = db.questions.Where(x => x.id == QId).FirstOrDefault();
                if(q != null)
                {
                    return q.status;
                }
                else
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
                int count = GetNumOfQuestions(data.episode_id, data.season_id);
                int quNum = count == 0 ? 1 : count + 1;
                var questions = new question
                {
                    episode_id = data.episode_id,
                    question_label = data.question_label,
                    season_id = data.season_id,
                    time = data.time,
                    quNum = quNum,
                    status= 0
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

        public int GetNumOfQuestions(int? episode,int? season)
        {

          using(var db = new Wiha_likiEntities())
            {
                var count = db.questions.Where(x => x.episode_id == episode && x.season_id == season).ToList();
                return count.Count;
            }
        }

        // public async Task<PagedResult<register>> EpisodeWinners(int currentPage, int pageSize)
        //{

        //    using (var db = new Wiha_likiEntities())
        //    {

        //        // Calculate the number of items to skip
        //        int skip = (currentPage - 1) * pageSize;
        //        int totalQuesions = GetTotalNumberOfQuesionsPerEpisodes();
        //        // Query the database and get the total count
        //        var query = db.registers.Where(x => x.totalAnswered == totalQuesions).AsQueryable();

        //        int totalItems = await query.CountAsync();


        //        // Apply pagination to the query
        //        var pagedData = await query.Skip(skip).Take(pageSize).ToListAsync();


        //        // Return the result as a PagedResult
        //        return new PagedResult<register>
        //        {
        //            CurrentPage = currentPage,
        //            PageSize = pageSize,
        //            TotalItems = totalItems,
        //            Data = pagedData
        //        };

        //    }
        //}
     


        public int GetTotalNumberOfQuesionsPerEpisodes()
        {

            int season = GetActiveSeason();
            int episode = GetActivEpisodes();
            using (var db = new Wiha_likiEntities())
            {
                var qu = db.questions.Where(x => x.episode_id == episode && x.season_id == episode).ToList();

                if(qu.Count > 0)
                {
                    return qu.Count;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
    public class PagedResult<T>
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public IList<T> Data { get; set; }
    }
}