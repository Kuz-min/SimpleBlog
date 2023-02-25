using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Constants;
using SimpleBlog.ModelBinders;
using SimpleBlog.Models;
using SimpleBlog.RequestModels;
using SimpleBlog.Services;
using SimpleBlog.ViewModels;

namespace SimpleBlog.Controllers;

[Route("api/posts")]
public class PostController : BaseApiController<PostController>
{
    public PostController(IAuthorizationService authorizationService, IPostService postService)
    {
        _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
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
    public async Task<IActionResult> CreateAsync([FromBody] PostCreateRequestModel request)
    {
        var accountId = GetAccountId();

        var post = new Post()
        {
            Title = request.Title,
            Content = request.Content,
            CreatedOn = DateTime.Now,
            OwnerId = accountId,
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
    //Policies.OwnerAccess
    [HttpPut("{postId:int}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] int postId, [FromBody] PostUpdateRequestModel request)
    {
        var accountId = GetAccountId();
        var post = await _postService.GetByIdAsync(postId);

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, post, Policies.OwnerAccess);
        if (authorizationResult == null || !authorizationResult.Succeeded)
            return Forbid();

        if (post == null)
            return BadRequest();

        post.Title = request.Title;
        post.Content = request.Content;

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
    //Policies.OwnerAccess
    [HttpDelete("{postId:int}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] int postId)
    {
        var accountId = GetAccountId();
        var post = await _postService.GetByIdAsync(postId);

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, post, Policies.OwnerAccess);
        if (authorizationResult == null || !authorizationResult.Succeeded)
            return Forbid();

        if (post == null)
            return BadRequest();

        await _postService.DeleteAsync(post);

        Logger.LogInformation($"Post with id {post.Id} deleted by user {accountId}");

        return Ok();
    }

    private readonly IAuthorizationService _authorizationService;
    private readonly IPostService _postService;
}
