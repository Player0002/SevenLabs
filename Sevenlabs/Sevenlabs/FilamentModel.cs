using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sevenlabs
{
    public class FilamentModel
    {
        private string _id;
        private string code;
        private string creator;
        private string type;
        private double weight;
        private string color;
        private int count;

        public FilamentModel(string code, string id, string creator, string type, double weight, string color, int count) {
            Console.WriteLine(creator);
            ID = id;
            Creator = creator;
            Type = type;
            Weight = weight;
            Color = color;
            Count = count;
            Code = code;
        }
    
        
        
        public string ID {
            get { return _id;  }
            set { _id = value; }
        }

        public string Creator {
            get { return creator;  } set { creator = value;  }
        }

        public string Type {
            get { return type; } set { type = value; }
        }

        public double Weight {
            get { return weight; } set { weight = value; }
        }
        public string Color { 
            get { return color; } set { color = value; }
        }
        public int Count {
            get { return count; } set { count = value; }
        }
        public string Code {
            get { return code; } set { code = value; }
        }
        public static FilamentModel fromJson(JObject obj) {
            FilamentModel model = new FilamentModel(obj["code"].ToString(), obj["_id"].ToString(), obj["creator"].ToString(), obj["type"].ToString(), obj["weight"].ToObject<double>(), obj["color"].ToString(), obj["count"].ToObject<int>());
            return model;
        }
    }
}
