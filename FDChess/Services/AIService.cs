using System.ClientModel;
using System.Text.Json;
using AutoGen.Core;
using AutoGen.OpenAI;
using AutoGen.OpenAI.Extension;
using FDChess.Model;
using Json.More;
using Microsoft.SemanticKernel.Agents;
using OpenAI;

namespace FDChess.Services
{
    public class AIService
    {

        // endpoint and API key
        private static string apiKey = "lm-studio";

        private static string endpoint = new string ("http://localhost:1234");

        // openai client
        private static OpenAIClient? _client;
        private MiddlewareStreamingAgent<OpenAIChatAgent> _lmagent;

        public AIService()
        {
            _client = new OpenAIClient(apiKey, new OpenAIClientOptions
            {
                Endpoint = new Uri(endpoint),
            });

            Console.WriteLine("Creating the agent!");
            CreateAgent();
            Console.WriteLine("Agent Created");
        }

        // Create a new AI agent
        public void CreateAgent()
        {
            if (_client == null)
            {
                throw new InvalidOperationException("OpenAI client is not initialized.");
            }
            _lmagent = new MiddlewareStreamingAgent<OpenAIChatAgent>(new OpenAIChatAgent(_client.GetChatClient("<does-not-matter>"),
                    name: "assistant")
                .RegisterMessageConnector()
                .RegisterPrintMessage());
        }

        // Use the agent to describe the board
        public async Task<string> DescribeBoardAsync(Board board)
        {
            var messages = new List<IMessage>
            {
                new TextMessage(Role.User, $"Describe the following chess board: {board}")
            };

            var response = await _lmagent.GenerateReplyAsync(messages);
            var descriptionContent = response.GetContent() ?? "No description available.";

            // Return the description as a JSON string
            return JsonSerializer.Serialize(new { description = descriptionContent });
        }
    }
}
