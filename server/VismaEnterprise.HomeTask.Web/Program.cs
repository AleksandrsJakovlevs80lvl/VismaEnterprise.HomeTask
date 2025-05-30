using VismaEnterprise.HomeTask.Web.Configurations;
using VismaEnterprise.HomeTask.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHomeTaskAuthentication(builder);
builder.Services.AddHomeTaskDatabase(builder);
builder.Services.AddBookCatalogue(builder);
builder.Services.AddSwagger(builder);

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAngularDevClient", policy => policy.WithOrigins("http://localhost:8081").AllowAnyHeader().AllowAnyMethod());
    });
}

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowAngularDevClient");
}

app.Run();