using Backend.Domain.DTOs.Common;
using Backend.Domain.DTOs.Member;

namespace Backend.Features.Member.Interfaces
{
    public interface IMemberService
    {
        Task<MemberDetailsDto> CreateAsync(MemberCreateDto dto);
        Task<IEnumerable<MemberDetailsDto>> FilteredListAsync(MemberFilterDto filter);
        Task<PagedResult<MemberDetailsDto>> FilteredPagedAsync(MemberFilterDto filter, int page, int pageSize);
        Task<MemberDetailsExtendedDto> GetByIdAsync(int id);
        System.Threading.Tasks.Task UpdateAsync(int id, MemberUpdateDto dto);
        System.Threading.Tasks.Task DeleteAsync(int id);

        // ➡️ Adicionar os métodos que estavam em falta
        Task<MemberDetailsDto> InviteToTaskAsync(int userId, int taskId, int projectId);

        Task<bool> RespondToTaskInvitationAsync(int memberId, bool accept);
        Task<bool> RespondToProjectInvitationAsync(int memberId, bool accept);
        System.Threading.Tasks.Task RemoveMemberFromProjectAsync(int currentUserId, int projectId, int userId);
        System.Threading.Tasks.Task RemoveMemberFromTaskAsync(int taskId, int userId);
        Task<MemberDetailsDto> InviteToProjectAsync(int currentUserId, int userId, int projectId);
        Task<IEnumerable<MemberDetailsDto>> GetPendingInvitationsAsync(int userId);
    }
}