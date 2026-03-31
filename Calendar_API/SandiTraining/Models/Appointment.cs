using System.ComponentModel.DataAnnotations;
namespace SandiTraining.Models
{
    public class Appointment
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Event name is required.")]
        public string EventName { get; set;}=string.Empty;
        [Required(ErrorMessage ="StartTime is required.")]
        public DateTime StartTime{get;set;}
        [Required(ErrorMessage ="EndTime is required.")]
        public DateTime EndTime{get;set;}
        public string EventDescription {get;set;}=string.Empty;
        public List<string>? receiverMail{get;set;}
    }
}
