using Crud;
using Grpc.Core;
using Grpc.Net.Client;
using System;

namespace GrpcConsoleClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string arg;
            if (args.Length == 0) arg = "https://localhost:7144";
            else arg = args[0];
            WorkerService.WorkerServiceClient client;

            // создаем канал для обмена сообщениями с сервером
            // параметр - адрес сервера gRPC
            using var channel = GrpcChannel.ForAddress(arg);

            // создаем клиент
            client = new WorkerService.WorkerServiceClient(channel);
            // формируем отправляемые заголовки
            Metadata requestHeaders = new Metadata();
            // добавляем один заголовок
            Guid guid = Guid.NewGuid();
            requestHeaders.Add("guid", guid.ToString());

            try
            {
                // получение списка объектов
                ListReply workers = await client.ListWorkersAsync(new Google.Protobuf.WellKnownTypes.Empty(), requestHeaders);

                foreach (var worker in workers.Workers)
                {
                    Console.WriteLine($"{worker.Id}. {worker.LastName} - {worker.Birthday} - {worker.Sex}");
                }
            }
            catch (RpcException ex)
            {
                Console.WriteLine(ex.Status.Detail);    // получаем статус ответа
            }


            // посылаем пустое сообщение и получаем набор сообщений
            var serverData = client.GetWorkerStream(new EmptyMessage(), requestHeaders);

            // получаем поток сервера
            var responseStream = serverData.ResponseStream;
            // с помощью итераторов извлекаем каждое сообщение из потока
            var task = Task.Run(async () =>
            {
                while (await responseStream.MoveNext(new CancellationToken()))
                {
                    var response = responseStream.Current;
                    await Console.Out.WriteLineAsync($"{response.ActionType} - {response.WorkerMessage.LastName}");

                }

            });
            
            

            int del = int.Parse(Console.ReadLine());
            while (del != 0)
            {
                try
                {
                    // удаление объекта с id = 2
                    WorkerEntiti workerEntiti = await client.DeleteWorkerAsync(new WorkerId { Id = del }, requestHeaders);
                    Console.WriteLine($"удалён {workerEntiti.Id}. {workerEntiti.LastName} - {workerEntiti.Sex}");
                }
                catch (RpcException ex)
                {
                    Console.WriteLine(ex.Status.Detail);
                }
                del = int.Parse(Console.ReadLine());
            }   
            task.Wait();
            await Console.Out.WriteLineAsync("END");

        }
    }
}