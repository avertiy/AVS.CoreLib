using AVS.CoreLib.Data.Domain.Messages;
using AVS.CoreLib.Data.EF;

namespace AVS.CoreLib.Data.Domain.Mappings
{
    public partial class EmailAccountMap : DynamicLoadEntityTypeConfiguration<EmailAccount>
    {
        public EmailAccountMap()
        {
            this.ToTable(nameof(EmailAccount), new CoreLibTableNameResolver());
            this.HasKey(ea => ea.Id);
            this.Property(ea => ea.Email).IsRequired().HasMaxLength(255);
            this.Property(ea => ea.DisplayName).HasMaxLength(255);
            this.Property(ea => ea.Host).IsRequired().HasMaxLength(255);
            this.Property(ea => ea.Username).IsRequired().HasMaxLength(255);
            this.Property(ea => ea.Password).IsRequired().HasMaxLength(255);
            this.Ignore(ea => ea.FriendlyName);
        }
    }


}