namespace TemporalioSamples.ClientAPIKey;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Temporalio.Client;
using Temporalio.Worker;

public class ClientAPIKey
{
    private static async Task Main(string[] args)
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancelToken = cancellationTokenSource.Token;

        var client = await TemporalClient.ConnectAsync(new("us-east-1.aws.api.temporal.io:7233")
        {
            Namespace = "testjlapikey.a2dd6",
            ApiKey = Environment.GetEnvironmentVariable("TEMPORAL_DOTNET_API_KEY"),
            RpcMetadata = new Dictionary<string, string>()
            {
                ["temporal-namesace"] = "testjlapikey.a2dd6",
            },
            Tls = new(),
        });

        using var worker = new TemporalWorker(
            client,
            new TemporalWorkerOptions(taskQueue: "api-key-test-sample").
                AddWorkflow<GreetingWorkflow>());

        var result = await client.ExecuteWorkflowAsync(
            (GreetingWorkflow wf) => wf.RunAsync("Temporal"),
            new(id: "client-mtls-workflow-id", taskQueue: "api-key-test-sample")
            {
                Rpc = new() { CancellationToken = cancelToken },
            });
        Console.WriteLine("Workflow result: {0}", result);
    }
}