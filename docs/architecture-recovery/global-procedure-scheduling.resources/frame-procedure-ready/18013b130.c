
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void VM_M_DisConnectModule(undefined4 param_1,undefined4 param_2)

{
  longlong *plVar1;
  int *piVar2;
  longlong *plVar3;
  char cVar4;
  undefined4 uVar5;
  undefined8 uVar6;
  undefined8 *puVar7;
  undefined1 auStack_c8 [32];
  undefined4 uStack_a8;
  undefined8 *puStack_a0;
  undefined4 uStack_98;
  longlong lStack_88;
  longlong *plStack_80;
  longlong lStack_78;
  longlong *plStack_70;
  longlong lStack_68;
  longlong *plStack_60;
  longlong lStack_58;
  longlong *plStack_50;
  undefined8 auStack_48 [3];
  ulonglong uStack_30;
  ulonglong uStack_28;
  
                    /* 0x13b130  71  VM_M_DisConnectModule */
  uStack_28 = _DAT_1802a5230 ^ (ulonglong)auStack_c8;
  cVar4 = FUN_18001dbc0(2);
  if (cVar4 != '\0') {
    uVar6 = FUN_18001db50();
    puStack_a0 = (undefined8 *)CONCAT44(puStack_a0._4_4_,param_1);
    uStack_a8 = 0x2e9;
    uStack_98 = param_2;
    FUN_180015b40(uVar6,2,&UNK_180236d68,&UNK_180236d50);
  }
  uVar5 = 0;
  if ((_DAT_1802b1888 & 1) == 0) {
    _DAT_1802b1888 = _DAT_1802b1888 | 1;
    FUN_180107fc0();
    FUN_1801e8930(&UNK_180202980);
  }
  FUN_18010a720(&DAT_1802b1890,&lStack_68,param_1);
  if ((_DAT_1802b1888 & 1) == 0) {
    _DAT_1802b1888 = _DAT_1802b1888 | 1;
    FUN_180107fc0();
    FUN_1801e8930(&UNK_180202980);
  }
  FUN_18010a720(&DAT_1802b1890,&lStack_88,param_2);
  if ((lStack_68 == 0) || (lStack_88 == 0)) {
    if ((_DAT_1802b1888 & 1) == 0) {
      _DAT_1802b1888 = _DAT_1802b1888 | 1;
      FUN_180107fc0();
      FUN_1801e8930(&UNK_180202980);
    }
    FUN_180108ba0(&DAT_1802b1890,&lStack_58,param_1);
    if ((_DAT_1802b1888 & 1) == 0) {
      _DAT_1802b1888 = _DAT_1802b1888 | 1;
      FUN_180107fc0();
      FUN_1801e8930(&UNK_180202980);
    }
    FUN_180108ba0(&DAT_1802b1890,&lStack_78,param_2);
    if ((lStack_58 != 0) && (lStack_78 != 0)) {
      if ((_DAT_1802b3120 & 1) == 0) {
        _DAT_1802b3120 = _DAT_1802b3120 | 1;
        FUN_1800e3630();
        FUN_1801e8930(&UNK_180202f30);
      }
      uVar5 = FUN_1800e3dd0(&DAT_1802b3128,param_1,param_2);
    }
    plVar3 = plStack_70;
    if (plStack_70 != (longlong *)0x0) {
      LOCK();
      plVar1 = plStack_70 + 1;
      *(int *)plVar1 = (int)*plVar1 + -1;
      UNLOCK();
      if ((int)*plVar1 == 0) {
        (**(code **)*plStack_70)(plStack_70);
        LOCK();
        piVar2 = (int *)((longlong)plVar3 + 0xc);
        *piVar2 = *piVar2 + -1;
        UNLOCK();
        if (*piVar2 == 0) {
          (**(code **)(*plStack_70 + 8))();
        }
      }
    }
    plVar3 = plStack_50;
    if (plStack_50 != (longlong *)0x0) {
      LOCK();
      plVar1 = plStack_50 + 1;
      *(int *)plVar1 = (int)*plVar1 + -1;
      UNLOCK();
      if ((int)*plVar1 == 0) {
        (**(code **)*plStack_50)(plStack_50);
        LOCK();
        piVar2 = (int *)((longlong)plVar3 + 0xc);
        *piVar2 = *piVar2 + -1;
        UNLOCK();
        if (*piVar2 == 0) {
          (**(code **)(*plStack_50 + 8))();
        }
      }
    }
  }
  else {
    if ((_DAT_1802b12c0 & 1) == 0) {
      _DAT_1802b12c0 = _DAT_1802b12c0 | 1;
      FUN_18003fde0();
      FUN_1801e8930(&UNK_180202970);
    }
    if ((_DAT_1802b1888 & 1) == 0) {
      _DAT_1802b1888 = _DAT_1802b1888 | 1;
      FUN_180107fc0();
      FUN_1801e8930(&UNK_180202980);
    }
    uVar5 = FUN_18010ad50(&DAT_1802b1890,param_1,param_2);
  }
  cVar4 = FUN_18001dbc0(2);
  if (cVar4 != '\0') {
    puVar7 = (undefined8 *)FUN_18008f750(auStack_48,uVar5);
    if (0xf < (ulonglong)puVar7[3]) {
      puVar7 = (undefined8 *)*puVar7;
    }
    uVar6 = FUN_18001db50();
    uStack_a8 = 0x2fd;
    puStack_a0 = puVar7;
    FUN_180015b40(uVar6,2,&UNK_180234258,&UNK_180236d50);
    if (0xf < uStack_30) {
      __3_YAXPEAX_Z(auStack_48[0]);
    }
  }
  plVar3 = plStack_80;
  if (plStack_80 != (longlong *)0x0) {
    LOCK();
    plVar1 = plStack_80 + 1;
    *(int *)plVar1 = (int)*plVar1 + -1;
    UNLOCK();
    if ((int)*plVar1 == 0) {
      (**(code **)*plStack_80)(plStack_80);
      LOCK();
      piVar2 = (int *)((longlong)plVar3 + 0xc);
      *piVar2 = *piVar2 + -1;
      UNLOCK();
      if (*piVar2 == 0) {
        (**(code **)(*plStack_80 + 8))();
      }
    }
  }
  plVar3 = plStack_60;
  if (plStack_60 != (longlong *)0x0) {
    LOCK();
    plVar1 = plStack_60 + 1;
    *(int *)plVar1 = (int)*plVar1 + -1;
    UNLOCK();
    if ((int)*plVar1 == 0) {
      (**(code **)*plStack_60)(plStack_60);
      LOCK();
      piVar2 = (int *)((longlong)plVar3 + 0xc);
      *piVar2 = *piVar2 + -1;
      UNLOCK();
      if (*piVar2 == 0) {
        (**(code **)(*plStack_60 + 8))();
      }
    }
  }
  FUN_1801e8810(uStack_28 ^ (ulonglong)auStack_c8);
  return;
}

