using AutoFusionPro.UI.Resources.Converters.Base;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Media.Imaging;

namespace AutoFusionPro.UI.Resources.Converters
{
     public class StringToImageSourceConverter : BaseValueConverter<StringToImageSourceConverter>
    {

        // Define the path to your placeholder image (ensure it's a Resource in your project)
        private const string PlaceholderImagePath = "pack://application:,,,/AutoFusionPro.UI;component/Assets/Images/placeholder_image.png";
        private static BitmapImage? _placeholderImage;

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string imagePath && !string.IsNullOrWhiteSpace(imagePath))
            {
                try
                {
                    Uri imageUri;
                    // Check if it's an absolute URI (like http, https, or pack)
                    if (Uri.TryCreate(imagePath, UriKind.Absolute, out imageUri))
                    {
                        BitmapImage bi = new BitmapImage();
                        bi.BeginInit();
                        bi.UriSource = imageUri;
                        // Optional: CacheOption can be useful for web images or if images change
                        // bi.CacheOption = BitmapCacheOption.OnLoad;
                        bi.CreateOptions = BitmapCreateOptions.IgnoreImageCache; // Often good for development to see changes
                        bi.EndInit();
                        bi.Freeze(); // Freeze for performance and cross-thread access
                        return bi;
                    }
                    // Check if it's a relative local file path that exists
                    // (This case is less common if you standardize on pack URIs or web URLs)
                    else if (File.Exists(imagePath))
                    {
                        BitmapImage bi = new BitmapImage();
                        bi.BeginInit();
                        bi.UriSource = new Uri(Path.GetFullPath(imagePath), UriKind.Absolute); // Ensure absolute path
                        bi.EndInit();
                        bi.Freeze();
                        return bi;
                    }
                    else
                    {
                        // _logger?.LogWarning("Image path is not a valid absolute URI and local file not found: {ImagePath}", imagePath);
                        Debug.WriteLine($"WARNING: Image path is not a valid absolute URI and local file not found: {imagePath}");
                    }
                }
                catch (Exception ex)
                {
                    // _logger?.LogError(ex, "Error loading image from path: {ImagePath}", imagePath);
                    Debug.WriteLine($"ERROR: Error loading image from path '{imagePath}'. Exception: {ex.Message}");
                    // Fall through to return placeholder
                }
            }
            // If value is null, empty, whitespace, or loading failed, return placeholder
            return GetPlaceholderImage();

        }



        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static BitmapImage GetPlaceholderImage()
        {
            if (_placeholderImage == null)
            {
                try
                {
                    _placeholderImage = new BitmapImage(new Uri(PlaceholderImagePath, UriKind.Absolute));
                }
                catch (Exception ex)
                {
                    // Log this critical error - placeholder itself failed to load
                    // This usually means the path is wrong or the image isn't set as Resource
                    Debug.WriteLine($"CRITICAL ERROR: Placeholder image failed to load from '{PlaceholderImagePath}'. Exception: {ex.Message}");
                    // Fallback to a null or a programmatically created minimal visual if necessary
                    return null!; // Or handle more gracefully
                }
            }
            return _placeholderImage;
        }
    }
}
