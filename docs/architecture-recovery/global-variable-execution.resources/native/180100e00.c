
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined8 FUN_180100e00(undefined8 param_1,longlong *param_2,longlong *param_3)

{
  longlong *plVar1;
  int *piVar2;
  int iVar3;
  undefined4 uVar4;
  longlong *plVar5;
  undefined8 uVar6;
  char cVar7;
  int iVar8;
  undefined8 uVar9;
  undefined8 *puVar10;
  undefined4 uVar11;
  undefined4 uVar12;
  undefined8 uVar13;
  longlong lVar14;
  longlong lStack_70;
  longlong *plStack_68;
  longlong lStack_60;
  longlong *plStack_58;
  undefined1 auStack_50 [8];
  longlong *plStack_48;
  
  lVar14 = *param_2;
  if (lVar14 == 0) {
    plVar5 = (longlong *)param_2[1];
    if (plVar5 != (longlong *)0x0) {
      LOCK();
      plVar1 = plVar5 + 1;
      *(int *)plVar1 = (int)*plVar1 + -1;
      UNLOCK();
      if ((int)*plVar1 == 0) {
        (**(code **)*plVar5)(plVar5);
        LOCK();
        piVar2 = (int *)((longlong)plVar5 + 0xc);
        *piVar2 = *piVar2 + -1;
        UNLOCK();
        if (*piVar2 == 0) {
          (**(code **)(*plVar5 + 8))(plVar5);
        }
      }
    }
    plVar5 = (longlong *)param_3[3];
    if (plVar5 != (longlong *)0x0) {
      (**(code **)(*plVar5 + 0x20))(plVar5,plVar5 != param_3);
      param_3[3] = 0;
    }
    plVar5 = (longlong *)param_3[3];
    if (plVar5 != (longlong *)0x0) {
      (**(code **)(*plVar5 + 0x20))(plVar5,plVar5 != param_3);
      param_3[3] = 0;
    }
    return 0xe0000001;
  }
  uVar6 = *(undefined8 *)(lVar14 + 0xf8);
  iVar3 = *(int *)(lVar14 + 0x28);
  if ((_DAT_1802b1888 & 1) == 0) {
    _DAT_1802b1888 = _DAT_1802b1888 | 1;
    FUN_180107fc0();
    FUN_1801e8930(&UNK_180202980);
  }
  FUN_180108ba0(&DAT_1802b1890,&lStack_70,iVar3);
  lVar14 = lStack_70;
  if ((lStack_70 != 0) && (*(char *)(lStack_70 + 0x130) == '\0')) {
    uVar9 = FUN_18008e5d0();
    FUN_18010ae80(uVar9,iVar3);
  }
  if ((_DAT_1802b1888 & 1) == 0) {
    _DAT_1802b1888 = _DAT_1802b1888 | 1;
    FUN_180107fc0();
    FUN_1801e8930(&UNK_180202980);
  }
  if (iVar3 == 100000) {
    _DAT_1802b19e0 = 1;
  }
  else {
    ((_IMAGE_RUNTIME_FUNCTION_ENTRY *)
    ((longlong)&_IMAGE_RUNTIME_FUNCTION_ENTRY_1802bc5bc + (longlong)iVar3 * -4))->BeginAddress = 1;
  }
  uVar11 = *(undefined4 *)(*param_2 + 0x2c);
  uVar9 = FUN_18008e6c0(uVar6);
  FUN_1800cb9e0(uVar9,uVar11);
  if ((_DAT_1802b30c0 & 1) == 0) {
    _DAT_1802b30c0 = _DAT_1802b30c0 | 1;
    FUN_1801e8930(&UNK_180202f10);
  }
  iVar8 = FUN_1800b98c0();
  if (iVar8 == 0) {
    iVar8 = (**(code **)(*(longlong *)*param_2 + 8))();
    if ((iVar8 == 0) || (iVar8 == 5)) {
      uVar11 = *(undefined4 *)(*param_2 + 0x7c);
      uVar12 = *(undefined4 *)(*param_2 + 0x2c);
      uVar9 = FUN_18008e6c0(uVar6);
      uVar13 = 0;
      goto LAB_180101037;
    }
  }
  else {
    *(undefined4 *)(*param_2 + 0x84) = 3;
    *(int *)(*param_2 + 0x7c) = iVar8;
    cVar7 = FUN_18001dbc0(1);
    if (cVar7 != '\0') {
      uVar11 = *(undefined4 *)(*param_2 + 0x2c);
      uVar9 = FUN_18001db50();
      FUN_180015b40(uVar9,1,&UNK_18022fab0,&UNK_18022faf8,0x178,uVar11);
    }
  }
  uVar11 = *(undefined4 *)(*param_2 + 0x7c);
  uVar12 = *(undefined4 *)(*param_2 + 0x2c);
  uVar9 = FUN_18008e6c0(uVar6);
  uVar13 = 1;
LAB_180101037:
  FUN_1800cb8f0(uVar9,uVar12,uVar13,uVar11);
  uVar11 = *(undefined4 *)(*param_2 + 0x2c);
  uVar9 = FUN_18008e6c0(uVar6);
  FUN_1800cae00(uVar9,&lStack_60,uVar11);
  plVar5 = plStack_58;
  if (lStack_60 != 0) {
    *(undefined4 *)(lStack_60 + 0x74) = 2;
  }
  if (plStack_58 != (longlong *)0x0) {
    LOCK();
    plVar1 = plStack_58 + 1;
    *(int *)plVar1 = (int)*plVar1 + -1;
    UNLOCK();
    lVar14 = lStack_70;
    if ((int)*plVar1 == 0) {
      (**(code **)*plStack_58)(plStack_58);
      LOCK();
      piVar2 = (int *)((longlong)plVar5 + 0xc);
      *piVar2 = *piVar2 + -1;
      UNLOCK();
      lVar14 = lStack_70;
      if (*piVar2 == 0) {
        (**(code **)(*plStack_58 + 8))();
        lVar14 = lStack_70;
      }
    }
  }
  uVar11 = *(undefined4 *)(lVar14 + 0xe0);
  uVar12 = *(undefined4 *)(*param_2 + 0x2c);
  uVar4 = *(undefined4 *)(*param_2 + 0x28);
  uVar9 = FUN_18008e6c0(uVar6);
  FUN_1800ccfc0(uVar9,uVar4,uVar12);
  uVar12 = *(undefined4 *)(lVar14 + 0xe0);
  uVar9 = FUN_18008e6c0(0);
  FUN_1800ce740(uVar9,iVar3,uVar12);
  if (*(int *)(*param_2 + 0x30) == 3) {
    puVar10 = (undefined8 *)FUN_1800d33b0(auStack_50);
    FUN_1800d34a0(*puVar10,*(undefined4 *)(*param_2 + 0x28),*(undefined4 *)(*param_2 + 0x2c),uVar6,
                  uVar11);
    if (plStack_48 != (longlong *)0x0) {
      LOCK();
      plVar5 = plStack_48 + 1;
      *(int *)plVar5 = (int)*plVar5 + -1;
      UNLOCK();
      if ((int)*plVar5 == 0) {
        (**(code **)*plStack_48)(plStack_48);
        LOCK();
        piVar2 = (int *)((longlong)plStack_48 + 0xc);
        *piVar2 = *piVar2 + -1;
        UNLOCK();
        if (*piVar2 == 0) {
          (**(code **)(*plStack_48 + 8))(plStack_48);
        }
      }
    }
  }
  uVar11 = *(undefined4 *)(*param_2 + 0x2c);
  if ((_DAT_1802b2f70 & 1) == 0) {
    _DAT_1802b2f70 = _DAT_1802b2f70 | 1;
    FUN_180093820();
    FUN_1801e8930(FUN_180202e40);
  }
  FUN_180093b20(&DAT_1802b2f80,uVar11);
  if ((longlong *)param_3[3] != (longlong *)0x0) {
    (**(code **)(*(longlong *)param_3[3] + 0x10))();
  }
  plVar5 = plStack_68;
  if (plStack_68 != (longlong *)0x0) {
    LOCK();
    plVar1 = plStack_68 + 1;
    *(int *)plVar1 = (int)*plVar1 + -1;
    UNLOCK();
    if ((int)*plVar1 == 0) {
      (**(code **)*plStack_68)(plStack_68);
      LOCK();
      piVar2 = (int *)((longlong)plVar5 + 0xc);
      *piVar2 = *piVar2 + -1;
      UNLOCK();
      if (*piVar2 == 0) {
        (**(code **)(*plStack_68 + 8))();
      }
    }
  }
  plVar5 = (longlong *)param_2[1];
  if (plVar5 != (longlong *)0x0) {
    LOCK();
    plVar1 = plVar5 + 1;
    *(int *)plVar1 = (int)*plVar1 + -1;
    UNLOCK();
    if ((int)*plVar1 == 0) {
      (**(code **)*plVar5)(plVar5);
      LOCK();
      piVar2 = (int *)((longlong)plVar5 + 0xc);
      *piVar2 = *piVar2 + -1;
      UNLOCK();
      if (*piVar2 == 0) {
        (**(code **)(*plVar5 + 8))(plVar5);
      }
    }
  }
  plVar5 = (longlong *)param_3[3];
  if (plVar5 != (longlong *)0x0) {
    (**(code **)(*plVar5 + 0x20))(plVar5,plVar5 != param_3);
    param_3[3] = 0;
  }
  plVar5 = (longlong *)param_3[3];
  if (plVar5 != (longlong *)0x0) {
    (**(code **)(*plVar5 + 0x20))(plVar5,plVar5 != param_3);
    param_3[3] = 0;
  }
  return 0;
}

