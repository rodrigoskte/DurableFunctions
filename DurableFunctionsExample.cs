using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace Company.Function
{
    public static class DurableFunctionsExample
    {
        [Function(nameof(DurableFunctionsExample))]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] TaskOrchestrationContext context)
        {
            ILogger logger = context.CreateReplaySafeLogger(nameof(DurableFunctionsExample));
            logger.LogInformation("Saying hello.");
            var outputs = new List<string>();

            // Replace name and input with values relevant for your Durable Functions Activity
            outputs.Add(await context.CallActivityAsync<string>(nameof(BuscarProduto), "Bicicleta"));
            outputs.Add(await context.CallActivityAsync<string>(nameof(EfetuarPagamento), "Bicicleta"));
            outputs.Add(await context.CallActivityAsync<string>(nameof(EntregarProduto), "Bicicleta"));
            return outputs;
        }

        [Function(nameof(BuscarProduto))]
        public static string BuscarProduto([ActivityTrigger] string name, FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("BuscarProduto");
            logger.LogInformation("Buscando produto: {name}.", name);
            return $"Buscando produto: {name}!";
        }

        [Function(nameof(EfetuarPagamento))]
        public static string EfetuarPagamento([ActivityTrigger] string name, FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("EfetuarPagamento");
            logger.LogInformation("Efetuando pagamento: {name}.", name);
            return $"Efetuando pagamento: {name}!";
        }

        [Function(nameof(EntregarProduto))]
        public static string EntregarProduto([ActivityTrigger] string name, FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("EntregarProduto");
            logger.LogInformation("Entregar produto: {name}.", name);
            return $"Entregar produto: {name}!";
        }

        [Function("DurableFunctionsExample_HttpStart")]
        public static async Task<HttpResponseData> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
            [DurableClient] DurableTaskClient client,
            FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("DurableFunctionsExample_HttpStart");

            // Function input comes from the request content.
            string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
                nameof(DurableFunctionsExample));

            logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

            // Returns an HTTP 202 response with an instance management payload.
            // See https://learn.microsoft.com/azure/azure-functions/durable/durable-functions-http-api#start-orchestration
            return client.CreateCheckStatusResponse(req, instanceId);
        }
    }
}
