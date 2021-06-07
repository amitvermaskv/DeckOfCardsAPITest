using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeckOfCards.API.Tests.Data
{
    public static class DeckDetails
    {
        public static List<Card> Cards { get; set; }
        static DeckDetails()
        {
            string deckDataJsonPath = AppDomain.CurrentDomain.BaseDirectory + "\\Data\\DeckData.json";
            if (File.Exists(deckDataJsonPath))
            {
                Cards = JsonConvert.DeserializeObject<List<Card>>(File.ReadAllText(deckDataJsonPath));
            }
        }
    }

    public class Card
    {
        public string code { get; set; }
        public string image { get; set; }
        public Images images { get; set; }
        public string value { get; set; }
        public string suit { get; set; }
    }

    public class Images
    {
        public string svg { get; set; }
        public string png { get; set; }
    }
}
