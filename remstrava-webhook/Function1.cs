using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Messaging.EventHubs.Producer;
using Azure.Messaging.EventHubs;

namespace remstrava_webhook
{
    public static class Function1
    {
        [FunctionName("remstrava-webhook")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            MainAsync().GetAwaiter().GetResult();

            string responseMessage = "OK";

            return new OkObjectResult(responseMessage);
        }

        private static async Task MainAsync()
        {
            var eventHubName = "newactivity";

            var connectionString = System.Environment.GetEnvironmentVariable("EventHubConnectionString", EnvironmentVariableTarget.Process);

            // Create a producer client that you can use to send events to an event hub
            var producerClient = new EventHubProducerClient(connectionString, eventHubName);

            // Create a batch of events 
            using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();

            eventBatch.TryAdd(new EventData("Event 1"));

            try
            {
                // Use the producer client to send the batch of events to the event hub
                await producerClient.SendAsync(eventBatch);
                Console.WriteLine($"A batch of 1 event has been published.");
            }
            finally
            {
                await producerClient.DisposeAsync();
            }
            Console.WriteLine("Press ENTER to exit.");
            Console.ReadLine();
        }
    }
}
