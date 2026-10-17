// Focused static analysis in the NImage-owned copy of the original core database.
import ghidra.app.script.GhidraScript;
import ghidra.app.decompiler.*;
import ghidra.program.model.listing.*;
import java.nio.file.*;
import java.util.*;
import ghidra.program.model.data.IntegerDataType;
import ghidra.program.model.symbol.SourceType;
public class NImageCore extends GhidraScript {
 public void run()throws Exception{
  Path out=Paths.get(getScriptArgs()[0]);Files.createDirectories(out);int maxDepth=Integer.parseInt(getScriptArgs()[1]);Set<Function> selected=new LinkedHashSet<>();
  Function cookie=getFunctionAt(toAddr("181571b80"));if(cookie!=null)cookie.setNoReturn(false);
  for(int i=2;i<getScriptArgs().length;i++){var address=toAddr(getScriptArgs()[i]);Function f=getFunctionAt(address);if(f==null){disassemble(address);f=createFunction(address,null);}if(f!=null){if(getScriptArgs()[i].equals("1808b0260"))f.setReturnType(IntegerDataType.dataType,SourceType.USER_DEFINED);selected.add(f);}}
  for(int depth=0;depth<maxDepth;depth++){Set<Function> next=new LinkedHashSet<>(selected);for(Function f:selected)next.addAll(f.getCalledFunctions(monitor));selected=next;}
  DecompInterface dec=new DecompInterface();dec.openProgram(currentProgram);StringBuilder graph=new StringBuilder();for(Function f:selected){if(f.isExternal())continue;f.setNoReturn(false);var result=dec.decompileFunction(f,90,monitor);if(result.decompileCompleted())Files.writeString(out.resolve(f.getEntryPoint()+".c"),result.getDecompiledFunction().getC());for(Function target:f.getCalledFunctions(monitor))graph.append(f.getEntryPoint()).append("\t").append(target.getEntryPoint()).append("\n");}
  Function special=getFunctionAt(toAddr("1808b0260"));if(special!=null){StringBuilder asm=new StringBuilder();var it=currentProgram.getListing().getInstructions(special.getBody(),true);while(it.hasNext()){var instruction=it.next();asm.append(instruction.getAddress()).append(" ").append(instruction).append("\n");}Files.writeString(out.resolve("1808b0260.asm.txt"),asm.toString());}
  Files.writeString(out.resolve("last-calls.tsv"),graph.toString());dec.dispose();
 }
}
