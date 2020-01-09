using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace SimpleProjectWithResiliency
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            int requestNumber = 0;

            app.Map("/retry-timeout", branch =>
            {
                branch.Run(async c =>
                {
                    if (requestNumber < 1)
                    {
                        requestNumber++;
                        await Task.Delay(2000); // Wait 2 seconds to force timeout
                    }
                    c.Response.StatusCode = 200;
                });
            });
        }
    }
}
