
int FUN_1800ca500(longlong param_1,undefined4 param_2,undefined8 param_3,undefined4 param_4,
                 undefined8 param_5,undefined4 *param_6)

{
  longlong *plVar1;
  int *piVar2;
  undefined4 *puVar3;
  longlong *plVar4;
  int iVar5;
  undefined4 auStack_48 [2];
  longlong alStack_40 [3];
  longlong *plStack_28;
  
  puVar3 = param_6;
  alStack_40[1] = 0xfffffffffffffffe;
  if (param_6 == (undefined4 *)0x0) {
    iVar5 = -0x1fffffff;
  }
  else {
    auStack_48[0] = param_2;
    FUN_18009ab20(param_1 + 8,alStack_40,auStack_48);
    if (alStack_40[0] == *(longlong *)(param_1 + 8)) {
      iVar5 = -0x1ffffcec;
    }
    else {
      FUN_1800cae00(param_1,alStack_40 + 2,param_2);
      if (alStack_40[2] == 0) {
        iVar5 = -0x1ffffffd;
      }
      else {
        param_6._0_4_ = 0;
        iVar5 = FUN_1800f5f40(alStack_40[2],param_3,param_4,param_5,&param_6);
        if (iVar5 == 0) {
          *puVar3 = param_6._0_4_;
        }
      }
      plVar4 = plStack_28;
      if (plStack_28 != (longlong *)0x0) {
        LOCK();
        plVar1 = plStack_28 + 1;
        *(int *)plVar1 = (int)*plVar1 + -1;
        UNLOCK();
        if ((int)*plVar1 == 0) {
          (**(code **)*plStack_28)(plStack_28);
          LOCK();
          piVar2 = (int *)((longlong)plVar4 + 0xc);
          *piVar2 = *piVar2 + -1;
          UNLOCK();
          if (*piVar2 == 0) {
            (**(code **)(*plStack_28 + 8))();
          }
        }
      }
    }
  }
  return iVar5;
}

