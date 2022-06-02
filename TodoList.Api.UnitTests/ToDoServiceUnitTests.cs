using Moq;
using System;
using System.Threading.Tasks;
using TodoList.Api.BL;
using TodoList.Api.DAL;
using Xunit;

namespace TodoList.Api.UnitTests
{
    public class ToDoServiceUnitTests
    {
        [Fact]
        public async void GetItemAsync_Found_record_in_DB_OK_result_Test()
        {
            var rep = new Mock<IDataRepository<TodoItem>>();

            rep.Setup(t => t.GetTask(It.IsAny<Guid>())).Returns(Task.Run(() => new TodoItem { }));

            IToDoService service = new ToDoService(rep.Object);

            var res = await service.GetItemAsync(It.IsAny<Guid>());

            Assert.True(res != null);
            Assert.True(res.Result != null);
            Assert.True(res.ComplexResult.Message == null);
            Assert.True(res.ComplexResult.ResultType == ResultType.OK);


        }

        [Fact]
        public async void GetItemAsync_not_Found_NotFound_result_Test()
        {
            var rep = new Mock<IDataRepository<TodoItem>>();

            rep.Setup(t => t.GetTask(It.IsAny<Guid>())).Returns(Task.FromResult<TodoItem>(null));

            IToDoService service = new ToDoService(rep.Object);

            var res = await service.GetItemAsync(It.IsAny<Guid>());

            Assert.True(res != null);
            Assert.True(res.Result == null);
            Assert.Contains("not found", res.ComplexResult.Message);
            Assert.True(res.ComplexResult.ResultType != ResultType.OK);
            Assert.True(res.ComplexResult.ResultType == ResultType.NotFound);


        }


        [Fact]
        public async void SaveTodoItem_invalid_input_Test()
        {
            var rep = new Mock<IDataRepository<TodoItem>>();

            IToDoService service = new ToDoService(rep.Object);

            var res = await service.SaveTodoItem(Guid.NewGuid(), new TodoItem { Id = Guid.NewGuid() });

            Assert.True(res != null);
            Assert.True(res.Result == null);
            Assert.Contains("provided is not the id in ToDo item provided ", res.ComplexResult.Message);
            Assert.True(res.ComplexResult.ResultType != ResultType.OK);
            Assert.True(res.ComplexResult.ResultType == ResultType.BadRequest);
        }

        [Fact]
        public async void SaveTodoItem_Not_found_Test()
        {
            var rep = new Mock<IDataRepository<TodoItem>>();
            rep.Setup(t => t.GetTask(It.IsAny<Guid>())).Returns(Task.FromResult<TodoItem>(null));

            IToDoService service = new ToDoService(rep.Object);
            var id = Guid.NewGuid();
            var res = await service.SaveTodoItem(id, new TodoItem { Id = id });

            Assert.True(res != null);
            Assert.True(res.Result == null);
            Assert.Contains("was not found in the system", res.ComplexResult.Message);
            Assert.True(res.ComplexResult.ResultType != ResultType.OK);
            Assert.True(res.ComplexResult.ResultType == ResultType.NotFound);
        }

        [Fact]
        public async void SaveTodoItem_DataRepository_Expceted_Exception_Test()
        {
            var rep = new Mock<IDataRepository<TodoItem>>();

            rep.Setup(t => t.GetTask(It.IsAny<Guid>())).Returns(Task.FromResult<TodoItem>(new TodoItem { }));
            rep.Setup(t => t.UpdateTask(It.IsAny<TodoItem>())).Throws(new Exception("My unexpected exception"));

            IToDoService service = new ToDoService(rep.Object);
            var id = Guid.NewGuid();
            var res = await service.SaveTodoItem(id, new TodoItem { Id = id });


            Assert.True(res != null);
            Assert.True(res.Result == null);
            Assert.Contains("My unexpected exception", res.ComplexResult.Message);
            Assert.True(res.ComplexResult.ResultType != ResultType.OK);
            Assert.True(res.ComplexResult.ResultType == ResultType.UnknownError);

        }

        [Fact]
        public async void SaveTodoItem_DataRepository_OK_Test()
        {
            var rep = new Mock<IDataRepository<TodoItem>>();

            rep.Setup(t => t.GetTask(It.IsAny<Guid>())).Returns(Task.Run(() => new TodoItem { }));
            rep.Setup(t => t.UpdateTask(It.IsAny<TodoItem>())).Returns(Task.Run(() => new TodoItem { }));

            IToDoService service = new ToDoService(rep.Object);
            var id = Guid.NewGuid();
            var res = await service.SaveTodoItem(id, new TodoItem { Id = id });


            Assert.True(res != null);
            Assert.True(res.Result == null);
            Assert.Null(res.ComplexResult.Message);
            Assert.True(res.ComplexResult.ResultType == ResultType.NoContent);

        }


        [Fact]
        public async void AddTodoItem_Description_Empty_test()
        {
            var rep = new Mock<IDataRepository<TodoItem>>();

            IToDoService service = new ToDoService(rep.Object);
            var res = await service.AddTodoItem(new TodoItem { });


            Assert.True(res != null);
            Assert.True(res.Result == null);
            Assert.Equal("Description is required", res.ComplexResult.Message);
            Assert.True(res.ComplexResult.ResultType == ResultType.BadRequest);

        }


      

        [Fact]
        public async void AddTodoItem_OK_Test()
        {
            var rep = new Mock<IDataRepository<TodoItem>>();

            rep.Setup(t => t.GetTask(It.IsAny<Guid>())).Returns(Task.Run(() => new TodoItem { }));

            rep.Setup(t => t.CreateTask(It.IsAny<TodoItem>())).Returns(Task.FromResult<TodoItem>(null));

            IToDoService service = new ToDoService(rep.Object);
            var id = Guid.NewGuid();
            var res = await service.AddTodoItem(new TodoItem { Id = id, Description = "some description" });


            Assert.True(res != null);
            Assert.True(res.Result != null);
            Assert.Null(res.ComplexResult.Message);
            Assert.True(res.ComplexResult.ResultType == ResultType.OK);


        }
    }
}
