using DTO.Payload;
using MGI.ClassificacaoContabil.API.ControllerAtributes;
using Microsoft.AspNetCore.Mvc.Controllers;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        string actionDescription = GetEndPointDescriptionAttribute(context);
        var response = new PayloadDTO($"Ocorreu um erro ao {actionDescription}", false);
        return context.Response.WriteAsJsonAsync(response);
    }

    private string GetEndPointDescriptionAttribute(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        var controllerActionDescriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();
        var actionName = controllerActionDescriptor?.ActionName ?? "ação indefinida.";

        return controllerActionDescriptor?.MethodInfo
            .GetCustomAttributes(typeof(ActionDescriptionAttribute), false)
            .Cast<ActionDescriptionAttribute>()
            .FirstOrDefault()?.Description ?? actionName;
    }
}
