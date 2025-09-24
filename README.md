# Kulipa .NET Client Library

C# .NET client library for integrating with the Kulipa crypto-based payment card API.  

## Overview
This library abstracts low-level HTTP communication, authentication, and request/response handling into a reusable SDK, enabling developers to build reliable financial applications faster with C# and .NET 8.0+.

## Key Features
### 📋 Resource-Based Architecture
  - Users Management - Create and manage user accounts with KYC support
  - Card Operations - Issue, manage, and control payment cards
  - Wallet Management - Handle crypto wallets, top-ups, and withdrawals
  - Card Payments - Track and manage card payment transactions
  - Webhooks - Secure webhook signature verification with ECDSA

### 🛡️ Security & Reliability
  - API Key Authentication - Secure authentication with configurable API keys
  - Idempotency Support - Built-in idempotency for POST/PUT operations
  - Rate Limiting - Intelligent rate limit handling with backoff strategies
  - Retry Policies - Configurable retry logic with exponential backoff
  - Circuit Breaker - Resilient HTTP clients using Polly

### 🔧 Developer Experience
  - Dependency Injection - Full integration with `Microsoft.Extensions.DependencyInjection`
  - Strongly Typed Models - Complete type safety for all API operations
  - Comprehensive Error Handling - Custom exception hierarchy for different HTTP status codes
  - XML Documentation - Complete IntelliSense support
  - Async/Await Support - Modern asynchronous programming patterns

## Requirements
- .NET 8.0 or later
- Secure storage of API keys and secrets (not hardcoded, not logged)

## Installation

```powershell
dotnet add package Kulipa.Sdk
```

## Quick Start
```csharp
  // Register services
  services.AddKulipaSdk(options =>
  {
      options.ApiKey = "your-api-key";
      options.Environment = KulipaEnvironment.Sandbox;
  });

  // Use the client
  public class PaymentService
  {
      private readonly IKulipaClient _kulipaClient;

      public PaymentService(IKulipaClient kulipaClient)
      {
          _kulipaClient = kulipaClient;
      }

      public async Task CreateUser()
      {
          var user = await _kulipaClient.Users.CreateAsync(new CreateUserRequest
          {
              // User details
          });
      }
  }
```

## Testing Support
The SDK is built with testing in mind:
  - MSTest framework with FluentAssertions
  - Moq for dependency mocking
  - Comprehensive unit and integration test coverage
  - Test utilities for webhook signature verification
  
## Development Guidelines
- Architecture: Clean architecture, SOLID principles
- Error Handling: Timeouts, retries, and structured exceptions
- Testing: Unit + integration tests with mocks for API responses
- Docs: XML documentation, examples, alignment with official API docs
- Versioning: Semantic versioning (SemVer)
- Distribution: Published via NuGet with release notes

## Contributing
Contributions are welcome. Please follow the repository’s coding standards and submit pull requests with tests included.

## License
This SDK is licensed under an Apache-2.0 license. See the [LICENSE](https://github.com/Beans-BV/kulipa-dotnet-sdk/blob/master/LICENSE.txt) file for details.

## Support
For questions, issues, or feature requests, please:
- Open an issue on GitHub
- Check the API documentation
- Review the SDK documentation
  
## Reference
- NuGet Package: https://www.nuget.org/packages/Kulipa.Sdk
- API Documentation: https://kulipa.readme.io/reference/