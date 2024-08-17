using PWUpdateChecker.Datas;

namespace PWUpdateChecker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public Worker(ILogger<Worker> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<UserDbContext>();

                    var sixMonthsAgo = DateTime.Now.AddMonths(-6);

                    var usersToUpdate = context.Users
                        .Where(u => u.LastUpdatePwd < sixMonthsAgo && u.Status != "REQUIRE_CHANGE_PWD")
                        .ToList();

                    foreach (var user in usersToUpdate)
                    {
                        user.Status = "REQUIRE_CHANGE_PWD";
                        await SendEmailNotification(user.Email);
                    }

                    context.UpdateRange(usersToUpdate);
                    await context.SaveChangesAsync(stoppingToken);
                }

                _logger.LogInformation("Background job executed at: {time}", DateTimeOffset.Now);

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken); // Chạy một lần mỗi ngày
            }
        }

        private Task SendEmailNotification(string email)
        {
            // Thực hiện logic gửi email
            _logger.LogInformation("Email đã được gửi tới {email}", email);
            return Task.CompletedTask;
        }
    }
}
