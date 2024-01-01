using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
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
            int season = GetActiveSeason();
            int episode = GetActivEpisodes();
            int result = 0;
            using (var ddb = new Wiha_likiEntities())
            {
                using (var db = new EBCSMSEntities())
                {
                    try
                    {

                        var registored = ddb.registers.Where(x=>x.episode_id == episode && x.season_id == season).ToList();

                        for (int i = 0; i < registored.Count(); i++)
                        {
                            var sms = db.Database.SqlQuery<ozekimessagein>("SELECT * FROM [EBCSMS].[dbo].[ozekimessagein] WHERE receiver='+800' AND receivedtime BETWEEN '" + startDate + "' AND '" + endDate + "' AND sender='" + registored[i].phone + "'").FirstOrDefault();
                            if (sms != null)
                            {

                                bool isAnswerCorrect = checkAnswer(sms.msg, QId);
                                if (isAnswerCorrect == true)
                                { 
                                    insertCompetitor(1, sms.msg, sms.sender, QId);
                                  

                                }
                                else
                                {
                                    insertCompetitor(0, sms.msg, sms.sender, QId);

                                }

                            }


                        }



                    }
                    catch (Exception e)
                    {
                        result = 0;
                    }

                    return result;
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
        public void updateCompetitorTotalAnswerd(string phone,int QId)
        {
            try
            {
                int season = GetActiveSeason();
                int episode = GetActivEpisodes();
                int? totalAnswerdByCompetitor = GetTotalAnswerdByCompetitor(phone);
                using (var db = new Wiha_likiEntities())
                {
                    var comp = db.registers.Where(x => x.phone.Equals(phone) && x.season_id == season && x.episode_id == episode).FirstOrDefault();
                    var qu = db.questions.Where(x => x.id == QId).FirstOrDefault();
                    if (comp != null)
                    {
                        comp.totalAnswered = totalAnswerdByCompetitor + 1;
                        db.SaveChanges();

                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public int? GetTotalAnswerdByCompetitor(string phone)
        {
            int season = GetActiveSeason();
            int episode = GetActivEpisodes();
            using (var db = new Wiha_likiEntities())
            {
                var comp = db.registers.Where(x => x.phone.Equals(phone) && x.episode_id == episode && x.season_id == season).FirstOrDefault();
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


                    for (int i = 0; i < data.id; i++)
                    {
                        string sender = GenerateRandomPhoneNumber();
                        string msg = GenerateRandomLetter();
                       
                       
                            var obj = new ozekimessagein
                            {
                                receiver = "+800",
                                sender = sender,
                                msg = msg,
                                senttime = DateTime.Now,
                                receivedtime = DateTime.Now,
                                @operator = "ETH-MTN",
                                msgtype = "SMS:TEXT"

                            };


                            db.ozekimessageins.Add(obj);
                            db.SaveChanges();
                        
                    }

                    
                }
            }catch(Exception w)
            {

            }

        }



// A function that generates a random letter between A-E
public static string GenerateRandomLetter()
    {
        //// Create a Random object to generate random numbers
        //Random random = new Random();

        //// Generate a random number from 0 to 4
        //int number = random.Next(0, 5);

        //// Convert the number to a letter using ASCII codes
        //// A has the code 65, B has 66, and so on
        //char letter = (char)(65 + number);

        // Return the letter
        return "a";
    }


    // A function that generates a random phone number with the format +2519XXXXXXXX
    public static string GenerateRandomPhoneNumber()
    {
            // Create a StringBuilder object to store the phone number
            Random random = new Random();
            string[] phoneNumbers =
                {
                "+251934567890",
                "+251934567890",
                 "+251964567890",
                  "+251994567890",
                   "+251984567890",
                    "+251934567890",
                     "+251934567890",

            };
            // Get the length of the array
            int length = phoneNumbers.Count();

            // Generate a random index from 0 to length - 1
            int index = random.Next(0, length);

            // Get the phone number at the random index
            string randomNumber = phoneNumbers[index];

            // Return the phone number as a string
            return randomNumber;
    }


    

        //[Route("api/registor/competitor")]
        //[System.Web.Http.AcceptVerbs("POST")]
        //[System.Web.Http.HttpPost]
        //public int RegistorCompetitor(register data)
        //{
        //    using(var db = new Wiha_likiEntities())
        //    {
        //        var reg = new register()
        //        {
        //             phone = data.phone

        //        };

        //        db.registers.Add(reg);
        //        db.SaveChanges();
        //        return 1;
        //    }
        //}




        public int checkIfCompetitorSendSMSOnce(int QId, string phone)
        {
            using (var db = new Wiha_likiEntities())
            {
                //var query = (from d in db.ozekimessageins
                //             where d.receivedtime >= startDate && d.receivedtime <= endDate
                //             select d).Where(x=>x.sender.Equals(phone)).ToList();
               var query = db.answer_sms_mapping.Where(x => x.qu_Id == QId && x.phone.Equals(phone)).FirstOrDefault();
                if(query != null)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

     
        public int insertCompetitor(int answer, string msg, string phone, int QId)
        {
            using (var db = new Wiha_likiEntities())
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var check = db.answer_sms_mapping
                            .Where(x => x.phone.Equals(phone) && x.qu_Id == QId)
                            .ToList();

                        if (check.Count() == 0)
                        {
                            var obj = new answer_sms_mapping
                            {
                                answer = answer,
                                phone = phone,
                                msg = msg,
                                qu_Id = QId,
                                regDate = DateTime.Now
                            };

                            if (answer == 1)
                            {
                                updateCompetitorTotalAnswerd(phone, QId);
                            }

                            db.answer_sms_mapping.Add(obj);
                            db.SaveChanges();

                            dbContextTransaction.Commit();
                            return 1;
                        }
                        else
                        {
                            // Duplicate entry, rollback the transaction
                            return 0;
                        }
                    }
                    catch (Exception e)
                    {
                        // Handle exceptions
                        dbContextTransaction.Rollback();
                        return 0;
                    }
                }

            }

        }

        public int checkCompetitorRegistered(string phone)
        {
            int season = GetActiveSeason();
            int episode = GetActivEpisodes();
            using (var db = new Wiha_likiEntities())
            {
                var query = db.registers.Where(x => x.phone.Equals(phone) && x.season_id == season && x.episode_id == episode).FirstOrDefault();
                if(query != null)
                {
                    return 1;
                }
                else
                {
                    return 0;
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
                    if (string.Equals(answer, query.alphabet, StringComparison.OrdinalIgnoreCase))
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