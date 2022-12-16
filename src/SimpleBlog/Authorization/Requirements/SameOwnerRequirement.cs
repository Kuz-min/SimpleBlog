using Microsoft.AspNetCore.Authorization;

namespace SimpleBlog.Authorization;

public class SameOwnerRequirement : IAuthorizationRequirement { }