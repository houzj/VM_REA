import ghidra.app.script.GhidraScript;
import ghidra.program.model.listing.Function;
import java.nio.file.*;
import java.util.*;

/** Read-only, identity-matched file-matrix caller inventory. */
public class D4FileDirectionAudit extends GhidraScript {
 public void run() throws Exception {
  String out=getScriptArgs()[0];
  Set<String> selected=new LinkedHashSet<>();
  selected.add("1807d4f20");selected.add("180730be0");
  StringBuilder refs=new StringBuilder();
  for(var ref:getReferencesTo(toAddr("1807d4f20"))){
   Function caller=getFunctionContaining(ref.getFromAddress());
   refs.append(ref).append("\tcaller=").append(caller==null?"unknown":caller.getEntryPoint()).append("\n");
   if(caller!=null)selected.add(caller.getEntryPoint().toString());
  }
  Files.createDirectories(Paths.get(out));
  Files.writeString(Paths.get(out,"file-direction-references.txt"),refs.toString());
  List<String> args=new ArrayList<>();args.add(out);args.addAll(selected);
  runScript("MapCalibrationAudit.java",args.toArray(new String[0]));
 }
}
