
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_180060b90(longlong *param_1,undefined8 param_2,undefined8 *param_3,undefined8 param_4,
                  longlong param_5)

{
  int *piVar1;
  int iVar2;
  ulonglong uVar3;
  undefined8 *puVar4;
  longlong *plVar5;
  longlong lVar6;
  ulonglong uVar7;
  ulonglong uVar8;
  char cVar9;
  uint uVar10;
  int iVar11;
  undefined8 *puVar12;
  undefined8 *puVar13;
  undefined8 *puVar14;
  longlong *plVar15;
  undefined1 *puVar16;
  undefined8 *puVar17;
  ulonglong uVar18;
  undefined1 *puVar19;
  int iVar20;
  undefined8 *puVar21;
  undefined8 **ppuVar22;
  undefined8 uVar23;
  undefined1 auStack_108 [32];
  longlong lStack_e8;
  undefined8 uStack_e0;
  undefined8 *puStack_d8;
  undefined8 uStack_c8;
  undefined8 *puStack_b8;
  undefined8 uStack_b0;
  undefined8 *puStack_a8;
  longlong *plStack_a0;
  undefined8 *puStack_98;
  undefined8 *puStack_90;
  longlong lStack_88;
  undefined8 uStack_80;
  undefined1 uStack_78;
  undefined7 uStack_77;
  ulonglong uStack_68;
  ulonglong uStack_60;
  undefined8 auStack_58 [3];
  ulonglong uStack_40;
  ulonglong uStack_38;
  
  uStack_80 = 0xfffffffffffffffe;
  uStack_38 = _DAT_1802a5230 ^ (ulonglong)auStack_108;
  lStack_88 = param_5;
  uStack_60 = 0xf;
  uStack_68 = 0;
  uStack_78 = 0;
  uStack_c8 = param_2;
  puStack_a8 = param_3;
  plStack_a0 = param_1;
  FUN_180006300(&uStack_78,&UNK_180222f00,0x10);
  uVar8 = uStack_60;
  uVar7 = uStack_68;
  puVar14 = (undefined8 *)param_3[1];
  puVar19 = (undefined1 *)CONCAT71(uStack_77,uStack_78);
  cVar9 = *(char *)((longlong)puVar14[1] + 0x19);
  puVar4 = puVar14;
  puVar21 = (undefined8 *)puVar14[1];
  while (cVar9 == '\0') {
    plVar15 = puVar21 + 4;
    puVar16 = &uStack_78;
    if (0xf < uVar8) {
      puVar16 = puVar19;
    }
    uVar3 = puVar21[6];
    if (0xf < (ulonglong)puVar21[7]) {
      plVar15 = (longlong *)*plVar15;
    }
    uVar18 = uVar7;
    if (uVar3 < uVar7) {
      uVar18 = uVar3;
    }
    if (uVar18 == 0) {
      uVar10 = 0;
    }
    else {
      uVar10 = func_0x0001801e92c0(plVar15,puVar16);
      puVar19 = (undefined1 *)CONCAT71(uStack_77,uStack_78);
    }
    if (uVar10 == 0) {
      if (uVar3 < uVar7) {
        uVar10 = 0xffffffff;
      }
      else {
        uVar10 = (uint)(uVar3 != uVar7);
      }
    }
    if ((int)uVar10 < 0) {
      puVar17 = (undefined8 *)puVar21[2];
      puVar21 = puVar4;
    }
    else {
      puVar17 = (undefined8 *)*puVar21;
    }
    puVar4 = puVar21;
    puVar21 = puVar17;
    param_3 = puStack_a8;
    cVar9 = *(char *)((longlong)puVar17 + 0x19);
  }
  if (puVar4 == puVar14) {
LAB_180060d07:
    puStack_90 = puVar14;
    ppuVar22 = &puStack_90;
  }
  else {
    plVar15 = puVar4 + 4;
    uVar3 = puVar4[6];
    if (0xf < (ulonglong)puVar4[7]) {
      plVar15 = (longlong *)*plVar15;
    }
    puVar16 = &uStack_78;
    if (0xf < uVar8) {
      puVar16 = puVar19;
    }
    uVar18 = uVar3;
    if (uVar7 < uVar3) {
      uVar18 = uVar7;
    }
    if (uVar18 == 0) {
      uVar10 = 0;
    }
    else {
      uVar10 = func_0x0001801e92c0(puVar16,plVar15);
      puVar19 = (undefined1 *)CONCAT71(uStack_77,uStack_78);
    }
    if (uVar10 == 0) {
      if (uVar7 < uVar3) {
        uVar10 = 0xffffffff;
      }
      else {
        uVar10 = (uint)(uVar7 != uVar3);
      }
    }
    if ((int)uVar10 < 0) goto LAB_180060d07;
    puStack_a8 = puVar4;
    ppuVar22 = &puStack_a8;
  }
  puVar14 = *ppuVar22;
  if (0xf < uVar8) {
    __3_YAXPEAX_Z(puVar19);
  }
  uStack_60 = 0xf;
  uStack_68 = 0;
  uStack_78 = 0;
  if (puVar14 == (undefined8 *)param_3[1]) {
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
    plVar15 = _DAT_1802b1240;
    lVar6 = _DAT_1802b1238;
    if (_DAT_1802b1240 != (longlong *)0x0) {
      LOCK();
      *(int *)(_DAT_1802b1240 + 1) = (int)_DAT_1802b1240[1] + 1;
      UNLOCK();
    }
    iVar20 = 0;
    if (lVar6 == 0) {
      uVar23 = uStack_c8;
      if (plVar15 != (longlong *)0x0) {
        LOCK();
        plVar5 = plVar15 + 1;
        *(int *)plVar5 = (int)*plVar5 + -1;
        UNLOCK();
        if ((int)*plVar5 == 0) {
          (**(code **)*plVar15)(plVar15);
          LOCK();
          piVar1 = (int *)((longlong)plVar15 + 0xc);
          *piVar1 = *piVar1 + -1;
          UNLOCK();
          uVar23 = uStack_c8;
          if (*piVar1 == 0) {
            (**(code **)(*plVar15 + 8))(plVar15);
            uVar23 = uStack_c8;
          }
        }
      }
    }
    else {
      iVar11 = *(int *)(lVar6 + 0x40);
      if (plVar15 != (longlong *)0x0) {
        LOCK();
        plVar5 = plVar15 + 1;
        *(int *)plVar5 = (int)*plVar5 + -1;
        UNLOCK();
        if ((int)*plVar5 == 0) {
          (**(code **)*plVar15)(plVar15);
          LOCK();
          piVar1 = (int *)((longlong)plVar15 + 0xc);
          *piVar1 = *piVar1 + -1;
          UNLOCK();
          if (*piVar1 == 0) {
            (**(code **)(*plVar15 + 8))(plVar15);
          }
        }
      }
      uVar23 = uStack_c8;
      if (iVar11 < 3) {
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
        uVar23 = uStack_c8;
        uStack_e0 = func_0x000180006450(uStack_c8);
        lStack_e8 = CONCAT44(lStack_e8._4_4_,0x17d3);
        FUN_180015b40(&DAT_1802b1230,2,&UNK_180222ea0,&UNK_180222f50);
      }
    }
  }
  else {
    iVar20 = *(int *)(puVar14 + 10);
    if (iVar20 < 1) {
      cVar9 = FUN_18001dbc0(2);
      uVar23 = uStack_c8;
      if (cVar9 != '\0') {
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
        uVar23 = uStack_c8;
        uStack_e0 = func_0x000180006450(uStack_c8);
        lStack_e8 = CONCAT44(lStack_e8._4_4_,0x17cd);
        FUN_180015b40(&DAT_1802b1230,2,&UNK_180222fc0,&UNK_180222f50);
      }
    }
    else {
      puStack_b8 = (undefined8 *)0x0;
      uStack_b0 = 0;
      puStack_b8 = (undefined8 *)FUN_1800703f0();
      puVar4 = (undefined8 *)plStack_a0[0x7a];
      puVar21 = (undefined8 *)*puVar4;
      plVar15 = plStack_a0;
      while (puVar21 != puVar4) {
        plStack_a0 = puVar21 + 5;
        puVar17 = puStack_b8;
        if (*(char *)((longlong)puStack_b8[1] + 0x19) == '\0') {
          puVar12 = (undefined8 *)puStack_b8[1];
          do {
            if (*(int *)((longlong)puVar12 + 0x1c) < (int)*plStack_a0) {
              puVar13 = (undefined8 *)puVar12[2];
            }
            else {
              puVar13 = (undefined8 *)*puVar12;
              puVar17 = puVar12;
            }
            puVar12 = puVar13;
          } while (*(char *)((longlong)puVar13 + 0x19) == '\0');
        }
        if ((puVar17 == puStack_b8) || ((int)*plStack_a0 < *(int *)((longlong)puVar17 + 0x1c))) {
          lStack_e8 = FUN_1800727a0(&puStack_b8,puStack_b8,&plStack_a0);
          FUN_1800727e0(&puStack_b8,&puStack_98,puVar17,lStack_e8 + 0x1c);
          puVar17 = puStack_98;
          plVar15 = plStack_a0;
        }
        plStack_a0 = plVar15;
        *(undefined4 *)(puVar17 + 4) = *(undefined4 *)(puVar21 + 4);
        plVar15 = plStack_a0;
        if (*(char *)((longlong)puVar21 + 0x19) == '\0') {
          puVar17 = (undefined8 *)puVar21[2];
          if (*(char *)((longlong)puVar17 + 0x19) == '\0') {
            cVar9 = *(char *)((longlong)*puVar17 + 0x19);
            puVar21 = puVar17;
            puVar17 = (undefined8 *)*puVar17;
            while (cVar9 == '\0') {
              cVar9 = *(char *)((longlong)*puVar17 + 0x19);
              puVar21 = puVar17;
              puVar17 = (undefined8 *)*puVar17;
            }
          }
          else {
            cVar9 = *(char *)((longlong)puVar21[1] + 0x19);
            puVar12 = (undefined8 *)puVar21[1];
            puVar17 = puVar21;
            while ((puVar21 = puVar12, cVar9 == '\0' && (puVar17 == (undefined8 *)puVar21[2]))) {
              cVar9 = *(char *)((longlong)puVar21[1] + 0x19);
              puVar12 = (undefined8 *)puVar21[1];
              puVar17 = puVar21;
            }
          }
        }
      }
      uVar23 = puVar14[8];
      plStack_a0 = plVar15;
      if ((_DAT_1802b2ec0 & 1) == 0) {
        _DAT_1802b2ec0 = _DAT_1802b2ec0 | 1;
        FUN_18009b340();
        FUN_1801e8930(&UNK_180202d50);
      }
      lStack_e8 = lStack_88;
      iVar11 = FUN_18009ce80(0x1802b2ed0,uVar23,*(undefined4 *)(puVar14 + 10),&puStack_b8);
      uVar23 = uStack_c8;
      if (iVar11 != 0) {
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
        plVar15 = _DAT_1802b1240;
        lVar6 = _DAT_1802b1238;
        if (_DAT_1802b1240 != (longlong *)0x0) {
          LOCK();
          *(int *)(_DAT_1802b1240 + 1) = (int)_DAT_1802b1240[1] + 1;
          UNLOCK();
        }
        if (lVar6 == 0) {
          uVar23 = uStack_c8;
          if (plVar15 != (longlong *)0x0) {
            LOCK();
            plVar5 = plVar15 + 1;
            *(int *)plVar5 = (int)*plVar5 + -1;
            UNLOCK();
            if ((int)*plVar5 == 0) {
              (**(code **)*plVar15)(plVar15);
              LOCK();
              piVar1 = (int *)((longlong)plVar15 + 0xc);
              *piVar1 = *piVar1 + -1;
              UNLOCK();
              uVar23 = uStack_c8;
              if (*piVar1 == 0) {
                (**(code **)(*plVar15 + 8))(plVar15);
                uVar23 = uStack_c8;
              }
            }
          }
        }
        else {
          iVar2 = *(int *)(lVar6 + 0x40);
          if (plVar15 != (longlong *)0x0) {
            LOCK();
            plVar5 = plVar15 + 1;
            *(int *)plVar5 = (int)*plVar5 + -1;
            UNLOCK();
            if ((int)*plVar5 == 0) {
              (**(code **)*plVar15)(plVar15);
              LOCK();
              piVar1 = (int *)((longlong)plVar15 + 0xc);
              *piVar1 = *piVar1 + -1;
              UNLOCK();
              if (*piVar1 == 0) {
                (**(code **)(*plVar15 + 8))(plVar15);
              }
            }
          }
          uVar23 = uStack_c8;
          if (iVar2 < 5) {
            puVar14 = (undefined8 *)FUN_18008f750(auStack_58,iVar11);
            if (0xf < (ulonglong)puVar14[3]) {
              puVar14 = (undefined8 *)*puVar14;
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
            uVar23 = uStack_c8;
            uStack_e0 = func_0x000180006450(uStack_c8);
            lStack_e8 = CONCAT44(lStack_e8._4_4_,0x17c8);
            puStack_d8 = puVar14;
            FUN_180015b40(&DAT_1802b1230,4,&UNK_180222f80,&UNK_180222f50);
            if (0xf < uStack_40) {
              __3_YAXPEAX_Z(auStack_58[0]);
            }
          }
        }
      }
      cVar9 = *(char *)((longlong)puStack_b8[1] + 0x19);
      plVar15 = (longlong *)puStack_b8[1];
      while (cVar9 == '\0') {
        FUN_18003c6d0(&puStack_b8,plVar15[2]);
        plVar5 = (longlong *)*plVar15;
        __3_YAXPEAX_Z(plVar15);
        plVar15 = plVar5;
        cVar9 = *(char *)((longlong)plVar5 + 0x19);
      }
      puStack_b8[1] = puStack_b8;
      *puStack_b8 = puStack_b8;
      puStack_b8[2] = puStack_b8;
      uStack_b0 = 0;
      __3_YAXPEAX_Z(puStack_b8);
      if (iVar11 != 0) goto LAB_18006143d;
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
  plVar15 = _DAT_1802b1240;
  lVar6 = _DAT_1802b1238;
  if (_DAT_1802b1240 != (longlong *)0x0) {
    LOCK();
    *(int *)(_DAT_1802b1240 + 1) = (int)_DAT_1802b1240[1] + 1;
    UNLOCK();
  }
  if (lVar6 == 0) {
    if (plVar15 != (longlong *)0x0) {
      LOCK();
      plVar5 = plVar15 + 1;
      lVar6 = *plVar5;
      *(int *)plVar5 = (int)*plVar5 + -1;
      UNLOCK();
      if ((int)lVar6 == 1) {
        (**(code **)*plVar15)(plVar15);
        LOCK();
        piVar1 = (int *)((longlong)plVar15 + 0xc);
        iVar20 = *piVar1;
        *piVar1 = *piVar1 + -1;
        UNLOCK();
        if (iVar20 == 1) {
          (**(code **)(*plVar15 + 8))(plVar15);
        }
      }
    }
  }
  else {
    iVar11 = *(int *)(lVar6 + 0x40);
    if (plVar15 != (longlong *)0x0) {
      LOCK();
      plVar5 = plVar15 + 1;
      lVar6 = *plVar5;
      *(int *)plVar5 = (int)*plVar5 + -1;
      UNLOCK();
      if ((int)lVar6 == 1) {
        (**(code **)*plVar15)(plVar15);
        LOCK();
        piVar1 = (int *)((longlong)plVar15 + 0xc);
        iVar2 = *piVar1;
        *piVar1 = *piVar1 + -1;
        UNLOCK();
        if (iVar2 == 1) {
          (**(code **)(*plVar15 + 8))(plVar15);
        }
      }
    }
    if (iVar11 < 3) {
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
      uStack_e0 = func_0x000180006450(uVar23);
      puStack_d8 = (undefined8 *)CONCAT44(puStack_d8._4_4_,iVar20);
      lStack_e8 = CONCAT44(lStack_e8._4_4_,0x17d8);
      FUN_180015b40(&DAT_1802b1230,2,&UNK_180222ed0,&UNK_180222f50);
    }
  }
LAB_18006143d:
  FUN_1801e8810(uStack_38 ^ (ulonglong)auStack_108);
  return;
}

