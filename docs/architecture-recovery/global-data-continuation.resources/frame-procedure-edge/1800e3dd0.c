
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined8 FUN_1800e3dd0(longlong param_1,undefined4 param_2,int param_3)

{
  longlong *plVar1;
  int *piVar2;
  char cVar3;
  undefined8 *puVar4;
  undefined8 *puVar5;
  longlong *plVar6;
  int iVar7;
  longlong lVar8;
  undefined8 **ppuVar9;
  undefined8 *puVar10;
  undefined8 uVar11;
  undefined8 *puVar12;
  undefined8 *puStackX_8;
  undefined4 auStackX_10 [2];
  int aiStackX_18 [2];
  undefined8 *puStackX_20;
  longlong lStack_48;
  longlong *plStack_40;
  longlong lStack_38;
  longlong *plStack_30;
  
  auStackX_10[0] = param_2;
  aiStackX_18[0] = param_3;
  iVar7 = _Mtx_lock();
  if (iVar7 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar7);
  }
  if ((_DAT_1802b1888 & 1) == 0) {
    _DAT_1802b1888 = _DAT_1802b1888 | 1;
    FUN_180107fc0();
    FUN_1801e8930(&UNK_180202980);
  }
  FUN_180108ba0(&DAT_1802b1890,&lStack_38,auStackX_10[0]);
  if (lStack_38 == 0) {
    uVar11 = 0xe0000316;
  }
  else {
    if ((_DAT_1802b1888 & 1) == 0) {
      _DAT_1802b1888 = _DAT_1802b1888 | 1;
      FUN_180107fc0();
      FUN_1801e8930(&UNK_180202980);
    }
    FUN_180108ba0(&DAT_1802b1890,&lStack_48,aiStackX_18[0]);
    if (lStack_48 == 0) {
      uVar11 = 0xe0000316;
    }
    else {
      uVar11 = FUN_18009e260(param_1 + 8,auStackX_10);
      lVar8 = func_0x00018007c900(uVar11,aiStackX_18);
      if (lVar8 != 0) {
        uVar11 = FUN_18009e260(param_1 + 8,auStackX_10);
        FUN_18006ddb0(uVar11,aiStackX_18);
        puVar4 = *(undefined8 **)(param_1 + 0x18);
        cVar3 = *(char *)((longlong)puVar4[1] + 0x19);
        puVar5 = puVar4;
        puVar12 = (undefined8 *)puVar4[1];
        while (cVar3 == '\0') {
          if (*(int *)(puVar12 + 4) < aiStackX_18[0]) {
            puVar10 = (undefined8 *)puVar12[2];
            puVar12 = puVar5;
          }
          else {
            puVar10 = (undefined8 *)*puVar12;
          }
          puVar5 = puVar12;
          puVar12 = puVar10;
          cVar3 = *(char *)((longlong)puVar10 + 0x19);
        }
        if ((puVar5 == puVar4) || (aiStackX_18[0] < *(int *)(puVar5 + 4))) {
          puStackX_20 = puVar4;
          ppuVar9 = &puStackX_20;
        }
        else {
          puStackX_8 = puVar5;
          ppuVar9 = &puStackX_8;
        }
        if (*ppuVar9 != puVar4) {
          uVar11 = FUN_1800e4570(param_1 + 0x18,aiStackX_18);
          FUN_18006ddb0(uVar11,auStackX_10);
          lVar8 = FUN_1800e4570(param_1 + 0x18,aiStackX_18);
          FUN_18006ddb0(lVar8 + 0x10,auStackX_10);
        }
      }
      uVar11 = 0;
    }
    plVar6 = plStack_40;
    if (plStack_40 != (longlong *)0x0) {
      LOCK();
      plVar1 = plStack_40 + 1;
      *(int *)plVar1 = (int)*plVar1 + -1;
      UNLOCK();
      if ((int)*plVar1 == 0) {
        (**(code **)*plStack_40)(plStack_40);
        LOCK();
        piVar2 = (int *)((longlong)plVar6 + 0xc);
        *piVar2 = *piVar2 + -1;
        UNLOCK();
        if (*piVar2 == 0) {
          (**(code **)(*plStack_40 + 8))();
        }
      }
    }
  }
  plVar6 = plStack_30;
  if (plStack_30 != (longlong *)0x0) {
    LOCK();
    plVar1 = plStack_30 + 1;
    *(int *)plVar1 = (int)*plVar1 + -1;
    UNLOCK();
    if ((int)*plVar1 == 0) {
      (**(code **)*plStack_30)(plStack_30);
      LOCK();
      piVar2 = (int *)((longlong)plVar6 + 0xc);
      *piVar2 = *piVar2 + -1;
      UNLOCK();
      if (*piVar2 == 0) {
        (**(code **)(*plStack_30 + 8))();
      }
    }
  }
  iVar7 = _Mtx_unlock(param_1 + 0x28);
  if (iVar7 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar7);
  }
  return uVar11;
}

