using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;

namespace SimpleBlog.Controllers;

public abstract class BaseController<TController> : ControllerBase
{
    protected ILogger<TController> Logger
    {
        get
        {
            if (_logger == null)
                _logger = HttpContext.RequestServices.GetRequiredService<ILogger<TController>>();

            return _logger;
        }
    }

    protected IMapper Mapper
    {
        get
        {
            if (_mapper == null)
                _mapper = HttpContext.RequestServices.GetRequiredService<IMapper>();

            return _mapper;
        }
    }

    protected T Map<T>(object o) => Mapper.Map<T>(o);

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

    private ILogger<TController>? _logger = null;
    private IMapper? _mapper = null;
    private Guid _accountId = Guid.Empty;
}