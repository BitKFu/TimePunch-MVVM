using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using TimePunch.MVVM.Controller;
using TimePunch.MVVM.EventAggregation;
using TimePunchIntroPlayerWinUI3.Events.AboutPage;
using TimePunchIntroPlayerWinUI3.Events.MainPage;
using TimePunchIntroPlayerWinUI3.Pages;

namespace TimePunchIntroPlayerWinUI3.Core
{
    public class TimePunchIntroPlayerWinUI3Controller : BaseController
        , IHandleMessageAsync<OpenStorageFileEvent>
        , IHandleMessage<OpenAboutWindowEvent>
        , IHandleMessage<VisitTwitterWebsiteEvent>
        , IHandleMessage<VisitHomepageSiteEvent>
    {
        public TimePunchIntroPlayerWinUI3Controller() : base(TimePunchIntroPlayerWinUi3Kernel.Get().EventAggregator)
        {

        }

        #region OpenNewPages

        public void Handle(OpenAboutWindowEvent message)
        {
            ((IDisposable)CurrentPage).Dispose();
            NavigateToPage(typeof(AboutPage), message);
        }

        public void Handle(VisitTwitterWebsiteEvent message)
        {
            NavigateToPage(typeof(TwitterPage), message);
        }

        public void Handle(VisitHomepageSiteEvent message)
        {
            NavigateToPage(typeof(HomePage), message);
        }

        #endregion

        /// <summary>
        /// Shows the menu, in which you can pick a file to execute in the mediaelement
        /// </summary>
        /// <param name="message">OpenStorageFileEvent</param>
        /// <returns>Returns the OpenStorageFileEvent</returns>
        async Task<OpenStorageFileEvent> IHandleMessageAsync<OpenStorageFileEvent>.Handle(OpenStorageFileEvent message)
        {
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.VideosLibrary,
            };
            WinUIConversionUtil.InitFileOpenPicker(picker);

            picker.FileTypeFilter.Add(".mp4");
            
            var pickFile = await picker.PickSingleFileAsync();
            if (pickFile != null)
            {
                var fileStream = await pickFile.OpenAsync(FileAccessMode.ReadWrite);
                
                message.ContentType = pickFile.ContentType;
                message.Result = true;
                message.FileStream = fileStream;
            }
            return message;
        }
    }
}
