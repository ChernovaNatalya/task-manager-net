namespace TaskManager.Api.DTOs;

public class TaskCountsResponse
{
    public int All { get; set; }
    public int Todo { get; set; }
    public int InProgress { get; set; }
    public int Done { get; set; }
}