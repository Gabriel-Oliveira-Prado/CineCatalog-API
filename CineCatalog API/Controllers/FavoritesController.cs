using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CineCatalog_API.Application.DTOs;
using CineCatalog_API.Application.Interfaces;

namespace CineCatalog_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;

        public FavoritesController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        /// <summary>
        /// Lista os filmes favoritos do usuário autenticado. (Requer Autenticação)
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<MovieResponse>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMyFavorites()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized("Usuário não autenticado ou inválido.");
            }

            var favorites = await _favoriteService.GetUserFavoritesAsync(userId);
            return Ok(favorites);
        }

        /// <summary>
        /// Adiciona um filme aos favoritos do usuário autenticado. (Requer Autenticação)
        /// </summary>
        [HttpPost("{movieId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AddFavorite([FromRoute] Guid movieId)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized("Usuário não autenticado ou inválido.");
            }

            await _favoriteService.AddAsync(userId, movieId);
            return Ok(new { message = "Filme adicionado aos favoritos com sucesso." });
        }

        /// <summary>
        /// Remove um filme dos favoritos do usuário autenticado. (Requer Autenticação)
        /// </summary>
        [HttpDelete("{movieId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveFavorite([FromRoute] Guid movieId)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized("Usuário não autenticado ou inválido.");
            }

            await _favoriteService.RemoveAsync(userId, movieId);
            return Ok(new { message = "Filme removido dos favoritos com sucesso." });
        }
    }
}
