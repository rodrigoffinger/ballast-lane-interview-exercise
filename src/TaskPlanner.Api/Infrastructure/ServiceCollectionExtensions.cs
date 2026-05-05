using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TaskPlanner.Application.Abstractions;
using TaskPlanner.Application.Auth;
using TaskPlanner.Application.Tasks;
using TaskPlanner.Infrastructure.Persistence;
using TaskPlanner.Infrastructure.Persistence.Repositories;
using TaskPlanner.Infrastructure.Security;
using TaskPlanner.Infrastructure.Seeding;
using TaskPlanner.Infrastructure.Time;

namespace TaskPlanner.Api.Infrastructure;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTaskPlannerServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=taskplanner.db";
        var jwtOptions = configuration.GetSection("Jwt").Get<JwtOptions>()
            ?? throw new InvalidOperationException("JWT configuration is missing.");

        services.AddSingleton(new SqliteConnectionFactory(connectionString));
        services.AddSingleton(jwtOptions);
        services.AddScoped<SqliteDatabaseInitializer>();
        services.AddScoped<DatabaseSeeder>();
        services.AddScoped<IUserRepository, SqliteUserRepository>();
        services.AddScoped<ITaskRepository, SqliteTaskRepository>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<ITokenService, JwtTokenService>();
        services.AddSingleton<IClock, SystemClock>();
        services.AddScoped<AuthService>();
        services.AddScoped<TaskService>();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey))
                };
            });

        services.AddAuthorization();

        return services;
    }
}

