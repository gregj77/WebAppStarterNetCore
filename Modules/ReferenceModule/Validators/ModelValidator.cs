using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using ReferenceModule.Models;
using Validation;

namespace ReferenceModule
{
    class ModelValidator : BaselineValidator<Model>
    {
        public ModelValidator()
        {
            RuleFor(p => p.Id)
                .GreaterThan(0)
                .LessThan(1000);

            RuleFor(p => p.Name)
                .NotEmpty()
                .MaximumLength(255);
        }
    }
}
