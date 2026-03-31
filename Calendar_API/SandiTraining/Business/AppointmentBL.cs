using SandiTraining.Models;
using SandiTraining.Data;
using SandiTraining.DataAccess;
using SandiTraining.CustomExceptions;
using  System.Globalization;
namespace SandiTraining.Business
{
    public class AppointmentBL:IAppointmentBL
    {
        private const string Format = "yyyy-MM-ddTHH:mm:ss";
        private readonly IAppointmentDAL _appointmentDAL;
        public AppointmentBL(){}
        public AppointmentBL(IAppointmentDAL appointmentDAL){
            _appointmentDAL=appointmentDAL;
        }
        public DateTime current=new DateTime();
        public bool DateValidation(string[] checkDates){
            foreach(string date in checkDates){
                if(DateTime.TryParseExact(date,EventData.formats,CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out DateTime temp)){
                        continue;
                }
                else{
                    return false;
                }
            }
            return true;
        }
        public bool TimeValidation(string date,Appointment appointment){
            if(appointment.StartTime == appointment.EndTime){
                throw new CustomExceptions.DateTimeMisMatchException(){StatusCode=200,ErrorMessage="no"};
            }
            else if(appointment.StartTime>appointment.EndTime){
                throw new CustomExceptions.DateTimeMisMatchException();
            }
            else if(EventData.meetingData.Count>0 && EventData.meetingData.ContainsKey(date)){
                if(EventData.meetingData[date].Count>0){
                    foreach(Appointment obj in EventData.meetingData[date]){
                        if(appointment.Id==obj.Id){
                            continue;
                        }
                        else if(obj.StartTime < appointment.EndTime &&  obj.EndTime>appointment.StartTime){
                            throw new CustomExceptions.MeetingOverLapException();
                        }
                }   
            }
            }
            return true;
        }
        public bool NewAppointment(string date,Appointment eventData){
            DateTime tempSt=eventData.StartTime,tempEt=eventData.EndTime;
            string [] dates={date,tempSt.ToString(Format),tempEt.ToString(Format)};
            bool IsValidDate=DateValidation(dates);
            bool IsTimeConflict=TimeValidation(date,eventData);
            if(eventData.StartTime>=DateTime.Now){
                return _appointmentDAL.CreateAppointment(date,eventData);  
            }
            else{
                throw new CustomExceptions.PastDateException();
            } 
        }

        public List<Appointment> RetriveAppointments(string date)
        {
            string [] dates={date};
            bool IsValidDate=DateValidation(dates);
            if(IsValidDate){
                return _appointmentDAL.GetAppointments(date);
            }
            else{
                throw new CustomExceptions.InValiDateException();
            }
        }

        public bool RemoveAppointmnet(string date, DateTime startTime)
        {  
            return _appointmentDAL.DeleteAppointment(date,startTime);
        }

        public bool UpdateAppointment(string date, Appointment updateData)
        {
            var updateDate=updateData.StartTime.ToString("dd-MM-yyyy");
            string [] dates={date,updateDate};
            bool IsValidDate=DateValidation(dates);
            bool IsTimeConflict=TimeValidation(updateDate,updateData);
            
            if(IsValidDate && IsTimeConflict){
                return _appointmentDAL.UpdateAppointment(date,updateData);
            }
            else{
                throw new CustomExceptions.InValiDateException();
            }
        }

        public List<string> getHolidays(string date){
            List<string> holidayList= new List<string>(){};
            if(EventData.holidays.ContainsKey(date)){
                return _appointmentDAL.HolidayData(date);
            }
            else{
                return holidayList;
            }
        }
        public List<Appointment> SearchEvents(string search,string type){
            if(type.Equals("date")){
               string [] dates={search};
                bool IsValidDate=DateValidation(dates);
                if(IsValidDate){
                    return _appointmentDAL.SearchEvents(search,type);
                }
            }
            if(type.Equals("Name")){
                return _appointmentDAL.SearchEvents(search,type);
            }

            else{
                throw new CustomExceptions.InValiDateException();
            }
        }
        public List<Appointment> EventsTimeRange(DateTime endRange){
            
            return _appointmentDAL.EventsTimeRange(endRange);
        }

        public Appointment getAppointmentById(Guid id)
        {
            var responseData=_appointmentDAL.GetAppointmentById(id);
            if(responseData.Count>0){
                return responseData[0];
            }
            else{
                throw new CustomExceptions.NotIdFoundException();
            }
        }
    }
}