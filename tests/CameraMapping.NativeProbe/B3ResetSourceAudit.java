import ghidra.app.script.GhidraScript;
import ghidra.app.decompiler.*;
import ghidra.program.model.listing.*;
import ghidra.program.model.symbol.Reference;
import java.nio.file.*;
import ghidra.app.util.PseudoDisassembler;

public class B3ResetSourceAudit extends GhidraScript {
 public void run() throws Exception {
  Path out=Paths.get(getScriptArgs()[0]);Files.createDirectories(out);
  Files.writeString(out.resolve("program-identity.txt"),currentProgram.getExecutablePath()+"\n"+currentProgram.getExecutableSHA256()+"\n");
  DecompInterface dec=new DecompInterface();dec.openProgram(currentProgram);
  for(int i=1;i<getScriptArgs().length;i++){
   var arg=getScriptArgs()[i];var address=arg.matches("[0-9a-fA-F]+")?toAddr(arg):getGlobalFunctions(arg).get(0).getEntryPoint();disassemble(address);
   Function f=currentProgram.getFunctionManager().getFunctionAt(address);
   if(f==null)f=createFunction(address,null);
   if(f==null)throw new Exception("Missing function "+address);
   var d=dec.decompileFunction(f,90,monitor);
   if(d.decompileCompleted())Files.writeString(out.resolve(address+".c"),d.getDecompiledFunction().getC());
   StringBuilder asm=new StringBuilder(),calls=new StringBuilder();
   // No autoanalysis: explicit instruction window also preserves register/stack
   // evidence when the imported function body initially contains one block.
   var instructions=currentProgram.getListing().getInstructions(address,true);
   while(instructions.hasNext()){
    var instruction=instructions.next();if(instruction.getAddress().subtract(address)>0x800)break;asm.append(instruction.getAddress()).append('\t').append(instruction).append('\n');
    for(Reference ref:instruction.getReferencesFrom())if(ref.getReferenceType().isCall())calls.append(instruction.getAddress()).append('\t').append(ref.getToAddress()).append('\t').append(ref.getReferenceType()).append('\n');
   }
   Files.writeString(out.resolve(address+"-instructions.tsv"),asm.toString());Files.writeString(out.resolve(address+"-calls.tsv"),calls.toString());
  }
  if(currentProgram.getName().equalsIgnoreCase("iMVS-6000PlatformSDK.dll")){var fmt=toAddr("180ac54ec");byte[] bytes=new byte[160];currentProgram.getMemory().getBytes(fmt,bytes);Files.write(out.resolve("180ac54ec-format.bin"),bytes);}
  if(currentProgram.getName().equalsIgnoreCase("PlatformManager.dll")){
   StringBuilder window=new StringBuilder();var pseudo=new PseudoDisassembler(currentProgram);var address=toAddr("18060e6ec");
   while(address.compareTo(toAddr("18060eb90"))<0){var instruction=pseudo.disassemble(address);if(instruction==null)throw new Exception("Decode failure "+address);window.append(address).append('\t').append(instruction).append('\n');address=address.add(instruction.getLength());}
   Files.writeString(out.resolve("command-419-instructions.tsv"),window.toString());
  }
  dec.dispose();
 }
}
