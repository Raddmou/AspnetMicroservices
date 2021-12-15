﻿using FluentValidation;

namespace Ordering.Application.Features.Orders.Commands.CheckoutOrder
{
	public class CheckoutOrderCommandValidator : AbstractValidator<CheckoutOrderCommand>
	{
		public CheckoutOrderCommandValidator()
		{
			RuleFor(a => a.UserName)
				.NotEmpty()
				.WithMessage("{UserName} is required.")
				.MaximumLength(50)
				.WithMessage("{UserName} must not exceed 50 characters.");

			RuleFor(a => a.EmailAddress)
				.NotEmpty()
				.WithMessage("{EmailAddress} is required.");

			RuleFor(a => a.TotalPrice)
				.NotEmpty()
				.WithMessage("{TotalPrice} is required.")
				.GreaterThan(0)
				.WithMessage("{TotalPrice} should be greater than 0.");			;
		}

		
	}
}
