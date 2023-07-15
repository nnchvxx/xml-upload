using XmlToJsonConverter.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddRouting(x => x.LowercaseUrls = true);
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactApp",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000") 
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});
builder.Services.AddScoped<IUtilService, UtilService>();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("ReactApp");

app.MapControllers();

app.Run();

