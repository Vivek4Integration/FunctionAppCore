using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ReadFromQueue
{
    public static class ReadFromQueue
    {
        [FunctionName("ReadFromQueue")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // using Azure.Messaging.ServiceBus;

            string connectionString = System.Environment.GetEnvironmentVariable("QueueConnection");
            string queueName = System.Environment.GetEnvironmentVariable("QueueName");

            

            // Because ServiceBusClient implements IAsyncDisposable, we'll create it 
            // with "await using" so that it is automatically disposed for us.
            await using var client = new ServiceBusClient(connectionString);
            

            // The receiver is responsible for reading messages from the queue.
            ServiceBusReceiver receiver = client.CreateReceiver(queueName);
            ServiceBusReceivedMessage receivedMessage = await receiver.ReceiveMessageAsync(new TimeSpan(0,0,0,2));


            string body = string.Empty; 
            if (receivedMessage != null)
            body = receivedMessage.Body.ToString();
            Console.WriteLine(body);

            string responseMessage = string.IsNullOrEmpty(body)
                ? "Body is empty"
                : body;

            return new OkObjectResult(responseMessage);
        }
    }
}
