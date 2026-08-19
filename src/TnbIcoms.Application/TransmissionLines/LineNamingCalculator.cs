namespace TnbIcoms.Application.TransmissionLines;

public record StationRef(int StationId, string Abbr);

/// <summary>
/// URS Module 1 §5.2.11 naming convention. Verified against all three worked examples in
/// the URS (2-station Single Line, and both the end-station and tee-station perspectives
/// of a 3-station Tee-Off): for a station being viewed FROM, its "counterpart" is the
/// opposite end of the chain if it is itself an end, or the alphabetically-first end if it
/// is a Tee-Off position; every other station in the line is appended after a "/",
/// alphabetically. This generalises cleanly to 4-station Tee-Offs, which the URS shows a
/// diagram for but gives no worked naming example - not independently verifiable, called
/// out here rather than presented as confirmed.
/// </summary>
public static class LineNamingCalculator
{
    public static string GenerateName(StationRef viewFrom, IReadOnlyList<StationRef> orderedChain, int namingInteger, int lineNumber)
    {
        var ends = new[] { orderedChain[0], orderedChain[^1] };
        var others = orderedChain.Where(s => s.StationId != viewFrom.StationId).ToList();

        var isEnd = ends.Any(e => e.StationId == viewFrom.StationId);
        var counterpart = isEnd
            ? ends.First(e => e.StationId != viewFrom.StationId)
            : ends.OrderBy(e => e.Abbr, StringComparer.OrdinalIgnoreCase).First();

        var remaining = others
            .Where(s => s.StationId != counterpart.StationId)
            .OrderBy(s => s.Abbr, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var suffix = remaining.Count > 0 ? "/" + string.Join("/", remaining.Select(s => s.Abbr)) : string.Empty;

        return $"{namingInteger}{viewFrom.Abbr}-{counterpart.Abbr}{suffix}{lineNumber}";
    }

    public static string LineFilterTypeFor(int stationCount) => stationCount switch
    {
        2 => "Single",
        3 => "Tee-Off",
        _ => "Quad"
    };
}
