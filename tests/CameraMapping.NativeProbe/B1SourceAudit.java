import ghidra.app.script.GhidraScript;
import ghidra.app.decompiler.*;
import ghidra.program.model.listing.*;
import ghidra.program.model.symbol.Reference;
import java.nio.file.*;
import ghidra.app.util.PseudoDisassembler;

public class B1SourceAudit extends GhidraScript {
 public void run() throws Exception {
  Path out=Paths.get(getScriptArgs()[0]);Files.createDirectories(out);
  Files.writeString(out.resolve("program-identity.txt"),currentProgram.getExecutablePath()+"\n"+currentProgram.getExecutableSHA256()+"\n"+currentProgram.getImageBase()+"\n");
  DecompInterface dec=new DecompInterface();dec.openProgram(currentProgram);
  for(int i=1;i<getScriptArgs().length;i++){
   Function f=currentProgram.getFunctionManager().getFunctionAt(toAddr(getScriptArgs()[i]));
   if(f==null)throw new Exception("Missing function "+getScriptArgs()[i]);
   String stem=f.getEntryPoint().toString();
   var d=dec.decompileFunction(f,90,monitor);
   if(d.decompileCompleted())Files.writeString(out.resolve(stem+".c"),d.getDecompiledFunction().getC());
   StringBuilder asm=new StringBuilder();
   var instructions=currentProgram.getListing().getInstructions(f.getBody(),true);
   while(instructions.hasNext()){
    var instruction=instructions.next();asm.append(instruction.getAddress()).append('\t').append(instruction).append('\n');
   }
   Files.writeString(out.resolve(stem+"-instructions.tsv"),asm.toString());
   if(stem.equals("180730be0")){
    // The retained database Function Body only owns the entry block; decode
    // the full known code interval without modifying the read-only Program.
    var pseudo=new PseudoDisassembler(currentProgram);
    var address=f.getEntryPoint();var limit=toAddr("180730d20");
    StringBuilder linear=new StringBuilder();
    while(address.compareTo(limit)<0){
     var instruction=pseudo.disassemble(address);
     if(instruction==null)throw new Exception("Decode failure "+address);
     linear.append(address).append('\t').append(instruction).append('\n');
     address=address.add(instruction.getLength());
    }
    Files.writeString(out.resolve(stem+"-linear-instructions.tsv"),linear.toString());
   }
   StringBuilder refs=new StringBuilder();
   for(Reference r:currentProgram.getReferenceManager().getReferencesTo(f.getEntryPoint())){
    Function caller=currentProgram.getFunctionManager().getFunctionContaining(r.getFromAddress());
    refs.append(r.getFromAddress()).append('\t').append(r.getReferenceType()).append('\t').append(caller==null?"<none>":caller.getEntryPoint()).append('\n');
   }
   Files.writeString(out.resolve(stem+"-callers.tsv"),refs.toString());
  }
  dec.dispose();
 }
}
