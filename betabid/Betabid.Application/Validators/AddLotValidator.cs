using Betabid.Application.DTOs.LotsDTOs;
using FluentValidation;

namespace Betabid.Application.Validators;

public class AddLotValidator : AbstractValidator<AddLotDto>
{
    public AddLotValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.StartPrice).GreaterThan(0);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.DateStarted).NotEmpty();
        RuleFor(x => x.Deadline).GreaterThan(x => x.DateStarted).WithMessage("Deadline must be greater than DateStarted");
        RuleFor(x=> x.DateStarted).LessThan(x => x.Deadline).WithMessage("DateStarted must be less than Deadline");
        RuleFor(x=> x.DateStarted).GreaterThanOrEqualTo(x => DateTime.Now).WithMessage("DateStarted must be greater than or equal to current date");
        RuleFor(x => x.Deadline).GreaterThan(x => DateTime.Now).WithMessage("Deadline must be greater than current date");
        RuleFor(x => x.Deadline).NotEmpty();
        RuleFor(x => x.BetStep).GreaterThan(0);
        RuleFor(x => x.OwnerId).NotEmpty();
    }
    
}