// Controllers/ParkingImageController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpotParkAPI.Models.Dtos;
using SpotParkAPI.Services;
using SpotParkAPI.Services.Interfaces;

[Route("api/parking/{parkingLotId}/images")]
[ApiController]
[Authorize]
public class ParkingImageController : ControllerBase
{
    private readonly ParkingImageService _imageService;
    private readonly ICommonService _commonService;

    public ParkingImageController(ParkingImageService imageService, ICommonService commonService)
    {
        _imageService = imageService;
        _commonService = commonService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadImages(int parkingLotId, [FromForm] List<IFormFile> files)
    {
        // Early validation - stop processing if no files
        if (files == null || files.Count == 0)
        {
            return BadRequest(new
            {
                Success = false,
                Message = "Please select at least one image file to upload"
            });
        }

        // Validate parking lot exists
        await _commonService.GetParkingLotByIdAsync(parkingLotId);
        var userId = _commonService.GetCurrentUserId();

        // Validate file count limit
        var existingCount = await _imageService.GetImageCountForUserAsync(userId, parkingLotId);
        var remainingSlots = 3 - existingCount;

        if (remainingSlots <= 0)
        {
            return BadRequest(new
            {
                Success = false,
                Message = "Maximum 3 images allowed per parking lot",
                CurrentImageCount = existingCount
            });
        }

        // Process valid files only
        var uploadedImages = new List<ParkingLotImageDto>();
        var errorMessages = new List<string>();
        var filesProcessed = 0;

        foreach (var file in files)
        {
            // Stop if we've reached the limit
            if (filesProcessed >= remainingSlots) break;

            try
            {
                // Validate file
                var validationResult = ValidateImageFile(file);
                if (!validationResult.IsValid)
                {
                    errorMessages.Add(validationResult.ErrorMessage);
                    continue;
                }

                // Upload valid file
                var imageDto = await _imageService.UploadImageAsync(parkingLotId, file);
                uploadedImages.Add(imageDto);
                filesProcessed++;
            }
            catch (Exception ex)
            {
                errorMessages.Add($"Failed to process {file.FileName}: {ex.Message}");
            }
        }

        // Return combined result
        return Ok(new
        {
            Success = true,
            Message = $"Processed {uploadedImages.Count} files successfully",
            UploadedImages = uploadedImages,
            Errors = errorMessages,
            RemainingSlots = 3 - existingCount - uploadedImages.Count
        });
    }

    private (bool IsValid, string ErrorMessage) ValidateImageFile(IFormFile file)
    {
        if (file.Length == 0)
            return (false, $"{file.FileName}: Empty file");

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var fileExtension = Path.GetExtension(file.FileName).ToLower();

        if (!allowedExtensions.Contains(fileExtension))
            return (false, $"{file.FileName}: Invalid file type ({fileExtension})");

        if (file.Length > 5 * 1024 * 1024) // 5MB
            return (false, $"{file.FileName}: Exceeds 5MB size limit");

        return (true, null);
    }


}