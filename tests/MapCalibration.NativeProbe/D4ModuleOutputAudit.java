import ghidra.app.script.GhidraScript;
import ghidra.app.decompiler.*;
import java.nio.file.*;

/** Finite module ROI state export, in an unsaved read-only project. */
public class D4ModuleOutputAudit extends GhidraScript {
 public void run() throws Exception {
  Path out=Paths.get(getScriptArgs()[0]);Files.createDirectories(out);
  DecompInterface dec=new DecompInterface();dec.openProgram(currentProgram);
  StringBuilder references=new StringBuilder();
  String[] entries={"18001f280","18001f390","18001fae0","1800203d0","180021340","180021690","180021c50","180022d50","180022ec0","180022f40","180023870"};
  String[] limits={"18001f310","18001fae0","18001fbd0","1800209c0","180021480","180021c40","1800220b0","180022ec0","180022f40","180023610","180024340"};
  java.util.Map<String,String> ends=new java.util.HashMap<>();for(int i=0;i<entries.length;i++)ends.put(entries[i],limits[i]);ends.put("180015660","180015830");ends.put("180015830","180015e30");
  ends.put("180006f80","1800071d0");ends.put("18001f190","18001f280");ends.put("18001c740","18001caa0");ends.put("180010d10","180011180");ends.put("180007480","180007580");if(getScriptArgs().length>1)entries=java.util.Arrays.copyOfRange(getScriptArgs(),1,getScriptArgs().length);
  for(int index=0;index<entries.length;index++){String entry=entries[index];
   var addr=toAddr(entry);disassemble(addr);var f=getFunctionAt(addr);
   if(f==null){disassemble(addr);f=createFunction(addr,null);}
   if(f==null)throw new Exception("Missing requested entry "+entry);
   var result=dec.decompileFunction(f,60,monitor);
   if(result.decompileCompleted())Files.writeString(out.resolve(entry+".c"),result.getDecompiledFunction().getC());
   // The historic noanalysis wrapper DB has one-instruction function bodies.
   // Export explicit bounded byte spans, rather than pretending those bodies are complete.
   String limit=ends.get(entry);
   StringBuilder asm=new StringBuilder();var cursor=addr;
   while(cursor.compareTo(toAddr(limit))<0){
    var instruction=getInstructionAt(cursor);
    if(instruction==null){disassemble(cursor);instruction=getInstructionAt(cursor);}
    if(instruction==null){asm.append(cursor).append("\tUNDECODED_BYTE ").append(String.format("%02x",getByte(cursor)&255)).append("\n");cursor=cursor.add(1);continue;}
    asm.append(instruction.getAddress()).append("\t").append(instruction).append("\n");
    for(var ref:instruction.getReferencesFrom()){
     var symbol=getSymbolAt(ref.getToAddress());
     if(symbol!=null)references.append(instruction.getAddress()).append("\t").append(ref).append("\t").append(symbol.getName(true)).append("\n");
    }
    cursor=cursor.add(instruction.getLength());
   }
   Files.writeString(out.resolve(entry+".asm"),asm.toString());
  }
  for(String entry:entries)for(var ref:getReferencesTo(toAddr(entry))){var caller=getFunctionContaining(ref.getFromAddress());references.append("XREF ").append(entry).append(" ").append(ref).append(" caller=").append(caller==null?"unknown":caller.getEntryPoint()).append("\n");}
  Files.writeString(out.resolve("references.txt"),references.toString());dec.dispose();
 }
}


