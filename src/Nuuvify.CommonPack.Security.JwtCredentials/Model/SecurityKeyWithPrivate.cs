using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Nuuvify.CommonPack.Security.JwtCredentials.Model
{

    public class SecurityKeyWithPrivate
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Parameters { get; set; }
        public string KeyId { get; set; }
        public string Type { get; set; }
        public string Algorithm { get; set; }
        public DateTimeOffset CreationDate { get; set; }



        public JsonWebKey GetSecurityKey()
        {
            return JsonSerializer.Deserialize<JsonWebKey>(Parameters);
        }


        public SigningCredentials GetSigningCredentials()
        {
            return new SigningCredentials(GetSecurityKey(), Algorithm);
        }

        public void SetParameters(SecurityKey key, Algorithm alg)
        {
            Parameters = JsonSerializer.Serialize(key, typeof(JsonWebKey),
                new JsonSerializerOptions() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, });

            Type = alg.Kty();
            KeyId = key.KeyId;
            Algorithm = alg;
            CreationDate = DateTimeOffset.Now;
        }

        public void SetParameters()
        {
            var jsonWebKey = GetSecurityKey();
            var publicWebKey = PublicJwk.FromJwk(jsonWebKey);

            Parameters = JsonSerializer.Serialize(publicWebKey.ToNativeJwk(),
                new JsonSerializerOptions() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });
        }
    }
}