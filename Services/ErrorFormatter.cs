using System;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace DotX.Desktop.Services;

public static class ErrorFormatter
{
    public static string Format(string? rawError, HttpStatusCode? statusCode = null)
    {
        // 1. Check HTTP status code first
        if (statusCode.HasValue)
        {
            switch (statusCode.Value)
            {
                case HttpStatusCode.BadGateway: // 502
                    return "The backend server is currently unavailable (502 Bad Gateway). Please try again in a moment.";
                case HttpStatusCode.ServiceUnavailable: // 503
                    return "The service is temporarily undergoing maintenance (503). Please try again shortly.";
                case HttpStatusCode.GatewayTimeout: // 504
                    return "Connection timed out (504). Please check your internet connection and retry.";
                case HttpStatusCode.InternalServerError: // 500
                    break;
                case HttpStatusCode.NotFound: // 404
                    return "The requested service endpoint was not found (404).";
                case HttpStatusCode.PaymentRequired: // 402
                    return "Storage quota exceeded. Please upgrade your plan or delete existing files.";
                case HttpStatusCode.RequestEntityTooLarge: // 413
                    return "The selected file is too large to upload (DotX maximum is 500 MB).";
            }
        }

        if (string.IsNullOrWhiteSpace(rawError))
        {
            return statusCode.HasValue && statusCode.Value == HttpStatusCode.InternalServerError
                ? "An internal server error occurred (500). Please try again later."
                : "An unexpected error occurred. Please try again.";
        }

        var clean = rawError.Trim();

        // 2. Detect and handle raw HTML (e.g. from Nginx, proxies, or gateways)
        if (clean.Contains("<html", StringComparison.OrdinalIgnoreCase) ||
            clean.Contains("<!DOCTYPE", StringComparison.OrdinalIgnoreCase) ||
            clean.Contains("<body", StringComparison.OrdinalIgnoreCase) ||
            clean.Contains("<center>", StringComparison.OrdinalIgnoreCase))
        {
            if (clean.Contains("502 Bad Gateway", StringComparison.OrdinalIgnoreCase) || clean.Contains("Bad Gateway", StringComparison.OrdinalIgnoreCase))
            {
                return "The server is currently unavailable (502 Bad Gateway). Please try again in a moment.";
            }
            if (clean.Contains("504 Gateway", StringComparison.OrdinalIgnoreCase))
            {
                return "The server connection timed out (504). Please check your internet connection and try again.";
            }
            if (clean.Contains("503 Service", StringComparison.OrdinalIgnoreCase))
            {
                return "The service is temporarily unavailable (503). Please try again shortly.";
            }
            if (clean.Contains("500 Internal", StringComparison.OrdinalIgnoreCase))
            {
                return "An internal server error occurred (500). Please try again later.";
            }
            if (clean.Contains("404 Not Found", StringComparison.OrdinalIgnoreCase))
            {
                return "The service endpoint could not be reached (404).";
            }

            var titleMatch = Regex.Match(clean, @"<title>(.*?)</title>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (titleMatch.Success && !string.IsNullOrWhiteSpace(titleMatch.Groups[1].Value))
            {
                var title = titleMatch.Groups[1].Value.Trim();
                return $"Server notice: {title}. Please try again later.";
            }

            return "Unable to reach server. Please verify your network and try again.";
        }

        // Trim quotation marks if the server returned a quoted JSON string
        if (clean.StartsWith("\"") && clean.EndsWith("\"") && clean.Length >= 2)
        {
            clean = clean.Substring(1, clean.Length - 2);
        }

        // Extract detail from gRPC status string e.g. Status(StatusCode="NotFound", Detail="Invalid API Key")
        if (clean.Contains("Detail=", StringComparison.OrdinalIgnoreCase))
        {
            var match = Regex.Match(clean, @"Detail=""([^""]+)""", RegexOptions.IgnoreCase);
            if (match.Success && !string.IsNullOrWhiteSpace(match.Groups[1].Value))
            {
                clean = match.Groups[1].Value;
            }
        }

        // If JSON error payload like { "message": "..." } or { "error": "..." }
        if (clean.StartsWith("{") && clean.EndsWith("}"))
        {
            try
            {
                using var jsonDoc = JsonDocument.Parse(clean);
                if (jsonDoc.RootElement.TryGetProperty("message", out var msgProp) && !string.IsNullOrWhiteSpace(msgProp.GetString()))
                {
                    clean = msgProp.GetString()!;
                }
                else if (jsonDoc.RootElement.TryGetProperty("error", out var errProp) && !string.IsNullOrWhiteSpace(errProp.GetString()))
                {
                    clean = errProp.GetString()!;
                }
                else if (jsonDoc.RootElement.TryGetProperty("detail", out var detailProp) && !string.IsNullOrWhiteSpace(detailProp.GetString()))
                {
                    clean = detailProp.GetString()!;
                }
                else if (jsonDoc.RootElement.TryGetProperty("title", out var titleProp) && !string.IsNullOrWhiteSpace(titleProp.GetString()))
                {
                    clean = titleProp.GetString()!;
                }
            }
            catch
            {
                // Not JSON, continue
            }
        }

        // Specific backend error codes
        if (clean.Equals("STORAGE_QUOTA_EXCEEDED", StringComparison.OrdinalIgnoreCase))
        {
            return "Your storage quota has been exceeded. Please delete unused files or upgrade your plan.";
        }
        if (clean.Equals("FILE_TOO_LARGE", StringComparison.OrdinalIgnoreCase))
        {
            return "File is too large. DotX allows documents up to 500 MB.";
        }

        // Map known authentication errors
        if (clean.Contains("Invalid API Key", StringComparison.OrdinalIgnoreCase) || clean.Contains("NotFound", StringComparison.OrdinalIgnoreCase))
        {
            return "Invalid API Key. Please verify your key in the console.";
        }
        
        if (clean.Contains("Invalid PIN", StringComparison.OrdinalIgnoreCase) || 
            clean.Contains("Invalid Password", StringComparison.OrdinalIgnoreCase) || 
            clean.Contains("PIN is incorrect", StringComparison.OrdinalIgnoreCase) ||
            clean.Contains("PIN", StringComparison.OrdinalIgnoreCase))
        {
            return "Invalid Security PIN. Please enter your 6-digit PIN.";
        }

        if (clean.Contains("Unauthenticated", StringComparison.OrdinalIgnoreCase) || 
            clean.Contains("Unauthorized", StringComparison.OrdinalIgnoreCase))
        {
            return "Authentication failed. Please check your credentials.";
        }

        // Map known raw technical errors
        if (clean.Contains("Connection refused", StringComparison.OrdinalIgnoreCase) ||
            clean.Contains("No connection could be made", StringComparison.OrdinalIgnoreCase))
        {
            return "Unable to connect to the backend server. Please verify your connection.";
        }

        return clean;
    }
}
