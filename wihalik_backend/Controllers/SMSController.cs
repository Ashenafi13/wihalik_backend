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

            int result = 0;
            using (var ddb = new Wiha_likiEntities())
            {
                using (var db = new EBCSMSEntities())
                {
                    try
                    {

                        var registored = ddb.registers.ToList();

                        for (int i = 0; i < registored.Count(); i++)
                        {
                            var sms = db.Database.SqlQuery<ozekimessagein>("SELECT * FROM [EBCSMS].[dbo].[ozekimessagein] WHERE receivedtime BETWEEN '" + startDate + "' AND '" + endDate + "' AND sender='" + registored[i].phone + "'").FirstOrDefault();
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
        // Create a Random object to generate random numbers
        Random random = new Random();

        // Generate a random number from 0 to 4
        int number = random.Next(0, 5);

        // Convert the number to a letter using ASCII codes
        // A has the code 65, B has 66, and so on
        char letter = (char)(65 + number);

        // Return the letter
        return letter.ToString();
    }


    // A function that generates a random phone number with the format +2519XXXXXXXX
    public static string GenerateRandomPhoneNumber()
    {
            // Create a StringBuilder object to store the phone number
            Random random = new Random();
            string[] phoneNumbers =
                {
                "+251934567890", 
                "+251945678901",
                "+251956789012",
                "251955115809",
"251972056584",
"251961594466",
"251901046233",
"251925420098",
"251973504331",
"251923469345",
"251930291384",
"251915577434",
"251923760734",
"251900768644",
"251989925772",
"251973014127",
"251909599366",
"251908326956",
"251968184424",
"251963828401",
"251993280706",
"251919149215",
"251949925221",
"251934218592",
"251905235660",
"251906149770",
"251949555150",
"251993606746",
"251924183169",
"251937226488",
"251916094112",
"251937256571",
"251995858609",
"251938120704",
"251946611653",
"251965406221",
"251942971775",
"251977476480",
"251928340058",
"251983856487",
"251919642527",
"251910158125",
"251962793870",
"251988160638",
"251994624503",
"251993648683",
"251920253107",
"251986013160",
"251942776092",
"251923920991",
"251909476420",
"251922553138",
"251972496265",
"251976015893",
"251992494436",
"251922030987",
"251974363359",
"251909693640",
"251955091743",
"251975234896",
"251942963654",
"251985506654",
"251934272342",
"251949930817",
"251941742143",
"251953610963",
"251917741066",
"251994338391",
"251967852519",
"251900989775",
"251903546333",
"251963660775",
"251927248729",
"251921109308",
"251960841937",
"251992252679",
"251973305605",
"251925181503",
"251902395654",
"251994731671",
"251993271880",
"251989929013",
"251900484597",
"251974432467",
"251905221422",
"251942591778",
"251932081555",
"251922224866",
"251937525202",
"251932597483",
"251912891320",
"251936668934",
"251994505916",
"251928676820",
"251964546109",
"251993172739",
"251953652673",
"251968157376",
"251916005997",
"251960364674",
"251944909375",
"251965476053",
"251989757929",
"251938530763",
"251982811514",
"251933111215",
"251986141621",
"251921480913",
"251909499362",
"251942160018",
"251911959888",
"251974585203",
"251938072250",
"251964748973",
"251931368109",
"251938086736",
"251926107558",
"251949852455",
"251964581042",
"251931577668",
"251924610788",
"251994334659",
"251973185762",
"251965864852",
"251975648165",
"251965117887",
"251975912311",
"251994793983",
"251939785147",
"251945108049",
"251973283457",
"251996618414",
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


    public int reg(string phone)
        {
            using(var db = new Wiha_likiEntities())
            {
                var r = new register
                {
                    phone = phone,
                    totalAnswered = 0
                };

                db.registers.Add(r);
                db.SaveChanges();
                return 1;
            }
        }

        [Route("api/registor/competitor")]
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        public int RegistorCompetitor(register data)
        {
            using(var db = new Wiha_likiEntities())
            {
                var reg = new register()
                {
                     phone = data.phone

                };

                db.registers.Add(reg);
                db.SaveChanges();
                return 1;
            }
        }




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

        public int insertCompetitor(int answer,string msg,string phone,int QId)
        {
            using (var db = new Wiha_likiEntities())
            {
               
                try
                {
                    var check = db.answer_sms_mapping.Where(x => x.phone.Equals(phone) && x.qu_Id == QId).ToList();
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

                        if(answer == 1)
                        {
                            updateCompetitorTotalAnswerd(phone);
                        }
                      
                        db.answer_sms_mapping.Add(obj);
                        db.SaveChanges();
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }catch(Exception e)
                {
                    return 0;
                }
              
            }

        }


        public int checkCompetitorRegistered(string phone)
        {
            using (var db = new Wiha_likiEntities())
            {
                var query = db.registers.Where(x => x.phone.Equals(phone)).FirstOrDefault();
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