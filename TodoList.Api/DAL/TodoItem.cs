using System;
using System.Text.Json.Serialization;

namespace TodoList.Api.DAL
{
    public interface IDataObject
    {
        public Guid Id { get; set; }
    }

    public class TodoItem : IDataObject
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("isCompleted")]
        public bool IsCompleted { get; set; }
    }
}
