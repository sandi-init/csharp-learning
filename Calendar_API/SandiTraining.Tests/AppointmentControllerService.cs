using Microsoft.AspNetCore.Mvc;
using Moq;
using SandiTraining.Controllers;
using SandiTraining.Business;
using FluentAssertions;
using SandiTraining.Models;
using SandiTraining.CustomExceptions;
using SandiTraining.DataAccess;
using SandiTraining.Data;
using System.Linq;
using SandiTraining.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace SandiTraining.Tests
{
    //Following the easy-to-write and Readable and reliable
    public class AppointmentControllerService : Controller
    {
        private readonly IAppointmentDAL _appointmentsDAL;
        // for DAL
        private readonly IAppointmentBL _appointmentsBL;
        // for BL
        private readonly AppointmentsController sut;
        public AppointmentControllerService()
        {
            _appointmentsDAL = new AppointmentDAL();
            _appointmentsBL = new AppointmentBL(_appointmentsDAL);
            sut = new AppointmentsController(_appointmentsBL);
            
        }
        private void RemoveTestData(DateTime delTime)
        {
            sut.DeleteAppointment(delTime);
        }
        [Fact]
        public void Appointment_Create_And_Retrive_And_Delete_Onsuccess()
        {
            var appointment = new Appointment()
            {
                Id = Guid.NewGuid(),
                EventName = "Meeting",
                StartTime = new DateTime(2023, 01, 22, 15, 08, 0),
                EndTime = new DateTime(2023, 01, 22, 16, 30, 0),
                EventDescription = "must attend",
                receiverMail=new List<string>(){"sandiaswi@gmail.com"}
            };
            string date = appointment.StartTime.ToString("dd-MM-yyyy");
            //Create Appointment
            //base case -- creating appointment successfully
            var result = sut.createAppointment(appointment) as CreatedResult;
            Assert.IsType<CreatedResult>(result);
            var createdData = Assert.IsType<Response>(result?.Value);
            Assert.Equal("Event Succesfully added", createdData.Message);

            //Get Appointmnet By date
            //base case  --- get the appointments by date successfully
            var getvalues = sut.getAppointments(date) as OkObjectResult;
            Assert.IsType<OkObjectResult>(getvalues);
            var added = Assert.IsType<List<Appointment>>(getvalues?.Value);
            Assert.Single(added);
            Assert.Equal(appointment, added[0]);
            Assert.Equal(appointment.receiverMail,added[0].receiverMail);

            //Delete Appointment
            //base case  --- deleting the appointment succesfully
            var deleteResult = sut.DeleteAppointment(appointment.StartTime) as NoContentResult;
            Assert.IsType<NoContentResult>(deleteResult);
            // Assert.Empty(added);
        }
        [Fact]
        public void Appointment_Conflict_OverLapMeet()
        {
            var appointment = new Appointment()
            {
                Id = Guid.NewGuid(),
                EventName = "Meeting",
                StartTime = new DateTime(2023, 01, 23, 01, 00, 0),
                EndTime = new DateTime(2023, 01, 23, 02, 30, 0),
                EventDescription = "must attend",
                receiverMail=new List<string>(){}
            };

            var conflictAppoinment = new Appointment()
            {
                Id = Guid.NewGuid(),
                EventName = "Meeting",
                StartTime = new DateTime(2023, 01, 23, 01, 08, 0),
                EndTime = new DateTime(2023, 01, 23, 02, 30, 0),
                EventDescription = "must attend",
                receiverMail=new List<string>(){}
            };
            // Create Appointment
            // Case-1 Return Conflict adding the appointment at sametime of another appointment
            sut.createAppointment(appointment);
            var result = sut.createAppointment(conflictAppoinment) as ConflictObjectResult;
            Assert.IsType<ConflictObjectResult>(result);
            var response = Assert.IsType<CustomExceptions.MeetingOverLapException>(result?.Value);
            RemoveTestData(appointment.StartTime);
        }
        [Fact]
        public void Appointment_BadRequest_MisMatchDate()
        {
            var appointment = new Appointment()
            {
                Id = Guid.NewGuid(),
                EventName = "Meeting",
                StartTime = new DateTime(2023, 01, 15, 15, 08, 0),
                EndTime = new DateTime(2023, 01, 15, 15, 08, 0),
                EventDescription = "must attend",
                receiverMail=new List<string>(){}
            };
            //Create Appointment
            //Case2 --  Creating the Appointment With same Starttime and Endtime
            var result = sut.createAppointment(appointment) as BadRequestObjectResult;
            Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<CustomExceptions.DateTimeMisMatchException>(result?.Value);
        }
        [Fact]
        public void Appointment_InValidDate_And_Time()
        {
            //Case for all the Valid date check
            string[] dates = { "2023-13-01", "2023-01-12T12:23:00AM" };
            var result = new AppointmentBL().DateValidation(dates);
            Assert.False(result);
            
        }
        [Fact]
        public void Create_Failure(){
            var appointment = new Appointment()
            {
                Id = Guid.NewGuid(),
                EventName = "Meeting",
                StartTime = new DateTime(2023, 01, 25, 03, 08, 0),
                EndTime = new DateTime(2023, 01, 25, 04, 30, 0),
                EventDescription = "must attend",
                receiverMail=new List<string>(){"sandiaswi@gma.in"}
            };
            //Create Appointmnet
            //Case4 --- Create the meeting and paticipant mail id is not  valid format
            var result=sut.createAppointment(appointment) as BadRequestObjectResult;
            Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Given mail id is not Valid",result?.Value);
            RemoveTestData(appointment.StartTime);
            
        }
        [Fact]
        public void GetAppointment_Failed()
        {
            var date = "2023-01-21";
            var date1="03-02-2023";
            //Get Appointment 
            //Case1  -- Given date to get apppointment is not valid format
            var result = sut.getAppointments(date) as BadRequestObjectResult;
            Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<CustomExceptions.InValiDateException>(result?.Value);
            //case2  ---In given date .there is no Appointment present
            var result1=sut.getAppointments(date1) as OkObjectResult;
            Assert.IsType<OkObjectResult>(result1);
            var response=Assert.IsType<List<Appointment>>(result1?.Value);
            Assert.Empty(response);
        }
        [Fact]
        public void DeleteAppointment_Failed()
        {
            //Delete Appointment 
            // Case1 --- Try to delete the non exisiting appointment 
            DateTime startTime = new DateTime(2023, 01, 25, 23, 12, 00);
            var result = sut.DeleteAppointment(startTime) as BadRequestObjectResult;
            Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<CustomExceptions.NoDataFoundException>(result?.Value);
            
            var appointment = new Appointment()
            {
                Id = Guid.NewGuid(),
                EventName = "Meeting",
                StartTime = new DateTime(2023, 01, 29, 09, 08, 0),
                EndTime = new DateTime(2023, 01, 29, 10, 30, 0),
                EventDescription = "must attend",
                receiverMail=new List<string>(){"sandiaswi@gmail.com"}
            };
            sut.createAppointment(appointment);
            DateTime startTime1=new DateTime(2023,01,29,02,15,00);
            var result2=sut.DeleteAppointment(startTime1) as BadRequestObjectResult;
            Assert.IsType<CustomExceptions.NoDataFoundException>(result?.Value);
            RemoveTestData(appointment.StartTime);
        }
        [Fact]
        public void UpdateAppointment_Success()
        {
            var appointment = new Appointment()
            {
                Id = Guid.NewGuid(),
                EventName = "Meeting",
                StartTime = new DateTime(2023, 02, 02, 01, 08, 0),
                EndTime = new DateTime(2023, 02, 02, 02, 30, 0),
                EventDescription = "must attend",
                receiverMail=new List<string>(){}
            };
            var updateAppointment=new Appointment()
            {
                Id = appointment.Id,
                EventName = "Meeting",
                StartTime = new DateTime(2023, 02, 02, 03, 08, 0),
                EndTime = new DateTime(2023, 02, 02, 04, 30, 0),
                EventDescription = "must attend",
                receiverMail=new List<string>(){}
            };
            var updateAppointment1 = new Appointment()
            {
                Id = updateAppointment.Id,
                EventName = "zoom Call",
                StartTime = new DateTime(2023, 02, 03, 01, 08, 0),
                EndTime = new DateTime(2023, 02, 03, 01, 30, 0),
                EventDescription = "must attend",
                receiverMail=new List<string>(){}
            };
            //Update Appointment

            string date = appointment.StartTime.ToString("dd-MM-yyyy");
            string update = updateAppointment.StartTime.ToString("dd-MM-yyyy");
            string update1= updateAppointment1.StartTime.ToString("dd-MM-yyyy");
            sut.createAppointment(appointment);

            //Base Case  --- Update the Appointment  successfully
            var result = sut.UpdateAppointment(date, updateAppointment);
            Assert.IsType<OkObjectResult>(result);
            var get = sut.getAppointments(update) as OkObjectResult;
            var value = Assert.IsType<List<Appointment>>(get?.Value);
            Assert.Equal(value[0].Id, updateAppointment.Id);
            Assert.Equal(updateAppointment.StartTime,value[0].StartTime);
            Assert.Equal(updateAppointment.EndTime,value[0].EndTime);
            Assert.Equal(updateAppointment.EventDescription,value[0].EventDescription);
            var result1=sut.UpdateAppointment(date,updateAppointment1);
            Assert.IsType<OkObjectResult>(result);
            RemoveTestData(updateAppointment1.StartTime);
        }
        [Fact]
        public void updateAppointment_Failed()
        {
            var appointment = new Appointment()
            {
                Id = Guid.NewGuid(),
                EventName = "Meeting",
                StartTime = new DateTime(2023, 02, 05, 07, 08, 0),
                EndTime = new DateTime(2023, 02, 05, 08, 30, 0),
                EventDescription = "must attend",
                receiverMail=new List<string>(){"welcome@gmail.com"}
            };
            var appointment2 = new Appointment()
            {
                Id = Guid.NewGuid(),
                EventName = "Meeting",
                StartTime = new DateTime(2023, 02, 07, 15, 08, 0),
                EndTime = new DateTime(2023, 02, 07, 16, 30, 0),
                EventDescription = "must attend",
                receiverMail=new List<string>(){"welcome@gmail.com"}
            };
            
            var updateAppointment = new Appointment()
            {
                Id = appointment.Id,
                EventName = "zoom Call",
                StartTime = new DateTime(2023, 02, 07, 15, 08, 0),
                EndTime = new DateTime(2023, 02, 07, 16, 10, 0),
                EventDescription = "must attend",
                receiverMail=new List<string>(){}
            };
            
            var updateAppointment2 = new Appointment()
            {
                Id = appointment2.Id,
                EventName = "zoom Call",
                StartTime = new DateTime(2023, 02, 17, 15, 18, 0),
                EndTime = new DateTime(2023, 02, 17, 15, 18, 0),
                EventDescription = "must attend",
                receiverMail=new List<string>(){}
            };
            var updateAppointment3 = new Appointment()
            {
                Id = Guid.NewGuid(),
                EventName = "zoom Call",
                StartTime = new DateTime(2023, 01, 15, 19, 18, 0),
                EndTime = new DateTime(2023, 01, 15, 20, 10, 0),
                EventDescription = "must attend",
                receiverMail=new List<string>(){}
            };
            string date = appointment.StartTime.ToString("dd-MM-yyyy");
            string update = updateAppointment.StartTime.ToString("dd-MM-yyyy");
            sut.createAppointment(appointment);
            sut.createAppointment(appointment2);

            //Update Appointment
            //Case 2 ---- overlap with  another on another day appointment  -conflict
            var overLapResult = sut.UpdateAppointment(date, updateAppointment) as ConflictObjectResult;
            Assert.IsType<ConflictObjectResult>(overLapResult);
            Assert.IsType<CustomExceptions.MeetingOverLapException>(overLapResult?.Value);

            //Upadte Appointment
            //Case 3  ----- Updated StartTime and EndTime be the same -badrequest
            var mismatchResult = sut.UpdateAppointment(date, updateAppointment2) as BadRequestObjectResult;
            Assert.IsType<BadRequestObjectResult>(mismatchResult);
            Assert.IsType<CustomExceptions.DateTimeMisMatchException>(mismatchResult?.Value);

            //Update Appointment 
            //Case 5   --- updated appointment Id is not found
            var NotFoundIdResult=sut.UpdateAppointment(date,updateAppointment3) as BadRequestObjectResult;
            Assert.IsType<BadRequestObjectResult>(NotFoundIdResult);
            var response=Assert.IsType<CustomExceptions.NotIdFoundException>(NotFoundIdResult?.Value);
            Assert.Equal("Meeting ID is not found",response.ErrorMessage);
            //verifying the any data is Updated
            var getValues = sut.getAppointments(date) as OkObjectResult;
            var data = Assert.IsType<List<Appointment>>(getValues?.Value);
            Assert.Single(data); // remove
            Assert.Equal(appointment, data[0]);
            RemoveTestData(appointment.StartTime);
            RemoveTestData(appointment2.StartTime);
        }
        [Fact]
        public void GetHolidays_Success_Empty()
        {
            //Get Holiday
            // Base Case --- get the holiday data for given day successfully
            string date = "01-01-2023";
            var result = sut.GetHolidays(date) as OkObjectResult;
            Assert.IsType<OkObjectResult>(result);
            var holiday = Assert.IsType<List<string>>(result?.Value);
            Assert.Equal("New Year's Day", holiday[0]);
            
            //Case 1  ---return the empty list because no holiday data present on that day
            string date1="02-01-2023";
            var result1=sut.GetHolidays(date1) as OkObjectResult;
            Assert.IsType<OkObjectResult>(result);
            var response=Assert.IsType<List<string>>(result1?.Value);
            Assert.Empty(response);
        }

        [Fact]
        public void Search_Success_And_Failure()
        {
            //Saerch the Appointment by  -- NAME --   and  -- DATE --
            var appointment = new Appointment()
            {
                Id = new Guid("9245fe4a-d402-451c-b9ed-9c1a03447404"),
                EventName = "catchup",
                StartTime = new DateTime(2023, 08, 01, 01, 08, 0),
                EndTime = new DateTime(2023, 08, 01, 06, 30, 0),
                EventDescription = "must attend",
                receiverMail=new List<string>(){"welcome@gmail.com"}
            };
            string data = "catch", type = "Name";
            string date = "01-08-2023", typ = "date";
            string failDate = "11-01-2023", faildata = "hello", InvalidDate = "2023-31-01";

            sut.createAppointment(appointment);
            //Base case ---     searched by Name  ----  Success
            var result = sut.Search(data, type) as OkObjectResult;
            Assert.IsType<OkObjectResult>(result);
            var searchedData = Assert.IsType<List<Appointment>>(result?.Value);
            Assert.Equal(appointment.Id, searchedData[0].Id);
            //Base case  ---     Searched By date   ---- Success

            var dateResult = sut.Search(date, typ) as OkObjectResult;
            Assert.IsType<OkObjectResult>(dateResult);
            var searchedDate = Assert.IsType<List<Appointment>>(result?.Value);
            Assert.Equal(appointment.Id, searchedDate[0].Id);

            //Case 1 ---  Searched By Name   ---- Failure
            var failresult = sut.Search(faildata, type) as NotFoundObjectResult;
            Assert.IsType<NotFoundObjectResult>(failresult);
            var failedData = Assert.IsType<string>(failresult?.Value);
            Assert.Equal("No Data result Found", failedData);

            //Case 1 ---  Searched By Date   ----- Failure
            var failDateResult = sut.Search(failDate, typ) as NotFoundObjectResult;
            Assert.IsType<NotFoundObjectResult>(failDateResult);
            var failedDate = Assert.IsType<string>(failDateResult?.Value);
            Assert.Equal("No Data result Found", failedDate);

            //Case 2 ---- Searched By date  ------ failure Invalid date

            var InvalidDateResult = sut.Search(InvalidDate, typ) as BadRequestObjectResult;
            Assert.IsType<BadRequestObjectResult>(InvalidDateResult);
            var InvalidResponse = Assert.IsType<CustomExceptions.InValiDateException>(InvalidDateResult?.Value);
            Assert.Equal("Invalid Format of date", InvalidResponse.ErrorMessage);
            RemoveTestData(appointment.StartTime);
            
        }

        [Fact]
        public void GetByRange_Success_Empty()
        {
            var appointment = new Appointment()
            {
                Id = Guid.NewGuid(),
                EventName = "Meeting",
                StartTime = new DateTime(2023, 09, 01, 01, 00, 0),
                EndTime = new DateTime(2023, 09, 01, 02, 30, 0),
                EventDescription = "must attend",
                receiverMail=new List<string>(){"welcome@gmail.com"}
            };
            var appointment2 = new Appointment()
            {
                Id = Guid.NewGuid(),
                EventName = "presentation",
                StartTime = new DateTime(2023, 09, 15, 04, 00, 0),
                EndTime = new DateTime(2023, 09, 15, 04, 30, 0),
                EventDescription = "must attend",
                receiverMail=new List<string>(){"welcome@gmail.com"}
            };
            //Get the Appointment for given range
            sut.createAppointment(appointment);
            sut.createAppointment(appointment2);
            DateTime endRange = new DateTime(2023, 09, 30, 00, 00, 00);
            //Base Case ---- return the list of appointments for given range
            var result = sut.RangeEvents(endRange) as OkObjectResult;
            Assert.IsType<OkObjectResult>(result);
            var rangeData = Assert.IsType<List<Appointment>>(result.Value);
            // Assert.Empty(rangeData);
            var checkRange1=sut.GetAppointmentById(appointment.Id) as OkObjectResult;
            var checkRange2=sut.GetAppointmentById(appointment2.Id) as OkObjectResult;
            var response1=Assert.IsType<Appointment>(checkRange1?.Value);
            var response2=Assert.IsType<Appointment>(checkRange2?.Value);
            Assert.InRange(response1.StartTime ,appointment.StartTime, endRange);
            Assert.InRange(response2.StartTime,appointment2.StartTime,  endRange);
            RemoveTestData(appointment.StartTime);
            RemoveTestData(appointment2.StartTime);
            
        }

        [Fact]
        public void ChecK_Email(){
            var appointment = new Appointment()
            {
                Id = Guid.NewGuid(),
                EventName = "Meeting",
                StartTime = new DateTime(2023, 07, 07, 15, 08, 0),
                EndTime = new DateTime(2023, 07, 07, 16, 30, 0),
                EventDescription = "must attend",
                receiverMail=new List<string>(){"sundar@gmail.com"}
            };
            var appointment2 = new Appointment()
            {
                Id = Guid.NewGuid(),
                EventName = "Project Demo",
                StartTime = new DateTime(2023, 07, 15, 06, 08, 0),
                EndTime = new DateTime(2023, 07, 15, 08, 30, 0),
                EventDescription = "must attend",
                receiverMail=new List<string>(){"sandiaswi@gma.in"}
            };
            //Sending Email 
            //Base Case  -- successfully send to the email
            var result=new AppointmentDAL().SendMail(appointment.receiverMail,appointment);
            Assert.True(result);
            var InvalidResponse=sut.createAppointment(appointment2) as BadRequestObjectResult;
            Assert.IsType<BadRequestObjectResult>(InvalidResponse);
            Assert.Equal("Given mail id is not Valid",InvalidResponse.Value);
            //Case 1 --- Mail id is not correct
            // try{
            //     var InvalidResponse=new AppointmentDAL().SendMail(appointment2.receiverMail,appointment2);
            // }
            // catch(Exception ex){
            //     Assert.Equal("Given mail id is not Valid",ex.Message);
            // }
            RemoveTestData(appointment.StartTime);
            RemoveTestData(appointment2.StartTime);
            
        }
        [Fact]
        public void Create_Two_Appointments(){
            var appointment = new Appointment()
            {
                Id = Guid.NewGuid(),
                EventName = "Meeting",
                StartTime = new DateTime(2023, 06, 01, 11, 08, 0),
                EndTime = new DateTime(2023, 06, 01, 12, 30, 0),
                EventDescription = "must attend",
                receiverMail=new List<string>(){"welcome@gmail.com"}
            };
            var appointment2 = new Appointment()
            {
                Id = Guid.NewGuid(),
                EventName = "catchup",
                StartTime = new DateTime(2023, 06, 01, 05, 08, 0),
                EndTime = new DateTime(2023, 06, 01, 06, 30, 0),
                EventDescription = " menties must attend",
                receiverMail=new List<string>(){"welcome@gmail.com"}
            };
            var appointment3 = new Appointment()
            {
                Id = Guid.NewGuid(),
                EventName = "catchup",
                StartTime = new DateTime(2023, 06, 01, 08, 08, 0),
                EndTime = new DateTime(2023, 06, 01, 09, 30, 0),
                EventDescription = " menties must attend",
                receiverMail=new List<string>(){"welcome@gmail.com"}
            };
            string date=appointment2.StartTime.ToString("dd-MM-yyyy");
            sut.createAppointment(appointment);
            //Create Appointment
            //Base Case 2-checking add multiple meeting on same day with no conflict
            var result=sut.createAppointment(appointment2) as CreatedResult;
            Assert.IsType<CreatedResult>(result);
            var getMeetings=sut.getAppointments(date) as OkObjectResult;
            var response=Assert.IsType<List<Appointment>>(getMeetings?.Value);
            // Assert.Equal(2,response.Count);
            sut.createAppointment(appointment3);
            //Delete Appointment
            //case2-delete the meet binary search handle the one corner case --- midvalue<startvalue
            var deleteResult=sut.DeleteAppointment(appointment.StartTime) as NoContentResult;
            Assert.IsType<NoContentResult>(deleteResult);
            sut.createAppointment(appointment);
            //DeleteAppointment
            //case3-delete the meet binary search handle the one corner case --- midvalue>startvalue
            var deleteResult1=sut.DeleteAppointment(appointment2.StartTime) as NoContentResult;
            Assert.IsType<NoContentResult>(deleteResult1);
            RemoveTestData(appointment.StartTime);
            RemoveTestData(appointment3.StartTime);
            
        }
        [Fact]
        public void Create_Update_MisMatchDate_Case3_And_InvalidDate(){
            var appointment = new Appointment()
            {
                Id = Guid.NewGuid(),
                EventName = "Meeting",
                StartTime = new DateTime(2023, 01, 15, 15, 08, 0),
                EndTime = new DateTime(2023, 01, 15, 14, 30, 0),
                EventDescription = "must attend",
                receiverMail=new List<string>(){}
            };
            var appointment2 = new Appointment()
            {
                Id = Guid.NewGuid(),
                EventName = "Meeting",
                StartTime = new DateTime(2023, 10, 01, 15, 08, 0),
                EndTime = new DateTime(2023, 10, 01, 16, 30, 0),
                EventDescription = "must attend",
                receiverMail=new List<string>(){}
            };
            var updateAppointment=new Appointment()
            {
                Id = appointment.Id,
                EventName = "Meeting",
                StartTime = new DateTime(2023, 01, 15, 17, 08, 0),
                EndTime = new DateTime(2023, 01, 15, 16, 30, 0),
                EventDescription = "must attend",
                receiverMail=new List<string>(){}
            };
            var updateAppointment1=new Appointment()
            {
                Id = appointment2.Id,
                EventName = "Meeting",
                StartTime = new DateTime(2023, 01, 15, 17, 08, 0),
                EndTime = new DateTime(2023, 01, 15, 18, 30, 0),
                EventDescription = "must attend",
                receiverMail=new List<string>(){}
            };
            //Create Appointment 
            //Case 3  ---  Create the appointment with startTime greater than endTime 
            string date=appointment2.StartTime.ToString("dd-MM-yyyy");
            var result =sut.createAppointment(appointment) as BadRequestObjectResult;
            Assert.IsType<BadRequestObjectResult>(result);
            sut.createAppointment(appointment2);
            //Update Appointment  
            //Case 3  ---- update  the appointment with startTime greater than endTime
            var updateResult=sut.UpdateAppointment(date,updateAppointment) as BadRequestObjectResult;
            Assert.IsType<BadRequestObjectResult>(updateResult);

            //Update Appointment
            //Case 4  --- upadte appointment date is not valid format
            string date1=appointment2.StartTime.ToString("yyyy-MM-dd");
            var upadteFail=sut.UpdateAppointment(date1,updateAppointment1) as BadRequestObjectResult;
            Assert.IsType<BadRequestObjectResult>(upadteFail);
            var response=Assert.IsType<CustomExceptions.InValiDateException>(upadteFail?.Value);
            Assert.Equal("Invalid Format of date",response.ErrorMessage);
            RemoveTestData(appointment2.StartTime);
        }

        [Fact]
        public void GetAppointmentById_Success_Empty(){
            var appointment = new Appointment()
            {
                Id = Guid.NewGuid(),
                EventName = "Meeting",
                StartTime = new DateTime(2023, 01, 30, 15, 08, 0),
                EndTime = new DateTime(2023, 01, 30, 16, 30, 0),
                EventDescription = "must attend",
                receiverMail=new List<string>(){"welcome@gmail.com"}
            };
            sut.createAppointment(appointment);
            var result=sut.GetAppointmentById(appointment.Id) as OkObjectResult;
            Assert.IsType<OkObjectResult>(result);
            Assert.Equivalent(appointment,result.Value);
            RemoveTestData(appointment.StartTime);
            var failresult=sut.GetAppointmentById(Guid.NewGuid()) as NotFoundObjectResult;
            Assert.IsType<NotFoundObjectResult>(failresult);
            Assert.IsType<CustomExceptions.NotIdFoundException>(failresult.Value); 
        }

        [Fact]
        public void Create_PastDate(){
            var appointment = new Appointment()
            {
                Id = Guid.NewGuid(),
                EventName = "Meeting",
                StartTime = new DateTime(2023, 01, 10, 15, 08, 0),
                EndTime = new DateTime(2023, 01, 10, 16, 30, 0),
                EventDescription = "must attend",
                receiverMail=new List<string>(){"welcome@gmail.com"}
            };

            var result=sut.createAppointment(appointment) as ConflictObjectResult;
            Assert.IsType<ConflictObjectResult>(result);
            Assert.IsType<CustomExceptions.PastDateException>(result?.Value);
        }
        
    }
}