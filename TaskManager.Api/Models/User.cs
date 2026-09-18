using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Api.Models;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    public List<TaskItem> Tasks { get; set; } = new();
}