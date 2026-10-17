import ghidra.app.script.GhidraScript;
import ghidra.app.decompiler.*;
import ghidra.program.model.listing.*;
import java.nio.file.*;
public class NImageFocused extends GhidraScript{
 public void run()throws Exception{Path output=Paths.get(getScriptArgs()[0]);Files.createDirectories(output);DecompInterface dec=new DecompInterface();dec.openProgram(currentProgram);for(String address:new String[]{"18001f090","18001f0a0","18001f5a0"}){Function f=getFunctionAt(toAddr(address));if(f==null)continue;DecompileResults r=dec.decompileFunction(f,60,monitor);if(r.decompileCompleted())Files.writeString(output.resolve(address+".c"),r.getDecompiledFunction().getC());StringBuilder asm=new StringBuilder();InstructionIterator it=currentProgram.getListing().getInstructions(f.getBody(),true);while(it.hasNext()){Instruction i=it.next();asm.append(i.getAddress()).append(" ").append(i).append("\n");}Files.writeString(output.resolve(address+".asm.txt"),asm.toString());}dec.dispose();}
}
