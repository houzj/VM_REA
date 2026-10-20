import ghidra.app.script.GhidraScript;
import ghidra.program.model.address.Address;
import ghidra.program.model.symbol.Reference;
import java.io.*;
public class D1LogZeroReferences extends GhidraScript {
 public void run() throws Exception {
  File out=new File(getScriptArgs()[0]);out.getParentFile().mkdirs();
  try(PrintWriter w=new PrintWriter(out)){
   Address a=toAddr("182fcc4c0");
   for(Reference r:currentProgram.getReferenceManager().getReferencesTo(a)){
    w.println(r.getFromAddress()+" "+r.getReferenceType()+" "+getInstructionAt(r.getFromAddress())+" function="+getFunctionContaining(r.getFromAddress()));
   }
  }
 }
}
