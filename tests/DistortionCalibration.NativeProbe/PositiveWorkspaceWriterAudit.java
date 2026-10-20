import ghidra.app.script.GhidraScript;
import ghidra.app.decompiler.*;
import java.nio.file.*;
public class PositiveWorkspaceWriterAudit extends GhidraScript {
 public void run() throws Exception {
  Path out=Paths.get(getScriptArgs()[0]);Files.createDirectories(out);
  for(String address : new String[]{"1807fdcff","1807ff184"}){
  var f=getFunctionContaining(toAddr(address));
  if(f==null)throw new Exception("Writer function unavailable in existing analysis");
  var dec=new DecompInterface();dec.openProgram(currentProgram);
  var d=dec.decompileFunction(f,60,monitor);
  if(!d.decompileCompleted())throw new Exception("No decompilation");
  Files.writeString(out.resolve(address+".c"),d.getDecompiledFunction().getC());
  var b=new StringBuilder(); var ins=currentProgram.getListing().getInstructions(f.getBody(),true);
  while(ins.hasNext()){var i=ins.next();b.append(i.getAddress()).append("\t").append(i).append("\n");}
  Files.writeString(out.resolve(address+"-asm.tsv"),b.toString());
  Files.writeString(out.resolve(address+"-identity.txt"),currentProgram.getExecutableSHA256()+"\n"+f.getEntryPoint()+"\n"+f.getBody());dec.dispose();
  }
 }
}

