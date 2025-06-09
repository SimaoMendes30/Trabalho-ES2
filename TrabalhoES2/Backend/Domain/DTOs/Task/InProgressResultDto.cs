namespace Backend.Domain.DTOs.Task;

public class InProgressResultDto
{
    public IEnumerable<TaskDetailsDto> Tasks { get; set; } = null!;
    public TimeSpan TotalTime { get; set; }
}