using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CineCatalog_API.Application.DTOs;
using CineCatalog_API.Application.DTOs.Common;
using CineCatalog_API.Application.Exceptions;
using CineCatalog_API.Application.Interfaces;
using CineCatalog_API.Domain.Entities;
using CineCatalog_API.Domain.Interfaces;

namespace CineCatalog_API.Application.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public MovieService(
            IMovieRepository movieRepository,
            IGenreRepository genreRepository,
            IReviewRepository reviewRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _movieRepository = movieRepository;
            _genreRepository = genreRepository;
            _reviewRepository = reviewRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<MovieDetailResponse> CreateAsync(MovieCreateRequest request)
        {
            var movie = _mapper.Map<Movie>(request);
            movie.Id = Guid.NewGuid();
            movie.CreatedAt = DateTime.UtcNow;
            movie.UpdatedAt = DateTime.UtcNow;

            // Load and validate Genres
            foreach (var genreId in request.GenreIds)
            {
                var genre = await _genreRepository.GetByIdAsync(genreId);
                if (genre == null)
                {
                    throw new BadRequestException($"O gênero com o ID '{genreId}' não foi encontrado.");
                }
                movie.Genres.Add(genre);
            }

            await _movieRepository.AddAsync(movie);
            return _mapper.Map<MovieDetailResponse>(movie);
        }

        public async Task<PagedResult<MovieResponse>> GetFilteredAndPaginatedAsync(MovieQueryParameters queryParams, Guid? userId = null)
        {
            var (movies, totalCount) = await _movieRepository.GetFilteredAndPaginatedAsync(
                queryParams.Search,
                queryParams.Genre,
                queryParams.Director,
                queryParams.ReleaseYear,
                queryParams.MinRating,
                queryParams.MinDuration,
                queryParams.PageNumber,
                queryParams.PageSize,
                queryParams.SortBy,
                queryParams.IsDescending);

            var mappedMovies = _mapper.Map<List<MovieResponse>>(movies);
            if (userId.HasValue && mappedMovies.Count > 0)
            {
                var reviewsByMovie = (await _reviewRepository.GetByUserIdAsync(userId.Value))
                    .GroupBy(review => review.MovieId)
                    .ToDictionary(group => group.Key, group => group.First().Rating);

                foreach (var movie in mappedMovies)
                {
                    if (reviewsByMovie.TryGetValue(movie.Id, out var rating))
                    {
                        movie.CurrentUserReviewRating = rating;
                    }
                }
            }

            return new PagedResult<MovieResponse>(mappedMovies, totalCount, queryParams.PageNumber, queryParams.PageSize);
        }

        public async Task<MovieDetailResponse> GetByIdAsync(Guid id)
        {
            var movie = await _movieRepository.GetByIdWithDetailsAsync(id);
            if (movie == null)
            {
                throw new NotFoundException("Filme não encontrado.");
            }

            return _mapper.Map<MovieDetailResponse>(movie);
        }

        public async Task<MovieDetailResponse> UpdateAsync(Guid id, MovieUpdateRequest request)
        {
            var movie = await _movieRepository.GetByIdWithDetailsAsync(id);
            if (movie == null)
            {
                throw new NotFoundException("Filme não encontrado.");
            }

            // Map standard properties over the entity
            _mapper.Map(request, movie);
            movie.UpdatedAt = DateTime.UtcNow;

            // Update genres relation
            movie.Genres.Clear();
            foreach (var genreId in request.GenreIds)
            {
                var genre = await _genreRepository.GetByIdAsync(genreId);
                if (genre == null)
                {
                    throw new BadRequestException($"O gênero com o ID '{genreId}' não foi encontrado.");
                }
                movie.Genres.Add(genre);
            }

            await _movieRepository.UpdateAsync(movie);
            return _mapper.Map<MovieDetailResponse>(movie);
        }

        public async Task DeleteAsync(Guid id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);
            if (movie == null)
            {
                throw new NotFoundException("Filme não encontrado.");
            }

            await _movieRepository.DeleteAsync(movie);
        }

        public async Task<ReviewResponse> AddReviewAsync(Guid movieId, Guid userId, ReviewCreateRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("Usuário não encontrado.");
            }

            var movie = await _movieRepository.GetByIdWithDetailsAsync(movieId);
            if (movie == null)
            {
                throw new NotFoundException("Filme não encontrado.");
            }

            var existingReview = await _reviewRepository.GetByUserAndMovieAsync(userId, movieId);
            if (existingReview != null)
            {
                throw new ConflictException("Você já avaliou este filme anteriormente.");
            }

            var review = new Review
            {
                Id = Guid.NewGuid(),
                MovieId = movieId,
                UserId = userId,
                Rating = request.Rating,
                Comment = request.Comment,
                CreatedAt = DateTime.UtcNow,
                User = user,
                Movie = movie
            };

            // Add the review
            await _reviewRepository.AddAsync(review);

            // Fetch updated reviews to recalculate score
            var allReviews = await _reviewRepository.GetByMovieIdAsync(movieId);
            var reviewsList = allReviews.ToList();
            
            movie.ReviewsCount = reviewsList.Count;
            movie.AverageRating = Math.Round(reviewsList.Average(r => r.Rating), 2);
            movie.UpdatedAt = DateTime.UtcNow;

            await _movieRepository.UpdateAsync(movie);

            var response = _mapper.Map<ReviewResponse>(review);
            return response;
        }

        public async Task<ReviewResponse> UpdateReviewAsync(Guid movieId, Guid reviewId, Guid userId, ReviewUpdateRequest request)
        {
            var movie = await _movieRepository.GetByIdWithDetailsAsync(movieId);
            if (movie == null)
            {
                throw new NotFoundException("Filme não encontrado.");
            }

            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if (review == null || review.MovieId != movieId)
            {
                throw new NotFoundException("Avaliação não encontrada para este filme.");
            }

            if (review.UserId != userId)
            {
                throw new ForbiddenException("Você só pode editar suas próprias avaliações.");
            }

            review.Rating = request.Rating;
            review.Comment = request.Comment;
            review.UpdatedAt = DateTime.UtcNow;

            await _reviewRepository.UpdateAsync(review);

            // Recalculate movie score
            var allReviews = await _reviewRepository.GetByMovieIdAsync(movieId);
            var reviewsList = allReviews.ToList();

            movie.ReviewsCount = reviewsList.Count;
            movie.AverageRating = Math.Round(reviewsList.Average(r => r.Rating), 2);
            movie.UpdatedAt = DateTime.UtcNow;

            await _movieRepository.UpdateAsync(movie);

            return _mapper.Map<ReviewResponse>(review);
        }

        public async Task DeleteReviewAsync(Guid movieId, Guid reviewId, Guid userId)
        {
            var movie = await _movieRepository.GetByIdWithDetailsAsync(movieId);
            if (movie == null)
            {
                throw new NotFoundException("Filme não encontrado.");
            }

            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if (review == null || review.MovieId != movieId)
            {
                throw new NotFoundException("Avaliação não encontrada para este filme.");
            }

            if (review.UserId != userId)
            {
                throw new ForbiddenException("Você só pode excluir suas próprias avaliações.");
            }

            await _reviewRepository.DeleteAsync(review);

            // Recalculate movie score
            var allReviews = await _reviewRepository.GetByMovieIdAsync(movieId);
            var reviewsList = allReviews.ToList();

            movie.ReviewsCount = reviewsList.Count;
            movie.AverageRating = reviewsList.Count > 0
                ? Math.Round(reviewsList.Average(r => r.Rating), 2)
                : 0;
            movie.UpdatedAt = DateTime.UtcNow;

            await _movieRepository.UpdateAsync(movie);
        }

        public async Task<IEnumerable<ReviewResponse>> GetReviewsAsync(Guid movieId)
        {
            var movie = await _movieRepository.GetByIdAsync(movieId);
            if (movie == null)
            {
                throw new NotFoundException("Filme não encontrado.");
            }

            var reviews = await _reviewRepository.GetByMovieIdAsync(movieId);
            return _mapper.Map<IEnumerable<ReviewResponse>>(reviews);
        }
    }
}