
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_180098150(void)

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
  undefined1 auStack_98 [32];
  undefined8 uStack_78;
  longlong lStack_70;
  undefined4 *puStack_68;
  undefined4 auStack_58 [2];
  undefined8 uStack_50;
  undefined1 uStack_48;
  undefined7 uStack_47;
  undefined8 uStack_38;
  ulonglong uStack_30;
  ulonglong uStack_28;
  
  uStack_50 = 0xfffffffffffffffe;
  uStack_28 = _DAT_1802a5230 ^ (ulonglong)auStack_98;
  if ((in_stack_00000038 == 0) || (in_stack_00000040 == (undefined4 *)0x0)) {
    cVar2 = FUN_18001dbc0(4);
    if (cVar2 != '\0') {
      uVar5 = FUN_18001db50();
      uStack_78 = CONCAT44(uStack_78._4_4_,0x420);
      FUN_180015b40(uVar5,4,&UNK_1802246d0,&UNK_180226848);
    }
  }
  else {
    lVar7 = 0;
    uVar3 = 0;
    auStack_58[0] = 0;
    piVar6 = (int *)*in_stack_00000028;
    if (*piVar6 == 3) {
      iVar4 = piVar6[0x23];
      piVar6 = piVar6 + 0x24;
      uStack_30 = 0xf;
      uStack_38 = 0;
      uStack_48 = 0;
      if ((char)*piVar6 != '\0') {
        lVar7 = -1;
        do {
          lVar7 = lVar7 + 1;
        } while (*(char *)((longlong)piVar6 + lVar7) != '\0');
      }
      FUN_180006300(&uStack_48,piVar6,lVar7);
      uVar3 = FUN_180099be0();
      uVar1 = *(uint *)(*in_stack_00000028 + 4);
      uVar5 = FUN_18008e6c0(0);
      puStack_68 = auStack_58;
      lStack_70 = in_stack_00000038;
      uStack_78 = in_stack_00000030;
      iVar4 = FUN_1800cacd0(uVar5,iVar4,&uStack_48,uVar3);
      if (iVar4 == 0) {
        uVar3 = auStack_58[0];
        if ((uVar1 & 0xfffffffd) == 0) {
          uVar3 = 1;
        }
      }
      else {
        cVar2 = FUN_18001dbc0(4);
        uVar3 = auStack_58[0];
        if (cVar2 != '\0') {
          uVar5 = FUN_18001db50();
          uStack_78 = CONCAT44(uStack_78._4_4_,0x43a);
          FUN_180015b40(uVar5,4,&UNK_180226690,&UNK_180226848);
          uVar3 = auStack_58[0];
        }
      }
      if (0xf < uStack_30) {
        __3_YAXPEAX_Z(CONCAT71(uStack_47,uStack_48));
      }
    }
    else {
      cVar2 = FUN_18001dbc0(4);
      if (cVar2 != '\0') {
        uVar5 = FUN_18001db50();
        uStack_78 = CONCAT44(uStack_78._4_4_,0x441);
        FUN_180015b40(uVar5,4,&UNK_180226718,&UNK_180226848);
      }
    }
    *in_stack_00000040 = uVar3;
  }
  FUN_1801e8810(uStack_28 ^ (ulonglong)auStack_98);
  return;
}

