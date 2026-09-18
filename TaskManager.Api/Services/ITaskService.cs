using TaskManager.Api.DTOs;
using TaskManager.Api.Models;

namespace TaskManager.Api.Services;

public interface ITaskService
{
    Task<List<TaskResponse>> GetAllAsync(Guid userId, TaskItemStatus? status = null);
    Task<TaskCountsResponse> GetCountsAsync(Guid userId);
    Task<TaskResponse?> GetByIdAsync(Guid id, Guid userId);
    Task<TaskResponse> CreateAsync(CreateTaskRequest request, Guid userId);
    Task<bool> UpdateAsync(Guid id, UpdateTaskRequest request, Guid userId);
    Task<bool> DeleteAsync(Guid id, Guid userId);
}