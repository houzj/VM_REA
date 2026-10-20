namespace VisionPlugins.GrayMatch;

// Original core c978c0, complete368-byte assembly. Owned group transport only.
public static class GrayMatchTrainingAngles
{
    public static IReadOnlyList<int[]> Group(ReadOnlySpan<float> angles)
    {
        if (angles.IsEmpty) throw new ArgumentException("Native empty table reads outside buffer; unsupported.");
        for (int i = 0; i < angles.Length; i++)
            if (!float.IsFinite(angles[i]) || (i > 0 && angles[i] < angles[i - 1]))
                throw new ArgumentException("Only finite nondecreasing caller angle tables supported.");
        if (angles.Length == 1) return new[] { new[] { 0 } };
        float tolerance = (angles[^1] - angles[0]) / (float)(angles.Length - 1);
        tolerance *= 0.5f;
        // Original MINSS second operand1. Input domain finite, arithmetic may overflow.
        if (float.IsNaN(tolerance) || tolerance > 1f) tolerance = 1f;
        byte[] visited = new byte[angles.Length];
        var groups = new List<int[]>();
        int current = 0;
        while (current < angles.Length)
        {
            if (visited[current] == 1) { current++; continue; }
            visited[current] = 1;
            var group = new List<int>(4) { current };
            for (int quarter = 90; group.Count < 4; quarter += 90)
            {
                float center = (float)quarter + angles[current];
                float upper = tolerance + center;
                float lower = center - tolerance;
                int next = current + 1;
                while (next < angles.Length && lower > angles[next]) next++;
                if (next == angles.Length || angles[next] < lower || upper < angles[next] || visited[next] != 0) break;
                visited[next] = 1; group.Add(next);
            }
            groups.Add(group.ToArray());
        }
        return groups.AsReadOnly();
    }
}
