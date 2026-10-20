import ghidra.app.script.GhidraScript;
import ghidra.app.decompiler.*;
import ghidra.program.model.listing.*;
import java.nio.file.*;
public class MapCalibrationAudit extends GhidraScript {
 public void run() throws Exception {
  Path out=Paths.get(getScriptArgs()[0]);Files.createDirectories(out);StringBuilder names=new StringBuilder();var functions=currentProgram.getFunctionManager().getFunctions(true);DecompInterface dec=new DecompInterface();dec.openProgram(currentProgram);
  while(functions.hasNext()){Function f=functions.next();names.append(f.getEntryPoint()).append("\t").append(f.getName()).append("\n");if(f.isExternal()||f.isThunk())continue;
   boolean selected=false;for(String method:java.util.Arrays.asList("GetParam","SetParam","GetProcessInput","ProcessAlg","ProcessOutput","ProcessCalibMatrix","WriteCalibFile","Process","ResetMember","ReadFileMatrix","WriteCalFile","WriteXmlData","GetInputImage","Init","ResetRoiData"))if(f.getName().equals(method)||f.getName().startsWith("?"+method+"@"))selected=true;if(currentProgram.getName().equals("MVDMapCalibCpp.dll") )selected=true;for(int i=1;i<getScriptArgs().length;i++)if(f.getEntryPoint().toString().equals(getScriptArgs()[i]))selected=true;if(!selected)continue;
   var d=dec.decompileFunction(f,60,monitor);if(d.decompileCompleted())Files.writeString(out.resolve(f.getEntryPoint()+".c"),d.getDecompiledFunction().getC());
   StringBuilder assembly=new StringBuilder();StringBuilder tails=new StringBuilder();var instructions=currentProgram.getListing().getInstructions(f.getBody(),true);while(instructions.hasNext()){var instruction=instructions.next();assembly.append(instruction.getAddress()).append("\t").append(instruction.toString()).append("\n");if(instruction.toString().equals("CALL 0x181571b80")){var address=instruction.getAddress().add(instruction.getLength());tails.append("# after cookie check ").append(instruction.getAddress()).append("\n");for(int count=0;count<64;count++){var tail=getInstructionAt(address);if(tail==null){disassemble(address);tail=getInstructionAt(address);}if(tail==null)break;tails.append(address).append("\t").append(tail).append("\n");if(tail.getMnemonicString().equals("RET"))break;address=address.add(tail.getLength());}}}Files.writeString(out.resolve(f.getEntryPoint()+".asm"),assembly.toString());Files.writeString(out.resolve(f.getEntryPoint()+"-cookie-tails.asm"),tails.toString());
  }Files.writeString(out.resolve("functions.tsv"),names.toString());dec.dispose();
 }
}





