﻿using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using Param_ItemNamespace.Helpers;
using Param_ItemNamespace.Models;

namespace Param_ItemNamespace.ViewModels
{
    public class wts.ItemNameExampleViewModel : Observable
    {
        private ShareOperation _shareOperation;

        private string _pageTitle;

        public string PageTitle
        {
            get => _pageTitle;
            set => Set(ref _pageTitle, value);
        }

        private SharedContentModel _sharedData;

        public SharedContentModel SharedData
        {
            get => _sharedData;
            set => Set(ref _sharedData, value);
        }

        private ICommand _completeCommand;

        public ICommand CompleteCommand => _completeCommand ?? (_completeCommand = new RelayCommand(OnComplete));

        public wts.ItemNameExampleViewModel()
        {
        }

        public async Task LoadAsync(ShareOperation shareOperation)
        {
            // TODO WTS: Configure your Share Target Declaration to allow other data formats.
            // Share Target declarations are defined in Package.appxmanifest.
            // Current declarations allow to share WebLink and image files with the app.
            // ShareTarget can be tested sharing the WebLink from Microsoft Edge or sharing images from Windows Photos.

            // ShareOperation contains all the information required to handle the action.
            _shareOperation = shareOperation;

            // TODO WTS: Custom SharedContentModel class adding properties for data that you need to extract from _shareOperation
            var newSharedData = new SharedContentModel
            {
                Title = _shareOperation.Data.Properties.Title
            };

            if (_shareOperation.Data.Contains(StandardDataFormats.WebLink))
            {
                PageTitle = "ShareTargetExample_WebLinkTitle".GetLocalized();
                newSharedData.DataFormat = StandardDataFormats.WebLink;
                newSharedData.Uri = await _shareOperation.GetWebLinkAsync();
            }

            if (_shareOperation.Data.Contains(StandardDataFormats.StorageItems))
            {
                PageTitle = "ShareTargetExample_ImagesTitle".GetLocalized();
                newSharedData.DataFormat = StandardDataFormats.StorageItems;
                var files = await _shareOperation.GetStorageItemsAsync();
                foreach (var file in files)
                {
                    var storageFile = file as StorageFile;
                    if (storageFile != null)
                    {
                        using (var inputStream = await storageFile.OpenReadAsync())
                        {
                            var img = new BitmapImage();
                            img.SetSource(inputStream);
                            newSharedData.Images.Add(img);
                        }
                    }
                }
            }

            SharedData = newSharedData;
        }

        private void OnComplete()
        {
            // TODO WTS: Implement the actions you want with the shared data before compleate the share operation.            
            // For further details check https://docs.microsoft.com/en-us/windows/uwp/app-to-app/receive-data
            _shareOperation.ReportCompleted();
        }
    }
}
