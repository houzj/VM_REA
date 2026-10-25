// Offline navigation within this task's owned database; never loads the image.
import ghidra.app.script.GhidraScript;
import ghidra.program.model.address.*;
import ghidra.program.model.listing.*;
import ghidra.program.model.symbol.*;
import java.nio.file.*;
import java.nio.charset.StandardCharsets;
import java.util.*;
public class VmGlobalReferences extends GhidraScript {
 public void run() throws Exception {
  String[] args=getScriptArgs();Path out=Paths.get(args[0]);Files.createDirectories(out);
  List<String> rows=new ArrayList<>();long scanned=0;
  for(int a=1;a<args.length;a++){
   String seed=args[a];
   if(seed.startsWith("refs:")){
    Address target=toAddr(seed.substring(5));ReferenceIterator refs=currentProgram.getReferenceManager().getReferencesTo(target);
    while(refs.hasNext()){Reference ref=refs.next();Function f=getFunctionContaining(ref.getFromAddress());Instruction ins=getInstructionAt(ref.getFromAddress());rows.add(seed+"\t"+ref.getFromAddress()+"\t"+(f==null?"":f.getEntryPoint())+"\t"+ref.getReferenceType()+"\t"+(ins==null?"":ins.toString()));}
   }else if(seed.startsWith("field:0x")){
    String displacement="0x"+Long.toHexString(Long.parseUnsignedLong(seed.substring(8),16));
    InstructionIterator it=currentProgram.getListing().getInstructions(true);
    while(it.hasNext()){monitor.checkCancelled();Instruction ins=it.next();scanned++;String text=ins.toString();if(text.matches(".*\\[[^\\]]* \\+ "+displacement+"\\].*")){Function f=getFunctionContaining(ins.getAddress());rows.add(seed+"\t"+ins.getAddress()+"\t"+(f==null?"":f.getEntryPoint())+"\tlisted-instruction\t"+text);}}
   }else throw new IllegalArgumentException(seed);
  }
  Files.writeString(out.resolve("references-"+System.currentTimeMillis()+".tsv"),"seed\taddress\tcontainingFunction\ttype\tinstruction\n"+String.join("\n",rows)+"\n",StandardCharsets.UTF_8);
  Files.writeString(out.resolve("coverage.txt"),"Listed instructions scanned="+scanned+". Existing database only; not proof of absence in undefined code, aliases, or dynamically generated code.\n",StandardCharsets.UTF_8);
  println("GLOBAL_REFERENCES rows="+rows.size()+" listedInstructionsScanned="+scanned);
 }
}
