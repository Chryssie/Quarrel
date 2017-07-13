﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Discord_UWP.CacheModels;
using Microsoft.Toolkit.Uwp.UI.Animations;
// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Discord_UWP.Controls
{
    public sealed partial class UserDetailsControl : UserControl
    {
        public Member DisplayedMember
        {
            get { return (Member)GetValue(DisplayedMemberProperty); }
            set { SetValue(DisplayedMemberProperty, value); }
        }
        public static readonly DependencyProperty DisplayedMemberProperty = DependencyProperty.Register(
            nameof(DisplayedMember),
            typeof(Member),
            typeof(UserDetailsControl),
            new PropertyMetadata(null, OnPropertyChangedStatic));
        private static void OnPropertyChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as UserDetailsControl;
            instance?.OnPropertyChanged(d, e.Property);
        }

        private async void OnPropertyChanged(DependencyObject d, DependencyProperty prop)
        {
            if (prop == DisplayedMemberProperty)
            {
                var user = DisplayedMember.Raw.User;
                if (DisplayedMember.Raw.Nick != null)
                {
                    UserStacker.Opacity = 0.5;
                    Nick.Text = DisplayedMember.Raw.Nick;
                } 
                else
                {
                    UserStacker.Opacity = 1;
                    Nick.Visibility = Visibility.Collapsed;
                }
                Username.Text = user.Username;
                Discriminator.Text = "#" + user.Discriminator;
                var imageURL = Common.AvatarUri(user.Avatar, user.Id);
                Avatar.ImageSource = new BitmapImage(imageURL);
                AvatarBlurred.Source = new BitmapImage(imageURL);
                BackgroundGrid.Blur(8, 0).Start();
                if (user.Avatar != null)
                {
                    var AvatarExtension = ".png";
                    if (user.Avatar.StartsWith("a_")) AvatarExtension = ".gif";
                    var image = new BitmapImage(new Uri("https://cdn.discordapp.com/avatars/" + user.Id + "/" + user.Avatar + AvatarExtension));
                    Avatar.ImageSource = image;
                    AvatarBlurred.Source = image;
                }
                else
                {
                    var image = new BitmapImage(new Uri("ms-appx:///Assets/DiscordIcon.png"));
                    Avatar.ImageSource = image;
                    AvatarBlurred.Source = image;
                }
                if (DisplayedMember.Raw.Roles.Count() == 0)
                {
                    RoleHeader.Visibility = Visibility.Collapsed;
                }
                else
                {
                    var roles = Storage.Cache.Guilds[App.CurrentGuildId].Roles;
                    foreach (var roleStr in DisplayedMember.Raw.Roles)
                    {
                        var role = roles[roleStr];
                        var c = Common.IntToColor(role.Color);
                        Border b = new Border()
                        {
                            CornerRadius = new CornerRadius(3, 3, 3, 3),
                            Background =new SolidColorBrush(Color.FromArgb(50, c.Color.R, c.Color.G, c.Color.B)),
                            BorderThickness=new Thickness(1),
                            BorderBrush=c,
                            Margin=new Thickness(2,2,2,2),
                            Child=new TextBlock()
                            {
                                FontSize=12,
                                Foreground = c,
                                Padding=new Thickness(4,2,4,4),
                                Text = role.Name
                            }
                        };
                        RoleWrapper.Children.Add(b);
                    }
                }
                //TODO: Note functionality and hook it up to the NoteChanged events
                //TODO: DM Functionality
                //TODO: Live status+playing indicator
                //TODO: 
            }
        }

        public UserDetailsControl()
        {
            this.InitializeComponent();
        }

        private void FadeIn_ImageOpened(object sender, RoutedEventArgs e)
        {
            (sender as Image).Fade(0.2f, 200).Start();
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            App.NavigateToProfile(DisplayedMember.Raw.User.Id);
        }
    }
}