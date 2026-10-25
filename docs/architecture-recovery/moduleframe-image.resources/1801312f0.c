
void VM_M_SetImageEx(longlong param_1,longlong param_2,undefined4 param_3,undefined8 *param_4,
                    longlong param_5)

{
  uint uVar1;
  char cVar2;
  undefined4 uVar3;
  undefined8 uVar4;
  char cVar5;
  undefined1 auStack_c8 [32];
  ulonglong uStack_a8;
  longlong lStack_a0;
  longlong lStack_98;
  undefined4 uStack_90;
  int iStack_88;
  undefined8 *puStack_80;
  undefined8 auStack_78 [3];
  ulonglong uStack_60;
  ulonglong uStack_58;
  
                    /* 0x1312f0  203  VM_M_SetImageEx */
  uStack_58 = DAT_1802a5230 ^ (ulonglong)auStack_c8;
  if ((((param_1 != 0) && (param_2 != 0)) && (param_4 != (undefined8 *)0x0)) && (param_5 != 0)) {
    cVar5 = '\x01';
    if ((*(char *)((longlong)param_4 + 0xc) != '\x01') || (DAT_1802b129c != '\0')) {
      cVar5 = '\0';
    }
    uVar1 = *(uint *)(param_4 + 1);
    uVar4 = *param_4;
    FUN_18008e690();
    lStack_98 = param_5;
    lStack_a0 = CONCAT71(lStack_a0._1_7_,cVar5);
    uStack_a8 = (ulonglong)uVar1;
    uVar3 = FUN_180122a40(param_1,param_2,param_3,uVar4);
    cVar2 = FUN_18001dbc0(0);
    if (cVar2 != '\0') {
      puStack_80 = (undefined8 *)FUN_18008f750(auStack_78,uVar3);
      if (0xf < (ulonglong)puStack_80[3]) {
        puStack_80 = (undefined8 *)*puStack_80;
      }
      uVar4 = FUN_18001db50();
      uStack_90 = *(undefined4 *)(param_4 + 1);
      lStack_98 = CONCAT44(lStack_98._4_4_,param_3);
      uStack_a8 = CONCAT44(uStack_a8._4_4_,0x191);
      lStack_a0 = param_2;
      iStack_88 = (int)cVar5;
      FUN_180015b40(uVar4,0,&DAT_180233fe0,&DAT_180234038);
      if (0xf < uStack_60) {
        __3_YAXPEAX_Z(auStack_78[0]);
      }
    }
  }
  FUN_1801e8810(uStack_58 ^ (ulonglong)auStack_c8);
  return;
}

