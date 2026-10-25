
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_180097d10(void)

{
  uint uVar1;
  char cVar2;
  undefined4 uVar3;
  int iVar4;
  undefined8 uVar5;
  int *piVar6;
  longlong lVar7;
  longlong *in_stack_00000028;
  undefined8 in_stack_00000030;
  longlong in_stack_00000038;
  undefined4 *in_stack_00000040;
  undefined8 in_stack_00000048;
  undefined1 auStack_b8 [32];
  undefined8 uStack_98;
  longlong lStack_90;
  undefined4 *puStack_88;
  undefined8 uStack_80;
  undefined8 uStack_78;
  undefined4 auStack_68 [2];
  undefined8 uStack_60;
  undefined1 uStack_58;
  undefined7 uStack_57;
  undefined8 uStack_48;
  ulonglong uStack_40;
  ulonglong uStack_38;
  
  uStack_60 = 0xfffffffffffffffe;
  uStack_38 = _DAT_1802a5230 ^ (ulonglong)auStack_b8;
  if ((in_stack_00000038 == 0) || (in_stack_00000040 == (undefined4 *)0x0)) {
    cVar2 = FUN_18001dbc0(4);
    if (cVar2 != '\0') {
      uVar5 = FUN_18001db50();
      uStack_98 = CONCAT44(uStack_98._4_4_,0x3bc);
      FUN_180015b40(uVar5,4,&UNK_1802246d0,&UNK_1802267e8);
    }
  }
  else {
    lVar7 = 0;
    uVar3 = 0;
    auStack_68[0] = 0;
    piVar6 = (int *)*in_stack_00000028;
    if (*piVar6 == 3) {
      iVar4 = piVar6[0x23];
      piVar6 = piVar6 + 0x24;
      uStack_40 = 0xf;
      uStack_48 = 0;
      uStack_58 = 0;
      if ((char)*piVar6 != '\0') {
        lVar7 = -1;
        do {
          lVar7 = lVar7 + 1;
        } while (*(char *)((longlong)piVar6 + lVar7) != '\0');
      }
      FUN_180006300(&uStack_58,piVar6,lVar7);
      uVar3 = FUN_180099be0();
      uVar1 = *(uint *)(*in_stack_00000028 + 4);
      uVar5 = FUN_18008e6c0(0);
      uStack_78 = 0x40;
      uStack_80 = in_stack_00000048;
      puStack_88 = auStack_68;
      lStack_90 = in_stack_00000038;
      uStack_98 = in_stack_00000030;
      iVar4 = FUN_1800caa60(uVar5,iVar4,&uStack_58,uVar3);
      if (iVar4 == 0) {
        uVar3 = auStack_68[0];
        if ((uVar1 & 0xfffffffd) == 0) {
          uVar3 = 1;
        }
      }
      else {
        cVar2 = FUN_18001dbc0(4);
        uVar3 = auStack_68[0];
        if (cVar2 != '\0') {
          uVar5 = FUN_18001db50();
          uStack_98 = CONCAT44(uStack_98._4_4_,0x3d6);
          FUN_180015b40(uVar5,4,&UNK_180226690,&UNK_1802267e8);
          uVar3 = auStack_68[0];
        }
      }
      if (0xf < uStack_40) {
        __3_YAXPEAX_Z(CONCAT71(uStack_57,uStack_58));
      }
    }
    else {
      cVar2 = FUN_18001dbc0(4);
      if (cVar2 != '\0') {
        uVar5 = FUN_18001db50();
        uStack_98 = CONCAT44(uStack_98._4_4_,0x3dd);
        FUN_180015b40(uVar5,4,&UNK_180226718,&UNK_1802267e8);
      }
    }
    *in_stack_00000040 = uVar3;
  }
  FUN_1801e8810(uStack_38 ^ (ulonglong)auStack_b8);
  return;
}

