﻿using System;
using Nuuvify.CommonPack.Security.JwtCredentials;
using Nuuvify.CommonPack.Security.JwtCredentials.Interfaces;
using Nuuvify.CommonPack.Security.JwtCredentials.Jwk;
using Nuuvify.CommonPack.Security.JwtCredentials.Jwks;
using Nuuvify.CommonPack.Security.JwtCredentials.Jwt;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class JwkSetManagerDependencyInjection
    {
        /// <summary>
        /// Sets the signing credential.
        /// </summary>
        /// <returns></returns>
        public static IJwksBuilder AddJwksManager(this IServiceCollection services, 
            Action<JwksOptions> action = null)
        {
            if (action != null)
                services.Configure(action);

            services.AddScoped<IJwkService, JwkService>();
            services.AddScoped<IJwkSetService, JwkSetService>();
            services.AddSingleton<IJwkStore, InMemoryStore>();

            return new JwksBuilder(services);
        }

        /// <summary>
        /// Sets the signing credential.
        /// </summary>
        /// <returns></returns>
        public static IJwksBuilder PersistKeysInCache(this IJwksBuilder builder)
        {
            builder.Services.AddSingleton<IJwkStore, InMemoryStore>();

            return builder;
        }


        /// <summary>
        /// Configura AddDistributedSqlServerCache para armazenamento de tokens, a tabela deve ser criada
        /// no banco conforme exemplo: 
        /// <example>
        /// <code>
        /// CREATE TABLE Audicon.CacheToken
        /// (
        ///     Id nvarchar(449) COLLATE SQL_Latin1_General_CP1_CS_AS NOT NULL,
        ///     Value varbinary(MAX) NOT NULL,
        ///     ExpiresAtTime datetimeoffset NOT NULL,
        ///     SlidingExpirationInSeconds bigint NULL,
        ///     AbsoluteExpiration datetimeoffset NULL,
        ///     CONSTRAINT Pk_CacheToken_Id PRIMARY KEY(Id)
        /// )
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="connectionString">String para conexão no banco SqlServer</param>
        /// <param name="schemaName">Nome do schema do banco onde ficara a tabela CacheToken, exemplo: Audicon.CacheToken</param>
        /// <param name="tableName">Nome da tabela usada para armazenar o token</param>
        public static IJwksBuilder CacheTokenSetup(this IJwksBuilder builder,
            string connectionString, string schemaName, string tableName = "CacheTokens")
        {

            builder.Services.AddScoped<IJwtSetService, JwtSetService>();

            builder.Services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = connectionString;

                options.SchemaName = schemaName;
                options.TableName = tableName;
            });

            return builder;


        }




    }
}
