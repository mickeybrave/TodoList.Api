
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoList.Api;

namespace TodoList.Api.DAL
{
    public interface IDataRepository<T> where T : IDataObject
    {
        void Update(T updatedData);
        Task UpdateTask(T updatedData);


        Task<IEnumerable<T>> GetAllTask();
        IEnumerable<T> GetAll();
        T Get(Guid id);
        Task<T> GetTask(Guid id);


        void Create(T newObject);

        Task CreateTask(T existingObjec);

        void Delete(Guid id);

        Task DeleteTask(Guid id);

    }
}
