
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_180094700(longlong param_1,undefined4 param_2,longlong *param_3,undefined8 param_4,
                  char param_5)

{
  longlong *plVar1;
  int *piVar2;
  longlong *plVar3;
  char cVar4;
  int iVar5;
  longlong lVar6;
  ulonglong uVar7;
  undefined8 *puVar8;
  undefined8 uVar9;
  int iVar10;
  ulonglong uVar11;
  ulonglong uVar12;
  undefined4 uVar13;
  undefined4 extraout_XMM0_Da;
  undefined4 uVar14;
  undefined1 auStack_f8 [32];
  undefined8 uStack_d8;
  undefined4 *puStack_d0;
  longlong *plStack_c8;
  int iStack_c0;
  undefined8 *puStack_b8;
  int iStack_b0;
  undefined4 auStack_a8 [2];
  undefined4 uStack_a0;
  int aiStack_9c [3];
  undefined8 auStack_90 [3];
  ulonglong uStack_78;
  undefined8 auStack_70 [3];
  ulonglong uStack_58;
  ulonglong uStack_50;
  
  aiStack_9c[1] = -2;
  aiStack_9c[2] = -1;
  uStack_50 = _DAT_1802a5230 ^ (ulonglong)auStack_f8;
  uVar7 = 0;
  uVar11 = 0;
  uStack_a0 = 0;
  aiStack_9c[0] = 0;
  uVar12 = uVar7;
  auStack_a8[0] = param_2;
  if (param_5 != '\0') {
    lVar6 = FUN_18009a990(param_1 + 0x18,auStack_a8);
    uVar7 = FUN_180007510(*(undefined8 *)(lVar6 + 0x18));
    FUN_18003fc70(uVar7,&UNK_180225e64);
    *(uint *)(uVar7 + 0x10) = *(uint *)(uVar7 + 0x10) | 1;
    plVar3 = *(longlong **)(uVar7 + 0x18);
    uVar12 = uVar11;
    if (plVar3 == _DAT_1802b05e8) {
      FUN_180001ac0(uVar7 + 0x18,param_3);
    }
    else if (plVar3 != param_3) {
      FUN_180006470(plVar3,param_3,0,0xffffffffffffffff);
    }
  }
  while( true ) {
    plStack_c8 = (longlong *)aiStack_9c;
    puStack_d0 = &uStack_a0;
    iVar10 = (int)uVar12;
    uStack_d8 = param_4;
    iVar5 = FUN_180096fe0();
    if (iVar5 != 0) {
      cVar4 = FUN_18001dbc0(4);
      if (cVar4 != '\0') {
        puVar8 = (undefined8 *)FUN_18008f750(auStack_70,iVar5);
        if (0xf < (ulonglong)puVar8[3]) {
          puVar8 = (undefined8 *)*puVar8;
        }
        if (0xf < (ulonglong)param_3[3]) {
          param_3 = (longlong *)*param_3;
        }
        uVar9 = FUN_18001db50();
        puStack_d0 = (undefined4 *)CONCAT44(puStack_d0._4_4_,auStack_a8[0]);
        uStack_d8 = CONCAT44(uStack_d8._4_4_,0xb2);
        plStack_c8 = param_3;
        iStack_c0 = iVar10;
        puStack_b8 = puVar8;
        FUN_180015b40(uVar9,4,&UNK_180225ea8,&UNK_180225e38);
        if (0xf < uStack_58) {
          __3_YAXPEAX_Z(auStack_70[0]);
        }
      }
      goto LAB_180094c55;
    }
    uVar14 = extraout_XMM0_Da;
    if ((_DAT_1802b1218 & 1) == 0) {
      _DAT_1802b1218 = _DAT_1802b1218 | 1;
      _DAT_1802b1230 = 1;
      _DAT_1802b1238 = 0;
      _DAT_1802b1240 = (longlong *)0x0;
      uRam00000001802b1248 = 0;
      _DAT_1802b1250 = 0;
      uRam00000001802b1258 = 0;
      _DAT_1802b1260 = 0;
      uRam00000001802b1268 = 0;
      _DAT_1802b1270 = 0;
      uVar14 = FUN_1801e8930(&UNK_180202850);
    }
    plVar3 = _DAT_1802b1240;
    lVar6 = _DAT_1802b1238;
    if (_DAT_1802b1240 != (longlong *)0x0) {
      LOCK();
      *(int *)(_DAT_1802b1240 + 1) = (int)_DAT_1802b1240[1] + 1;
      UNLOCK();
    }
    if (lVar6 == 0) {
      uVar13 = uStack_a0;
      if (plVar3 != (longlong *)0x0) {
        LOCK();
        plVar1 = plVar3 + 1;
        *(int *)plVar1 = (int)*plVar1 + -1;
        UNLOCK();
        if ((int)*plVar1 == 0) {
          uVar14 = (**(code **)*plVar3)(plVar3);
          LOCK();
          piVar2 = (int *)((longlong)plVar3 + 0xc);
          *piVar2 = *piVar2 + -1;
          UNLOCK();
          uVar13 = uStack_a0;
          if (*piVar2 == 0) {
            uVar14 = (**(code **)(*plVar3 + 8))(plVar3);
            uVar13 = uStack_a0;
          }
        }
      }
    }
    else {
      iVar5 = *(int *)(lVar6 + 0x40);
      if (plVar3 != (longlong *)0x0) {
        LOCK();
        plVar1 = plVar3 + 1;
        *(int *)plVar1 = (int)*plVar1 + -1;
        UNLOCK();
        if ((int)*plVar1 == 0) {
          uVar14 = (**(code **)*plVar3)(plVar3);
          LOCK();
          piVar2 = (int *)((longlong)plVar3 + 0xc);
          *piVar2 = *piVar2 + -1;
          UNLOCK();
          if (*piVar2 == 0) {
            uVar14 = (**(code **)(*plVar3 + 8))(plVar3);
          }
        }
      }
      uVar13 = uStack_a0;
      if (iVar5 < 1) {
        if ((_DAT_1802b1218 & 1) == 0) {
          _DAT_1802b1218 = _DAT_1802b1218 | 1;
          _DAT_1802b1230 = 1;
          _DAT_1802b1238 = 0;
          _DAT_1802b1240 = (longlong *)0x0;
          uRam00000001802b1248 = 0;
          _DAT_1802b1250 = 0;
          uRam00000001802b1258 = 0;
          _DAT_1802b1260 = 0;
          uRam00000001802b1268 = 0;
          _DAT_1802b1270 = 0;
          FUN_1801e8930(&UNK_180202850);
        }
        plStack_c8 = (longlong *)func_0x000180006450(param_3);
        uVar13 = uStack_a0;
        iStack_b0 = aiStack_9c[0];
        puStack_b8 = (undefined8 *)CONCAT44(puStack_b8._4_4_,uStack_a0);
        puStack_d0 = (undefined4 *)CONCAT44(puStack_d0._4_4_,auStack_a8[0]);
        uStack_d8 = CONCAT44(uStack_d8._4_4_,0xb6);
        iStack_c0 = iVar10;
        uVar14 = FUN_180015b40(&DAT_1802b1230,0,&UNK_180225e68,&UNK_180225e38);
      }
    }
    if (param_5 != '\0') {
      if (*(int *)(uVar7 + 0x68) == *(int *)(uVar7 + 0x6c)) {
        uVar14 = FUN_18000ee00(uVar7 + 0x68,*(int *)(uVar7 + 0x6c) + 1);
      }
      *(undefined4 *)(*(longlong *)(uVar7 + 0x70) + 8 + (longlong)*(int *)(uVar7 + 0x68) * 4) =
           uVar13;
      *(int *)(uVar7 + 0x68) = *(int *)(uVar7 + 0x68) + 1;
    }
    uStack_d8 = CONCAT44(uStack_d8._4_4_,uVar13);
    iVar5 = FUN_180098630(uVar14,auStack_a8[0],param_3,uVar12);
    if (iVar5 != 0) break;
    if ((_DAT_1802b1218 & 1) == 0) {
      _DAT_1802b1218 = _DAT_1802b1218 | 1;
      _DAT_1802b1230 = 1;
      _DAT_1802b1238 = 0;
      _DAT_1802b1240 = (longlong *)0x0;
      uRam00000001802b1248 = 0;
      _DAT_1802b1250 = 0;
      uRam00000001802b1258 = 0;
      _DAT_1802b1260 = 0;
      uRam00000001802b1268 = 0;
      _DAT_1802b1270 = 0;
      FUN_1801e8930(&UNK_180202850);
    }
    plVar3 = _DAT_1802b1240;
    lVar6 = _DAT_1802b1238;
    if (_DAT_1802b1240 != (longlong *)0x0) {
      LOCK();
      *(int *)(_DAT_1802b1240 + 1) = (int)_DAT_1802b1240[1] + 1;
      UNLOCK();
    }
    if (lVar6 == 0) {
      if (plVar3 != (longlong *)0x0) {
        LOCK();
        plVar1 = plVar3 + 1;
        *(int *)plVar1 = (int)*plVar1 + -1;
        UNLOCK();
        if ((int)*plVar1 == 0) {
          (**(code **)*plVar3)(plVar3);
          LOCK();
          piVar2 = (int *)((longlong)plVar3 + 0xc);
          *piVar2 = *piVar2 + -1;
          UNLOCK();
          if (*piVar2 == 0) {
            (**(code **)(*plVar3 + 8))(plVar3);
          }
        }
      }
    }
    else {
      iVar5 = *(int *)(lVar6 + 0x40);
      if (plVar3 != (longlong *)0x0) {
        LOCK();
        plVar1 = plVar3 + 1;
        *(int *)plVar1 = (int)*plVar1 + -1;
        UNLOCK();
        if ((int)*plVar1 == 0) {
          (**(code **)*plVar3)(plVar3);
          LOCK();
          piVar2 = (int *)((longlong)plVar3 + 0xc);
          *piVar2 = *piVar2 + -1;
          UNLOCK();
          if (*piVar2 == 0) {
            (**(code **)(*plVar3 + 8))(plVar3);
          }
        }
      }
      if (iVar5 < 1) {
        if ((_DAT_1802b1218 & 1) == 0) {
          _DAT_1802b1218 = _DAT_1802b1218 | 1;
          _DAT_1802b1230 = 1;
          _DAT_1802b1238 = 0;
          _DAT_1802b1240 = (longlong *)0x0;
          uRam00000001802b1248 = 0;
          _DAT_1802b1250 = 0;
          uRam00000001802b1258 = 0;
          _DAT_1802b1260 = 0;
          uRam00000001802b1268 = 0;
          _DAT_1802b1270 = 0;
          FUN_1801e8930(&UNK_180202850);
        }
        plStack_c8 = (longlong *)func_0x000180006450(param_3);
        puStack_b8 = (undefined8 *)CONCAT44(puStack_b8._4_4_,uStack_a0);
        puStack_d0 = (undefined4 *)CONCAT44(puStack_d0._4_4_,auStack_a8[0]);
        uStack_d8 = CONCAT44(uStack_d8._4_4_,0xd7);
        iStack_c0 = iVar10;
        FUN_180015b40(&DAT_1802b1230,0,&UNK_180225ee8,&UNK_180225e38);
      }
    }
    uVar12 = (ulonglong)(iVar10 + 1U);
    if (aiStack_9c[0] <= (int)(iVar10 + 1U)) {
LAB_180094c55:
      FUN_1801e8810(uStack_50 ^ (ulonglong)auStack_f8);
      return;
    }
  }
  cVar4 = FUN_18001dbc0(4);
  if (cVar4 != '\0') {
    puVar8 = (undefined8 *)FUN_18008f750(auStack_90,iVar5);
    if (0xf < (ulonglong)puVar8[3]) {
      puVar8 = (undefined8 *)*puVar8;
    }
    if (0xf < (ulonglong)param_3[3]) {
      param_3 = (longlong *)*param_3;
    }
    uVar9 = FUN_18001db50();
    puStack_d0 = (undefined4 *)CONCAT44(puStack_d0._4_4_,auStack_a8[0]);
    uStack_d8 = CONCAT44(uStack_d8._4_4_,0xd3);
    plStack_c8 = param_3;
    iStack_c0 = iVar10;
    puStack_b8 = puVar8;
    FUN_180015b40(uVar9,4,&UNK_180225f20,&UNK_180225e38);
    if (0xf < uStack_78) {
      __3_YAXPEAX_Z(auStack_90[0]);
    }
  }
  goto LAB_180094c55;
}

