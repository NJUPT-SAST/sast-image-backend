﻿namespace SastImgAPI.Models.RequestDtos
{
    public record ImageRequestDto(
        IFormFile ImageFile,
        int AlbumId,
        string Title,
        string[] Tags,
        string Description = "",
        bool IsExifEnabled = false,
        int CategoryId = 1
    );
}