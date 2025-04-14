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
        private const Visibility CollapsedVisibility = Visibility.Collapsed;
        private const Visibility VisibleVisibility = Visibility.Visible;
        private AppController _appController;
        private string _profileImage;

        public LoginRegisterPage()
        {
            InitializeComponent();
            InitializePageFlow();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _appController = App.Services.GetService<AppController>();
        }

        private void InitializePageFlow()
        {
            SetInitialVisibilityStates();
            SetInitialContentValues();
            SetInitialEventHandlers();
        }

        private void SetInitialVisibilityStates()
        {
            EmailTextbox.Visibility = VisibleVisibility;
            UsernameTextbox.Visibility = CollapsedVisibility;
            PasswordTextbox.Visibility = CollapsedVisibility;
            ConfirmPasswordTextbox.Visibility = CollapsedVisibility;
            UploadedImage.Visibility = CollapsedVisibility;
            UploadImgButton.Visibility = CollapsedVisibility;
            RemoveImgButton.Visibility = CollapsedVisibility;
            CheckBox.Visibility = CollapsedVisibility;
            ContinueButton.Visibility = VisibleVisibility;
        }

        private void SetInitialContentValues()
        {
            PageName.Text = "Login/Register";
            ContinueButton.Content = "Continue";
        }

        private void SetInitialEventHandlers()
        {
            ContinueButton.Click += OnContinueClicked;
        }

        public void OnContinueClicked(object sender, RoutedEventArgs e)
        {
            if (_appController.EmailExists(EmailTextbox.Text))
            {
                InitializeLoginFlow();
            }
            else
            {
                InitializeRegistrationFlow();
            }
        }

        private void InitializeLoginFlow()
        {
            SetLoginVisibilityStates();
            SetLoginContentValues();
            SetLoginEventHandlers();
        }

        private void SetLoginVisibilityStates()
        {
            PasswordTextbox.Visibility = VisibleVisibility;
        }

        private void SetLoginContentValues()
        {
            PageName.Text = "Login";
            ContinueButton.Content = "Login";
            ErrorTextbox.Text = "";
        }

        private void SetLoginEventHandlers()
        {
            ContinueButton.Click -= OnContinueClicked;
            ContinueButton.Click += OnLoginClicked;
        }

        private void OnLoginClicked(object sender, RoutedEventArgs e)
        {
            if (!_appController.Login(EmailTextbox.Text, PasswordTextbox.Password))
            {
                ErrorTextbox.Visibility = VisibleVisibility;
                ErrorTextbox.Text = "Incorrect password.";
                PasswordTextbox.Password = "";
            }
            else
            {
                Frame.Navigate(typeof(HomeScreen), _appController);
            }
        }

        private void InitializeRegistrationFlow()
        {
            SetRegistrationVisibilityStates();
            SetRegistrationContentValues();
            SetRegistrationEventHandlers();
        }

        private void SetRegistrationVisibilityStates()
        {
            PasswordTextbox.Visibility = VisibleVisibility;
            UsernameTextbox.Visibility = VisibleVisibility;
            ConfirmPasswordTextbox.Visibility = VisibleVisibility;
            UploadedImage.Visibility = VisibleVisibility;
            UploadImgButton.Visibility = VisibleVisibility;
            RemoveImgButton.Visibility = VisibleVisibility;
            CheckBox.Visibility = VisibleVisibility;
        }

        private void SetRegistrationContentValues()
        {
            PageName.Text = "Register";
            ContinueButton.Content = "Register";
            ErrorTextbox.Text = "";
            UploadedImage.Child = new Image
            {
                Source = new BitmapImage(new Uri("ms-appx:///Assets/User.png"))
            };
            _profileImage = string.Empty;
        }

        private void SetRegistrationEventHandlers()
        {
            ContinueButton.Click -= OnContinueClicked;
            ContinueButton.Click += OnRegisterClicked;
        }

        private async void OnImageUpload(object sender, RoutedEventArgs e)
        {
            try
            {
                var filePicker = new FileOpenPicker
                {
                    ViewMode = PickerViewMode.Thumbnail,
                    SuggestedStartLocation = PickerLocationId.PicturesLibrary
                };
                filePicker.FileTypeFilter.Add(".jpg");
                filePicker.FileTypeFilter.Add(".jpeg");
                filePicker.FileTypeFilter.Add(".png");

                var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(App.CurrentWindow);
                WinRT.Interop.InitializeWithWindow.Initialize(filePicker, windowHandle);

                StorageFile selectedFile = await filePicker.PickSingleFileAsync();
                if (selectedFile != null)
                {
                    _profileImage = await AppController.EncodeImageToBase64Async(selectedFile);
                    var bitmapImage = new BitmapImage();
                    using (IRandomAccessStream fileStream = await selectedFile.OpenAsync(FileAccessMode.Read))
                    {
                        await bitmapImage.SetSourceAsync(fileStream);
                    }
                    UploadedImage.Child = new Image { Source = bitmapImage };
                }
            }
            catch (Exception ex)
            {
                ErrorTextbox.Text = $"Error uploading image: {ex.Message}";
            }
        }

        private void OnImageRemoval(object sender, RoutedEventArgs e)
        {
            _profileImage = string.Empty;
            UploadedImage.Child = new Image
            {
                Source = new BitmapImage(new Uri("ms-appx:///Assets/User.png"))
            };
        }

        private void OnRegisterClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                ValidatePasswordMatch(PasswordTextbox.Password, ConfirmPasswordTextbox.Password);
                ValidateTermsAcceptance();
                CompleteRegistration();
            }
            catch (Exception ex)
            {
                ErrorTextbox.Text = ex.Message;
            }
        }

        private static void ValidatePasswordMatch(string password, string confirmedPassword)
        {
            if (password != confirmedPassword)
            {
                throw new Exception("Passwords must match");
            }
        }

        private void ValidateTermsAcceptance()
        {
            if (CheckBox.IsChecked == null || CheckBox.IsChecked == false)
            {
                throw new Exception("You must accept the terms and conditions!");
            }
        }

        private void CompleteRegistration()
        {
            try
            {
                _appController.Register(UsernameTextbox.Text, EmailTextbox.Text, PasswordTextbox.Password, _profileImage);
                Frame.Navigate(typeof(HomeScreen), _appController);
            }
            catch (Exception ex)
            {
                ErrorTextbox.Text = ex.Message;
            }
        }
    }
}