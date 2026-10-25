
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined4
FUN_180098630(undefined8 param_1,undefined4 param_2,undefined8 *param_3,int param_4,
             undefined4 param_5)

{
  longlong *plVar1;
  int *piVar2;
  longlong *plVar3;
  char cVar4;
  int iVar5;
  undefined4 uVar6;
  undefined8 uVar7;
  undefined8 *puVar8;
  longlong lStack_30;
  longlong *plStack_28;
  
  if (((param_4 == 0) && (iVar5 = FUN_18009a7e0(), iVar5 != 0)) &&
     (cVar4 = FUN_18001dbc0(4), cVar4 != '\0')) {
    puVar8 = param_3;
    if (0xf < (ulonglong)param_3[3]) {
      puVar8 = (undefined8 *)*param_3;
    }
    uVar7 = FUN_18001db50();
    FUN_180015b40(uVar7,4,&UNK_180226930,&UNK_180226878,0x46f,param_2,puVar8,iVar5);
  }
  FUN_1801e5430(0x1802b1208);
  if ((_DAT_1802b2b48 & 1) == 0) {
    _DAT_1802b2b48 = _DAT_1802b2b48 | 1;
    FUN_1800c8f10(0x1802b2b50);
    FUN_1801e8930(&UNK_180202a80);
  }
  FUN_1800cae00(0x1802b2b50,&lStack_30,param_2);
  plVar3 = plStack_28;
  if (lStack_30 == 0) {
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
    uVar6 = 0xe0000003;
  }
  else {
    uVar6 = FUN_1800f47f0(lStack_30,param_3,param_4,param_5);
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
  }
  FUN_1801e5390(0x1802b1208);
  return uVar6;
}

