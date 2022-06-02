
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TodoList.Api;

namespace TodoList.Api.DAL
{
    public class DataRepository<T> : IDataRepository<T> where T : IDataObject
    {
        public const string ConstDefaultDataPath = "DAL/ToDo.json";
        private readonly string _filePath;
        private readonly IDataUpdater<T> _dataUpdater;
        private IConfiguration _config;
        private const string ConstDataPathConfig = "DataPath";
        public DataRepository(IConfiguration config, IDataUpdater<T> dataUpdater)
        {
            _config = config;
            var dataSourceJsonPath = _config[ConstDataPathConfig];
            if (string.IsNullOrEmpty(dataSourceJsonPath))
            {
                throw new ArgumentNullException(nameof(dataSourceJsonPath));
            }
            this._filePath = dataSourceJsonPath;
            this._dataUpdater = dataUpdater;
        }
        public DataRepository(IDataUpdater<T> dataUpdater)
        {
            this._filePath = ConstDefaultDataPath;
            this._dataUpdater = dataUpdater;
        }

        public Task<IEnumerable<T>> GetAllTask()
        {
            return Task.Run(() => GetAll());
        }

        public IEnumerable<T> GetAll()
        {
            using (StreamReader r = new StreamReader(_filePath))
            {
                string json = r.ReadToEnd();
                return  JsonConvert.DeserializeObject<IEnumerable<T>>(json);
            }
        }

        public T Get(Guid id)
        {
            using (StreamReader r = new StreamReader(_filePath))
            {
                string json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<List<T>>(json)
                    .FirstOrDefault(f => f.Id== id);
            }
        }

        public Task<T> GetTask(Guid id)
        {
            return Task.Run(() => this.Get(id));
        }


        public void Create(T newObject)
        {
            var json = File.ReadAllText(_filePath);
            var allData = JsonConvert.DeserializeObject<List<T>>(json);
            allData.Add(newObject);
            File.WriteAllText(_filePath, JsonConvert.SerializeObject(allData));
        }
        public Task CreateTask(T existingObjec)
        {
            return Task.Run(() => Create(existingObjec));
        }

        public void Delete(Guid id)
        {
            var json = File.ReadAllText(_filePath);
            var allData = JsonConvert.DeserializeObject<List<T>>(json);
            var indexToRemove = allData.FindIndex(i => i.Id == id);
            allData.RemoveAt(indexToRemove);
            File.WriteAllText(_filePath, JsonConvert.SerializeObject(allData));
        }

        public Task DeleteTask(Guid id)
        {
            return Task.Run(() => Delete(id));
        }

        public void Update(T updatedData)
        {
            var json = File.ReadAllText(_filePath);
            var dataObjects = JsonConvert.DeserializeObject<List<T>>(json);

            foreach (var obj in dataObjects)
            {
                if (updatedData.Id == obj.Id)
                {
                    _dataUpdater.UpdateDataObject(obj, updatedData);
                }
            }

            File.WriteAllText(_filePath, JsonConvert.SerializeObject(dataObjects));
        }


        public Task UpdateTask(T updatedData)
        {
            return Task.Run(() => Update(updatedData));
        }
    }
}
