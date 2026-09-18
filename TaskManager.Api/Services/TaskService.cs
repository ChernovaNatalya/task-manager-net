using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Data;
using TaskManager.Api.DTOs;
using TaskManager.Api.Models;

namespace TaskManager.Api.Services;

public class TaskService : ITaskService
{
    private readonly AppDbContext _db;

    public TaskService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<TaskResponse>> GetAllAsync(Guid userId, TaskItemStatus? status = null)
    {
        var query = _db.Tasks
            .AsNoTracking()
            .Where(t => t.UserId == userId);  // ← фильтр по пользователю

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);

        return await query
            .Select(t => t.ToResponse())
            .ToListAsync();
    }

    public async Task<TaskCountsResponse> GetCountsAsync(Guid userId)
    {
        var counts = await _db.Tasks
            .AsNoTracking()
            .Where(t => t.UserId == userId)
            .GroupBy(t => t.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.Status, g => g.Count);

        return new TaskCountsResponse
        {
            All = counts.Values.Sum(),
            Todo = counts.GetValueOrDefault(TaskItemStatus.Todo),
            InProgress = counts.GetValueOrDefault(TaskItemStatus.InProgress),
            Done = counts.GetValueOrDefault(TaskItemStatus.Done)
        };
    }

    public async Task<TaskResponse?> GetByIdAsync(Guid id, Guid userId)
    {
        var task = await _db.Tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);  // ← защита от чтения чужих задач

        return task?.ToResponse();
    }

    public async Task<TaskResponse> CreateAsync(CreateTaskRequest request, Guid userId)
    {
        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Status = request.Status,
            Priority = request.Priority,
            DueDate = request.DueDate,
            CreatedAt = DateTime.UtcNow,
            UserId = userId
        };

        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();

        return task.ToResponse();
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateTaskRequest request, Guid userId)
    {
        var task = await _db.Tasks
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        if (task is null)
            return false;

        task.Title = request.Title;
        task.Description = request.Description;
        task.Status = request.Status;
        task.Priority = request.Priority;
        task.DueDate = request.DueDate;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId)
    {
        var task = await _db.Tasks
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        if (task is null)
            return false;

        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync();
        return true;
    }
}