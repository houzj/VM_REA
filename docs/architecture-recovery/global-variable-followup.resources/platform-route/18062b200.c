
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_18062b200(undefined8 param_1,char *param_2,int param_3,undefined8 *param_4,longlong param_5
                  ,longlong param_6,longlong *param_7)

{
  longlong *plVar1;
  int *piVar2;
  int iVar3;
  bool bVar4;
  undefined4 uVar5;
  int iVar6;
  longlong lVar7;
  ulonglong uVar8;
  char *pcVar9;
  uint uVar10;
  longlong *plVar12;
  undefined1 auStack_a8 [32];
  undefined4 uStack_88;
  longlong lStack_80;
  int iStack_78;
  undefined8 uStack_68;
  char acStack_60 [32];
  ulonglong uStack_40;
  ulonglong uVar11;
  
  uStack_68 = 0xfffffffffffffffe;
  uStack_40 = _DAT_1808559e0 ^ (ulonglong)auStack_a8;
  acStack_60[0] = '\0';
  acStack_60[1] = '\0';
  acStack_60[2] = '\0';
  acStack_60[3] = '\0';
  acStack_60[4] = '\0';
  acStack_60[5] = '\0';
  acStack_60[6] = '\0';
  acStack_60[7] = '\0';
  acStack_60[8] = '\0';
  acStack_60[9] = '\0';
  acStack_60[10] = '\0';
  acStack_60[0xb] = '\0';
  acStack_60[0xc] = '\0';
  acStack_60[0xd] = '\0';
  acStack_60[0xe] = '\0';
  acStack_60[0xf] = '\0';
  acStack_60[0x10] = '\0';
  acStack_60[0x11] = '\0';
  acStack_60[0x12] = '\0';
  acStack_60[0x13] = '\0';
  acStack_60[0x14] = '\0';
  acStack_60[0x15] = '\0';
  acStack_60[0x16] = '\0';
  acStack_60[0x17] = '\0';
  acStack_60[0x18] = '\0';
  acStack_60[0x19] = '\0';
  acStack_60[0x1a] = '\0';
  acStack_60[0x1b] = '\0';
  acStack_60[0x1c] = '\0';
  acStack_60[0x1d] = '\0';
  acStack_60[0x1e] = '\0';
  acStack_60[0x1f] = 0;
  bVar4 = false;
  uVar8 = 0;
  uVar10 = 0;
  if (0 < param_3) {
    pcVar9 = param_2;
    uVar11 = uVar8;
    do {
      uVar10 = (uint)uVar11;
      if (0x1f < (longlong)uVar8) break;
      if (*pcVar9 == '-') {
        if (bVar4) break;
        bVar4 = true;
      }
      else if (bVar4) {
        acStack_60[uVar8] = *pcVar9;
        uVar8 = uVar8 + 1;
      }
      uVar10 = uVar10 + 1;
      uVar11 = (ulonglong)uVar10;
      pcVar9 = pcVar9 + 1;
    } while ((longlong)pcVar9 - (longlong)param_2 < (longlong)param_3);
  }
  uVar5 = atoi(acStack_60);
  if ((_DAT_18087957c & 1) == 0) {
    _DAT_18087957c = _DAT_18087957c | 1;
    FUN_1805d2e40();
    FUN_180745c88(&UNK_18075ec30);
  }
  iVar6 = FUN_1805edd00(&DAT_180879580,uVar5,param_2 + (longlong)(int)uVar10 + 1,
                        (param_3 - uVar10) + -1);
  if (iVar6 == 0) {
    _snprintf(param_5,param_6 + -1,&UNK_1807d7774);
  }
  else {
    _snprintf(param_5,param_6 + -1,&UNK_1807d7c04,iVar6);
  }
  lVar7 = -1;
  do {
    lVar7 = lVar7 + 1;
  } while (*(char *)(lVar7 + param_5) != '\0');
  *param_7 = lVar7;
  if ((_DAT_1808793b0 & 1) == 0) {
    _DAT_1808793b0 = _DAT_1808793b0 | 1;
    _DAT_180879360 = 1;
    _DAT_180879368 = 0;
    _DAT_180879370 = (longlong *)0x0;
    uRam0000000180879378 = 0;
    _DAT_180879380 = 0;
    uRam0000000180879388 = 0;
    _DAT_180879390 = 0;
    uRam0000000180879398 = 0;
    _DAT_1808793a0 = 0;
    FUN_180745c88(&UNK_18075e4e0);
  }
  plVar12 = _DAT_180879370;
  lVar7 = _DAT_180879368;
  if (_DAT_180879370 != (longlong *)0x0) {
    LOCK();
    *(int *)(_DAT_180879370 + 1) = (int)_DAT_180879370[1] + 1;
    UNLOCK();
  }
  if (lVar7 == 0) {
    if (plVar12 != (longlong *)0x0) {
      LOCK();
      plVar1 = plVar12 + 1;
      *(int *)plVar1 = (int)*plVar1 + -1;
      UNLOCK();
      if ((int)*plVar1 == 0) {
        (**(code **)*plVar12)(plVar12);
        LOCK();
        piVar2 = (int *)((longlong)plVar12 + 0xc);
        *piVar2 = *piVar2 + -1;
        UNLOCK();
        if (*piVar2 == 0) {
          (**(code **)(*plVar12 + 8))(plVar12);
        }
      }
    }
  }
  else {
    iVar3 = *(int *)(lVar7 + 0x40);
    if (plVar12 != (longlong *)0x0) {
      LOCK();
      plVar1 = plVar12 + 1;
      *(int *)plVar1 = (int)*plVar1 + -1;
      UNLOCK();
      if ((int)*plVar1 == 0) {
        (**(code **)*plVar12)(plVar12);
        LOCK();
        piVar2 = (int *)((longlong)plVar12 + 0xc);
        *piVar2 = *piVar2 + -1;
        UNLOCK();
        if (*piVar2 == 0) {
          (**(code **)(*plVar12 + 8))(plVar12);
        }
      }
    }
    if (iVar3 < 2) {
      plVar12 = (longlong *)*param_4;
      if (0xf < (ulonglong)plVar12[3]) {
        plVar12 = (longlong *)*plVar12;
      }
      if ((_DAT_1808793b0 & 1) == 0) {
        _DAT_1808793b0 = _DAT_1808793b0 | 1;
        _DAT_180879360 = 1;
        _DAT_180879368 = 0;
        _DAT_180879370 = (longlong *)0x0;
        uRam0000000180879378 = 0;
        _DAT_180879380 = 0;
        uRam0000000180879388 = 0;
        _DAT_180879390 = 0;
        uRam0000000180879398 = 0;
        _DAT_1808793a0 = 0;
        FUN_180745c88(&UNK_18075e4e0);
      }
      uStack_88 = 0x1778;
      lStack_80 = (longlong)plVar12;
      iStack_78 = iVar6;
      FUN_18057a4b0(&DAT_180879360,1,&UNK_1807e1d98,&UNK_1807e1dd8);
    }
  }
  FUN_180745b60(uStack_40 ^ (ulonglong)auStack_a8);
  return;
}

