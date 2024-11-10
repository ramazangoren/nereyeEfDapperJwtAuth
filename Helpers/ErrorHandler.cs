using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace api.Helpers
{
    public static class ErrorHandler
    {
        // Static method to handle errors
        public static ActionResult HandleError(Exception ex, string action, ILogger logger)
        {
            logger.LogError(ex, $"Error occurred during {action}.");
            return new StatusCodeResult(500); // Returns a generic 500 Internal Server Error
        }
    }
}
