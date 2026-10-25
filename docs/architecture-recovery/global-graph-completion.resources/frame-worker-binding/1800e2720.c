
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

int FUN_1800e2720(longlong param_1)

{
  int *piVar1;
  bool bVar2;
  bool bVar3;
  char cVar4;
  int iVar5;
  int iVar6;
  longlong *plVar7;
  undefined8 uVar8;
  undefined1 auStack_40 [8];
  longlong *plStack_38;
  undefined1 auStack_30 [8];
  longlong *plStack_28;
  
  bVar2 = false;
  iVar6 = 0;
  if (*(char *)(param_1 + 0xea) == '\0') {
    plVar7 = (longlong *)FUN_1800ffa60(param_1 + 200,auStack_40);
    bVar2 = true;
    if (*plVar7 == 0) goto LAB_1800e2776;
    bVar3 = false;
  }
  else {
LAB_1800e2776:
    bVar3 = true;
  }
  if ((bVar2) && (plStack_38 != (longlong *)0x0)) {
    LOCK();
    plVar7 = plStack_38 + 1;
    *(int *)plVar7 = (int)*plVar7 + -1;
    UNLOCK();
    if ((int)*plVar7 == 0) {
      (**(code **)*plStack_38)(plStack_38);
      LOCK();
      piVar1 = (int *)((longlong)plStack_38 + 0xc);
      *piVar1 = *piVar1 + -1;
      UNLOCK();
      if (*piVar1 == 0) {
        (**(code **)(*plStack_38 + 8))(plStack_38);
      }
    }
  }
  bVar2 = false;
  if (!bVar3) {
    return 0;
  }
  iVar5 = _Mtx_lock(param_1 + 0x108);
  if (iVar5 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar5);
  }
  if (*(char *)(param_1 + 0xea) == '\0') {
    plVar7 = (longlong *)FUN_1800ffa60(param_1 + 200,auStack_30);
    bVar2 = true;
    if (*plVar7 != 0) {
      bVar3 = false;
      goto LAB_1800e2802;
    }
  }
  bVar3 = true;
LAB_1800e2802:
  if ((bVar2) && (plStack_28 != (longlong *)0x0)) {
    LOCK();
    plVar7 = plStack_28 + 1;
    *(int *)plVar7 = (int)*plVar7 + -1;
    UNLOCK();
    if ((int)*plVar7 == 0) {
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
  if (bVar3) {
    if ((_DAT_1802b1888 & 1) == 0) {
      _DAT_1802b1888 = _DAT_1802b1888 | 1;
      FUN_180107fc0();
      FUN_1801e8930(&UNK_180202980);
    }
    iVar6 = FUN_18010ae80(&DAT_1802b1890,*(undefined4 *)(param_1 + 0x50));
    if ((iVar6 != 0) && (cVar4 = FUN_18001dbc0(4), cVar4 != '\0')) {
      uVar8 = FUN_18001db50();
      FUN_180015b40(uVar8,4,&UNK_18022d5a8,&UNK_18022d548,0x58c,*(undefined4 *)(param_1 + 0x50),
                    iVar6);
    }
    *(undefined1 *)(param_1 + 0xea) = 0;
    FUN_1800deba0(param_1);
  }
  iVar5 = _Mtx_unlock(param_1 + 0x108);
  if (iVar5 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar5);
  }
  return iVar6;
}

