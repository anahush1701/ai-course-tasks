//  ISSUE: The PaymentProcessingService intermittently throws a NullReferenceException when deserializing the 
//      gateway’s JSON response (which can be empty or malformed), and lacks any resilience logic (e.g. retries or circuit breaker), 
//      leading to unhandled errors at runtime.

//  TASK: Write a prompt for an LLM to fix an error.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Task3.Models;

namespace Task3.Services
{
    public class GatewayResponse
    {
        public bool Success  { get; set; }
        public string Message { get; set; }
    }

    public interface IPaymentProcessingService
    {
        Task<PaymentResult> ProcessPaymentAsync(int accountId, decimal amount);
    }

    public class PaymentProcessingService : IPaymentProcessingService
    {
        private readonly ILogger<PaymentProcessingService> _logger;
        private readonly HttpClient _httpClient;
        private readonly Dictionary<int, Account> _accounts = new Dictionary<int, Account>();

        public PaymentProcessingService(ILogger<PaymentProcessingService> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;

            _accounts[1] = new Account { Id = 1, Balance = 5000m };
            _accounts[2] = new Account { Id = 2, Balance = 0m };
            // ...
        }

        public async Task<PaymentResult> ProcessPaymentAsync(int accountId, decimal amount)
        {
            _logger.LogInformation("Start processing payment for account {AccountId}, amount {Amount}", accountId, amount);

            // 1) Check cache
            if (!_accounts.TryGetValue(accountId, out var account))
            {
                _logger.LogError("Account {AccountId} not found in cache", accountId);
                return PaymentResult.Failed("Account not found");
            }

            // 2) Send request to payment service with resilience (retry logic)
            var request = new
            {
                AccountId = account.Id,
                Amount = amount
            };

            const int maxRetries = 3;
            int attempt = 0;
            GatewayResponse gatewayResult = null;
            HttpResponseMessage response = null;
            string json = null;

            while (attempt < maxRetries)
            {
                try
                {
                    response = await _httpClient.PostAsync(
                        "https://api.payment-gateway.com/v1/payments",
                        new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json")
                    );

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogWarning("Payment gateway returned status {StatusCode} (attempt {Attempt})", response.StatusCode, attempt + 1);
                        // Optionally, break on certain status codes (e.g., 4xx)
                    }
                    else
                    {
                        json = await response.Content.ReadAsStringAsync();

                        // Defensive deserialization
                        if (string.IsNullOrWhiteSpace(json))
                        {
                            _logger.LogWarning("Payment gateway returned empty response (attempt {Attempt})", attempt + 1);
                        }
                        else
                        {
                            try
                            {
                                gatewayResult = JsonSerializer.Deserialize<GatewayResponse>(json);
                            }
                            catch (JsonException ex)
                            {
                                _logger.LogError(ex, "Failed to deserialize gateway response: {Json}", json);
                            }
                        }

                        if (gatewayResult != null)
                            break; // Success
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception during payment gateway call (attempt {Attempt})", attempt + 1);
                }

                attempt++;
                if (attempt < maxRetries)
                {
                    // Exponential backoff
                    await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt)));
                }
            }

            if (gatewayResult == null)
            {
                return PaymentResult.Failed("Gateway error or invalid response");
            }

            if (!gatewayResult.Success)
            {
                _logger.LogWarning("Payment gateway failed: {Message}", gatewayResult.Message);
                return PaymentResult.Failed("Gateway failure: " + gatewayResult.Message);
            }

            // 3) Correct balance locally
            if (amount < 0)
            {
                _logger.LogWarning("Negative payment amount: {Amount}", amount);
                return PaymentResult.Failed("Amount must be non-negative");
            }

            if (account.Balance < amount)
            {
                _logger.LogWarning("Insufficient funds for account {AccountId}", accountId);
                return PaymentResult.Failed("Insufficient funds");
            }

            account.Balance -= amount;
            _logger.LogInformation("Payment successful. New balance: {Balance}", account.Balance);

            return PaymentResult.Success(account.Balance);
        }
    }
}

//LOGS
// 2025-06-30 11:15:42.001 +02:00 [INF] MyApp.Services.PaymentProcessingService 
//     Start processing payment for account 5, amount 120.00
// 2025-06-30 11:15:42.105 +02:00 [ERR] MyApp.Services.PaymentProcessingService 
//     Object reference not set to an instance of an object.
//    at MyApp.Services.PaymentProcessingService.ProcessPaymentAsync(Int32 accountId, Decimal amount) in /src/Services/PaymentProcessingService.cs:line  thirty-some
// ...
// 2025-06-30 11:16:10.250 +02:00 [INFO] Request to ProcessPaymentAsync for account 5
// 2025-06-30 11:16:10.352 +02:00 [ERR] MyApp.Services.PaymentProcessingService 
//     System.Collections.Generic.KeyNotFoundException: The given key was not present in the dictionary.

