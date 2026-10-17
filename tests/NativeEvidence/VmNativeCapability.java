// Read-only original input; changes apply only to the owned analysis database.
import ghidra.app.script.GhidraScript;
import ghidra.app.decompiler.*;
import ghidra.app.cmd.function.CreateFunctionCmd;
import ghidra.program.model.address.*;
import ghidra.program.model.listing.*;
import java.nio.file.*;
import java.nio.charset.StandardCharsets;
import java.util.*;
public class VmNativeCapability extends GhidraScript {
 public void run() throws Exception {
  String[] args=getScriptArgs(); Path out=Paths.get(args[0]); Files.createDirectories(out);
  DecompInterface dec=new DecompInterface(); dec.openProgram(currentProgram);
  List<String> seeds=new ArrayList<>();for(int i=1;i<args.length;i++)seeds.add(args[i]);
  List<String> records=new ArrayList<>();
  for(String seed:seeds){
   monitor.checkCancelled();Address a=toAddr(seed);disassemble(a);Function f=getFunctionAt(a);if(f==null)f=createFunction(a,null);
   if(f!=null)CreateFunctionCmd.fixupFunctionBody(currentProgram,f,monitor);
   if(f==null){records.add(seed+"\tNO_FUNCTION");continue;}
   DecompileResults r=dec.decompileFunction(f,90,monitor);String stem=Long.toHexString(a.getOffset());
   if(r.decompileCompleted()&&r.getDecompiledFunction()!=null)Files.writeString(out.resolve(stem+".c"),r.getDecompiledFunction().getC(),StandardCharsets.UTF_8);
   else Files.writeString(out.resolve(stem+".error.txt"),r.getErrorMessage(),StandardCharsets.UTF_8);
   StringBuilder asm=new StringBuilder();Set<String> calls=new TreeSet<>();InstructionIterator it=currentProgram.getListing().getInstructions(f.getBody(),true);
   while(it.hasNext()){Instruction ins=it.next();asm.append(ins.getAddress()).append("\t").append(ins).append("\n");if(ins.getFlowType().isCall())for(Address target:ins.getFlows())calls.add(target.toString());}
   Files.writeString(out.resolve(stem+".asm.txt"),asm.toString(),StandardCharsets.UTF_8);
   records.add(seed+"\t"+f.getName()+"\t"+f.getBody().getNumAddresses()+"\t"+r.decompileCompleted()+"\t"+String.join(",",calls));
   println("CAPABILITY "+seed+" "+f.getName()+" decompile="+r.decompileCompleted()+" bytes="+f.getBody().getNumAddresses());
  }
  Files.writeString(out.resolve("functions-"+System.currentTimeMillis()+".tsv"),"address\tname\tbodyBytes\tdecompiled\tdirectCalls\n"+String.join("\n",records),StandardCharsets.UTF_8);dec.dispose();
 }
}

