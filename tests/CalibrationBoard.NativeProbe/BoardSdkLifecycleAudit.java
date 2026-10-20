import ghidra.app.script.GhidraScript;
import ghidra.app.decompiler.*;
import java.nio.file.*;

public class BoardSdkLifecycleAudit extends GhidraScript {
 public void run() throws Exception {
  Path out=Paths.get(getScriptArgs()[0]);Files.createDirectories(out);
  var dec=new DecompInterface();dec.openProgram(currentProgram);
  StringBuilder index=new StringBuilder();
  for(String address:new String[]{"180002470","180002490","180002690","1800026a0","1800026e0","180003720","180003df0","180005940"}){
   var f=getFunctionAt(toAddr(address));if(f==null){disassemble(toAddr(address));f=createFunction(toAddr(address),null);}
   if(f==null)throw new Exception("Missing function "+address);
   var result=dec.decompileFunction(f,45,monitor);
   if(!result.decompileCompleted())throw new Exception("No body "+address);
   Files.writeString(out.resolve(address+".c"),result.getDecompiledFunction().getC());
   StringBuilder asm=new StringBuilder();var instructions=currentProgram.getListing().getInstructions(f.getBody(),true);
   while(instructions.hasNext()){var i=instructions.next();asm.append(i.getAddress()).append(" ").append(i).append("\n");}
   Files.writeString(out.resolve(address+".asm"),asm.toString());
   index.append(address).append("\t").append(f.getName(true)).append("\n");
  }
  Files.writeString(out.resolve("functions.tsv"),index.toString());dec.dispose();
 }
}
