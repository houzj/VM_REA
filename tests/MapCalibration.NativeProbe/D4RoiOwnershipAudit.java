import ghidra.app.script.GhidraScript;
import ghidra.app.decompiler.*;
import java.nio.file.*;

/** Finite wrapper ROI setter/destructor export, in an unsaved read-only project. */
public class D4RoiOwnershipAudit extends GhidraScript {
 public void run() throws Exception {
  Path out=Paths.get(getScriptArgs()[0]);Files.createDirectories(out);
  DecompInterface dec=new DecompInterface();dec.openProgram(currentProgram);
  StringBuilder references=new StringBuilder();
  for(String entry:new String[]{"1800019c0","1800019f0","180001aa0","180001ab0",
    "180001b60","180001b70","180001c00","1800032a0","180003340","18000e1f4","18000e250"}){
   var addr=toAddr(entry);disassemble(addr);var f=getFunctionAt(addr);
   if(f==null){disassemble(addr);f=createFunction(addr,null);}
   if(f==null)throw new Exception("Missing requested entry "+entry);
   var result=dec.decompileFunction(f,60,monitor);
   if(result.decompileCompleted())Files.writeString(out.resolve(entry+".c"),result.getDecompiledFunction().getC());
   // The historic noanalysis wrapper DB has one-instruction function bodies.
   // Export explicit bounded byte spans, rather than pretending those bodies are complete.
   String limit=switch(entry){
    case "1800019c0" -> "1800019f0"; case "1800019f0" -> "180001a70";
    case "180001aa0" -> "180001ab0"; case "180001ab0" -> "180001b20";
    case "180001b60" -> "180001b70"; case "180001b70" -> "180001be0";
    case "180001c00" -> "180001d20"; case "1800032a0" -> "180003340";
    case "180003340" -> "1800033f0"; case "18000e1f4" -> "18000e1fa";
    case "18000e250" -> "18000e256"; default -> throw new Exception(entry);
   };
   StringBuilder asm=new StringBuilder();var cursor=addr;
   while(cursor.compareTo(toAddr(limit))<0){
    var instruction=getInstructionAt(cursor);
    if(instruction==null){disassemble(cursor);instruction=getInstructionAt(cursor);}
    if(instruction==null)throw new Exception("Undecoded byte at "+cursor);
    asm.append(instruction.getAddress()).append("\t").append(instruction).append("\n");
    for(var ref:instruction.getReferencesFrom()){
     var symbol=getSymbolAt(ref.getToAddress());
     if(symbol!=null)references.append(instruction.getAddress()).append("\t").append(ref).append("\t").append(symbol.getName(true)).append("\n");
    }
    cursor=cursor.add(instruction.getLength());
   }
   Files.writeString(out.resolve(entry+".asm"),asm.toString());
  }
  Files.writeString(out.resolve("references.txt"),references.toString());dec.dispose();
 }
}
