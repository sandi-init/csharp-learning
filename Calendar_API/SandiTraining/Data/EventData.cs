using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SandiTraining.Models;

namespace SandiTraining.Data
{
    public static class EventData
    {

        public static int count=0;
        public static string organizer="sandiashwi@gmail.com";
        public static string password="cjrjaspnblhftzfv";
        public static string [] formats={"dd-MM-yyyy","yyyy-MM-ddTHH:mm:ss","yyyy-MM-ddTHH:mm","yyyy-MM-ddTHH:mm:ss.mmmZ"};
        public static Dictionary<string,List<Appointment>> meetingData=new Dictionary<string,List<Appointment>>();
        public static Dictionary<string,List<string>> holidays=new Dictionary<string, List<string>>(){
            {"01-01-2023",new List<string>(){"New Year's Day"}},
            {"14-01-2023",new List<string>(){"Makar Sankaranti"}},
            {"15-01-2023",new List<string>(){"Pongal"}},
            {"26-01-2023",new List<string>(){"Republic Day","Vasant Panchami"}},
            {"05-02-2023",new List<string>(){"Guru Ravidas Jayanti","Hazarat Ali's Birthday"}},
            {"15-02-2023",new List<string>(){"Maharishi Dayanand Saraswati Jayanti"}},
            {"18-02-2023",new List<string>(){"Maha Shivaratri-Shivaratri"}},
            {"19-02-2023",new List<string>(){"Shivaji Jayanti"}},
            {"07-03-2023",new List<string>(){"Dolyatra","Holika Dahana"}},
            {"08-03-2023",new List<string>(){"Holi"}},
            {"22-03-2023",new List<string>(){"Chaitra Sukhladi","Ugadi","Gudi Padwa"}},
            {"30-03-2023",new List<string>(){"Rama Navami"}},
            {"04-04-2023",new List<string>(){"Mahavir Jayanti"}},
            {"07-04-2023",new List<string>(){"Good Friday"}},
            {"09-04-2023",new List<string>(){"Easter Day"}},
            {"14-04-2023",new List<string>(){"Vaisakhi"}},
            {"15-04-2023",new List<string>(){"Mesadi - Vaisakhadi"}},
            {"21-04-2023",new List<string>(){"Jamat Ul-Vida (Tentative Date)"}},
            {"22-04-2023",new List<string>(){"Ramzan Id-Eid-ul-Fitar (Tentative Date)"}},
            {"01-05-2023",new List<string>(){"International Worker's Day"}},
            {"05-05-2023",new List<string>(){"Buddha Purnima-Vesak"}},
            {"09-05-2023",new List<string>(){"Birthday of Rabindranath"}},
            {"20-06-2023",new List<string>(){"Rath Yatra"}},
            {"29-07-2023",new List<string>(){"Bakrid-Eid ul-Adha","Muharram-Ashura"}},
            {"15-08-2023",new List<string>(){"Independence Day"}},
            {"16-08-2023",new List<string>(){"Parsi New Year"}},
            {"20-08-2023",new List<string>(){"Vinayaka Chathurthi"}},
            {"29-08-2023",new List<string>(){"Onam"}},
            {"30-08-2023",new List<string>(){"Raksha Bandhan (Rakhi)"}},
            {"06-09-2023",new List<string>(){"Janmashtami (Smarta)"}},
            {"07-09-2023",new List<string>(){"Janmashtami"}},
            {"19-09-2023",new List<string>(){"Ganesh Chaturthi-Vinayaka Chaturthi"}},
            {"28-09-2023",new List<string>(){"Milad un-Nabi-Id-e-Milad"}},
            {"02-10-2023",new List<string>(){"Mahatma Gandhi Jayanti"}},
            {"21-10-2023",new List<string>(){"Maha Saptami"}},
            {"22-10-2023",new List<string>(){"Maha Ashtami"}},
            {"23-10-2023",new List<string>(){"Maha Navami"}},
            {"24-10-2023",new List<string>(){"Dussehra"}},
            {"28-10-2023",new List<string>(){"Maharishi Valmiki Jayanti"}},
            {"01-11-2023",new List<string>(){"Karaka Chaturthi (Karva Chauth)"}},
            {"12-11-2023",new List<string>(){"Naraka Chaturdasi","Diwali-Deepavali"}},
            {"13-11-2023",new List<string>(){"Govardhan Puja"}},
            {"15-11-2023",new List<string>(){"Bhai Duj"}},
            {"19-11-2023",new List<string>(){"Chhat Puja (Pratihar Sashthi-Surya Sashthi)"}},
            {"24-11-2023",new List<string>(){"Guru Tegh Bahadur's Martyrdom Day"}},
            {"27-11-2023",new List<string>(){"Guru Nanak Jayanti"}},
            {"24-12-2023",new List<string>(){"Christmas Eve"}},
            {"25-12-2023",new List<string>(){"Christmas"}}
        };

                // -- new Appointment(){
                // --     Id="sa",
                // --     EventName="daily-standup",
                // --     StartTime=new DateTime(2022,12,22,12,39,12),
                // --     EndTime=new DateTime(2022,12,22,2,39,12),
                // --     EventDescription="Everyone should attend the meeting without absent"
                // -- },
                // -- new Appointment(){
                // --     Id="as",
                // --     EventName="mentor-standup",
                // --     StartTime=new DateTime(2022,12,22,12,39,12),
                // --     EndTime=new DateTime(2022,12,22,2,39,12),
                // --     EventDescription="Mentie should attend"
                // -- }
            

      
    }
}
