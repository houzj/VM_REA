
void FUN_18001dfd0(longlong param_1,longlong *param_2)

{
  int *piVar1;
  longlong *plVar2;
  undefined4 uVar3;
  longlong *plVar4;
  char cVar5;
  int iVar6;
  undefined8 uVar7;
  undefined8 uStack_30;
  undefined8 uStack_28;
  
  iVar6 = _Mtx_lock(param_1 + 0x38);
  if (iVar6 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar6);
  }
  if (*param_2 != 0) {
    uStack_30 = 0;
    uStack_28 = 0;
    if (param_2[1] != 0) {
      LOCK();
      piVar1 = (int *)(param_2[1] + 8);
      *piVar1 = *piVar1 + 1;
      UNLOCK();
    }
    FUN_180015c40(&uStack_30);
    cVar5 = FUN_18001e340(param_1,&uStack_30);
    if (cVar5 == '\0') {
      cVar5 = FUN_18001dbc0(1);
      if (cVar5 != '\0') {
        uVar3 = *(undefined4 *)(*param_2 + 0x2c);
        uVar7 = FUN_18001db50();
        FUN_180015b40(uVar7,1,&UNK_18021d110,&UNK_18021d0f0,0x34,*(undefined4 *)(param_1 + 0x28),
                      *(undefined4 *)(param_1 + 0x2c),uVar3);
      }
      FUN_18001ee20(param_1 + 0x48,param_2);
      *(int *)(*param_2 + 0x20) = *(int *)(*param_2 + 0x20) + 1;
      *(int *)(*param_2 + 0x24) = *(int *)(*param_2 + 0x24) + 1;
    }
  }
  iVar6 = _Mtx_unlock(param_1 + 0x38);
  if (iVar6 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar6);
  }
  plVar4 = (longlong *)param_2[1];
  if (plVar4 != (longlong *)0x0) {
    LOCK();
    plVar2 = plVar4 + 1;
    *(int *)plVar2 = (int)*plVar2 + -1;
    UNLOCK();
    if ((int)*plVar2 == 0) {
      (**(code **)*plVar4)(plVar4);
      LOCK();
      piVar1 = (int *)((longlong)plVar4 + 0xc);
      *piVar1 = *piVar1 + -1;
      UNLOCK();
      if (*piVar1 == 0) {
        (**(code **)(*plVar4 + 8))(plVar4);
      }
    }
  }
  return;
}

