
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_180095230(longlong param_1,undefined4 param_2,longlong *param_3,undefined8 param_4,
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
  undefined4 uVar14;
  undefined4 uVar15;
  undefined1 auStack_118 [32];
  undefined8 uStack_f8;
  undefined8 **ppuStack_f0;
  longlong *plStack_e8;
  int iStack_e0;
  undefined8 *puStack_d8;
  int iStack_d0;
  undefined4 auStack_c8 [2];
  int aiStack_c0 [2];
  undefined8 *puStack_b8;
  undefined8 uStack_b0;
  undefined8 auStack_a8 [3];
  ulonglong uStack_90;
  undefined8 auStack_88 [3];
  ulonglong uStack_70;
  ulonglong uStack_68;
  
  uStack_b0 = 0xfffffffffffffffe;
  uStack_68 = _DAT_1802a5230 ^ (ulonglong)auStack_118;
  uVar7 = 0;
  uVar11 = 0;
  puStack_b8 = (undefined8 *)0x0;
  aiStack_c0[0] = 0;
  uVar12 = uVar7;
  auStack_c8[0] = param_2;
  if (param_5 != '\0') {
    lVar6 = FUN_18009a990(param_1 + 0x18,auStack_c8);
    uVar7 = FUN_180007510(*(undefined8 *)(lVar6 + 0x18));
    FUN_18003fc70(uVar7,&UNK_1802260c0);
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
  do {
    plStack_e8 = (longlong *)aiStack_c0;
    ppuStack_f0 = &puStack_b8;
    iVar10 = (int)uVar12;
    uStack_f8 = param_4;
    iVar5 = FUN_180097610();
    if (iVar5 != 0) {
      cVar4 = FUN_18001dbc0(4);
      if (cVar4 != '\0') {
        puVar8 = (undefined8 *)FUN_18008f750(auStack_88,iVar5);
        if (0xf < (ulonglong)puVar8[3]) {
          puVar8 = (undefined8 *)*puVar8;
        }
        if (0xf < (ulonglong)param_3[3]) {
          param_3 = (longlong *)*param_3;
        }
        uVar9 = FUN_18001db50();
        ppuStack_f0 = (undefined8 **)CONCAT44(ppuStack_f0._4_4_,auStack_c8[0]);
        uStack_f8 = CONCAT44(uStack_f8._4_4_,0x149);
        plStack_e8 = param_3;
        iStack_e0 = iVar10;
        puStack_d8 = puVar8;
        FUN_180015b40(uVar9,4,&UNK_180225fd8,&UNK_180226090);
        if (0xf < uStack_70) {
          __3_YAXPEAX_Z(auStack_88[0]);
        }
      }
      goto LAB_180095798;
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
LAB_18009549d:
      uVar14 = SUB84(puStack_b8,0);
      uVar15 = (undefined4)((ulonglong)puStack_b8 >> 0x20);
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
      if (0 < iVar5) goto LAB_18009549d;
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
      plStack_e8 = (longlong *)func_0x000180006450(param_3);
      iStack_d0 = aiStack_c0[0];
      uVar14 = SUB84(puStack_b8,0);
      uVar15 = (undefined4)((ulonglong)puStack_b8 >> 0x20);
      puStack_d8 = puStack_b8;
      ppuStack_f0 = (undefined8 **)CONCAT44(ppuStack_f0._4_4_,auStack_c8[0]);
      uStack_f8 = CONCAT44(uStack_f8._4_4_,0x14d);
      iStack_e0 = iVar10;
      uVar13 = FUN_180015b40(&DAT_1802b1230,0,&UNK_180225f98,&UNK_180226090);
    }
    if (param_5 != '\0') {
      if (*(int *)(uVar7 + 0x78) == *(int *)(uVar7 + 0x7c)) {
        uVar13 = FUN_18000efb0(uVar7 + 0x78,*(int *)(uVar7 + 0x7c) + 1);
      }
      *(ulonglong *)(*(longlong *)(uVar7 + 0x80) + 8 + (longlong)*(int *)(uVar7 + 0x78) * 8) =
           CONCAT44(uVar15,uVar14);
      *(int *)(uVar7 + 0x78) = *(int *)(uVar7 + 0x78) + 1;
    }
    uStack_f8 = CONCAT44(uVar15,uVar14);
    iVar5 = FUN_180098950(uVar13,auStack_c8[0],param_3,uVar12);
    if (iVar5 != 0) {
      cVar4 = FUN_18001dbc0(4);
      if (cVar4 != '\0') {
        puVar8 = (undefined8 *)FUN_18008f750(auStack_a8,iVar5);
        if (0xf < (ulonglong)puVar8[3]) {
          puVar8 = (undefined8 *)*puVar8;
        }
        if (0xf < (ulonglong)param_3[3]) {
          param_3 = (longlong *)*param_3;
        }
        uVar9 = FUN_18001db50();
        ppuStack_f0 = (undefined8 **)CONCAT44(ppuStack_f0._4_4_,auStack_c8[0]);
        uStack_f8 = CONCAT44(uStack_f8._4_4_,0x159);
        plStack_e8 = param_3;
        iStack_e0 = iVar10;
        puStack_d8 = puVar8;
        FUN_180015b40(uVar9,4,&UNK_180226050,&UNK_180226090);
        if (0xf < uStack_90) {
          __3_YAXPEAX_Z(auStack_a8[0]);
        }
      }
      goto LAB_180095798;
    }
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
        plStack_e8 = (longlong *)func_0x000180006450(param_3);
        puStack_d8 = puStack_b8;
        ppuStack_f0 = (undefined8 **)CONCAT44(ppuStack_f0._4_4_,auStack_c8[0]);
        uStack_f8 = CONCAT44(uStack_f8._4_4_,0x15d);
        iStack_e0 = iVar10;
        FUN_180015b40(&DAT_1802b1230,0,&UNK_180226018,&UNK_180226090);
      }
    }
    uVar12 = (ulonglong)(iVar10 + 1U);
    if (aiStack_c0[0] <= (int)(iVar10 + 1U)) {
LAB_180095798:
      FUN_1801e8810(uStack_68 ^ (ulonglong)auStack_118);
      return;
    }
  } while( true );
}

