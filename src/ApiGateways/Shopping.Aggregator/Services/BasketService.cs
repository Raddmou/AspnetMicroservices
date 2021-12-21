using Microsoft.Extensions.Logging;
using Shopping.Aggregator.Extensions;
using Shopping.Aggregator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Shopping.Aggregator.Services
{
	public class BasketService : IBasketService
	{
        private readonly HttpClient _client;
        private readonly ILogger<BasketService> _logger;

		public BasketService(HttpClient client, ILogger<BasketService> logger)
		{
			_client = client ?? throw new ArgumentNullException(nameof(client));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<BasketModel> GetBasket(string userName)
        {
			_logger.LogInformation(string.Format("{0}{1}", _client.BaseAddress, $"/api/v1/Basket/{userName}"));

            var response = await _client.GetAsync($"/api/v1/Basket/{userName}");
            return await response.ReadContentAs<BasketModel>();
        }
    }
}
