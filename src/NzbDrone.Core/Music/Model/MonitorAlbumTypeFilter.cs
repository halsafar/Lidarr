using System;

namespace NzbDrone.Core.Music
{
    [Flags]
    public enum MonitorAlbumTypeFilter
    {
        None        = 0,
        Studio      = 1,
        Single      = 2,
        EP          = 4,
        Live        = 8,
        Compilation = 16
    }
}
