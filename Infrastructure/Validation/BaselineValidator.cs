using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;

namespace Validation
{
    public abstract class BaselineValidator<T> : AbstractValidator<T>
    {
        protected BaselineValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
        }

        public override ValidationResult Validate(ValidationContext<T> ctx)
        {
            return this.StoreResult(ctx.InstanceToValidate, base.Validate(ctx));
        }

        public override async Task<ValidationResult> ValidateAsync(ValidationContext<T> ctx,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await base.ValidateAsync(ctx, cancellationToken);
            return this.StoreResult(ctx.InstanceToValidate, result);
        }
    }
}
