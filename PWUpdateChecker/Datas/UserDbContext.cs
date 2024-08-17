using Microsoft.EntityFrameworkCore;
using PWUpdateChecker.Models;

namespace PWUpdateChecker.Datas
{
    public class UserDbContext : DbContext
    {       
        private readonly IConfiguration _configuration;
        public UserDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString, sqlServerOptions =>
            {
                sqlServerOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,      // Số lần thử lại tối đa
                    maxRetryDelay: TimeSpan.FromSeconds(10), // Thời gian trễ giữa các lần thử lại
                    errorNumbersToAdd: null); // Các mã lỗi cụ thể có thể thêm vào
            });
        }
    }
}
