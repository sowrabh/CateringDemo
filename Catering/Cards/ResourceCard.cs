using AdaptiveCards;
using AdaptiveCards.Templating;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace Catering.Cards
{
    public class CardResource
    {
        private readonly string TeamsTemplateJsonFileName = "TeamsAdaptiveTemplate.json";

        private readonly string _fileName;
        private readonly string _filePath;

        public CardResource(string fileName)
        {
            _fileName = fileName;
            _filePath = Path.Combine(".", "Resources", fileName);
        }

        public string AsJson()
        {
            return EncodeAdaptiveCardForTeams(File.ReadAllText(_filePath));
        }

        public string AsJson<T>(T data)
        {
            var cardJson = File.ReadAllText(_filePath);
            var cardData = JsonConvert.SerializeObject(data);

            var transformer = new AdaptiveTransformer();
            return EncodeAdaptiveCardForTeams(transformer.Transform(cardJson, cardData));
        }

        private string EncodeAdaptiveCardForTeams(string card)
        {
            var teamsJsonTemplatePath = Path.Combine(".", "Resources", TeamsTemplateJsonFileName);
            var teamsCardJson = File.ReadAllText(teamsJsonTemplatePath);

            var base64Card = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(card));
            var data = JObject.FromObject(new {
                encodedAdaptiveCard = base64Card
            }).ToString();

            return new AdaptiveTransformer().Transform(teamsCardJson, data);
        }

        public object AsJObject()
        {
            return JsonConvert.DeserializeObject(AsJson());
        }

        public object AsJObject<T>(T data)
        {
            return JsonConvert.DeserializeObject(AsJson(data));
        }

        public Attachment AsAttachment()
        {
            return new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = JsonConvert.DeserializeObject(AsJson()),
            };
        }
        public Attachment AsAttachment<T>(T data)
        {
            return new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = JsonConvert.DeserializeObject(AsJson(data)),
            };
        }
    }
}
