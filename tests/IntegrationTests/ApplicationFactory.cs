using Core.Dto;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Utility.Factories;
using static Core.Dto.Response;

namespace IntegrationTests;

public abstract class ApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    private const string ConnectionString =
        "Server=(localdb)\\mssqllocaldb;Database=GalaTest;Trusted_Connection=True;MultipleActiveResultSets=true";
    private TokenableResponse? _tokenableResponse;
    public HttpResponseMessage Response { get; protected set; } = null!;
    public HttpStatusCode ResponseStatusCode => Response.StatusCode;
    protected HttpClient Client { get; private set; } = null!;
    public Bro Bro { get; private set; } = null!;
    protected abstract string DefaultStartUrl { get; }

    public override async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await base.DisposeAsync();
    }

    public void Initialize()
    {
        Client = CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    public async Task InitAndSetTokenAsync()
    {
        Initialize();
        await LoginThenSetBearerTokenInClientForRequests();
    }

    public async Task ResetDatabase()
    {
        await DatabaseInitializer.ResetDatabase(ConnectionString);
    }

    public void ClearToken()
    {
        _tokenableResponse = null;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(IDbContextOptionsConfiguration<ApplicationDbContext>));

            services.Remove(dbContextDescriptor!);

            var dbConnectionDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbConnection));

            services.Remove(dbConnectionDescriptor!);

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(ConnectionString);
            });
        });

        builder.UseEnvironment("Test");
    }

    private async Task LoginThenSetBearerTokenInClientForRequests()
    {
        Bro = await BroFactory.CreateBroAsync("test@example.net", "Hey@123", Services);
        var credentials = new LoginCredentials("test@example.net", "Hey@123");
        Response = await Client.PostAsJsonAsync("api/Sessions", credentials);

        SetBearerTokenInHeaders();
    }

    private void SetBearerTokenInHeaders()
    {
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            TokenableResponse().Result?.Token);
    }

    private async Task<TokenableResponse?> TokenableResponse()
    {
        return _tokenableResponse ??= await Response.Content.ReadFromJsonAsync<TokenableResponse>();
    }
}