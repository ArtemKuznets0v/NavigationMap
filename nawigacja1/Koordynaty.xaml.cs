using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace nawigacja1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Koordynaty : Page
    {
        public Koordynaty()
        {
            this.InitializeComponent();
            GdzieJaNaMapie();
        }

        private async void szukajBut(object sender, RoutedEventArgs e)
        {
            DaneGeograficzne.opisCelu = txAdres.Text;
            if (DaneGeograficzne.opisCelu == "")
                return;

            MapLocationFinderResult ZnajdzAdres = await MapLocationFinder.FindLocationsAsync(txAdres.Text, new Geopoint(DaneGeograficzne.pktStartowy), 3);

            if (ZnajdzAdres.Status == MapLocationFinderStatus.Success)
            {
                DaneGeograficzne.pktDocelowy = ZnajdzAdres.Locations[0].Point.Position;
                DaneGeograficzne.opisCelu = ZnajdzAdres.Locations[0].DisplayName;
                tbDlg.Text += ZnajdzAdres.Locations[0].Point.Position.Longitude.ToString();
                tbSzer.Text += ZnajdzAdres.Locations[0].Point.Position.Latitude.ToString();
            }
            else
            {
                txAdres.Text = "Blad";
            }
        }

        private void backMapa(object sender, RoutedEventArgs e)
        {
                Frame.GoBack();
        }
        private async void GdzieJaNaMapie()
        {
            
            Geolocator mojGps = new Geolocator();
            mojGps.DesiredAccuracy = PositionAccuracy.High;
            Geoposition mojeZGPS = await mojGps.GetGeopositionAsync();




            tbGPS.Text = mojeZGPS.Coordinate.Point.Position.Latitude.ToString() + " " + mojeZGPS.Coordinate.Point.Position.Longitude.ToString();
            DaneGeograficzne.pktStartowy = mojeZGPS.Coordinate.Point.Position;
            DaneGeograficzne.opisCelu = "";
            BasicGeoposition PozycjaStartu = new BasicGeoposition();
            PozycjaStartu.Latitude = mojeZGPS.Coordinate.Point.Position.Latitude;
            PozycjaStartu.Longitude = mojeZGPS.Coordinate.Point.Position.Longitude;


        }
    }
}
