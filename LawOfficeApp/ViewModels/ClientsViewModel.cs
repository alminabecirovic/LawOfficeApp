using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using LawOfficeApp.Data;
using LawOfficeApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LawOfficeApp.MVVM.ViewModels
{
    public class ClientsViewModel : ViewModelBase
    {
        private readonly LawOfficeDbContext db;

        // Collections
        private ObservableCollection<Client> _clients;
        private ObservableCollection<object> _updateClientList;

        public ObservableCollection<Client> Clients
        {
            get => _clients;
            set => SetProperty(ref _clients, value);
        }

        public ObservableCollection<object> UpdateClientList
        {
            get => _updateClientList;
            set => SetProperty(ref _updateClientList, value);
        }

        // Add Client Fields
        private string _firstName;
        private string _lastName;
        private string _email;
        private string _phone;

        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Phone
        {
            get => _phone;
            set => SetProperty(ref _phone, value);
        }

        // Update Client Fields
        private object _selectedUpdateClient;
        private string _updateEmail;

        public object SelectedUpdateClient
        {
            get => _selectedUpdateClient;
            set => SetProperty(ref _selectedUpdateClient, value);
        }

        public string UpdateEmail
        {
            get => _updateEmail;
            set => SetProperty(ref _updateEmail, value);
        }

        // Commands
        public ICommand AddClientCommand { get; }
        public ICommand UpdateClientCommand { get; }

        public ClientsViewModel(LawOfficeDbContext dbContext)
        {
            db = dbContext;

            AddClientCommand = new RelayCommand(_ => AddClient());
            UpdateClientCommand = new RelayCommand(_ => UpdateClient());

            LoadData();
        }

        public void LoadData()
        {
            try
            {
                var clients = db.Clients
                    .Include(c => c.Cases)
                    .ToList();

                Clients = new ObservableCollection<Client>(clients);

                var clientsDisplay = clients.Select(c => new
                {
                    Id = c.Id,
                    FullName = $"{c.FirstName} {c.LastName}",
                    Email = c.Email
                }).ToList();

                UpdateClientList = new ObservableCollection<object>(clientsDisplay);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading clients: {ex.Message}", "Error");
            }
        }

        private void AddClient()
        {
            try
            {
                var client = new Client
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    Email = Email,
                    PhoneNumber = Phone
                };

                db.Clients.Add(client);
                db.SaveChanges();

                FirstName = string.Empty;
                LastName = string.Empty;
                Email = string.Empty;
                Phone = string.Empty;

                LoadData();
                MessageBox.Show("Client added successfully!", "Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error");
            }
        }

        private void UpdateClient()
        {
            try
            {
                if (SelectedUpdateClient == null)
                {
                    MessageBox.Show("Please select a client!", "Error");
                    return;
                }

                var clientData = SelectedUpdateClient as dynamic;
                int id = clientData.Id;
                var client = db.Clients.Find(id);

                if (client != null)
                {
                    client.Email = UpdateEmail;
                    db.SaveChanges();

                    UpdateEmail = string.Empty;

                    LoadData();
                    MessageBox.Show("Client updated successfully!", "Success");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error");
            }
        }
    }
}