using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Authorization;
using SimpleBlog.Models;
using SimpleBlog.RequestModels;
using SimpleBlog.Services;
using SimpleBlog.ViewModels.ModelExtensions;

namespace SimpleBlog.Controllers;

[ApiController]
[Route("api/post-tags")]
public class PostTagController : BaseController<PostTagController>
{
    public PostTagController(IPostTagService postTagService)
    {
        _postTagService = postTagService ?? throw new ArgumentNullException(nameof(postTagService));
    }

    [HttpGet("{tagId:int}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] int tagId)
    {
        var tag = await _postTagService.GetByIdAsync(tagId);

        if (tag == null)
            return NotFound();

        return Ok(tag.ToViewModel());
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var tags = await _postTagService.GetAllAsync();

        if (tags == null || tags.Count() == 0)
            return NotFound();

        var vm = tags.Select(p => p.ToViewModel());

        return Ok(vm);
    }

    [Authorize]
    [Authorize(Policies.PostTagFullAccess)]
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] PostTagCreateRequestModel request)
    {
        var accountId = GetAccountId();

        var tag = await _postTagService.InsertAsync(new PostTag()
        {
            Title = request.Title,
        });

        Logger.LogInformation($"PostTag with id {tag.Id} created by user {accountId}");

        return Created($"api/post-tags/{tag.Id}", tag.ToViewModel());
    }

    [Authorize]
    [Authorize(Policies.PostTagFullAccess)]
    [HttpPut("{tagId:int}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] int tagId, [FromBody] PostTagUpdateRequestModel request)
    {
        var accountId = GetAccountId();

        var tag = await _postTagService.GetByIdAsync(tagId);

        if (tag == null)
            return BadRequest();

        tag.Title = request.Title;

        await _postTagService.UpdateAsync(tag);

        Logger.LogInformation($"PostTag with id {tag.Id} updated by user {accountId}");

        return Ok(tag.ToViewModel());
    }

    [Authorize]
    [Authorize(Policies.PostTagFullAccess)]
    [HttpDelete("{tagId:int}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] int tagId)
    {
        var accountId = GetAccountId();

        var tag = await _postTagService.GetByIdAsync(tagId);

        if (tag == null)
            return BadRequest();

        await _postTagService.DeleteAsync(tag);

        Logger.LogInformation($"PostTag with id {tag.Id} deleted by user {accountId}");

        return Ok();
    }

    private readonly IPostTagService _postTagService;
}