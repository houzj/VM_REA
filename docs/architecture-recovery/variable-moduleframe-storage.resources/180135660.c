
void VM_M_DelGlobalVar(longlong param_1,longlong param_2)

{
  char cVar1;
  int iVar2;
  undefined8 uVar3;
  undefined1 auStack_78 [32];
  undefined4 uStack_58;
  longlong lStack_50;
  undefined8 *puStack_48;
  undefined8 auStack_38 [3];
  ulonglong uStack_20;
  ulonglong uStack_18;
  
                    /* 0x135660  64  VM_M_DelGlobalVar */
  uStack_18 = DAT_1802a5230 ^ (ulonglong)auStack_78;
  if ((param_1 != 0) && (param_2 != 0)) {
    if ((DAT_1802b2b28 & 1) == 0) {
      DAT_1802b2b28 = DAT_1802b2b28 | 1;
      FUN_1801e8930(&DAT_1802029f0);
    }
    iVar2 = FUN_180127e60();
    if (iVar2 == 0) {
      cVar1 = FUN_18001dbc0(0);
      if (cVar1 == '\0') goto LAB_18013579f;
      puStack_48 = (undefined8 *)FUN_18008f750(auStack_38,0);
      if (0xf < (ulonglong)puStack_48[3]) {
        puStack_48 = (undefined8 *)*puStack_48;
      }
      uVar3 = FUN_18001db50();
      uStack_58 = 0x505;
      lStack_50 = param_2;
      FUN_180015b40(uVar3,0,&DAT_180235190,&DAT_180235178);
    }
    else {
      cVar1 = FUN_18001dbc0(4);
      if (cVar1 == '\0') goto LAB_18013579f;
      puStack_48 = (undefined8 *)FUN_18008f750(auStack_38,iVar2);
      if (0xf < (ulonglong)puStack_48[3]) {
        puStack_48 = (undefined8 *)*puStack_48;
      }
      uVar3 = FUN_18001db50();
      uStack_58 = 0x501;
      lStack_50 = param_2;
      FUN_180015b40(uVar3,4,&DAT_180235190,&DAT_180235178);
    }
    if (0xf < uStack_20) {
      __3_YAXPEAX_Z(auStack_38[0]);
    }
  }
LAB_18013579f:
  FUN_1801e8810(uStack_18 ^ (ulonglong)auStack_78);
  return;
}

