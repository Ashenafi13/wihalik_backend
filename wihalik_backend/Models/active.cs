﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wihalik_backend.Models
{
    public class active
    {
        public string season { get; set; }
        public string episode { get; set; }

        public double? episodeStartTime { get; set; }
        public int? time_status { get; set; }
    }
}