import ghidra.app.script.GhidraScript;
import ghidra.app.decompiler.*;
import ghidra.program.model.listing.*;
import java.nio.file.*;
import java.util.*;
public class DistortionC1CommonAudit extends GhidraScript {
 public void run() throws Exception {
  Path out=Paths.get(getScriptArgs()[0]);Files.createDirectories(out);Set<Function> selected=new HashSet<>();
  var all=currentProgram.getFunctionManager().getFunctions(true);var names=new StringBuilder();
  while(all.hasNext()){var f=all.next();names.append(f.getEntryPoint()).append("\t").append(f.getName()).append("\n");if(f.getName().contains("PoolMemory"))selected.add(f);}
  for(String literal:new String[]{"180001550","180001570"}){var f=getFunctionAt(toAddr(literal));if(f!=null)selected.add(f);}
  var refs=new StringBuilder();
  for(String target:new String[]{"180002630","18001a780"})for(var ref:getReferencesTo(toAddr(target))){var caller=getFunctionContaining(ref.getFromAddress());refs.append(target).append("\t").append(ref.getFromAddress()).append("\t").append(caller==null?"unknown":caller.getEntryPoint()).append("\n");if(caller!=null)selected.add(caller);}
  Files.writeString(out.resolve("pool-lifecycle-references.tsv"),refs.toString());
  for(int depth=0;depth<3;depth++){var copy=new HashSet<>(selected);for(var f:copy)for(var callee:f.getCalledFunctions(monitor))if(!callee.isExternal()&&!callee.isThunk())selected.add(callee);}
  var dec=new DecompInterface();dec.openProgram(currentProgram);
  for(var f:selected){var d=dec.decompileFunction(f,45,monitor);if(d.decompileCompleted())Files.writeString(out.resolve(f.getEntryPoint()+".c"),d.getDecompiledFunction().getC());var b=new StringBuilder();var ins=currentProgram.getListing().getInstructions(f.getBody(),true);while(ins.hasNext()){var i=ins.next();b.append(i.getAddress()).append("\t").append(i).append("\n");}Files.writeString(out.resolve(f.getEntryPoint()+"-asm.tsv"),b.toString());var c=new StringBuilder();for(var g:f.getCalledFunctions(monitor))c.append(g.getEntryPoint()).append("\t").append(g.getName()).append("\n");Files.writeString(out.resolve(f.getEntryPoint()+"-calls.tsv"),c.toString());}
  Files.writeString(out.resolve("functions.tsv"),names.toString());Files.writeString(out.resolve("identity.txt"),currentProgram.getExecutablePath()+"\n"+currentProgram.getExecutableSHA256());dec.dispose();
 }
}


