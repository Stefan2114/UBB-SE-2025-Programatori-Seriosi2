using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SocialApp.Components;
using SocialApp.Entities;
using SocialApp.Repository;
using SocialApp.Services;

namespace SocialApp.Pages
{
    public sealed partial class GroupPage : Page
    {
        private const Visibility CollapsedVisibility = Visibility.Collapsed;
        private const Visibility VisibleVisibility = Visibility.Visible;

        private UserRepository _userRepository;
        private UserService _userService;
        private PostRepository _postRepository;
        private PostService _postService;
        private GroupRepository _groupRepository;
        private GroupService _groupService;

        private AppController _appController;
        private long _groupId;
        private Entities.Group _group;

        public GroupPage()
        {
            InitializeComponent();
            InitializePageFlow();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is long id)
            {
                _groupId = id;
            }

            _appController = App.Services.GetService<AppController>();
        }

        private void InitializePageFlow()
        {
            InitializeServices();
            SetInitialVisibilityStates();
            SetInitialContentValues();
            SetInitialEventHandlers();
        }

        private void InitializeServices()
        {
            _userRepository = new UserRepository();
            _userService = new UserService(_userRepository);

            _groupRepository = new GroupRepository();
            _groupService = new GroupService(_groupRepository, _userRepository);

            _postRepository = new PostRepository();
            _postService = new PostService(_postRepository, _userRepository, _groupRepository);
        }

        private void SetInitialVisibilityStates()
        {
            PostsFeed.Visibility = CollapsedVisibility;
            // AdminPanel.Visibility = CollapsedVisibility; // if applicable
        }

        private void SetInitialContentValues()
        {
            GroupTitle.Text = "Loading...";
            GroupDescription.Text = string.Empty;
        }

        private void SetInitialEventHandlers()
        {
            Loaded += OnPageLoaded;
            CreatePostInGroupButton.Click += OnCreatePostClicked;
        }

        private async void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            InitializeTopBar();
            await LoadGroupDataAsync();
            InitializeUI();
        }

        private void InitializeTopBar()
        {
            TopBar.SetFrame(Frame);
            TopBar.SetNone();
        }

        private async Task LoadGroupDataAsync()
        {
            _group = _groupService.GetById(_groupId);

            if (!string.IsNullOrEmpty(_group.Image))
            {
                GroupImage.Source = await AppController.DecodeBase64ToImageAsync(_group.Image);
            }
        }

        private void InitializeUI()
        {
            SetGroupInfo();
            PopulateFeed();
            PopulateMembers();
            SetAdminControlsVisibility();
        }

        private void SetGroupInfo()
        {
            GroupTitle.Text = _group.Name;
            GroupDescription.Text = _group.Description;
        }

        private void PopulateFeed()
        {
            PostsFeed.ClearPosts();
            var posts = _postService.GetPostsByGroupId(_groupId);

            foreach (var post in posts)
            {
                PostsFeed.AddPost(new PostComponent(
                    post.Title,
                    post.Visibility,
                    post.UserId,
                    post.Content,
                    post.CreatedDate,
                    post.Tag,
                    post.Id));
            }

            PostsFeed.Visibility = VisibleVisibility;
            PostsFeed.DisplayCurrentPage();
        }

        private void PopulateMembers()
        {
            MembersList.Children.Clear();
            var isAdmin = IsCurrentUserAdmin();
            var members = _groupService.GetUsersFromGroup(_groupId);

            foreach (var member in members)
            {
                MembersList.Children.Add(new Member(member, Frame, _groupId, isAdmin));
            }
        }

        private void SetAdminControlsVisibility()
        {
            var isAdmin = IsCurrentUserAdmin();
            // AdminPanel.Visibility = isAdmin ? VisibleVisibility : CollapsedVisibility;
        }

        private bool IsCurrentUserAdmin()
        {
            return _appController.CurrentUser != null &&
                   _group.AdminId == _appController.CurrentUser.Id;
        }

        private void OnCreatePostClicked(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CreatePost), new PostNavigationParams
            {
                GroupId = _groupId
            });
        }
    }

    public class PostNavigationParams
    {
        public long GroupId { get; set; }
    }
}
