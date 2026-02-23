using Challenge.CRM.Rommanel.Api;

await WebApplication.CreateBuilder(args)
    .RegisterServices().Build()
    .UseServices().RunAsync();