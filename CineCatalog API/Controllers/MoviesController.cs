using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CineCatalog_API.Application.DTOs;
using CineCatalog_API.Application.DTOs.Common;
using CineCatalog_API.Application.Interfaces;

namespace CineCatalog_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;
        private readonly IValidator<MovieCreateRequest> _createValidator;
        private readonly IValidator<MovieUpdateRequest> _updateValidator;
        private readonly IValidator<ReviewCreateRequest> _reviewCreateValidator;
        private readonly IValidator<ReviewUpdateRequest> _reviewUpdateValidator;

        public MoviesController(
            IMovieService movieService,
            IValidator<MovieCreateRequest> createValidator,
            IValidator<MovieUpdateRequest> updateValidator,
            IValidator<ReviewCreateRequest> reviewCreateValidator,
            IValidator<ReviewUpdateRequest> reviewUpdateValidator)
        {
            _movieService = movieService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _reviewCreateValidator = reviewCreateValidator;
            _reviewUpdateValidator = reviewUpdateValidator;
        }

        /// <summary>
        /// Lista os filmes cadastrados com paginação, filtros e ordenação.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResult<MovieResponse>))]
        public async Task<IActionResult> Get([FromQuery] MovieQueryParameters queryParams)
        {
            var result = await _movieService.GetFilteredAndPaginatedAsync(queryParams);
            return Ok(result);
        }

        /// <summary>
        /// Obtém os detalhes completos de um filme por ID, incluindo avaliações.
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MovieDetailResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var movie = await _movieService.GetByIdAsync(id);
            return Ok(movie);
        }

        /// <summary>
        /// Cadastra um novo filme no catálogo. (Requer Autenticação)
        /// </summary>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(MovieDetailResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Create([FromBody] MovieCreateRequest request)
        {
            var validationResult = await _createValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToDictionary());
            }

            var response = await _movieService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

        /// <summary>
        /// Atualiza as informações de um filme cadastrado. (Requer Autenticação)
        /// </summary>
        [HttpPut("{id:guid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MovieDetailResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] MovieUpdateRequest request)
        {
            var validationResult = await _updateValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToDictionary());
            }

            var response = await _movieService.UpdateAsync(id, request);
            return Ok(response);
        }

        /// <summary>
        /// Remove um filme do catálogo. (Requer Autenticação)
        /// </summary>
        [HttpDelete("{id:guid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            await _movieService.DeleteAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Adiciona uma avaliação de nota/comentário para o filme. (Requer Autenticação)
        /// </summary>
        [HttpPost("{id:guid}/reviews")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ReviewResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AddReview([FromRoute] Guid id, [FromBody] ReviewCreateRequest request)
        {
            var validationResult = await _reviewCreateValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToDictionary());
            }

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized("Usuário não autenticado ou inválido.");
            }

            var response = await _movieService.AddReviewAsync(id, userId, request);
            return CreatedAtAction(nameof(GetReviews), new { id = response.MovieId }, response);
        }

        /// <summary>
        /// Recupera a lista de avaliações de um filme específico.
        /// </summary>
        [HttpGet("{id:guid}/reviews")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ReviewResponse>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetReviews([FromRoute] Guid id)
        {
            var reviews = await _movieService.GetReviewsAsync(id);
            return Ok(reviews);
        }

        /// <summary>
        /// Atualiza uma avaliação existente. Apenas o autor pode editar. (Requer Autenticação)
        /// </summary>
        [HttpPut("{id:guid}/reviews/{reviewId:guid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReviewResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateReview([FromRoute] Guid id, [FromRoute] Guid reviewId, [FromBody] ReviewUpdateRequest request)
        {
            var validationResult = await _reviewUpdateValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToDictionary());
            }

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized("Usuário não autenticado ou inválido.");
            }

            var response = await _movieService.UpdateReviewAsync(id, reviewId, userId, request);
            return Ok(response);
        }

        /// <summary>
        /// Remove uma avaliação existente. Apenas o autor pode excluir. (Requer Autenticação)
        /// </summary>
        [HttpDelete("{id:guid}/reviews/{reviewId:guid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteReview([FromRoute] Guid id, [FromRoute] Guid reviewId)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized("Usuário não autenticado ou inválido.");
            }

            await _movieService.DeleteReviewAsync(id, reviewId, userId);
            return NoContent();
        }
    }
}
