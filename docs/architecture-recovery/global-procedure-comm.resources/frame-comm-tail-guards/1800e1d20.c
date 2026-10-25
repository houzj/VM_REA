
undefined8 FUN_1800e1d20(longlong param_1,undefined8 param_2,undefined8 param_3,int param_4)

{
  longlong *plVar1;
  int *piVar2;
  longlong *plVar3;
  int iVar4;
  undefined8 uVar5;
  undefined8 uVar6;
  longlong lVar7;
  undefined1 uStack_48;
  undefined8 uStack_38;
  longlong *plStack_30;
  undefined8 uStack_28;
  longlong *plStack_20;
  
  lVar7 = (longlong)param_4;
  uVar6 = 0;
  iVar4 = _stricmp(param_2,&UNK_18021f550);
  if (iVar4 == 0) {
    if (0 < param_4) {
      *(int *)(param_1 + 0x220) = param_4;
      uVar5 = func_0x0001801e8774(lVar7);
      uStack_38 = 0;
      plStack_30 = (longlong *)0x0;
      FUN_180071e00(&uStack_38,uVar5,uStack_48);
      FUN_18001d870(param_1 + 0x210,&uStack_38);
      plVar3 = plStack_30;
      if (plStack_30 != (longlong *)0x0) {
        LOCK();
        plVar1 = plStack_30 + 1;
        *(int *)plVar1 = (int)*plVar1 + -1;
        UNLOCK();
        if ((int)*plVar1 == 0) {
          (**(code **)*plStack_30)(plStack_30);
          LOCK();
          piVar2 = (int *)((longlong)plVar3 + 0xc);
          *piVar2 = *piVar2 + -1;
          UNLOCK();
          if (*piVar2 == 0) {
            (**(code **)(*plStack_30 + 8))();
          }
        }
      }
      func_0x0001801e918e(*(undefined8 *)(param_1 + 0x210),param_3,lVar7);
    }
  }
  else {
    iVar4 = _stricmp(param_2,&UNK_18021f560);
    if (iVar4 == 0) {
      if (0 < param_4) {
        *(int *)(param_1 + 0x238) = param_4;
        uVar6 = func_0x0001801e8774(lVar7);
        uStack_28 = 0;
        plStack_20 = (longlong *)0x0;
        FUN_180071e00(&uStack_28,uVar6,uStack_48);
        FUN_18001d870(param_1 + 0x228,&uStack_28);
        plVar3 = plStack_20;
        if (plStack_20 != (longlong *)0x0) {
          LOCK();
          plVar1 = plStack_20 + 1;
          *(int *)plVar1 = (int)*plVar1 + -1;
          UNLOCK();
          if ((int)*plVar1 == 0) {
            (**(code **)*plStack_20)(plStack_20);
            LOCK();
            piVar2 = (int *)((longlong)plVar3 + 0xc);
            *piVar2 = *piVar2 + -1;
            UNLOCK();
            if (*piVar2 == 0) {
              (**(code **)(*plStack_20 + 8))();
            }
          }
        }
        func_0x0001801e918e(*(undefined8 *)(param_1 + 0x228),param_3,lVar7);
        uVar6 = 0;
      }
    }
    else {
      uVar6 = 0xe0000001;
    }
  }
  return uVar6;
}

