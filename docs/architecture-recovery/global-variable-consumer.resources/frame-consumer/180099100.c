
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined4
FUN_180099100(longlong param_1,undefined4 param_2,longlong param_3,undefined4 param_4,
             longlong param_5,undefined4 param_6,char param_7)

{
  longlong *plVar1;
  int *piVar2;
  longlong *plVar3;
  undefined4 uVar4;
  longlong lVar5;
  longlong lVar6;
  undefined4 auStackX_10 [2];
  longlong lStack_28;
  longlong *plStack_20;
  
  auStackX_10[0] = param_2;
  if (param_7 != '\0') {
    lVar5 = FUN_18009a990(param_1 + 0x18,auStackX_10);
    lVar6 = FUN_180007510(*(undefined8 *)(lVar5 + 0x18));
    FUN_18003fc70(lVar6,&UNK_180226b10);
    *(uint *)(lVar6 + 0x10) = *(uint *)(lVar6 + 0x10) | 1;
    lVar5 = *(longlong *)(lVar6 + 0x18);
    if (lVar5 == _DAT_1802b05e8) {
      FUN_180001ac0(lVar6 + 0x18,param_3);
    }
    else if (lVar5 != param_3) {
      FUN_180006470(lVar5,param_3,0,0xffffffffffffffff);
    }
    lVar5 = FUN_180007330(lVar6);
    FUN_180093760(lVar5,param_5);
    *(uint *)(lVar5 + 0x10) = *(uint *)(lVar5 + 0x10) | 1;
    *(undefined4 *)(lVar5 + 0x20) = param_6;
  }
  FUN_1801e5430(0x1802b1208);
  if ((_DAT_1802b2b48 & 1) == 0) {
    _DAT_1802b2b48 = _DAT_1802b2b48 | 1;
    FUN_1800c8f10(0x1802b2b50);
    FUN_1801e8930(&UNK_180202a80);
  }
  if (param_5 == 0) {
    uVar4 = 0xe0000001;
  }
  else {
    FUN_1800cae00(0x1802b2b50,&lStack_28,auStackX_10[0]);
    plVar3 = plStack_20;
    if (lStack_28 == 0) {
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
      uVar4 = 0xe0000003;
    }
    else {
      uVar4 = FUN_1800f5d00(lStack_28,param_3,param_4,param_5,param_6);
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
    }
  }
  FUN_1801e5390(0x1802b1208);
  return uVar4;
}

