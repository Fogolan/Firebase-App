using FirebaseSharp.Portable;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Core;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using Win10UniversalClient.Entities;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Win10UniversalClient
{
    public sealed partial class MainPage : Page
    {
        CoreDispatcher _dispatcher;
        FirebaseApp _app;
        private string _displayName = null;
        public static ApplicationDataContainer _settings = ApplicationData.Current.RoamingSettings;
        public bool isEditEnable = false;

        private int previousSelectedIndex = -1;

        public int PreviousSelectedIndex
        {
            get { return previousSelectedIndex; }

            set { previousSelectedIndex = value; }
        }

        public MainPage()
        {
            this.InitializeComponent();
            _dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
            _app = new FirebaseApp(
                new Uri(
                    "https://win10-and-web-app.firebaseio.com/")); //Initialize Firebase instance with your Firebase Url
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            var scoresRef = _app.Child("dataSample") //Your data start node name
                .On("value",
                    (snap, previous_child, context) => AddOrUpdate(snap)); //Subscribe to changing data in Firebase
        }

        private void DataList_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        {
            var data = JsonConvert.SerializeObject(DataList.Items);
            _app.Child("dataSample").Set(data); //send data to Firebase

            UpdateButton.IsEnabled = DataList.SelectedItem != null;

            DataSample selectedItem = DataList.SelectedItem as DataSample;
            if (selectedItem != null)
            {
                selectedItem.IsReadOnly = true;
                PreviousSelectedIndex = DataList.SelectedIndex;
            }
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
            if (DataList.Items.Count == 0) //When you start app, your DataList is empty
                InitializeDataListView(_firebaseItems);
        }

        private List<DataSample>
            GetDifferentFromFirebase(List<DataSample> firebaseItems) //You need to know, what rows have changed
        {
            var different = new List<DataSample>();
            foreach (var item in DataList.Items)
            {
                var dataItem = item as DataSample;
                if (dataItem.Id != firebaseItems[DataList.Items.IndexOf(item)].Id)
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
                var index = DataList.Items.IndexOf(DataList.Items.FirstOrDefault(x => x.Equals(item)));
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
            var keyFrame1 = new LinearColorKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0)),
                Value = Colors.White
            };
            var keyFrame2 = new LinearColorKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(400)),
                Value = Colors.Red
            };
            var keyFrame3 = new LinearColorKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(1200)),
                Value = Colors.White
            };
            colorAnimation.KeyFrames.Add(keyFrame1);
            colorAnimation.KeyFrames.Add(keyFrame2);
            colorAnimation.KeyFrames.Add(keyFrame3);

            Storyboard.SetTarget(colorAnimation, item);
            Storyboard.SetTargetProperty(colorAnimation, "(Control.Background).(SolidColorBrush.Color)");

            var storyboard = new Storyboard();
            storyboard.Children.Add(colorAnimation);
            storyboard.Begin();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.Current.Resources.ContainsKey("ida:ClientID"))
            {
                InfoText.Text = ResourceLoader.GetForCurrentView().GetString("NoClientIdMessage");
                ConnectButton.IsEnabled = false;
            }
            else
            {
                InfoText.Text = ResourceLoader.GetForCurrentView().GetString("ConnectPrompt");
                ConnectButton.IsEnabled = true;
            }
        }

        public async Task<bool> SignInCurrentUserAsync()
        {
            var graphClient = AuthenticationHelper.GetAuthenticatedClient();
            try
            {
                if (graphClient != null)
                {
                    var user = await graphClient.Me.Request().GetAsync();
                    string userId = user.Id;
                    _displayName = user.DisplayName;
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }

        }

        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            ProgressBar.Visibility = Visibility.Visible;
            try
            {
                if (await SignInCurrentUserAsync())
                {
                    InfoText.Text = "Hi " + _displayName + "," + Environment.NewLine +
                                    ResourceLoader.GetForCurrentView().GetString("SendMailPrompt");
                    DataList.IsEnabled = true;
                    DataList.Visibility = Visibility.Visible;
                    DeleteButton.Visibility = Visibility.Visible;
                    UpdateButton.Visibility = Visibility.Visible;
                    ConnectButton.Visibility = Visibility.Collapsed;
                    DisconnectButton.Visibility = Visibility.Visible;
                }
                else
                {
                    InfoText.Text = ResourceLoader.GetForCurrentView().GetString("AuthenticationErrorMessage");
                }

                ProgressBar.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {

            }
        }

        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            ProgressBar.Visibility = Visibility.Visible;
            AuthenticationHelper.SignOut();
            ProgressBar.Visibility = Visibility.Collapsed;
            DataList.IsEnabled = false;
            DataList.Visibility = Visibility.Collapsed;
            ConnectButton.Visibility = Visibility.Visible;
            UpdateButton.Visibility = Visibility.Collapsed;
            DeleteButton.Visibility = Visibility.Collapsed;
            InfoText.Text = ResourceLoader.GetForCurrentView().GetString("ConnectPrompt");
            this._displayName = null;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            DataList.Items.Remove(DataList.SelectedItem);
            var data = JsonConvert.SerializeObject(DataList.Items);
            _app.Child("dataSample").Set(data); //send data to Firebase
        }

        private void DataList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DeleteButton.IsEnabled = UpdateButton.IsEnabled = DataList.SelectedItem != null;

            if (PreviousSelectedIndex != -1)
            {
                DataSample previousSelectedItem = DataList.Items[PreviousSelectedIndex] as DataSample;
                if (previousSelectedItem != null)
                {
                    if (!previousSelectedItem.IsReadOnly)
                    {
                        previousSelectedItem.IsReadOnly = true;
                        PreviousSelectedIndex = DataList.SelectedIndex;

                        var data = JsonConvert.SerializeObject(DataList.Items);
                        _app.Child("dataSample").Set(data); //send data to Firebase
                    }
                }
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            DataSample selectedItem = DataList.SelectedItem as DataSample;
            if (selectedItem != null)
            {
                PreviousSelectedIndex = DataList.SelectedIndex;
                selectedItem.IsReadOnly = false;
            }
        }
    }
}
