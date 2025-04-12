using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SocialApp.Repository;
using SocialApp.Services;
using SocialApp.Components;
using SocialApp.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace SocialApp.Pages
{
    public sealed partial class GroupPage : Page
    {
        private const Visibility Collapsed = Visibility.Collapsed;
        private const Visibility Visible = Visibility.Visible;

        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IPostRepository _postRepository;
        private readonly IPostService _postService;
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupService _groupService;

        private long _groupId;
        private Entities.Group _group;
        private AppController _controller;

        public GroupPage()
        {
            InitializeComponent();
            InitializeServices();
            Loaded += OnPageLoaded;
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is long id)
            {
                _groupId = id;
            }

            _controller = App.Services.GetService<AppController>();
            InitializeTopBar();
        }

        private void InitializeTopBar()
        {
            TopBar.SetFrame(Frame);
            TopBar.SetNone();
        }

        private async void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            await LoadGroupDataAsync();
            InitializeUI();
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
            var groupPosts = _postService.GetPostsByGroupId(_groupId);

            foreach (var post in groupPosts)
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

            PostsFeed.Visibility = Visible;
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
            // Set visibility of admin-only controls here
            // Example: AdminPanel.Visibility = isAdmin ? Visible : Collapsed;
        }

        private bool IsCurrentUserAdmin()
        {
            return _controller.CurrentUser != null &&
                   _group.AdminId == _controller.CurrentUser.Id;
        }

        private void CreatePostInGroupButton_Click(object sender, RoutedEventArgs e)
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