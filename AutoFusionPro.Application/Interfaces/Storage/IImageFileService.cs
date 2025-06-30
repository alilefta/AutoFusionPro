using System.IO;

namespace AutoFusionPro.Application.Interfaces.Storage
{
    /// <summary>
    /// Defines operations for managing image files within the application's storage.
    /// Implementations will handle the specifics of where and how files are stored (e.g., local, cloud).
    /// </summary>
    public interface IImageFileService
    {
        /// <summary>
        /// Saves an image from a source local path to the designated storage for part images.
        /// Optionally compresses JPEG images.
        /// </summary>
        /// <param name="sourceLocalPath">The full path to the source image file on the local system.</param>
        /// <param name="compressJpg">Whether to apply JPEG compression if the image is a JPG.</param>
        /// <returns>The absolute path to the saved image in the application's storage, or null if saving failed.</returns>
        /// <exception cref="ArgumentException">Thrown if sourceLocalPath is invalid or file doesn't exist.</exception>
        /// <exception cref="System.IO.IOException">Thrown for file system errors during copy/save.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown for image processing errors.</exception>
        Task<string?> SavePartImageAsync(string sourceLocalPath, bool compressJpg = true);

        /// <summary>
        /// Saves an image from a source local path to the designated storage for user avatars.
        /// Optionally compresses JPEG images.
        /// </summary>
        /// <param name="sourceLocalPath">The full path to the source image file on the local system.</param>
        /// <param name="compressJpg">Whether to apply JPEG compression if the image is a JPG.</param>
        /// <returns>The absolute path to the saved image in the application's storage, or null if saving failed.</returns>
        Task<string?> SaveUserAvatarAsync(string sourceLocalPath, bool compressJpg = true);

        /// <summary>
        /// Saves an image from a source local path to a specified subfolder within the application's image storage.
        /// Optionally compresses JPEG images.
        /// </summary>
        /// <param name="sourceLocalPath">The full path to the source image file on the local system.</param>
        /// <param name="targetSubfolderName">The name of the subfolder (e.g., "Categories", "Vehicles") within the base image storage.</param>
        /// <param name="compressJpg">Whether to apply JPEG compression if the image is a JPG.</param>
        /// <returns>The absolute path to the saved image, or null if saving failed.</returns>
        Task<string?> SaveImageAsync(string sourceLocalPath, string targetSubfolderName, bool compressJpg = true);


        /// <summary>
        /// Saves an image from a stream to a specified subfolder within the application's image storage.
        /// The originalFileName is used to determine the file extension.
        /// Optionally compresses JPEG images.
        /// </summary>
        /// <param name="imageStream">The stream containing the image data.</param>
        /// <param name="originalFileName">The original name of the file, used to extract the extension.</param>
        /// <param name="targetSubfolderName">The name of the subfolder (e.g., "Parts", "Categories") within the base image storage.</param>
        /// <param name="compressJpg">Whether to apply JPEG compression if the image is a JPG.</param>
        /// <returns>The absolute path to the saved image, or null if saving failed or stream was invalid.</returns>
        /// <exception cref="ArgumentNullException">Thrown if imageStream or originalFileName is null.</exception>
        /// <exception cref="ArgumentException">Thrown if originalFileName is empty or has no extension.</exception>
        /// <exception cref="System.IO.IOException">Thrown for file system errors during save.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown for image processing errors.</exception>
        Task<string?> SaveImageAsync(Stream imageStream, string originalFileName, string targetSubfolderName, bool compressJpg = true);

        /// <summary>
        /// Deletes an image file from the application's storage given its absolute path.
        /// Performs safety checks to ensure the path is within the application's designated storage area.
        /// </summary>
        /// <param name="absolutePathToImage">The full absolute path to the image file to be deleted.</param>
        /// <returns>True if deletion was successful or file didn't exist; false if deletion failed for other reasons (e.g. access denied and path was valid).</returns>
        Task<bool> DeleteImageAsync(string? absolutePathToImage);
    }
}
