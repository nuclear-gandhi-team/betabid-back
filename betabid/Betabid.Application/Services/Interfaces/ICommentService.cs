using Betabid.Application.DTOs.CommentDtos;

namespace Betabid.Application.Services.Interfaces;

public interface ICommentService
{
    Task AddCommentAsync(CreateCommentDto createCommentDto, string userId);

    Task<IList<GetCommentDto>> GetAllLotCommentsAsync(int lotId);
}