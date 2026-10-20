import ghidra.app.script.GhidraScript;
import ghidra.app.decompiler.*;
import ghidra.program.model.listing.*;
import java.nio.file.*;import java.util.*;
public class BoardCoreFocused extends GhidraScript{
 public void run()throws Exception{Path out=Paths.get(getScriptArgs()[0]);Files.createDirectories(out);var cookie=getFunctionAt(toAddr("181571b80"));if(cookie!=null)cookie.setNoReturn(false);Set<Function> selected=new LinkedHashSet<>();for(int i=1;i<getScriptArgs().length;i++){var address=toAddr(getScriptArgs()[i]);var f=getFunctionAt(address);if(f==null){disassemble(address);f=createFunction(address,null);}if(f!=null)selected.add(f);}var dec=new DecompInterface();dec.openProgram(currentProgram);var graph=new StringBuilder();for(var f:selected){f.setNoReturn(false);var result=dec.decompileFunction(f,45,monitor);if(result.decompileCompleted())Files.writeString(out.resolve(f.getEntryPoint()+".c"),result.getDecompiledFunction().getC());for(var target:f.getCalledFunctions(monitor))graph.append(f.getEntryPoint()).append("\t").append(target.getEntryPoint()).append("\n");}Files.writeString(out.resolve("calls.tsv"),graph.toString());dec.dispose();}
}
