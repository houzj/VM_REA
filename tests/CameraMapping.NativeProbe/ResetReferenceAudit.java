import ghidra.app.script.GhidraScript;
import ghidra.app.decompiler.*;
import ghidra.program.model.listing.*;
import ghidra.program.model.symbol.Reference;
import java.nio.file.*;

public class ResetReferenceAudit extends GhidraScript {
 public void run() throws Exception {
  Path out=Paths.get(getScriptArgs()[0]);Files.createDirectories(out);
  DecompInterface dec=new DecompInterface();dec.openProgram(currentProgram);
  for(int i=1;i<getScriptArgs().length;i++){
   var address=toAddr(getScriptArgs()[i]);Function target=currentProgram.getFunctionManager().getFunctionAt(address);
   String stem=getScriptArgs()[i].replace(':','_');
   if(target!=null&&!target.isExternal()){
    var result=dec.decompileFunction(target,90,monitor);
    if(result.decompileCompleted())Files.writeString(out.resolve(stem+".c"),result.getDecompiledFunction().getC());
   }
   StringBuilder refs=new StringBuilder("target\t").append(address).append("\t").append(target==null?"<no-function>":target.getName()).append("\n");
   for(Reference ref:currentProgram.getReferenceManager().getReferencesTo(address)){
    Function caller=currentProgram.getFunctionManager().getFunctionContaining(ref.getFromAddress());
    refs.append(ref.getFromAddress()).append('\t').append(ref.getReferenceType()).append('\t').append(caller==null?"<unknown>":caller.getEntryPoint()+" "+caller.getName()).append('\n');
   }
   Files.writeString(out.resolve(stem+"-refs.tsv"),refs.toString());
  }
  dec.dispose();
 }
}
