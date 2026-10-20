import ghidra.app.script.GhidraScript;
import java.nio.file.*;
public class DistortionC3Vtables extends GhidraScript {
 public void run()throws Exception { Path out=Paths.get(getScriptArgs()[0]);Files.createDirectories(out);var b=new StringBuilder();var it=currentProgram.getSymbolTable().getAllSymbols(true);while(it.hasNext()){var s=it.next();String name=s.getName(true);if(name.contains("CImageCalibToolImpl")&&name.contains("vftable")){b.append(s.getAddress()).append("\t").append(name).append("\n");for(int i=0;i<20;i++)b.append(Integer.toHexString(i*8)).append("\t").append(Long.toHexString(currentProgram.getMemory().getLong(s.getAddress().add(i*8)))).append("\n");}}Files.writeString(out.resolve("vtables.tsv"),b.toString()); }
}
