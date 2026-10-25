
void FUN_1800a77a0(longlong param_1)

{
  longlong *plVar1;
  int *piVar2;
  longlong *plVar3;
  
  FUN_1800a03a0(param_1 + 0xe0);
  plVar3 = *(longlong **)(param_1 + 0xd0);
  if (plVar3 != (longlong *)0x0) {
    LOCK();
    plVar1 = plVar3 + 1;
    *(int *)plVar1 = (int)*plVar1 + -1;
    UNLOCK();
    if ((int)*plVar1 == 0) {
      (**(code **)*plVar3)(plVar3);
      LOCK();
      piVar2 = (int *)((longlong)plVar3 + 0xc);
      *piVar2 = *piVar2 + -1;
      UNLOCK();
      if (*piVar2 == 0) {
        (**(code **)(*plVar3 + 8))(plVar3);
      }
    }
  }
  FUN_1800a03a0(param_1 + 0x38);
  if (0xf < *(ulonglong *)(param_1 + 0x30)) {
    __3_YAXPEAX_Z(*(undefined8 *)(param_1 + 0x18));
  }
  *(undefined8 *)(param_1 + 0x30) = 0xf;
  *(undefined8 *)(param_1 + 0x28) = 0;
  *(undefined1 *)(param_1 + 0x18) = 0;
  return;
}

