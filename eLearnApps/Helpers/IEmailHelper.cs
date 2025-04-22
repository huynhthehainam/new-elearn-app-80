using System.Collections.Generic;
using System.Threading.Tasks;
using eLearnApps.Entity.LmsTools;
using eLearnApps.Core;
using eLearnApps.Models;
using System.Net.Mail;

namespace eLearnApps.Helpers
{
    public interface IEmailHelper
    {
        bool SendMail(string toEmail, string toName, string fromAddress, string fromName, string subject, string body, bool isBodyHtml = true);
        Task<bool> SendMailAsync(string toEmail, string toName, string fromAddress, string fromName, string subject, string body, bool isBodyHtml = true);
        bool SendMailWithAttachment(ToolName toolName,string toEmail, string toName, string fromAddress, string fromName, string subject, string body, Attachment attachments, bool isBodyHtml = true);
        void SendMailInvite(Meeting meeting, List<MeetingAttendee> lstAttendees, string rootPath);
        void SendMailCancel(Meeting meeting, List<MeetingAttendee> attendee, string rootPath);
        void SendMailCancel(Meeting meeting, MeetingAttendee attendee, string rootPath);
        void SendMailInviteRespondedAlert(Meeting meeting, int attendeeId, List<MeetingAttendee> lstAttendees, InviteResponseType responseType, string rootPath);
        void SendMailInvite(Meeting originalMeeting, List<MeetingAttendee> lstOriginalAttendees, Meeting newMeeting, List<MeetingAttendee> lstNewAttendees, string rootPath);
        void SendMailNotifyGradeReleaseUpdate(string content,  List<UserModel> users);
    }
}