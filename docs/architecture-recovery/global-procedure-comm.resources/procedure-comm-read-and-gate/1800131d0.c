
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_1800131d0(longlong param_1,longlong *param_2)

{
  longlong *plVar1;
  int *piVar2;
  longlong lVar3;
  longlong *plVar4;
  undefined4 *puVar5;
  char *pcVar6;
  char cVar7;
  int iVar8;
  int iVar9;
  undefined1 *puVar10;
  undefined8 *puVar11;
  longlong lVar12;
  undefined *puVar13;
  undefined1 auStack_178 [32];
  undefined8 *puStack_158;
  undefined4 uStack_150;
  undefined1 *puStack_148;
  undefined1 *puStack_140;
  int iStack_138;
  undefined8 uStack_128;
  char *pcStack_120;
  longlong lStack_118;
  longlong *plStack_110;
  undefined8 uStack_108;
  undefined1 uStack_100;
  undefined7 uStack_ff;
  ulonglong uStack_f0;
  ulonglong uStack_e8;
  undefined8 auStack_e0 [3];
  ulonglong uStack_c8;
  undefined8 auStack_c0 [3];
  ulonglong uStack_a8;
  undefined4 auStack_a0 [6];
  undefined1 auStack_88 [64];
  ulonglong uStack_48;
  
  uStack_108 = 0xfffffffffffffffe;
  uStack_48 = _DAT_180025000 ^ (ulonglong)auStack_178;
  lVar3 = *param_2;
  lStack_118 = param_1;
  plStack_110 = param_2;
  if (lVar3 == 0) {
    plVar4 = (longlong *)param_2[1];
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
  }
  else {
    lVar12 = 0;
    uStack_128 = 0;
    pcStack_120 = (char *)0x0;
    uStack_e8 = 0xf;
    uStack_f0 = 0;
    uStack_100 = 0;
    auStack_a0[0] = *(undefined4 *)(param_1 + 0x48);
    iVar8 = *(int *)(lVar3 + 4);
    FUN_180018900(auStack_88,&UNK_18001cd20,*(undefined4 *)(lVar3 + 0x10));
    puStack_158 = &uStack_128;
    iVar8 = FUN_180013f10(param_1,auStack_a0,*(undefined4 *)*param_2,iVar8 == 1);
    if ((iVar8 != 0) && (cVar7 = HSlog_ShouldLog(4), cVar7 != '\0')) {
      puVar5 = (undefined4 *)*param_2;
      if ((_DAT_180025eb0 & 1) == 0) {
        _DAT_180025eb0 = _DAT_180025eb0 | 1;
        FUN_1800013f0();
        FUN_180018df8(FUN_18001a860);
      }
      puVar13 = &DAT_180025e90;
      if (0xf < _DAT_180025ea8) {
        puVar13 = _DAT_180025e90;
      }
      puStack_140 = (undefined1 *)CONCAT44(puStack_140._4_4_,iVar8);
      puStack_148 = (undefined1 *)CONCAT44(puStack_148._4_4_,*puVar5);
      uStack_150 = 0x75;
      puStack_158 = (undefined8 *)&UNK_18001cd30;
      HSlog_LogPrint(4,&UNK_18001cd50,puVar13,*(undefined4 *)(param_1 + 0x48));
    }
    pcVar6 = pcStack_120;
    iVar8 = (int)uStack_128;
    if ((int)uStack_128 == 1) {
      puVar10 = (undefined1 *)FUN_1800129d0(auStack_c0,(ulonglong)pcStack_120 & 0xffffffff);
      if (&uStack_100 != puVar10) {
        if (0xf < uStack_e8) {
          __3_YAXPEAX_Z(CONCAT71(uStack_ff,uStack_100));
        }
        uStack_e8 = 0xf;
        uStack_f0 = 0;
        uStack_100 = 0;
        FUN_180002730(&uStack_100,puVar10);
      }
      if (0xf < uStack_a8) {
        __3_YAXPEAX_Z(auStack_c0[0]);
      }
    }
    else if ((int)uStack_128 == 2) {
      puVar10 = (undefined1 *)FUN_180012b80(auStack_e0,pcStack_120._0_4_);
      if (&uStack_100 != puVar10) {
        FUN_1800020c0(&uStack_100,puVar10);
      }
      if (0xf < uStack_c8) {
        __3_YAXPEAX_Z(auStack_e0[0]);
      }
    }
    else if ((int)uStack_128 == 3) {
      if (*pcStack_120 != '\0') {
        lVar12 = -1;
        do {
          lVar12 = lVar12 + 1;
        } while (pcStack_120[lVar12] != '\0');
      }
      FUN_180002190(&uStack_100,pcStack_120,lVar12);
    }
    else {
      FUN_180002190(&uStack_100,&UNK_18001cd98,7);
      cVar7 = HSlog_ShouldLog(4);
      if (cVar7 != '\0') {
        puVar11 = (undefined8 *)FUN_1800013b0();
        if (0xf < (ulonglong)puVar11[3]) {
          puVar11 = (undefined8 *)*puVar11;
        }
        uStack_150 = 0x8b;
        puStack_158 = (undefined8 *)&UNK_18001cd30;
        HSlog_LogPrint(4,&UNK_18001cda0,puVar11,*(undefined4 *)(param_1 + 0x48));
      }
    }
    puVar10 = &uStack_100;
    if (0xf < uStack_e8) {
      puVar10 = (undefined1 *)CONCAT71(uStack_ff,uStack_100);
    }
    iVar9 = VM_M_SetBinayData(*(undefined4 *)(*param_2 + 8),auStack_88,puVar10,
                              uStack_f0 & 0xffffffff);
    if ((iVar9 != 0) && (cVar7 = HSlog_ShouldLog(4), cVar7 != '\0')) {
      puVar5 = (undefined4 *)*param_2;
      if ((_DAT_180025eb0 & 1) == 0) {
        _DAT_180025eb0 = _DAT_180025eb0 | 1;
        FUN_1800013f0();
        FUN_180018df8(FUN_18001a860);
      }
      puVar13 = &DAT_180025e90;
      if (0xf < _DAT_180025ea8) {
        puVar13 = _DAT_180025e90;
      }
      puStack_140 = (undefined1 *)CONCAT44(puStack_140._4_4_,*puVar5);
      puStack_148 = (undefined1 *)CONCAT44(puStack_148._4_4_,puVar5[2]);
      uStack_150 = 0x92;
      puStack_158 = (undefined8 *)&UNK_18001cd30;
      iStack_138 = iVar9;
      HSlog_LogPrint(4,&UNK_18001cdd0,puVar13,*(undefined4 *)(param_1 + 0x48));
    }
    cVar7 = HSlog_ShouldLog(2);
    if (cVar7 != '\0') {
      lVar3 = *param_2;
      puVar10 = &uStack_100;
      if (0xf < uStack_e8) {
        puVar10 = (undefined1 *)CONCAT71(uStack_ff,uStack_100);
      }
      if ((_DAT_180025eb0 & 1) == 0) {
        _DAT_180025eb0 = _DAT_180025eb0 | 1;
        FUN_1800013f0();
        FUN_180018df8(FUN_18001a860);
      }
      puVar13 = &DAT_180025e90;
      if (0xf < _DAT_180025ea8) {
        puVar13 = _DAT_180025e90;
      }
      iStack_138 = *(int *)(lVar3 + 8);
      puStack_148 = auStack_88;
      uStack_150 = 0x94;
      puStack_158 = (undefined8 *)&UNK_18001cd30;
      puStack_140 = puVar10;
      HSlog_LogPrint(2,&UNK_18001ce18,puVar13,*(undefined4 *)(lStack_118 + 0x48));
    }
    if (0xf < uStack_e8) {
      __3_YAXPEAX_Z(CONCAT71(uStack_ff,uStack_100));
    }
    uStack_e8 = 0xf;
    uStack_f0 = 0;
    uStack_100 = 0;
    if ((iVar8 == 3) && (pcVar6 != (char *)0x0)) {
      ___V_YAXPEAX_Z(pcVar6);
    }
    plVar4 = (longlong *)param_2[1];
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
  }
  FUN_180018cd0(uStack_48 ^ (ulonglong)auStack_178);
  return;
}

