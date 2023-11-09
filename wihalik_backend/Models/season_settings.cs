using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wihalik_backend.Models
{
    public class season_settings
    {
        public int SId { get; set; }
        public string season { get; set; }
        public double numberOfEpisodes { get; set; }
        public int? status { get; set; }
    }

}