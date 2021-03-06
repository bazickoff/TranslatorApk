﻿using System;
using System.Windows;
using System.Windows.Input;
using TranslatorApk.Logic.ViewModels.Windows;

namespace TranslatorApk.Windows
{
    public partial class AboutProgramWindow
    {
        public AboutProgramWindow()
        {
            InitializeComponent();

            ViewModel = new AboutProgramWindowViewModel();
        }

        internal AboutProgramWindowViewModel ViewModel
        {
            get => DataContext as AboutProgramWindowViewModel;
            private set => DataContext = value;
        }

        private async void AboutProgramWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadItems();
        }

        private void AboutProgramWindow_OnClosed(object sender, EventArgs e)
        {
            ViewModel.Dispose();
        }

        private void WebMoney_LeftMouseDown(object sender, MouseButtonEventArgs e)
        {
            ViewModel.WebMoneyClickedCommand.Execute(e);
        }
    }
}
