using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SupabaseApiCall.Infrastructure
{
    // Questa classe è pensata per contenere il metodo di validazione dei JWT token.
    // Questi attualmente vengono generati in Go, l'algoritmo utilizzato è HS256 (HMac + SHA-256).
    // La chiave segreta è condivisa tra i due servizi, attualmente contenuta nel appsetting.json.

    public static class JWTValidator
    {
        public static IServiceCollection AddJwtValidator(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var jwtKey = configuration["JWT:Key"]
                ?? throw new InvalidOperationException("Auth Key mancante");

            
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                              Encoding.UTF8.GetBytes(jwtKey)
                        ),

                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,

                        ValidateIssuer = false,
                        ValidateAudience = false,

                        ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 }
                    };

                   
                });

            services.AddAuthorization();

            return services;
        }
    }


}
