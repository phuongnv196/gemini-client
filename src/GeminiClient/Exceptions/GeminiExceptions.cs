// Copyright 2025 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Net;

namespace GoogleGenAI.Client.Exceptions;

/// <summary>
/// Base exception for all Google Generative AI client errors.
/// </summary>
public class GeminiException : Exception
{
    /// <summary>
    /// Error code associated with this exception.
    /// </summary>
    public string? ErrorCode { get; }

    /// <summary>
    /// HTTP status code if this is an HTTP-related error.
    /// </summary>
    public HttpStatusCode? StatusCode { get; }

    /// <summary>
    /// Additional details about the error.
    /// </summary>
    public Dictionary<string, object>? Details { get; }

    public GeminiException(string message) : base(message)
    {
    }

    public GeminiException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public GeminiException(string message, string? errorCode, HttpStatusCode? statusCode = null, Dictionary<string, object>? details = null) 
        : base(message)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
        Details = details;
    }

    public GeminiException(string message, string? errorCode, Exception innerException, HttpStatusCode? statusCode = null, Dictionary<string, object>? details = null) 
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
        Details = details;
    }
}

/// <summary>
/// Exception thrown when authentication fails.
/// </summary>
public class GeminiAuthenticationException : GeminiException
{
    public GeminiAuthenticationException(string message) : base(message, "AUTHENTICATION_ERROR", HttpStatusCode.Unauthorized)
    {
    }

    public GeminiAuthenticationException(string message, Exception innerException) 
        : base(message, "AUTHENTICATION_ERROR", innerException, HttpStatusCode.Unauthorized)
    {
    }
}

/// <summary>
/// Exception thrown when the request is invalid.
/// </summary>
public class GeminiBadRequestException : GeminiException
{
    public GeminiBadRequestException(string message) : base(message, "BAD_REQUEST", HttpStatusCode.BadRequest)
    {
    }

    public GeminiBadRequestException(string message, Exception innerException) 
        : base(message, "BAD_REQUEST", innerException, HttpStatusCode.BadRequest)
    {
    }

    public GeminiBadRequestException(string message, Dictionary<string, object>? details) 
        : base(message, "BAD_REQUEST", HttpStatusCode.BadRequest, details)
    {
    }
}

/// <summary>
/// Exception thrown when the request is forbidden.
/// </summary>
public class GeminiForbiddenException : GeminiException
{
    public GeminiForbiddenException(string message) : base(message, "FORBIDDEN", HttpStatusCode.Forbidden)
    {
    }

    public GeminiForbiddenException(string message, Exception innerException) 
        : base(message, "FORBIDDEN", innerException, HttpStatusCode.Forbidden)
    {
    }
}

/// <summary>
/// Exception thrown when the requested resource is not found.
/// </summary>
public class GeminiNotFoundException : GeminiException
{
    public GeminiNotFoundException(string message) : base(message, "NOT_FOUND", HttpStatusCode.NotFound)
    {
    }

    public GeminiNotFoundException(string message, Exception innerException) 
        : base(message, "NOT_FOUND", innerException, HttpStatusCode.NotFound)
    {
    }
}

/// <summary>
/// Exception thrown when rate limits are exceeded.
/// </summary>
public class GeminiRateLimitException : GeminiException
{
    /// <summary>
    /// When the rate limit will reset (if available).
    /// </summary>
    public DateTimeOffset? RetryAfter { get; }

    public GeminiRateLimitException(string message, DateTimeOffset? retryAfter = null) 
        : base(message, "RATE_LIMIT_EXCEEDED", HttpStatusCode.TooManyRequests)
    {
        RetryAfter = retryAfter;
    }

    public GeminiRateLimitException(string message, Exception innerException, DateTimeOffset? retryAfter = null) 
        : base(message, "RATE_LIMIT_EXCEEDED", innerException, HttpStatusCode.TooManyRequests)
    {
        RetryAfter = retryAfter;
    }
}

/// <summary>
/// Exception thrown when content is blocked by safety filters.
/// </summary>
public class GeminiContentFilteredException : GeminiException
{
    /// <summary>
    /// Safety ratings that caused the content to be blocked.
    /// </summary>
    public List<string> BlockedCategories { get; }

    public GeminiContentFilteredException(string message, List<string> blockedCategories) 
        : base(message, "CONTENT_FILTERED", HttpStatusCode.BadRequest)
    {
        BlockedCategories = blockedCategories ?? new List<string>();
    }

    public GeminiContentFilteredException(string message, List<string> blockedCategories, Exception innerException) 
        : base(message, "CONTENT_FILTERED", innerException, HttpStatusCode.BadRequest)
    {
        BlockedCategories = blockedCategories ?? new List<string>();
    }
}

/// <summary>
/// Exception thrown when a server error occurs.
/// </summary>
public class GeminiServerException : GeminiException
{
    public GeminiServerException(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError) 
        : base(message, "SERVER_ERROR", statusCode)
    {
    }

    public GeminiServerException(string message, Exception innerException, HttpStatusCode statusCode = HttpStatusCode.InternalServerError) 
        : base(message, "SERVER_ERROR", innerException, statusCode)
    {
    }
}

/// <summary>
/// Exception thrown when a timeout occurs.
/// </summary>
public class GeminiTimeoutException : GeminiException
{
    public GeminiTimeoutException(string message) : base(message, "TIMEOUT")
    {
    }

    public GeminiTimeoutException(string message, Exception innerException) 
        : base(message, "TIMEOUT", innerException)
    {
    }
}

/// <summary>
/// Exception thrown when configuration is invalid.
/// </summary>
public class GeminiConfigurationException : GeminiException
{
    public GeminiConfigurationException(string message) : base(message, "CONFIGURATION_ERROR")
    {
    }

    public GeminiConfigurationException(string message, Exception innerException) 
        : base(message, "CONFIGURATION_ERROR", innerException)
    {
    }
}
