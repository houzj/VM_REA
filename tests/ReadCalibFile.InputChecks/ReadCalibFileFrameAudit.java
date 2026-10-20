import ghidra.app.script.GhidraScript;
import ghidra.app.decompiler.*;
import ghidra.program.model.listing.*;
import ghidra.program.model.symbol.*;
import java.nio.file.*;
import java.util.*;
public class ReadCalibFileFrameAudit extends GhidraScript {
 public void run() throws Exception {
  Path out=Paths.get(getScriptArgs()[0]);Files.createDirectories(out);
  LinkedHashSet<Function> selected=new LinkedHashSet<>();
  FunctionIterator all=currentProgram.getFunctionManager().getFunctions(true);
  while(all.hasNext()){Function f=all.next();if(f.getName().equals("VM_M_GetInt")||f.getName().equals("VM_M_GetFloat")||f.getName().equals("VM_M_SetInt")||f.getName().equals("VM_M_SetFloat")||f.getName().equals("VM_M_GetString"))selected.add(f);}
  for(int i=1;i<getScriptArgs().length;i++){Function f=currentProgram.getFunctionManager().getFunctionAt(toAddr(getScriptArgs()[i]));if(f!=null)selected.add(f);}
  for(int level=0;level<2;level++)for(Function f:new ArrayList<>(selected))for(Function c:f.getCalledFunctions(monitor))if(!c.isExternal())selected.add(c);
  DecompInterface dec=new DecompInterface();dec.openProgram(currentProgram);StringBuilder index=new StringBuilder();
  for(Function f:selected){String stem=f.getEntryPoint().toString();index.append(stem).append("\t").append(f.getName()).append("\n");DecompileResults r=dec.decompileFunction(f,120,monitor);if(r.decompileCompleted())Files.writeString(out.resolve(stem+".c"),r.getDecompiledFunction().getC());StringBuilder asm=new StringBuilder();InstructionIterator it=currentProgram.getListing().getInstructions(f.getBody(),true);while(it.hasNext()){Instruction ins=it.next();asm.append(ins.getAddress()).append("\t").append(ins).append("\n");}Files.writeString(out.resolve(stem+".asm.tsv"),asm.toString());}
  Files.writeString(out.resolve("functions.tsv"),index.toString());Files.writeString(out.resolve("identity.txt"),currentProgram.getExecutablePath()+"\n"+currentProgram.getExecutableSHA256()+"\n");dec.dispose();
 }
}
