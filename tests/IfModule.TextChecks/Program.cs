using VisionPlugins.IfModule;

int checks = 0;
void Check(bool yes, string name) { if (!yes) throw new Exception(name); checks++; }
var rows = new[] { new IfConditionRow("A", "int", "1", "1000", "alpha"), new IfConditionRow("B", "float", "-2.5", "3.5", "beta") };
Check(IfConditionText.Serialize(rows) == "A:1:1000:alpha;B:-2.5:3.5:beta;", "original UI row order and terminal separator");
Check(IfConditionText.Serialize([]) == "0", "original empty sentinel");
var restored = IfConditionText.Restore(rows, "B:0:5:new;A:2:8:next;");
Check(restored[0].Minimum == "2" && restored[0].Remark == "next" && restored[1].Maximum == "5", "restore by existing row name rather than input order");
var partial = IfConditionText.Restore(rows, "A:2:8;");
Check(partial[0].Minimum == "2" && partial[0].Maximum == "8" && partial[0].Remark == "alpha", "malformed missing remark retains already assigned bounds");
try { IfConditionText.Serialize([rows[0] with { Remark = "invalid:semicolon;" }]); throw new Exception("accepted delimiter"); }
catch (ArgumentException) { checks++; }
var and = IfConditionEvaluation.Evaluate([new(rows[0], IntegerValue:1),new(rows[1], FloatValue:0f)],1);
Check(and.NativeErrorCode==0&&and.AggregatePublished&&and.ModuStatus&&and.Result&&and.ItemResults.SequenceEqual([true,true]),"original inclusive int/float AND path");
var or = IfConditionEvaluation.Evaluate([new(rows[0], IntegerValue:0),new(rows[1], FloatValue:0f)],2);
Check(or.Result&&or.ItemResults.SequenceEqual([false,true]),"source OR aggregation keeps per-item result");
var missing = IfConditionEvaluation.Evaluate([new(rows[0]),new(rows[1])],1);
Check(missing.NativeErrorCode==IfConditionEvaluation.AllInputsUnreadable&&missing.AggregatePublished&&!missing.ModuStatus&&!missing.Result,"all reads fail but native aggregate is still published");
Check(IfConditionEvaluation.Evaluate([],1) is {NativeErrorCode:IfConditionEvaluation.EmptyConditions,AggregatePublished:false},"empty conditions return before aggregate publication");
var inverted = IfConditionEvaluation.Evaluate([new(rows[0],IntegerValue:0),new(rows[1],FloatValue:0f)],3);
Check(inverted.Result&&inverted.ItemResults.SequenceEqual([false,true]),"native mode3 AND then invert, outside ordinary UI choices");
var unordered = IfConditionEvaluation.Evaluate([new(rows[1],FloatValue:float.NaN)],1);
Check(!unordered.Result&&unordered.ModuStatus,"COMISS unordered sample is a failed range item, not an unreadable input");
var invalid = IfConditionEvaluation.Evaluate([new(rows[0] with{Minimum="9",Maximum="1"},IntegerValue:5)],1);
Check(invalid.NativeErrorCode==IfConditionEvaluation.InvalidRange&&!invalid.AggregatePublished,"invalid bounds return before aggregate publication");
var decimalInt = rows[0] with { Minimum = "1.5", Maximum = "3.5" };
Check(IfConditionEvaluation.Evaluate([new(decimalInt,IntegerValue:2)],1).Result &&
      !IfConditionEvaluation.Evaluate([new(decimalInt,IntegerValue:3)],1).Result,
    "native strtok positive fractional tokens tighten both integer bounds");
Check(IfConditionEvaluation.Evaluate([new(rows[0] with{Minimum=".5",Maximum="2."},IntegerValue:0)],1).Result,
    "native strtok skips leading and trailing empty decimal tokens");
Check(IfConditionEvaluation.Evaluate([new(rows[0] with{Minimum="-1.5",Maximum="2"},IntegerValue:0)],1).Result,
    "negative integer lower bound uses atoi then positive fractional token increment");
