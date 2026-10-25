
void VM_M_BatchSetGlobalVarInt
               (longlong param_1,longlong param_2,longlong param_3,undefined4 param_4,
               undefined1 param_5)

{
  char cVar1;
  int iVar2;
  undefined8 uVar3;
  undefined1 auStack_98 [32];
  undefined4 uStack_78;
  longlong lStack_70;
  undefined4 uStack_68;
  undefined8 *puStack_60;
  undefined8 auStack_58 [3];
  ulonglong uStack_40;
  ulonglong uStack_38;
  
                    /* 0x134710  43  VM_M_BatchSetGlobalVarInt */
  uStack_38 = DAT_1802a5230 ^ (ulonglong)auStack_98;
  if (((param_1 != 0) && (param_2 != 0)) && (param_3 != 0)) {
    if ((DAT_1802b2b28 & 1) == 0) {
      DAT_1802b2b28 = DAT_1802b2b28 | 1;
      FUN_1801e8930(&DAT_1802029f0);
    }
    lStack_70 = CONCAT71(lStack_70._1_7_,param_5);
    uStack_78 = param_4;
    iVar2 = FUN_180126f10();
    if (iVar2 == 0) {
      cVar1 = FUN_18001dbc0(0);
      if (cVar1 == '\0') goto LAB_180134868;
      puStack_60 = (undefined8 *)FUN_18008f750(auStack_58,0);
      if (0xf < (ulonglong)puStack_60[3]) {
        puStack_60 = (undefined8 *)*puStack_60;
      }
      uVar3 = FUN_18001db50();
      uStack_78 = 0x425;
      lStack_70 = param_2;
      uStack_68 = param_4;
      FUN_180015b40(uVar3,0,&DAT_180234d20,&DAT_180234cf8);
    }
    else {
      cVar1 = FUN_18001dbc0(4);
      if (cVar1 == '\0') goto LAB_180134868;
      puStack_60 = (undefined8 *)FUN_18008f750(auStack_58,iVar2);
      if (0xf < (ulonglong)puStack_60[3]) {
        puStack_60 = (undefined8 *)*puStack_60;
      }
      uVar3 = FUN_18001db50();
      uStack_78 = 0x421;
      lStack_70 = param_2;
      uStack_68 = param_4;
      FUN_180015b40(uVar3,4,&DAT_180234d20,&DAT_180234cf8);
    }
    if (0xf < uStack_40) {
      __3_YAXPEAX_Z(auStack_58[0]);
    }
  }
LAB_180134868:
  FUN_1801e8810(uStack_38 ^ (ulonglong)auStack_98);
  return;
}

