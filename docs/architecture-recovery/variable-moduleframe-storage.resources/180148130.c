
char VM_M_SetGlobalVarLockStatus(undefined4 param_1,undefined4 param_2,undefined1 param_3)

{
  char cVar1;
  char cVar2;
  undefined8 uVar3;
  
                    /* 0x148130  196  VM_M_SetGlobalVarLockStatus */
  cVar1 = FUN_18001dbc0(0);
  if (cVar1 != '\0') {
    uVar3 = FUN_18001db50();
    FUN_180015b40(uVar3,0,&DAT_180239690,&DAT_180239770,0xb90,param_1,param_2,param_3);
  }
  if ((DAT_1802b2f70 & 1) == 0) {
    DAT_1802b2f70 = DAT_1802b2f70 | 1;
    FUN_180093820();
    FUN_1801e8930(FUN_180202e40);
  }
  cVar1 = FUN_18009a140(&DAT_1802b2f80,param_1,param_2,param_3);
  if (cVar1 == '\0') {
    cVar2 = FUN_18001dbc0(4);
    if (cVar2 != '\0') {
      uVar3 = FUN_18001db50();
      FUN_180015b40(uVar3,4,&DAT_1802396e0,&DAT_180239770,0xb95,param_1,param_2,param_3);
    }
  }
  cVar2 = FUN_18001dbc0(0);
  if (cVar2 != '\0') {
    uVar3 = FUN_18001db50();
    FUN_180015b40(uVar3,0,&DAT_180239790,&DAT_180239770,0xb97);
  }
  return cVar1;
}

