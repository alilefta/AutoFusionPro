using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace AutoFusionPro.UI.Helpers
{
    public static class MessageBoxHelper
    {


        public static async Task ShowMessageWithTitleAsync(string title, string subtitle, string content, bool isError = false, FlowDirection flowDirection = FlowDirection.RightToLeft)
        {
            var titleForeground = (SolidColorBrush)System.Windows.Application.Current.Resources["Dialog.Title.ForegroundBrush"];
            var textForeground = (SolidColorBrush)System.Windows.Application.Current.Resources["Dialog.Subtitle.ForegroundBrush"];
            var background = (SolidColorBrush)System.Windows.Application.Current.Resources["Dialog.BackgroundBrush"];
            var fontFamily = (FontFamily)System.Windows.Application.Current.Resources["DynamicFontFamilyRegular"];

            var messageBox = new Wpf.Ui.Controls.MessageBox
            {
                ShowTitle = true,
                Title = title,
                Foreground = titleForeground,
                Background = background,
                FlowDirection = flowDirection,

                Content = new Border
                {
                    Padding = new Thickness(10, 10, 10, 20),
                    Child = new StackPanel
                    {
                        Children =
                        {
                            new TextBlock
                            {
                                Text = subtitle,
                                FontWeight = FontWeights.Bold,
                                FontSize = 18,
                                Margin = new Thickness(0, 10, 0, 20),
                                TextWrapping = TextWrapping.Wrap,

                            },
                            new RichTextBox
                            {
                                FontSize = 14,
                                Padding= new Thickness(10, 10, 10, 10),
                                IsReadOnly = true,
                                BorderThickness = new Thickness(0),
                                Background = background,
                                Foreground = textForeground,
                                Document = new FlowDocument
                                {
                                    FontFamily = fontFamily,
                                    Blocks =
                                    {
                                        new Paragraph(new Run(content))
                                    }
                                }
                            }
                        },
                    }
                },
                CloseButtonAppearance = isError ? Wpf.Ui.Controls.ControlAppearance.Danger : Wpf.Ui.Controls.ControlAppearance.Success,
                CloseButtonText = "Close",
                Owner = System.Windows.Application.Current.MainWindow

            };

            await messageBox.ShowDialogAsync();
        }

        // Method for showing a MessageBox without a title
        public static async Task ShowMessageWithoutTitleAsync(string content, bool isError = false, FlowDirection flowDirection = FlowDirection.RightToLeft)
        {
            var titleForeground = (SolidColorBrush)System.Windows.Application.Current.Resources["Dialog.Title.ForegroundBrush"];
            var textForeground = (SolidColorBrush)System.Windows.Application.Current.Resources["Dialog.Subtitle.ForegroundBrush"];
            var background = (SolidColorBrush)System.Windows.Application.Current.Resources["Dialog.BackgroundBrush"];

            var fontFamily = (FontFamily)System.Windows.Application.Current.Resources["DynamicFontFamilyRegular"] ?? new FontFamily("Arial");

            var messageBox = new Wpf.Ui.Controls.MessageBox
            {
                ShowTitle = false,
                Foreground = titleForeground,
                Background = background,
                FlowDirection = flowDirection,
                Content = new Border
                {
                    Padding = new Thickness(10),
                    Child = new RichTextBox
                    {
                        FontSize = 14,
                        Padding = new Thickness(10, 10, 10, 10),
                        IsReadOnly = true,
                        BorderThickness = new Thickness(0),
                        Background = background,
                        Foreground = textForeground,
                        Document = new FlowDocument
                        {
                            FontFamily = fontFamily,
                            Blocks =
                                    {
                                        new Paragraph(new Run(content))
                                    }
                        }
                    }
                },
                CloseButtonAppearance = isError ? Wpf.Ui.Controls.ControlAppearance.Danger : Wpf.Ui.Controls.ControlAppearance.Primary,
                CloseButtonText = "Close",
                Owner = System.Windows.Application.Current.MainWindow.IsLoaded ? System.Windows.Application.Current.MainWindow : null



            };

            await messageBox.ShowDialogAsync();
        }

        public static async Task ShowLisenceMessage()
        {
            var titleForeground = (SolidColorBrush)System.Windows.Application.Current.Resources["Dialog.Title.ForegroundBrush"];
            var textForeground = (SolidColorBrush)System.Windows.Application.Current.Resources["Dialog.Subtitle.ForegroundBrush"];
            var background = (SolidColorBrush)System.Windows.Application.Current.Resources["Dialog.BackgroundBrush"];
            var fontFamily = (FontFamily)System.Windows.Application.Current.Resources["DynamicFontFamilyRegular"] ?? new FontFamily("Arial");


            var message = """
                                      Software License Agreement

                This Software License Agreement ("Agreement") is made between:

                Licensor: Ali Lefta (Developer)
                Licensee: Oscar Dental Lab
                1. Grant of License
                The Licensor grants the Licensee a non-exclusive, non-transferable license to install and use the Dental Lab Management System (the "Software") for internal business operations, subject to the terms and conditions in this Agreement.

                2. Scope of License
                License Type: [Single User/Multiple User/Site License]
                Permitted Use: The Licensee may use the Software on the agreed number of machines at one location.
                License Key: The Software will require a valid license key to activate and function.
                3. License Restrictions
                The Licensee shall not:

                Modify, distribute, or create derivative works based on the Software.
                Reverse engineer, decompile, or disassemble the Software.
                Rent, lease, or sub-license the Software to a third party.
                4. Payment
                The Licensee agrees to pay a one-time fee of [amount] or an annual fee of [amount] as specified in the purchase agreement. Failure to pay on time may result in termination of the license.

                5. Ownership
                The Software is the property of the Licensor and is protected by copyright laws and international intellectual property treaties. The Licensee acquires only the right to use the Software as described in this Agreement.

                6. Support and Maintenance
                Support: The Licensor agrees to provide technical support for the duration of the license period, subject to the limitations stated in the support agreement.
                Updates: The Licensee will receive updates to the Software if and when they are made available by the Licensor.
                7. Termination
                This license is effective until terminated. The Licensor may terminate the license if the Licensee fails to comply with the terms of this Agreement. Upon termination, the Licensee must cease all use of the Software and destroy all copies.

                8. Limitation of Liability
                In no event shall the Licensor be liable for any damages resulting from the use or inability to use the Software, including but not limited to loss of data or profits.

                9. Confidentiality
                The Licensee agrees to maintain the confidentiality of the Software and any documentation provided and shall not disclose any proprietary information to third parties.

                10. Governing Law
                This Agreement shall be governed by and construed in accordance with the laws of [Jurisdiction].

                By using the Software, you agree to the terms of this License Agreement.
                """;

            var messageBox = new Wpf.Ui.Controls.MessageBox
            {
                ShowTitle = false,
                Foreground = titleForeground,
                Background = background,
                FlowDirection = FlowDirection.LeftToRight,
                Content = new ScrollViewer()
                {
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    Content = new Border
                    {
                        Padding = new Thickness(20, 10, 20, 10),
                        Child = new RichTextBox
                        {
                            FontSize = 14,
                            Padding = new Thickness(0, 10, 0, 20),
                            IsReadOnly = true,
                            BorderThickness = new Thickness(0),
                            Background = background,
                            Foreground = textForeground,
                            Document = new FlowDocument
                            {
                                FontFamily = fontFamily,
                                Blocks =
                                    {
                                        new Paragraph(new Run(message))
                                    }
                            }
                        }
                    },
                },
                CloseButtonAppearance = Wpf.Ui.Controls.ControlAppearance.Info,
                CloseButtonText = "Close",
                Owner = System.Windows.Application.Current.MainWindow



            };

            await messageBox.ShowDialogAsync();
        }

        public static async Task<bool> ShowConfirmationAsync(
                    string title,
                    string subtitle,
                    string content,
                    string confirmButtonText = "Yes",
                    string cancelButtonText = "No",
                    Wpf.Ui.Controls.ControlAppearance confirmAppearance = Wpf.Ui.Controls.ControlAppearance.Primary,
                    FlowDirection flowDirection = FlowDirection.RightToLeft)
        {
            var titleForeground = (SolidColorBrush)System.Windows.Application.Current.Resources["Dialog.Title.ForegroundBrush"];
            var textForeground = (SolidColorBrush)System.Windows.Application.Current.Resources["Dialog.Subtitle.ForegroundBrush"];
            var background = (SolidColorBrush)System.Windows.Application.Current.Resources["Dialog.BackgroundBrush"];
            var fontFamily = (FontFamily)System.Windows.Application.Current.Resources["DynamicFontFamilyRegular"] ?? new FontFamily("Arial");

            var messageBox = new Wpf.Ui.Controls.MessageBox
            {
                ShowTitle = true,
                Title = title,
                Foreground = titleForeground,
                Background = background,
                FlowDirection = flowDirection,
                Content = new Border
                {
                    Padding = new Thickness(10, 10, 10, 20),
                    Child = new StackPanel
                    {
                        Children =
                        {
                            new TextBlock
                            {
                                Text = subtitle,
                                FontWeight = FontWeights.Bold,
                                FontSize = 18,
                                Margin = new Thickness(0, 10, 0, 20),
                                TextWrapping = TextWrapping.Wrap,
                            },
                             new RichTextBox
                            {
                                FontSize = 14,
                                Padding = new Thickness(0, 10, 0, 20),
                                IsReadOnly = true,
                                BorderThickness = new Thickness(0),
                                Background = background,
                                Foreground = textForeground,
                                Document = new FlowDocument
                                {
                                    FontFamily = fontFamily,
                                    Blocks =
                                            {
                                                new Paragraph(new Run(content))
                                            }
                                }
                            }
                        }
                    }
                },
                PrimaryButtonText = confirmButtonText,
                SecondaryButtonText = cancelButtonText,
                PrimaryButtonAppearance = confirmAppearance,
                CloseButtonText = cancelButtonText,  // This hides the close button
                CloseButtonAppearance = Wpf.Ui.Controls.ControlAppearance.Transparent,
                IsSecondaryButtonEnabled = false,
                IsPrimaryButtonEnabled = true,
                Owner = System.Windows.Application.Current.MainWindow

            };

            var result = await messageBox.ShowDialogAsync();
            return result == Wpf.Ui.Controls.MessageBoxResult.Primary;
        }

        public static async Task<bool> ShowConfirmMessageWithTitleAsync(
                    string title,
                    string subtitle,
                    string content,
                    bool isError = false,  // Default parameter for error handling
                    string confirmButtonText = "Yes",
                    string cancelButtonText = "No",
                    Wpf.Ui.Controls.ControlAppearance confirmAppearance = Wpf.Ui.Controls.ControlAppearance.Primary,
                    FlowDirection flowDirection = FlowDirection.RightToLeft)
                        {
            var titleForeground = (SolidColorBrush)System.Windows.Application.Current.Resources["Dialog.Title.ForegroundBrush"];
            var textForeground = (SolidColorBrush)System.Windows.Application.Current.Resources["Dialog.Subtitle.ForegroundBrush"];
            var background = (SolidColorBrush)System.Windows.Application.Current.Resources["Dialog.BackgroundBrush"];
            var fontFamily = (FontFamily)System.Windows.Application.Current.Resources["DynamicFontFamilyRegular"] ?? new FontFamily("Arial");

                            var messageBox = new Wpf.Ui.Controls.MessageBox
                            {
                                ShowTitle = true,
                                Title = title,
                                Foreground = titleForeground,
                                Background = background,
                                FlowDirection = flowDirection,
                                Content = new Border
                                {
                                    Padding = new Thickness(10, 10, 10, 20),
                                    Child = new StackPanel
                                    {
                                        Children =
                                {
                                    new TextBlock
                                    {
                                        Text = subtitle,
                                        FontWeight = FontWeights.Bold,
                                        FontSize = 18,
                                        Margin = new Thickness(0, 10, 0, 20),
                                        TextWrapping = TextWrapping.Wrap,
                                    },
                                    new RichTextBox
                                    {
                                        FontSize = 14,
                                        Padding = new Thickness(0, 10, 0, 20),
                                        IsReadOnly = true,
                                        BorderThickness = new Thickness(0),
                                        Background = background,
                                        Foreground = textForeground,
                                        Document = new FlowDocument
                                        {
                                            FontFamily = fontFamily,
                                            Blocks =
                                            {
                                                new Paragraph(new Run(content))
                                            }
                                        }
                                    }
                                }
                                    }
                                },
                                PrimaryButtonText = confirmButtonText,
                                SecondaryButtonText = cancelButtonText,
                                PrimaryButtonAppearance = confirmAppearance,
                                CloseButtonText = cancelButtonText,  // This hides the close button
                                CloseButtonAppearance = isError ? Wpf.Ui.Controls.ControlAppearance.Danger : Wpf.Ui.Controls.ControlAppearance.Transparent,
                                IsSecondaryButtonEnabled = false,
                                IsPrimaryButtonEnabled = true,
                                Owner = System.Windows.Application.Current.MainWindow
                            };

                            var result = await messageBox.ShowDialogAsync();
                            return result == Wpf.Ui.Controls.MessageBoxResult.Primary;
                        }



        public static async Task ShowMessageWithTitleAndOwnerAsync(string title, string subtitle, string content, bool isError = false, FlowDirection flowDirection = FlowDirection.RightToLeft, Window owner = null)
        {
            var titleForeground = (SolidColorBrush)System.Windows.Application.Current.Resources["Dialog.Title.ForegroundBrush"];
            var textForeground = (SolidColorBrush)System.Windows.Application.Current.Resources["Dialog.Subtitle.ForegroundBrush"];
            var background = (SolidColorBrush)System.Windows.Application.Current.Resources["Dialog.BackgroundBrush"];
            var fontFamily = (FontFamily)System.Windows.Application.Current.Resources["DynamicFontFamilyRegular"];

            var messageBox = new Wpf.Ui.Controls.MessageBox
            {
                ShowTitle = true,
                Title = title,
                Foreground = titleForeground,
                Background = background,
                FlowDirection = flowDirection,

                Content = new Border
                {
                    Padding = new Thickness(10, 10, 10, 20),
                    Child = new StackPanel
                    {
                        Children =
                        {
                            new TextBlock
                            {
                                Text = subtitle,
                                FontWeight = FontWeights.Bold,
                                FontSize = 18,
                                Margin = new Thickness(0, 10, 0, 20),
                                TextWrapping = TextWrapping.Wrap,

                            },
                            new RichTextBox
                            {
                                FontSize = 14,
                                Padding= new Thickness(0, 10, 0, 20),
                                IsReadOnly = true,
                                BorderThickness = new Thickness(0),
                                Background = background,
                                Foreground = textForeground,
                                Document = new FlowDocument
                                {
                                    FontFamily = fontFamily,
                                    Blocks =
                                    {
                                        new Paragraph(new Run(content))
                                    }
                                }
                            }
                        },
                    }
                },
                CloseButtonAppearance = isError ? Wpf.Ui.Controls.ControlAppearance.Danger : Wpf.Ui.Controls.ControlAppearance.Success,
                CloseButtonText = "Close",
               

                Owner = owner ?? System.Windows.Application.Current.MainWindow,

            };

            await messageBox.ShowDialogAsync();
        }


    }


}

