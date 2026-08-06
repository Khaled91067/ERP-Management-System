
namespace ERP.Application.Behaviors;

using System.Diagnostics;

using global::Microsoft.Extensions.Logging;

using MediatR;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        
        _logger.LogInformation("Starting Request: {RequestName}", requestName);
        var timer = Stopwatch.StartNew();

        try
        {
            var response = await next();
            
            timer.Stop();
            _logger.LogInformation("Completed Request: {RequestName} in {ElapsedMilliseconds}ms", requestName, timer.ElapsedMilliseconds);
            
            return response;
        }
        catch (Exception)
        {
            timer.Stop();
            _logger.LogWarning("Request: {RequestName} failed after {ElapsedMilliseconds}ms", requestName, timer.ElapsedMilliseconds);
            throw;
        }
    }
}

