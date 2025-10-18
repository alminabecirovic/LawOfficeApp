using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using LawOfficeApp.Data;
using LawOfficeApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LawOfficeApp.MVVM
{
    public class DashboardViewModel : ViewModelBase
    {
        private readonly LawOfficeDbContext db;

        private ObservableCollection<Case> _activeCases;
        private ObservableCollection<Case> _upcomingDeadlines;

        public ObservableCollection<Case> ActiveCases
        {
            get => _activeCases;
            set => SetProperty(ref _activeCases, value);
        }

        public ObservableCollection<Case> UpcomingDeadlines
        {
            get => _upcomingDeadlines;
            set => SetProperty(ref _upcomingDeadlines, value);
        }

        public DashboardViewModel(LawOfficeDbContext dbContext)
        {
            db = dbContext;
            LoadData();
        }

        public void LoadData()
        {
            try
            {
                var cases = db.Cases
                    .Include(c => c.Client)
                    .Include(c => c.Lawyer)
                    .ToList();

                ActiveCases = new ObservableCollection<Case>(cases);
                UpcomingDeadlines = new ObservableCollection<Case>(cases);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading dashboard data: {ex.Message}", "Error");
            }
        }
    }
}