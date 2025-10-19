using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LawOfficeApp.Data;
using LawOfficeApp.Models;
using LawOfficeApp.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LawOfficeApp.Services
{
    public class ClientService
    {
        private readonly LawOfficeDbContext _context;
        private readonly EventMediator _eventMediator;
        private readonly IRepository<Client> _clientRepository;

        public ClientService(LawOfficeDbContext context, EventMediator eventMediator,
                           IRepository<Client> clientRepository)
        {
            _context = context;
            _eventMediator = eventMediator;
            _clientRepository = clientRepository;
        }

        // Get all clients
        public async Task<List<Client>> GetAllClients()
        {
            try
            {
                return await _context.Clients
                    .Include(c => c.Cases)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error loading clients: {ex.Message}");
                return new List<Client>();
            }
        }

        // Get client by ID
        public async Task<Client> GetClientById(int id)
        {
            try
            {
                return await _context.Clients
                    .Include(c => c.Cases)
                    .FirstOrDefaultAsync(c => c.Id == id);
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error loading client: {ex.Message}");
                return null;
            }
        }

        // Add new client
        public async Task<bool> AddClient(Client client)
        {
            try
            {
                await _clientRepository.AddAsync(client);

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
        public async Task<bool> UpdateClient(int id, string newEmail = null, string newPhone = null)
        {
            try
            {
                var client = await _context.Clients.FindAsync(id);
                if (client == null)
                {
                    _eventMediator.RaiseDataChanged("Client not found");
                    return false;
                }

                if (!string.IsNullOrEmpty(newEmail))
                    client.Email = newEmail;

                if (!string.IsNullOrEmpty(newPhone))
                    client.PhoneNumber = newPhone;

                await _clientRepository.UpdateAsync(client);

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
        public async Task<bool> DeleteClient(int id)
        {
            try
            {
                var client = await _context.Clients.FindAsync(id);
                if (client == null)
                {
                    _eventMediator.RaiseDataChanged("Client not found");
                    return false;
                }

                _context.Clients.Remove(client);
                await _context.SaveChangesAsync();

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
        public async Task<List<Client>> SearchClients(string searchTerm)
        {
            try
            {
                return await _context.Clients
                    .Include(c => c.Cases)
                    .Where(c => c.FirstName.Contains(searchTerm) ||
                               c.LastName.Contains(searchTerm) ||
                               c.Email.Contains(searchTerm))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _eventMediator.RaiseDataChanged($"Error searching clients: {ex.Message}");
                return new List<Client>();
            }
        }
    }
}