using System;
using Grpc.Net.Client;
using DiscountCodeServer.Protos;

class Program
{
    static async Task Main(string[] args)
    {
        // Enable support for insecure HTTP/2 (development only!)
        AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

        // Create channel with explicit HTTP/2 configuration
        var channel = GrpcChannel.ForAddress("https://localhost:5023");

        var client = new DiscountCodeService.DiscountCodeServiceClient(channel);

        try
        {
            Console.WriteLine("Testing connection...");
            var response = await client.GenerateCodesAsync(
                new GenerateRequest { Count = 1, Length = 8 });

            Console.WriteLine($"Success! Result: {response.Result}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed: {ex.Message}");
        }

        Console.ReadKey();
    }
}