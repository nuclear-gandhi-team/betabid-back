using AutoMapper;
using Betabid.Application.DTOs.CommentDtos;
using Betabid.Application.Exceptions;
using Betabid.Application.Interfaces.Repositories;
using Betabid.Application.Services.Interfaces;
using Betabid.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Betabid.Application.Services.Implementations;

public class CommentService : ICommentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public CommentService(IUnitOfWork unitOfWork, UserManager<User> userManager, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task AddCommentAsync(CreateCommentDto createCommentDto, string userId)
    {
        if (await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId) is null)
        {
            throw new EntityNotFoundException($"No user with Id '{userId}'");
        }

        if (await _unitOfWork.Lots.GetByIdAsync(createCommentDto.LotId) is null)
        {
            throw new EntityNotFoundException($"No lot with Id '{createCommentDto.LotId}'");
        }

        if (createCommentDto.Body.IsNullOrEmpty())
        {
            throw new EmptyCommentException();
        }

        var comment = new Comment
        {
            Body = createCommentDto.Body,
            LotId = createCommentDto.LotId,
            UserId = userId,
        };
        
        if (createCommentDto.ParentCommentId is not null)
        {
            var parentComment = await _unitOfWork.Comments.GetByIdAsync((int)createCommentDto.ParentCommentId) 
                                ?? throw new EntityNotFoundException($"No parent comment with Id '{createCommentDto.LotId}'");

            comment.ParentCommentId = parentComment.Id;
        }

        await _unitOfWork.Comments.AddAsync(comment);
        await _unitOfWork.CommitAsync();
    }

    public async Task<IList<GetCommentDto>> GetAllLotCommentsAsync(int lotId)
    {
        var lot = await _unitOfWork.Lots.GetByIdWithCommentsAsync(lotId)
                   ?? throw new EntityNotFoundException($"No lot with id '{lotId}'");

        var comments = (await _unitOfWork.Comments.GetAllAsync())
            .Where(c => c.LotId == lot.Id && c.ParentCommentId is null)
            .ToList();

        var commentsDtos = _mapper.Map<List<GetCommentDto>>(comments);

        return commentsDtos!;
    }
}