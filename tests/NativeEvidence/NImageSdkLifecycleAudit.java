import ghidra.app.script.GhidraScript;
import ghidra.app.decompiler.*;
import ghidra.program.model.address.*;
import ghidra.program.model.listing.*;
import java.nio.file.*;
import java.util.*;

// Export actual SDK vtable targets; no target execution or saved project changes.
public class NImageSdkLifecycleAudit extends GhidraScript {
 public void run() throws Exception {
  Path out=Paths.get(getScriptArgs()[0]);Files.createDirectories(out);
  var symbols=currentProgram.getSymbolTable().getAllSymbols(true);
  Set<Address> targets=new LinkedHashSet<>();StringBuilder table=new StringBuilder();
  while(symbols.hasNext()){
   var s=symbols.next();String name=s.getName(true);
   if(!name.contains("CNImageCalibV2ToolImpl")||!name.endsWith("vftable"))continue;
   for(int slot=0;slot<5;slot++){
    long ptr=currentProgram.getMemory().getLong(s.getAddress().add(slot*8));
    table.append(name).append('\t').append(s.getAddress()).append('\t').append(slot*8).append('\t').append(Long.toHexString(ptr)).append('\n');
    targets.add(toAddr(ptr));
   }
  }
  if(targets.isEmpty())throw new Exception("Exact SDK vtable symbol not found; do not guess targets");
  Files.writeString(out.resolve("vtable.tsv"),table.toString());
  DecompInterface dec=new DecompInterface();dec.openProgram(currentProgram);
  for(Address address:targets){
   var f=getFunctionAt(address);if(f==null){disassemble(address);f=createFunction(address,null);}
   if(f==null)throw new Exception("Missing actual vtable function "+address);
   var r=dec.decompileFunction(f,60,monitor);
   if(!r.decompileCompleted()||r.getDecompiledFunction()==null)throw new Exception("Decompile failed "+address+": "+r.getErrorMessage());
   String stem=Long.toHexString(address.getOffset());Files.writeString(out.resolve(stem+".c"),r.getDecompiledFunction().getC());
   StringBuilder asm=new StringBuilder();var it=currentProgram.getListing().getInstructions(f.getBody(),true);
   while(it.hasNext()){var i=it.next();asm.append(i.getAddress()).append('\t').append(i).append('\n');}
   Files.writeString(out.resolve(stem+".asm.txt"),asm.toString());
  }
  dec.dispose();Files.writeString(out.resolve("done.txt"),"Actual vtable targets exported; readonly project.\n");
 }
}
