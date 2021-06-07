using NUnit.Framework;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DeckOfCards.API.Tests.Models.DeckNew;

namespace DeckOfCards.API.Tests.Tests
{
    [TestFixture]
    public class DeckNew
    {
        private readonly string BaseURL = TestContext.Parameters["BaseURL"].ToString();
        private readonly string DeckNewMethodName = @"/api/deck/new";

        [Test]
        public void NewDeck_StatusCode()
        {
            //arrange
            RestClient client = new RestClient(BaseURL);
            RestRequest request = new RestRequest(DeckNewMethodName, Method.GET);

            //act
            IRestResponse apiResponse = client.Execute(request);

            //assert
            Assert.That(apiResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public void NewDeck_Response()
        {
            //arrange
            RestClient client = new RestClient(BaseURL);
            RestRequest request = new RestRequest(DeckNewMethodName, Method.GET);

            //act
            IRestResponse apiResponse = client.Execute(request);

            //assert
            Assert.That(apiResponse.Content, Is.Not.Null, "DeckNew api response is NULL. Expected NOT NULL");
            Response deck = JsonConvert.DeserializeObject<Response>(apiResponse.Content);
            Assert.That(deck.success, Is.EqualTo(true));
            Assert.IsTrue(!String.IsNullOrEmpty(deck.deck_id),"DeckID is returned as null or empty string. Expected value to be a string deckid");
            Assert.That(deck.shuffled, Is.EqualTo(false));
            Assert.That(deck.remaining, Is.EqualTo(52));
        }

        [TestCase(Method.GET, "", 52)]
        [TestCase(Method.GET, "true", 54)]
        [TestCase(Method.GET, "false", 52)]
        [TestCase(Method.GET, "InvalidString", 52)]
        [TestCase(Method.GET, "1234", 52)]
        [TestCase(Method.POST, "", 52)]
        [TestCase(Method.POST, "true", 54)]
        [TestCase(Method.POST, "false", 52)]
        [TestCase(Method.POST, "InvalidString", 52)]
        [TestCase(Method.POST, "1234", 52)]
        public void NewDeck_AddJoker(Method Method, string JokerEnabled, int ExpectedCardCount)
        {
            //arrange
            RestClient client = new RestClient(BaseURL);
            string methodName = DeckNewMethodName + (!String.IsNullOrEmpty(JokerEnabled) ? $"?jokers_enabled={JokerEnabled}" : "");
            RestRequest request = new RestRequest(methodName,Method);

            //act
            IRestResponse apiResponse = client.Execute(request);

            //assert
            Assert.That(apiResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Response deck = JsonConvert.DeserializeObject<Response>(apiResponse.Content);
            Assert.That(deck.success, Is.EqualTo(true));
            Assert.IsTrue(!String.IsNullOrEmpty(deck.deck_id), "DeckID is returned as null or empty string. Expected value to be a string deckid");
            Assert.That(deck.shuffled, Is.EqualTo(false));
            Assert.That(deck.remaining, Is.EqualTo(ExpectedCardCount));

        }
    }
}
