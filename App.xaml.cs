﻿namespace MauiWeatherApp;

public partial class App : Application
{
    public App(MainPage page)
    {
        InitializeComponent();

        MainPage = page;
    }
}