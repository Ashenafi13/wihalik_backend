//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace wihalik_backend.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class question
    {
        public int id { get; set; }
        public string question_label { get; set; }
        public Nullable<int> season_id { get; set; }
        public Nullable<int> episode_id { get; set; }
        public Nullable<int> time { get; set; }
        public Nullable<int> status { get; set; }
    }
}