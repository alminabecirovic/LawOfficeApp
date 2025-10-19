using System;
using LawOfficeApp.Models;

namespace LawOfficeApp.Services
{
    
    public delegate void DataChangedDelegate(string message);

    
    public class EventMediator
    {
        public event DataChangedDelegate DataChanged;

        public void RaiseDataChanged(string message)
        {
            DataChanged?.Invoke(message);
        }
    }
}