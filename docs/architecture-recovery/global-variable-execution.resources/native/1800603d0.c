
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_1800603d0(undefined8 param_1,undefined8 *param_2,longlong *param_3,undefined1 *param_4)

{
  int *piVar1;
  longlong *plVar2;
  int iVar3;
  undefined8 *puVar4;
  ulonglong uVar5;
  longlong lVar6;
  ulonglong uVar7;
  char cVar8;
  int iVar9;
  undefined4 uVar10;
  uint uVar11;
  uint uVar12;
  undefined8 uVar13;
  longlong *plVar14;
  undefined1 *puVar15;
  undefined4 *puVar16;
  undefined8 *puVar17;
  undefined8 *puVar18;
  ulonglong uVar19;
  int iVar20;
  undefined1 auStack_138 [32];
  undefined1 *puStack_118;
  undefined8 *puStack_110;
  undefined8 *puStack_108;
  int iStack_f8;
  undefined1 uStack_f4;
  undefined4 uStack_f0;
  int iStack_ec;
  undefined4 *puStack_e8;
  longlong *plStack_e0;
  undefined1 *puStack_d8;
  longlong *plStack_d0;
  undefined4 *puStack_c8;
  longlong *plStack_c0;
  undefined4 *puStack_b8;
  longlong *plStack_b0;
  int iStack_a8;
  longlong *plStack_a0;
  undefined8 *puStack_98;
  undefined8 *puStack_90;
  undefined8 uStack_88;
  undefined1 uStack_80;
  undefined7 uStack_7f;
  ulonglong uStack_70;
  ulonglong uStack_68;
  undefined8 auStack_60 [3];
  ulonglong uStack_48;
  ulonglong uStack_40;
  
  uStack_88 = 0xfffffffffffffffe;
  uStack_40 = _DAT_1802a5230 ^ (ulonglong)auStack_138;
  uVar12 = 0;
  iStack_f8 = 0;
  puStack_d8 = param_4;
  plStack_d0 = param_3;
  puStack_90 = param_2;
  if ((_DAT_1802b2ec0 & 1) == 0) {
    _DAT_1802b2ec0 = _DAT_1802b2ec0 | 1;
    FUN_18009b340();
    FUN_1801e8930(&UNK_180202d50);
  }
  iVar9 = FUN_18009c640(0x1802b2ed0,param_4);
  iVar20 = iVar9;
  if (_DAT_1802b12a0 == 1) {
    iVar20 = iVar9 + 4;
  }
  puStack_e8 = (undefined4 *)0x0;
  plStack_e0 = (longlong *)0x0;
  iStack_ec = iVar20;
  if (iVar9 < 1) {
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
    plVar14 = _DAT_1802b1240;
    lVar6 = _DAT_1802b1238;
    if (_DAT_1802b1240 != (longlong *)0x0) {
      LOCK();
      *(int *)(_DAT_1802b1240 + 1) = (int)_DAT_1802b1240[1] + 1;
      UNLOCK();
    }
    if (lVar6 == 0) {
      if (plVar14 != (longlong *)0x0) {
        LOCK();
        plVar2 = plVar14 + 1;
        *(int *)plVar2 = (int)*plVar2 + -1;
        UNLOCK();
        if ((int)*plVar2 == 0) {
          (**(code **)*plVar14)(plVar14);
          LOCK();
          piVar1 = (int *)((longlong)plVar14 + 0xc);
          *piVar1 = *piVar1 + -1;
          UNLOCK();
          if (*piVar1 == 0) {
            (**(code **)(*plVar14 + 8))(plVar14);
          }
        }
      }
    }
    else {
      iVar9 = *(int *)(lVar6 + 0x40);
      if (plVar14 != (longlong *)0x0) {
        LOCK();
        plVar2 = plVar14 + 1;
        *(int *)plVar2 = (int)*plVar2 + -1;
        UNLOCK();
        if ((int)*plVar2 == 0) {
          (**(code **)*plVar14)(plVar14);
          LOCK();
          piVar1 = (int *)((longlong)plVar14 + 0xc);
          *piVar1 = *piVar1 + -1;
          UNLOCK();
          if (*piVar1 == 0) {
            (**(code **)(*plVar14 + 8))(plVar14);
          }
        }
      }
      if (iVar9 < 3) {
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
        puStack_110 = (undefined8 *)func_0x000180006450(param_2);
        puStack_118 = (undefined1 *)CONCAT44(puStack_118._4_4_,0x179a);
        FUN_180015b40(&DAT_1802b1230,2,&UNK_180222ea0,&UNK_180222e30);
      }
    }
  }
  else {
    uVar13 = func_0x0001801e8774((longlong)iVar20);
    puStack_c8 = (undefined4 *)0x0;
    plStack_c0 = (longlong *)0x0;
    FUN_180071e00(&puStack_c8,uVar13,uStack_f4);
    plVar14 = plStack_c0;
    puVar16 = puStack_c8;
    plStack_e0 = plStack_c0;
    puStack_e8 = puStack_c8;
    func_0x0001801e9194(puStack_c8,0,(longlong)iVar20);
    if (_DAT_1802b12a0 == 1) {
      uVar10 = htonl(iVar20 + -4);
      *puVar16 = uVar10;
      puVar16 = puVar16 + 1;
    }
    uStack_f0 = 0;
    if ((_DAT_1802b2ec0 & 1) == 0) {
      _DAT_1802b2ec0 = _DAT_1802b2ec0 | 1;
      FUN_18009b340();
      FUN_1801e8930(&UNK_180202d50);
    }
    puStack_118 = puStack_d8;
    iVar9 = FUN_18009c7a0(0x1802b2ed0,puVar16,iVar9,&uStack_f0);
    iStack_f8 = iVar9;
    if (iVar9 != 0) {
      cVar8 = FUN_18001dbc0(4);
      if (cVar8 != '\0') {
        puStack_108 = (undefined8 *)FUN_18008f750(auStack_60,iVar9);
        if (0xf < (ulonglong)puStack_108[3]) {
          puStack_108 = (undefined8 *)*puStack_108;
        }
        if (0xf < (ulonglong)param_2[3]) {
          param_2 = (undefined8 *)*param_2;
        }
        uVar13 = FUN_18001db50();
        puStack_118 = (undefined1 *)CONCAT44(puStack_118._4_4_,0x1794);
        puStack_110 = param_2;
        FUN_180015b40(uVar13,4,&UNK_180222e60,&UNK_180222e30);
        if (0xf < uStack_48) {
          __3_YAXPEAX_Z(auStack_60[0]);
        }
      }
      plVar2 = plStack_c0;
      if (plVar14 != (longlong *)0x0) {
        LOCK();
        plVar14 = plVar14 + 1;
        *(int *)plVar14 = (int)*plVar14 + -1;
        UNLOCK();
        if ((int)*plVar14 == 0) {
          (**(code **)*plStack_c0)(plStack_c0);
          LOCK();
          piVar1 = (int *)((longlong)plVar2 + 0xc);
          *piVar1 = *piVar1 + -1;
          UNLOCK();
          if (*piVar1 == 0) {
            (**(code **)(*plStack_c0 + 8))();
          }
        }
      }
      goto LAB_180060b69;
    }
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
  plVar14 = _DAT_1802b1240;
  lVar6 = _DAT_1802b1238;
  if (_DAT_1802b1240 != (longlong *)0x0) {
    LOCK();
    *(int *)(_DAT_1802b1240 + 1) = (int)_DAT_1802b1240[1] + 1;
    UNLOCK();
  }
  if (lVar6 == 0) {
    if (plVar14 != (longlong *)0x0) {
      LOCK();
      plVar2 = plVar14 + 1;
      lVar6 = *plVar2;
      *(int *)plVar2 = (int)*plVar2 + -1;
      UNLOCK();
      if ((int)lVar6 == 1) {
        (**(code **)*plVar14)(plVar14);
        LOCK();
        piVar1 = (int *)((longlong)plVar14 + 0xc);
        iVar9 = *piVar1;
        *piVar1 = *piVar1 + -1;
        UNLOCK();
        if (iVar9 == 1) {
          (**(code **)(*plVar14 + 8))(plVar14);
        }
      }
    }
  }
  else {
    iVar9 = *(int *)(lVar6 + 0x40);
    if (plVar14 != (longlong *)0x0) {
      LOCK();
      plVar2 = plVar14 + 1;
      lVar6 = *plVar2;
      *(int *)plVar2 = (int)*plVar2 + -1;
      UNLOCK();
      if ((int)lVar6 == 1) {
        (**(code **)*plVar14)(plVar14);
        LOCK();
        piVar1 = (int *)((longlong)plVar14 + 0xc);
        iVar3 = *piVar1;
        *piVar1 = *piVar1 + -1;
        UNLOCK();
        if (iVar3 == 1) {
          (**(code **)(*plVar14 + 8))(plVar14);
        }
      }
    }
    if (iVar9 < 3) {
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
      puStack_110 = (undefined8 *)func_0x000180006450(param_2);
      puStack_108 = (undefined8 *)CONCAT44(puStack_108._4_4_,iVar20);
      puStack_118 = (undefined1 *)CONCAT44(puStack_118._4_4_,0x179f);
      FUN_180015b40(&DAT_1802b1230,2,&UNK_180222ed0,&UNK_180222e30);
    }
  }
  plVar14 = plStack_d0;
  plStack_a0 = plStack_d0 + 3;
  iVar9 = _Mtx_lock();
  if (iVar9 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar9);
  }
  uStack_68 = 0xf;
  uStack_70 = 0;
  uStack_80 = 0;
  FUN_180006300(&uStack_80,&UNK_180222f00,0x10);
  uVar7 = uStack_70;
  if (plStack_e0 != (longlong *)0x0) {
    LOCK();
    *(int *)(plStack_e0 + 1) = (int)plStack_e0[1] + 1;
    UNLOCK();
  }
  plStack_b0 = plStack_e0;
  puStack_b8 = puStack_e8;
  plStack_d0 = plVar14 + 1;
  puVar4 = (undefined8 *)*plStack_d0;
  puVar17 = puVar4;
  puVar18 = (undefined8 *)puVar4[1];
  iStack_a8 = iVar20;
  while (*(char *)((longlong)puVar18 + 0x19) == '\0') {
    plVar14 = puVar18 + 4;
    puVar15 = &uStack_80;
    if (0xf < uStack_68) {
      puVar15 = (undefined1 *)CONCAT71(uStack_7f,uStack_80);
    }
    uVar5 = puVar18[6];
    if (0xf < (ulonglong)puVar18[7]) {
      plVar14 = (longlong *)*plVar14;
    }
    uVar19 = uVar7;
    if (uVar5 < uVar7) {
      uVar19 = uVar5;
    }
    uVar11 = uVar12;
    if (uVar19 != 0) {
      uVar11 = func_0x0001801e92c0(plVar14,puVar15);
    }
    if (uVar11 == 0) {
      if (uVar7 <= uVar5) {
        uVar11 = (uint)(uVar5 != uVar7);
        goto LAB_1800609e0;
      }
LAB_1800609ec:
      puVar18 = (undefined8 *)puVar18[2];
    }
    else {
LAB_1800609e0:
      if ((int)uVar11 < 0) goto LAB_1800609ec;
      puVar17 = puVar18;
      puVar18 = (undefined8 *)*puVar18;
    }
  }
  if (puVar17 == puVar4) {
LAB_180060a4e:
    plVar14 = plStack_d0;
    puStack_d8 = &uStack_80;
    puStack_118 = (undefined1 *)FUN_180071e70(plStack_d0);
    FUN_180071ee0(plVar14,&puStack_98,puVar17,puStack_118 + 0x20);
    puVar17 = puStack_98;
  }
  else {
    plVar14 = puVar17 + 4;
    uVar5 = puVar17[6];
    if (0xf < (ulonglong)puVar17[7]) {
      plVar14 = (longlong *)*plVar14;
    }
    puVar15 = &uStack_80;
    if (0xf < uStack_68) {
      puVar15 = (undefined1 *)CONCAT71(uStack_7f,uStack_80);
    }
    uVar19 = uVar5;
    if (uVar7 < uVar5) {
      uVar19 = uVar7;
    }
    if (uVar19 != 0) {
      uVar12 = func_0x0001801e92c0(puVar15,plVar14);
    }
    if (uVar12 == 0) {
      if (uVar7 < uVar5) {
        uVar12 = 0xffffffff;
      }
      else {
        uVar12 = (uint)(uVar7 != uVar5);
      }
    }
    if ((int)uVar12 < 0) goto LAB_180060a4e;
  }
  FUN_18001d870(puVar17 + 8,&puStack_b8);
  plVar14 = plStack_b0;
  *(int *)(puVar17 + 10) = iStack_a8;
  if (plStack_b0 != (longlong *)0x0) {
    LOCK();
    plVar2 = plStack_b0 + 1;
    lVar6 = *plVar2;
    *(int *)plVar2 = (int)*plVar2 + -1;
    UNLOCK();
    if ((int)lVar6 == 1) {
      (**(code **)*plStack_b0)(plStack_b0);
      LOCK();
      piVar1 = (int *)((longlong)plVar14 + 0xc);
      iVar20 = *piVar1;
      *piVar1 = *piVar1 + -1;
      UNLOCK();
      if (iVar20 == 1) {
        (**(code **)(*plStack_b0 + 8))();
      }
    }
  }
  if (0xf < uStack_68) {
    __3_YAXPEAX_Z(CONCAT71(uStack_7f,uStack_80));
  }
  iVar20 = _Mtx_unlock(plStack_a0);
  if (iVar20 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar20);
  }
  plVar14 = plStack_e0;
  if (plStack_e0 != (longlong *)0x0) {
    LOCK();
    plVar2 = plStack_e0 + 1;
    lVar6 = *plVar2;
    *(int *)plVar2 = (int)*plVar2 + -1;
    UNLOCK();
    if ((int)lVar6 == 1) {
      (**(code **)*plStack_e0)(plStack_e0);
      LOCK();
      piVar1 = (int *)((longlong)plVar14 + 0xc);
      iVar20 = *piVar1;
      *piVar1 = *piVar1 + -1;
      UNLOCK();
      if (iVar20 == 1) {
        (**(code **)(*plStack_e0 + 8))();
      }
    }
  }
LAB_180060b69:
  FUN_1801e8810(uStack_40 ^ (ulonglong)auStack_138);
  return;
}

