using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Mini7
{
    public partial class MainPage : ContentPage
    {
        private const string Url = "https://jsonplaceholder.typicode.com/posts";
        private readonly HttpClient _httpClient = new HttpClient();
        private ObservableCollection<Post> _posts;

        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            var content = await _httpClient.GetStringAsync(Url);
            var posts = JsonConvert.DeserializeObject<List<Post>>(content);

            _posts = new ObservableCollection<Post>(posts);
            postsListView.ItemsSource = _posts;

            base.OnAppearing();
        }

        async void OnAdd(object sender, System.EventArgs e)
        {
            var post = new Post { Title = "Title " + DateTime.Now.ToString() };

            var content = JsonConvert.SerializeObject(post);
            await _httpClient.PostAsync(Url, new StringContent(content));

            _posts.Insert(0, post);
        }

        async void OnUpdate(object sender, System.EventArgs e)
        {
            if (_posts.Count == 0)
            {
                await DisplayAlert("END", "No more Posts to update!", "OK");
                return;
            }
            var post = _posts[0];
            post.Title += " UPDATED";

            var content = JsonConvert.SerializeObject(post);
            await _httpClient.PutAsync($"{Url}/{post.Id}", new StringContent(content));
        }

        async void OnDelete(object sender, System.EventArgs e)
        {
            if (_posts.Count == 0)
            {
                await DisplayAlert("END", "No more Posts to delete!", "OK");
                return;
            }
            var post = _posts[0];
           
            await _httpClient.DeleteAsync($"{Url}/{post.Id}");

            _posts.Remove(post);
        }
    }

    public class Post : INotifyPropertyChanged
    {
        public int Id { get; set; }

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title == value)
                    return;

                _title = value;
                OnPropertyChanged();
            }
        }

        public string Body { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
