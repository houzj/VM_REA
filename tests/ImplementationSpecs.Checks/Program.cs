using System.Text.Json;

internal static class Program
{
    private static int _checks;

    private static JsonElement Read(string relativePath)
    {
        var full = Path.GetFullPath(relativePath);
        if (!File.Exists(full)) throw new FileNotFoundException("missing ledger: " + relativePath);
        return JsonDocument.Parse(File.ReadAllText(full)).RootElement.Clone();
    }

    private static int Int(JsonElement node, string name) => node.GetProperty(name).GetInt32();

    private static string Str(JsonElement node, string name) => node.GetProperty(name).GetString() ?? "";

    private static void Check(bool condition, string label)
    {
        if (!condition) throw new Exception("FAIL " + label);
        _checks++;
        Console.WriteLine("PASS " + label);
    }

    private static int Main()
    {
        try
        {
            var status = Read("docs/VM-LOCAL-RESTORATION-STATUS.json");
            var phase1 = status.GetProperty("phase1Landing");
            var incremental = status.GetProperty("incrementalLanding");
            var members = Read("tests/ArchitectureRecovery/original-members.map.json");
            var specs = Read("tests/ArchitectureRecovery/tool-implementation-specs.map.json");
            var matrix = Read("analysis/full-audit/batch1-20261107/C03/c03-s0-rerun-matrix-20261110.json");

            Check(Str(phase1, "s0GateStatus") == "S0-closed-on-local-slice; C03-root-cause-still-open",
                "S0 closed on local slice, C03 root cause still open");
            Check(Str(phase1.GetProperty("s0ExitGates"), "S0.1") == "satisfied-on-current-build" &&
                  Str(phase1.GetProperty("s0ExitGates"), "S0.2") == "satisfied-on-current-build",
                "S0.1/S0.2 satisfied on current build");
            Check(Int(phase1, "unknowns") == 5 && phase1.GetProperty("productChanges").GetArrayLength() == 0,
                "five unknowns preserved, zero product algorithm changes");
            Check(Int(phase1.GetProperty("diffPreserved").GetProperty("originalVm"), "blobCount") == 45 &&
                  Int(phase1.GetProperty("diffPreserved").GetProperty("directSdkAndLocal"), "blobCount") == 47,
                "open differential 45 vs 47 preserved");
            Check(Int(incremental, "commandsExecuted") == 1, "C01 rerun command executed once");
            Check(Str(incremental.GetProperty("batchStatus"), "B0") == "open" &&
                  Str(incremental.GetProperty("batchStatus"), "B1") == "blocked",
                "B0 open, B1 still blocked");

            var memberTotals = members.GetProperty("totals");
            Check(Int(memberTotals, "assemblies") == 25 && Int(memberTotals, "treePasses") == 28 &&
                  Int(memberTotals, "duplicatePasses") == 3, "25 assemblies from 28 trees, 3 duplicate passes");
            Check(Int(memberTotals, "canonicalCsFiles") == Int(memberTotals, "baselineUniqueCs"),
                "canonical .cs count matches baseline deduplicated .cs");
            Check(members.GetProperty("assemblies").GetArrayLength() == 25, "all 25 assembly ledgers present");
            Check(Int(memberTotals, "declaredTypes") > 0 && Int(memberTotals, "methodSignatures") > 0,
                "type and method signatures extracted");

            var specCounts = specs.GetProperty("counts");
            Check(Int(specCounts, "tools") == 162 && Int(specCounts, "categories") == 14,
                "162 tools across 14 categories");
            Check(Int(specCounts.GetProperty("implementationStates"), "not-established") == 161 &&
                  Int(specCounts.GetProperty("implementationStates"), "partial") == 1,
                "161 not established plus 1 partial");
            Check(Int(specCounts, "totalEvidenceFiles") == 2015, "2015 hashed evidence files registered");
            var toolArray = specs.GetProperty("tools");
            var anchors = new HashSet<string>();
            var unknownCount = 0;
            foreach (var tool in toolArray.EnumerateArray())
            {
                var anchor = Str(tool, "taskCardAnchor");
                Check(anchors.Add(anchor), "unique task card anchor " + anchor);
                if (Str(tool.GetProperty("nativeBoundary"), "state") != "not-recovered")
                    throw new Exception("FAIL native boundary must stay not-recovered");
                unknownCount += tool.GetProperty("unknowns").GetArrayLength();
            }
            Check(anchors.Count == 162, "all 162 tool anchors unique");
            Check(unknownCount >= 162 * 2, "every tool registers native and parity unknowns");

            var rerun = matrix.GetProperty("buildIdentity");
            Check(Int(rerun, "exitCode") == 0 && Int(rerun, "passChecks") == 18, "C01 rerun exit 0 / 18 PASS");
            var sameInput = matrix.GetProperty("sameInput");
            Check(Str(sameInput, "reportSha256") == Str(sameInput, "historyReportSha256"),
                "same-input rerun report byte-identical to history");
            Check(Int(sameInput.GetProperty("blob"), "count") == 47 &&
                  Int(sameInput.GetProperty("blob"), "totalArea") == 973202,
                "same-input rerun reproduces 47/973202");
            var samples = matrix.GetProperty("samples");
            Check(samples.GetArrayLength() == 12, "12 calibration samples recorded");
            foreach (var sample in samples.EnumerateArray())
            {
                var index = Int(sample, "index");
                Check(Int(sample, "reloadedCount") == Int(sample, "count") &&
                      Int(sample, "sessionStatus") == 0 && Int(sample, "outputCount") == 13,
                    "sample " + index + " reload/session/output contract");
            }
            Check(Str(matrix.GetProperty("s0Recheck"), "overall") ==
                  "S0-closed-on-local-slice; C03-root-cause-still-open", "S0 recheck wording aligned");
            var boundaries = matrix.GetProperty("openBoundaries");
            var joined = string.Join("|", boundaries.EnumerateArray().Select(x => x.GetString()));
            Check(joined.Contains("45/971486") && joined.Contains("986444"),
                "coincidental 45 count explicitly disclaimed");
            var regressions = matrix.GetProperty("regressions");
            Check(regressions.GetProperty("executed").GetArrayLength() == 11,
                "11 executed regressions registered");
            Check(regressions.GetProperty("environmentRepairs").GetArrayLength() == 3,
                "3 environment repairs registered");
            Check(regressions.GetProperty("notExecutedExternalFixture").GetArrayLength() == 2,
                "2 external-fixture-blocked regressions registered, not counted as passed");
            Check(joined.Contains("原厂外部数据"), "external-fixture boundary must be explicit");

            Console.WriteLine("ImplementationSpecs.Checks: " + _checks + " checks passed.");
            return 0;
        }
        catch (Exception error)
        {
            Console.WriteLine("ImplementationSpecs.Checks FAILED: " + error.Message);
            return 1;
        }
    }
}
