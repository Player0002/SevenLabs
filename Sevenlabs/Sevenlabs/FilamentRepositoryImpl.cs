using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sevenlabs
{
    class FilamentRepositoryImpl : FilamentRepository
    {
        HttpClient client = new HttpClient();
        string url = "http://localhost:3000/";

        public FilamentRepositoryImpl() {

            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<FilamentModel> AddFilaments(object model)
        {
            Console.WriteLine(JsonConvert.SerializeObject(model));
            var response = await client.PostAsJsonAsync("/datas", model);
            if (response.IsSuccessStatusCode) {
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine(json);
                return FilamentModel.fromJson(JObject.Parse(JObject.Parse(json)["data"].ToString()));
            }
            return null;
        }

        public async Task<FilamentModel> DeleteFilaments(FilamentModel model)
        {
            var response = await HttpClientExtensions.DeleteAsJsonAsync(client, "/datas", new { 
                creator = model.Creator,
                count = model.Count,
                _id = model.ID,
                type = model.Type,
                weight = model.Weight,
                color = model.Color
            });
            if (response.IsSuccessStatusCode) {
                return model;
            }
            return null;
        }

        public async Task<List<FilamentModel>> GetFilaments(string creator, string type, string color)
        {
            string url = "/datas";
            StringBuilder builder = new StringBuilder();
            if (creator != null && !creator.Equals("")) builder.Append("creator=" + creator + "&");
            if (type != null && !type.Equals("")) builder.Append("type=" + type + "&");
            if (color != null && !color.Equals("")) builder.Append("color=" + color + "&");
            var response = await client.GetAsync(builder.Length > 0 ? url + "?" + builder.ToString().Substring(0, builder.Length - 1) : url);
            if (response.IsSuccessStatusCode) {
                var contents = await response.Content.ReadAsStringAsync();
                JObject result = JObject.Parse(contents);
                JArray array = JArray.Parse(result["data"].ToString());
                List<FilamentModel> models = new List<FilamentModel>();
                foreach (JObject item in array) {
                    Console.WriteLine(item);
                    FilamentModel model = FilamentModel.fromJson(item);
                    models.Add(model);
                }
                return models;
            }
            return null;

        }

        public async Task<FilamentModel> UpdateFilaments(FilamentModel model)
        {
            var updated = await client.PutAsJsonAsync("/datas", HttpClientExtensions.Serialize(model));
            if (updated.IsSuccessStatusCode) return model;
            return null;
        }
    }
}
