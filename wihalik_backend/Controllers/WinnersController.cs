using System;
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
    public class WinnersController : ApiController
    {
        [Route("api/season/episode/winners")]
        [System.Web.Http.AcceptVerbs("GET")]
        [System.Web.Http.HttpGet]
        public async Task<PagedResult<register>> EpisodeWinners(int currentPage, int pageSize)
        {
            int season = GetActiveSeason();
            int episode = GetActivEpisodes();
            using (var db = new Wiha_likiEntities())
            {
                // Calculate the number of items to skip
                int skip = Math.Max(0, (currentPage - 1) * pageSize);
                int totalQuesions = GetTotalNumberOfQuesionsPerEpisodes();

                // Query the database and get the total count

                var query = db.registers
                   .Where(e => e.totalAnswered == db.registers.Max(x => x.totalAnswered) && e.totalAnswered > 0 && e.season_id == season && e.episode_id == episode)
                   .OrderBy(x => x.id) // Replace SomeProperty with the property you want to order by
                   .AsQueryable();



                int totalItems = await query.CountAsync();


                // Apply pagination to the query
                var pagedData = await query.Skip(skip).Take(pageSize).ToListAsync();

                // Return the result as a PagedResult
                return new PagedResult<register>
                {
                    CurrentPage = currentPage,
                    PageSize = pageSize,
                    TotalItems = totalItems,
                    Data = pagedData
                };
            }
        }

        public int GetTotalNumberOfQuesionsPerEpisodes()
        {

            int season = GetActiveSeason();
            int episode = GetActivEpisodes();
            using (var db = new Wiha_likiEntities())
            {
                var qu = db.questions.Where(x => x.episode_id == episode && x.season_id == episode).ToList();

                if (qu.Count > 0)
                {
                    return qu.Count;
                }
                else
                {
                    return 0;
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

    }
}