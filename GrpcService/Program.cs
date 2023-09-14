using CrudGrpcApp.Services;
using Microsoft.EntityFrameworkCore;

namespace GrpcService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ������ �����������
            string connStr = "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=yfrfpe.obq";
            // ��������� �������� ApplicationContext � �������� ������� � ����������
            builder.Services.AddDbContext<ApplicationContext>(options => options.UseNpgsql(connStr));
            builder.Services.AddGrpc();

            var app = builder.Build();


            app.MapGrpcService<WorkerApiService>();
            app.MapGet("/", () => "AdvRecProject");

            app.Run();
        }
    }
}