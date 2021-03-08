using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace Wpf_Reactive_Search
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly List<string> list = new List<string>() { "Afghanistan", "Åland Islands", "Albania", "Algeria", "American Samoa", "Andorra", "Angola", "Anguilla", "Antarctica", "Antigua and Barbuda", "Argentina", "Armenia", "Aruba", "Australia", "Austria", "Azerbaijan", "Bahamas", "Bahrain", "Bangladesh", "Barbados", "Belarus", "Belgium", "Belize", "Benin", "Bermuda", "Bhutan", "Bolivia (Plurinational State of)", "Bonaire, Sint Eustatius and Saba", "Bosnia and Herzegovina", "Botswana", "Bouvet Island", "Brazil", "British Indian Ocean Territory", "United States Minor Outlying Islands", "Virgin Islands (British)", "Virgin Islands (U.S.)", "Brunei Darussalam", "Bulgaria", "Burkina Faso", "Burundi", "Cambodia", "Cameroon", "Canada", "Cabo Verde", "Cayman Islands", "Central African Republic", "Chad", "Chile", "China", "Christmas Island", "Cocos (Keeling) Islands", "Colombia", "Comoros", "Congo", "Congo (Democratic Republic of the)", "Cook Islands", "Costa Rica", "Croatia", "Cuba", "Curaçao", "Cyprus", "Czech Republic", "Denmark", "Djibouti", "Dominica", "Dominican Republic", "Ecuador", "Egypt", "El Salvador", "Equatorial Guinea", "Eritrea", "Estonia", "Ethiopia", "Falkland Islands (Malvinas)", "Faroe Islands", "Fiji", "Finland", "France", "French Guiana", "French Polynesia", "French Southern Territories", "Gabon", "Gambia", "Georgia", "Germany", "Ghana", "Gibraltar", "Greece", "Greenland", "Grenada", "Guadeloupe", "Guam", "Guatemala", "Guernsey", "Guinea", "Guinea-Bissau", "Guyana", "Haiti", "Heard Island and McDonald Islands", "Holy See", "Honduras", "Hong Kong", "Hungary", "Iceland", "India", "Indonesia", "Côte d'Ivoire", "Iran (Islamic Republic of)", "Iraq", "Ireland", "Isle of Man", "Israel", "Italy", "Jamaica", "Japan", "Jersey", "Jordan", "Kazakhstan", "Kenya", "Kiribati", "Kuwait", "Kyrgyzstan", "Lao People's Democratic Republic", "Latvia", "Lebanon", "Lesotho", "Liberia", "Libya", "Liechtenstein", "Lithuania", "Luxembourg", "Macao", "Macedonia (the former Yugoslav Republic of)", "Madagascar", "Malawi", "Malaysia", "Maldives", "Mali", "Malta", "Marshall Islands", "Martinique", "Mauritania", "Mauritius", "Mayotte", "Mexico", "Micronesia (Federated States of)", "Moldova (Republic of)", "Monaco", "Mongolia", "Montenegro", "Montserrat", "Morocco", "Mozambique", "Myanmar", "Namibia", "Nauru", "Nepal", "Netherlands", "New Caledonia", "New Zealand", "Nicaragua", "Niger", "Nigeria", "Niue", "Norfolk Island", "Korea (Democratic People's Republic of)", "Northern Mariana Islands", "Norway", "Oman", "Pakistan", "Palau", "Palestine, State of", "Panama", "Papua New Guinea", "Paraguay", "Peru", "Philippines", "Pitcairn", "Poland", "Portugal", "Puerto Rico", "Qatar", "Republic of Kosovo", "Réunion", "Romania", "Russian Federation", "Rwanda", "Saint Barthélemy", "Saint Helena, Ascension and Tristan da Cunha", "Saint Kitts and Nevis", "Saint Lucia", "Saint Martin (French part)", "Saint Pierre and Miquelon", "Saint Vincent and the Grenadines", "Samoa", "San Marino", "Sao Tome and Principe", "Saudi Arabia", "Senegal", "Serbia", "Seychelles", "Sierra Leone", "Singapore", "Sint Maarten (Dutch part)", "Slovakia", "Slovenia", "Solomon Islands", "Somalia", "South Africa", "South Georgia and the South Sandwich Islands", "Korea (Republic of)", "South Sudan", "Spain", "Sri Lanka", "Sudan", "Suriname", "Svalbard and Jan Mayen", "Swaziland", "Sweden", "Switzerland", "Syrian Arab Republic", "Taiwan", "Tajikistan", "Tanzania, United Republic of", "Thailand", "Timor-Leste", "Togo", "Tokelau", "Tonga", "Trinidad and Tobago", "Tunisia", "Turkey", "Turkmenistan", "Turks and Caicos Islands", "Tuvalu", "Uganda", "Ukraine", "United Arab Emirates", "United Kingdom of Great Britain and Northern Ireland", "United States of America", "Uruguay", "Uzbekistan", "Vanuatu", "Venezuela (Bolivarian Republic of)", "Viet Nam", "Wallis and Futuna", "Western Sahara", "Yemen", "Zambia", "Zimbabwe" };
        public MainWindow()
        {
            InitializeComponent();

            //IObservable<EventPattern<TEventArgs>> FromEventPattern<TDelegate, TEventArgs>(Action<TDelegate> addHandler, Action<TDelegate> removeHandler);
            var query = Observable.FromEventPattern
            <TextChangedEventHandler, TextChangedEventArgs>(
                h => et.TextChanged += h, // 
                h => et.TextChanged -= h)
            .Throttle(TimeSpan.FromMilliseconds(600))
            .ObserveOnDispatcher()
            .Select(x => et.Text)
            .DistinctUntilChanged()
            .Do(x => lb.ItemsSource = null)
            .ObserveOn(Scheduler.Default)
            .Select(x => ExecuteSearch(x))
            .ObserveOnDispatcher();

            query.Subscribe(x => lb.ItemsSource = x);
        }

        private ObservableCollection<string> ExecuteSearch(string searchText)
        {
            //string[] filters = searchText.Split(new[] { ' ' });
            //var q = from x in list
            //where filters.All(f => x.Contains(f))
            //select x;

            FuzzySearch _fuzz = new FuzzySearch(searchText);

            var q = from x in list
                    where _fuzz.IsMatch(x)
                    select x;


            //var q = from s in list where s.ToLower().Contains(searchText.ToLower()) select s;
            var results = new ObservableCollection<string>(q);
            return results;
        }

        class FuzzySearch
        {

            private string _searchTerm;
            private string[] _searchTerms;
            private Regex _searchPattern;

            public FuzzySearch(string searchTerm)
            {
                _searchTerm = searchTerm;
                _searchTerms = searchTerm.Split(new char[] { ' ' });
                _searchPattern = new Regex(
                    "(?i)(?=.*" + string.Join(")(?=.*", _searchTerms) + ")");
            }

            public bool IsMatch(string value)
            {
                if (_searchTerm == value) return true;
                if (value.Contains(_searchTerm)) return true;
                if (_searchPattern.IsMatch(value)) return true;
                return false;
            }

        }
    }
}
