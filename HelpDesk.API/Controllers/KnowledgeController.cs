using System.Net;
using System.Security.Claims;
using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Common.Resources;
using HelpDesk.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Org.BouncyCastle.Bcpg;

namespace HelpDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType(StatusCodes.Status409Conflict)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class KnowledgeController(ICategoryService categoryService, IArticleService articleService,
 IArticleTranslationService articleTranslationService, IKBSearchService kbSearchService, IImageService imageService,
 IArticleViewService articleViewService, IResponseService<object> responseService, IArticleFeddbackService articleFeddbackService,
 IStringLocalizer<Messages> localizer) : ControllerBase
{
    private readonly IImageService _fileService = imageService;
    private readonly ICategoryService _categoryService = categoryService;
    private readonly IArticleService _articleService = articleService;
    private readonly IArticleTranslationService _articleTranslationService = articleTranslationService;
    private readonly IKBSearchService _kbSearchService = kbSearchService;
    private readonly IArticleViewService _articleViewService = articleViewService;
    private readonly IArticleFeddbackService _articleFeddbackService = articleFeddbackService;
    private readonly IResponseService<object> _responseService = responseService;
    private readonly IStringLocalizer<Messages> _localizer = localizer;

    /// <summary>
    /// Get all categories with optional projectId and search parameters
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="search"></param>
    [HttpGet("get-categories")]
    public async Task<IActionResult> GetCategories([FromQuery] int? projectId = null, [FromQuery] string? search = null)
    {
        IEnumerable<CategoryDto> result = await _categoryService.GetCategoriesAsync(projectId, search);
        return _responseService.GetSuccessResponse(HttpStatusCode.OK, result, [_localizer["ENTITY_FETCH_SUCCEED", _localizer["FIELD_CATEGORY"]]]);
    }
    
    /// <summary>
    /// Get all articles with optional filter parameters
    /// </summary>
    /// <param name="filter"></param>
    [HttpGet("get-articles")]
    public async Task<IActionResult> GetArticles([FromQuery] ArticleFilterDto filter)
    {
        IEnumerable<ArticleDto> result = await _articleService.GetArticlesAsync(filter);
        return _responseService.GetSuccessResponse(HttpStatusCode.OK, result, [_localizer["ENTITY_FETCH_SUCCEED", _localizer["FIELD_ARTICLE"]]]);
    }

    /// <summary>
    /// Get KB search analytics with optional projectId, fromDate, and toDate parameters
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="fromDate"></param>
    /// <param name="toDate"></param>
    [HttpGet("get-kb-search-analytics")]
    public async Task<IActionResult> GetKbSearchAnalytics([FromQuery] int? projectId = null, [FromQuery] DateOnly? fromDate = null, [FromQuery] DateOnly? toDate = null)
    {
        IEnumerable<KbSearchAnalyticsDto> result = await _kbSearchService.GetKbSearchAnalyticsAsync(projectId, fromDate, toDate);
        return _responseService.GetSuccessResponse(HttpStatusCode.OK, result, [_localizer["ENTITY_FETCH_SUCCEED", _localizer["FIELD_KB_SEARCH_ANALYTICS"]]]);
    }
    
    /// <summary>
    /// Get article translations by article ID
    /// </summary>
    /// <param name="articleId"></param>
    [HttpGet("get-category-by-id/{categoryId}")]
    public async Task<IActionResult> GetCategoryById([FromRoute] int categoryId)
    {
        CategoryDto result = await _categoryService.GetCategoryByIdAsync(categoryId);
        return _responseService.GetSuccessResponse(HttpStatusCode.OK, result, [_localizer["ENTITY_FETCH_SUCCEED", _localizer["FIELD_CATEGORY"]]]);
    }

    /// <summary>
    /// Get article by ID
    /// </summary>
    /// <param name="articleId"></param>
    [HttpGet("get-article-by-id/{articleId}")]
    public async Task<IActionResult> GetArticleById([FromRoute] int articleId)
    {
        ArticleDto result = await _articleService.GetArticleByIdAsync(articleId);
        return _responseService.GetSuccessResponse(HttpStatusCode.OK, result, [_localizer["ENTITY_FETCH_SUCCEED", _localizer["FIELD_ARTICLE"]]]);
    }
    
    /// <summary>
    /// Add a new category
    /// </summary>
    /// <param name="request"></param>
    [HttpPost("add-category")]
    public async Task<IActionResult> AddCategory([FromBody] CategoryCreateDto request)
    {
        string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (UserId == null) throw new UnauthorizedAccessException(_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]);
        request.CreatedBy = int.Parse(UserId);
        CategoryDto result = await _categoryService.AddCategoryAsync(request);
        return _responseService.GetSuccessResponse(HttpStatusCode.OK, result, [_localizer["ENTITY_CREATION_SUCCEED", _localizer["FIELD_CATEGORY"]]]);
    }
    
    /// <summary>
    /// Add a new article
    /// </summary>
    /// <param name="request"></param>
    [HttpPost("add-article")]
    public async Task<IActionResult> AddArticle([FromBody] ArticleRequestDto request)
    {
        string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (UserId == null) throw new UnauthorizedAccessException(_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]);
        request.CreatedBy = int.Parse(UserId);
        ArticleCreateDto result = await _articleService.AddArticleAsync(request);
        return _responseService.GetSuccessResponse(HttpStatusCode.OK, result, [_localizer["ENTITY_CREATION_SUCCEED", _localizer["FIELD_ARTICLE"]]]);
    }
    
    /// <summary>
    /// Add article feedback
    /// </summary>
    /// <param name="request"></param>
    [HttpPost("add-article-feedback")]
    public async Task<IActionResult> AddArticleFeedback([FromBody] ArticleFeedbackCreateDto request)
    {
        ArticleFeedbackDto result = await _articleFeddbackService.AddArticleFeedback(request);
        return _responseService.GetSuccessResponse(HttpStatusCode.OK, result, [_localizer["ENTITY_CREATION_SUCCEED", _localizer["FIELD_ARTICLE_FEEDBACK"]]]);
    }
    
    /// <summary>
    /// Add article view
    /// </summary>
    /// <param name="request"></param>
    [HttpPost("add-article-view")]
    public async Task<IActionResult> AddArticleView([FromBody] ArticleViewCreateDto request)
    {
        ArticleViewDto result = await _articleViewService.AddArticleView(request);
        return _responseService.GetSuccessResponse(HttpStatusCode.OK, result, [_localizer["ENTITY_CREATION_SUCCEED", _localizer["FIELD_ARTICLE_VIEW"]]]);
    }
    
    /// <summary>
    /// Add KB search analytics
    /// </summary>
    /// <param name="request"></param>
    [HttpPost("add-kb-search-analytics")]
    public async Task<IActionResult> AddKbSearchAnalytics([FromBody] KbSearchAnalyticsCreateDto request)
    {
        KbSearchAnalyticsDto result = await _kbSearchService.AddKbSearchAnalyticsAsync(request);
        return _responseService.GetSuccessResponse(HttpStatusCode.OK, result, [_localizer["ENTITY_CREATION_SUCCEED", _localizer["FIELD_KB_SEARCH_ANALYTICS"]]]);
    }
    
    /// <summary>
    /// Add article translation
    /// </summary>
    /// <param name="request"></param>
    [HttpPost("add-article-translation")]
    public async Task<IActionResult> AddArticleTranslation([FromBody] ArticleTranslationCreateDto request)
    {
        ArticleTranslationDto result = await _articleTranslationService.AddArticleTranslationAsync(request);
        return _responseService.GetSuccessResponse(HttpStatusCode.OK, result, [_localizer["ENTITY_CREATION_SUCCEED", _localizer["FIELD_ARTICLE_TRANSLATION"]]]);
    }
    
    /// <summary>
    /// Update an existing category
    /// </summary>
    /// <param name="request"></param>
    [HttpPut("update-category")]
    public async Task<IActionResult> UpdateCategory([FromBody] CategoryUpdateDto request)
    {
        string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (UserId == null) throw new UnauthorizedAccessException(_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]);
        request.UpdatedBy = int.Parse(UserId);
        CategoryDto result = await _categoryService.UpdateCategoryAsync(request);
        return _responseService.GetSuccessResponse(HttpStatusCode.OK, result, [_localizer["ENTITY_UPDATE_SUCCEED", _localizer["FIELD_CATEGORY"]]]);
    }

    /// <summary>
    /// Update an existing article
    /// </summary>
    /// <param name="request"></param>
    [HttpPut("update-article")]
    public async Task<IActionResult> UpdateArticle([FromBody] ArticleUpdateDto request)
    {
        string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (UserId == null) throw new UnauthorizedAccessException(_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]);
        request.UpdatedBy = int.Parse(UserId);
        ArticleCreateDto result = await _articleService.UpdateArticleAsync(request);
        return _responseService.GetSuccessResponse(HttpStatusCode.OK, result, [_localizer["ENTITY_UPDATE_SUCCEED", _localizer["FIELD_ARTICLE"]]]);
    }
    
    /// <summary>
    /// Delete category by ID
    /// </summary>
    /// <param name="categoryId"></param>
    [HttpDelete("delete-category/{categoryId}")]
    public async Task<IActionResult> DeleteCategory([FromRoute] int categoryId)
    {
        string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (UserId == null) throw new UnauthorizedAccessException(_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]);
        int updatedBy = int.Parse(UserId);
        KnowledgeBaseResponseDto result = await _categoryService.DeleteCategoryAsync(categoryId, updatedBy);
        return _responseService.GetSuccessResponse(HttpStatusCode.OK, result, [_localizer["ENTITY_DELETION_SUCCEED", _localizer["FIELD_CATEGORY"]]]);
    }

    /// <summary>
    /// Delete article by ID
    /// </summary>
    /// <param name="articleId"></param>
    [HttpDelete("delete-article/{articleId}")]
    public async Task<IActionResult> DeleteArticle([FromRoute] int articleId)
    {
        string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (UserId == null) throw new UnauthorizedAccessException(_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]);

        int updatedBy = int.Parse(UserId);
        KnowledgeBaseResponseDto result = await _articleService.DeleteArticleAsync(articleId, updatedBy);
        return _responseService.GetSuccessResponse(HttpStatusCode.OK, result, [_localizer["ENTITY_DELETION_SUCCEED", _localizer["FIELD_ARTICLE"]]]);
    }
    
    /// <summary>
    /// Delete article translation by ID
    /// </summary>
    /// <param name="translationId"></param>
    [HttpDelete("delete-article-translation/{translationId}")]

    public async Task<IActionResult> DeleteArticleTranslation([FromRoute] int translationId)
    {
        KnowledgeBaseResponseDto result = await _articleTranslationService.DeleteArticleTranslationAsync(translationId);
        return _responseService.GetSuccessResponse(HttpStatusCode.OK, result, [_localizer["ENTITY_DELETION_SUCCEED", _localizer["FIELD_ARTICLE_TRANSLATION"]]]);
    }
    
    /// <summary>
    /// Archive article by ID
    /// </summary>
    /// <param name="articleId"></param>
    /// <param name="updatedBy"></param>
    [HttpPut("archive-article/{articleId}")]
    public async Task<IActionResult> ArchiveArticle([FromRoute] int articleId, [FromQuery] int updatedBy)
    {
        KnowledgeBaseResponseDto result = await _articleService.ArchiveArticleAsync(articleId, updatedBy);
        return _responseService.GetSuccessResponse(HttpStatusCode.OK, result, [_localizer["ENTITY_UPDATION_SUCCEED", _localizer["FIELD_ARTICLE"]]]);
    }
    
    /// <summary>
    /// Upload article file
    /// </summary>
    /// <param name="file"></param> 
    [HttpPost("upload-article-file")]
    public async Task<IActionResult> UploadArticleFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        var relativePath = await _fileService.SaveImageFileAsync(file, "uploads/articles");

        var absoluteUrl = $"{Request.Scheme}://{Request.Host}{relativePath}";

        return Ok(new { url = absoluteUrl });
    }

}
