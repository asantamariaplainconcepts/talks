var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis")
    .WithRedisCommander();

var sql = builder.AddPostgres("sql")
    .WithDataVolume("sql-data")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgWeb();

var db = sql.AddDatabase("aspire");

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
