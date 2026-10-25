
void VM_M_SetGlobalVarFloat(longlong param_1,longlong param_2,undefined4 param_3,float param_4)

{
  char cVar1;
  int iVar2;
  undefined8 uVar3;
  undefined1 auStack_b8 [32];
  float fStack_98;
  longlong lStack_90;
  undefined4 uStack_88;
  double dStack_80;
  undefined8 *puStack_78;
  undefined8 auStack_68 [3];
  ulonglong uStack_50;
  ulonglong uStack_48;
  
                    /* 0x133e30  193  VM_M_SetGlobalVarFloat */
  uStack_48 = DAT_1802a5230 ^ (ulonglong)auStack_b8;
  if ((param_1 != 0) && (param_2 != 0)) {
    if ((DAT_1802b2b28 & 1) == 0) {
      DAT_1802b2b28 = DAT_1802b2b28 | 1;
      FUN_1801e8930(&DAT_1802029f0);
    }
    fStack_98 = param_4;
    iVar2 = FUN_180126780();
    if (iVar2 == 0) {
      cVar1 = FUN_18001dbc0(0);
      if (cVar1 == '\0') goto LAB_180133f9e;
      puStack_78 = (undefined8 *)FUN_18008f750(auStack_68,0);
      if (0xf < (ulonglong)puStack_78[3]) {
        puStack_78 = (undefined8 *)*puStack_78;
      }
      uVar3 = FUN_18001db50();
      fStack_98 = 1.31862e-42;
      lStack_90 = param_2;
      uStack_88 = param_3;
      dStack_80 = (double)param_4;
      FUN_180015b40(uVar3,0,&DAT_180234a70,&DAT_180234a58);
    }
    else {
      cVar1 = FUN_18001dbc0(4);
      if (cVar1 == '\0') goto LAB_180133f9e;
      puStack_78 = (undefined8 *)FUN_18008f750(auStack_68,iVar2);
      if (0xf < (ulonglong)puStack_78[3]) {
        puStack_78 = (undefined8 *)*puStack_78;
      }
      uVar3 = FUN_18001db50();
      fStack_98 = 1.31302e-42;
      lStack_90 = param_2;
      uStack_88 = param_3;
      dStack_80 = (double)param_4;
      FUN_180015b40(uVar3,4,&DAT_180234a70,&DAT_180234a58);
    }
    if (0xf < uStack_50) {
      __3_YAXPEAX_Z(auStack_68[0]);
    }
  }
LAB_180133f9e:
  FUN_1801e8810(uStack_48 ^ (ulonglong)auStack_b8);
  return;
}

