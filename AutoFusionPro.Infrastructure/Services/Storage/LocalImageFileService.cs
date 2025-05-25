using AutoFusionPro.Application.Interfaces.Settings;
using AutoFusionPro.Application.Interfaces.Storage;
using AutoFusionPro.Core.Exceptions.Service;
using Microsoft.Extensions.Logging;
using System.Drawing.Imaging;
using System.IO;

namespace AutoFusionPro.Infrastructure.Services.Storage
{
    public class LocalImageFileService : IImageFileService
    {
        private readonly string _baseStoragePath;
        private readonly string _partsSubfolder = "Parts";
        private readonly string _userAvatarsSubfolder = "UserAvatars";
        // Add other target subfolder constants as needed

        private readonly ILogger<LocalImageFileService> _logger;
        private readonly IGlobalSettingsService _globalSettingsService;
        private const long DefaultJpegQuality = 80L; // Default JPEG compression quality

        // Define your application's specific folder name within LocalApplicationData
        private string ApplicationDataFolderName = "AutoFusionPro";
        private const string ImagesRootSubfolderName = "Images";

        public LocalImageFileService(ILogger<LocalImageFileService> logger, IGlobalSettingsService globalSettingsService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _globalSettingsService = globalSettingsService ?? throw new ArgumentNullException(nameof(globalSettingsService));

            ApplicationDataFolderName = _globalSettingsService.SystemName ?? "AutoFusionPro";

            try
            {
                string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                _baseStoragePath = Path.Combine(localAppDataPath, ApplicationDataFolderName, ImagesRootSubfolderName);

                // Ensure base and common subdirectories exist
                Directory.CreateDirectory(_baseStoragePath);
                Directory.CreateDirectory(Path.Combine(_baseStoragePath, _partsSubfolder));
                Directory.CreateDirectory(Path.Combine(_baseStoragePath, _userAvatarsSubfolder));

                _logger.LogInformation("LocalImageFileService initialized. Base image storage path: {BasePath}", _baseStoragePath);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "CRITICAL FAILURE: Could not initialize image storage directories at LocalApplicationData/{AppName}/{ImagesRoot}. Image operations will fail.", ApplicationDataFolderName, ImagesRootSubfolderName);
                // This is a critical setup error. Depending on application requirements,
                // you might re-throw to halt startup or allow the app to run in a degraded state.
                throw new InvalidOperationException($"Failed to initialize image storage directories. See logs for details. Base Path Target: {_baseStoragePath}", ex);
            }
        }

        public async Task<string?> SavePartImageAsync(string sourceLocalPath, bool compressJpg = true)
        {
            return await SaveImageAsync(sourceLocalPath, _partsSubfolder, compressJpg);
        }

        public async Task<string?> SaveUserAvatarAsync(string sourceLocalPath, bool compressJpg = true)
        {
            return await SaveImageAsync(sourceLocalPath, _userAvatarsSubfolder, compressJpg);
        }

