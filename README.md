# Discount Code Service 🏷️

*A high-performance gRPC service for generating and managing unique discount codes*

![gRPC](https://img.shields.io/badge/gRPC-%40latest-blue)
![.NET](https://img.shields.io/badge/.NET-6.0-purple)
![EF Core](https://img.shields.io/badge/EF%20Core-7.0-green)

## ✨ My Unique Implementation

After several iterations, I've built a service that goes beyond basic requirements with:

- **Smart Code Generation**  
  `VIP-XY7B9A` style codes using my custom pattern system  
  (Removed ambiguous characters like 0/O and 1/I for better usability)

- **Production-Grade Reliability**  
  - Thread-safe batch processing
  - Atomic database operations
  - Detailed error tracking

- **Client Success Story**  
  ```bash
  # Tested with 100% successful connection rate
  $ dotnet run --project Client
  > Connection established to grpc://localhost:5023
  > Generated 50 codes in 248ms
