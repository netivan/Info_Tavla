using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Info_Tavla.Models.LottoRader;
using Info_Tavla.Controllers;

namespace Info_Tavla.Controllers
{
    [Route("[controller]")]
    [ApiController]


    public class SuperenalottoController : ControllerBase
    {
        Random rnd = new Random();

        private readonly ILogger<SuperenalottoController> _logger;

        static readonly string[] scopeRequiredByApi = new string[] { "access_as_user" };

        public SuperenalottoController(ILogger<SuperenalottoController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<LottoRader> Get()
        {
            return Get(6);
        }

        // om man inte skriver ett nummer som indikerar antal kolumner högre än 6 får man tillbaka 6 rader

        [HttpGet("{mx:int}")]   // om man sätter ett nummer hamnar det i mx
        public IEnumerable<LottoRader> Get(int mx)    // mx antal rader
        {
            return Enumerable.Range(1, mx).Select(index => new LottoRader { Rad = skapaRad().ToArray() }).ToArray() ;
        }

        public List<int> skapaRad()
        {
            List<int> numren = new List<int>();

            int n;

            while(numren.Count() < 6)
            {
                n = (int)rnd.Next(1, 91);

                if (!numren.Contains(n))    // om nummer n inte finns i lista då lägg det i listan
                {
                    numren.Add(n);
                }

            }

            numren.Sort();

            return numren;
        }

    }
}
