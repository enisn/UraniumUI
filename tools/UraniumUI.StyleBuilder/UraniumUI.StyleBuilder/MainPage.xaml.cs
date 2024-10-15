﻿using InputKit.Shared.Controls;
using UraniumUI.Pages;

namespace UraniumUI.StyleBuilder;
public partial class MainPage
{
    public MainPage(MainPageViewModel viewModel)
    {
        InitializeComponent();
        
        BindingContext = viewModel;
    }
}