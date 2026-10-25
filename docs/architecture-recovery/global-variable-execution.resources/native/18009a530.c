
void FUN_18009a530(longlong param_1,int param_2)

{
  int *piVar1;
  longlong *plVar2;
  code *pcVar3;
  int iVar4;
  longlong lVar5;
  undefined8 uVar6;
  longlong lVar7;
  longlong *plVar8;
  int aiStackX_10 [2];
  longlong *plStackX_18;
  longlong *plStackX_20;
  int **ppiVar9;
  int *piStack_68;
  longlong lStack_60;
  undefined8 uStack_58;
  longlong *aplStack_50 [2];
  undefined1 auStack_40 [8];
  longlong *plStack_38;
  undefined1 auStack_30 [8];
  longlong *plStack_28;
  
  uStack_58 = 0xfffffffffffffffe;
  if (((param_2 - 24000U < 2000) || (param_2 - 13000U < 1000)) || (param_2 - 31000U < 3000)) {
    lStack_60 = param_1 + 8;
    aiStackX_10[0] = param_2;
    iVar4 = _Mtx_lock();
    if (iVar4 != 0) {
      __Throw_C_error_std__YAXH_Z(iVar4);
    }
    plVar8 = (longlong *)(param_1 + 0x18);
    plVar2 = (longlong *)*plVar8;
    FUN_18009ab20(plVar8,&plStackX_20,aiStackX_10);
    if (plStackX_20 == plVar2) {
      FUN_18009ab20(plVar8,&plStackX_18,aiStackX_10);
      if (plStackX_18 == plVar2) {
        piStack_68 = aiStackX_10;
        lVar7 = *plVar2;
        ppiVar9 = &piStack_68;
        lVar5 = FUN_18009b300(piStack_68,lVar7,*(undefined8 *)(lVar7 + 8));
        if (*(longlong *)(param_1 + 0x20) == 0x3fffffffffffffe) {
          __Xlength_error_std__YAXPEBD_Z(&UNK_18021d318);
          pcVar3 = (code *)swi(3);
          (*pcVar3)();
          return;
        }
        *(longlong *)(param_1 + 0x20) = *(longlong *)(param_1 + 0x20) + 1;
        *(longlong *)(lVar7 + 8) = lVar5;
        **(longlong **)(lVar5 + 8) = lVar5;
        FUN_18009b010(plVar8,aplStack_50,*(longlong *)*plVar8 + 0x10,*(longlong *)*plVar8,ppiVar9);
        plStackX_18 = aplStack_50[0];
      }
      plVar2 = plStackX_18;
      *(undefined2 *)(plStackX_18 + 3) = 0;
      uVar6 = FUN_18009ae80(auStack_40);
      FUN_18001d870(plVar2 + 4,uVar6);
      if (plStack_38 != (longlong *)0x0) {
        LOCK();
        plVar8 = plStack_38 + 1;
        *(int *)plVar8 = (int)*plVar8 + -1;
        UNLOCK();
        if ((int)*plVar8 == 0) {
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
      uVar6 = FUN_18009af30(auStack_30);
      plVar2 = plVar2 + 6;
      FUN_18001d870(plVar2,uVar6);
      if (plStack_28 != (longlong *)0x0) {
        LOCK();
        plVar8 = plStack_28 + 1;
        *(int *)plVar8 = (int)*plVar8 + -1;
        UNLOCK();
        if ((int)*plVar8 == 0) {
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
      iVar4 = aiStackX_10[0];
      lVar7 = FUN_180007490(*plVar2);
      *(uint *)(lVar7 + 0x10) = *(uint *)(lVar7 + 0x10) | 1;
      *(int *)(lVar7 + 0x28) = iVar4;
    }
    iVar4 = _Mtx_unlock(lStack_60);
    if (iVar4 != 0) {
      __Throw_C_error_std__YAXH_Z(iVar4);
    }
  }
  return;
}

