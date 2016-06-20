using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdrTableApp.Models;
using System.Web.Helpers;
using System.Web.Script.Serialization;
using System.Linq.Expressions;

namespace AdrTableApp.Controllers
{
    //public struct SortFlags 
    //{
    //    public int country { get; set; }
    //    public int city { get; set; }
    //    public int street { get; set; }
    //    public int house { get; set; }
    //    public int postcode { get; set; }
    //    public int created { get; set; }
    //} 

    //public delegate IEnumerable<Adress> SortPredicate()

    

    public struct PageData
    {
        public int from {get; set;}
        public int count {get; set;}
        public string country { get; set; }
        public string city { get; set; }
        public string street { get; set; }
        public int house { get; set; }
        public string postcode { get; set; }
        public string dateStart { get; set; }
        public string dateEnd { get; set; }
        public bool needSort { get; set; }
        public bool sortDir { get; set; }
        public int sortCol { get; set; }
        
    }

    public class DataToClient
    {
        public IQueryable<Adress> adresses { get; set; }
        public int recTotal { get; set; }
    }

    public class MainController : Controller
    {
        AdressContext db = new AdressContext();

        //
        // GET: /Main/

        public ActionResult Index()
        {
            IEnumerable<Adress> adresses = db.Adresses;
            return View();
        }

        private DateTime ParseDateFromClient(string date)
        {
            if (null == date)
            {
                return DateTime.MinValue;
            }

            string[] yymmdd;
            string[] hhmm;

            yymmdd = date.Substring(0, 10).Split('-');
            hhmm = date.Substring(11, 5).Split(':');
            return new DateTime(Convert.ToInt32(yymmdd[0]), Convert.ToInt32(yymmdd[1]), Convert.ToInt32(yymmdd[2]),
                Convert.ToInt32(hhmm[0]), Convert.ToInt32(hhmm[1]), 0);
        }

        // [HttpPost]
        public JsonResult GetData(PageData data)
        {
            var country = data.country != null ? data.country : "";
            var city = data.city != null ? data.city : "";
            var street = data.street != null ? data.street : "";
            var house = data.house <= 0 ? 0 : data.house;
            var postcode = data.postcode != null ? data.postcode : "";

            DateTime d1 = ParseDateFromClient(data.dateStart);
            DateTime d2 = (null == data.dateEnd) ? DateTime.MaxValue : ParseDateFromClient(data.dateEnd);

            DataToClient dataToClient = new DataToClient();

            Expression<Func<Adress, Object>> lambda0 = x => x.country;
            Expression<Func<Adress, Object>> lambda1 = x => x.city;
            Expression<Func<Adress, Object>> lambda2 = x => x.street;
            Expression<Func<Adress, Object>> lambda3 = x => x.house;
            Expression<Func<Adress, Object>> lambda4 = x => x.postcode;
            Expression<Func<Adress, Object>> lambda5 = x => x.created;


            Expression<Func<Adress, Object>>[] lambdaArr = { lambda0, lambda1, lambda2, lambda3, lambda4, lambda5 };          
            
            dataToClient.adresses = db.Adresses
                .Where(a => a.country.Contains(country) && a.city.Contains(city) && a.postcode.StartsWith(postcode) && (a.house >= house) &&
                    a.street.Contains(street) && (a.created >= d1) && (a.created <= d2)).ToList<Adress>().AsQueryable();

            if (data.needSort)
            {
                if (data.sortDir)
                {
                    dataToClient.adresses = dataToClient.adresses.OrderByDescending(lambdaArr[data.sortCol]);
                }
                else
                {
                    dataToClient.adresses = dataToClient.adresses.OrderBy(lambdaArr[data.sortCol]);
                }
            }

            dataToClient.recTotal = dataToClient.adresses.Count();
            dataToClient.adresses = dataToClient.adresses.Skip(data.from).Take(data.count);                

            return Json(dataToClient, JsonRequestBehavior.AllowGet);
        }
    }
}