        public async Task<string?> SaveImageAsync(string sourceLocalPath, string targetSubfolderName, bool compressJpg = true)
        {
            if (string.IsNullOrWhiteSpace(sourceLocalPath))
                throw new ArgumentException("Source image path cannot be null or whitespace.", nameof(sourceLocalPath));
            if (!File.Exists(sourceLocalPath))
                throw new ArgumentException($"Source image file does not exist: {sourceLocalPath}", nameof(sourceLocalPath));
            if (string.IsNullOrWhiteSpace(targetSubfolderName))
                throw new ArgumentException("Target subfolder name cannot be null or whitespace.", nameof(targetSubfolderName));

            string targetDirectory = Path.Combine(_baseStoragePath, targetSubfolderName);

            try
            {
                // Ensure the specific target subfolder exists (it might be a new one)
                Directory.CreateDirectory(targetDirectory);

                string fileExtension = Path.GetExtension(sourceLocalPath)?.ToLowerInvariant() ?? ".tmp";
                string uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                string destinationPath = Path.Combine(targetDirectory, uniqueFileName);

                _logger.LogDebug("Attempting to save image from {SourcePath} to {DestinationPath}", sourceLocalPath, destinationPath);

                // Perform file operations on a background thread to avoid blocking UI if called directly
                await Task.Run(() =>
                {
                    using (var image = System.Drawing.Image.FromFile(sourceLocalPath))
                    {
                        if (compressJpg && (fileExtension == ".jpg" || fileExtension == ".jpeg"))
                        {
                            SaveWithJpegCompression(image, destinationPath);
                        }
                        else if (fileExtension == ".png")
                        {
                            image.Save(destinationPath, ImageFormat.Png);
                        }
                        else
                        {
                            // For other types, try to save in original format, or just copy the file
                            // File.Copy is safer if System.Drawing can't handle the format well for saving.
                            // However, using image.Save() allows any internal conversion GDI+ might do.
                            try { image.Save(destinationPath); }
                            catch
                            {
                                // Fallback to direct copy if save fails for unknown format
                                File.Copy(sourceLocalPath, destinationPath, true);
                            }
                        }
                    }
                });

                _logger.LogInformation("Image successfully saved: {DestinationPath}", destinationPath);
                return destinationPath; // Return the full absolute path
            }
            catch (UnauthorizedAccessException uaEx)
            {
                _logger.LogError(uaEx, "Unauthorized access while saving image from {SourcePath} to {DestinationPath}", sourceLocalPath, targetDirectory);
                throw new ServiceException($"Permission denied while saving image to '{targetDirectory}'.", uaEx);
            }
            catch (IOException ioEx)
            {
                _logger.LogError(ioEx, "IO error while saving image from {SourcePath} to {DestinationPath}", sourceLocalPath, targetDirectory);
                throw new ServiceException($"A file system error occurred while saving the image.", ioEx);
            }
            catch (Exception ex) // Catch-all for other errors (e.g., System.Drawing issues)
            {
                _logger.LogError(ex, "Failed to process and save image from {SourcePath} to {TargetDirectory}", sourceLocalPath, targetDirectory);
                throw new InvalidOperationException("An error occurred while processing the image.", ex);
            }
        }


        public async Task<bool> DeleteImageAsync(string? absolutePathToImage)
        {
            if (string.IsNullOrWhiteSpace(absolutePathToImage))
            {
                _logger.LogDebug("DeleteImageAsync called with null or empty path. No action taken.");
                return true; // No file to delete, so conceptually successful or no-op.
            }

            // Security check: Ensure we are only deleting files within our designated storage area.
            if (!absolutePathToImage.StartsWith(_baseStoragePath, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Attempted to delete image outside the application's storage area: {ImagePath}", absolutePathToImage);
                // Depending on policy, you might throw an exception here or just return false.
                // For now, let's treat it as a failed delete attempt to prevent unintended actions.
                return false;
            }

            try
            {
                if (File.Exists(absolutePathToImage))
                {
                    // Perform file operations on a background thread
                    await Task.Run(() => File.Delete(absolutePathToImage));
                    _logger.LogInformation("Image file deleted successfully: {ImagePath}", absolutePathToImage);
                    return true;
                }
                else
                {
                    _logger.LogWarning("Attempted to delete image file that does not exist: {ImagePath}", absolutePathToImage);
                    return true; // File doesn't exist, so deletion is effectively complete.
                }
            }
            catch (IOException ioEx)
            {
                _logger.LogWarning(ioEx, "IO error (e.g., file locked) while deleting image: {ImagePath}", absolutePathToImage);
                return false;
            }
            catch (UnauthorizedAccessException uaEx)
            {
                _logger.LogError(uaEx, "Unauthorized access while attempting to delete image: {ImagePath}", absolutePathToImage);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting image: {ImagePath}", absolutePathToImage);
                return false;
            }
        }

        // Helper method for JPEG compression
        private void SaveWithJpegCompression(System.Drawing.Image image, string filePath, long quality = DefaultJpegQuality)
        {
            ImageCodecInfo? jpgEncoder = GetEncoder(ImageFormat.Jpeg);
            if (jpgEncoder == null)
            {
                _logger.LogWarning("JPEG encoder not found. Saving image {FilePath} with default JPEG settings.", filePath);
                image.Save(filePath, ImageFormat.Jpeg); // Fallback
                return;
            }

            System.Drawing.Imaging.Encoder qualityEncoder = System.Drawing.Imaging.Encoder.Quality;
            var encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(qualityEncoder, quality);
            image.Save(filePath, jpgEncoder, encoderParameters);
        }

        // Static private helper to get image encoders
        private static ImageCodecInfo? GetEncoder(ImageFormat format)
        {
            return ImageCodecInfo.GetImageDecoders().FirstOrDefault(codec => codec.FormatID == format.Guid);
        }
    }

