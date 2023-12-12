using Microsoft.Extensions.Hosting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.BackGroundServices
{
    public class MaterialBackgroundService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Material background service running...");

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}
