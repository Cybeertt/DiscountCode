using System;
using Grpc.Net.Client;
using DiscountCodeServer.Protos;

// Enable insecure HTTP/2 support
AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

// Create channel
var channel = GrpcChannel.ForAddress("http://localhost:5023");
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
    Console.WriteLine($"Error: {ex.Message}");
}

Console.ReadKey();