﻿using System.Windows;
using BeatSpy.Helpers;
using System.Windows.Input;
using System.Windows.Controls;
using BeatSpy.DataTypes.Constants;
using System.Windows.Media.Animation;
using System;

namespace BeatSpy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// This method is fired when ever the user mouse clicks with the window border
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowBorderMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        /// <summary>
        /// This method is fired when ever the user mouse clicks on the context button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnContextMenuClick(object sender, RoutedEventArgs e)
        {
            if(sender is Button contextButton)
            {
                contextButton.ContextMenu.IsOpen = true;
            }
        }

        /// <summary>
        /// This method is fired when ever the user loads the context menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnContextMenuLoaded(object sender, RoutedEventArgs e)
        {
            if(sender is ContextMenu contextMenu)
            {
                contextMenu.DataContext = DataContext;
            }
        }

        /// <summary>
        /// This method is fired when the user clicks BeatSpy logo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBeatSpyClicked(object sender, RoutedEventArgs e)
        {
            BrowsUtil.OpenUrl(DefaultConstants.LINK_GITHUB_REPO);
        }
    }
}