Check(and.StrResult=="OK"&&and.IfItemResult.SequenceEqual(["OK","OK"])&&and.IfItemStatus.SequenceEqual([1,1]),"PE literal and native item verdict fields");
Check(missing.StrResult=="NG"&&missing.IfItemResult.SequenceEqual(["NG","NG"])&&missing.IfItemStatus.SequenceEqual([0,0])&&missing.ItemReads.SequenceEqual([false,false]),"unreadable items publish verdict 0 separately from read state");
Check(invalid.StrResult is null&&invalid.IfItemResult.Count==0,"early error does not materialize result strings");
var visiblePorts=IfConditionPorts.VisibleRows([new("int1","Temperature","int"),new("float1","Offset","float")]);
Check(visiblePorts.Inputs.Select(x=>x.Name).SequenceEqual(["int1","float1"])&&visiblePorts.Inputs[0].Kind==VisionFramework.DataKind.Integer&&visiblePorts.Inputs[1].Kind==VisionFramework.DataKind.Number,"visible AddIO input name and type path");
Check(visiblePorts.Outputs.Any(x=>x.Name=="TemperatureOut"&&x.Kind==VisionFramework.DataKind.Integer)&&visiblePorts.Outputs.Any(x=>x.Name=="OffsetMin"&&x.Kind==VisionFramework.DataKind.Number)&&visiblePorts.Outputs.Any(x=>x.Name=="OffsetMax"&&x.Kind==VisionFramework.DataKind.Number),"AddIO output suffixes and types");
Check(IfConditionPorts.FixedOutputs.Single(x=>x.Name=="IfItemResult").Cardinality==VisionFramework.ValueCardinality.Array&&IfConditionPorts.FixedOutputs.Single(x=>x.Name=="IfItemStatus").Cardinality==VisionFramework.ValueCardinality.Array,"original result lists remain typed arrays");
try{IfConditionPorts.VisibleRows([new("s","s","string")]);throw new Exception("visible AddIO accepted SDK-only string");}catch(ArgumentException){checks++;}
var inventory = new IfConditionDynamicInventory();
inventory.Add(new("a", "A", "int"));
inventory.Add(new("ab", "AB", "float"));
Check(inventory.Rows.Count == 2 && inventory.DynamicOutputs.Count == 6, "visible AddIO creates three dynamic outputs per row");
var removedOutputs = inventory.Remove("a");
Check(inventory.Rows.Single().Name == "ab" && removedOutputs.Count == 6 && inventory.DynamicOutputs.Count == 0,
    "original RemoveIO display-prefix deletion also removes another row's output descriptors");
Check(inventory.Outputs().Select(x => x.Name).SequenceEqual(IfConditionPorts.FixedOutputs.Select(x => x.Name)),
    "display-prefix deletion preserves fixed result outputs");
Check(inventory.Remove("missing").Count == 0, "missing input leaves inventory unchanged");
var binaryDefinitions = IfConditionDynamicParameters.Parse(System.Text.Encoding.UTF8.GetBytes("A_Max,int;A_Min,float;A_ReMark,string;B_Max,int;ignored,BOOL;A_Max,float;empty,,;"));
Check(binaryDefinitions.Count == 4 && binaryDefinitions["A_Max"].Kind == "float", "DynamicParams clears/rebuilds and last duplicate name wins");
Check(binaryDefinitions["A_Min"].FirstNumericLimit == 100000.0 && binaryDefinitions["B_Max"].SecondNumericLimit == -999999999.0,
    "original numeric ParamItem constructor arguments are retained in source order");
var item = IfConditionDynamicParameters.FindItem(binaryDefinitions, "A");
Check(item is not null && item.Maximum.Name == "A_Max" && item.Minimum.Name == "A_Min" && item.Remark.Name == "A_ReMark",
    "GetIfItemParam requires all three dynamic names and retains Max Min ReMark order");
Check(IfConditionDynamicParameters.FindItem(binaryDefinitions, "B") is null, "missing Min or ReMark returns null");
Check(IfConditionDynamicParameters.Parse([]).Count == 0, "empty DynamicParams payload clears prior snapshot");
try { IfConditionDynamicParameters.FindItem(binaryDefinitions, ""); throw new Exception("accepted empty item name"); }
catch (ArgumentException) { checks++; }
Console.WriteLine($"PASS {checks} IfModule IFText and normal Process checks");
