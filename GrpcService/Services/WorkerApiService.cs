using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcService.Models;
using Crud;  // пространство имен сервиса WorkerService.WorkerServiceBase
using Google.Protobuf;
using System.Collections.Generic;

namespace CrudGrpcApp.Services;
public class WorkerApiService : WorkerService.WorkerServiceBase
{
    private ApplicationContext db;
    private static Dictionary<string, IServerStreamWriter<WorkerAction>> pooWorkerActions = new();
    private DateTime deadline;
    private bool Valide { get; set; } = true;
    public WorkerApiService(ApplicationContext db)    
    {
        this.db = db; 
    }

    /// <summary>
    /// заполнение очередей на отправку
    /// </summary>
    /// <param name="workerAction"></param>
    private async Task UpdatePoll(WorkerAction workerAction, ServerCallContext context)
    {
        foreach (var responseStream in pooWorkerActions)
        {
            var userGuid = context.RequestHeaders.GetValue("guid");
            if (responseStream.Key != userGuid) await responseStream.Value.WriteAsync(workerAction);
        }
    }

    /// <summary>
    /// отправляем список пользователей
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<ListReply> ListWorkers(Empty request, ServerCallContext context)
    {
        var listReply = new ListReply();    // определяем список

        // преобразуем каждый объект Worker в объект WorkerReply
        var workerList = db.Workers.Select(item => new WorkerEntiti 
        { 
            Id = item.Id,
            LastName = item.LastName,
            FirstName = item.FirstName,
            MiddleName = item.MiddleName,
            Birthday = item.Birthday,
            Sex = item.Sex,
            HaveChildren = item.HasChildren,
        }).ToList();
        listReply.Workers.AddRange(workerList);
        return await Task.FromResult(listReply);
    }

    /// <summary>
    /// отправляем одного пользователя по id
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="RpcException"></exception>    
    public override async Task<WorkerEntiti> GetWorker(WorkerId request, ServerCallContext context)
    {
        var worker = await db.Workers.FindAsync(request.Id);

        // если пользователь не найден, генерируем исключение
        if (worker is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Worker not found"));
        }
        WorkerEntiti workerReply = new WorkerEntiti
        { 
            Id = worker.Id,
            FirstName= worker.FirstName,
            MiddleName= worker.MiddleName,
            LastName = worker.LastName,
            Birthday = worker.Birthday,
            Sex = worker.Sex, 
            HaveChildren = worker.HasChildren,
        };
        return await Task.FromResult(workerReply);
    }

    /// <summary>
    ///  добавление пользователя
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<WorkerEntiti> CreateWorker(CreateWorkerRequest request, ServerCallContext context)
    {
        // формируем из данных объект Worker и добавляем его в список workers
        var worker = new Worker 
        { 
            LastName = request.LastName,
            FirstName = request.FirstName,
            MiddleName = request.MiddleName,
            Birthday = request.Birthday,
            Sex = request.Sex,
            HasChildren = request.HaveChildren,
        };
        await db.Workers.AddAsync(worker);
        await db.SaveChangesAsync();
        var reply = new WorkerEntiti() 
        { 
            Id = worker.Id,
            FirstName = worker.FirstName,
            MiddleName = worker.MiddleName,
            LastName = worker.LastName,
            Birthday = worker.Birthday,
            Sex = worker.Sex,
            HaveChildren = worker.HasChildren,
        };
        await UpdatePoll(new WorkerAction
        {
            ActionType = Crud.Action.CreateAction,
            WorkerMessage = reply
        }, context);
        return await Task.FromResult(reply);
    }

    /// <summary>
    /// обновление пользователя
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="RpcException"></exception>
    public override async Task<WorkerEntiti> UpdateWorker(WorkerEntiti request, ServerCallContext context)
    {
        var userGuid = context.RequestHeaders.GetValue("guid");
        var worker = await db.Workers.FindAsync(request.Id);
        if (worker == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Worker not found"));
        }
        // обновляем даннные
        worker.LastName = request.LastName;
        worker.FirstName = request.FirstName;
        worker.MiddleName = request.MiddleName;
        worker.Birthday = request.Birthday;
        worker.Sex = request.Sex;
        worker.HasChildren = request.HaveChildren;
        await db.SaveChangesAsync();
        var reply = new WorkerEntiti() 
        { 
            Id = worker.Id,
            LastName = worker.LastName,
            FirstName = worker.FirstName,
            MiddleName = worker.MiddleName,
            Birthday = worker.Birthday,
            Sex = worker.Sex,
            HaveChildren = worker.HasChildren,
        };
        await UpdatePoll(new WorkerAction
        {
            ActionType = Crud.Action.UpdateAction,
            WorkerMessage = reply
        }, context);
        return await Task.FromResult(reply);
    }

    /// <summary>
    /// удаление пользователя
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="RpcException"></exception>
    public override async Task<WorkerEntiti> DeleteWorker(WorkerId request, ServerCallContext context)
    {
        var userGuid = context.RequestHeaders.GetValue("guid");
        var worker = await db.Workers.FindAsync(request.Id);
        if (worker == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Worker not found"));
        }
        // удаляем пользователя из бд
        db.Workers.Remove(worker);
        await db.SaveChangesAsync();
        var reply = new WorkerEntiti()
        {
            Id = worker.Id,
            LastName = worker.LastName,
            Birthday = worker.Birthday,
            Sex = worker.Sex,
        };
        await UpdatePoll(new WorkerAction
        {
            ActionType = Crud.Action.DeleteAction,
            WorkerMessage = reply
        }, context);
        return await Task.FromResult(reply);
    }

    /// <summary>
    /// отправляем поток изменений в данных
    /// </summary>
    /// <param name="request"></param>
    /// <param name="responseStream"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task GetWorkerStream(EmptyMessage request, IServerStreamWriter<WorkerAction> responseStream, ServerCallContext context)
    {
        // добовляем конкретный стрим в список стримов для брудкаста
        var userGuid = context.RequestHeaders.GetValue("guid");
        pooWorkerActions.Add(userGuid, responseStream);

        deadline = context.Deadline;
        // ожидание завершения стрима от клиента или истечение времени токена
        while (!context.CancellationToken.IsCancellationRequested)
        {
            await Task.Delay(deadline - DateTime.Now, context.CancellationToken);
        }

        // удаляем стрим из коллекции
        pooWorkerActions.Remove(userGuid);       
    }

}