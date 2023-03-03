using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Constants;
using SimpleBlog.FileStorage;
using SimpleBlog.ModelBinders;
using SimpleBlog.Models;
using SimpleBlog.RequestModels;
using SimpleBlog.Services;
using SimpleBlog.ViewModels;
using System.Security.Claims;

namespace SimpleBlog.Controllers;

[Route("api/posts")]
public class PostController : BaseApiController<PostController>
{
    public PostController(IAuthorizationService authorizationService, IPublicFileStorage fileStorage, IPostService postService)
    {
        _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
        _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
        _postService = postService ?? throw new ArgumentNullException(nameof(postService));
    }

    [AllowAnonymous]
    [HttpGet("{postId:int}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] int postId)
    {
        var post = await _postService.GetByIdAsync(postId);

        if (post == null)
            return NotFound();

        return Ok(Map<PostViewModel>(post));
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetByIdAsync(
        [FromQuery(Name = "ids")][ModelBinder(typeof(SeparatedStringToArrayBinder))] IEnumerable<int>? ids = default
        )
    {
        if (ids == null || ids.Count() == 0)
            return BadRequest();

        var posts = await _postService.GetByIdAsync(ids);

        if (posts == null || posts.Count() == 0)
            return NotFound();

        var vm = posts.Select(Map<PostViewModel>);

        return Ok(vm);
    }

    [AllowAnonymous]
    [HttpGet("search")]
    public async Task<IActionResult> SearchAsync([FromQuery] PostSearchRequestModel request)
    {
        var posts = await _postService.SearchAsync(request.TagIds, request.Offset, request.Count);

        if (posts == null || posts.Count() == 0)
            return NotFound();

        var vm = posts.Select(Map<PostViewModel>);

        return Ok(vm);
    }

    [Authorize]
    [HttpPost]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(524_288)]//512 kib
    public async Task<IActionResult> CreateAsync([FromForm] PostCreateRequestModel request)
    {
        var accountId = GetAccountId();

        var post = new Post()
        {
            Title = request.Title,
            Content = request.Content,
            CreatedOn = DateTime.Now,
            OwnerId = accountId,
            Image = request.Image != null ? await SaveImageAsync(request.Image) : null,
            Tags = new HashSet<Post_PostTag>(),
        };

        if (request.TagIds != null && request.TagIds.Count() != 0)
        {
            foreach (var tagId in request.TagIds)
            {
                post.Tags.Add(new Post_PostTag { PostTagId = tagId });
            }
        }

        await _postService.InsertAsync(post);

        Logger.LogInformation($"Post with id {post.Id} created by user {accountId}");

        return Created($"api/posts/{post.Id}", Map<PostViewModel>(post));
    }

    [Authorize]
    //Policies.OwnerAccess || Policies.PostFullAccess
    [HttpPut("{postId:int}")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(524_288)]//512 kib
    public async Task<IActionResult> UpdateAsync([FromRoute] int postId, [FromForm] PostUpdateRequestModel request)
    {
        var accountId = GetAccountId();

        var post = await _postService.GetByIdAsync(postId);

        if (!(await IsAuthorized(User, Policies.OwnerAccess, post) || await IsAuthorized(User, Policies.PostFullAccess)))
            return Forbid();

        if (post == null)
            return BadRequest();

        post.Title = request.Title;
        post.Content = request.Content;

        if (request.Image != null)
        {
            post.Image = await SaveImageAsync(request.Image);
        }

        if (request.TagIds == null || request.TagIds.Count() == 0)
        {
            post.Tags.Clear();
        }
        else
        {
            foreach (var tag in post.Tags)
                if (!request.TagIds.Contains(tag.PostTagId))
                    post.Tags.Remove(tag);

            foreach (var id in request.TagIds)
                if (!post.Tags.Any(tag => tag.PostTagId == id))
                    post.Tags.Add(new Post_PostTag { PostTagId = id });
        }

        await _postService.UpdateAsync(post);

        Logger.LogInformation($"Post with id {post.Id} updated by user {accountId}");

        return Ok(Map<PostViewModel>(post));
    }

    [Authorize]
    //Policies.OwnerAccess || Policies.PostFullAccess
    [HttpDelete("{postId:int}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] int postId)
    {
        var accountId = GetAccountId();
        var post = await _postService.GetByIdAsync(postId);

        if (!(await IsAuthorized(User, Policies.OwnerAccess, post) || await IsAuthorized(User, Policies.PostFullAccess)))
            return Forbid();

        if (post == null)
            return BadRequest();

        await _postService.DeleteAsync(post);

        Logger.LogInformation($"Post with id {post.Id} deleted by user {accountId}");

        return Ok();
    }

    private async Task<bool> IsAuthorized(ClaimsPrincipal user, string policyName, Post? resource = default)
    {
        var authorizationResult = await _authorizationService.AuthorizeAsync(user, resource, policyName);
        return (authorizationResult != null && authorizationResult.Succeeded);
    }

    private async Task<Uri?> SaveImageAsync(IFormFile image)
    {
        var contentSubType = image.ContentType.Split('/') is [_, var b] ? b : null;
        var name = $"{Guid.NewGuid()}.{contentSubType}";
        using var stream = image.OpenReadStream();
        return await _fileStorage.CreateOrUpdateFileAsync(FileTypes.PostImage, name, stream);
    }

    private readonly IAuthorizationService _authorizationService;
    private readonly IPublicFileStorage _fileStorage;
    private readonly IPostService _postService;
}
