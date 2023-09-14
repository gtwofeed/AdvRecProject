using Crud;
using Grpc.Core;
using Grpc.Net.Client;

namespace GrpcConsoleClient1
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string arg;
            if (args.Length == 0) arg = "https://localhost:5001";
            else arg = args[0];
            WorkerService.WorkerServiceClient client;

            // создаем канал для обмена сообщениями с сервером
            // параметр - адрес сервера gRPC
            using var channel = GrpcChannel.ForAddress(arg);

            // создаем клиент
            client = new WorkerService.WorkerServiceClient(channel);

            try
            {
                // получение списка объектов
                ListReply workers = await client.ListWorkersAsync(new Google.Protobuf.WellKnownTypes.Empty());

                foreach (var worker in workers.Workers)
                {
                    Console.WriteLine($"{worker.Id}. {worker.LastName} - {worker.Birthday} - {worker.Sex}");
                }
            }
            catch (RpcException ex)
            {
                Console.WriteLine(ex.Status.Detail);    // получаем статус ответа
            }
            await Console.Out.WriteAsync("Введите ид: ");
            int id = int.Parse(Console.ReadLine());
            try
            {
                // удаление объекта с id = 2
                WorkerReply worker = client.DeleteWorker(new DeleteWorkerRequest { Id = id });
                Console.WriteLine($"{worker.Id}. {worker.LastName} - {worker.Birthday} - {worker.Sex}");
            }
            catch (RpcException ex)
            {
                Console.WriteLine(ex.Status.Detail);
            }
        }
    }
}