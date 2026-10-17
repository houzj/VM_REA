// Original shipped DLL is read only; all analysis changes stay in this owned project.
import ghidra.app.script.GhidraScript;
import ghidra.app.decompiler.*;
import ghidra.program.model.listing.*;
import java.nio.file.*;
import java.util.*;
public class VmRotateCalibrationAudit extends GhidraScript {
 public void run() throws Exception {
  Path out=Paths.get(getScriptArgs()[0]);Files.createDirectories(out);
  var it=currentProgram.getFunctionManager().getFunctions(true);StringBuilder index=new StringBuilder();Set<Function> selected=new LinkedHashSet<>();
  while(it.hasNext()){Function f=it.next();String name=f.getName(true);index.append(f.getEntryPoint()).append("\t").append(name).append("\n");if(name.contains("RotateCalibModu")||name.contains("CAlgorithmModule"))selected.add(f);if(name.contains("security_check_cookie"))f.setNoReturn(false);}
  Files.writeString(out.resolve("functions.tsv"),index.toString());
  StringBuilder asm=new StringBuilder();var ins=currentProgram.getListing().getInstructions(toAddr(0x1800376b0L),true);while(ins.hasNext()){var i=ins.next();if(i.getAddress().getOffset()>=0x180038680L)break;asm.append(i.getAddress()).append(" ").append(i).append("\n");}Files.writeString(out.resolve("process.asm"),asm.toString());
  var ext=currentProgram.getFunctionManager().getExternalFunctions();while(ext.hasNext()){Function f=ext.next();if(f.getName().startsWith("VM_M_"))f.setNoReturn(false);}
  Set<Function> expanded=new LinkedHashSet<>(selected);for(Function f:selected)expanded.addAll(f.getCalledFunctions(monitor));
  DecompInterface dec=new DecompInterface();var options=new DecompileOptions();options.setEliminateUnreachable(false);dec.setOptions(options);dec.openProgram(currentProgram);
  for(Function f:expanded){if(f.isExternal())continue;var d=dec.decompileFunction(f,60,monitor);if(d.decompileCompleted())Files.writeString(out.resolve(f.getEntryPoint()+".c"),d.getDecompiledFunction().getC());}
  dec.dispose();
 }
}
