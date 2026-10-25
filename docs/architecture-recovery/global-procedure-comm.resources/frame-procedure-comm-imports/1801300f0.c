
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void VM_M_GetInt(longlong param_1,longlong param_2,undefined4 param_3,longlong param_4,
                longlong param_5)

{
  char cVar1;
  undefined4 uVar2;
  undefined8 uVar3;
  undefined1 auStack_98 [32];
  longlong lStack_78;
  longlong lStack_70;
  undefined4 uStack_68;
  undefined8 *puStack_60;
  undefined8 auStack_58 [3];
  ulonglong uStack_40;
  ulonglong uStack_38;
  
                    /* 0x1300f0  106  VM_M_GetInt */
  uStack_38 = _DAT_1802a5230 ^ (ulonglong)auStack_98;
  if ((((param_1 != 0) && (param_2 != 0)) && (param_4 != 0)) && (param_5 != 0)) {
    uVar3 = FUN_18008e690();
    lStack_70 = param_5;
    lStack_78 = param_4;
    uVar2 = FUN_18011e580(uVar3,param_1,param_2,param_3);
    cVar1 = FUN_18001dbc0(0);
    if (cVar1 != '\0') {
      puStack_60 = (undefined8 *)FUN_18008f750(auStack_58,uVar2);
      if (0xf < (ulonglong)puStack_60[3]) {
        puStack_60 = (undefined8 *)*puStack_60;
      }
      uVar3 = FUN_18001db50();
      lStack_78 = CONCAT44(lStack_78._4_4_,0x81);
      lStack_70 = param_2;
      uStack_68 = param_3;
      FUN_180015b40(uVar3,0,&UNK_1802339e8,&UNK_1802339d8);
      if (0xf < uStack_40) {
        __3_YAXPEAX_Z(auStack_58[0]);
      }
    }
  }
  FUN_1801e8810(uStack_38 ^ (ulonglong)auStack_98);
  return;
}

