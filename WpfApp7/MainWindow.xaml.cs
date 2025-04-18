using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp7;

namespace WpfApp7
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly HttpClient client = new HttpClient();
        private const string ApiKey = "eb4eef9a51c416fe612aea9109273d9c";
        private const string ApiUrl = "https://api.openweathermap.org/data/2.5/forecast?q={0}&appid={1}&units=metric&lang=ru";
        public MainWindow()
        {
            InitializeComponent();
            string currentCity = "Пермь";
            UpdateWeather(currentCity);
        }
        private async Task UpdateWeather(string city)
        {
            var weatherData = await FetchWeatherData(city);
            WeatherDataGrid.ItemsSource = weatherData;
            LocationLabel.Content = $"Текущее место: {city}";
        }

        private async void UpdateWeather_Click(object sender, RoutedEventArgs e)
        {
            string city = CityTextBox.Text.Trim();

            if (string.IsNullOrEmpty(city))
            {
                city = "Пермь";
            }

            await UpdateWeather(city);
        }

        private async void UpdateKey(object sender, KeyEventArgs e)
        {
            string city = CityTextBox.Text.Trim();
            if (e.Key == Key.Enter)
            {
                if (string.IsNullOrEmpty(city))
                {
                    MessageBox.Show("Неккоректный ввод города");
                    return;
                }
                await UpdateWeather(city);
            }
        }
        private async Task<List<WeatherData>> FetchWeatherData(string city)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = string.Format(ApiUrl, city, ApiKey);
                HttpResponseMessage response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Ошибка {response.StatusCode}: {response.ReasonPhrase}");
                }

                string responseBody = await response.Content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<dynamic>(responseBody);

                var weatherList = new List<WeatherData>();

                foreach (var item in json.list)
                {
                    weatherList.Add(new WeatherData
                    {
                        DateTime = Convert.ToDateTime(item.dt_txt).ToString("dd.MM.yyyy HH:mm"),
                        Temperature = $"{item.main.temp} °C",
                        Pressure = $"{item.main.pressure} мм рт. ст.",
                        Humidity = $"{item.main.humidity}%",
                        WindSpeed = $"{item.wind.speed} м/с",
                        FeelsLike = $"{item.main.feels_like} °C",
                        WeatherDescription = item.weather[0].description.ToString(),
                    });
                }

                return weatherList;
            }
        }
    }

}
