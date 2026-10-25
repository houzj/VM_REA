
void VM_M_GetImageEx(longlong param_1,undefined8 *param_2,undefined4 param_3,longlong param_4,
                    longlong param_5)

{
  char cVar1;
  undefined4 uVar2;
  undefined8 uVar3;
  longlong *plVar4;
  undefined1 auStack_a8 [32];
  longlong lStack_88;
  undefined8 *puStack_80;
  longlong lStack_78;
  longlong *plStack_70;
  undefined8 uStack_68;
  undefined8 uStack_58;
  undefined8 auStack_50 [3];
  ulonglong uStack_38;
  ulonglong uStack_30;
  
                    /* 0x130530  103  VM_M_GetImageEx */
  uStack_30 = DAT_1802a5230 ^ (ulonglong)auStack_a8;
  if ((((param_1 != 0) && (param_2 != (undefined8 *)0x0)) && (param_4 != 0)) && (param_5 != 0)) {
    uStack_58 = 0;
    uVar3 = FUN_18008e690();
    uStack_68 = 0x40;
    plStack_70 = (longlong *)(param_4 + 0xd);
    puStack_80 = &uStack_58;
    lStack_78 = param_5;
    lStack_88 = param_4;
    uVar2 = FUN_180121720(uVar3,param_1,param_2,param_3);
    *(undefined4 *)(param_4 + 8) = (undefined4)uStack_58;
    cVar1 = FUN_18001dbc0(0);
    if (cVar1 != '\0') {
      plVar4 = (longlong *)FUN_18008f750(auStack_50,uVar2);
      if (0xf < (ulonglong)plVar4[3]) {
        plVar4 = (longlong *)*plVar4;
      }
      uVar3 = FUN_18001db50();
      lStack_78 = CONCAT44(lStack_78._4_4_,param_3);
      lStack_88 = CONCAT44(lStack_88._4_4_,0xc6);
      puStack_80 = param_2;
      plStack_70 = plVar4;
      FUN_180015b40(uVar3,0,&DAT_180233b68,&DAT_180233b30);
      if (0xf < uStack_38) {
        __3_YAXPEAX_Z(auStack_50[0]);
      }
    }
  }
  FUN_1801e8810(uStack_30 ^ (ulonglong)auStack_a8);
  return;
}

