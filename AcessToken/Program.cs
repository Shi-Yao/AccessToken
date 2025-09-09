using AcessToken;
using AcessToken.DbSetting;
using AcessToken.Repository;
using Microsoft.OpenApi.Models;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddSingleton<IDbSettings, DbSettings>();
builder.Services.AddSingleton<DapperDbContext>();
builder.Services.AddScoped<IApiClientRepository, ApiClientRepository>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379"; // �A�� Redis �s�u�r��
    options.InstanceName = "TokenCache:";     // �i��A�[�e���קK key �Ĭ�
});

// �K�[ Swagger �ͦ���
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

    // �K�[�w���w�q
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // �K�[�w���n�D
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
