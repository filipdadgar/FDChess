
using FDChess.Helper;
using FDChess.Interfaces;
using FDChess.Services;
using Google.Api;
using Microsoft.FeatureManagement;


namespace FDChess
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var configuration = builder.Configuration;
            builder.Services.AddFeatureManagement(configuration.GetSection("FeatureManagement"));

            var featureManager = builder.Services.BuildServiceProvider().GetRequiredService<IFeatureManager>();

            // Add services to the container.
            if (await featureManager.IsEnabledAsync("AIService"))
            {
                builder.Services.AddSingleton<AIService>();
            }
            builder.Services.AddSingleton<IChessService>(sp =>
            {
                var aiService = sp.GetService<AIService>();
                return new ChessService(featureManager, aiService);
            });

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new PieceConverter());
                });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Add CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseCors("AllowAllOrigins");

            app.MapControllers();

            await app.RunAsync();
        }
    }
}
