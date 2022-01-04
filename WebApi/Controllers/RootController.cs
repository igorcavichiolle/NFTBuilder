using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class RootController : ControllerBase
    {
        public RootController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // GET: api/Root
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<Root>> GetRoot([FromQuery] string earth_date, string rover)
        {
            var teste = Request.Body;
            using (var client = new HttpClient())
            {

                HttpResponseMessage response = await client.GetAsync(BuildUrl(earth_date, rover));
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    Root root = JsonConvert.DeserializeObject<Root>(result);

                    return root;
                }
                else
                {
                    return null;
                }
            }
        }

        public Uri BuildUrl(string earth_date)
        {
            const string url = "https://api.nasa.gov/mars-photos/api/v1/rovers/curiosity/photos";
            var param = new Dictionary<string, string>()
            {
                { "earth_date", earth_date } ,
                { "api_key", Configuration.GetValue<string>("ApiKeyNasa")} ,
            };


            return new Uri(QueryHelpers.AddQueryString(url, param));
        }

        public Uri BuildUrl(string earth_date, string rover)
        {
             string url = string.Format("https://api.nasa.gov/mars-photos/api/v1/rovers/{0}/photos", rover);
            var param = new Dictionary<string, string>()
            {
                { "earth_date", earth_date } ,
                { "api_key", Configuration.GetValue<string>("ApiKeyNasa")} ,
            };


            return new Uri(QueryHelpers.AddQueryString(url, param));
        }
    }
}
