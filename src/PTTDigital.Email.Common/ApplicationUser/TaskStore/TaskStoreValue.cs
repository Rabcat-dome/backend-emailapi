using PTTDigital.Email.Common.ApplicationUser.User;

namespace PTTDigital.Email.Common.ApplicationUser.TaskStore;

public record TaskStoreValue
{
    public static TaskStoreValue NewTask(Guid taskId, IApplicationUser appUser)
    {
        return new TaskStoreValue
        {
            TaskId = taskId,
            AppUser = appUser,
            IsCompleted = false,
            Timestamp = DateTime.Now,
            Result = null,
        };
    }

    public static TaskStoreValue CompleteTask(Guid taskId, IApplicationUser appUser, object result, bool isCompleted = true)
    {
        return new TaskStoreValue
        {
            TaskId = taskId,
            AppUser = appUser,
            IsCompleted = isCompleted,
            Timestamp = DateTime.Now,
            Result = result,
        };
    }

    public Guid TaskId { get; init; }
    public bool IsCompleted { get; init; }
    public DateTime Timestamp { get; init; }
    public object? Result { get; init; }
    public IApplicationUser? AppUser { get; init; }
}
