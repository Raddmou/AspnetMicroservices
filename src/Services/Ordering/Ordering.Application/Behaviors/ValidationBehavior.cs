using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.Application.Behaviors
{
	public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	{
		private readonly IEnumerable<IValidator<TRequest>> _validators;

		public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
		{
			_validators = validators ?? throw new ArgumentNullException(nameof(validators));
		}

		public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
		{
			if(_validators.Any())
			{
				var context = new ValidationContext<TRequest>(request);

				var validationResults = await Task.WhenAll(_validators.Select(a => a.ValidateAsync(context, cancellationToken)));

				var failures = validationResults.SelectMany(a => a.Errors)
												.Where(a => a != null)
												.ToList();

				if (failures.Count != 0)
					throw new Ordering.Application.Exceptions.ValidationException(failures);
			}
			return await next();
		}
	}
}
