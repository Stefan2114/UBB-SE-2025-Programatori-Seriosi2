namespace SocialApp.Pages
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Navigation;
    using SocialApp.Components;
    using SocialApp.Entities;
    using SocialApp.Repository;
    using SocialApp.Services;

    public sealed partial class UserPage : Page
    {
        private AppController controller;
        private UserRepository userRepository;
        private UserService userService;
        private PostRepository postRepository;
        private PostService postService;
        private GroupRepository groupRepository;
        private User displayedUser; // User to display (may differ from CurrentUser)

        public UserPage()
        {
            this.InitializeComponent();

            this.userRepository = new UserRepository();
            this.userService = new UserService(this.userRepository);
            this.postRepository = new PostRepository();
            this.groupRepository = new GroupRepository();
            this.postService = new PostService(this.postRepository, this.userRepository, this.groupRepository);

            // Use null-coalescing operator to handle potential null reference
            this.controller = App.Services.GetService<AppController>() ?? throw new System.InvalidOperationException("AppController service is not registered.");

            this.Loaded += this.SetContent;
            this.Loaded += this.PostsClick;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is UserPageNavigationArgs args)
            {
                this.displayedUser = args.SelectedUser;
                TopBar.SetFrame(this.Frame);
            }
            else
            {
                this.displayedUser = controller.CurrentUser; // Default to CurrentUser if no specific user provided
                TopBar.SetFrame(this.Frame);
            }
        }

        private async void SetContent(object sender, RoutedEventArgs e)
        {
            if (displayedUser != null)
            {
                if (!string.IsNullOrEmpty(displayedUser.Image))
                    ProfileImage.Source = await AppController.DecodeBase64ToImageAsync(displayedUser.Image);
                Username.Text = displayedUser.Username;

                if (controller.CurrentUser != null && controller.CurrentUser.Id == displayedUser.Id)
                {
                    FollowLogOutButton.Content = "Logout";
                    FollowLogOutButton.Click -= Logout;
                    FollowLogOutButton.Click += Logout;
                }
                else
                {
                    FollowLogOutButton.Content = IsFollowed() ? "Unfollow" : "Follow";
                    FollowLogOutButton.Click -= FollowUnfollow;
                    FollowLogOutButton.Click += FollowUnfollow;
                }
                SetPostsContent(sender, e);
            }
            else
            {
                FollowLogOutButton.Content = "Follow";
            }
        }

        private bool IsFollowed()
        {
            // Check if CurrentUser follows displayedUser
            return controller?.CurrentUser != null && userService.GetUserFollowing(controller.CurrentUser.Id).Any(u => u.Id == displayedUser.Id);
        }

        private void FollowUnfollow(object sender, RoutedEventArgs e)
        {
            if (controller?.CurrentUser != null && displayedUser != null)
            {
                if (IsFollowed())
                {
                    userService.UnfollowUser(controller.CurrentUser.Id, displayedUser.Id);
                    FollowLogOutButton.Content = "Follow";
                }
                else
                {
                    userService.FollowUser(controller.CurrentUser.Id, displayedUser.Id);
                    FollowLogOutButton.Content = "Unfollow";
                }
            }
        }

        private void Logout(object sender, RoutedEventArgs e)
        {
            controller.Logout();
            Frame.Navigate(typeof(HomeScreen), controller);
        }

        private void PostsClick(object sender, RoutedEventArgs e)
        {
            SetPostsContent(sender, e);
            PostsFeed.DisplayCurrentPage();
        }

        private void SetPostsContent(object sender, RoutedEventArgs e)
        {
            PostsButton.IsEnabled = false;
            WorkoutsButton.IsEnabled = true;
            MealsButton.IsEnabled = true;
            FollowersButton.IsEnabled = true;

            PostsFeed.Visibility = Visibility.Visible;
            FollowersScrollViewer.Visibility = Visibility.Collapsed;

            PopulateFeed();
        }

        private void PopulateFeed()
        {
            PostsFeed.ClearPosts();

            if (displayedUser != null)
            {
                List<Post> userPosts = postService.GePostsByUserId(displayedUser.Id);
                foreach (Post post in userPosts)
                {
                    PostsFeed.AddPost(new PostComponent(post.Title, post.Visibility, post.UserId, post.Content, post.CreatedDate, post.Tag, post.Id));
                }
                PostsFeed.DisplayCurrentPage();
            }
        }

        private void WorkoutsClick(object sender, RoutedEventArgs e)
        {
            SetWorkoutsContent();
        }

        private void SetWorkoutsContent()
        {
            PostsButton.IsEnabled = true;
            WorkoutsButton.IsEnabled = false;
            MealsButton.IsEnabled = true;
            FollowersButton.IsEnabled = true;

            PostsFeed.Visibility = Visibility.Collapsed;
            FollowersScrollViewer.Visibility = Visibility.Collapsed;
        }

        private void MealsClick(object sender, RoutedEventArgs e)
        {
            SetMealsContent();
        }

        private void SetMealsContent()
        {
            PostsButton.IsEnabled = true;
            WorkoutsButton.IsEnabled = true;
            MealsButton.IsEnabled = false;
            FollowersButton.IsEnabled = true;

            PostsFeed.Visibility = Visibility.Collapsed;
            FollowersScrollViewer.Visibility = Visibility.Collapsed;
        }

        private void FollowersClick(object sender, RoutedEventArgs e)
        {
            SetFollowersContent();
        }

        private void SetFollowersContent()
        {
            PostsButton.IsEnabled = true;
            WorkoutsButton.IsEnabled = true;
            MealsButton.IsEnabled = true;
            FollowersButton.IsEnabled = false;

            PostsFeed.Visibility = Visibility.Collapsed;
            FollowersScrollViewer.Visibility = Visibility.Visible;

            PopulateFollowers();
        }

        private void PopulateFollowers()
        {
            FollowersStack.Children.Clear();

            if (displayedUser != null)
            {
                List<User> followers = userService.GetUserFollowers(displayedUser.Id);
                foreach (User user in followers)
                {
                    FollowersStack.Children.Add(new Follower(user.Username, userService.GetUserFollowing(controller.CurrentUser?.Id ?? -1).Contains(user), user, this.Frame));
                }
            }
        }
    }
}