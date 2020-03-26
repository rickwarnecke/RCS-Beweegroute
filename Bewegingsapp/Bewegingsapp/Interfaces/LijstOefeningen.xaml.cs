﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bewegingsapp.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Bewegingsapp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LijstOefeningen : ContentPage
    {
        public LijstOefeningen()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            Oefeningen.ItemsSource = await App.Database.LijstOefeningen();
        }

        private async void Bewerk_Oefening_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BewerkOefening());
        }

        private async void Add_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new OefeningToevoegen());
        }

        private async void Oefeningen_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            await Navigation.PushAsync(new BewerkOefening
            {
                BindingContext = e.SelectedItem
            });
            
        }
    }
}