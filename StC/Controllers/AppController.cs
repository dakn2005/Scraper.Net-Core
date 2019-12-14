using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using HtmlAgilityPack;
using StC.Classes;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StC.Controllers
{
    [Route("api/[controller]")]
    public class AppController : Controller
    {
        private MySqlDatabase MySqlDatabase { get; set; }
        private string host { get; set; }
        private string href { get; set; }

        public AppController(MySqlDatabase mysqlDB)
        {
            this.MySqlDatabase = mysqlDB;
        }

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
        public Dictionary<string, int> Post([FromBody] string str)
        {
            string[] sub = str.Split(" ");
            this.host = sub[1];
            this.href = sub[2];

            Task<string> kk = processTxtAsync(sub[0]);
            string docText = kk.Result;

            Dictionary<string, int> wo = wordOccurences(docText);

            if (wo.Count > 0)
                saveToDB(wo);

            return wo; //new Dictionary<string, string> { { "message", kk.Result } };
        }

        protected void saveToDB(Dictionary<string, int> words)
        {
            List<string> values = new List<string>();
            var cmd = this.MySqlDatabase.Connection.CreateCommand() as MySqlCommand;
            var cmd2 = this.MySqlDatabase.Connection.CreateCommand() as MySqlCommand;

            cmd.CommandText = @"INSERT INTO sites(sitename,sitehref) VALUES (@site, @href);";
            cmd.Parameters.AddWithValue("@site", this.host);
            cmd.Parameters.AddWithValue("@href", this.href);

            try {
                var recs = cmd.ExecuteNonQuery();
                var lastID = cmd.LastInsertedId;

                foreach(KeyValuePair<string,int> wrd in words) values.Add($"({lastID}, '{wrd.Key}', {wrd.Value} )");

                var vals = String.Join(",", values.ToArray());

                cmd2.CommandText = $"INSERT INTO wordstats(site_id, word, cnt) VALUES {vals}";
                var rec_wrd = cmd2.ExecuteNonQuery();
                
            }
            catch(Exception err)
            {
                throw err;
            }
            
            
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
            words = words.Distinct().Where(w=>w.Length > 2).Select(w=>w.ToLower()).ToList();
            
            Dictionary<string, int> maneno = new Dictionary<string, int>();

            foreach (string needle in words)
            {
                int count = 0;

                foreach (string wrd in words)
                    if (wrd.Contains(needle))
                        count++;

                maneno.Add(needle, count);
            }

            Dictionary<string, int> output = maneno.OrderByDescending(w => w.Value).Take(100).ToDictionary(g => g.Key, g => g.Value);

            return output;

            //does not check for contains

            //return words
            //    .Select(w => w.ToLower().Trim())
            //    .Where(w => !Regex.IsMatch(w, @"^\d+$"))
            //    //.Where(w => Encoding.UTF8.GetByteCount(w) == w.Length)
            //    .GroupBy(tag => tag)
            //    .ToDictionary(group => group.Key, group => group.Count())
            //    .OrderByDescending(w => w.Value)
            //    .ToDictionary(g => g.Key, g => g.Value);
        }

        //protected List<string> checkValidEnglishWord(List<string> kamusi)
        //{
            //NetSpell.SpellChecker.Dictionary.WordDictionary oDict = new NetSpell.SpellChecker.Dictionary.WordDictionary();
            //oDict.DictionaryFile = "wwwroot/lib/en-us.dic";
            //oDict.Initialize();
            //NetSpell.SpellChecker.Spelling oSpell = new NetSpell.SpellChecker.Spelling();
            //oSpell.Dictionary = oDict;

            //return kamusi.Where(w => oSpell.TestWord(w)).ToList();
        //}

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
