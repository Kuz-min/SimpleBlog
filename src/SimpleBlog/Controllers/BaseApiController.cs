using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;

namespace SimpleBlog.Controllers;

[ApiController]
public abstract class BaseApiController<TController> : ControllerBase where TController : ControllerBase
{
    protected ILogger<TController> Logger
    {
        get => _logger ??= HttpContext.RequestServices.GetRequiredService<ILogger<TController>>();
    }

    protected IMapper Mapper
    {
        get => _mapper ??= HttpContext.RequestServices.GetRequiredService<IMapper>();
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
