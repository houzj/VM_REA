
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

int FUN_180619670(undefined8 param_1,longlong *param_2,longlong param_3,longlong param_4,
                 longlong *param_5)

{
  longlong *plVar1;
  int *piVar2;
  longlong lVar3;
  longlong lVar4;
  char cVar5;
  undefined4 uVar6;
  int iVar7;
  undefined8 uVar8;
  undefined8 *puVar9;
  longlong *plVar10;
  longlong lVar11;
  undefined8 in_stack_ffffffffffffffb0;
  undefined4 uVar12;
  longlong lStack_30;
  longlong *plStack_28;
  
  uVar12 = (undefined4)((ulonglong)in_stack_ffffffffffffffb0 >> 0x20);
  cVar5 = FUN_18057a5b0(2);
  if (cVar5 != '\0') {
    uVar8 = FUN_18057a310();
    FUN_18057a4b0(uVar8,2,&UNK_1807dee58,&UNK_1807dee88,0xa56);
  }
  lVar11 = -1;
  if ((param_2[1] - *param_2 & 0xffffffffffffffe0U) == 0x60) {
    if ((_DAT_1808793b0 & 1) == 0) {
      _DAT_1808793b0 = _DAT_1808793b0 | 1;
      _DAT_180879360 = 1;
      _DAT_180879368 = 0;
      _DAT_180879370 = (longlong *)0x0;
      uRam0000000180879378 = 0;
      _DAT_180879380 = 0;
      uRam0000000180879388 = 0;
      _DAT_180879390 = 0;
      uRam0000000180879398 = 0;
      _DAT_1808793a0 = 0;
      FUN_180745c88(&UNK_18075e4e0);
    }
    plVar10 = _DAT_180879370;
    lVar3 = _DAT_180879368;
    if (_DAT_180879370 != (longlong *)0x0) {
      LOCK();
      *(int *)(_DAT_180879370 + 1) = (int)_DAT_180879370[1] + 1;
      UNLOCK();
    }
    if (lVar3 == 0) {
      if (plVar10 != (longlong *)0x0) {
        LOCK();
        plVar1 = plVar10 + 1;
        *(int *)plVar1 = (int)*plVar1 + -1;
        UNLOCK();
        if ((int)*plVar1 == 0) {
          (**(code **)*plVar10)(plVar10);
          LOCK();
          piVar2 = (int *)((longlong)plVar10 + 0xc);
          *piVar2 = *piVar2 + -1;
          UNLOCK();
          if (*piVar2 == 0) {
            (**(code **)(*plVar10 + 8))(plVar10);
          }
        }
      }
    }
    else {
      iVar7 = *(int *)(lVar3 + 0x40);
      if (plVar10 != (longlong *)0x0) {
        LOCK();
        plVar1 = plVar10 + 1;
        *(int *)plVar1 = (int)*plVar1 + -1;
        UNLOCK();
        if ((int)*plVar1 == 0) {
          (**(code **)*plVar10)(plVar10);
          LOCK();
          piVar2 = (int *)((longlong)plVar10 + 0xc);
          *piVar2 = *piVar2 + -1;
          UNLOCK();
          if (*piVar2 == 0) {
            (**(code **)(*plVar10 + 8))(plVar10);
          }
        }
      }
      if (iVar7 < 3) {
        lVar3 = *param_2;
        plVar10 = (longlong *)(lVar3 + 0x40);
        if (0xf < *(ulonglong *)(lVar3 + 0x58)) {
          plVar10 = (longlong *)*plVar10;
        }
        puVar9 = (undefined8 *)(lVar3 + 0x20);
        if (0xf < *(ulonglong *)(lVar3 + 0x38)) {
          puVar9 = (undefined8 *)*puVar9;
        }
        if ((_DAT_1808793b0 & 1) == 0) {
          _DAT_1808793b0 = _DAT_1808793b0 | 1;
          _DAT_180879360 = 1;
          _DAT_180879368 = 0;
          _DAT_180879370 = (longlong *)0x0;
          uRam0000000180879378 = 0;
          _DAT_180879380 = 0;
          uRam0000000180879388 = 0;
          _DAT_180879390 = 0;
          uRam0000000180879398 = 0;
          _DAT_1808793a0 = 0;
          FUN_180745c88(&UNK_18075e4e0);
        }
        FUN_18057a4b0(&DAT_180879360,2,&UNK_1807dee18,&UNK_1807dee88,0xa5c,puVar9,plVar10);
        uVar12 = (undefined4)((ulonglong)puVar9 >> 0x20);
      }
    }
    puVar9 = (undefined8 *)(*param_2 + 0x20);
    if (0xf < *(ulonglong *)(*param_2 + 0x38)) {
      puVar9 = (undefined8 *)*puVar9;
    }
    uVar6 = atoi(puVar9);
    puVar9 = (undefined8 *)(*param_2 + 0x40);
    if (0xf < *(ulonglong *)(*param_2 + 0x58)) {
      puVar9 = (undefined8 *)*puVar9;
    }
    atoi(puVar9);
    if ((_DAT_18087957c & 1) == 0) {
      _DAT_18087957c = _DAT_18087957c | 1;
      FUN_1805d2e40();
      FUN_180745c88(&UNK_18075ec30);
    }
    FUN_1805dd580(&DAT_180879580,&lStack_30,uVar6);
    if (lStack_30 == 0) {
      cVar5 = FUN_18057a5b0(4);
      if (cVar5 != '\0') {
        uVar8 = FUN_18057a310();
        FUN_18057a4b0(uVar8,4,&UNK_1807dedd8,&UNK_1807dee88,0xa62,CONCAT44(uVar12,uVar6));
      }
      iVar7 = -0x1ffffcea;
    }
    else {
      iVar7 = (**(code **)(*(longlong *)(lStack_30 + 0x218) + 0x10))();
      plVar10 = plStack_28;
      if (iVar7 == 0) {
        if (plStack_28 != (longlong *)0x0) {
          LOCK();
          plVar1 = plStack_28 + 1;
          *(int *)plVar1 = (int)*plVar1 + -1;
          UNLOCK();
          if ((int)*plVar1 == 0) {
            (**(code **)*plStack_28)(plStack_28);
            LOCK();
            piVar2 = (int *)((longlong)plVar10 + 0xc);
            *piVar2 = *piVar2 + -1;
            UNLOCK();
            if (*piVar2 == 0) {
              (**(code **)(*plStack_28 + 8))();
            }
          }
        }
        _snprintf(param_3,param_4 + -1,&UNK_1807d7774);
        goto LAB_180619a50;
      }
      cVar5 = FUN_18057a5b0(4);
      if (cVar5 != '\0') {
        uVar8 = FUN_18057a310();
        FUN_18057a4b0(uVar8,4,&UNK_1807def48,&UNK_1807dee88,0xa6a);
      }
    }
    plVar10 = plStack_28;
    if (plStack_28 != (longlong *)0x0) {
      LOCK();
      plVar1 = plStack_28 + 1;
      *(int *)plVar1 = (int)*plVar1 + -1;
      UNLOCK();
      if ((int)*plVar1 == 0) {
        (**(code **)*plStack_28)(plStack_28);
        LOCK();
        piVar2 = (int *)((longlong)plVar10 + 0xc);
        *piVar2 = *piVar2 + -1;
        UNLOCK();
        if (*piVar2 == 0) {
          (**(code **)(*plStack_28 + 8))();
        }
      }
    }
  }
  else {
    iVar7 = -0x1ffffef7;
    cVar5 = FUN_18057a5b0(4);
    if (cVar5 != '\0') {
      lVar3 = param_2[1];
      lVar4 = *param_2;
      uVar8 = FUN_18057a310();
      FUN_18057a4b0(uVar8,4,&UNK_1807deda0,&UNK_1807dee88,0xa5b,CONCAT44(uVar12,3),
                    lVar3 - lVar4 >> 5);
    }
  }
  _snprintf(param_3,param_4 + -1,&UNK_1807d7c04,iVar7);
LAB_180619a50:
  do {
    lVar11 = lVar11 + 1;
  } while (*(char *)(lVar11 + param_3) != '\0');
  *param_5 = lVar11;
  cVar5 = FUN_18057a5b0(2);
  if (cVar5 != '\0') {
    uVar8 = FUN_18057a310();
    FUN_18057a4b0(uVar8,2,&UNK_1807def20,&UNK_1807dee88,0xa70);
  }
  return iVar7;
}

