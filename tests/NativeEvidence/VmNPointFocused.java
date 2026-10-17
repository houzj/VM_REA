// Static evidence only. Repairs the variadic import signature in the owned database.
import ghidra.app.script.GhidraScript;
import ghidra.app.decompiler.*;
import ghidra.program.model.listing.*;
import java.nio.file.*;
public class VmNPointFocused extends GhidraScript {
 public void run() throws Exception {
  Path out=Paths.get(getScriptArgs()[0]);Files.createDirectories(out);
  FunctionIterator functions=currentProgram.getFunctionManager().getFunctions(true);
  while(functions.hasNext()){Function f=functions.next();if(f.getName().contains("sscanf"))f.setVarArgs(true);}
  DecompInterface dec=new DecompInterface();dec.openProgram(currentProgram);
  for(String rva:new String[]{"1800373c0","180037320","180037410","18003e510","18003e560","180039c20","18003b150","18003c2f0","18003f990"}){
   Function f=getFunctionAt(toAddr(rva));if(f==null)continue;
   DecompileResults d=dec.decompileFunction(f,120,monitor);if(d.decompileCompleted())Files.writeString(out.resolve(rva+".c"),d.getDecompiledFunction().getC());
   StringBuilder asm=new StringBuilder();InstructionIterator it=currentProgram.getListing().getInstructions(f.getBody(),true);while(it.hasNext()){Instruction i=it.next();asm.append(i.getAddress()).append(" ").append(i.toString()).append("\n");}Files.writeString(out.resolve(rva+".asm.txt"),asm.toString());
  }
  dec.dispose();
 }
}
