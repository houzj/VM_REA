import ghidra.app.script.GhidraScript;
import ghidra.app.decompiler.*;
import ghidra.app.util.PseudoDisassembler;
import java.nio.file.*;

public class B3ReturnSourceAudit extends GhidraScript {
 public void run() throws Exception {
  var args=getScriptArgs();Path out=Paths.get(args[0]);Files.createDirectories(out);
  Files.writeString(out.resolve("program-identity.txt"),currentProgram.getExecutablePath()+"\n"+currentProgram.getExecutableSHA256()+"\n");
  var dec=new DecompInterface();dec.openProgram(currentProgram);var pseudo=new PseudoDisassembler(currentProgram);
  for(int i=1;i<args.length;i++){
   var spec=args[i].split(":");
   if(spec[0].equals("w")||spec[0].equals("r")){
    var address=toAddr(spec[1]);var limit=toAddr(spec[2]);StringBuilder text=new StringBuilder();
    while(address.compareTo(limit)<0){var instruction=pseudo.disassemble(address);if(instruction==null)throw new Exception("Decode failure "+address);text.append(address).append('\t').append(instruction).append('\n');address=address.add(instruction.getLength());if(spec[0].equals("r")&&instruction.getMnemonicString().equals("RET"))break;}
    Files.writeString(out.resolve(spec[1]+"-"+spec[2]+"-linear.tsv"),text.toString());
   }else{
    var address=toAddr(spec[0]);disassemble(address);var f=getFunctionAt(address);if(f==null)f=createFunction(address,null);if(f==null)throw new Exception("No function "+address);
    var result=dec.decompileFunction(f,90,monitor);
    Files.writeString(out.resolve(address+(result.decompileCompleted()?".c":"-error.txt")),result.decompileCompleted()?result.getDecompiledFunction().getC():result.getErrorMessage());
   }
  }
  dec.dispose();
 }
}
