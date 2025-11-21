var builder = DistributedApplication.CreateBuilder(args);

// Add dependencies
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgAdmin();

var database = postgres.AddDatabase("myapidb");

var redis = builder.AddRedis("redis")
    .WithDataVolume()
    .WithRedisCommander();

// Add the API project with references to dependencies
var api = builder.AddProject<Projects.MyApi_Api>("myapi")
    .WithReference(database)
    .WithReference(redis);

builder.Build().Run();
