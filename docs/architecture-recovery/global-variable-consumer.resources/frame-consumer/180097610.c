
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_180097610(void)

{
  longlong *plVar1;
  uint uVar2;
  longlong *plVar3;
  char cVar4;
  undefined4 uVar5;
  int iVar6;
  undefined8 uVar7;
  undefined8 *puVar8;
  int *piVar9;
  longlong lVar10;
  undefined4 in_R9D;
  longlong *in_stack_00000028;
  undefined8 *in_stack_00000030;
  undefined4 *in_stack_00000038;
  undefined1 auStack_c8 [32];
  undefined8 *puStack_a8;
  undefined4 *puStack_a0;
  undefined4 auStack_98 [2];
  longlong lStack_90;
  longlong *plStack_88;
  undefined8 uStack_80;
  undefined1 uStack_78;
  undefined7 uStack_77;
  undefined8 uStack_68;
  ulonglong uStack_60;
  undefined1 uStack_58;
  undefined7 uStack_57;
  undefined8 uStack_48;
  ulonglong uStack_40;
  ulonglong uStack_38;
  
  uStack_80 = 0xfffffffffffffffe;
  uStack_38 = _DAT_1802a5230 ^ (ulonglong)auStack_c8;
  if ((in_stack_00000030 == (undefined8 *)0x0) || (in_stack_00000038 == (undefined4 *)0x0)) {
    cVar4 = FUN_18001dbc0(4);
    if (cVar4 != '\0') {
      uVar7 = FUN_18001db50();
      puStack_a8 = (undefined8 *)CONCAT44(puStack_a8._4_4_,0x319);
      FUN_180015b40(uVar7,4,&UNK_1802246d0,&UNK_180226788);
    }
  }
  else {
    lVar10 = 0;
    auStack_98[0] = 0;
    piVar9 = (int *)*in_stack_00000028;
    iVar6 = *piVar9;
    if (iVar6 == 1) {
      uVar7 = atof(piVar9 + 1);
      *in_stack_00000030 = uVar7;
      auStack_98[0] = 1;
    }
    else if (iVar6 == 2) {
      uVar7 = FUN_18008e5d0();
      FUN_180108ba0(uVar7,&lStack_90,10999);
      if (lStack_90 == 0) {
        cVar4 = FUN_18001dbc0(4);
        if (cVar4 != '\0') {
          uVar7 = FUN_18001db50();
          puStack_a0 = (undefined4 *)CONCAT44(puStack_a0._4_4_,10999);
          puStack_a8 = (undefined8 *)CONCAT44(puStack_a8._4_4_,0x32d);
          FUN_180015b40(uVar7,4,&UNK_180224770,&UNK_180226788);
        }
      }
      else {
        puVar8 = (undefined8 *)FUN_18001ec90(lStack_90,&uStack_58);
        if (0xf < (ulonglong)puVar8[3]) {
          puVar8 = (undefined8 *)*puVar8;
        }
        uVar7 = atof(puVar8);
        *in_stack_00000030 = uVar7;
        if (0xf < uStack_40) {
          __3_YAXPEAX_Z(CONCAT71(uStack_57,uStack_58));
        }
        uStack_40 = 0xf;
        uStack_48 = 0;
        uStack_58 = 0;
        auStack_98[0] = 1;
      }
      plVar3 = plStack_88;
      if (plStack_88 != (longlong *)0x0) {
        LOCK();
        plVar1 = plStack_88 + 1;
        *(int *)plVar1 = (int)*plVar1 + -1;
        UNLOCK();
        if ((int)*plVar1 == 0) {
          (**(code **)*plStack_88)(plStack_88);
          LOCK();
          piVar9 = (int *)((longlong)plVar3 + 0xc);
          *piVar9 = *piVar9 + -1;
          UNLOCK();
          if (*piVar9 == 0) {
            (**(code **)(*plStack_88 + 8))();
          }
        }
      }
    }
    else if (iVar6 == 3) {
      iVar6 = piVar9[0x23];
      piVar9 = piVar9 + 0x24;
      uStack_60 = 0xf;
      uStack_68 = 0;
      uStack_78 = 0;
      if ((char)*piVar9 != '\0') {
        lVar10 = -1;
        do {
          lVar10 = lVar10 + 1;
        } while (*(char *)((longlong)piVar9 + lVar10) != '\0');
      }
      uVar7 = FUN_180006300(&uStack_78,piVar9,lVar10);
      uVar5 = FUN_180099be0(uVar7,*in_stack_00000028 + 4,in_R9D);
      uVar2 = *(uint *)(*in_stack_00000028 + 4);
      uVar7 = FUN_18008e6c0(0);
      puStack_a0 = auStack_98;
      puStack_a8 = in_stack_00000030;
      iVar6 = FUN_1800ca940(uVar7,iVar6,&uStack_78,uVar5);
      if (iVar6 == 0) {
        if ((uVar2 & 0xfffffffd) == 0) {
          auStack_98[0] = 1;
        }
      }
      else {
        cVar4 = FUN_18001dbc0(4);
        if (cVar4 != '\0') {
          uVar7 = FUN_18001db50();
          puStack_a8 = (undefined8 *)CONCAT44(puStack_a8._4_4_,0x347);
          FUN_180015b40(uVar7,4,&UNK_180226690,&UNK_180226788);
        }
      }
      if (0xf < uStack_60) {
        __3_YAXPEAX_Z(CONCAT71(uStack_77,uStack_78));
      }
    }
    else {
      cVar4 = FUN_18001dbc0(4);
      if (cVar4 != '\0') {
        uVar7 = FUN_18001db50();
        puStack_a8 = (undefined8 *)CONCAT44(puStack_a8._4_4_,0x34e);
        FUN_180015b40(uVar7,4,&UNK_180226718,&UNK_180226788);
      }
      auStack_98[0] = 0;
    }
    *in_stack_00000038 = auStack_98[0];
  }
  FUN_1801e8810(uStack_38 ^ (ulonglong)auStack_c8);
  return;
}

