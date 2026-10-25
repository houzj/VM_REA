
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void VM_M_StopRunProcedure(undefined4 param_1)

{
  char cVar1;
  undefined4 uVar2;
  undefined8 uVar3;
  undefined8 *puVar4;
  undefined1 auStack_68 [32];
  undefined4 uStack_48;
  undefined8 *puStack_40;
  undefined8 auStack_38 [3];
  ulonglong uStack_20;
  ulonglong uStack_18;
  
                    /* 0x13c250  233  VM_M_StopRunProcedure */
  uStack_18 = _DAT_1802a5230 ^ (ulonglong)auStack_68;
  cVar1 = FUN_18001dbc0(2);
  if (cVar1 != '\0') {
    uVar3 = FUN_18001db50();
    puStack_40 = (undefined8 *)CONCAT44(puStack_40._4_4_,param_1);
    uStack_48 = 0x37c;
    FUN_180015b40(uVar3,2,&UNK_180237040,&UNK_1802370e8);
  }
  if ((_DAT_1802b12c0 & 1) == 0) {
    _DAT_1802b12c0 = _DAT_1802b12c0 | 1;
    FUN_18003fde0();
    FUN_1801e8930(&UNK_180202970);
  }
  uVar2 = FUN_180043cb0(&DAT_1802b12d0,param_1);
  cVar1 = FUN_18001dbc0(2);
  if (cVar1 != '\0') {
    puVar4 = (undefined8 *)FUN_18008f750(auStack_38,uVar2);
    if (0xf < (ulonglong)puVar4[3]) {
      puVar4 = (undefined8 *)*puVar4;
    }
    uVar3 = FUN_18001db50();
    uStack_48 = 0x380;
    puStack_40 = puVar4;
    FUN_180015b40(uVar3,2,&UNK_180234258,&UNK_1802370e8);
    if (0xf < uStack_20) {
      __3_YAXPEAX_Z(auStack_38[0]);
    }
  }
  FUN_1801e8810(uStack_18 ^ (ulonglong)auStack_68);
  return;
}

