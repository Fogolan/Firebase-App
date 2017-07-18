using FirebaseSharp.Portable;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Core;
using Newtonsoft.Json;
using System.Collections.Generic;
using Win10UniversalClient.Entities;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Win10UniversalClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        CoreDispatcher _dispatcher;
        FirebaseApp _app;

        public MainPage()
        {
            this.InitializeComponent();
            _dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
            _app = new FirebaseApp(new Uri("https://win10-and-web-app.firebaseio.com/"));
        }

        private void Grid_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var scoresRef = _app.Child("dataSample")
                .On("value", (snap, previous_child, context) => AddOrUpdate(snap));
        }

        private void DataList_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        {
            var data = JsonConvert.SerializeObject(DataList.Items);
            _app.Child("dataSample").Set(data);
        }

        private void AddOrUpdate(IDataSnapshot snap)
        {
            var ignored = _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                SetDataList(snap);
            });
        }

        private void SetDataList(IDataSnapshot snap)
        {
            List<DataSample> _firebaseItems = JsonConvert.DeserializeObject<List<DataSample>>(snap.Value());
            AnimateRows(GetDifferentFromFirebase(_firebaseItems));
            if (DataList.Items.Count == 0)
                InitializeDataListView(_firebaseItems);
        }

        private List<DataSample> GetDifferentFromFirebase(List<DataSample> firebaseItems)
        {
            var different = new List<DataSample>();
            foreach (var item in DataList.Items)
            {
                if (!item.Equals(firebaseItems[DataList.Items.IndexOf(item)]))
                {
                    different.Add(firebaseItems[DataList.Items.IndexOf(item)]);
                    DataList.Items[DataList.Items.IndexOf(item)] = firebaseItems[DataList.Items.IndexOf(item)];
                }
            }
            return different;
        }

        private void InitializeDataListView(List<DataSample> dataList)
        {
            foreach (var item in dataList)
            {
                DataList.Items.Add(item);
            }
        }

        private void AnimateRows(List<DataSample> different)
        {
            foreach (var item in different)
            {
                var index = DataList.Items.IndexOf(DataList.Items.Where(x => x.Equals(item)).FirstOrDefault());
                if (DataList.ContainerFromIndex(index) is ListViewItem ItemContainer)
                    ColorAnimation(ItemContainer);
            }
        }

        private void ColorAnimation(ListViewItem item)
        {
            var colorAnimation = new ColorAnimationUsingKeyFrames
            {
                BeginTime = TimeSpan.FromMilliseconds(300)
            };
            var keyFrame1 = new LinearColorKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0)), Value = Colors.White };
            var keyFrame2 = new LinearColorKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(400)), Value = Colors.LightGray };
            var keyFrame3 = new LinearColorKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(1200)), Value = Colors.White };
            colorAnimation.KeyFrames.Add(keyFrame1);
            colorAnimation.KeyFrames.Add(keyFrame2);
            colorAnimation.KeyFrames.Add(keyFrame3);

            Storyboard.SetTarget(colorAnimation, item);
            Storyboard.SetTargetProperty(colorAnimation, "(Control.Background).(SolidColorBrush.Color)");

            var storyboard = new Storyboard();
            storyboard.Children.Add(colorAnimation);
            storyboard.Begin();
        }
    }
}
