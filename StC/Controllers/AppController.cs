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
using Microsoft.Extensions.Configuration;
using StC.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StC.Controllers
{
    [Route("api/[controller]")]
    public class AppController : Controller
    {
        private MySqlDatabase MySqlDatabase { get; set; }
        private IConfiguration conf;
        private string host { get; set; }
        private string href { get; set; }

        public AppController(MySqlDatabase mysqlDB, IConfiguration configuration)
        {
            this.MySqlDatabase = mysqlDB;
            conf = configuration;
        }


        // GET: api/values
        [HttpGet]
        public IEnumerable<WordStat> Get()
        {
            List<WordStat> ws = new List<WordStat>();

            MySqlCommand cmd = this.MySqlDatabase.Connection.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"select * from wordstats order by cnt desc;";

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var k = Convert.FromBase64String(conf.GetSection("ENC_KEY").GetSection("key").Value);
                    var iv = Convert.FromBase64String(reader["iv"].ToString());

                    ws.Add(new WordStat
                    {
                        Word = Encryptor.Decrypt(Convert.FromBase64String(reader["encrypted_word"].ToString()), k, iv),
                        Cnt = Convert.ToInt32(reader["cnt"])
                        //Id = Convert.ToInt32(reader["Id"]),
                        //Name = reader["Name"].ToString(),
                        //ArtistName = reader["ArtistName"].ToString(),
                        //Price = Convert.ToInt32(reader["Price"]),
                        //Genre = reader["genre"].ToString()
                    });
                }
            }

            return ws;

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
            this.href = sub[0];

            Task<string> kk = processTxtAsync(sub[0]);
            string docText = kk.Result;

            Dictionary<string, int> wo = wordOccurences(docText);

            if (wo.Count > 0)
                saveToDB(wo);

            return wo; //new Dictionary<string, string> { { "message", kk.Result } };
        }

        #region app logic

        protected void saveToDB(Dictionary<string, int> words)
        {
            List<string> values = new List<string>();
            var cmd = this.MySqlDatabase.Connection.CreateCommand() as MySqlCommand;
            var cmd2 = this.MySqlDatabase.Connection.CreateCommand() as MySqlCommand;

            cmd.CommandText = @"INSERT INTO sites(sitename,sitehref) VALUES (@site, @href);";
            cmd.Parameters.AddWithValue("@site", this.host);
            cmd.Parameters.AddWithValue("@href", this.href);

            try
            {
                var recs = cmd.ExecuteNonQuery();
                var lastID = cmd.LastInsertedId;

                foreach (KeyValuePair<string, int> wrd in words)
                {
                    var slt = Secure.GetSalt();
                    var hash = Secure.Hasher(wrd.Key, slt);
                    //var k = Encryptor.genKey(wrd.Key, slt); //
                    //string kstr = Convert.ToBase64String(k);
                    var k = Convert.FromBase64String(conf.GetSection("ENC_KEY").GetSection("key").Value);
                    var iv = Encryptor.genIV(wrd.Key, slt);
                    string enc = Encryptor.Encrypt(wrd.Key, k, iv);
                    //string dec =  Encryptor.Decrypt(System.Text.Encoding.Unicode.GetBytes(enc), k, iv);

                    values.Add($"('{hash}', {lastID}, '{enc}', {wrd.Value}, '{Convert.ToBase64String(slt)}', '{Convert.ToBase64String(iv)}' )");
                }

                var vals = String.Join(",", values.ToArray());

                cmd2.CommandText = $"INSERT INTO wordstats(hash_id, site_id, encrypted_word, cnt, salt, iv) VALUES {vals}";
                var rec_wrd = cmd2.ExecuteNonQuery();

            }
            catch (Exception err)
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
            words = words.Distinct().Where(w => w.Length > 2).Where(w => w.Length < 100).Select(w => w.ToUpper()).ToList();

            Dictionary<string, int> maneno = new Dictionary<string, int>();

            foreach (string needle in words)
            {
                int count = 0;

                foreach (string wrd in words)
                    if (wrd.Contains(needle))
                        count++;

                if (!maneno.ContainsKey(needle))
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

        #endregion

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
