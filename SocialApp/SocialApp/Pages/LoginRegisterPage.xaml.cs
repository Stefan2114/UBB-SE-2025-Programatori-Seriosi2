// <copyright file="LoginRegisterPage.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SocialApp.Pages
{
    using System;
    using System.Threading.Tasks;
    using global::Windows.Storage;
    using global::Windows.Storage.Pickers;
    using global::Windows.Storage.Streams;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Media.Imaging;
    using Microsoft.UI.Xaml.Navigation;
    using SocialApp.Services;

    /// <summary>
    /// Represents the login and registration page of the application.
    /// </summary>
    public sealed partial class LoginRegisterPage : Page
    {
        private const Visibility Collapsed = Visibility.Collapsed;
        private const Visibility Visible = Visibility.Visible;
        private AppController controller;
        private string image;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginRegisterPage"/> class.
        /// </summary>
        public LoginRegisterPage()
        {
            this.InitializeComponent();
            this.controller = App.Services.GetService<AppController>() ?? throw new InvalidOperationException("AppController service not found.");
            this.image = string.Empty;
            this.InitialFlow();
        }

        /// <summary>
        /// Handles the click event of the continue button.
        /// </summary>
        /// <param name="sender">
        /// the object that raised the event.
        /// </param>
        /// <param name="e">
        /// the event data.
        /// </param>
        public void ContinueClick(object sender, RoutedEventArgs e)
        {
            if (this.controller.EmailExists(this.EmailTextbox.Text))
            {
                this.LoginFlow();
            }
            else
            {
                this.RegisterFlow();
            }
        }

        /// <summary>
        /// Handles the navigation to this page.
        /// </summary>
        /// <param name="e">
        /// the event data that contains the navigation parameters.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// thrown when the AppController service is not found.
        /// </exception>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.controller = App.Services.GetService<AppController>() ?? throw new InvalidOperationException("AppController service not found.");
        }

        private void InitialFlow()
        {
            this.SetInitialVisibilities();
            this.SetInitialContent();
            this.SetInitialHandlers();
        }

        private void SetInitialVisibilities()
        {
            this.EmailTextbox.Visibility = Visible;
            this.UsernameTextbox.Visibility = Collapsed;
            this.PasswordTextbox.Visibility = Collapsed;
            this.ConfirmPasswordTextbox.Visibility = Collapsed;
            this.UploadedImage.Visibility = Collapsed;
            this.UploadImgButton.Visibility = Collapsed;
            this.RemoveImgButton.Visibility = Collapsed;
            this.CheckBox.Visibility = Collapsed;
            this.ContinueButton.Visibility = Visible;
        }

        private void SetInitialContent()
        {
            this.PageName.Text = "Login/Register";
            this.ContinueButton.Content = "Continue";
        }

        private void SetInitialHandlers()
        {
            this.ContinueButton.Click += this.ContinueClick;
        }

        private void LoginFlow()
        {
            this.SetLoginVisibilities();
            this.SetLoginContent();
            this.SetLoginHandlers();
        }

        private void SetLoginVisibilities()
        {
            this.PasswordTextbox.Visibility = Visible;
        }

        private void SetLoginContent()
        {
            this.PageName.Text = "Login";
            this.ContinueButton.Content = "Login";
            this.ErrorTextbox.Text = string.Empty;
        }

        private void SetLoginHandlers()
        {
            this.ContinueButton.Click -= this.ContinueClick;
            this.ContinueButton.Click += this.LoginClick;
        }

        private void LoginClick(object sender, RoutedEventArgs e)
        {
            if (!this.controller.Login(this.EmailTextbox.Text, this.PasswordTextbox.Password)) // Use Password property
            {
                this.ErrorTextbox.Visibility = Visible;
                this.ErrorTextbox.Text = "Incorrect password.";
                this.PasswordTextbox.Password = string.Empty;
            }
            else
            {
                this.Frame.Navigate(typeof(HomeScreen), this.controller);
            }
        }

        private void RegisterFlow()
        {
            this.SetRegisterVisibilities();
            this.SetRegisterContent();
            this.SetRegisterHandlers();
        }

        private void SetRegisterVisibilities()
        {
            this.PasswordTextbox.Visibility = Visible;
            this.UsernameTextbox.Visibility = Visible;
            this.ConfirmPasswordTextbox.Visibility = Visible;
            this.UploadedImage.Visibility = Visible;
            this.UploadImgButton.Visibility = Visible;
            this.RemoveImgButton.Visibility = Visible;
            this.CheckBox.Visibility = Visible;
        }

        private void SetRegisterContent()
        {
            this.PageName.Text = "Register";
            this.ContinueButton.Content = "Register";
            this.ErrorTextbox.Text = string.Empty;
            this.UploadedImage.Child = new Image
            {
                Source = new BitmapImage(new Uri("ms-appx:///Assets/User.png")),
            };
            this.image = string.Empty;
        }

        private void SetRegisterHandlers()
        {
            this.ContinueButton.Click -= this.ContinueClick;
            this.ContinueButton.Click += this.RegisterClick;
        }

        private async void UploadImage(object sender, RoutedEventArgs e)
        {
            try
            {
                var picker = new FileOpenPicker
                {
                    ViewMode = PickerViewMode.Thumbnail,
                    SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                };
                picker.FileTypeFilter.Add(".jpg");
                picker.FileTypeFilter.Add(".jpeg");
                picker.FileTypeFilter.Add(".png");

                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.CurrentWindow);
                WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

                StorageFile file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    this.image = await AppController.EncodeImageToBase64Async(file);
                    var bitmapImage = new BitmapImage();
                    using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
                    {
                        await bitmapImage.SetSourceAsync(stream);
                    }

                    this.UploadedImage.Child = new Image { Source = bitmapImage };
                }
            }
            catch (Exception ex)
            {
                this.ErrorTextbox.Text = $"Error uploading image: {ex.Message}";
            }
        }

        private void RemoveImage(object sender, RoutedEventArgs e)
        {
            this.image = string.Empty;
            this.UploadedImage.Child = new Image
            {
                Source = new BitmapImage(new Uri("ms-appx:///Assets/User.png")),
            };
        }

        private void RegisterClick(object sender, RoutedEventArgs e)
        {
            try
            {
                this.PasswordsMatch(this.PasswordTextbox.Password, this.ConfirmPasswordTextbox.Password); // Use Password property
                this.AreTermAccepted();
                this.Register();
            }
            catch (Exception ex)
            {
                this.ErrorTextbox.Text = ex.Message;
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
            if (this.CheckBox.IsChecked == null || this.CheckBox.IsChecked == false)
            {
                throw new Exception("You must accept the terms and conditions!");
            }
        }

        private void Register()
        {
            try
            {
                this.controller.Register(this.UsernameTextbox.Text, this.EmailTextbox.Text, this.PasswordTextbox.Password, this.image); // Use Password property
                this.Frame.Navigate(typeof(HomeScreen), this.controller);
            }
            catch (Exception ex)
            {
                this.ErrorTextbox.Text = ex.Message;
            }
        }
    }
}