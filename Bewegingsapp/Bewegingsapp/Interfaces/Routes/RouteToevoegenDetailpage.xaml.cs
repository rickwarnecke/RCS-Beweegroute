﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Bewegingsapp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RouteToevoegenDetailpage : ContentPage
    {
        public RouteToevoegenDetailpage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            Oefeningen_Picker.ItemsSource = await App.Database.LijstOefeningen();
        }
    }
}