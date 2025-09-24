# Kulipa .NET Client Library

C# .NET client library for integrating with the Kulipa crypto-based payment card API.  
API documentation: [https://kulipa.readme.io/reference/](https://kulipa.readme.io/reference/)

---

## Overview

This library provides a strongly typed, secure, and maintainable integration layer between .NET applications and Kulipa’s external API endpoints.  
It abstracts low-level HTTP communication, authentication, and request/response handling into a reusable SDK, enabling developers to build reliable financial applications faster.

---

## Requirements

- .NET 8.0 or later
- Secure storage of API keys and secrets (not hardcoded, not logged)

---

## Installation

```powershell
dotnet add package Kulipa.Sdk

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
