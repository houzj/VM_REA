
void FUN_1806a3180(undefined8 *param_1)

{
  int *piVar1;
  longlong *plVar2;
  longlong *plVar3;
  undefined8 *puStackX_8;
  undefined1 auStackX_10 [8];
  undefined8 uVar4;
  
  uVar4 = 0xfffffffffffffffe;
  *param_1 = &UNK_1807ebac0;
  param_1[0x43] = &UNK_1807ebc48;
  plVar2 = (longlong *)param_1[0x5e];
  puStackX_8 = param_1;
  if (plVar2 != (longlong *)0x0) {
    LOCK();
    plVar3 = plVar2 + 1;
    *(int *)plVar3 = (int)*plVar3 + -1;
    UNLOCK();
    if ((int)*plVar3 == 0) {
      (**(code **)*plVar2)(plVar2);
      LOCK();
      piVar1 = (int *)((longlong)plVar2 + 0xc);
      *piVar1 = *piVar1 + -1;
      UNLOCK();
      if (*piVar1 == 0) {
        (**(code **)(*plVar2 + 8))(plVar2);
      }
    }
  }
  plVar2 = (longlong *)param_1[0x5c];
  if (plVar2 != (longlong *)0x0) {
    LOCK();
    plVar3 = plVar2 + 1;
    *(int *)plVar3 = (int)*plVar3 + -1;
    UNLOCK();
    if ((int)*plVar3 == 0) {
      (**(code **)*plVar2)(plVar2);
      LOCK();
      piVar1 = (int *)((longlong)plVar2 + 0xc);
      *piVar1 = *piVar1 + -1;
      UNLOCK();
      if (*piVar1 == 0) {
        (**(code **)(*plVar2 + 8))(plVar2);
      }
    }
  }
  DeleteCriticalSection(param_1 + 0x56);
  FUN_1806a52f0(param_1 + 0x52,auStackX_10,*(undefined8 *)param_1[0x52],(undefined8 *)param_1[0x52],
                uVar4);
  __3_YAXPEAX_Z(param_1[0x52]);
  DeleteCriticalSection(param_1 + 0x4d);
  DeleteCriticalSection(param_1 + 0x47);
  plVar2 = (longlong *)param_1[0x45];
  plVar3 = (longlong *)*plVar2;
  *plVar2 = (longlong)plVar2;
  *(undefined8 *)(param_1[0x45] + 8) = param_1[0x45];
  param_1[0x46] = 0;
  if (plVar3 != (longlong *)param_1[0x45]) {
    do {
      plVar3 = (longlong *)*plVar3;
      __3_YAXPEAX_Z();
    } while (plVar3 != (longlong *)param_1[0x45]);
  }
  __3_YAXPEAX_Z(param_1[0x45]);
  FUN_1806a62f0(param_1 + 0x43);
  *param_1 = &UNK_1807e56a8;
  puStackX_8 = param_1;
  FUN_18059f090(param_1 + 0x40);
  __3_YAXPEAX_Z(param_1[0x40]);
  DeleteCriticalSection(param_1 + 0x3b);
  if (0xf < (ulonglong)param_1[0x39]) {
    __3_YAXPEAX_Z(param_1[0x36]);
  }
  param_1[0x39] = 0xf;
  param_1[0x38] = 0;
  *(undefined1 *)(param_1 + 0x36) = 0;
  DeleteCriticalSection(param_1 + 0x2f);
  FUN_18059fa80(param_1 + 0x2d);
  __3_YAXPEAX_Z(param_1[0x2d]);
  DeleteCriticalSection(param_1 + 0x28);
  plVar2 = (longlong *)param_1[0x26];
  plVar3 = (longlong *)*plVar2;
  *plVar2 = (longlong)plVar2;
  *(undefined8 *)(param_1[0x26] + 8) = param_1[0x26];
  param_1[0x27] = 0;
  if (plVar3 != (longlong *)param_1[0x26]) {
    do {
      plVar3 = (longlong *)*plVar3;
      __3_YAXPEAX_Z();
    } while (plVar3 != (longlong *)param_1[0x26]);
  }
  __3_YAXPEAX_Z(param_1[0x26]);
  DeleteCriticalSection(param_1 + 0x21);
  plVar2 = (longlong *)param_1[0x1f];
  plVar3 = (longlong *)*plVar2;
  *plVar2 = (longlong)plVar2;
  *(undefined8 *)(param_1[0x1f] + 8) = param_1[0x1f];
  param_1[0x20] = 0;
  if (plVar3 != (longlong *)param_1[0x1f]) {
    do {
      plVar3 = (longlong *)*plVar3;
      __3_YAXPEAX_Z();
    } while (plVar3 != (longlong *)param_1[0x1f]);
  }
  __3_YAXPEAX_Z(param_1[0x1f]);
  FUN_180745f44(param_1 + 0xb,0x20,5,FUN_180577d70);
  *param_1 = &UNK_1807edd80;
  *(undefined1 *)(param_1 + 9) = 1;
  FUN_1806b6c10(param_1);
  DeleteCriticalSection(param_1 + 4);
  FUN_1806b8b20(param_1 + 2,&puStackX_8,*(undefined8 *)param_1[2],(undefined8 *)param_1[2]);
                    /* WARNING: Could not recover jumptable at 0x000180652c14. Too many branches */
                    /* WARNING: Treating indirect jump as call */
  __3_YAXPEAX_Z(param_1[2]);
  return;
}

