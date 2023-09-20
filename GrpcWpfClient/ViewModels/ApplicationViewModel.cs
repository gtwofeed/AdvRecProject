using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GrpcWpfClient.Models;
using Grpc.Net.Client;
using GrpcWpfClient.Views;
using Grpc.Core;
using System.Windows;
using System.Threading;
using Crud;

namespace GrpcWpfClient.ViewModels
{
    public class ApplicationViewModel
    {
        public ApplicationViewModel()
        {
            // создаем канал для обмена сообщениями с сервером
            // параметр - адрес сервера gRPC
            // string connStr = "https://localhost:7144";
            string connStr = Properties.Settings.Default.ConnectService;

            // создаем канал для обмена сообщениями с сервером
            // параметр - адрес сервера gRPC
            var channel = GrpcChannel.ForAddress(connStr);

            // создаем клиент
            WorkerServiceClient = new WorkerService.WorkerServiceClient(channel);

            // добавляем один заголовок
            requestHeaders.Add("guid", guid);
            deadline = new DateTime(DateTime.Now.AddDays(1).Ticks, DateTimeKind.Utc);

            Workers = GetObservableCollectionWorkers();
        }

        private string guid = Guid.NewGuid().ToString(); // guid для индитификации подключения
        private Metadata requestHeaders = new Metadata(); // заголовки для передачи на сервер
        private DateTime deadline; // время жизни потока

        // Команды
        private RelayCommand? addCommand;
        private RelayCommand? editCommand;
        private RelayCommand? deleteCommand;
        public ObservableCollection<Worker> Workers { get; set; }

        // клиент gRPC
        private WorkerService.WorkerServiceClient WorkerServiceClient { get; set; }


        /// <summary>
        /// команда добавления
        /// </summary>
        public RelayCommand AddCommand
        {
            get
            {
                return addCommand ??
                  (addCommand = new RelayCommand(async (o) =>
                  {
                      WorkerWindow workerWindow = new WorkerWindow(new Worker());
                      if (workerWindow.ShowDialog() == true)
                      {
                          Worker worker = workerWindow.Worker;

                          // добавить нового работника через контракт
                          await AddWorkerAsync(worker);
                      }
                  }));
            }
        }
        /// <summary>
        /// команда редактирования
        /// </summary>
        public RelayCommand EditCommand
        {
            get
            {
                return editCommand ??
                  (editCommand = new RelayCommand(async (selectedItem) =>
                  {
                      // получаем выделенный объект
                      Worker? worker = selectedItem as Worker;
                      if (worker == null) return;

                      Worker vm = new Worker
                      {
                          Id = worker.Id,
                          LastName = worker.LastName,
                          FirstName = worker.FirstName,
                          MiddleName = worker.MiddleName,
                          Birthday = worker.Birthday,
                          Sex = worker.Sex,
                          HasChildren = worker.HasChildren,                          
                      };

                      WorkerWindow workerWindow = new WorkerWindow(vm);

                      if (workerWindow.ShowDialog() == true)
                      {
                          await EditWorkerAsync(worker, workerWindow);
                      }
                  }));
            }
        }
        /// <summary>
        /// команда удаления
        /// </summary>
        public RelayCommand DeleteCommand
        {
            get
            {
                return deleteCommand ??
                  (deleteCommand = new RelayCommand(async (selectedItem) =>
                  {
                      // получаем выделенный объект
                      Worker? worker = selectedItem as Worker;
                      if (worker == null) return;

                      // удаляем работника через контракт
                      await DeleteWorkerAsync(worker);
                  }));
            }
        }
        private async Task EditWorkerAsync(Worker worker, WorkerWindow workerWindow)
        {
            try
            {
                worker.Id = workerWindow.Worker.Id;
                worker.FirstName = workerWindow.Worker.FirstName;
                worker.MiddleName = workerWindow.Worker.MiddleName;
                worker.LastName = workerWindow.Worker.LastName;
                worker.Birthday = workerWindow.Worker.Birthday;
                worker.Sex = workerWindow.Worker.Sex;
                worker.HasChildren = workerWindow.Worker.HasChildren;
                WorkerEntiti updateWorkerReply = await WorkerServiceClient.UpdateWorkerAsync(new WorkerEntiti
                {
                    Id = worker.Id,
                    LastName = worker.LastName,
                    FirstName = worker.FirstName,
                    MiddleName = worker.MiddleName,
                    Birthday = worker.Birthday,
                    Sex = worker.Sex,
                    HaveChildren = worker.HasChildren,                    
                }, requestHeaders);
            }
            catch (RpcException ex)
            {
                MessageBox.Show(ex.Status.Detail);
            }

        }
        private async Task AddWorkerAsync(Worker worker)
        {
            try
            {
                // добавление работника в базу
                WorkerEntiti addWorkerReply = await WorkerServiceClient.CreateWorkerAsync(new CreateWorkerRequest
                {
                    LastName = worker.LastName,
                    FirstName = worker.FirstName,
                    MiddleName = worker.MiddleName,
                    Birthday = worker.Birthday,
                    Sex = worker.Sex,
                    HaveChildren= worker.HasChildren,
                }, requestHeaders);

                Workers.Add(new Worker
                {
                    Id = addWorkerReply.Id,
                    LastName= addWorkerReply.LastName,
                    FirstName= addWorkerReply.FirstName,
                    MiddleName= addWorkerReply.MiddleName,
                    Birthday = addWorkerReply.Birthday,
                    Sex = addWorkerReply.Sex,
                    HasChildren = addWorkerReply.HaveChildren,
                });
            }
            catch(RpcException ex)
            {
                MessageBox.Show(ex.Status.Detail);
            }
        }
        private async Task DeleteWorkerAsync(Worker worker)
        {
            try
            {
                // удаление объекта по id
                var delWorker = await WorkerServiceClient.DeleteWorkerAsync(new WorkerId { Id = worker.Id }, requestHeaders);
                Workers.Remove(worker);
            }
            catch (RpcException ex)
            {
                MessageBox.Show(ex.Status.Detail);
            }
        }
        private ObservableCollection<Worker> GetObservableCollectionWorkers()
        {
            // получение списка объектов
            ListReply workers = WorkerServiceClient.ListWorkers(new Google.Protobuf.WellKnownTypes.Empty(), requestHeaders);

            var result = new ObservableCollection<Worker>();
            foreach (var worker in workers.Workers)
            {
                result.Add(new Worker
                {
                    Id = worker.Id,
                    LastName = worker.LastName,
                    FirstName = worker.FirstName,
                    MiddleName = worker.MiddleName,
                    Birthday = worker.Birthday,
                    Sex = worker.Sex,
                    HasChildren = worker.HaveChildren,
                });
            }
            return result;
        }

