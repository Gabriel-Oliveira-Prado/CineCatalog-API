using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CineCatalog_API.Application.DTOs;
using CineCatalog_API.Application.Exceptions;
using CineCatalog_API.Application.Interfaces;
using CineCatalog_API.Domain.Entities;
using CineCatalog_API.Domain.Interfaces;

namespace CineCatalog_API.Application.Services
{
    public class GenreService : IGenreService
    {
        private readonly IGenreRepository _genreRepository;
        private readonly IMapper _mapper;

        public GenreService(IGenreRepository genreRepository, IMapper mapper)
        {
            _genreRepository = genreRepository;
            _mapper = mapper;
        }

        public async Task<GenreResponse> CreateAsync(GenreCreateRequest request)
        {
            var existingGenre = await _genreRepository.GetByNameAsync(request.Name);
            if (existingGenre != null)
            {
                throw new ConflictException($"O gênero '{request.Name}' já está cadastrado.");
            }

            var genre = new Genre
            {
                Id = Guid.NewGuid(),
                Name = request.Name
            };

            await _genreRepository.AddAsync(genre);

            return _mapper.Map<GenreResponse>(genre);
        }

        public async Task<IEnumerable<GenreResponse>> GetAllAsync()
        {
            var genres = await _genreRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<GenreResponse>>(genres);
        }

        public async Task<GenreResponse> GetByIdAsync(Guid id)
        {
            var genre = await _genreRepository.GetByIdAsync(id);
            if (genre == null)
            {
                throw new NotFoundException("Gênero não encontrado.");
            }

            return _mapper.Map<GenreResponse>(genre);
        }

        public async Task<GenreResponse> UpdateAsync(Guid id, GenreUpdateRequest request)
        {
            var genre = await _genreRepository.GetByIdAsync(id);
            if (genre == null)
            {
                throw new NotFoundException("Gênero não encontrado.");
            }

            var duplicateGenre = await _genreRepository.GetByNameAsync(request.Name);
            if (duplicateGenre != null && duplicateGenre.Id != id)
            {
                throw new ConflictException($"Já existe outro gênero cadastrado com o nome '{request.Name}'.");
            }

            genre.Name = request.Name;
            await _genreRepository.UpdateAsync(genre);

            return _mapper.Map<GenreResponse>(genre);
        }

        public async Task DeleteAsync(Guid id)
        {
            var genre = await _genreRepository.GetByIdAsync(id);
            if (genre == null)
            {
                throw new NotFoundException("Gênero não encontrado.");
            }

            await _genreRepository.DeleteAsync(genre);
        }
    }
}
