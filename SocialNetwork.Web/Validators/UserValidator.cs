﻿using FluentValidation;
using SocialNetwork.BL.Models;
using SocialNetwork.Web.Models;

namespace SocialNetwork.Web.Validators;

public class UserValidator : AbstractValidator<UserCreateViewModel>
{
    public UserValidator()
    {
        
        RuleFor(x => x.Login).NotEmpty().NotNull()
            .Length(6,30)
            .Matches("^[a-zA-Z0-9_-]*$")
            .WithMessage("Not a valid login format. Only Latin letters, digits, underscore and hyphen are allowed.");
        
        RuleFor(x => x.Password).NotEmpty().NotNull()
            .Length(8, 50)
            .Matches("^[A-Za-z\\d!@#$%^&*()\\-_=+{};:,<.>]+$")
            .WithMessage("Not a valid password format. Only Latin letters, digits and this symbols ! @ # $ % ^ & * ( ) - _ = + { } ; : , < . > are allowed.");
        
        RuleFor(x => x.Profile)
            .NotNull()
            .SetValidator(new ProfileValidator());
    }
}