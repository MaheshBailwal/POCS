//using System;
//using System.Collections.Generic;
//using System.Linq;
//using IdentityServer4.Models;
//using IdentityServer4.Validation;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.DependencyInjection;


//namespace SVM.Enterprise.Users.Infrastructure.Extensions
//{
//    public static class IdentityExtensions
//    {
//        public static IIdentityServerBuilder AddDbScopeValidator<TScopeValidator>(this IIdentityServerBuilder builder)
//            where TScopeValidator : ScopeValidator
//        {
//            var scopeValidatorRegistration = builder.Services.LastOrDefault(x => x.ServiceType == typeof(IdentityServer4.Validation.ScopeValidator));

//            if (scopeValidatorRegistration == null)
//            {
//                throw new InvalidOperationException($"Service type: {typeof(IdentityServer4.Validation.ScopeValidator).Name} not registered.");
//            }
//            builder.Services.Remove(scopeValidatorRegistration);

//            builder.Services.AddTransient<ScopeValidator, TScopeValidator>();
//            return builder;
//        }

//        public static ApiResource CloneWithScopes(this ApiResource apiResource, IEnumerable<Scope> scopes)
//        {
//            return new ApiResource
//            {
//                Enabled = apiResource.Enabled,
//                Name = apiResource.Name,
//                ApiSecrets = apiResource.ApiSecrets,
//                Scopes = new HashSet<Scope>(scopes.ToArray()),
//                UserClaims = apiResource.UserClaims
//            };
//        }

//        public static IIdentityServerBuilder AddUserStore(this IIdentityServerBuilder builder)
//        {
//            builder.Services.AddTransient<IUserClaimsPrincipalFactory, UserClaimsPrincipalFactory>();
//            builder.AddResourceOwnerValidator<ResourceOwnerPasswordValidator>();
//            builder.AddProfileService<ProfileService>();
//            builder.AddResourceStore<DbResourceStore>();
//            builder.AddClientStore<ClientStore>();
//            return builder;
//        }

//        internal static void AddTransientDecorator<TService, TImplementation>(this IServiceCollection services)
//            where TService : class
//            where TImplementation : class, TService
//        {
//            services.AddDecorator<TService>();
//            services.AddTransient<TService, TImplementation>();
//        }

//        //internal static void AddDecorator<TService>(this IServiceCollection services)
//        //{
//        //    var registration = services.LastOrDefault(x => x.ServiceType == typeof(TService));
//        //    if (registration == null)
//        //    {
//        //        throw new InvalidOperationException("Service type: " + typeof(TService).Name + " not registered.");
//        //    }
//        //    if (services.Any(x => x.ServiceType == typeof(Decorator<TService>)))
//        //    {
//        //        throw new InvalidOperationException("Decorator already registered for type: " + typeof(TService).Name + ".");
//        //    }

//        //    services.Remove(registration);

//        //    if (registration.ImplementationInstance != null)
//        //    {
//        //        var type = registration.ImplementationInstance.GetType();
//        //        var innerType = typeof(Decorator<,>).MakeGenericType(typeof(TService), type);
//        //        services.Add(new ServiceDescriptor(typeof(Decorator<TService>), innerType, ServiceLifetime.Transient));
//        //        services.Add(new ServiceDescriptor(type, registration.ImplementationInstance));
//        //    }
//        //    else if (registration.ImplementationFactory != null)
//        //    {
//        //        services.Add(new ServiceDescriptor(typeof(Decorator<TService>), provider => new DisposableDecorator<TService>((TService)registration.ImplementationFactory(provider)), registration.Lifetime));
//        //    }
//        //    else
//        //    {
//        //        var type = registration.ImplementationType;
//        //        var innerType = typeof(Decorator<,>).MakeGenericType(typeof(TService), registration.ImplementationType);
//        //        services.Add(new ServiceDescriptor(typeof(Decorator<TService>), innerType, ServiceLifetime.Transient));
//        //        services.Add(new ServiceDescriptor(type, type, registration.Lifetime));
//        //    }
//        //}

//        internal static bool IsMissing(this string value)
//        {
//            return string.IsNullOrWhiteSpace(value);
//        }

//        internal static bool IsPresent(this string value)
//        {
//            return !string.IsNullOrWhiteSpace(value);
//        }

//        internal static bool HasExceeded(this DateTime creationTime, int seconds, DateTime now)
//        {
//            return (now > creationTime.AddSeconds(seconds));
//        }

//        internal static int GetLifetimeInSeconds(this DateTime creationTime, DateTime now)
//        {
//            return ((int)(now - creationTime).TotalSeconds);
//        }

//        internal static string GetRequestAddress(this IHttpContextAccessor context)
//        {
//            string requestAddress = string.Empty;
//            string remoteIpAddress = context.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
//            if (context.HttpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
//            {
//                remoteIpAddress = context.HttpContext.Request.Headers["X-Forwarded-For"];

//                if (remoteIpAddress.Contains(','))
//                {
//                    var clientIP = remoteIpAddress.Split(',').ToList();
//                    remoteIpAddress = clientIP.LastOrDefault();
//                }

//                if (remoteIpAddress.Contains(':'))
//                {
//                    requestAddress = remoteIpAddress.Split(':')[0].Trim();
//                }
//                else
//                {
//                    requestAddress = remoteIpAddress.Trim();
//                }
//            }
//            return requestAddress;
//        }
//    }
//}