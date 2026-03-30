using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod());
});
builder.Services.AddHttpClient<HackerNewsAggregator.Services.IHackerNewsClient, HackerNewsAggregator.Services.HackerNewsClient>(client =>
{
    client.BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/");
});
builder.Services.AddScoped<HackerNewsAggregator.Services.IHackerNewsService, HackerNewsAggregator.Services.HackerNewsService>();
builder.Services.AddScoped<HackerNewsAggregator.Services.IHackerNewsStorySearchService, HackerNewsAggregator.Services.HackerNewsStorySearchService>();
builder.Services.Configure<HackerNewsAggregator.Services.HackerNewsOptions>(builder.Configuration.GetSection("HackerNews"));
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
