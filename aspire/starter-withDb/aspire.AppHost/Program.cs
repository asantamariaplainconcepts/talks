using aspire.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis")
    .WithRedisCommander();

var sql = builder.AddPostgres("sql")
    .WithDataVolume("aspire-talk-with-sql-data")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgWeb();

var db = sql.AddDatabase("todosdb");

builder.AddProject<Projects.aspire_ApiDbService>("apidbservice")
    .WithReference(db)
    .WaitFor(db)
    .WithHttpsHealthCheck("/health")
    .WithHttpsCommand("/reset-db", "Reset Database", iconName: "DatabaseLightning");

var apiService = builder
    .AddProject<Projects.aspire_ApiService>("apiservice")
    .WithReference(redis)
    .WithReference(db)
    .WaitFor(db);

builder.AddNpmApp("vue", "../spa")
    .WithReference(apiService)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints();

builder.Build().Run();