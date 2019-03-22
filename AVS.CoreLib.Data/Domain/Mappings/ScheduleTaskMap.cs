using AVS.CoreLib.Data.Domain.Tasks;
using AVS.CoreLib.Data.EF;

namespace AVS.CoreLib.Data.Domain.Mappings
{
    public partial class ScheduleTaskMap : DynamicLoadEntityTypeConfiguration<ScheduleTask>
    {
        public ScheduleTaskMap()
        {
            this.ToTable(nameof(ScheduleTask), new CoreLibTableNameResolver());
            this.HasKey(t => t.Id);
            this.Property(t => t.Name).HasMaxLength(100).IsRequired();
            this.Property(t => t.Description).HasMaxLength(100);
            this.Property(t => t.Group).HasMaxLength(25);
            this.Property(t => t.Type).HasMaxLength(1000).IsRequired();
        }
    }
}