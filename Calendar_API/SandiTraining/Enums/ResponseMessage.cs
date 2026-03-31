using System.ComponentModel;
namespace SandiTraining.Enums
{
    public enum ResponseMessage
    {
        [Description("Given EndTime is not greater than the StartTime")]
        MismatchDate,
        [Description("Already Meeting is there")]
        DateTimeOverLap,
        [Description("Event Succesfully added")]
        Created,
        [Description("Meeting ID is not found")]
        IdNotFound,
        [Description("No Data results found")]
        DataNotFound,
        [Description("Updated Successfully")]
        Updated
    }
}