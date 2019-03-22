using System;
using AVS.CoreLib.Data.Domain.Tasks;
using AVS.CoreLib.Services.Emails;
using AVS.CoreLib.Services.Logging.LogWriters;

namespace AVS.CoreLib.Services.Tasks.AppTasks
{
    /// <summary>
    /// Task sends queued messages 
    /// </summary>
    public class QueuedMessagesSendTask : ITask
    {
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly IEmailSender _emailSender;

        public static ScheduleTask DefaultScheduleTask
        {
            get
            {
                var task = new ScheduleTask
                {
                    Name = "Send email",
                    Seconds = 60,
                    Enabled = true,
                    StopOnError = false,
                    Type = typeof(QueuedMessagesSendTask).AssemblyQualifiedName
                };
                return task;
            }
        }

        public QueuedMessagesSendTask(IQueuedEmailService queuedEmailService,
            IEmailSender emailSender)
        {
            this._queuedEmailService = queuedEmailService;
            this._emailSender = emailSender;
        }


        public void Execute(TaskLogWriter log)
        {
            var maxTries = 3;
            var queuedEmails = _queuedEmailService.SearchEmails(null, null, null, null,
                true, maxTries, false, 0, 500);
            
            foreach (var queuedEmail in queuedEmails)
            {
                var bcc = String.IsNullOrWhiteSpace(queuedEmail.Bcc)
                    ? null
                    : queuedEmail.Bcc.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                var cc = String.IsNullOrWhiteSpace(queuedEmail.CC)
                    ? null
                    : queuedEmail.CC.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                try
                {
                    _emailSender.SendEmail(queuedEmail.EmailAccount, queuedEmail.Subject, queuedEmail.Body,
                        queuedEmail.From, queuedEmail.FromName, queuedEmail.To, queuedEmail.ToName, bcc, cc,
                        queuedEmail.AttachmentFilePath, queuedEmail.AttachmentFileName);

                    queuedEmail.SentOnUtc = DateTime.UtcNow;
                }
                catch (Exception exc)
                {
                    log.Write($"Error sending e-mail. {exc.Message}");
                    log.WriteDetails(exc.ToString());
                }
                finally
                {
                    queuedEmail.SentTries = queuedEmail.SentTries + 1;
                    _queuedEmailService.Update(queuedEmail);
                }
            }
        }
    }
}
