
void VM_M_SetGlobalVarString(longlong param_1,longlong param_2,undefined4 param_3,longlong param_4)

{
  char cVar1;
  int iVar2;
  undefined8 uVar3;
  undefined1 auStack_a8 [32];
  longlong lStack_88;
  longlong lStack_80;
  undefined4 uStack_78;
  longlong lStack_70;
  undefined8 *puStack_68;
  undefined8 auStack_58 [3];
  ulonglong uStack_40;
  ulonglong uStack_38;
  
                    /* 0x1340e0  198  VM_M_SetGlobalVarString */
  uStack_38 = DAT_1802a5230 ^ (ulonglong)auStack_a8;
  if (((param_1 != 0) && (param_2 != 0)) && (param_4 != 0)) {
    if ((DAT_1802b2b28 & 1) == 0) {
      DAT_1802b2b28 = DAT_1802b2b28 | 1;
      FUN_1801e8930(&DAT_1802029f0);
    }
    lStack_88 = param_4;
    iVar2 = FUN_1801269f0();
    if (iVar2 == 0) {
      cVar1 = FUN_18001dbc0(0);
      if (cVar1 == '\0') goto LAB_18013424d;
      puStack_68 = (undefined8 *)FUN_18008f750(auStack_58,0);
      if (0xf < (ulonglong)puStack_68[3]) {
        puStack_68 = (undefined8 *)*puStack_68;
      }
      uVar3 = FUN_18001db50();
      lStack_88 = CONCAT44(lStack_88._4_4_,0x3cf);
      lStack_80 = param_2;
      uStack_78 = param_3;
      lStack_70 = param_4;
      FUN_180015b40(uVar3,0,&DAT_180234b50,&DAT_180234b30);
    }
    else {
      cVar1 = FUN_18001dbc0(4);
      if (cVar1 == '\0') goto LAB_18013424d;
      puStack_68 = (undefined8 *)FUN_18008f750(auStack_58,iVar2);
      if (0xf < (ulonglong)puStack_68[3]) {
        puStack_68 = (undefined8 *)*puStack_68;
      }
      uVar3 = FUN_18001db50();
      lStack_88 = CONCAT44(lStack_88._4_4_,0x3cb);
      lStack_80 = param_2;
      uStack_78 = param_3;
      lStack_70 = param_4;
      FUN_180015b40(uVar3,4,&DAT_180234b50,&DAT_180234b30);
    }
    if (0xf < uStack_40) {
      __3_YAXPEAX_Z(auStack_58[0]);
    }
  }
LAB_18013424d:
  FUN_1801e8810(uStack_38 ^ (ulonglong)auStack_a8);
  return;
}

