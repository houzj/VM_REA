
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_180094c80(longlong param_1,undefined4 param_2,longlong *param_3,undefined8 param_4,
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
  undefined4 extraout_XMM0_Da;
  undefined4 uVar13;
  float fVar14;
  undefined1 auStack_118 [32];
  undefined8 uStack_f8;
  float *pfStack_f0;
  longlong *plStack_e8;
  int iStack_e0;
  undefined8 *puStack_d8;
  int iStack_d0;
  undefined4 auStack_c8 [2];
  float fStack_c0;
  int aiStack_bc [3];
  undefined8 auStack_b0 [3];
  ulonglong uStack_98;
  undefined8 auStack_90 [3];
  ulonglong uStack_78;
  ulonglong uStack_70;
  
  aiStack_bc[1] = -2;
  aiStack_bc[2] = -1;
  uStack_70 = _DAT_1802a5230 ^ (ulonglong)auStack_118;
  uVar7 = 0;
  uVar11 = 0;
  fStack_c0 = 0.0;
  aiStack_bc[0] = 0;
  uVar12 = uVar7;
  auStack_c8[0] = param_2;
  if (param_5 != '\0') {
    lVar6 = FUN_18009a990(param_1 + 0x18,auStack_c8);
    uVar7 = FUN_180007510(*(undefined8 *)(lVar6 + 0x18));
    FUN_18003fc70(uVar7,&UNK_180225f90);
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
    plStack_e8 = (longlong *)aiStack_bc;
    pfStack_f0 = &fStack_c0;
    iVar10 = (int)uVar12;
    uStack_f8 = param_4;
    iVar5 = FUN_1800972f0();
    if (iVar5 != 0) {
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
        pfStack_f0 = (float *)CONCAT44(pfStack_f0._4_4_,auStack_c8[0]);
        uStack_f8 = CONCAT44(uStack_f8._4_4_,0xff);
        plStack_e8 = param_3;
        iStack_e0 = iVar10;
        puStack_d8 = puVar8;
        FUN_180015b40(uVar9,4,&UNK_180225fd8,&UNK_180225f60);
        if (0xf < uStack_78) {
          __3_YAXPEAX_Z(auStack_90[0]);
        }
      }
      goto LAB_1800951ed;
    }
    uVar13 = extraout_XMM0_Da;
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
      uVar13 = FUN_1801e8930(&UNK_180202850);
    }
    plVar3 = _DAT_1802b1240;
    lVar6 = _DAT_1802b1238;
    if (_DAT_1802b1240 != (longlong *)0x0) {
      LOCK();
      *(int *)(_DAT_1802b1240 + 1) = (int)_DAT_1802b1240[1] + 1;
      UNLOCK();
    }
    if (lVar6 == 0) {
      fVar14 = fStack_c0;
      if (plVar3 != (longlong *)0x0) {
        LOCK();
        plVar1 = plVar3 + 1;
        *(int *)plVar1 = (int)*plVar1 + -1;
        UNLOCK();
        if ((int)*plVar1 == 0) {
          uVar13 = (**(code **)*plVar3)(plVar3);
          LOCK();
          piVar2 = (int *)((longlong)plVar3 + 0xc);
          *piVar2 = *piVar2 + -1;
          UNLOCK();
          fVar14 = fStack_c0;
          if (*piVar2 == 0) {
            uVar13 = (**(code **)(*plVar3 + 8))(plVar3);
            fVar14 = fStack_c0;
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
          uVar13 = (**(code **)*plVar3)(plVar3);
          LOCK();
          piVar2 = (int *)((longlong)plVar3 + 0xc);
          *piVar2 = *piVar2 + -1;
          UNLOCK();
          if (*piVar2 == 0) {
            uVar13 = (**(code **)(*plVar3 + 8))(plVar3);
          }
        }
      }
      fVar14 = fStack_c0;
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
        fVar14 = fStack_c0;
        puVar8 = (undefined8 *)(double)fStack_c0;
        plStack_e8 = (longlong *)func_0x000180006450(param_3);
        iStack_d0 = aiStack_bc[0];
        pfStack_f0 = (float *)CONCAT44(pfStack_f0._4_4_,auStack_c8[0]);
        uStack_f8 = CONCAT44(uStack_f8._4_4_,0x103);
        iStack_e0 = iVar10;
        puStack_d8 = puVar8;
        uVar13 = FUN_180015b40(&DAT_1802b1230,0,&UNK_180225f98,&UNK_180225f60);
      }
    }
    if (param_5 != '\0') {
      if (*(int *)(uVar7 + 0x40) == *(int *)(uVar7 + 0x44)) {
        uVar13 = FUN_18000ee00(uVar7 + 0x40,*(int *)(uVar7 + 0x44) + 1);
      }
      *(float *)(*(longlong *)(uVar7 + 0x48) + 8 + (longlong)*(int *)(uVar7 + 0x40) * 4) = fVar14;
      *(int *)(uVar7 + 0x40) = *(int *)(uVar7 + 0x40) + 1;
    }
    uStack_f8 = CONCAT44(uStack_f8._4_4_,fVar14);
    iVar5 = FUN_1800987c0(uVar13,auStack_c8[0],param_3,uVar12);
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
        puVar8 = (undefined8 *)(double)fStack_c0;
        plStack_e8 = (longlong *)func_0x000180006450(param_3);
        pfStack_f0 = (float *)CONCAT44(pfStack_f0._4_4_,auStack_c8[0]);
        uStack_f8 = CONCAT44(uStack_f8._4_4_,0x124);
        iStack_e0 = iVar10;
        puStack_d8 = puVar8;
        FUN_180015b40(&DAT_1802b1230,0,&UNK_180226018,&UNK_180225f60);
      }
    }
    uVar12 = (ulonglong)(iVar10 + 1U);
    if (aiStack_bc[0] <= (int)(iVar10 + 1U)) {
LAB_1800951ed:
      FUN_1801e8810(uStack_70 ^ (ulonglong)auStack_118);
      return;
    }
  }
  cVar4 = FUN_18001dbc0(4);
  if (cVar4 != '\0') {
    puVar8 = (undefined8 *)FUN_18008f750(auStack_b0,iVar5);
    if (0xf < (ulonglong)puVar8[3]) {
      puVar8 = (undefined8 *)*puVar8;
    }
    if (0xf < (ulonglong)param_3[3]) {
      param_3 = (longlong *)*param_3;
    }
    uVar9 = FUN_18001db50();
    pfStack_f0 = (float *)CONCAT44(pfStack_f0._4_4_,auStack_c8[0]);
    uStack_f8 = CONCAT44(uStack_f8._4_4_,0x120);
    plStack_e8 = param_3;
    iStack_e0 = iVar10;
    puStack_d8 = puVar8;
    FUN_180015b40(uVar9,4,&UNK_180226050,&UNK_180225f60);
    if (0xf < uStack_98) {
      __3_YAXPEAX_Z(auStack_b0[0]);
    }
  }
  goto LAB_1800951ed;
}

