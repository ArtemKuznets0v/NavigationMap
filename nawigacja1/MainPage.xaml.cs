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
using Windows.Services.Maps;
using Windows.UI.Xaml.Controls.Maps;
using Windows.Devices.Geolocation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace nawigacja1
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();
            MapService.ServiceToken = "ljxj8RBsFrc6iO9Ogahg~983av3IOndKm3Y_lNQAjQA~Av_YXYDNMiDE2ufnGKiZ5NrQ1aTel4aiQ1IJpHNYxa7pIegHhKL4bvKvA4-vTPs8";

        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (DaneGeograficzne.opisCelu == null)
                return;
            Geopoint PunktStartowy = new Geopoint(DaneGeograficzne.pktStartowy);
            MapIcon znacznikStart = new MapIcon()
            {
                Location = PunktStartowy,
                Title = "Tutaj jestem!"
            };
            mojaMapa.MapElements.Add(znacznikStart);
            Geopoint PunktDocelowy = new Geopoint(DaneGeograficzne.pktDocelowy);
            MapIcon znacznikKoniec = new MapIcon()
            {
                Location = PunktDocelowy,
                Title = "Target"
            };
            mojaMapa.MapElements.Add(znacznikKoniec);

            MapPolyline trasaLotem = new MapPolyline()
            {
                StrokeColor = Windows.UI.Colors.Black,
                StrokeDashed = true,
                StrokeThickness = 3,
                Path = new Geopath(new List<BasicGeoposition> { DaneGeograficzne.pktStartowy, DaneGeograficzne.pktDocelowy })
            };
            mojaMapa.MapElements.Add(trasaLotem);
            await mojaMapa.TrySetViewAsync(new Geopoint(DaneGeograficzne.pktStartowy), 8);
            Trasa();
            Dystancja();
        }
        private async void Dystancja()

        {
            double latitudeX = DaneGeograficzne.pktStartowy.Latitude;
            double latitudeY = DaneGeograficzne.pktDocelowy.Latitude;
            double longitudeX = DaneGeograficzne.pktStartowy.Longitude;
            double longitudeY = DaneGeograficzne.pktDocelowy.Longitude;

            double rlat1 = Math.PI * latitudeX / 180;
            double rlat2 = Math.PI * latitudeY / 180;
            double theta = longitudeX - longitudeY;
            double rtheta = Math.PI * theta / 180;
            double dyst =
                Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                Math.Cos(rlat2) * Math.Cos(rtheta);
            dyst = Math.Acos(dyst);
            dyst = dyst * 180 / Math.PI;
            dyst = dyst * 60 * 1.1515; ;
            dyst = Math.Round(dyst, 1);

            ContentDialog dystancja = new ContentDialog
            {
                Title = "Dystancja",
                Content = "Odleglosc: " + dyst + " km",
                CloseButtonText = "OK!"

            };
            await dystancja.ShowAsync();

            DaneGeograficzne.kmDystancja = dyst.ToString();
        }

        private async void Trasa()
        {
            MapRouteFinderResult routeResult =
         await MapRouteFinder.GetDrivingRouteAsync(
         new Geopoint(DaneGeograficzne.pktStartowy),
         new Geopoint(DaneGeograficzne.pktDocelowy),
         MapRouteOptimization.Time,
         MapRouteRestrictions.None);
            if (routeResult.Status == MapRouteFinderStatus.Success)
            {

                MapRouteView viewOfRoute = new MapRouteView(routeResult.Route);
                viewOfRoute.RouteColor = Windows.UI.Colors.Aqua;
                viewOfRoute.OutlineColor = Windows.UI.Colors.Black;
                mojaMapa.Routes.Add(viewOfRoute);
            }
        }

        private void trybMapy(object sender, RoutedEventArgs e)
        {
            AppBarButton ab = (AppBarButton)sender;
            var fIcon = new FontIcon();


            // mojaMapa.Style == MapStyle.AerialWithRoads;
            // enum Mapstyle.Roads;
            // MapStyle: Road / AerialWithRoads;
            fIcon.FontFamily = FontFamily.XamlAutoFontFamily;
            if (mojaMapa.Style == MapStyle.AerialWithRoads)
            {
                mojaMapa.Style = MapStyle.Road;
                ab.Label = "satelita";
                fIcon.Glyph = "S";
                ab.Icon = fIcon;
            }
            else
            {
                mojaMapa.Style = MapStyle.AerialWithRoads;
                ab.Label = "mapa";
                fIcon.Glyph = "M";
                ab.Icon = fIcon;
            }
        }
        private void pomMapa(object sender, RoutedEventArgs e)
        {
            mojaMapa.ZoomLevel--;
        }

        private void powMapa(object sender, RoutedEventArgs e)
        {
            mojaMapa.ZoomLevel++;
        }

        private void koordynaty(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoForward)
                Frame.GoForward();
            else
                Frame.Navigate(typeof(Koordynaty));
        }

        private async void koniec(object sender, RoutedEventArgs e)
        {
            ContentDialog KoniecBut = new ContentDialog
            {
                Title = "Na pewno chcesz zamknąć program?",
                CloseButtonText = "Nie",
                PrimaryButtonText = "Tak",

            };
            ContentDialogResult result = await KoniecBut.ShowAsync();
            if (result == ContentDialogResult.Primary)
                Environment.Exit(0);
        }
    }
}
