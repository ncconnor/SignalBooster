using System.Text;
using Microsoft.Extensions.Logging;

namespace Synapse.SignalBoosterExample.OrderSender
{
    public class HttpOrderSender : IOrderSender
    {
        private readonly ILogger _logger;

        private const string SubmittingOrderMessage = "Submitting order to API";
        private const string SuccessMessage = "Successfully submitted order to API";
        private const string FailedMessage = "Failed to submit order to API";
        private const string MediaTypeJson = "application/json";

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderSender"/> class.
        /// </summary>
        /// <param name="logger">The logger instance for logging.</param>
        public HttpOrderSender(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Submits the order data to the API endpoint.
        /// </summary>
        /// <param name="orderJson">The JSON representation of the order data.</param>
        public void Submit(string orderJson, string apiUrl)
        {
            try
            {
                _logger.LogDebug(SubmittingOrderMessage);

                using var httpClient = new HttpClient();
                var content = new StringContent(orderJson, Encoding.UTF8, MediaTypeJson);

                var response = httpClient.PostAsync(apiUrl, content).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();

                _logger.LogDebug(SuccessMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, FailedMessage);
                throw;
            }
        }
    }
}