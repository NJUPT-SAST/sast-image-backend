﻿using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Response;
using SastImgAPI.Models;
using SastImgAPI.Models.DbSet;
using SastImgAPI.Models.RequestDtos;
using SastImgAPI.Models.Identity;
using System.Security.Claims;
using Swashbuckle.AspNetCore.Annotations;
using SastImgAPI.Models.ResponseDtos;

namespace SastImgAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlbumController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly DatabaseContext _dbContext;
        private readonly IValidator<AlbumRequestDto> _albumSetValidator;

        public AlbumController(
            UserManager<User> userManager,
            DatabaseContext dbContext,
            IValidator<AlbumRequestDto> albumSetValidator
        )
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _albumSetValidator = albumSetValidator;
        }

        /// <summary>
        /// Retrieves a list of albums associated with a user.
        /// </summary>
        /// <remarks>
        /// Allow users to retrieve a list of albums created by a specified user.
        /// If the user doesn't have any albums, a default album is created.
        /// The response includes album details such as ID, name, description, creation date, author information, and accessibility settings.
        /// Access to private albums is restricted to the album's owner.
        /// </remarks>
        /// <param name="username">The normalized username of the user whose albums are to be retrieved.</param>
        /// <param name="clt">A CancellationToken used for canceling the operation.</param>
        [HttpGet]
        [SwaggerResponse(
            StatusCodes.Status200OK,
            "Ok: Returns a list of albums associated with the specified user.",
            typeof(AlbumResponseDto[])
        )]
        [SwaggerResponse(
            StatusCodes.Status404NotFound,
            "Not Found: The specified user or album is not found.",
            typeof(ErrorResponseDto)
        )]
        public async Task<IActionResult> Albums([FromQuery] string username, CancellationToken clt)
        {
            // Find the user by their normalized username
            var user = await _userManager.Users
                .Include(user => user.Albums)
                .FirstOrDefaultAsync(user => user.NormalizedUserName == username.ToUpper(), clt);

            // Check if the user exists
            if (user is null)
                return ResponseDispatcher
                    .Error(StatusCodes.Status404NotFound, "Couldn't find the album.")
                    .Build();

            // Create a default album for the user if they have no albums
            if (!user.Albums.Any())
            {
                user.Albums.Add(new Album { Name = "Default", Author = user });
                await _dbContext.SaveChangesAsync(clt);
            }

            // Select and prepare album details for the response
            var albums = user.Albums.Select(
                album =>
                    new AlbumResponseDto(
                        album.Id,
                        album.Name,
                        album.Description,
                        album.CreatedAt,
                        album.Accessibility,
                        new(album.Author.Id, album.Author.UserName!, album.Author.Nickname)
                    )
            );

            // Restrict access to private albums if the requester is not the album's owner
            if (User.FindFirstValue("sub") != user.UserName)
                albums = albums.Where(album => album.Accessibility != Accessibility.OnlyMe);

            // Return the list of albums as a successful response
            return ResponseDispatcher.Data(albums);
        }

        /// <summary>
        /// Creates a new album for an authenticated user.
        /// </summary>
        /// <remarks>
        /// Allow authenticated users to create a new album with the specified details.
        /// The album's name, description, and accessibility settings are provided in the request body (AlbumDto).
        /// The response includes album details such as ID, name, description, creation date, and author information.
        /// </remarks>
        /// <param name="albumDto">An object containing album details.</param>
        /// <param name="clt">A CancellationToken used for canceling the operation.</param>
        [HttpPost]
        [Authorize(Roles = "User")]
        [SwaggerResponse(
            StatusCodes.Status200OK,
            "Ok: Returns the newly created album details.",
            typeof(AlbumResponseDto)
        )]
        [SwaggerResponse(
            StatusCodes.Status400BadRequest,
            "Bad Request: One or more parameters in your request are invalid.",
            typeof(ErrorResponseDto)
        )]
        public async Task<IActionResult> CreateAlbum(
            [FromBody] AlbumRequestDto albumDto,
            CancellationToken clt
        )
        {
            // Validate the provided album details
            var validationResult = await _albumSetValidator.ValidateAsync(albumDto);

            // Check for validation errors
            if (!validationResult.IsValid)
            {
                return ResponseDispatcher
                    .Error(
                        StatusCodes.Status400BadRequest,
                        "One or more parameters in your request are invalid."
                    )
                    .Add(validationResult.Errors)
                    .Build();
            }

            // Find the authenticated user
            var user = (await _userManager.FindByNameAsync(User.FindFirstValue("sub")!))!;

            // Create a new album with the provided details
            var album = new Album
            {
                Name = albumDto.Name,
                Description = albumDto.Description,
                Accessibility = albumDto.Accessibility,
                Author = user
            };

            // Add the new album to the database
            await _dbContext.Albums.AddAsync(album, clt);
            await _dbContext.SaveChangesAsync(clt);

            // Prepare the response with created album details
            var createdAlbum = new AlbumResponseDto(
                album.Id,
                album.Name,
                album.Description,
                album.CreatedAt,
                album.Accessibility,
                new(album.Author.Id, album.Author.UserName!, album.Author.Nickname)
            );

            // Return the created album details as a successful response
            return ResponseDispatcher.Data(createdAlbum);
        }

        /// <summary>
        /// Modifies the details of an existing album owned by an authenticated user.
        /// </summary>
        /// <remarks>
        /// Allow authenticated users to modify the details of an album they own.
        /// The album's ID is specified in the route parameter, and the new album details are provided in the request body (AlbumDto).
        /// The response is a 204 No Content status code if the update is successful.
        /// </remarks>
        /// <param name="id">The unique ID of the album to be modified.</param>
        /// <param name="albumDto">An object containing the updated album details.</param>
        /// <param name="clt">A CancellationToken used for canceling the operation.</param>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "User")]
        [SwaggerResponse(
            StatusCodes.Status204NoContent,
            "No Content: The album details have been successfully updated."
        )]
        [SwaggerResponse(
            StatusCodes.Status400BadRequest,
            "Bad Request: One or more parameters in your request are invalid.",
            typeof(ErrorResponseDto)
        )]
        [SwaggerResponse(
            StatusCodes.Status403Forbidden,
            "Forbidden: The authenticated user does not own the album.",
            typeof(ErrorResponseDto)
        )]
        [SwaggerResponse(
            StatusCodes.Status404NotFound,
            "Not Found: The specified album was not found.",
            typeof(ErrorResponseDto)
        )]
        public async Task<IActionResult> ChangeAlbum(
            int id,
            [FromBody] AlbumRequestDto albumDto,
            CancellationToken clt
        )
        {
            // Find the album by its unique ID
            var album = await _dbContext.Albums.FirstOrDefaultAsync(album => album.Id == id, clt);

            // Check if the album exists
            if (album is null)
            {
                return ResponseDispatcher
                    .Error(StatusCodes.Status404NotFound, "The specified album was not found.")
                    .Build();
            }

            // Check if the authenticated user owns the album
            if (album.AuthorId.ToString() != User.FindFirstValue("id"))
            {
                return Forbid();
            }

            // Validate the provided album details
            var validationResult = await _albumSetValidator.ValidateAsync(albumDto);

            // Check for validation errors
            if (!validationResult.IsValid)
            {
                return ResponseDispatcher
                    .Error(
                        StatusCodes.Status400BadRequest,
                        "Bad Request: One or more parameters in your request are invalid."
                    )
                    .Add(validationResult.Errors)
                    .Build();
            }

            // Update the album's details with the provided data
            album.Name = albumDto.Name;
            album.Description = albumDto.Description;
            album.Accessibility = albumDto.Accessibility;

            // Save the changes to the database
            await _dbContext.SaveChangesAsync(clt);

            // Return a 204 No Content response indicating success
            return NoContent();
        }

        /// <summary>
        /// Retrieves the details of a specific album based on its unique ID.
        /// </summary>
        /// <remarks>
        /// Allow users to retrieve the details of a specific album identified by its unique ID.
        /// The response includes album details such as ID, name, description, creation date, author information,
        /// images associated with the album, and accessibility settings.
        /// The user's authentication status is considered for accessing private (Auth) albums.
        /// </remarks>
        /// <param name="id">The unique ID of the album to retrieve.</param>
        /// <param name="clt">A CancellationToken used for canceling the operation.</param>
        [HttpGet("{id:int}")]
        [SwaggerResponse(
            StatusCodes.Status200OK,
            "Ok: Returns the album details if found.",
            typeof(AlbumDetailedResponseDto)
        )]
        [SwaggerResponse(
            StatusCodes.Status200OK,
            "Not Found: The specified album was not found.",
            typeof(ErrorResponseDto)
        )]
        public async Task<IActionResult> GetAlbum(int id, CancellationToken clt)
        {
            // Determine the user ID based on authentication status
            int userId = User.Identity!.IsAuthenticated
                ? int.Parse(User.FindFirstValue("id")!)
                : default;

            // Query the database to retrieve the album details
            var album = await _dbContext.Albums
                .Include(album => album.Author)
                .Include(album => album.Images)
                .Select(
                    album =>
                        new AlbumDetailedResponseDto(
                            album.Id,
                            album.Name,
                            album.Description,
                            album.CreatedAt,
                            album.Accessibility,
                            new(album.Author.Id, album.Author.UserName!, album.Author.Nickname),
                            album.Images
                                .Select(
                                    image =>
                                        new AlbumDetailedResponseDto.ImageDto(
                                            image.Id,
                                            image.Title,
                                            image.IsExifEnabled
                                        )
                                )
                                .ToList()
                        )
                )
                .Where(
                    album =>
                        album.Accessibility == Accessibility.Everyone
                        || album.Author.Id == userId
                        || (
                            User.Identity.IsAuthenticated
                            && album.Accessibility == Accessibility.Auth
                        )
                )
                .FirstOrDefaultAsync(album => album.Id == id);

            // Check if the album was found
            if (album is null)
            {
                return ResponseDispatcher
                    .Error(StatusCodes.Status404NotFound, "The specified album was not found.")
                    .Build();
            }
            else
            {
                // Return the album details in a successful response
                return ResponseDispatcher.Data(album);
            }
        }

        /// <summary>
        /// Deletes an album based on its unique ID, if the authenticated user is the owner.
        /// </summary>
        /// <remarks>
        /// Allow authenticated users to delete an album identified by its unique ID.
        /// The album can only be deleted if the authenticated user is the owner of the album.
        /// </remarks>
        /// <param name="id">The unique ID of the album to be deleted.</param>
        /// <param name="clt">A CancellationToken used for canceling the operation.</param>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "User")]
        [SwaggerResponse(
            StatusCodes.Status204NoContent,
            "No Content: The album has been successfully deleted."
        )]
        [SwaggerResponse(
            StatusCodes.Status403Forbidden,
            "Forbidden: The authenticated user doesn't have permission to delete the album.",
            typeof(ErrorResponseDto)
        )]
        [SwaggerResponse(
            StatusCodes.Status404NotFound,
            "Not Found: The specified album was not found.",
            typeof(ErrorResponseDto)
        )]
        public async Task<IActionResult> DeleteAlbum(int id, CancellationToken clt)
        {
            // Find the album by its unique ID
            var album = await _dbContext.Albums.FirstOrDefaultAsync(album => album.Id == id, clt);

            // Check if the album exists
            if (album is null)
            {
                return ResponseDispatcher
                    .Error(StatusCodes.Status404NotFound, "The specified album was not found.")
                    .Build();
            }

            // Check if the authenticated user is the owner of the album
            if (album.AuthorId.ToString() != User.FindFirstValue("id"))
            {
                return ResponseDispatcher
                    .Error(
                        StatusCodes.Status403Forbidden,
                        "You don't have the access to the album."
                    )
                    .Build();
            }

            // Remove the album from the database and save changes
            _dbContext.Remove(album);
            await _dbContext.SaveChangesAsync(clt);

            // Return a 204 No Content response indicating successful deletion
            return NoContent();
        }
    }
}
