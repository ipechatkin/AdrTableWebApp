using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AdrTableApp.Models
{
    public class Adress
    {
        public int id { get; set; }
        public string country { get; set; }
        public string city { get; set; }
        public string street { get; set; }
        public int house { get; set; }
        public string postcode { get; set; }
        public DateTime created { get; set; }
    }

    public class AdressContext : DbContext
    {
        public DbSet<Adress> Adresses { get; set; }
    }
}