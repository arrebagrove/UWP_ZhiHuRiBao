﻿using Brook.ZhiHuRiBao.Authorization;
using Brook.ZhiHuRiBao.Common;
using Brook.ZhiHuRiBao.Utils;
using Brook.ZhiHuRiBao.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace Brook.ZhiHuRiBao.Pages
{
    public sealed partial class CommentPage : Page
    {
        public CommentViewModel VM { get { return DataContext as CommentViewModel; } }

        public Visibility ToolBarVisibility { get { return Config.UIStatus == AppUIStatus.List ? Visibility.Visible : Visibility.Collapsed; } }

        private bool _isLoadComplete = false;

        public CommentPage()
        {
            this.InitializeComponent();

            CommentListView.Refresh = RefreshCommentList;
            CommentListView.LoadMore = LoadMoreComments;
            Loaded += CommentPage_Loaded;
        }

        private void CommentPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(ViewModelBase.CurrentStoryId))
            {
                CommentListView.SetRefresh(true);
            }
        }

        private async void RefreshCommentList()
        {
            await VM.RequestComments(false);
            CommentListView.SetRefresh(false);
        }

        private async void LoadMoreComments()
        {
            if (_isLoadComplete)
            {
                CommentListView.FinishLoadingMore();
                return;
            }

            var preCount = VM.CurrentCommentCount;
            await VM.RequestComments(true);
            CommentListView.FinishLoadingMore();
            _isLoadComplete = preCount == VM.CurrentCommentCount;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private async void SendComment()
        {
            if(!AuthorizationHelper.IsLogin)
            {
                PopupMessage.DisplayMessageInRes("NeedLogin");
                return;
            }

            if (string.IsNullOrEmpty(VM.CommentContent))
                return;

            await VM.SendComment();
            VM.CommentContent = "";
            CommentListView.SetRefresh(true);
        }
    }
}
