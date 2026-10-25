
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

int FUN_1800ddfc0(longlong param_1,undefined4 param_2,undefined8 param_3,undefined8 param_4)

{
  int *piVar1;
  bool bVar2;
  longlong *plVar3;
  char cVar4;
  int iVar5;
  undefined8 *puVar6;
  longlong lVar7;
  bool bVar8;
  longlong *plVar9;
  undefined8 uVar10;
  longlong lStack_50;
  longlong *plStack_48;
  undefined8 uStack_40;
  undefined8 uStack_38;
  undefined1 auStack_30 [8];
  longlong *plStack_28;
  
  uVar10 = 0xfffffffffffffffe;
  bVar8 = false;
  if ((_DAT_1802b1888 & 1) == 0) {
    _DAT_1802b1888 = _DAT_1802b1888 | 1;
    FUN_180107fc0();
    FUN_1801e8930(&UNK_180202980);
  }
  FUN_1801111d0(&DAT_1802b1890,&lStack_50,param_2,param_4,uVar10);
  if (lStack_50 == 0) {
    iVar5 = -0x1ffffcfe;
    plVar9 = plStack_48;
    goto LAB_1800de13f;
  }
  if (*(char *)(lStack_50 + 0x1b8) == '\0') {
    puVar6 = (undefined8 *)FUN_1800d33b0(auStack_30);
    bVar8 = true;
    cVar4 = FUN_1800d5900(*puVar6,param_2,*(undefined8 *)(param_1 + 0x198));
    if (cVar4 == '\0') goto LAB_1800de072;
    bVar2 = false;
  }
  else {
LAB_1800de072:
    bVar2 = true;
  }
  if ((bVar8) && (plStack_28 != (longlong *)0x0)) {
    LOCK();
    plVar9 = plStack_28 + 1;
    *(int *)plVar9 = (int)*plVar9 + -1;
    UNLOCK();
    if ((int)*plVar9 == 0) {
      (**(code **)*plStack_28)(plStack_28);
      LOCK();
      piVar1 = (int *)((longlong)plStack_28 + 0xc);
      *piVar1 = *piVar1 + -1;
      UNLOCK();
      if (*piVar1 == 0) {
        (**(code **)(*plStack_28 + 8))(plStack_28);
      }
    }
  }
  if (bVar2) {
    iVar5 = -0x1ffffd00;
    plVar9 = plStack_48;
  }
  else {
    iVar5 = FUN_1800e2720(param_1);
    plVar9 = plStack_48;
    if (iVar5 == 0) {
      iVar5 = *(int *)(param_1 + 0x50);
      lVar7 = FUN_18008e5d0();
      if (iVar5 == 100000) {
        *(undefined4 *)(lVar7 + 0x150) = 1;
      }
      else {
        *(undefined4 *)(lVar7 + (0x2b4b - (longlong)iVar5) * 4) = 1;
      }
      uVar10 = FUN_18008e5d0();
      FUN_18011a690(uVar10,param_2);
      plVar9 = plStack_48;
      uStack_40 = 0;
      uStack_38 = 0;
      if (plStack_48 != (longlong *)0x0) {
        LOCK();
        *(int *)(plStack_48 + 1) = (int)plStack_48[1] + 1;
        UNLOCK();
      }
      FUN_180015c40(&uStack_40,lStack_50,plStack_48);
      iVar5 = FUN_180100d70(param_1 + 200,&uStack_40);
    }
  }
LAB_1800de13f:
  plVar3 = plStack_48;
  if (plVar9 != (longlong *)0x0) {
    LOCK();
    plVar9 = plVar9 + 1;
    *(int *)plVar9 = (int)*plVar9 + -1;
    UNLOCK();
    if ((int)*plVar9 == 0) {
      (**(code **)*plStack_48)(plStack_48);
      LOCK();
      piVar1 = (int *)((longlong)plVar3 + 0xc);
      *piVar1 = *piVar1 + -1;
      UNLOCK();
      if (*piVar1 == 0) {
        (**(code **)(*plStack_48 + 8))();
      }
    }
  }
  return iVar5;
}

