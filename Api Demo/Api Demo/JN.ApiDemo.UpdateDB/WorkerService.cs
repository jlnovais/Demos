using System;
using System.Threading;
using System.Threading.Tasks;
using JN.ApiDemo.Identity;
using JN.ApiDemo.Identity.Data;
using JN.ApiDemo.Identity.Domain;
using JN.ApiDemo.Identity.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JN.ApiDemo.UpdateDB
{
    public class WorkerService : IHostedService
    {
        private readonly IdentityDataContext _identityDataContext;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IIdentityService _identityService;

        private TaskCompletionSource<bool> TaskCompletionSource { get; } = new TaskCompletionSource<bool>();
        private CancellationTokenSource CancellationTokenSource { get; } = new CancellationTokenSource();
        private ILogger<WorkerService> _logger { get; }
        private IHostApplicationLifetime _appLifetime { get; }


        public WorkerService(ILogger<WorkerService> logger, IHostApplicationLifetime appLifetime,
            IdentityDataContext identityDataContext,
            RoleManager<ApplicationRole> roleManager,
            IIdentityService identityService
        )
        {
            _identityDataContext = identityDataContext;
            _roleManager = roleManager;
            _identityService = identityService;
            _logger = logger;
            _appLifetime = appLifetime;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _appLifetime.ApplicationStarted.Register(OnStarted);
            _appLifetime.ApplicationStopping.Register(OnStopping);
            _appLifetime.ApplicationStopped.Register(OnStopped);

            Task.Run(() => DoWork(CancellationTokenSource.Token));
            return Task.CompletedTask;
        }

 
        public Task StopAsync(CancellationToken cancellationToken)
        {
            CancellationTokenSource.Cancel();
            // Defer completion promise, until our application has reported it is done.
            return TaskCompletionSource.Task;
        }


        public async Task DoWork(CancellationToken cancellationToken)
        {

            if (!await _identityDataContext.Database.CanConnectAsync(cancellationToken))
            {
                _logger.LogInformation("DB not available");
                return;
            }

            _logger.LogInformation("DB available; starting migrations");
            await _identityDataContext.Database.MigrateAsync(cancellationToken);


            var values = Enum.GetValues(typeof(IdentityConstants.UserRoles));

            foreach (IdentityConstants.UserRoles value in values)
            {
                await CreateRole(value.ToString());
            }

            var user = new ApplicationUser()
            {
                FirstName = "administrator",
                Active = true,
                Description = "administrator",
                UserName = "admin",
                Email = "defaultEmail@email.email"
            };

            await _identityService.RegisterUserAsync(user, "Password!123",
                new[] {IdentityConstants.UserRoles.Admin.ToString()});

            _appLifetime.StopApplication();

            TaskCompletionSource.SetResult(true);

        }

        private async Task CreateRole(string roleName)
        {
            _logger.LogInformation($"Creating role {roleName}");
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var role = new ApplicationRole(roleName);
                await _roleManager.CreateAsync(role);
            }
        }


        private void OnStarted()
        {
            _logger.LogInformation("Starting DB update...");
            // Handle OnStarted
        }

        private void OnStopping()
        {
            _logger.LogInformation("Stopping...");
            // Handle OnStopping
        }

        private void OnStopped()
        {
            _logger.LogInformation("Stopped");
            // Handle OnStopped


        }
    }
}