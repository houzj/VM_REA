
void FUN_18006e570(undefined8 param_1,longlong *param_2)

{
  longlong *plVar1;
  int *piVar2;
  char cVar3;
  longlong *plVar4;
  longlong *plVar5;
  
  cVar3 = *(char *)((longlong)param_2 + 0x19);
  while (cVar3 == '\0') {
    FUN_18006e570(param_1,param_2[2]);
    plVar4 = (longlong *)param_2[9];
    plVar5 = (longlong *)*param_2;
    if (plVar4 != (longlong *)0x0) {
      LOCK();
      plVar1 = plVar4 + 1;
      *(int *)plVar1 = (int)*plVar1 + -1;
      UNLOCK();
      if ((int)*plVar1 == 0) {
        (**(code **)*plVar4)(plVar4);
        LOCK();
        piVar2 = (int *)((longlong)plVar4 + 0xc);
        *piVar2 = *piVar2 + -1;
        UNLOCK();
        if (*piVar2 == 0) {
          (**(code **)(*plVar4 + 8))(plVar4);
        }
      }
    }
    if (0xf < (ulonglong)param_2[7]) {
      __3_YAXPEAX_Z(param_2[4]);
    }
    param_2[7] = 0xf;
    param_2[6] = 0;
    *(undefined1 *)(param_2 + 4) = 0;
    __3_YAXPEAX_Z(param_2);
    param_2 = plVar5;
    cVar3 = *(char *)((longlong)plVar5 + 0x19);
  }
  return;
}

