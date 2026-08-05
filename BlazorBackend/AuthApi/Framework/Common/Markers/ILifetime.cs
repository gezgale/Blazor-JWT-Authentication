namespace AuthApi.Framework.Common.Markers;

/// <summary>
/// Marker interface indicating that a service should be registered
/// with the Dependency Injection container using the Scoped lifetime.
///
/// Scoped services are created once per HTTP request and shared
/// throughout that request.
/// </summary>
public interface IScopedDependency
{
}

/// <summary>
/// Marker interface indicating that a service should be registered
/// with the Dependency Injection container using the Transient lifetime.
///
/// A new instance is created every time the service is requested.
/// </summary>
public interface ITransientDependency
{
}

/// <summary>
/// Marker interface indicating that a service should be registered
/// with the Dependency Injection container using the Singleton lifetime.
///
/// A single instance is created and shared for the entire lifetime
/// of the application.
/// </summary>
public interface ISingletonDependency
{
}