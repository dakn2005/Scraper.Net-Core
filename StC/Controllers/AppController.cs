using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using HtmlAgilityPack;
using StC.Classes;
using System.Text.RegularExpressions;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StC.Controllers
{
    [Route("api/[controller]")]
    public class AppController : Controller
    {
        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public Dictionary<string, int> Post([FromBody]string url)
        {
            Task<string> kk = processTxtAsync(url);
            string docText = kk.Result;

            return wordOccurences(docText); //new Dictionary<string, string> { { "message", kk.Result } };
        }

        protected async Task<string> processTxtAsync(string url)
        {
            // instance or static variable
            HttpClient client = new HttpClient();

            // get answer in non-blocking way
            using (var response = await client.GetAsync(url))
            {
                using (var content = response.Content)
                {
                    // read answer in non-blocking way
                    var result = await content.ReadAsStringAsync();
                    var document = new HtmlDocument();
                    document.LoadHtml(result);
                    var nodes = document.DocumentNode.InnerText; //.SelectNodes("//div");
                    //Some work with page....
                    //List<string> txts = new List<string>();
                    //foreach (HtmlNode node in document.DocumentNode.SelectNodes("//text()"))
                    //{
                    //    txts.Add(node.InnerText);
                    //}

                    //replace all special characters
                    string clean = StopWordsRemover.RemoveStopwords(nodes.Replace("\n", " ").Replace("\t", " ").Trim());
                    clean = Regex.Replace(clean, @"[\:|\;|\.]", "");

                    return clean;
                }
            }
        }

        protected Dictionary<string, int> wordOccurences(string documentTxt)
        {
            List<string> words = new List<string>(documentTxt.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            words = words.Select(w => Regex.Replace(w, "[^A-Za-z']", "")).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

            return words
                .Select(w => w.ToLower().Trim())
                .Where(w => !Regex.IsMatch(w, @"^\d+$"))
                //.Where(w => Encoding.UTF8.GetByteCount(w) == w.Length)
                .GroupBy(tag => tag)
                .ToDictionary(group => group.Key, group => group.Count())
                .OrderByDescending(w => w.Value)
                .ToDictionary(g => g.Key, g => g.Value);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
