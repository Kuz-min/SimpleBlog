using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Models;
using SimpleBlog.RequestModels;
using SimpleBlog.Services;
using SimpleBlog.ViewModels.ModelExtensions;

namespace SimpleBlog.Controllers;

[ApiController]
[Route("api/post-tags")]
public class PostTagController : BaseController
{
    public PostTagController(ILogger<PostTagController> logger, IPostTagService postTagService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _postTagService = postTagService ?? throw new ArgumentNullException(nameof(postTagService));
    }

    [HttpGet("{tagId:int}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] int tagId)
    {
        PostTag? tag;
        try
        {
            tag = await _postTagService.GetByIdAsync(tagId);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        if (tag == null)
            return NotFound();

        return Ok(tag.ToViewModel());
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        IEnumerable<PostTag> tags;
        try
        {
            tags = await _postTagService.GetAllAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        if (tags == null || tags.Count() < 1)
            return NotFound();

        var vm = tags.Select(p => p.ToViewModel());

        return Ok(vm);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] PostTagCreateRequestModel request)
    {
        var profile = await GetProfileAsync();

        if (profile == null)
            return StatusCode(StatusCodes.Status500InternalServerError);

        PostTag? tag;
        try
        {
            tag = await _postTagService.InsertAsync(new PostTag()
            {
                Title = request.Title,
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        _logger.LogInformation($"PostTag created with id {tag.Id} by user {profile.Id}");
        return StatusCode(StatusCodes.Status201Created, tag.ToViewModel());
    }

    [Authorize]
    [HttpPut("{tagId:int}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] int tagId, [FromBody] PostTagUpdateRequestModel request)
    {
        var profile = await GetProfileAsync();

        if (profile == null)
            return StatusCode(StatusCodes.Status500InternalServerError);

        PostTag? tag;
        try
        {
            tag = await _postTagService.GetByIdAsync(tagId);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        if (tag == null)
            return StatusCode(StatusCodes.Status400BadRequest);

        tag.Title = request.Title;

        try
        {
            await _postTagService.UpdateAsync(tag);

        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        _logger.LogInformation($"PostTag with id {tag.Id} updated by user {profile.Id}");
        return StatusCode(StatusCodes.Status200OK, tag.ToViewModel());
    }

    [Authorize]
    [HttpDelete("{tagId:int}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] int tagId)
    {
        var profile = await GetProfileAsync();

        if (profile == null)
            return StatusCode(StatusCodes.Status500InternalServerError);

        PostTag? tag;
        try
        {
            tag = await _postTagService.GetByIdAsync(tagId);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        if (tag == null)
            return StatusCode(StatusCodes.Status400BadRequest);

        try
        {
            await _postTagService.DeleteAsync(tag);

        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        _logger.LogInformation($"PostTag with id {tag.Id} deleted by user {profile.Id}");
        return StatusCode(StatusCodes.Status200OK);
    }

    private readonly ILogger<PostTagController> _logger;
    private readonly IPostTagService _postTagService;
}