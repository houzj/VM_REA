EnclosingCircleChecks.Run((ok,message)=>{if(!ok)throw new Exception(message);});
BinaryHullChecks.Run((ok,message)=>{if(!ok)throw new Exception(message);});
Console.WriteLine($"PASS {File.ReadLines("docs/plugins/calibration-board-enclosing-circle-fixtures.jsonl").Count()} native enclosing-circle fixtures, all center/radius float bits.");
