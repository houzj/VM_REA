import ghidra.app.script.GhidraScript;
import ghidra.app.decompiler.*;
import ghidra.program.model.listing.*;
import java.nio.file.*;
import java.util.*;
public class BoardMaskAudit extends GhidraScript{
 public void run()throws Exception{Path out=Paths.get(getScriptArgs()[0]);Files.createDirectories(out);Set<Function> selected=new LinkedHashSet<>();var it=currentProgram.getFunctionManager().getFunctions(true);StringBuilder index=new StringBuilder();while(it.hasNext()){var f=it.next();String name=f.getName(true);index.append(f.getEntryPoint()).append("\t").append(name).append("\n");if(name.contains("CreatePreproMaskToolInstance")||f.getEntryPoint().toString().equals("180003100")||f.getEntryPoint().toString().equals("180004980")||f.getEntryPoint().toString().equals("1800059a0"))selected.add(f);if(name.contains("security_check_cookie"))f.setNoReturn(false);}Files.writeString(out.resolve("functions.tsv"),index.toString());for(int i=0;i<2;i++){var all=new LinkedHashSet<Function>(selected);for(var f:selected)all.addAll(f.getCalledFunctions(monitor));selected=all;}var runFunction=getFunctionAt(toAddr("180002af0"));if(runFunction!=null)selected.add(runFunction);var dec=new DecompInterface();dec.openProgram(currentProgram);for(var f:selected)if(!f.isExternal()){f.setNoReturn(false);var result=dec.decompileFunction(f,30,monitor);if(result.decompileCompleted())Files.writeString(out.resolve(f.getEntryPoint()+".c"),result.getDecompiledFunction().getC());}dec.dispose();}
}

