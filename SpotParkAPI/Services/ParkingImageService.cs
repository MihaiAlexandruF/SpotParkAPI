using AutoMapper;
using SpotParkAPI.Models.Dtos;
using SpotParkAPI.Models.Entities;
using SpotParkAPI.Repositories.Interfaces;
using SpotParkAPI.Services.Interfaces;

namespace SpotParkAPI.Services
{
    public class ParkingImageService
    {
        private readonly IParkingImageRepository _imageRepo;
        private readonly IWebHostEnvironment _hostingEnv;
        private readonly IMapper _mapper;
        private readonly ICommonService _commonService;

        public ParkingImageService(
            IParkingImageRepository imageRepo,
            IWebHostEnvironment hostingEnv,
            IMapper mapper,
            ICommonService commonService)
        {
            _imageRepo = imageRepo;
            _hostingEnv = hostingEnv;
            _mapper = mapper;
            _commonService = commonService;
        }

        public async Task<ParkingLotImageDto> UploadImageAsync(int parkingLotId, IFormFile file)
        {
            // Extrage User ID din token
            var userId = _commonService.GetCurrentUserId();

            // Verifică limita de 3 imagini
            var imageCount = await _imageRepo.GetImageCountForUserAsync(userId, parkingLotId);
            if (imageCount >= 3) throw new Exception("Maximum 3 images per parking lot.");

            // Validează fișierul
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
                throw new Exception("Invalid file type. Allowed: JPG, JPEG, PNG.");

            // Creează folderul
            var uploadsDir = Path.Combine(_hostingEnv.WebRootPath, "uploads", $"user-{userId}", $"parkinglot-{parkingLotId}");
            Directory.CreateDirectory(uploadsDir);

            // Generează nume unic
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsDir, fileName);

            // Salvează pe disk
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Salvează în baza de date
            var image = new ParkingLotImage
            {
                ParkingLotId = parkingLotId,
                UserId = userId,
                ImagePath = Path.Combine("uploads", $"user-{userId}", $"parkinglot-{parkingLotId}", fileName),
                UploadedAt = DateTime.UtcNow
            };

            await _imageRepo.AddImageAsync(image);

            // Generează URL
            var imageDto = _mapper.Map<ParkingLotImageDto>(image);
            imageDto.ImageUrl = $"{GetBaseUrl()}/{image.ImagePath.Replace("\\", "/")}";

            return imageDto;
        }

        public async Task<List<ParkingLotImageDto>> GetImagesForParkingLotAsync(int parkingLotId)
        {
            var images = await _imageRepo.GetImagesForParkingLotAsync(parkingLotId);
            return _mapper.Map<List<ParkingLotImageDto>>(images);
        }

        public async Task<int> GetImageCountForUserAsync(int userId, int parkingLotId)
        {
            return await _imageRepo.GetImageCountForUserAsync(userId, parkingLotId);
        }


        private string GetBaseUrl() => "https://localhost:5001"; // Schimbă în producție
    }
}
