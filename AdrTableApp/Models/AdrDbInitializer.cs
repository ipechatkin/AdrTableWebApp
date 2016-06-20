using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AdrTableApp.Models
{
    public class Place
    {
        public string country;
        public string city;
        public string street;
    }

    public class AdrDbInitializer : CreateDatabaseIfNotExists/*DropCreateDatabaseAlways*/<AdressContext>
    {

        Place[] places = { new Place {country = "Russia", city = "Moscow", street = "Tverskaya"},
                           new Place {country = "Russia", city = "Ulyanovsk", street = "Goncharova"}, 
                           new Place {country = "Russia", city = "Saint-Petersburg", street = "Nevsky Prospekt"},
                           new Place {country = "Russia", city = "Ulyanovsk", street = "Narimanova"},
                           new Place {country = "Russia", city = "Moscow", street = "New Arbat"},
                           new Place {country = "USA", city = "Chikago", street = "Lasalle"},
                           new Place {country = "USA", city = "New York", street = "Goncharova"}, 
                           new Place {country = "USA", city = "Saint-Petersburg", street = "Nevsky Prospekt"},
                           new Place {country = "USA", city = "Ulyanovsk", street = "Narimanova"},
                           new Place {country = "USA", city = "Moscow", street = "New Arbat"},
                           new Place {country = "USA", city = "New York", street = "Tverskaya"},
                           new Place {country = "United Kingdom", city = "Ulyanovsk", street = "Goncharova"}, 
                           new Place {country = "United Kingdom", city = "Saint-Petersburg", street = "Nevsky Prospekt"},
                           new Place {country = "United Kingdom", city = "Ulyanovsk", street = "Narimanova"},
                           new Place {country = "United Kingdom", city = "Moscow", street = "New Arbat"},
                           new Place {country = "United Kingdom", city = "Moscow", street = "Tverskaya"},
                           new Place {country = "Canada", city = "Ottava", street = "Lasalle"}, 
                           new Place {country = "Canada", city = "Saint-Petersburg", street = "Nevsky Prospekt"},
                           new Place {country = "Canada", city = "Ulyanovsk", street = "Narimanova"},
                           new Place {country = "Canada", city = "Moscow", street = "New Arbat"},
                           new Place {country = "Canada", city = "Ottava", street = "Tverskaya"}
                         };

        string[] countries = { "Russia", "USA", "Canada", "United Kingdom", "France", "Germany", "Japan", "China", "Poland", "Greece" };
        string[] cities = { "Moscow", "Saint-Petersburg", "Ulyanovsk", "Ottava", "Varshava", 
                              "Berlin", "Tokio", "Peking", "Athens", "Paris"};
        string[] streets = { "Tverskaya", "Goncharova", "Nevsky Prospekt", "New Arbat", "Lasalle", 
                               "Krymova", "Vareikisa", "Bebelya", "Tukaya", "Urickogo"};
                           

        protected override void Seed(AdressContext db)
        {
            //DateTime dt = new DateTime(1970, 1, 1);
            //created = (DateTime.Now.Ticks - dt.Ticks)/10000

            Random rnd = new Random();

            for (int index = 0; index < 150000; index++)
            {
                var place = places[rnd.Next(0, places.Length)];

                db.Adresses.Add(new Adress
                {
                    country = countries[rnd.Next(0, countries.Length - 1)],
                    city = cities[rnd.Next(0, countries.Length - 1)],
                    street = streets[rnd.Next(0, countries.Length - 1)],
                    house = rnd.Next(1, 500),
                    postcode = Convert.ToString(rnd.Next(1, 999999)),
                    created = new DateTime(rnd.Next(1930, 2016), rnd.Next(1, 12), rnd.Next(1, 28), 
                        rnd.Next(0, 23), rnd.Next(0, 60), rnd.Next(0, 60)).ToUniversalTime()   
                });
            }
        }
    }
}