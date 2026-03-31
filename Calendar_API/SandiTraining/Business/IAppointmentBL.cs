using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SandiTraining.Models;
using SandiTraining.Data;
namespace SandiTraining.Business
{
    public interface IAppointmentBL
    {
        public bool NewAppointment(string date,Appointment eventData);
        public List<Appointment> RetriveAppointments(string date);
        public Appointment getAppointmentById(Guid id);
        public bool RemoveAppointmnet(string date,DateTime startTime);
        public bool UpdateAppointment(string date,Appointment updateData);
        public List<string> getHolidays(string date);
        public List<Appointment> SearchEvents(string search,string type);
        public List<Appointment> EventsTimeRange(DateTime endRange);
    }
}