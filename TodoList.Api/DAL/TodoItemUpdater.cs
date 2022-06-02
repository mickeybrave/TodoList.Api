
namespace TodoList.Api.DAL
{
    public class TodoItemUpdater<T> : IDataUpdater<T> where T : TodoItem
    {
        public void UpdateDataObject(T objectToUpdate, T newObject)
        {
            objectToUpdate.Description = newObject.Description; 
            objectToUpdate.Id = newObject.Id; 
            objectToUpdate.IsCompleted = newObject.IsCompleted; 

        }
    }
}
