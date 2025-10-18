using System;
using System.Windows;
using LawOfficeApp.MVVM;

namespace LawOfficeApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            try
            {
                DataContext = new MainViewModel();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing: {ex.Message}\n\n{ex.InnerException?.Message}", "Error");
            }
        }
    }
}