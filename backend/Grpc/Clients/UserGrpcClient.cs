using System.Threading.Tasks;
using DotX.Api.Grpc.Protos;

namespace DotX.Api.Grpc.Clients;

public class UserGrpcClient
{
    private readonly AuthService.AuthServiceClient _client;

    public UserGrpcClient(AuthService.AuthServiceClient client)
    {
        _client = client;
    }

    public async Task<UserResponse> GetUserByApiKeyAsync(string apiKey)
    {
        var request = new ApiKeyRequest { ApiKey = apiKey };
        return await _client.GetUserByApiKeyAsync(request);
    }
}
