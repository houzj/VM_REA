// Read-only original DLL, owned Ghidra project. Export focused dispatch evidence.
import ghidra.app.script.GhidraScript;
import ghidra.app.decompiler.*;
import ghidra.program.model.listing.*;
import java.nio.file.*;
import java.util.*;
import ghidra.program.model.data.FloatDataType;
import ghidra.program.model.symbol.SourceType;
public class VmNPointSolverCore extends GhidraScript {
 public void run() throws Exception {
  Path out=Paths.get(getScriptArgs()[0]);Files.createDirectories(out);
  Function cookie=getFunctionAt(toAddr("181571b80"));if(cookie!=null)cookie.setNoReturn(false);
  StringBuilder constants=new StringBuilder();for(String s:new String[]{"18162b60c","18162b610","18162b61c","18162b620","18162b624","181629a84"})constants.append(s).append(" ").append(Float.intBitsToFloat(currentProgram.getMemory().getInt(toAddr(s)))).append("\n");Files.writeString(out.resolve("constants.txt"),constants.toString());
  StringBuilder names=new StringBuilder();var functions=currentProgram.getFunctionManager().getFunctions(true);
  Set<Function> selected=new LinkedHashSet<>();
  while(functions.hasNext()){Function f=functions.next();names.append(f.getEntryPoint()).append("\t").append(f.getName()).append("\n");if(f.getName().equals("MVD_Algorithm_Process"))selected.add(f);
   if(Arrays.asList("atan2f","cosf","sinf").contains(f.getName())){var pars=new ArrayList<Parameter>();pars.add(new ParameterImpl("a",FloatDataType.dataType,currentProgram));if(f.getName().equals("atan2f"))pars.add(new ParameterImpl("b",FloatDataType.dataType,currentProgram));f.setReturnType(FloatDataType.dataType,SourceType.USER_DEFINED);f.replaceParameters(pars,Function.FunctionUpdateType.DYNAMIC_STORAGE_ALL_PARAMS,true,SourceType.USER_DEFINED);}
  }
  Files.writeString(out.resolve("functions.tsv"),names.toString());
  for(int i=1;i<getScriptArgs().length;i++){var address=toAddr(getScriptArgs()[i]);Function f=getFunctionAt(address);if(f==null){disassemble(address);f=createFunction(address,null);}if(f!=null)selected.add(f);}
  for(int depth=0;depth<2;depth++){Set<Function> next=new LinkedHashSet<>(selected);for(Function f:selected)next.addAll(f.getCalledFunctions(monitor));selected=next;}
  DecompInterface dec=new DecompInterface();dec.openProgram(currentProgram);
  for(Function f:selected){if(f.isExternal())continue;f.setNoReturn(false);var d=dec.decompileFunction(f,60,monitor);if(d.decompileCompleted())Files.writeString(out.resolve(f.getEntryPoint()+".c"),d.getDecompiledFunction().getC());}
  dec.dispose();
 }
}
