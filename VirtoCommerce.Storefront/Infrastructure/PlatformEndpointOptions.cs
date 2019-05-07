using System;

namespace VirtoCommerce.Storefront.Infrastructure
{
    public class PlatformEndpointOptions
    {
        public ApiAuthMode AuthMode { get; set; } = ApiAuthMode.BarrierToken;
        public Uri Url { get; set; }
        /// <summary>
        /// Credentials used for ApiAuthMode.BarrierToken mode
        /// </summary>
        public string AppId { get; set; }
        public string SecretKey { get; set; }

        /// <summary>
        /// Credentials used for ApiAuthMode.OAuthPassword mode
        /// </summary>
        public string UserName { get; set; }
        public string Password { get; set; }

        public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(30);

        public PollyPoliciesOptions PollyPolicies { get; set; }
    }



    /// <summary>
    /// Options for polly policies
    /// </summary>
    public class PollyPoliciesOptions
    {
        public CircuitBreakerPolicyOptions HttpCircuitBreaker { get; set; }
        public RetryPolicyOptions HttpRetry { get; set; }


        public class CircuitBreakerPolicyOptions
        {
            public TimeSpan DurationOfBreak { get; set; } = TimeSpan.FromSeconds(30);
            public int ExceptionsAllowedBeforeBreaking { get; set; } = 12;
        }

        public class RetryPolicyOptions
        {
            public int Count { get; set; } = 3;
            public int BackoffPower { get; set; } = 2;
        }
    }

}
