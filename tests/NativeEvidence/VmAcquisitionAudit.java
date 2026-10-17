// Analysis output only; never modifies or executes the original DLL.
import ghidra.app.script.GhidraScript;
import ghidra.app.decompiler.*;
import ghidra.program.model.listing.*;
import java.nio.file.*;
import java.util.*;
public class VmAcquisitionAudit extends GhidraScript {
 public void run() throws Exception {
  Path out=Paths.get(getScriptArgs()[0]);Files.createDirectories(out);
  List<String> names=new ArrayList<>();DecompInterface dec=new DecompInterface();dec.openProgram(currentProgram);
  FunctionIterator it=currentProgram.getFunctionManager().getFunctions(true);
  while(it.hasNext()) {Function f=it.next();names.add(f.getEntryPoint()+"\t"+f.getName());
   if(!f.isExternal()&&!f.isThunk()&&f.getBody().getNumAddresses()>80){
    DecompileResults r=dec.decompileFunction(f,60,monitor);
    if(r.decompileCompleted()&&r.getDecompiledFunction()!=null)Files.writeString(out.resolve(f.getEntryPoint()+".c"),r.getDecompiledFunction().getC());
   }
  }
  Files.write(out.resolve("functions.tsv"),names);dec.dispose();
 }
}
