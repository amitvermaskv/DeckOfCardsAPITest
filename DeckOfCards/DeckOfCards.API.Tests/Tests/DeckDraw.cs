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
using DeckOfCards.API.Tests.Models.DeckDraw;
using DeckOfCards.API.Tests.Data;

namespace DeckOfCards.API.Tests.Tests
{
    [TestFixture]
    public class DeckDraw
    {
        private readonly string BaseURL = TestContext.Parameters["BaseURL"].ToString();
        private readonly string DeckDrawMethodName = @"/api/deck/<<deck_id>>/draw";
        private readonly string DeckNewMethodName = @"/api/deck/new";
        private Models.DeckNew.Response testDeck;
        private int testDeckRemainingCount;

        [OneTimeSetUp]
        public void Setup()
        {
            RestClient client = new RestClient(BaseURL);
            RestRequest request = new RestRequest(DeckNewMethodName, Method.GET);
            IRestResponse apiResponse = client.Execute(request);
            testDeck = JsonConvert.DeserializeObject<Models.DeckNew.Response>(apiResponse.Content);
            testDeckRemainingCount = testDeck.remaining;
        }

        [TestCase(2)]
        [TestCase(10)]
        [TestCase(0)]
        [TestCase(null)]
        public void Draw_TestDeck(int? CardsToDraw)
        {
            //arrange
            RestClient client = new RestClient(BaseURL);
            string methodName = DeckDrawMethodName.Replace("<<deck_id>>", testDeck.deck_id) + (CardsToDraw.HasValue ? $"?count={CardsToDraw}" : "");
            testDeckRemainingCount -= CardsToDraw.HasValue ? CardsToDraw.Value : 1;
            RestRequest request = new RestRequest(methodName, Method.GET);

            //act
            IRestResponse apiResponse = client.Execute(request);

            //assert
            Assert.That(apiResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Response deckCards = JsonConvert.DeserializeObject<Response>(apiResponse.Content);
            Assert.That(deckCards.success, Is.True);
            Assert.That(deckCards.cards.Count, Is.EqualTo(CardsToDraw.HasValue ? CardsToDraw.Value : 1));
            Assert.That(deckCards.deck_id, Is.EqualTo(testDeck.deck_id));
            Assert.That(deckCards.remaining, Is.EqualTo(testDeckRemainingCount));
            ValidateCardsData(deckCards.cards);
        }

        [TestCase(5)]
        [TestCase(10)]
        [TestCase(52)]
        [TestCase(0)]
        [TestCase(null)]
        [TestCase(-10)]
        [TestCase(-52)]
        public void Draw_NewDeck_ValidScenarios(int? CardsToDraw)
        {
            //arrange
            RestClient client = new RestClient(BaseURL);
            string methodName = DeckDrawMethodName.Replace("<<deck_id>>", "new") + (CardsToDraw.HasValue ? $"?count={CardsToDraw}" : "");
            int remainingCount = (CardsToDraw.HasValue ? ( CardsToDraw.Value >=0 ? 52 - CardsToDraw.Value : -1 * CardsToDraw.Value ) : 51);
            RestRequest request = new RestRequest(methodName, Method.GET);

            //act
            IRestResponse apiResponse = client.Execute(request);

            //assert
            Assert.That(apiResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Response deckCards = JsonConvert.DeserializeObject<Response>(apiResponse.Content);
            Assert.That(deckCards.success, Is.True);
            Assert.That(deckCards.cards.Count, Is.EqualTo(52-remainingCount));
            Assert.IsTrue(!String.IsNullOrEmpty(deckCards.deck_id), "DeckID is returned as null or empty string. Expected value to be a string deckid");
            Assert.That(deckCards.remaining, Is.EqualTo(remainingCount));
            ValidateCardsData(deckCards.cards);
        }

        [Test]
        public void Draw_Count_InvalidString()
        {
            //arrange
            RestClient client = new RestClient(BaseURL);
            string methodName = DeckDrawMethodName.Replace("<<deck_id>>", "new") + "?count=InvalidString";
            RestRequest request = new RestRequest(methodName, Method.GET);

            //act
            IRestResponse apiResponse = client.Execute(request);

            //assert
            Assert.That(apiResponse.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        }

        [TestCase(53)]
        [TestCase(100)]
        public void Draw_Count_OutOfBound(int CardsToDraw)
        {
            //arrange
            RestClient client = new RestClient(BaseURL);
            string methodName = DeckDrawMethodName.Replace("<<deck_id>>", "new") + $"?count={CardsToDraw}";
            RestRequest request = new RestRequest(methodName, Method.GET);

            //act
            IRestResponse apiResponse = client.Execute(request);

            //assert
            Assert.That(apiResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Response deckCards = JsonConvert.DeserializeObject<Response>(apiResponse.Content);
            Assert.That(deckCards.success, Is.False);
            Assert.That(deckCards.cards.Count, Is.EqualTo(52));
            Assert.That(deckCards.error, Is.EqualTo($"Not enough cards remaining to draw {CardsToDraw} additional"));
        }


        #region HelperFunctions
        private void ValidateCardsData(List<Models.DeckDraw.Card> Cards)
        {
            foreach (var card in Cards)
            {
                var cardData = DeckDetails.Cards.Where(x => x.code == card.code).FirstOrDefault();
                Assert.That(card.value, Is.EqualTo(cardData.value));
                Assert.That(card.suit, Is.EqualTo(cardData.suit));
                Assert.That(card.image, Is.EqualTo(cardData.image));
                Assert.That(card.images.png, Is.EqualTo(cardData.images.png));
                Assert.That(card.images.svg, Is.EqualTo(cardData.images.svg));
            }
        }
        #endregion
    }
}
