using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;

namespace SimpleBlog.Controllers;

public abstract class BaseController<T> : ControllerBase
{
    protected ILogger<T> Logger
    {
        get
        {
            if (_logger == null)
                _logger = HttpContext.RequestServices.GetRequiredService<ILogger<T>>();

            return _logger;
        }
    }

    protected Guid GetAccountId()
    {
        if (_accountId == Guid.Empty)
        {
            var rawAccountId = User.Claims.SingleOrDefault(c => c.Type == OpenIddictConstants.Claims.Subject)?.Value;

            if (string.IsNullOrWhiteSpace(rawAccountId))
                throw new Exception("User is not authenticated");

            _accountId = Guid.Parse(rawAccountId);
        }

        return _accountId;
    }

    private ILogger<T>? _logger = null;
    private Guid _accountId = Guid.Empty;
}