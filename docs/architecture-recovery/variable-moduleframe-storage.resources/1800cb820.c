
void FUN_1800cb820(longlong param_1,undefined4 param_2,undefined4 param_3,undefined8 param_4)

{
  longlong *plVar1;
  int *piVar2;
  longlong *plVar3;
  int iVar4;
  longlong lStack_30;
  longlong *plStack_28;
  
  iVar4 = _Mtx_lock(param_1 + 0x58);
  if (iVar4 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar4);
  }
  FUN_1800cae00(param_1,&lStack_30,param_2);
  if (lStack_30 != 0) {
    FUN_1800f82f0(lStack_30,param_3,param_4);
  }
  plVar3 = plStack_28;
  if (plStack_28 != (longlong *)0x0) {
    LOCK();
    plVar1 = plStack_28 + 1;
    *(int *)plVar1 = (int)*plVar1 + -1;
    UNLOCK();
    if ((int)*plVar1 == 0) {
      (**(code **)*plStack_28)(plStack_28);
      LOCK();
      piVar2 = (int *)((longlong)plVar3 + 0xc);
      *piVar2 = *piVar2 + -1;
      UNLOCK();
      if (*piVar2 == 0) {
        (**(code **)(*plStack_28 + 8))();
      }
    }
  }
  iVar4 = _Mtx_unlock(param_1 + 0x58);
  if (iVar4 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar4);
  }
  return;
}

