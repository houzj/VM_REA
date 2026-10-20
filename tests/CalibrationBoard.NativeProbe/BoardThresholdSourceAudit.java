import ghidra.app.script.GhidraScript;
import ghidra.program.model.address.AddressSet;
import java.nio.file.*;

// Read-only address-range export: function-body membership may omit jump-table cases.
public class BoardThresholdSourceAudit extends GhidraScript {
 public void run() throws Exception {
  Path out=Paths.get(getScriptArgs()[0]);Files.createDirectories(out);
  byte[] table=new byte[18];currentProgram.getMemory().getBytes(toAddr("181642f38"),table);
  Files.write(out.resolve("181642f38.bin"),table);
  StringBuilder targets=new StringBuilder();
  for(int j=0;j<9;j++){
   long address=0x1808156ddL+((table[j*2]&255)|((table[j*2+1]&255)<<8));
   var target=toAddr(address);disassemble(target);
   targets.append(j+1).append("\t").append(target).append("\n");
  }
  Files.writeString(out.resolve("switch-targets.tsv"),targets.toString());
  String[][] ranges={{"180815560","1808162e0"},{"180873de0","180874b00"}};
  for(String[] range:ranges){
   StringBuilder listing=new StringBuilder();
   var instructions=currentProgram.getListing().getInstructions(new AddressSet(toAddr(range[0]),toAddr(range[1])),true);
   while(instructions.hasNext()){var i=instructions.next();listing.append(i.getAddress()).append(" ").append(i).append("\n");}
   Files.writeString(out.resolve(range[0]+"-range.asm"),listing.toString());
  }
  for(String address:new String[]{"181644a14","181628c24","181628bb4","181642dc0"}){
   byte[] bytes=new byte[8];currentProgram.getMemory().getBytes(toAddr(address),bytes);
   Files.write(out.resolve(address+".bin"),bytes);
  }
  Files.writeString(out.resolve("done.txt"),"read-only address ranges and constants exported\n");
 }
}
