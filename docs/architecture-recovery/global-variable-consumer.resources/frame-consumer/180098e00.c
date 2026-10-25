
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_180098e00(longlong param_1,ulonglong param_2,longlong *param_3,undefined4 param_4,
                  longlong param_5,undefined4 param_6,undefined8 param_7,char param_8)

{
  longlong *plVar1;
  int *piVar2;
  longlong lVar3;
  char cVar4;
  int iVar5;
  longlong lVar6;
  undefined8 uVar7;
  longlong lVar8;
  longlong *plVar9;
  longlong lVar10;
  undefined1 auStack_138 [32];
  longlong lStack_118;
  undefined8 uStack_110;
  longlong *plStack_108;
  undefined4 uStack_100;
  uint auStack_f8 [2];
  longlong lStack_f0;
  longlong *plStack_e8;
  longlong alStack_e0 [3];
  char acStack_c8 [128];
  ulonglong uStack_48;
  
  alStack_e0[1] = 0xfffffffffffffffe;
  uStack_48 = _DAT_1802a5230 ^ (ulonglong)auStack_138;
  auStack_f8[0] = (uint)param_2;
  alStack_e0[0] = 0;
  plVar9 = param_3;
  if (0xf < (ulonglong)param_3[3]) {
    plVar9 = (longlong *)*param_3;
  }
  if ((_DAT_1802b3358 & 1) == 0) {
    _DAT_1802b3358 = _DAT_1802b3358 | 1;
    FUN_18008f080();
    FUN_1801e8930(&UNK_180202f60);
    param_2 = (ulonglong)auStack_f8[0];
  }
  if ((int)param_2 != -1) {
    plStack_108 = alStack_e0;
    uStack_110 = CONCAT44(uStack_110._4_4_,param_6);
    lStack_118 = param_5;
    iVar5 = FUN_1800ee6b0(&DAT_1802b3360,param_2,plVar9,param_4);
    lVar8 = alStack_e0[0];
    if (iVar5 == 0) goto LAB_180098f16;
  }
  cVar4 = FUN_18001dbc0(4);
  lVar8 = param_5;
  if (cVar4 != '\0') {
    plVar9 = param_3;
    if (0xf < (ulonglong)param_3[3]) {
      plVar9 = (longlong *)*param_3;
    }
    uVar7 = FUN_18001db50();
    uStack_100 = param_6;
    uStack_110 = CONCAT44(uStack_110._4_4_,auStack_f8[0]);
    lStack_118 = CONCAT44(lStack_118._4_4_,0x4c9);
    plStack_108 = plVar9;
    FUN_180015b40(uVar7,4,&UNK_180226a00,&UNK_180226a88);
  }
LAB_180098f16:
  if (param_8 != '\0') {
    lVar6 = FUN_18009a990(param_1 + 0x18,auStack_f8);
    lVar6 = FUN_180007510(*(undefined8 *)(lVar6 + 0x18));
    FUN_18003fc70(lVar6,&UNK_180226ad0);
    *(uint *)(lVar6 + 0x10) = *(uint *)(lVar6 + 0x10) | 1;
    plVar9 = *(longlong **)(lVar6 + 0x18);
    if (plVar9 == _DAT_1802b05e8) {
      FUN_180001ac0(lVar6 + 0x18,param_3);
    }
    else if (plVar9 != param_3) {
      FUN_180006470(plVar9,param_3,0,0xffffffffffffffff);
    }
    lStack_118 = lVar8;
    FUN_18009ae50(acStack_c8,0x80,&UNK_180226ac4,param_6);
    uVar7 = FUN_18000de60(lVar6 + 0x28);
    if (acStack_c8[0] == '\0') {
      lVar10 = 0;
    }
    else {
      lVar6 = -1;
      do {
        lVar10 = lVar6 + 1;
        lVar3 = lVar6 + 1;
        lVar6 = lVar10;
      } while (acStack_c8[lVar3] != '\0');
    }
    FUN_180006300(uVar7,acStack_c8,lVar10);
  }
  alStack_e0[2] = 0x1802b1208;
  FUN_1801e5430(0x1802b1208);
  if ((_DAT_1802b2b48 & 1) == 0) {
    _DAT_1802b2b48 = _DAT_1802b2b48 | 1;
    FUN_1800c8f10(0x1802b2b50);
    FUN_1801e8930(&UNK_180202a80);
  }
  if (lVar8 != 0) {
    FUN_1800cae00(0x1802b2b50,&lStack_f0,auStack_f8[0]);
    plVar9 = plStack_e8;
    if (lStack_f0 == 0) {
      if (plStack_e8 != (longlong *)0x0) {
        LOCK();
        plVar1 = plStack_e8 + 1;
        *(int *)plVar1 = (int)*plVar1 + -1;
        UNLOCK();
        if ((int)*plVar1 == 0) {
          (**(code **)*plStack_e8)(plStack_e8);
          LOCK();
          piVar2 = (int *)((longlong)plVar9 + 0xc);
          *piVar2 = *piVar2 + -1;
          UNLOCK();
          if (*piVar2 == 0) {
            (**(code **)(*plStack_e8 + 8))();
          }
        }
      }
    }
    else {
      uStack_110 = 0;
      lStack_118 = CONCAT44(lStack_118._4_4_,param_6);
      FUN_1800f4e80(lStack_f0,param_3,param_4,lVar8);
      plVar9 = plStack_e8;
      if (plStack_e8 != (longlong *)0x0) {
        LOCK();
        plVar1 = plStack_e8 + 1;
        *(int *)plVar1 = (int)*plVar1 + -1;
        UNLOCK();
        if ((int)*plVar1 == 0) {
          (**(code **)*plStack_e8)(plStack_e8);
          LOCK();
          piVar2 = (int *)((longlong)plVar9 + 0xc);
          *piVar2 = *piVar2 + -1;
          UNLOCK();
          if (*piVar2 == 0) {
            (**(code **)(*plStack_e8 + 8))();
          }
        }
      }
    }
  }
  FUN_1801e5390(0x1802b1208);
  FUN_1801e8810(uStack_48 ^ (ulonglong)auStack_138);
  return;
}

