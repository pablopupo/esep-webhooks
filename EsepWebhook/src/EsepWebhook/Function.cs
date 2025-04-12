using Amazon.Lambda.Core;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace EsepWebhook;

public class Function
{
    private static readonly HttpClient client = new HttpClient();
    
    public string FunctionHandler(object input, ILambdaContext context)
    {
        dynamic json = JsonConvert.DeserializeObject<dynamic>(input.ToString());
        
        string payload = $"{{\"text\":\"Issue Created: {json.issue.html_url}\"}}";
        
        string webhookUrl = Environment.GetEnvironmentVariable("SLACK_URL");
        
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, webhookUrl)
        {
            Content = new StringContent(payload, Encoding.UTF8, "application/json")
        };
        
        var response = client.Send(httpRequest);
        using var reader = new StreamReader(response.Content.ReadAsStream());
        
        return reader.ReadToEnd();
    }
}
