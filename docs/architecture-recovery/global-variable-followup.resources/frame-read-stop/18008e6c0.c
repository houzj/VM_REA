
/* WARNING: Type propagation algorithm not settling */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

longlong FUN_18008e6c0(longlong param_1)

{
  int *piVar1;
  char cVar2;
  longlong *plVar3;
  undefined8 uVar4;
  int *piVar5;
  longlong lVar6;
  longlong *plVar7;
  longlong *plVar8;
  undefined4 extraout_XMM0_Da;
  undefined4 uVar9;
  ulonglong auStackX_10 [2];
  ulonglong *puStackX_20;
  longlong *plStack_88;
  undefined8 uStack_80;
  undefined1 auStack_78 [8];
  undefined8 uStack_70;
  longlong *plStack_68;
  longlong lStack_60;
  undefined8 uStack_58;
  undefined8 uStack_50;
  longlong alStack_48 [2];
  longlong *plStack_38;
  
  uStack_80 = 0xfffffffffffffffe;
  uVar4 = 0;
  if ((_DAT_1802b2b30 & 1) == 0) {
    _DAT_1802b2b30 = _DAT_1802b2b30 | 1;
    auStackX_10[0] = __2_YAPEAX_K_Z(0xb0);
    if (auStackX_10[0] != 0) {
      uVar4 = FUN_1800c8f10(auStackX_10[0]);
    }
    auStackX_10[0] = auStackX_10[0] & 0xffffffff00000000;
    uStack_70 = 0;
    plStack_68 = (longlong *)0x0;
    FUN_180091db0(&uStack_70,uVar4);
    piVar5 = (int *)func_0x000180091940(alStack_48,auStackX_10,&uStack_70);
    lStack_60 = (longlong)*piVar5;
    piVar1 = piVar5 + 2;
    uStack_58 = 0;
    uStack_50 = 0;
    if ((int *)&uStack_58 != piVar1) {
      uStack_50 = *(undefined8 *)(piVar5 + 4);
      piVar5[4] = 0;
      piVar5[5] = 0;
      uStack_58 = *(undefined8 *)piVar1;
      piVar1[0] = 0;
      piVar1[1] = 0;
    }
    if (plStack_38 != (longlong *)0x0) {
      LOCK();
      plVar8 = plStack_38 + 1;
      *(int *)plVar8 = (int)*plVar8 + -1;
      UNLOCK();
      if ((int)*plVar8 == 0) {
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
    plVar8 = plStack_68;
    if (plStack_68 != (longlong *)0x0) {
      LOCK();
      plVar7 = plStack_68 + 1;
      *(int *)plVar7 = (int)*plVar7 + -1;
      UNLOCK();
      if ((int)*plVar7 == 0) {
        (**(code **)*plStack_68)(plStack_68);
        LOCK();
        piVar1 = (int *)((longlong)plVar8 + 0xc);
        *piVar1 = *piVar1 + -1;
        UNLOCK();
        if (*piVar1 == 0) {
          (**(code **)(*plStack_68 + 8))();
        }
      }
    }
    _DAT_1802b2b38 = (longlong *)0x0;
    _DAT_1802b2b40 = 0;
    _DAT_1802b2b38 = (longlong *)FUN_18003c7f0();
    plVar8 = &lStack_60;
    uVar9 = extraout_XMM0_Da;
    do {
      plVar7 = _DAT_1802b2b38;
      lVar6 = FUN_1800924e0(uVar9,plVar8);
      uVar9 = FUN_180091e60(&DAT_1802b2b38,auStack_78,plVar7,lVar6 + 0x20,lVar6);
      plVar8 = plVar8 + 3;
    } while (plVar8 != alStack_48);
    FUN_1801e8bb4(&lStack_60,0x18,1,FUN_18007cee0);
    FUN_1801e8930(FUN_180202a00);
  }
  if ((DAT_1802b129e == '\0') || (param_1 == 0)) {
    auStackX_10[1] = 0;
    cVar2 = *(char *)(_DAT_1802b2b38[1] + 0x19);
    plVar8 = (longlong *)_DAT_1802b2b38[1];
    plVar7 = _DAT_1802b2b38;
    while (plVar3 = plVar8, cVar2 == '\0') {
      plVar8 = (longlong *)*plVar3;
      cVar2 = *(char *)((longlong)plVar8 + 0x19);
      plVar7 = plVar3;
    }
    if ((plVar7 == _DAT_1802b2b38) || (plVar7[4] != 0)) {
      puStackX_20 = auStackX_10 + 1;
      lVar6 = FUN_180091e20();
      FUN_180091e60(&DAT_1802b2b38,&plStack_88,plVar7,lVar6 + 0x20,lVar6);
      plVar7 = plStack_88;
    }
    lVar6 = plVar7[5];
  }
  else {
    if ((_DAT_1802b3158 & 1) == 0) {
      _DAT_1802b3158 = _DAT_1802b3158 | 1;
      FUN_1800e8550();
      FUN_1801e8930(&UNK_180202f40);
    }
    lVar6 = FUN_1800ea800(&DAT_1802b3160,param_1);
  }
  return lVar6;
}

