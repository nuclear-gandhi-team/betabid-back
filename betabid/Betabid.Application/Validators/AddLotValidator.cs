using Betabid.Application.DTOs.LotsDTOs;
using FluentValidation;

namespace Betabid.Application.Validators;

public class AddLotValidator : AbstractValidator<AddLotDto>
{
    public AddLotValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100).WithMessage("Name must be provided and less than 100 characters");
        RuleFor(x => x.StartPrice).NotEmpty().GreaterThan(0).WithMessage("StartPrice must be greater than 0");
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000).WithMessage("Description must be provided and less than 1000 characters");
        RuleFor(x => x.DateStarted).NotEmpty().WithMessage("DateStarted must be provided");
        RuleFor(x => x.Deadline).GreaterThan(x => x.DateStarted).WithMessage("Deadline must be greater than DateStarted");
        RuleFor(x=> x.DateStarted).LessThan(x => x.Deadline).WithMessage("DateStarted must be less than Deadline");
        RuleFor(x=> x.DateStarted).GreaterThanOrEqualTo(x => DateTime.Now).WithMessage("DateStarted must be greater than or equal to current date");
        RuleFor(x => x.Deadline).GreaterThan(x => DateTime.Now).WithMessage("Deadline must be greater than current date");
        RuleFor(x => x.Deadline).NotEmpty().WithMessage("Deadline must be provided");
        RuleFor(x => x.BetStep).NotEmpty().GreaterThan(0).WithMessage("BetStep must be greater than 0");
        RuleFor(x=>x.TagIds).Must(x => x.Count <= 2).WithMessage("You can add up to 2 tags to a lot");
    }
}