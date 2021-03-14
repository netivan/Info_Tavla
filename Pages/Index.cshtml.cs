using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Info_Tavla.Models;
using Info_Tavla.Models.Departure;
using System.Net.Http;
//using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Http;
using Info_Tavla.Models.Lotto;
using Info_Tavla.Models.Weather;
using Newtonsoft.Json;
using Info_Tavla.Models.LottoRader;

namespace Info_Tavla.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public HttpClient client = new HttpClient();  // The HttpClient class instance acts as a session to send HTTP requests. An HttpClient instance is a collection of settings applied to all requests executed by that instance.
       
        
        public Superlotto suplotto { get; set; }


        public Weather weatherStk { get; set; }


        public List<LottoRader> rader { get; set; }   // raderna (från min API) som får vi på tavlan 



        [BindProperty(SupportsGet = true)]
        public string siteId { get; set; }

        public Departures departure { get; set; }

        [BindProperty(SupportsGet = true)]
        public string location { get; set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        } 

        public async Task<IActionResult> OnGetAsync()
        {
            //if(location != null)
            //{
            //    HttpContext.Session.SetString("location", location);
            //}
            //else
            //{
            //    if(HttpContext.Session.GetString("location") !=null)
            //    {
            //        location = HttpContext.Session.GetString("location");
            //    }
            //    else
            //    {
            //        location = "";
            //    }
            //}

            //hpl = await GetStopsAsync();
            departure = await GetDeparturesAsync();

            suplotto = await GetSlottoAsync();

            weatherStk = await GetMeteoAsync();

            rader = await GetRaderAsync();

            return Page();
        }

        
        public async Task<Superlotto> GetSlottoAsync()
        {
            string nrDragning = "31-2021";    // drangningen 
            string link = $"https://www.superenalotto.it/sisal-gn-proxy-servlet-web/proxy-videoestrazioni/servizi-sisal/api.php/superenalotto/extraction/{nrDragning}/json";
            Task<string> LottoTask = client.GetStringAsync(link);
            string LottoJson = await LottoTask;
            if (LottoJson[0] == '{')    // se till att det är en Json fil
            {
                var SuperDrag = JsonConvert.DeserializeObject<Superlotto>(LottoJson); // Skriver in i fälten av klassen SuperLotto innehållet som finns i länken i Json format 
                return SuperDrag;
            }
            else
                return null;
        }


        public async Task<List<LottoRader>> GetRaderAsync()
        {
            string mx = "/6";
            System.Threading.Thread.Sleep(3000);   // om man ansluter sig innan får man error.
            string link = $"https://localhost:44377/Superenalotto{mx}";
            Task<string> raderStringTask = client.GetStringAsync(link);
            string raderString = await raderStringTask;
            return JsonConvert.DeserializeObject<List<LottoRader>>(raderString);
            
        }




        public async Task<Departures> GetDeparturesAsync()
        {
            siteId = "1010"; // hållplats Stureplan
            string link = $"https://api.sl.se/api2/realtimedeparturesV4.json?key=8a95638236ff4236921d7ac6bd8713eb&siteid={siteId}&timewindow=10";
            Task<string> getBusDepartureStringTask = client.GetStringAsync(link);
            string busDepartureString = await getBusDepartureStringTask; 
            departure = JsonConvert.DeserializeObject<Departures>(busDepartureString);
            return departure;
        }

        public async Task<Weather> GetMeteoAsync()
        {
            
            string link = $"http://api.weatherstack.com/current?access_key=547175f4de58929c5a624d8958ef81db&query=Stockholm";
            Task<string> MeteoTask = client.GetStringAsync(link);
            string MeteoJson = await MeteoTask;
            if (MeteoJson[0] == '{')    // se till att det är en Json fil
            {
                var VaderStk = JsonConvert.DeserializeObject<Weather>(MeteoJson); // Skriver in i fälten av klassen SuperLotto innehållet som finns i länken i Json format 
                return VaderStk;
            }
            else
                return null;
        }


    }
}
