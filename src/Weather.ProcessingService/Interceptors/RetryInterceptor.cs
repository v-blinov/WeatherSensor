using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Weather.ProcessingService.Interceptors;

public class RetryInterceptor : Interceptor
{
    private readonly ILogger<RetryInterceptor> _logger;
    private delegate AsyncDuplexStreamingCall<TRequest,TResponse> CustomContinuation<TRequest,TResponse>(ClientInterceptorContext<TRequest, TResponse> context) 
        where TRequest : class where TResponse : class;
    
    public RetryInterceptor(ILogger<RetryInterceptor> logger)
    {
        _logger = logger;
    }

    public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncDuplexStreamingCallContinuation<TRequest, TResponse> continuation)
    {
        return base.AsyncDuplexStreamingCall(context, continuation);
        
        // var result = base.AsyncDuplexStreamingCall(context, continuation);
        
        // if(result.GetStatus().StatusCode == StatusCode.Unavailable)
        // {
        //     var backoff = TimeSpan.FromSeconds(1);
        //     var maxBackoff = TimeSpan.FromSeconds(5);
        //     const double backoffMultiplier = 1.5;
        //
        //     _logger.LogWarning("Retry grpc bi-directional call");
        //     
        //     while(result.GetStatus().StatusCode == StatusCode.Unavailable)
        //     {
        //         Task.Delay(backoff).Wait();
        //         result = base.AsyncDuplexStreamingCall(context, continuation);
        //         if(backoff >= maxBackoff || backoff * backoffMultiplier >= maxBackoff)
        //             backoff *= backoffMultiplier;
        //     }
        // }

        // return result;
    }

    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        _logger.LogInformation("call DemoInterceptor");

        return base.AsyncUnaryCall(request, context, continuation);
    }
}
