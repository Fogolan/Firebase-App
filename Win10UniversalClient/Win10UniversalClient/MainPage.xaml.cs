using FirebaseSharp.Portable;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Core;
using Newtonsoft.Json;
using System.Collections.Generic;
using Win10UniversalClient.Entities;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Win10UniversalClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        CoreDispatcher _dispatcher;
        ObservableCollection<DataSample> _items;
        FirebaseApp _app;

        public MainPage()
        {
            this.InitializeComponent();
            _dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
            _app = new FirebaseApp(new Uri("https://win10-and-web-app.firebaseio.com/"));
        }

        private void AddOrUpdate(IDataSnapshot snap)
        {
            var ignored = _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                PaintItem(snap);
            });
        }

        private void PaintItem(IDataSnapshot snap)
        {
            _items = JsonConvert.DeserializeObject<ObservableCollection<DataSample>>(snap.Value());
            DataList.ItemsSource = _items;
        }

        private void Grid_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _items.CollectionChanged += Items_CollectionChanged;
            var scoresRef = _app.Child("dataSample")
                .On("child_changed", (snap, previous_child, context) => AddOrUpdate(snap));
            scoresRef = _app.Child("dataSample")
                .On("value", (snap, previous_child, context) => AddOrUpdate(snap));
        }

        private static void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            
        }
    }
}
