
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void VM_M_SetReportStateOfBindingVar(undefined4 param_1,byte param_2)

{
  char cVar1;
  undefined4 uVar2;
  undefined8 uVar3;
  undefined8 *puVar4;
  undefined1 auStack_78 [32];
  undefined4 uStack_58;
  undefined8 *puStack_50;
  uint uStack_48;
  undefined8 auStack_38 [3];
  ulonglong uStack_20;
  ulonglong uStack_18;
  
                    /* 0x147ae0  227  VM_M_SetReportStateOfBindingVar */
  uStack_18 = _DAT_1802a5230 ^ (ulonglong)auStack_78;
  cVar1 = FUN_18001dbc0(2);
  if (cVar1 != '\0') {
    uVar3 = FUN_18001db50();
    uStack_48 = (uint)param_2;
    puStack_50 = (undefined8 *)CONCAT44(puStack_50._4_4_,param_1);
    uStack_58 = 0xb57;
    FUN_180015b40(uVar3,2,&UNK_1802394e0,&UNK_180239560);
  }
  if ((_DAT_1802b2f70 & 1) == 0) {
    _DAT_1802b2f70 = _DAT_1802b2f70 | 1;
    FUN_180093820();
    FUN_1801e8930(FUN_180202e40);
  }
  uVar2 = FUN_18009a000(&DAT_1802b2f80,param_1,param_2);
  cVar1 = FUN_18001dbc0(2);
  if (cVar1 != '\0') {
    puVar4 = (undefined8 *)FUN_18008f750(auStack_38,uVar2);
    if (0xf < (ulonglong)puVar4[3]) {
      puVar4 = (undefined8 *)*puVar4;
    }
    uVar3 = FUN_18001db50();
    uStack_58 = 0xb5b;
    puStack_50 = puVar4;
    FUN_180015b40(uVar3,2,&UNK_180234258,&UNK_180239560);
    if (0xf < uStack_20) {
      __3_YAXPEAX_Z(auStack_38[0]);
    }
  }
  FUN_1801e8810(uStack_18 ^ (ulonglong)auStack_78);
  return;
}

