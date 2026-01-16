using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace HC.Blazor.Extensions;

/// <summary>
/// Extension methods for IJSRuntime to safely handle JavaScript interop calls
/// and automatically catch JSDisconnectedException
/// </summary>
public static class JSRuntimeExtensions
{
    /// <summary>
    /// Safely invokes a JavaScript function that returns void, catching JSDisconnectedException
    /// </summary>
    /// <param name="jsRuntime">The JS runtime instance</param>
    /// <param name="identifier">The JavaScript function identifier</param>
    /// <param name="args">Arguments to pass to the JavaScript function</param>
    /// <returns>Task representing the async operation</returns>
    public static async ValueTask SafeInvokeVoidAsync(
        this IJSRuntime jsRuntime,
        string identifier,
        params object?[]? args)
    {
        try
        {
            await jsRuntime.InvokeVoidAsync(identifier, args);
        }
        catch (JSDisconnectedException)
        {
            // Component is being disposed, ignore the exception
            // This is expected behavior when the circuit disconnects
        }
        catch (TaskCanceledException)
        {
            // Task was cancelled, likely due to component disposal
            // This is also expected behavior
        }
    }

    /// <summary>
    /// Safely invokes a JavaScript function that returns a value, catching JSDisconnectedException
    /// </summary>
    /// <typeparam name="TValue">The return type</typeparam>
    /// <param name="jsRuntime">The JS runtime instance</param>
    /// <param name="identifier">The JavaScript function identifier</param>
    /// <param name="args">Arguments to pass to the JavaScript function</param>
    /// <returns>Task representing the async operation with the return value, or default(TValue) if disconnected</returns>
    public static async ValueTask<TValue> SafeInvokeAsync<TValue>(
        this IJSRuntime jsRuntime,
        string identifier,
        params object?[]? args)
    {
        try
        {
            return await jsRuntime.InvokeAsync<TValue>(identifier, args);
        }
        catch (JSDisconnectedException)
        {
            // Component is being disposed, return default value
            return default(TValue)!;
        }
        catch (TaskCanceledException)
        {
            // Task was cancelled, return default value
            return default(TValue)!;
        }
    }

    /// <summary>
    /// Safely invokes a JavaScript function that returns void with cancellation token, catching JSDisconnectedException
    /// </summary>
    /// <param name="jsRuntime">The JS runtime instance</param>
    /// <param name="identifier">The JavaScript function identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <param name="args">Arguments to pass to the JavaScript function</param>
    /// <returns>Task representing the async operation</returns>
    public static async ValueTask SafeInvokeVoidAsync(
        this IJSRuntime jsRuntime,
        string identifier,
        System.Threading.CancellationToken cancellationToken,
        params object?[]? args)
    {
        try
        {
            await jsRuntime.InvokeVoidAsync(identifier, cancellationToken, args);
        }
        catch (JSDisconnectedException)
        {
            // Component is being disposed, ignore the exception
        }
        catch (TaskCanceledException)
        {
            // Task was cancelled, ignore the exception
        }
    }

    /// <summary>
    /// Safely invokes a JavaScript function that returns a value with cancellation token, catching JSDisconnectedException
    /// </summary>
    /// <typeparam name="TValue">The return type</typeparam>
    /// <param name="jsRuntime">The JS runtime instance</param>
    /// <param name="identifier">The JavaScript function identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <param name="args">Arguments to pass to the JavaScript function</param>
    /// <returns>Task representing the async operation with the return value, or default(TValue) if disconnected</returns>
    public static async ValueTask<TValue> SafeInvokeAsync<TValue>(
        this IJSRuntime jsRuntime,
        string identifier,
        System.Threading.CancellationToken cancellationToken,
        params object?[]? args)
    {
        try
        {
            return await jsRuntime.InvokeAsync<TValue>(identifier, cancellationToken, args);
        }
        catch (JSDisconnectedException)
        {
            // Component is being disposed, return default value
            return default(TValue)!;
        }
        catch (TaskCanceledException)
        {
            // Task was cancelled, return default value
            return default(TValue)!;
        }
    }
}
