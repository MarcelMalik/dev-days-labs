using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using DevDaysSpeakers.Model;
using DevDaysSpeakers.Services;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace DevDaysSpeakers.ViewModel
{
    public class SpeakersViewModel : INotifyPropertyChanged
    {
        private bool _isBusy;

        private void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<Speaker> Speakers { get; set; }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                OnPropertyChanged();
                // Update the can execute
                GetSpeakersCommand.ChangeCanExecute();
            }
        }

        public Command GetSpeakersCommand { get; private set; }

        public SpeakersViewModel()
        {
            Speakers = new ObservableCollection<Speaker>();

            GetSpeakersCommand = new Command(
                async () => await GetSpeakers(),
                () => !IsBusy);
        }

        private async Task GetSpeakers()
        {
            if (IsBusy)
                return;

            Exception error = null;
            try
            {
                IsBusy = true;

                /*using (var client = new HttpClient())
                {
                    // grab json from server
                    var json = await client.GetStringAsync("http://demo4404797.mockable.io/speakers");

                    var items = JsonConvert.DeserializeObject<List<Speaker>>(json);

                    Speakers.Clear();
                    foreach (var item in items)
                    {
                        Speakers.Add(item);
                    }
                }*/

                var service = DependencyService.Get<AzureService>();
                var items = await service.GetSpeakers();

                Speakers.Clear();
                foreach (var item in items)
                {
                    Speakers.Add(item);
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }
            finally
            {
                IsBusy = false;
            }
            if (error != null)
            {
                await Application.Current.MainPage.DisplayAlert("Error!", error.Message, "OK");
            }
        }
    }
}
