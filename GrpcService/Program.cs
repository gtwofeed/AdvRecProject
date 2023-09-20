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
            string connStr = builder.Configuration["connStr"];
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