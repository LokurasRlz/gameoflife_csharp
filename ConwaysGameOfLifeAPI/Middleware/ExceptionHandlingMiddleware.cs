﻿using Microsoft.AspNetCore.Http;
using Serilog;
using System.Net;
using System.Threading.Tasks;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            Log.Error($"Something went wrong: {ex}");
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsync(new
        {
            StatusCode = context.Response.StatusCode,
            Message = "Internal Server Error. Please try again later."
        }.ToString());
    }
}