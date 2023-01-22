using System.Security.Cryptography;
using System.Text;
using Grpc.Core;
using Auto.FineInfo;
namespace Auto.OwnerHashService.Services;

public class OwnerService : FineInfoService.FineInfoServiceBase
{
    public override Task<FineResponse> GetFineInfo(FineRequest request, ServerCallContext context)
    {
        var i = new Random().Next(0,10);
        var fineInfoString = "";
        switch (i)
        {
            case >= 0 and <= 3:
            {
                fineInfoString = "The fines are paid";
                break;
            }
            case > 3 and <= 5:
            {
                fineInfoString = "The fines are not paid yet";
                break;
            }
            default:
            {
                fineInfoString = "none";
                break;
            }
        }

        return Task.FromResult(new FineResponse(){FineStatus = fineInfoString});
    }
}