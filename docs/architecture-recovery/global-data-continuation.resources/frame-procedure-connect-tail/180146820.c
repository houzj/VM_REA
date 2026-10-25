
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void VM_M_DisConnectProcedure(undefined4 param_1,undefined4 param_2)

{
  char cVar1;
  undefined4 uVar2;
  undefined8 uVar3;
  undefined8 *puVar4;
  undefined1 auStack_78 [32];
  undefined4 uStack_58;
  undefined8 *puStack_50;
  undefined4 uStack_48;
  undefined8 auStack_38 [3];
  ulonglong uStack_20;
  ulonglong uStack_18;
  
                    /* 0x146820  72  VM_M_DisConnectProcedure */
  uStack_18 = _DAT_1802a5230 ^ (ulonglong)auStack_78;
  if (DAT_1802b129e == '\0') {
    cVar1 = FUN_18001dbc0(2);
    if (cVar1 != '\0') {
      uVar3 = FUN_18001db50();
      puStack_50 = (undefined8 *)CONCAT44(puStack_50._4_4_,param_1);
      uStack_58 = 0xa99;
      uStack_48 = param_2;
      FUN_180015b40(uVar3,2,&UNK_180238fc0,&UNK_180238f98);
    }
    if ((_DAT_1802b3120 & 1) == 0) {
      _DAT_1802b3120 = _DAT_1802b3120 | 1;
      FUN_1800e3630();
      FUN_1801e8930(&UNK_180202f30);
    }
    uVar2 = FUN_1800e3dd0(&DAT_1802b3128,param_1,param_2);
    cVar1 = FUN_18001dbc0(2);
    if (cVar1 != '\0') {
      puVar4 = (undefined8 *)FUN_18008f750(auStack_38,uVar2);
      if (0xf < (ulonglong)puVar4[3]) {
        puVar4 = (undefined8 *)*puVar4;
      }
      uVar3 = FUN_18001db50();
      uStack_58 = 0xa9d;
      puStack_50 = puVar4;
      FUN_180015b40(uVar3,2,&UNK_180234258,&UNK_180238f98);
      if (0xf < uStack_20) {
        __3_YAXPEAX_Z(auStack_38[0]);
      }
    }
  }
  else {
    cVar1 = FUN_18001dbc0(4);
    if (cVar1 != '\0') {
      uVar3 = FUN_18001db50();
      uStack_58 = 0xa97;
      FUN_180015b40(uVar3,4,&UNK_180237cc0,&UNK_180238f98);
    }
  }
  FUN_1801e8810(uStack_18 ^ (ulonglong)auStack_78);
  return;
}

