using System;
using System.Collections.Generic;
using System.Linq;
using LawOfficeApp.Data;
using LawOfficeApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LawOfficeApp.Services
{
    public class ClientService
    {
        private readonly LawOfficeDbContext _context;
        private readonly EventMediator _eventMediator;

        public ClientService(LawOfficeDbContext context, EventMediator eventMediator)
        {
            _context = context;
            _eventMediator = eventMediator;
        }

        // Get all clients
        public List<Client> GetAllClients()
        {
            try
            {
                return _context.Clients
                    .Include(c => c.Cases)
                    .ToList();
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error loading clients: {ex.Message}");
                return new List<Client>();
            }
        }

        // Get client by ID
        public Client GetClientById(int id)
        {
            try
            {
                return _context.Clients
                    .Include(c => c.Cases)
                    .FirstOrDefault(c => c.Id == id);
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error loading client: {ex.Message}");
                return null;
            }
        }

        // Add new client
        public bool AddClient(Client client)
        {
            try
            {
                _context.Clients.Add(client);
                _context.SaveChanges();

                _eventMediator.RaiseDataChanged($"Client {client.GetFullName()} added successfully");
                return true;
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error adding client: {ex.Message}");
                return false;
            }
        }

        // Update client
        public bool UpdateClient(int id, string newEmail = null, string newPhone = null)
        {
            try
            {
                var client = _context.Clients.Find(id);
                if (client == null)
                {
                    _eventMediator.RaiseDataChanged("Client not found");
                    return false;
                }

                if (!string.IsNullOrEmpty(newEmail))
                    client.Email = newEmail;

                if (!string.IsNullOrEmpty(newPhone))
                    client.PhoneNumber = newPhone;

                _context.SaveChanges();

                _eventMediator.RaiseDataChanged($"Client {client.GetFullName()} updated");
                return true;
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error updating client: {ex.Message}");
                return false;
            }
        }

        // Delete client
        public bool DeleteClient(int id)
        {
            try
            {
                var client = _context.Clients.Find(id);
                if (client == null)
                {
                    _eventMediator.RaiseDataChanged("Client not found");
                    return false;
                }

                _context.Clients.Remove(client);
                _context.SaveChanges();

                _eventMediator.RaiseDataChanged($"Client {client.GetFullName()} deleted");
                return true;
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error deleting client: {ex.Message}");
                return false;
            }
        }

        // Search clients
        public List<Client> SearchClients(string searchTerm)
        {
            try
            {
                return _context.Clients
                    .Include(c => c.Cases)
                    .Where(c => c.FirstName.Contains(searchTerm) ||
                               c.LastName.Contains(searchTerm) ||
                               c.Email.Contains(searchTerm))
                    .ToList();
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error searching clients: {ex.Message}");
                return new List<Client>();
            }
        }
    }
}