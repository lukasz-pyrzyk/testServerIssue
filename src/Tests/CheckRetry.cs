using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using Xunit;

namespace Tests
{
    public class CheckRetry : IntegrationTests<SimpleProjectWithResiliency.Startup>
    {
        private const string RemoteEndpoint = "https://localhost:5001";
        private const string ClientName = "randomClient";

        [Fact]
        public async Task ShouldRetry_OnInMemoryInstance()
        {
            var client = BuildClient(ClientName, useInMemoryInstance: true);

            var result = await client.GetAsync("/retry-timeout");

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }
        
        [Fact]
        public async Task ShouldRetry_OnRemoteInstance()
        {
            var client = BuildClient(ClientName, useInMemoryInstance: false);

            var result = await client.GetAsync("/retry-timeout");

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        private HttpClient BuildClient(string clientName, bool useInMemoryInstance)
        {
            var serviceCollection = new ServiceCollection();

            IHttpClientBuilder builder;

            if (useInMemoryInstance)
            {
                builder = serviceCollection.AddHttpClient(clientName, x => { x.BaseAddress = Server.BaseAddress; })
                    .ConfigurePrimaryHttpMessageHandler(Server.CreateHandler);
            }
            else
            {
                builder = serviceCollection.AddHttpClient(clientName, x =>
                {
                    x.BaseAddress = new Uri(RemoteEndpoint);
                });
            }

            builder.AddPolicyHandler((services, request) =>
            {
                var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(1));
                var retryPolicy = HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .Or<TimeoutRejectedException>()
                    .WaitAndRetryAsync(new[]
                    {
                        TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(3)
                    });

                return retryPolicy.WrapAsync(timeoutPolicy);
            });

            var services = serviceCollection.BuildServiceProvider(true);
            var factory = services.GetService<IHttpClientFactory>();
            var client = factory.CreateClient(clientName);
            return client;
        }
    }
}
