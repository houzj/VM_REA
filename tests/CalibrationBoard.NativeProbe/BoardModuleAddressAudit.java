import ghidra.app.script.GhidraScript;
import ghidra.app.decompiler.*;
import java.nio.file.*;

/** Export exact members from the owned module database; never save edits. */
public class BoardModuleAddressAudit extends GhidraScript {
 public void run() throws Exception {
  String expected="bd62ee940a2e299c0c47e41909e64ca78c09d7717e7cefc629dbfb9e21105c1d";
  if(!expected.equalsIgnoreCase(currentProgram.getExecutableSHA256()))
   throw new Exception("Module database identity mismatch");
  String[] args=getScriptArgs();
  if(args.length<2) throw new Exception("Expected output directory and exact addresses");
  Path out=Paths.get(args[0]); Files.createDirectories(out);
  var dec=new DecompInterface(); dec.openProgram(currentProgram);
  StringBuilder index=new StringBuilder();
  try {
   for(int n=1;n<args.length;n++) {
    var address=toAddr(args[n]); var f=getFunctionAt(address);
    if(f==null) throw new Exception("No existing function at "+address);
    var result=dec.decompileFunction(f,60,monitor);
    if(!result.decompileCompleted()) throw new Exception("Incomplete body "+address);
    Files.writeString(out.resolve(address+".c"),result.getDecompiledFunction().getC());
    var asm=new StringBuilder(); var instructions=currentProgram.getListing().getInstructions(f.getBody(),true);
    while(instructions.hasNext()) { var i=instructions.next(); asm.append(i.getAddress()).append(" ").append(i).append("\n"); }
    Files.writeString(out.resolve(address+".asm"),asm.toString());
    index.append(address).append("\t").append(f.getName(true)).append("\n");
   }
   Files.writeString(out.resolve("functions.tsv"),index.toString());
  } finally { dec.dispose(); }
 }
}
