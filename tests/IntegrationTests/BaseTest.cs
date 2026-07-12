namespace IntegrationTests;

public class BaseTest(ApplicationFactory<Program> app, bool isAuthenticated = true) : IAsyncLifetime
{
    public virtual Task DisposeAsync() => Task.CompletedTask;

    public async Task InitializeAsync()
    {
        await app.ResetDatabase();
        await InitializeClientAsync();
    }

    private async Task InitializeClientAsync()
    {
        if (isAuthenticated)
        {
            await app.InitAndSetTokenAsync();
        }
        else
        {
            app.Initialize();
        }
    }
}