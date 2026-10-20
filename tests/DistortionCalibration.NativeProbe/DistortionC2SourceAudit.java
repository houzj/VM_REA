import ghidra.app.script.GhidraScript;
import ghidra.app.decompiler.*;
import ghidra.program.model.address.*;
import ghidra.program.model.listing.*;
import ghidra.program.model.data.*;
import ghidra.program.model.symbol.SourceType;
import java.nio.file.*;
public class DistortionC2SourceAudit extends GhidraScript {
 public void run() throws Exception {
  Path out=Paths.get(getScriptArgs()[0]);Files.createDirectories(out);var cookie=getFunctionAt(toAddr("181571b80"));if(cookie!=null)cookie.setNoReturn(false);
  String[][] ranges={{"1807fd4e0","1807fec3f"},{"1807fec40","1807ff23f"}};var notes=new StringBuilder();
  for(var range:ranges){var start=toAddr(range[0]);var end=toAddr(range[1]);var scope=new AddressSet(start,end);var manager=currentProgram.getFunctionManager();var others=manager.getFunctionsOverlapping(scope);var remove=new java.util.ArrayList<Address>();while(others.hasNext()){var f=others.next();if(!f.getEntryPoint().equals(start))remove.add(f.getEntryPoint());}for(var address:remove){notes.append("Removed inferred overlapping fragment in disposable analysis: ").append(address).append("\n");manager.removeFunction(address);}
   disassemble(start);if(range[0].equals("1807fec40")){disassemble(toAddr("1807ff1d7"));disassemble(toAddr("1807feede"));}else{disassemble(toAddr("1807febe1"));disassemble(toAddr("1807fe9eb"));}var f=getFunctionAt(start);if(f==null)f=createFunction(start,null);f.setNoReturn(false);f.setBody(scope);
   var continuations=new java.util.ArrayList<Address>();var scan=currentProgram.getListing().getInstructions(scope,true);while(scan.hasNext()){var i=scan.next();if(i.getMnemonicString().equals("CALL")&&i.toString().contains("181571b80"))continuations.add(i.getAddress().add(i.getLength()));}for(var address:continuations){disassemble(address);notes.append("Recovered security-cookie continuation: ").append(address).append("\n");}
   var dec=new DecompInterface();dec.openProgram(currentProgram);var d=dec.decompileFunction(f,60,monitor);if(!d.decompileCompleted())throw new Exception("No complete decompile "+range[0]);Files.writeString(out.resolve(range[0]+".c"),d.getDecompiledFunction().getC());var asm=new StringBuilder();var ins=currentProgram.getListing().getInstructions(scope,true);while(ins.hasNext()){var i=ins.next();asm.append(i.getAddress()).append("\t").append(i).append("\n");}Files.writeString(out.resolve(range[0]+"-asm.tsv"),asm.toString());dec.dispose();notes.append("PE .pdata runtime-function extent inclusive ").append(start).append("..").append(end).append("; includes cold blocks/padding, not proof every byte is executed\n");
  }
  byte[] constants=new byte[32];currentProgram.getMemory().getBytes(toAddr("18162b690"),constants);Files.write(out.resolve("constants-18162b690.bin"),constants);Files.writeString(out.resolve("identity.txt"),currentProgram.getExecutableSHA256()+"\n"+notes.toString());
 }
}


