﻿using System.ComponentModel;
using System.IO;
using System.Linq;
using TranslatorApk.Logic.Classes;
using TranslatorApk.Logic.Interfaces.SettingsPages;
using TranslatorApk.Logic.OrganisationItems;
using TranslatorApk.Logic.Utils;
using TranslatorApk.Logic.WebServices;
using TranslatorApk.Resources.Localizations;
using TranslatorApk.Windows;

namespace TranslatorApk.Logic.ViewModels.SettingsPages
{
    public class AppSettingsPageViewModel : ViewModelBase, ISettingsPageViewModel
    {
        private readonly AppSettings _appSettings = GlobalVariables.AppSettings;

        public AppSettingsPageViewModel()
        {
            RefreshData();

            _appSettings.PropertyChanged += SettingsOnPropertyChanged;
        }

        public string PageTitle { get; } = StringResources.AppSettings_Caption;

        public string[] YesNoItems { get; private set; }

        public int LanguageOfAppIndex
        {
            get => TranslateService.SupportedProgramLangs.IndexOf(_appSettings.LanguageOfApp);
            set => CommonUtils.SetLanguageOfApp(TranslateService.SupportedProgramLangs[value], true);
        }

        public int ShowPreviewsIndex
        {
            get => _appSettings.ShowPreviews ? 0 : 1;
            set => _appSettings.ShowPreviews = value == 0;
        }

        public int TopMostIndex
        {
            get => _appSettings.TopMost ? 0 : 1;
            set => _appSettings.TopMost = value == 0;
        }

        public int ShowNotificationsIndex
        {
            get => _appSettings.ShowNotifications ? 0 : 1;
            set => _appSettings.ShowNotifications = value == 0;
        }

        public int AlternateRowsIndex
        {
            get => _appSettings.AlternatingRows ? 0 : 1;
            set => _appSettings.AlternatingRows = value == 0;
        }

        public string OtherFileExts
        {
            get => _appSettings.OtherExtensions.JoinStr("|");
            set => _appSettings.OtherExtensions = value.SplitRemove('|').Select(_ => _.Trim()).Distinct().ToList();
        }

        public string ImageFileExts
        {
            get => _appSettings.ImageExtensions.JoinStr("|");
            set => _appSettings.ImageExtensions = value.SplitRemove('|').Select(_ => _.Trim()).Distinct().ToList();
        }

        public int FontSize
        {
            get => _appSettings.FontSize;
            set => _appSettings.FontSize = value;
        }

        public int GridFontSize
        {
            get => _appSettings.GridFontSize;
            set => _appSettings.GridFontSize = value;
        }

        public ObservableRangeCollection<string> Themes { get; } = new ObservableRangeCollection<string>();

        public ObservableRangeCollection<string> ApktoolVersions { get; } = new ObservableRangeCollection<string>();

        public string CurrentTheme
        {
            get => GlobalVariables.Themes.First(theme => theme.name == _appSettings.Theme).localizedName;
            set => ThemeUtils.ChangeTheme(GlobalVariables.Themes.First(theme => theme.localizedName == value).name);
        }

        public string CurrentApktoolVersion
        {
            get => _appSettings.ApktoolVersion;
            set
            {
                if (value == null)
                    CommonUtils.IgnoreComboBoxChange();

                if (value == StringResources.Catalog)
                {
                    new ApktoolCatalogWindow().ShowDialog();

                    LoadApktools();

                    CommonUtils.IgnoreComboBoxChange();
                }

                _appSettings.ApktoolVersion = value;
            }
        }

        public void RefreshData()
        {
            YesNoItems = new[] { StringResources.Yes, StringResources.No };

            Themes.ReplaceRange(GlobalVariables.Themes.Select(theme => theme.localizedName));

            LoadApktools();
        }

        private void LoadApktools()
        {
            ApktoolVersions.ReplaceRange(
                Directory.EnumerateFiles(GlobalVariables.PathToApktoolVersions)
                    .Select(Path.GetFileNameWithoutExtension)
                    .Select(s => s.Split('_'))
                    .Where(split => split.Length == 2)
                    .Select(split => split[1])
            );

            ApktoolVersions.Add(StringResources.Catalog);

            if (!ApktoolVersions.Contains(_appSettings.ApktoolVersion))
                _appSettings.ApktoolVersion = ApktoolVersions[0];
            else
                OnPropertyChanged(nameof(CurrentApktoolVersion));
        }

        private void SettingsOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
            {
                case nameof(AppSettings.ApktoolVersion):
                    OnPropertyChanged(nameof(CurrentApktoolVersion));
                    break;
                case nameof(AppSettings.Theme):
                    OnPropertyChanged(nameof(CurrentTheme));
                    break;
                case nameof(AppSettings.ShowPreviews):
                    OnPropertyChanged(nameof(ShowPreviewsIndex));
                    break;
                case nameof(AppSettings.TopMost):
                    OnPropertyChanged(nameof(TopMostIndex));
                    break;
                case nameof(AppSettings.ShowNotifications):
                    OnPropertyChanged(nameof(ShowNotificationsIndex));
                    break;
                case nameof(AppSettings.AlternatingRows):
                    OnPropertyChanged(nameof(AlternateRowsIndex));
                    break;
                case nameof(AppSettings.OtherExtensions):
                    OnPropertyChanged(nameof(OtherFileExts));
                    break;
                case nameof(AppSettings.ImageExtensions):
                    OnPropertyChanged(nameof(ImageFileExts));
                    break;
            }
        }

        public override void UnsubscribeFromEvents()
        {
            _appSettings.PropertyChanged -= SettingsOnPropertyChanged;
        }
    }
}
