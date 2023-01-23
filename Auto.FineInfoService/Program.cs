
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddGrpc();
        var app = builder.Build();
        app.MapGrpcService<Auto.OwnerHashService.Services.FineService>();
        app.MapGet("/",
            () => "");
        app.Run();
    }
}