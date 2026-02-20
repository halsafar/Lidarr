using FluentMigrator;
using NzbDrone.Core.Datastore.Migration.Framework;

namespace NzbDrone.Core.Datastore.Migration
{
    [Migration(2026022001)] // Using a very high number until this PR is accepted
    public class relax_not_null_constraints_extra_files : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Alter.Table("ExtraFiles").AlterColumn("TrackFileId").AsInt32().Nullable();
        }
    }
}
