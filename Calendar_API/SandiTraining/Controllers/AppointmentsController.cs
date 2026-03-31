using SandiTraining.Models;
using Microsoft.AspNetCore.Mvc;
using SandiTraining.Business;
using SandiTraining.Enums;
using SandiTraining.Extensions;
using SandiTraining.CustomExceptions;
using SandiTraining.Data;
namespace SandiTraining.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentBL _appointmentBL;
        public AppointmentsController(IAppointmentBL appointmentBL)
        {
            _appointmentBL=appointmentBL;
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Response))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest,Type =typeof(Response))]

        public IActionResult createAppointment(Appointment data){
           string date=data.StartTime.ToString("dd-MM-yyyy");
           try{
            bool eventData=  _appointmentBL.NewAppointment(date,data);
            return Created("~api/appointments",new Response(){StatusCode=(int)Statuscodes.ReturnCodeCreated,Message=ResponseMessage.Created.GetDescription()});  
           }
            catch(CustomExceptions.DateTimeMisMatchException)
            {
                return BadRequest(new CustomExceptions.DateTimeMisMatchException(){StatusCode=(int)Statuscodes.ReturnCodeBadRequest,ErrorMessage=ResponseMessage.MismatchDate.GetDescription()});
            }
             catch(CustomExceptions.MeetingOverLapException){
                return Conflict(new CustomExceptions.MeetingOverLapException(){StatusCode=(int)Statuscodes.ReturnCodeConflict,ErrorMessage=ResponseMessage.DateTimeOverLap.GetDescription()});
            }
            catch(CustomExceptions.PastDateException){
                return Conflict(new CustomExceptions.PastDateException(){StatusCode=(int)Statuscodes.ReturnCodeConflict,ErrorMessage="Meeting can't to be added in the past time"});
            }
            catch(Exception ex){
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("{date}")]
        [ProducesResponseType(StatusCodes.Status200OK,Type=typeof(List<Appointment>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest,Type =typeof(Response))] 
        public IActionResult getAppointments(string date){
            try{
                List<Appointment> targetData=_appointmentBL.RetriveAppointments(date);
                return Ok(targetData);
            }
            catch(CustomExceptions.InValiDateException){
                return BadRequest(new CustomExceptions.InValiDateException(){ErrorMessage="Invalid Format of date"});
            } 
        }
        [HttpDelete("{startTime}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest,Type =typeof(Response))]
        public IActionResult DeleteAppointment(DateTime startTime){
            DateTime st=startTime;
            string date=st.ToString("dd-MM-yyyy");
            
            bool IsDeleted=_appointmentBL.RemoveAppointmnet(date,startTime);
            if(IsDeleted){
                return NoContent();
            } 
           else{
                return BadRequest(new CustomExceptions.NoDataFoundException(){StatusCode=(int)Statuscodes.ReturnCodeBadRequest,ErrorMessage=ResponseMessage.DataNotFound.GetDescription()});
            }    
        }
        [HttpPut("{date}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest,Type =typeof(Response))]
        public IActionResult UpdateAppointment(string date,Appointment updateData){
            try{
                bool IsUpdated=_appointmentBL.UpdateAppointment(date,updateData);
                if(IsUpdated){
                    return Ok(new Response(){StatusCode=(int)Statuscodes.ReturnCodeOK ,Message=ResponseMessage.Updated.GetDescription()});
                }
                else{
                    return BadRequest(new CustomExceptions.NotIdFoundException(){StatusCode=(int)Statuscodes.ReturnCodeBadRequest ,ErrorMessage=ResponseMessage.IdNotFound.GetDescription()});
                }
            }
            catch(CustomExceptions.DateTimeMisMatchException)
            {
                return BadRequest(new CustomExceptions.DateTimeMisMatchException(){StatusCode=(int)Statuscodes.ReturnCodeBadRequest,ErrorMessage=ResponseMessage.MismatchDate.GetDescription()});
            }
             catch(CustomExceptions.MeetingOverLapException){
                return Conflict(new CustomExceptions.MeetingOverLapException(){StatusCode=(int)Statuscodes.ReturnCodeConflict,ErrorMessage=ResponseMessage.DateTimeOverLap.GetDescription()});
            } 
            catch(CustomExceptions.InValiDateException){
                return BadRequest(new CustomExceptions.InValiDateException(){ErrorMessage="Invalid Format of date"});
            } 
        }

        [HttpGet("holiday/{date}")]
        [ProducesResponseType(StatusCodes.Status200OK ,Type =typeof(List<string>))]
        public  IActionResult GetHolidays(string date){
            List<string> holidayData=_appointmentBL.getHolidays(date);
            return Ok(holidayData);
        }

        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Search([FromQuery]string search,[FromQuery]string type){
            try{
                List<Appointment> FoundData=_appointmentBL.SearchEvents(search,type);
                if(FoundData.Count>0){
                    return Ok(FoundData);
                }
                else{
                    return NotFound("No Data result Found");
                }
            }
            catch(CustomExceptions.InValiDateException){
                return BadRequest(new CustomExceptions.InValiDateException(){ErrorMessage="Invalid Format of date"});
            } 
        }
        [HttpGet("range/{endRange}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult RangeEvents(DateTime endRange){
            List<Appointment> RangeData=_appointmentBL.EventsTimeRange(endRange);
            return Ok(RangeData);
        }

        [HttpGet("id/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type =typeof(Response))]
        public IActionResult GetAppointmentById(Guid id){
            try{
                Appointment eventData=_appointmentBL.getAppointmentById(id);
                return Ok(eventData);
            }
            catch(CustomExceptions.NotIdFoundException){
                return NotFound(new CustomExceptions.NotIdFoundException(){StatusCode=(int)Statuscodes.ReturnCodeNotFound , ErrorMessage=ResponseMessage.IdNotFound.GetDescription()});
            }
        }
    }
}
