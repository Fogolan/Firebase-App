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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Win10UniversalClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        CoreDispatcher _dispatcher;
        ObservableCollection<DataSample> ListItems;
        FirebaseApp _app;

        public MainPage()
        {
            this.InitializeComponent();
            _dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
            _app = new FirebaseApp(new Uri("https://win10-and-web-app.firebaseio.com/"));
            ListItems = new ObservableCollection<DataSample>();
        }

        private void AddOrUpdate(IDataSnapshot snap)
        {
            var ignored = _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                PrintItems(snap);
            });
        }

        private void PrintItems(IDataSnapshot snap)
        {
            ObservableCollection<DataSample> _firebaseItems = JsonConvert.DeserializeObject<ObservableCollection<DataSample>>(snap.Value());
            if (!_firebaseItems.SequenceEqual(ListItems))
            {
                ListItems = _firebaseItems;
                DataList.ItemsSource = ListItems;
            }
        }

        private void Grid_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var scoresRef = _app.Child("dataSample")
                .On("child_changed", (snap, previous_child, context) => AddOrUpdate(snap));
            scoresRef = _app.Child("dataSample")
                .On("value", (snap, previous_child, context) => AddOrUpdate(snap));
        }

        private void DataList_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        {
            var data = JsonConvert.SerializeObject(ListItems);
            _app.Child("dataSample").Set(data);
        }
    }
}
