using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using SocialApp.Services;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;

namespace SocialApp.Pages
{
    public sealed partial class LoginRegisterPage : Page
    {
        private const Visibility Collapsed = Visibility.Collapsed;
        private const Visibility Visible = Visibility.Visible;
        private AppController _controller;
        private string _image;
        private bool _isLoginFlow;

        public LoginRegisterPage()
        {
            InitializeComponent();
            InitializePage();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _controller = App.Services.GetService<AppController>();
        }

        private void InitializePage()
        {
            SetInitialVisibilities();
            SetInitialContent();
            SetInitialHandlers();
        }

        private void SetInitialVisibilities()
        {
            EmailTextbox.Visibility = Visible;
            UsernameTextbox.Visibility = Collapsed;
            PasswordTextbox.Visibility = Collapsed;
            ConfirmPasswordTextbox.Visibility = Collapsed;
            UploadedImage.Visibility = Collapsed;
            UploadImgButton.Visibility = Collapsed;
            RemoveImgButton.Visibility = Collapsed;
            CheckBox.Visibility = Collapsed;
            ContinueButton.Visibility = Visible;
            ErrorTextbox.Visibility = Collapsed;
        }

        private void SetInitialContent()
        {
            PageName.Text = "Login/Register";
            ContinueButton.Content = "Continue";
            ErrorTextbox.Text = string.Empty;
        }

        private void SetInitialHandlers()
        {
            ContinueButton.Click += ContinueClick;
        }

        private void ContinueClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EmailTextbox.Text))
            {
                ShowError("Please enter your email address");
                return;
            }

            if (!IsValidEmail(EmailTextbox.Text))
            {
                ShowError("Please enter a valid email address");
                return;
            }

            _isLoginFlow = _controller.EmailExists(EmailTextbox.Text);
            
            if (_isLoginFlow)
            {
                InitializeLoginFlow();
            }
            else
            {
                InitializeRegisterFlow();
            }
        }

        private void InitializeLoginFlow()
        {
            SetLoginVisibilities();
            SetLoginContent();
            SetLoginHandlers();
        }

        private void SetLoginVisibilities()
        {
            PasswordTextbox.Visibility = Visible;
            ErrorTextbox.Visibility = Collapsed;
        }

        private void SetLoginContent()
        {
            PageName.Text = "Login";
            ContinueButton.Content = "Login";
            ErrorTextbox.Text = string.Empty;
        }

        private void SetLoginHandlers()
        {
            ContinueButton.Click -= ContinueClick;
            ContinueButton.Click += LoginClick;
        }

        private void LoginClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PasswordTextbox.Password))
            {
                ShowError("Please enter your password");
                return;
            }

            try
            {
                if (!_controller.Login(EmailTextbox.Text, PasswordTextbox.Password))
                {
                    ShowError("Incorrect password");
                    PasswordTextbox.Password = string.Empty;
                }
                else
                {
                    Frame.Navigate(typeof(HomeScreen), _controller);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Login failed: {ex.Message}");
            }
        }

        private void InitializeRegisterFlow()
        {
            SetRegisterVisibilities();
            SetRegisterContent();
            SetRegisterHandlers();
        }

        private void SetRegisterVisibilities()
        {
            PasswordTextbox.Visibility = Visible;
            UsernameTextbox.Visibility = Visible;
            ConfirmPasswordTextbox.Visibility = Visible;
            UploadedImage.Visibility = Visible;
            UploadImgButton.Visibility = Visible;
            RemoveImgButton.Visibility = Visible;
            CheckBox.Visibility = Visible;
            ErrorTextbox.Visibility = Collapsed;
        }

        private void SetRegisterContent()
        {
            PageName.Text = "Register";
            ContinueButton.Content = "Register";
            ErrorTextbox.Text = string.Empty;
            UploadedImage.Child = new Image
            {
                Source = new BitmapImage(new Uri("ms-appx:///Assets/User.png"))
            };
            _image = string.Empty;
        }

        private void SetRegisterHandlers()
        {
            ContinueButton.Click -= ContinueClick;
            ContinueButton.Click += RegisterClick;
            UploadImgButton.Click += UploadImage;
            RemoveImgButton.Click += RemoveImage;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void ShowError(string message)
        {
            ErrorTextbox.Text = message;
            ErrorTextbox.Visibility = Visible;
        }

        private async void UploadImage(object sender, RoutedEventArgs e)
        {
            try
            {
                var picker = new FileOpenPicker
                {
                    ViewMode = PickerViewMode.Thumbnail,
                    SuggestedStartLocation = PickerLocationId.PicturesLibrary
                };
                picker.FileTypeFilter.Add(".jpg");
                picker.FileTypeFilter.Add(".jpeg");
                picker.FileTypeFilter.Add(".png");

                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.CurrentWindow);
                WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

                StorageFile file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    _image = await AppController.EncodeImageToBase64Async(file);
                    var bitmapImage = new BitmapImage();
                    using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
                    {
                        await bitmapImage.SetSourceAsync(stream);
                    }
                    UploadedImage.Child = new Image { Source = bitmapImage };
                }
            }
            catch (Exception ex)
            {
                ErrorTextbox.Text = $"Error uploading image: {ex.Message}";
            }
        }

        private void RemoveImage(object sender, RoutedEventArgs e)
        {
            _image = string.Empty;
            UploadedImage.Child = new Image
            {
                Source = new BitmapImage(new Uri("ms-appx:///Assets/User.png"))
            };
        }

        private void RegisterClick(object sender, RoutedEventArgs e)
        {
            try
            {
                PasswordsMatch(PasswordTextbox.Password, ConfirmPasswordTextbox.Password); // Use Password property
                AreTermAccepted();
                Register();
            }
            catch (Exception ex)
            {
                ErrorTextbox.Text = ex.Message;
            }
        }

        private void PasswordsMatch(string password, string confirmedPassword)
        {
            if (password != confirmedPassword)
            {
                throw new Exception("Passwords must match");
            }
        }

        private void AreTermAccepted()
        {
            if (CheckBox.IsChecked == null || CheckBox.IsChecked == false)
            {
                throw new Exception("You must accept the terms and conditions!");
            }
        }

        private void Register()
        {
            try
            {
                _controller.Register(UsernameTextbox.Text, EmailTextbox.Text, PasswordTextbox.Password, _image); // Use Password property
                Frame.Navigate(typeof(HomeScreen), _controller);
            }
            catch (Exception ex)
            {
                ErrorTextbox.Text = ex.Message;
            }
        }
    }
}