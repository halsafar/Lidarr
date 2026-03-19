using System;
using System.Collections.Generic;
using System.Linq;

namespace NzbDrone.Core.Music
{
    public static class MonitorAlbumTypeFilterExtensions
    {
        public static bool PassesAlbumTypeFilter(Album album, MonitorAlbumTypeFilter filter)
        {
            // Zero means "no filter" — everything passes
            if (filter == MonitorAlbumTypeFilter.None)
            {
                return true;
            }

            var primary = album.AlbumType?.Trim() ?? string.Empty;

            // Single and EP live in PrimaryAlbumType
            if (filter.HasFlag(MonitorAlbumTypeFilter.Single) &&
                string.Equals(primary, "Single", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (filter.HasFlag(MonitorAlbumTypeFilter.EP) &&
                string.Equals(primary, "EP", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            // Studio / Live / Compilation are SecondaryAlbumTypes on "Album" primary releases
            if (string.Equals(primary, "Album", StringComparison.OrdinalIgnoreCase))
            {
                var secondaryNames = album.SecondaryTypes?
                    .Select(t => t.Name?.Trim())
                    .Where(n => n != null)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase)
                    ?? new HashSet<string>();

                // A "Studio" album is an Album with no secondary type, or secondary == "Studio"
                if (filter.HasFlag(MonitorAlbumTypeFilter.Studio) &&
                    (!secondaryNames.Any() || secondaryNames.Contains("Studio")))
                {
                    return true;
                }

                if (filter.HasFlag(MonitorAlbumTypeFilter.Live) &&
                    secondaryNames.Contains("Live"))
                {
                    return true;
                }

                if (filter.HasFlag(MonitorAlbumTypeFilter.Compilation) &&
                    secondaryNames.Contains("Compilation"))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
