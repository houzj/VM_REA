import ghidra.app.script.GhidraScript;
import ghidra.program.model.address.*;
import java.nio.file.*;
public class DistortionCatchContinuations extends GhidraScript {
 public void run() throws Exception { if(!currentProgram.getExecutableSHA256().equalsIgnoreCase("0a6bac886f1aa1ccc41433ef9d35fc1091b3d43354de5794563454d8bba03bcb"))throw new Exception("Wrong target");Path out=Paths.get(getScriptArgs()[0]);Files.createDirectories(out);String[] points={"18002074c","180020782","1800209b9","1800209d3","180020b06","1800212ec"};for(String p:points){var a=toAddr(p);disassemble(a);var body=new AddressSet(a,a.add(79));var b=new StringBuilder();var it=currentProgram.getListing().getInstructions(body,true);while(it.hasNext()){var i=it.next();b.append(i.getAddress()).append("\t").append(i).append("\n");}Files.writeString(out.resolve(p+"-continuation.tsv"),b.toString());}Files.writeString(out.resolve("identity.txt"),currentProgram.getExecutableSHA256()); }
}
