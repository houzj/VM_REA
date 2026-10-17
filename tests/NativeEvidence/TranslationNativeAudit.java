// Static export-focused evidence in a dedicated Translation analysis database.
import ghidra.app.script.GhidraScript;
import ghidra.app.decompiler.*;
import ghidra.program.model.listing.*;
import java.nio.file.*;
public class TranslationNativeAudit extends GhidraScript {
 public void run() throws Exception {
  Path out=Paths.get(getScriptArgs()[0]);Files.createDirectories(out);
  FunctionIterator imports=currentProgram.getFunctionManager().getFunctions(true);
  while(imports.hasNext()){Function f=imports.next();String name=f.getName();if(name.contains("sscanf")||name.contains("sprintf")||name.contains("snprintf"))f.setVarArgs(true);}
  DecompInterface dec=new DecompInterface();dec.openProgram(currentProgram);
  FunctionIterator functions=currentProgram.getFunctionManager().getFunctions(true);
  StringBuilder index=new StringBuilder("address\tname\n");
  while(functions.hasNext()){
   Function f=functions.next();String name=f.getName();
   if(name.contains("sscanf"))f.setVarArgs(true);
   if(!f.isExternal() && (name.contains("Translation") || name.contains("NPointCalib") || (getScriptArgs().length>1 && java.util.Arrays.asList(getScriptArgs()).contains(f.getEntryPoint().toString())))){
    DecompileResults result=dec.decompileFunction(f,120,monitor);String stem=f.getEntryPoint().toString();
    if(result.decompileCompleted())Files.writeString(out.resolve(stem+".c"),result.getDecompiledFunction().getC());
    StringBuilder asm=new StringBuilder();InstructionIterator it=currentProgram.getListing().getInstructions(f.getBody(),true);while(it.hasNext()){Instruction instruction=it.next();asm.append(instruction.getAddress()).append(" ").append(instruction.toString()).append("\n");}Files.writeString(out.resolve(stem+".asm.txt"),asm.toString());index.append(stem).append("\t").append(name).append("\n");
   }
  }
  Files.writeString(out.resolve("functions.tsv"),index.toString());dec.dispose();
 }
}
