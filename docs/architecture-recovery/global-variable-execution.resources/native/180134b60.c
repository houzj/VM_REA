
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void VM_M_GetGlobalVarInt
               (longlong param_1,longlong param_2,undefined4 param_3,undefined4 *param_4,
               longlong param_5)

{
  char cVar1;
  int iVar2;
  undefined8 uVar3;
  undefined1 auStack_a8 [32];
  undefined4 *puStack_88;
  longlong lStack_80;
  undefined4 uStack_78;
  undefined4 uStack_70;
  undefined8 *puStack_68;
  undefined8 auStack_58 [3];
  ulonglong uStack_40;
  ulonglong uStack_38;
  
                    /* 0x134b60  98  VM_M_GetGlobalVarInt */
  uStack_38 = _DAT_1802a5230 ^ (ulonglong)auStack_a8;
  if ((((param_1 != 0) && (param_2 != 0)) && (param_4 != (undefined4 *)0x0)) && (param_5 != 0)) {
    FUN_18008e690();
    lStack_80 = param_5;
    puStack_88 = param_4;
    iVar2 = FUN_180127470();
    if (iVar2 == 0) {
      cVar1 = FUN_18001dbc0(0);
      if (cVar1 == '\0') goto LAB_180134cbc;
      puStack_68 = (undefined8 *)FUN_18008f750(auStack_58,0);
      if (0xf < (ulonglong)puStack_68[3]) {
        puStack_68 = (undefined8 *)*puStack_68;
      }
      uVar3 = FUN_18001db50();
      uStack_70 = *param_4;
      puStack_88 = (undefined4 *)CONCAT44(puStack_88._4_4_,0x463);
      lStack_80 = param_2;
      uStack_78 = param_3;
      FUN_180015b40(uVar3,0,&UNK_180234f40,&UNK_180234f20);
    }
    else {
      cVar1 = FUN_18001dbc0(4);
      if (cVar1 == '\0') goto LAB_180134cbc;
      puStack_68 = (undefined8 *)FUN_18008f750(auStack_58,iVar2);
      if (0xf < (ulonglong)puStack_68[3]) {
        puStack_68 = (undefined8 *)*puStack_68;
      }
      uVar3 = FUN_18001db50();
      uStack_70 = *param_4;
      puStack_88 = (undefined4 *)CONCAT44(puStack_88._4_4_,0x45f);
      lStack_80 = param_2;
      uStack_78 = param_3;
      FUN_180015b40(uVar3,4,&UNK_180234f40,&UNK_180234f20);
    }
    if (0xf < uStack_40) {
      __3_YAXPEAX_Z(auStack_58[0]);
    }
  }
LAB_180134cbc:
  FUN_1801e8810(uStack_38 ^ (ulonglong)auStack_a8);
  return;
}