        /// <summary>
        /// стрим от сервера
        /// </summary>
        /// <returns></returns>
        public async Task GetWorkerStream()
        {           

            // посылаем пустое сообщение и получаем набор сообщений
            var serverData = WorkerServiceClient.GetWorkerStream(new EmptyMessage(), requestHeaders, deadline);

            // получаем поток сервера
            var responseStream = serverData.ResponseStream;
            while (await responseStream.MoveNext(new CancellationToken()))
            {
                WorkerAction response = responseStream.Current;
                switch (response.ActionType)
                {
                    case Crud.Action.CreateAction:
                        Workers.Add(new Worker
                        {
                            Id = response.WorkerMessage.Id,
                            LastName = response.WorkerMessage.LastName,
                            FirstName = response.WorkerMessage.FirstName,
                            MiddleName = response.WorkerMessage.MiddleName,
                            Birthday= response.WorkerMessage.Birthday,
                            Sex= response.WorkerMessage.Sex,
                            HasChildren= response.WorkerMessage.HaveChildren,
                        });
                        break;
                    case Crud.Action.UpdateAction:
                        Worker workerUpd = Workers.Where(w => w.Id == response.WorkerMessage.Id).First();
                        workerUpd.LastName = response.WorkerMessage.LastName;
                        workerUpd.FirstName = response.WorkerMessage.FirstName;
                        workerUpd.MiddleName = response.WorkerMessage.MiddleName;
                        workerUpd.Birthday = response.WorkerMessage.Birthday;
                        workerUpd.Sex = response.WorkerMessage.Sex;
                        workerUpd.HasChildren = response.WorkerMessage.HaveChildren;
                        break;
                    case Crud.Action.DeleteAction:
                        Worker workerDel = Workers.Where(w => w.Id == response.WorkerMessage.Id).First();
                        Workers.Remove(workerDel);
                        break;
                    default:
                        MessageBox.Show("Что-то не то происходит сервер пишет");
                        break;
                }
            }
        }

    }
}
