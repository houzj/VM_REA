
void VM_M_SetImage(longlong param_1,longlong param_2,int param_3,undefined8 *param_4)

{
  uint uVar1;
  char cVar2;
  undefined4 uVar3;
  undefined8 uVar4;
  char cVar5;
  undefined1 auStack_b8 [32];
  ulonglong uStack_98;
  longlong lStack_90;
  ulonglong uStack_88;
  undefined4 uStack_80;
  int iStack_78;
  undefined8 *puStack_70;
  undefined8 auStack_68 [3];
  ulonglong uStack_50;
  ulonglong uStack_48;
  
                    /* 0x131190  201  VM_M_SetImage */
  uStack_48 = DAT_1802a5230 ^ (ulonglong)auStack_b8;
  if ((((param_1 != 0) && (param_2 != 0)) && (param_4 != (undefined8 *)0x0)) && (param_3 == 0)) {
    cVar5 = '\x01';
    if ((*(char *)((longlong)param_4 + 0xc) != '\x01') || (DAT_1802b129c != '\0')) {
      cVar5 = '\0';
    }
    uVar1 = *(uint *)(param_4 + 1);
    uVar4 = *param_4;
    FUN_18008e690();
    uStack_88 = 0;
    lStack_90 = CONCAT71(lStack_90._1_7_,cVar5);
    uStack_98 = (ulonglong)uVar1;
    uVar3 = FUN_180122a40(param_1,param_2,0,uVar4);
    cVar2 = FUN_18001dbc0(0);
    if (cVar2 != '\0') {
      puStack_70 = (undefined8 *)FUN_18008f750(auStack_68,uVar3);
      if (0xf < (ulonglong)puStack_70[3]) {
        puStack_70 = (undefined8 *)*puStack_70;
      }
      uVar4 = FUN_18001db50();
      uStack_80 = *(undefined4 *)(param_4 + 1);
      uStack_88 = uStack_88 & 0xffffffff00000000;
      uStack_98 = CONCAT44(uStack_98._4_4_,0x17c);
      lStack_90 = param_2;
      iStack_78 = (int)cVar5;
      FUN_180015b40(uVar4,0,&DAT_180233fe0,&DAT_180233f08);
      if (0xf < uStack_50) {
        __3_YAXPEAX_Z(auStack_68[0]);
      }
    }
  }
  FUN_1801e8810(uStack_48 ^ (ulonglong)auStack_b8);
  return;
}

