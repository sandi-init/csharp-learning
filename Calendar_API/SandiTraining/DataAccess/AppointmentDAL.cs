using SandiTraining.Models;
using SandiTraining.Data;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using System.Net.Mail;

namespace SandiTraining.DataAccess
{
    public class AppointmentDAL:IAppointmentDAL
    {
        public AppointmentDAL(){}
        public bool CreateAppointment(string date,Appointment eventData){
            List<Appointment> temp=new List<Appointment>(){};
            if(EventData.meetingData.ContainsKey(date)){
                EventData.meetingData[date].Add(eventData);
                EventData.meetingData[date]=EventData.meetingData[date]
                .OrderBy(p=>p.StartTime).ToList();
                if(eventData.receiverMail.Count>0){
                    SendMail(eventData.receiverMail,eventData);
                }
                
            }
            else{
                temp.Add(eventData);
                EventData.meetingData.Add(date,temp);
                if(eventData.receiverMail.Count>0){
                    SendMail(eventData.receiverMail,eventData);
                }
            } 
            return true; 
        }

        public List<Appointment> GetAppointments(string date)
        {
            if(EventData.meetingData.ContainsKey(date)){
                return EventData.meetingData[date];
            }else{
                return new List<Appointment>();
            }
        }

        public bool DeleteAppointment(string date,DateTime startTime)
        {
            if(EventData.meetingData.ContainsKey(date)){
                int start=0;
                int end=EventData.meetingData[date].Count;
                while(start<end){
                    int mid=(start+end)/2;
                    if(EventData.meetingData[date][mid].StartTime==startTime){
                        EventData.meetingData[date].Remove(EventData.meetingData[date][mid]);
                        if(EventData.meetingData[date].Count==0){
                            EventData.meetingData.Remove(date);
                        }
                        return true;
                    }
                    else if(EventData.meetingData[date][mid].StartTime>startTime){
                        end=mid;
                    }
                    else{
                        start=mid;
                    }
                }
            }
            return false;    
        }

        public bool UpdateAppointment(string date, Appointment updateData)
        {
            var st=updateData.StartTime;
            string updateDate=st.ToString("dd-MM-yyyy");
            if(EventData.meetingData.ContainsKey(date)){
                List<Appointment> temp=new List<Appointment>(){};
                foreach(Appointment obj in EventData.meetingData[date]){
                    if(obj.Id==updateData.Id && date.Equals(updateDate)){
                        obj.EventName=updateData.EventName;
                        obj.StartTime=updateData.StartTime;
                        obj.EndTime=updateData.EndTime;
                        obj.EventDescription=updateData.EventDescription;
                        obj.receiverMail=updateData.receiverMail;
                        EventData.meetingData[date]=EventData.meetingData[date].OrderBy(p=>p.StartTime).ToList();
                        // if(obj.receiverMail.Count>0){
                        SendMail(obj.receiverMail,obj);
                        // }
                        return true;
                    }
                    else if(obj.Id==updateData.Id && date.Equals(updateDate)==false){
                        CreateAppointment(updateDate,updateData);
                        EventData.meetingData[date].Remove(obj);
                        if(EventData.meetingData[date].Count==0){
                            EventData.meetingData.Remove(date);
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        public List<string> HolidayData(string date){
            return EventData.holidays[date];
        }

        public List<Appointment> SearchEvents(string search,string type){
            List<Appointment> SearchedData=new List<Appointment>(){};
            if(type.Equals("date")){
                return GetAppointments(search);
            }
            else if(type.Equals("Name")){
                var queryData=EventData.meetingData.SelectMany(meet=>meet.Value)
                            .Where(meet=>meet.EventName.ToLower().Contains(search.ToLower()))
                            .Select(meet=>new Appointment{EventName= meet.EventName,StartTime= meet.StartTime,EndTime= meet.EndTime,EventDescription= meet.EventDescription,Id=meet.Id});
                foreach(Appointment obj in queryData){
                    SearchedData.Add(obj);
                }
            }
            return SearchedData;
        }
        public List<Appointment> EventsTimeRange(DateTime endRange){
            List<Appointment> SearchedData=new List<Appointment>(){};
            var queryData=EventData.meetingData.SelectMany(meet=>meet.Value)
                    .Where(meet=>meet.StartTime<endRange)
                    .Select(meet=>new Appointment{EventName= meet.EventName,StartTime= meet.StartTime,EndTime= meet.EndTime,EventDescription= meet.EventDescription,Id=meet.Id});
                foreach(Appointment obj in queryData){
                    SearchedData.Add(obj);
                }
            
            return SearchedData;
        }

        public bool SendMail(List<string> receiverMail, Appointment eventData)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(EventData.organizer));
            email.To.Add(MailboxAddress.Parse(EventData.organizer));
            email.Subject = "Remainder :  " + eventData.EventName;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = "You are invited to this event\n" +"Oragnizer:Santhosh Kumar s\n"+ eventData.EventName+"\n"+eventData.StartTime+ " - " + eventData.EndTime +"\n" +eventData.EventDescription };
            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate(EventData.organizer, EventData.password);
            foreach (string receiver in receiverMail)
            {
                if(receiver.Contains("@gmail.com")){
                    email.Cc.Add(MailboxAddress.Parse(receiver));   
                } 
                else{
                    throw new Exception("Given mail id is not Valid"); 
                } 
            }
            smtp.Send(email);
            smtp.Disconnect(true);
            return true;
        }


        public List<Appointment> GetAppointmentById(Guid id){
            var responseData=EventData.meetingData.SelectMany(meet=>meet.Value)
                    .Where(meet=>meet.Id==id)
                    .Select(meet=>new Appointment
                    {EventName= meet.EventName,StartTime= meet.StartTime,EndTime= meet.EndTime,
                    EventDescription= meet.EventDescription,Id=meet.Id,receiverMail=meet.receiverMail}).ToList();
            return responseData;
        }
        /*
        public bool deleteAppointment(string date,Guid id){
            if(EventData.meetingData.ContainsKey(date)){
                foreach(Appointment obj in EventData.meetingData[date]){
                    if(obj.Id==id){
                        EventData.meetingData[date].remove(obj);
                        return true;
                    }
                }
            }
            return false;
        }
        */
    }
}