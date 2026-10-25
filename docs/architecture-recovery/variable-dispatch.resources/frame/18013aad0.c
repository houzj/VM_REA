
void VM_M_SetBinayData(undefined4 param_1,undefined8 param_2,undefined8 param_3,undefined4 param_4)

{
  char cVar1;
  undefined4 uVar2;
  undefined8 uVar3;
  undefined8 *puVar4;
  undefined1 auStack_98 [32];
  undefined4 uStack_78;
  undefined8 *puStack_70;
  undefined8 uStack_68;
  undefined4 uStack_60;
  undefined8 auStack_58 [3];
  ulonglong uStack_40;
  ulonglong uStack_38;
  
                    /* 0x13aad0  180  VM_M_SetBinayData */
  uStack_38 = DAT_1802a5230 ^ (ulonglong)auStack_98;
  cVar1 = FUN_18001dbc0(1);
  if (cVar1 != '\0') {
    uVar3 = FUN_18001db50();
    puStack_70 = (undefined8 *)CONCAT44(puStack_70._4_4_,param_1);
    uStack_78 = 0x2a7;
    uStack_68 = param_2;
    uStack_60 = param_4;
    FUN_180015b40(uVar3,1,&DAT_180236c28,&DAT_180236c10);
  }
  if ((DAT_1802b12c0 & 1) == 0) {
    DAT_1802b12c0 = DAT_1802b12c0 | 1;
    FUN_18003fde0();
    FUN_1801e8930(&DAT_180202970);
  }
  uStack_78 = param_4;
  uVar2 = FUN_180043120(&DAT_1802b12d0,param_1,param_2,param_3);
  cVar1 = FUN_18001dbc0(1);
  if (cVar1 != '\0') {
    puVar4 = (undefined8 *)FUN_18008f750(auStack_58,uVar2);
    if (0xf < (ulonglong)puVar4[3]) {
      puVar4 = (undefined8 *)*puVar4;
    }
    uVar3 = FUN_18001db50();
    uStack_78 = 0x2ab;
    puStack_70 = puVar4;
    FUN_180015b40(uVar3,1,&DAT_180234258,&DAT_180236c10);
    if (0xf < uStack_40) {
      __3_YAXPEAX_Z(auStack_58[0]);
    }
  }
  FUN_1801e8810(uStack_38 ^ (ulonglong)auStack_98);
  return;
}

