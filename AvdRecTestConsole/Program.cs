using GrpcService.Models;

namespace AvdRecTestConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Worker worker = new();
            var a = Top.Down.ToString();
            var enumDataSource = System.Enum.GetValues(typeof(AvdRecTestConsole.Top));
            Console.WriteLine(enumDataSource);

        }
    }
    internal enum Top { Up, Down}
}