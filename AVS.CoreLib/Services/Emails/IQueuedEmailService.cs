using System;
using AVS.CoreLib.Data;
using AVS.CoreLib.Data.Domain.Messages;
using AVS.CoreLib.Data.Services;

namespace AVS.CoreLib.Services.Emails
{
    public interface IQueuedEmailService : IEntityServiceBase<QueuedEmail>
    {
        /// <summary>
        /// Search queued emails
        /// </summary>
        /// <param name="fromEmail">From Email</param>
        /// <param name="toEmail">To Email</param>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="loadNotSentItemsOnly">A value indicating whether to load only not sent emails</param>
        /// <param name="maxSendTries">Maximum send tries</param>
        /// <param name="loadNewest">A value indicating whether we should sort queued email descending; otherwise, ascending.</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Email item collection</returns>
        IPagedList<QueuedEmail> SearchEmails(string fromEmail,
            string toEmail, DateTime? createdFromUtc, DateTime? createdToUtc,
            bool loadNotSentItemsOnly, int maxSendTries,
            bool loadNewest, int pageIndex, int pageSize);
    }
}