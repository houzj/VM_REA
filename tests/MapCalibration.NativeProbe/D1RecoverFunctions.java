import ghidra.app.script.GhidraScript;

/** Define only explicitly requested missing code entries in this read-only
 * session. The headless project discards these database annotations on exit;
 * neither target bytes nor the saved project are modified. */
public class D1RecoverFunctions extends GhidraScript {
 public void run() throws Exception {
  String[] args=getScriptArgs();
  for(int i=1;i<args.length;i++){
   var address=toAddr(args[i]);
   if(getFunctionAt(address)==null){
    if(getInstructionAt(address)==null)disassemble(address);
    var function=createFunction(address,"D1_"+args[i]);
    if(function==null)throw new IllegalStateException("Could not define requested entry "+args[i]);
    println("D1 temporary function entry "+address);
   }
  }
  runScript("MapCalibrationAudit.java",args);
 }
}
