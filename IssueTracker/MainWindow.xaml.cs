﻿using FirstFloor.ModernUI.Presentation;
using FirstFloor.ModernUI.Windows.Controls;
using IssueTracker.Properties;

namespace IssueTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ModernWindow_Closed(object sender, System.EventArgs e)
        {
            Settings.Default.ChosenTheme = AppearanceManager.Current.ThemeSource;
            Settings.Default.ChosenFontSize = AppearanceManager.Current.FontSize;
            Settings.Default.ChosenAccent = AppearanceManager.Current.AccentColor;

            Settings.Default.Save();
        }

        private void ModernWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            AppearanceManager.Current.ThemeSource = Settings.Default.ChosenTheme;
            AppearanceManager.Current.FontSize = Settings.Default.ChosenFontSize;
            AppearanceManager.Current.AccentColor = Settings.Default.ChosenAccent;
        }
    }
}
