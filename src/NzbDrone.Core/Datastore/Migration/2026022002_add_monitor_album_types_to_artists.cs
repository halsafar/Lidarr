using FluentMigrator;
using NzbDrone.Core.Datastore.Migration.Framework;

namespace NzbDrone.Core.Datastore.Migration
{
    [Migration(2026022002)]
    public class add_monitor_album_types_to_artists : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            if (!Schema.Table("Artists").Column("MonitorNewItemFilter").Exists())
            {
                Alter.Table("Artists")
                    .AddColumn("MonitorNewItemFilter")
                    .AsInt32()
                    .WithDefaultValue(0);
            }
        }
    }
}
