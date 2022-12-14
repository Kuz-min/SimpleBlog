using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Authorization;
using SimpleBlog.ModelBinders;
using SimpleBlog.Models;
using SimpleBlog.RequestModels;
using SimpleBlog.Services;
using SimpleBlog.ViewModels.ModelExtensions;

namespace SimpleBlog.Controllers;

[ApiController]
[Route("api/posts")]
public class PostController : BaseController
{
    public PostController(ILogger<PostController> logger, IAuthorizationService authorizationService, IPostService postService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
        _postService = postService ?? throw new ArgumentNullException(nameof(postService));
    }

    [HttpGet("{postId:int}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] int postId)
    {
        Post? post;
        try
        {
            post = await _postService.GetByIdAsync(postId);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        if (post == null)
            return NotFound();

        return Ok(post.ToViewModel());
    }

    [HttpGet]
    public async Task<IActionResult> GetByIdAsync(
        [FromQuery(Name = "ids")][ModelBinder(typeof(SeparatedStringToArrayBinder))] IEnumerable<int>? ids = default
        )
    {
        if (ids == null || ids.Count() < 1)
            return StatusCode(StatusCodes.Status400BadRequest, "ids not set");

        IEnumerable<Post>? posts;
        try
        {
            posts = await _postService.GetByIdAsync(ids);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        if (posts == null || posts.Count() < 1)
            return NotFound();

        var vm = posts.Select(p => p.ToViewModel());

        return Ok(vm);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchAsync([FromQuery] PostSearchRequestModel request)
    {
        IEnumerable<Post> posts;
        try
        {
            posts = await _postService.SearchAsync(request.TagIds, request.Offset, request.Count);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        if (posts == null || posts.Count() < 1)
            return NotFound();

        var vm = posts.Select(p => p.ToViewModel());

        return Ok(vm);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] PostCreateRequestModel request)
    {
        var profile = await GetProfileAsync();

        if (profile == null)
            return StatusCode(StatusCodes.Status500InternalServerError);

        var post = new Post()
        {
            Title = request.Title,
            Content = request.Content,
            CreatedOn = DateTime.Now,
            OwnerId = profile.Id,
        };

        if (request.TagIds != null && request.TagIds.Count() > 0)
        {
            post.Tags = new List<Post_PostTag>();
            foreach (var tagId in request.TagIds)
            {
                post.Tags.Add(new Post_PostTag { PostTagId = tagId });
            }
        }

        try
        {
            await _postService.InsertAsync(post);

        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        _logger.LogInformation($"Post created with id {post.Id} by user {profile.Id}");
        return StatusCode(StatusCodes.Status201Created, post.ToViewModel());
    }

    [Authorize]
    [HttpPut("{postId:int}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] int postId, [FromBody] PostUpdateRequestModel request)
    {
        var profile = await GetProfileAsync();

        if (profile == null)
            return StatusCode(StatusCodes.Status500InternalServerError);

        Post? post;
        try
        {
            post = await _postService.GetByIdAsync(postId);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        if (post == null)
            return StatusCode(StatusCodes.Status400BadRequest);


        var authorizationResult = await _authorizationService.AuthorizeAsync(User, post, Policies.SameOwner);
        if (authorizationResult == null || !authorizationResult.Succeeded)
            return StatusCode(StatusCodes.Status403Forbidden);


        post.Title = request.Title;
        post.Content = request.Content;

        if (request.TagIds == null)
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

        try
        {
            await _postService.UpdateAsync(post);

        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        _logger.LogInformation($"Post with id {post.Id} updated by user {profile.Id}");
        return StatusCode(StatusCodes.Status200OK, post.ToViewModel());
    }

    [Authorize]
    [HttpDelete("{postId:int}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] int postId)
    {
        var profile = await GetProfileAsync();

        if (profile == null)
            return StatusCode(StatusCodes.Status500InternalServerError);

        Post? post;
        try
        {
            post = await _postService.GetByIdAsync(postId);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        if (post == null)
            return StatusCode(StatusCodes.Status400BadRequest);


        var authorizationResult = await _authorizationService.AuthorizeAsync(User, post, Policies.SameOwner);
        if (authorizationResult == null || !authorizationResult.Succeeded)
            return StatusCode(StatusCodes.Status403Forbidden);


        try
        {
            await _postService.DeleteAsync(post);

        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        _logger.LogInformation($"Post with id {post.Id} deleted by user {profile.Id}");
        return StatusCode(StatusCodes.Status200OK);
    }

    private readonly ILogger<PostController> _logger;
    private readonly IAuthorizationService _authorizationService;
    private readonly IPostService _postService;
}
