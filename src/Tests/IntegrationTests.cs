using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace Tests
{
    public abstract class IntegrationTests<TStartup> : IAsyncDisposable where TStartup : class
    {
        private readonly WebApplicationFactory<TStartup> _factory;

        protected IntegrationTests() : this(typeof(TStartup).Assembly.GetName().Name)
        {
        }

        protected IntegrationTests(string projectRelativePath)
        {
            _factory = new WebApplicationFactory<TStartup>().WithWebHostBuilder(b => b.UseSolutionRelativeContentRoot(Path.Combine("src", projectRelativePath)));
            Client = _factory.CreateDefaultClient(new Uri("http://google.com/"));
            Server = _factory.Server;
        }

        public TestServer Server { get; }

        protected HttpClient Client { get; }

        public virtual ValueTask DisposeAsync()
        {
            _factory.Dispose();
            return new ValueTask();
        }
    }
}
