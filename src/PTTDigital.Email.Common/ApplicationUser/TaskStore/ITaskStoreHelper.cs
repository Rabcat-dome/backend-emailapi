namespace PTTDigital.Email.Common.ApplicationUser.TaskStore;

public interface ITaskStoreHelper
{
    void SetTaskValue(TaskStoreValue obj);
    TaskStoreValue GetTaskValue(Guid key);

    object GetResult(Guid id);
    TResult GetResult<TResult>(Guid id);

    void RemoveTasks(int hour);
    bool IsCompleted(Guid id);
    bool IsRunning(Guid id);
}