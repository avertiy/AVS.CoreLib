using AVS.CoreLib.Data.Domain.Messages;
using AVS.CoreLib.Data.EF;

namespace AVS.CoreLib.Data.Domain.Mappings
{
    public partial class QueuedEmailMap : DynamicLoadEntityTypeConfiguration<QueuedEmail>
    {
        public QueuedEmailMap()
        {
            this.ToTable(nameof(QueuedEmail), new CoreLibTableNameResolver());
            this.HasKey(qe => qe.Id);

            this.Property(qe => qe.From).IsRequired().HasMaxLength(260);
            this.Property(qe => qe.FromName).HasMaxLength(260);
            this.Property(qe => qe.To).IsRequired().HasMaxLength(260);
            this.Property(qe => qe.ToName).HasMaxLength(260);
            this.Property(qe => qe.CC).HasMaxLength(260);
            this.Property(qe => qe.Bcc).HasMaxLength(260);
            this.Property(qe => qe.Subject).HasMaxLength(500);
            this.Property(qe => qe.AttachmentFileName).HasMaxLength(260);
            this.Property(qe => qe.AttachmentFilePath).HasMaxLength(260);


            this.HasRequired(qe => qe.EmailAccount)
                .WithMany()
                .HasForeignKey(qe => qe.EmailAccountId).WillCascadeOnDelete(true);
        }
    }
}