﻿using System;
using System.Collections.Generic;
using System.Linq;
using Bewegingsapp.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.Maps;
using Xamarin.Essentials;

namespace Bewegingsapp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BewerkRouteToevoegen : ContentPage
    {
        public List<Coördinaat> CoördinatenRoute = new List<Coördinaat>(); // lijst met alle aangemaakte coördinaten, nodig voor het maken van polylines en het opslaan in de database van de coördinaten
        Coördinaat coördinaat;
        Polyline polyline;
        public List<Pin> NieuwPunt = new List<Pin>();
        public List<Pin> PinsLijst = new List<Pin>();

        public BewerkRouteToevoegen()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            Route route = (Route)BindingContext; //ophalen van geselecteerde route
            CoördinatenRoute = await App.Database.LijstCoördinatenRoute(route.IDRoute); //coördinaten ophalen van geselecteerde route

            var latitude = CoördinatenRoute[0].Locatie1;
            var Longitude = CoördinatenRoute[0].Locatie2;
            Map_Route_Bewerken.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(latitude, Longitude), Distance.FromKilometers(0.5))); //startpunt, locatie van eerste coördinaat

            foreach (Coördinaat coördinaat in CoördinatenRoute) //voor iedere coördinaat in de geslecteerde route
            {
                double location1 = coördinaat.Locatie1;
                double location2 = coördinaat.Locatie2;
                Pin pin = new Pin
                {
                    Label = coördinaat.Nummer.ToString(),
                    Type = PinType.Place,
                    Position = new Position(location1, location2)
                };
                pin.MarkerClicked += (s, args) =>
                {
                    args.HideInfoWindow = true;
                };
                Map_Route_Bewerken.Pins.Add(pin); //pin toevoegen aan de map
                PinsLijst.Add(pin); //lijst van pins om polylines te tekenen

                if (PinsLijst.Count >= 2) //als er 2 of meer pins zijn
                {
                    Polyline polyline = new Polyline
                    {
                        StrokeColor = Color.Blue,
                        StrokeWidth = 10,
                        Geopath =
                        {
                            new Position(CoördinatenRoute[PinsLijst.Count - 1].Locatie1, CoördinatenRoute[PinsLijst.Count - 1].Locatie2), // pakt longitude en latitude van voorlaatste item in de list
                            new Position(CoördinatenRoute[PinsLijst.Count - 2].Locatie1, CoördinatenRoute[PinsLijst.Count - 2].Locatie2) // pakt longitude en latitude van laatste item in de list
                        }
                    };

                    Map_Route_Bewerken.MapElements.Add(polyline); //polyline tekenen
                }

            }

        }

        private async void Toevoegen_Clicked(object sender, EventArgs e) //opslaan van nieuw toegevoegde coördinaat
        {
            Route UpdateCoördinatenRoute = (Route)BindingContext;
            Console.WriteLine("Naam Route : " + UpdateCoördinatenRoute.NaamRoute + "IDRoute van coördinaat : " + coördinaat.IDRoute);
            await App.Database.ToevoegenCoördinaat(coördinaat);
            await Navigation.PopAsync();
        }

        private void Map_Route_Bewerken_MapClicked(object sender, MapClickedEventArgs e)
        {
            Route route = (Route)BindingContext; // vereist voor het krijgen van IDRoute
            //longitude en latitude zijn vereist voor het maken van pins op de map
            double location1 = e.Position.Latitude;
            double location2 = e.Position.Longitude;
            int NummerCoördinaat = CoördinatenRoute.Count + 1;

            if (NieuwPunt.Count == 1) //hier wordt ervoor gezord dat er niet meerdere nieuwe pins tegelijk kunnen worden neergezet
            {
                Map_Route_Bewerken.Pins.Remove(NieuwPunt.Last());
                NieuwPunt.Remove(NieuwPunt.Last());
                Map_Route_Bewerken.MapElements.Remove(polyline);
            }
            Pin pin = new Pin //maak nieuwe pin aan op aangeklikte plek op de map
            {
                Label = NummerCoördinaat.ToString(),
                Type = PinType.Place,
                Position = new Position(location1, location2)
            };
            pin.MarkerClicked += (s, args) =>
            {
                args.HideInfoWindow = true;
            };
            Map_Route_Bewerken.Pins.Add(pin);
            NieuwPunt.Add(pin); //pin toevoegen aan de map

            coördinaat = new Coördinaat
            {
                Nummer = NummerCoördinaat,
                Locatie1 = location1,
                Locatie2 = location2,
                IDOEfening = null,
                IDRoute = route.IDRoute
            };

            polyline = new Polyline //maak nieuwe polyline tussen nieuwe pin en laatste pin in route
            {
                StrokeColor = Color.Blue,
                StrokeWidth = 10,
                Geopath =
                        {
                            new Position(CoördinatenRoute.Last().Locatie1, CoördinatenRoute.Last().Locatie2), // pakt longitude en latitude van voorlaatste item in de list
                            new Position(coördinaat.Locatie1, coördinaat.Locatie2) // pakt longitude en latitude van laatste item in de list
                        }
            };
            Map_Route_Bewerken.MapElements.Add(polyline); //polyline tekenen
        }

        private async void Info_Clicked(object sender, EventArgs e) //informatie knop, hoe het toevoegen werkt
        {
            await DisplayAlert("Punt toevoegen", "Het is niet mogelijk om meer dan 1 punt tegelijk toe te voegen. \n \n" +
                                "Het nieuwe punt wordt automatisch toegevoegd aan het einde van de list, het is niet mogelijk " +
                                "om het nieuwe punt tussendoor of aan het begin toe te voegen.", "ok");
        }
    }
}