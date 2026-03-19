using FluentMigrator;
using NzbDrone.Core.Datastore.Migration.Framework;

namespace NzbDrone.Core.Datastore.Migration
{
    [Migration(2026022003)]
    public class add_monitor_album_types_to_root_folders : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            if (!Schema.Table("RootFolders").Column("DefaultMonitorNewItemFilter").Exists())
            {
                Alter.Table("RootFolders")
                    .AddColumn("DefaultMonitorNewItemFilter")
                    .AsInt32()
                    .WithDefaultValue(0);
            }
        }
    }
}
