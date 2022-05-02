# Functional APIs
This repository is a development sandbox for applying functional programming to C# web APIs. I've used it to test ideas before applying them to professional code.

## Current Features
1. RESTful routes backed by a consistent data storage interface
    - `DataApis.ModelController` provides a RESTful template for individual model controllers.
    - `Data.Repositories.IRepository` provides a consistent CRUD interface that `ModelController` can wrap almost directly.
    - `Data.Models.Model` establishes a consistent identification systems for persisted objects - both on and offline (See below).
1. Follows functional programming principles wherever possible
    - Models are records, encouraging immutability.
    - `IRepository` makes heavy use of the `Either` monad, which forces developers to anticipate and respond to exceptions and errors.
    - A preference for extension methods over instance methods encourages small, simple functions and facilitates method chaining for clean, concise, and legible code.
1. Consistent, useful error handling with `Data.Errors.StatusCodeError`
    - The `Code` and `Message` properties provide public information about what went wrong.
    - The `Exception` property propagates exception information for logging or other processing without breaking the normal flow of code. This is achieved by using the `Try` monad to avoid explicit `try/catch` blocks, and immediately converting any exceptions to `StatusCodeError`s as the left branch of an `Either<StatusCodeError, T>`. This approach features multiple benefits:
        1. Because exceptions are converted to `StatusCodeError`s close to where they're thrown, a meaningful status code and message can be generated without controllers needing to understand the inner workings of their dependencies,
        1. Deferring logging and other processing as long as possible enables optimal integration with different runtimes and platforms. For example, 
            - Different logger types can be injected into the controller without having to pass them into dependencies, and
            - The controller can choose the correct amount of data to expose depending on whether the endpoint is internal (and therefore secure) or public (and therefor insecure).
1. Support for Mobile or PWA front ends with offline mode
    - The `Id` type includes both a "permanent" and "temporary" option. This enables front ends to persist and manipulate new objects without saving them to the server by assigning a temporary id. By saving a history of the local operations, these front ends can replay their steps against the back end when they come online. The API will accept the temporary id in a POST route, and assign a permanent id. Then the front end substitutes the permanent id for the temporary in all subsequent steps of the history when reporting them to the back end.
1. Runs on Azure Functions
    - Allows for maximum scaling for inconsistent, unpredictable workflows with minimal DevOps effort.
    - Saves money over dedicated servers or VMs that must be scaled predictively and typically leave a large percentage of their resources as unused buffer.
1. Takes advantage of .NET 6 and C# 10 features such as
    - File-scoped namespace declarations,
    - Sleek null checking with `CallerArgumentExpression` (used in `Utilities.ArgumentValidation.ThrowIfNull)`),
    - Though the functional style avoids the use of `null`, nullable reference types are enabled to help catch programmer errors and enforce the functional style, and
    - Utilizes `System.Text.Json` as its serialization platform, taking advantage of recent performance improvements.

## Planned Features
1. Logging
    - Msft offers multiple logging implementations (`ITelemetryClient`, `ILogger`, etc.), but they all rely on extension methods for convenience, which cannot be easily mocked. Log calls are side effects, and should be unit tested. Therefore, it's important to wrap these interfaces with a custom (passthrough) implementation that can be easily mocked for testing.
1. Dependency Injection
    - While the default behavior in Azure Functions is to inject a logger as a parameter in each function, this approach will not work well with a custom logger as it would force each controller to perform the wrapping. It is much better to add a `Startup.cs` file to `PackingApi`, configure the loggers there, and injection them into controllers via their constructors.
    - Reflection can be used to discover all the controllers in `PackingApi/Controllers`, register them in the DI container, and register a corresponding `ILogger<T>`.
1. Add an ORM Database
    - As a proof of concept, I implemented `IRepository<T>` using an in-memory dictionary. This works for establishing interfaces and local testing, but it's obviously not an enterprise grade solution. Ultimately, I want to add a dockerized database backend. This will likely bring with it an update to the `IQuery` interface as simple filtering won't be ideal.
1. Make Repositories Async
    - Because C# dictionaries perform their operations synchronously, there was no point in making operations asynchronous, but file IO and database operations are naturally performed asynchronously. So in the future I will update `IRepository<T>`'s methods to be asynchronous.
