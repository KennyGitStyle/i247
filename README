## i247 Group Tech Test

### Introduction
In developing this .NET application, I've embraced a clean, maintainable, and robust architectural approach, focusing on resilience and usability. My primary goal was to ensure that the application could handle external dependencies gracefully, particularly when fetching data from the RandomUser.me API. Here, I'll outline my architectural choices, coding practices, and the rationale behind implementing retries to enhance application performance and reliability.

### Architectural Approach
Choosing N-Tier Architecture
I decided on an N-Tier architectural style to segregate responsibilities across different layers, enhancing modularity and facilitating independent development and testing. This approach comprises:

### Presentation Layer: Handles UI and user interactions.
Business Logic Layer (BLL): Encapsulates the core functionality and rules of the application.
Data Access Layer (DAL): Manages data interactions, particularly with external APIs.
This separation allows for scalability and maintainability, ensuring that changes in one layer (e.g., updating the UI or modifying business rules) have minimal impact on others.

### Dependency Injection (DI)
I used DI extensively to promote loose coupling and testability. By injecting dependencies (like services, configuration, and logging) into components, I made the code more modular and easier to test. DI is particularly advantageous in the BLL, where business services can be developed and tested independently of their concrete implementations.

### Coding Approach
Emphasis on Clean Code and SOLID Principles
Striving for readability and maintainability, I adhered to clean coding practices and SOLID principles. This approach ensures that each class has a single responsibility, interfaces are used for abstraction, and components are easily extendable without modifying existing code.

### Fluent Assertions and Moq for Testing
For unit testing, I utilized Fluent Assertions and Moq alongside xUnit. This combination allowed me to write expressive and flexible tests, mocking external dependencies (like HttpClient) and asserting outcomes with readable, intention-revealing language.

### Handling External API Calls with HttpClient
The application consumes the RandomUser.me API, employing HttpClient for HTTP requests. I chose to use IHttpClientFactory for creating HttpClient instances, avoiding socket exhaustion and ensuring efficient handling of HTTP connections.

Implementing Retries for Enhanced Reliability
### Rationale
Given the dependency on external services, network volatility or intermittent API failures can affect application stability. Implementing a retry mechanism mitigates these issues, enhancing resilience by attempting failed operations a configurable number of times before failing.

### Polly for Retry Policies
To implement retries, I integrated Polly, a .NET resilience and transient-fault-handling library. Polly's policies can be configured to handle specific exceptions or HTTP response codes, providing a robust solution for retrying failed requests with exponential backoff and circuit breaker patterns.

### Benefits of Retries
Improved User Experience: Retries can reduce the likelihood of a request failing due to temporary issues, leading to a smoother user experience.
Resilience to Transient Failures: The application becomes more resilient to transient failures, such as temporary network issues or short-lived problems with the external API.
Configurable Strategy: The retry strategy (including the number of attempts and wait duration) can be tailored to the application's needs and the characteristics of the external service.
### Conclusion
In developing this application, I aimed for a balance between clean architecture, maintainability, and operational resilience. The use of an N-Tier architecture, adherence to SOLID principles, and strategic implementation of retries have collectively enhanced the application's robustness and user experience. Looking forward, I plan to continuously refine these practices, incorporating feedback and evolving requirements to ensure the application remains responsive, reliable, and easy to maintain.