    // Example Usage

    //public class PartService : IPartService
    //{
    //    private readonly IUnitOfWork _unitOfWork;
    //    private readonly IImageFileService _imageFileService;
    //    private readonly ILogger<PartService> _logger;

    //    public PartService(IUnitOfWork unitOfWork, IImageFileService imageFileService, ILogger<PartService> logger)
    //    {
    //        _unitOfWork = unitOfWork;
    //        _imageFileService = imageFileService;
    //        _logger = logger;
    //    }

    //    public async Task UpdatePartImageAsync(int partId, string sourceLocalPath)
    //    {
    //        var part = await _unitOfWork.Parts.GetByIdAsync(partId);
    //        if (part == null)
    //            throw new ServiceException($"Part with ID {partId} not found.");

    //        string? oldImagePath = part.ImagePath;
    //        string? newImagePath = null;

    //        try
    //        {
    //            newImagePath = await _imageFileService.SavePartImageAsync(sourceLocalPath);
    //            if (string.IsNullOrEmpty(newImagePath))
    //            {
    //                // This case might not be hit if SavePartImageAsync always throws on failure
    //                throw new ServiceException("Failed to save the new image.");
    //            }

    //            part.ImagePath = newImagePath;
    //            await _unitOfWork.SaveChangesAsync();
    //            _logger.LogInformation("Successfully updated image path for Part ID {PartId} to {NewPath}", partId, newImagePath);

    //            // If successful and old image existed and is different, delete old one
    //            if (!string.IsNullOrWhiteSpace(oldImagePath) && oldImagePath != newImagePath)
    //            {
    //                await _imageFileService.DeleteImageAsync(oldImagePath);
    //            }
    //        }
    //        catch (Exception ex) // Catch exceptions from _imageFileService or SaveChangesAsync
    //        {
    //            // If new image was saved but DB update failed, attempt to delete the newly saved orphaned image
    //            if (!string.IsNullOrEmpty(newImagePath) && (part.ImagePath != newImagePath || ex is DbUpdateException))
    //            {
    //                _logger.LogWarning(ex, "DB update failed after saving new image. Attempting to clean up orphaned image: {OrphanedImagePath}", newImagePath);
    //                await _imageFileService.DeleteImageAsync(newImagePath);
    //            }
    //            _logger.LogError(ex, "Error in UpdatePartImageAsync for Part ID {PartId}", partId);
    //            throw; // Re-throw or wrap in ServiceException
    //        }
    //    }

    //    public async Task DeletePartAsync(int partId) // Example incorporating image deletion
    //    {
    //        var partToDelete = await _unitOfWork.Parts.GetByIdAsync(partId);
    //        if (partToDelete == null)
    //        {
    //            _logger.LogWarning("Part with ID {PartId} not found for deletion.", partId);
    //            return; // Or throw
    //        }

    //        string? imagePathToDelete = partToDelete.ImagePath;

    //        _unitOfWork.Parts.Delete(partToDelete); // Assuming hard delete for this example
    //        await _unitOfWork.SaveChangesAsync();
    //        _logger.LogInformation("Part with ID {PartId} deleted from database.", partId);

    //        if (!string.IsNullOrWhiteSpace(imagePathToDelete))
    //        {
    //            await _imageFileService.DeleteImageAsync(imagePathToDelete);
    //        }
    //    }



            //    // Inside IPartService implementation - DeletePartAsync(int partId)
            //// ... after fetching the partToDelete ...
            //string? imagePathToDelete = partToDelete.ImagePath;

            //        // ... proceed to delete part from database via _unitOfWork.Parts.Delete ...
            //        await _unitOfWork.SaveChangesAsync();

            //// After successful DB deletion, delete the image file
            //if (!string.IsNullOrWhiteSpace(imagePathToDelete))
            //{
            //    _imageFileService.DeleteImage(imagePathToDelete);
}



    //}
