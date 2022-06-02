


namespace TodoList.Api.DAL
{
    public interface IDataUpdater<T> where T : IDataObject
    {
        void UpdateDataObject(T objectToUpdate, T newObject);
    }
}