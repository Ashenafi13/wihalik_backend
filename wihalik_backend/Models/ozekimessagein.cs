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
    
    public partial class ozekimessagein
    {
        public int id { get; set; }
        public string sender { get; set; }
        public string receiver { get; set; }
        public string msg { get; set; }
        public Nullable<System.DateTime> senttime { get; set; }
        public Nullable<System.DateTime> receivedtime { get; set; }
        public string @operator { get; set; }
        public string msgtype { get; set; }
        public string reference { get; set; }
    }
}
