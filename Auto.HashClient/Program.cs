using Grpc.Net.Client;

using var channel = GrpcChannel.ForAddress("https://localhost:7297");
var client = new 

Console.WriteLine("Ready! Press any key to send a gRPC request (or Ctrl-C to quit):");

while (true) {
    Console.ReadKey(true);
}