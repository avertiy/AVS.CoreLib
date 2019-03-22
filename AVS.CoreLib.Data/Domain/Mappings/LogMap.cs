using AVS.CoreLib.Data.Domain.Logging;
using AVS.CoreLib.Data.EF;

namespace AVS.CoreLib.Data.Domain.Mappings
{
    public partial class LogMap : DynamicLoadEntityTypeConfiguration<Log>
    {
        public LogMap()
        {
            this.ToTable(nameof(Log), new CoreLibTableNameResolver());
            this.HasKey(l => l.Id);
            this.Property(l => l.ShortMessage).IsRequired();
            this.Ignore(l => l.LogLevel);
        }
    }
}