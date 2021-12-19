using AutoMapper;
using Basket.API.Entities;
using Basket.API.GrpsServices;
using Basket.API.Repositories;
using EventBus.Messages.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Basket.API.Controllers
{
	[ApiController]
	[Route("api/v1/[controller]")]
	public class BasketController : ControllerBase
	{
		private readonly IBasketRepository _repository;
        private readonly DiscountGrpcService _discountGrpcService;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<BasketController> _logger;

		public BasketController(IBasketRepository repository, DiscountGrpcService discountGrpcService, IMapper mapper, IPublishEndpoint publishEndpoint, ILogger<BasketController> logger)
		{
			_repository = repository ?? throw new ArgumentNullException(nameof(repository));
			_discountGrpcService = discountGrpcService ?? throw new ArgumentNullException(nameof(discountGrpcService));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
			_publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

		[HttpGet("{userName}", Name = "GetBasket")]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
        {
            var basket = await _repository.GetBasket(userName);
            return Ok(basket ?? new ShoppingCart(userName));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
        {
			// TODO : Communicate with Discount.Grpc
			// and Calculate latest prices of product into shopping cart
			// consume Discount Grpc
			foreach (var item in basket.Items)
			{
				var coupon = await _discountGrpcService.GetDiscount(item.ProductName);
				item.Price -= coupon.Amount;
			}

			return Ok(await _repository.UpdateBasket(basket));
        }

        [HttpDelete("{userName}", Name = "DeleteBasket")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteBasket(string userName)
        {
            await _repository.DeleteBasket(userName);
            return Ok();
        }

        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
        {
            // get existing basket with total price 
            // Create basketCheckoutEvent -- Set TotalPrice on basketCheckout eventMessage
            // send checkout event to rabbitmq
            // remove the basket

            _logger.LogInformation("Checkout for basketckeckout userName {userName}", basketCheckout.UserName);

            // get existing basket with total price
            var basket = await _repository.GetBasket(basketCheckout.UserName);
            if (basket == null)
            {
                _logger.LogError("Basket not found for userName {userName}", basketCheckout.UserName);
                return BadRequest();
            }

            // send checkout event to rabbitmq
            var eventMessage = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
            eventMessage.TotalPrice = basket.TotalPrice;

            _logger.LogInformation("Calculating TotalPrice: {totalPrice}", basket.TotalPrice);

            await _publishEndpoint.Publish(eventMessage);

            _logger.LogInformation("BasketCheckoutEvent published", basket.TotalPrice);

            // remove the basket
            await _repository.DeleteBasket(basket.UserName);

            _logger.LogInformation("Basket deleted for userName {userName}", basket.UserName);

            return Accepted();
        }
    }
}
