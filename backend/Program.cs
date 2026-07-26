using DotX.Api.Grpc.Clients;
using DotX.Api.Grpc.Protos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Configure gRPC Client
builder.Services.AddGrpcClient<AuthService.AuthServiceClient>(o =>
{
    // TODO: move to appsettings.json
    o.Address = new Uri("https://localhost:5001");
});
builder.Services.AddScoped<UserGrpcClient>();

var app = builder.Build();

app.UseRouting();
app.MapControllers();

app.Run();
