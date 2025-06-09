using Frontend.DTOs.Project;
using Frontend.DTOs.Task;
using Frontend.DTOs.User;

namespace Frontend.DTOs.Member
{
    public class MemberDetailsExtendedDto : MemberDetailsDto
    {
        public UserDetailsDto IdUserNavigation { get; set; } = null!;
        public ProjectDetailsDto IdProjectNavigation { get; set; } = null!;
        public TaskDetailsDto? IdTaskNavigation { get; set; }
    }
}