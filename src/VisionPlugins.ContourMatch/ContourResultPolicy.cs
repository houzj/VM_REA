// Source-confirmed result policy. No matcher, VM transport, or native pointer ABI is supplied here.
// Evidence: module SHA56156A...F64, RVAs 22840/22ee0/23d00/267c0/1c690/1c6d0.
using System;
using System.Collections.Generic;
using System.Linq;

namespace VisionPlugins.ContourMatch;

public readonly record struct FloatLimit(int Enabled, float Low, float High)
{
    public bool Accept(float value)
    {
        if (Enabled != 1) return true;
        // MOVSS/SUBSS/ADDSS, not integer conversion of the range storage.
        const float epsilon = 1.1920928955078125e-7f;
        return !(value < Low - epsilon || High + epsilon < value);
    }
}

public readonly record struct CountLimit(int Enabled, int Low, int High)
{
    public bool Accept(int count) => Enabled != 1 || (Low <= count && count <= High);
}

public readonly record struct EdgePoint(float X, float Y, float Score);
public readonly record struct OutlinePoint(float X, float Y, float Score, int ContourIndex);
public sealed record Match(
    int ModelOrdinal, float BoxCenterX, float BoxCenterY, float Width, float Height,
    float Angle, float MatchPointX, float MatchPointY, float Scale, float ScaleX,
    float ScaleY, float Score, float Rms, float SkewX, IReadOnlyList<EdgePoint> Edges);
public sealed record Limits(
    CountLimit Count, FloatLimit Angle, FloatLimit Scale, FloatLimit ScaleX,
    FloatLimit ScaleY, FloatLimit Score, FloatLimit MatchPointX, FloatLimit MatchPointY,
    FloatLimit BoxCenterX, FloatLimit BoxCenterY);
public sealed record PublishedMatch(Match Value, int OriginalModelSlot, bool Status);
public sealed record ResultPacket(
    bool ModuleStatus, int ActualMatchCount, IReadOnlyList<PublishedMatch> Published,
    IReadOnlyList<OutlinePoint> Outline);

public static class ContourResultPolicy
{
    public static ResultPacket Build(IReadOnlyList<Match> matches,
        IReadOnlyList<int> activeModelSlots, Limits limits, bool okWhenZero,
        int maxPublished, bool includeOutline)
    {
        // Independent safe admission for this candidate's normal UI domain.
        // Original resources constrain MaxMatchNum to 1..50000.
        if (maxPublished is < 1 or > 50000) throw new ArgumentOutOfRangeException(nameof(maxPublished));
        var status = new bool[matches.Count];
        bool moduleStatus = (okWhenZero || matches.Count > 0) && limits.Count.Accept(matches.Count);
        var published = new List<PublishedMatch>(Math.Min(matches.Count, maxPublished));
        var outline = new List<OutlinePoint>();
        for (int i = 0; i < matches.Count; i++)
        {
            Match m = matches[i];
            if ((uint)m.ModelOrdinal >= (uint)activeModelSlots.Count)
                throw new InvalidOperationException("Unsupported model ordinal: backend/map contract is not satisfied.");
            status[i] = limits.Angle.Accept(m.Angle) & limits.Scale.Accept(m.Scale)
                & limits.ScaleX.Accept(m.ScaleX) & limits.ScaleY.Accept(m.ScaleY)
                & limits.Score.Accept(m.Score) & limits.MatchPointX.Accept(m.MatchPointX)
                & limits.MatchPointY.Accept(m.MatchPointY) & limits.BoxCenterX.Accept(m.BoxCenterX)
                & limits.BoxCenterY.Accept(m.BoxCenterY);
            moduleStatus &= status[i];
            // LimitJudge evaluates every match before ProcessOutput caps MatchNum.
            if (i < maxPublished)
                published.Add(new PublishedMatch(m with { Edges = Array.AsReadOnly(m.Edges.ToArray()) },
                    activeModelSlots[m.ModelOrdinal], status[i]));
            if (includeOutline)
                foreach (EdgePoint p in m.Edges)
                    outline.Add(new OutlinePoint(p.X, p.Y,
                        status[i] ? (float)(((double)p.Score * 1000.0) / 1000.0) : 2f, i));
        }
        return new ResultPacket(moduleStatus, matches.Count, published.AsReadOnly(), outline.AsReadOnly());
    }
}

