
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_1800ffaa0(undefined8 param_1,longlong *param_2,undefined4 param_3)

{
  int *piVar1;
  undefined4 uVar2;
  longlong lVar3;
  longlong *plVar4;
  char cVar5;
  undefined8 uVar6;
  longlong *plVar7;
  longlong *plStack_f0;
  undefined8 uStack_e8;
  undefined1 auStack_e0 [24];
  undefined8 uStack_c8;
  undefined8 uStack_c0;
  undefined1 auStack_b8 [32];
  undefined1 auStack_98 [32];
  undefined1 auStack_78 [24];
  longlong *plStack_60;
  undefined1 auStack_50 [24];
  longlong *plStack_38;
  
  uStack_c0 = 0xfffffffffffffffe;
  cVar5 = FUN_1800e1ec0(*(undefined4 *)(*param_2 + 0x28));
  if (cVar5 == '\0') {
    if (*(int *)(*param_2 + 0x30) == 5) {
      uVar6 = FUN_180101240(auStack_78);
      plVar7 = (longlong *)FUN_1801012a0(auStack_98,uVar6);
      plStack_f0 = plVar7;
      if ((_DAT_1802b2980 & 1) == 0) {
        _DAT_1802b2980 = _DAT_1802b2980 | 1;
        FUN_18015a9d0();
        FUN_1801e8930(FUN_180202990);
      }
      uStack_c8 = 0;
      FUN_1800f3be0(auStack_e0,plVar7);
      FUN_1800fb9d0(0x1802b2998,auStack_e0);
      plVar4 = (longlong *)plVar7[3];
      if (plVar4 != (longlong *)0x0) {
        (**(code **)(*plVar4 + 0x20))(plVar4,plVar4 != plVar7);
        plVar7[3] = 0;
      }
      plVar4 = (longlong *)plVar7[3];
      plStack_38 = plStack_60;
      if (plVar4 != (longlong *)0x0) {
        (**(code **)(*plVar4 + 0x20))(plVar4,plVar4 != plVar7);
        plVar7[3] = 0;
        plStack_38 = plStack_60;
      }
    }
    else {
      uVar6 = FUN_180101240(auStack_50);
      uVar6 = FUN_1801012a0(auStack_b8,uVar6);
      uVar2 = *(undefined4 *)(*param_2 + 0x28);
      if ((_DAT_1802b2980 & 1) == 0) {
        _DAT_1802b2980 = _DAT_1802b2980 | 1;
        FUN_18015a9d0();
        FUN_1801e8930(FUN_180202990);
      }
      FUN_18015aba0(&DAT_1802b2990,uVar2,uVar6);
    }
    if (plStack_38 != (longlong *)0x0) {
      LOCK();
      plVar7 = plStack_38 + 1;
      *(int *)plVar7 = (int)*plVar7 + -1;
      UNLOCK();
      if ((int)*plVar7 == 0) {
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
  }
  else {
    plStack_f0 = (longlong *)0x0;
    uStack_e8 = 0;
    lVar3 = *param_2;
    if (param_2[1] != 0) {
      LOCK();
      piVar1 = (int *)(param_2[1] + 8);
      *piVar1 = *piVar1 + 1;
      UNLOCK();
    }
    FUN_180015c40(&plStack_f0,lVar3);
    FUN_1800ffd00(param_1,&plStack_f0,param_3);
  }
  plVar7 = (longlong *)param_2[1];
  if (plVar7 != (longlong *)0x0) {
    LOCK();
    plVar4 = plVar7 + 1;
    *(int *)plVar4 = (int)*plVar4 + -1;
    UNLOCK();
    if ((int)*plVar4 == 0) {
      (**(code **)*plVar7)(plVar7);
      LOCK();
      piVar1 = (int *)((longlong)plVar7 + 0xc);
      *piVar1 = *piVar1 + -1;
      UNLOCK();
      if (*piVar1 == 0) {
        (**(code **)(*plVar7 + 8))(plVar7);
      }
    }
  }
  return;
}

