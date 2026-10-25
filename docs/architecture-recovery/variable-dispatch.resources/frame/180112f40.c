
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_180112f40(longlong param_1,undefined4 param_2,int param_3,undefined8 *param_4,
                  undefined8 *param_5,undefined4 param_6,undefined8 *param_7)

{
  int *piVar1;
  longlong lVar2;
  char cVar3;
  int iVar4;
  int iVar5;
  undefined8 *puVar6;
  undefined8 *puVar7;
  undefined1 *puVar8;
  undefined8 uVar9;
  undefined8 **ppuVar10;
  ushort *****pppppuVar11;
  undefined8 *puVar12;
  undefined8 *puVar13;
  ushort *puVar14;
  undefined8 *puVar15;
  undefined8 *puVar16;
  longlong *plVar17;
  uint uVar18;
  ulonglong uVar19;
  longlong *plVar20;
  longlong *plVar21;
  longlong lVar22;
  undefined1 auStack_2a8 [32];
  undefined1 *puStack_288;
  undefined1 *puStack_280;
  int aiStack_278 [2];
  undefined1 *puStack_270;
  undefined4 auStack_268 [2];
  longlong lStack_260;
  longlong *plStack_258;
  undefined8 *puStack_250;
  undefined8 *puStack_248;
  undefined8 *puStack_240;
  undefined8 uStack_238;
  longlong *plStack_230;
  undefined8 *puStack_228;
  undefined8 *puStack_220;
  longlong lStack_218;
  longlong lStack_210;
  undefined8 *puStack_208;
  int *piStack_200;
  longlong lStack_1f8;
  undefined8 *puStack_1f0;
  undefined8 *puStack_1e8;
  longlong lStack_1e0;
  int *piStack_1d8;
  undefined8 *puStack_1d0;
  undefined8 *puStack_1c8;
  undefined8 *puStack_1c0;
  int *piStack_1b8;
  undefined1 auStack_1b0 [16];
  undefined8 uStack_1a0;
  undefined8 uStack_198;
  undefined1 auStack_190 [16];
  undefined8 uStack_180;
  undefined8 uStack_178;
  longlong *plStack_168;
  undefined1 auStack_160 [16];
  undefined8 uStack_150;
  undefined8 uStack_148;
  longlong *plStack_138;
  undefined1 auStack_130 [16];
  undefined8 uStack_120;
  undefined8 uStack_118;
  undefined1 auStack_110 [16];
  undefined8 uStack_100;
  undefined8 uStack_f8;
  undefined1 auStack_f0 [16];
  undefined8 uStack_e0;
  undefined8 uStack_d8;
  undefined1 auStack_d0 [16];
  undefined8 uStack_c0;
  undefined8 uStack_b8;
  longlong lStack_b0;
  undefined8 uStack_a8;
  undefined1 *puStack_a0;
  longlong lStack_98;
  longlong *plStack_90;
  undefined1 auStack_88 [16];
  undefined8 *puStack_78;
  undefined8 *puStack_70;
  undefined8 *puStack_68;
  ushort ****appppuStack_60 [2];
  ulonglong uStack_50;
  ulonglong uStack_48;
  ulonglong uStack_40;
  
  uStack_a8 = 0xfffffffffffffffe;
  uStack_40 = DAT_1802a5230 ^ (ulonglong)auStack_2a8;
  puStack_248 = param_5;
  puStack_70 = param_5;
  puStack_250 = param_7;
  puStack_68 = param_7;
  puVar7 = (undefined8 *)0x0;
  puStack_270 = (undefined1 *)((ulonglong)puStack_270 & 0xffffffff00000000);
  uStack_d8 = 0xf;
  uStack_e0 = 0;
  auStack_f0[0] = 0;
  aiStack_278[0] = param_3;
  auStack_268[0] = param_2;
  puStack_240 = param_4;
  lStack_218 = param_1;
  lStack_210 = param_1;
  puStack_78 = param_4;
  FUN_180006470(auStack_f0,param_4,0,0xffffffffffffffff);
  FUN_1800da760(appppuStack_60,auStack_f0,DAT_1802a56c4 == 1);
  pppppuVar11 = appppuStack_60;
  if (7 < uStack_48) {
    pppppuVar11 = (ushort *****)appppuStack_60[0];
  }
  uVar19 = 0xf;
  if (uStack_50 < 0xf) {
    uVar19 = uStack_50;
  }
  puVar16 = puVar7;
  if (uVar19 != 0) {
    puVar14 = &DAT_18022c628;
    do {
      if (*(ushort *)pppppuVar11 != *puVar14) {
        puVar16 = (undefined8 *)0x1;
        if (*(ushort *)pppppuVar11 < *puVar14) {
          puVar16 = (undefined8 *)0xffffffff;
        }
        break;
      }
      pppppuVar11 = (ushort *****)((longlong)pppppuVar11 + 2);
      puVar14 = puVar14 + 1;
      uVar19 = uVar19 - 1;
    } while (uVar19 != 0);
  }
  uVar18 = (uint)puVar16;
  if (uVar18 == 0) {
    if (uStack_50 < 0xf) {
      uVar18 = 0xffffffff;
    }
    else {
      uVar18 = (uint)(uStack_50 != 0xf);
    }
  }
  if (uVar18 == 0) {
    puVar6 = (undefined8 *)__2_YAPEAX_K_Z(0x160);
    puVar16 = puVar7;
    puStack_220 = puVar6;
    if (puVar6 != (undefined8 *)0x0) {
      FUN_18001dc50(puVar6);
      *puVar6 = &DAT_180229e50;
      puVar6[0x25] = 0;
      *(undefined1 *)(puVar6 + 0x26) = 0;
      iVar4 = _Mtx_init(puVar6 + 0x27,2);
      if (iVar4 != 0) {
        __Throw_C_error_std__YAXH_Z(iVar4);
      }
      puVar6[0x28] = 0;
      puVar6[0x2a] = 0;
      *puVar6 = &DAT_18021db30;
      puVar6[0x29] = &DAT_18021daf0;
      *(undefined4 *)(puVar6 + 0x2b) = 0xffffffff;
      *(undefined4 *)(puVar6 + 6) = 3;
      puVar16 = puVar6;
    }
    lStack_260 = 0;
    plStack_258 = (longlong *)0x0;
    FUN_18011c2b0(&lStack_260,puVar16);
    plVar20 = plStack_258;
    lVar22 = lStack_260;
    if (plStack_258 != (longlong *)0x0) {
      LOCK();
      *(int *)(plStack_258 + 1) = (int)plStack_258[1] + 1;
      UNLOCK();
    }
    plStack_90 = plStack_258;
    lStack_98 = lStack_260;
    lStack_1e0 = param_1 + 0x118;
    iVar4 = _Mtx_lock(lStack_1e0);
    if (iVar4 != 0) {
      __Throw_C_error_std__YAXH_Z(iVar4);
    }
    plVar21 = (longlong *)(param_1 + 0x40);
    puVar16 = (undefined8 *)*plVar21;
    cVar3 = *(char *)((longlong)puVar16[1] + 0x19);
    puVar6 = puVar16;
    puVar15 = (undefined8 *)puVar16[1];
    while (cVar3 == '\0') {
      if (*(int *)(puVar15 + 4) < aiStack_278[0]) {
        puVar12 = (undefined8 *)puVar15[2];
        puVar15 = puVar6;
      }
      else {
        puVar12 = (undefined8 *)*puVar15;
      }
      puVar6 = puVar15;
      puVar15 = puVar12;
      cVar3 = *(char *)((longlong)puVar12 + 0x19);
    }
    if ((puVar6 == puVar16) || (aiStack_278[0] < *(int *)(puVar6 + 4))) {
      piStack_1b8 = aiStack_278;
      puStack_288 = (undefined1 *)FUN_180085630(plVar21,aiStack_278[0],&piStack_1b8);
      FUN_180085670(plVar21,&puStack_1c8,puVar6,(longlong)puStack_288 + 0x20);
      puVar6 = puStack_1c8;
    }
    if (plVar20 != (longlong *)0x0) {
      LOCK();
      *(int *)(plVar20 + 1) = (int)plVar20[1] + 1;
      UNLOCK();
      plVar20 = plStack_258;
      lVar22 = lStack_260;
    }
    plVar21 = (longlong *)puVar6[6];
    puVar6[6] = plVar20;
    puVar6[5] = lVar22;
    plStack_138 = plVar21;
    if (plVar21 != (longlong *)0x0) {
      LOCK();
      plVar17 = plVar21 + 1;
      *(int *)plVar17 = (int)*plVar17 + -1;
      UNLOCK();
      plVar20 = plStack_258;
      lVar22 = lStack_260;
      if ((int)*plVar17 == 0) {
        (**(code **)*plVar21)(plVar21);
        LOCK();
        piVar1 = (int *)((longlong)plVar21 + 0xc);
        *piVar1 = *piVar1 + -1;
        UNLOCK();
        plVar20 = plStack_258;
        lVar22 = lStack_260;
        if (*piVar1 == 0) {
          (**(code **)(*plStack_138 + 8))();
          plVar20 = plStack_258;
          lVar22 = lStack_260;
        }
      }
    }
    iVar4 = _Mtx_unlock(lStack_1e0);
    if (iVar4 != 0) {
      __Throw_C_error_std__YAXH_Z(iVar4);
    }
    puStack_220 = (undefined8 *)auStack_130;
    puStack_a0 = auStack_1b0;
    uStack_118 = 0xf;
    uStack_120 = 0;
    auStack_130[0] = 0;
    FUN_180006470(auStack_130,puStack_250,0,0xffffffffffffffff);
    uStack_198 = 0xf;
    uStack_1a0 = 0;
    auStack_1b0[0] = 0;
    FUN_180006470(auStack_1b0,puStack_248,0,0xffffffffffffffff);
    uStack_f8 = 0xf;
    uStack_100 = 0;
    auStack_110[0] = 0;
    FUN_180006470(auStack_110,puStack_240,0,0xffffffffffffffff);
    puStack_280 = auStack_130;
    puStack_288 = auStack_1b0;
    FUN_1800b8620(lVar22,auStack_268[0],aiStack_278[0],auStack_110);
    plVar21 = plStack_258;
    if (lVar22 != 0) {
      puVar7 = (undefined8 *)(lVar22 + 0x148);
    }
    plVar17 = (longlong *)0x0;
    if (plVar20 != (longlong *)0x0) {
      LOCK();
      plVar20 = plVar20 + 1;
      *(int *)plVar20 = (int)*plVar20 + -1;
      UNLOCK();
      plVar17 = plStack_258;
      if ((int)*plVar20 == 0) {
        (**(code **)*plStack_258)(plStack_258);
        plVar17 = plStack_258;
        LOCK();
        piVar1 = (int *)((longlong)plVar21 + 0xc);
        *piVar1 = *piVar1 + -1;
        UNLOCK();
        if (*piVar1 == 0) {
          (**(code **)(*plStack_258 + 8))(plStack_258);
        }
      }
    }
    plVar20 = plStack_258;
    if (plVar17 == (longlong *)0x0) goto LAB_180113841;
    LOCK();
    plVar17 = plVar17 + 1;
    *(int *)plVar17 = (int)*plVar17 + -1;
    UNLOCK();
    if ((int)*plVar17 != 0) goto LAB_180113841;
    (**(code **)*plStack_258)(plStack_258);
    LOCK();
    piVar1 = (int *)((longlong)plVar20 + 0xc);
    *piVar1 = *piVar1 + -1;
    iVar4 = *piVar1;
    UNLOCK();
    plVar20 = plStack_258;
  }
  else {
    puVar6 = (undefined8 *)__2_YAPEAX_K_Z(0x148);
    puVar16 = puVar7;
    puStack_228 = puVar6;
    if (puVar6 != (undefined8 *)0x0) {
      FUN_18001dc50(puVar6);
      *puVar6 = &DAT_180229e50;
      puVar6[0x25] = 0;
      *(undefined1 *)(puVar6 + 0x26) = 0;
      iVar4 = _Mtx_init(puVar6 + 0x27,2);
      if (iVar4 != 0) {
        __Throw_C_error_std__YAXH_Z(iVar4);
      }
      puVar6[0x28] = 0;
      *(undefined4 *)(puVar6 + 6) = 3;
      puVar16 = puVar6;
    }
    uStack_238 = 0;
    plStack_230 = (longlong *)0x0;
    FUN_1800eb4d0(&uStack_238,puVar16);
    lStack_1f8 = param_1 + 0x118;
    iVar4 = _Mtx_lock(lStack_1f8);
    if (iVar4 != 0) {
      __Throw_C_error_std__YAXH_Z(iVar4);
    }
    plVar20 = (longlong *)(param_1 + 0x40);
    puVar16 = (undefined8 *)*plVar20;
    cVar3 = *(char *)((longlong)puVar16[1] + 0x19);
    puVar6 = puVar16;
    puVar15 = (undefined8 *)puVar16[1];
    while (cVar3 == '\0') {
      if (*(int *)(puVar15 + 4) < aiStack_278[0]) {
        puVar12 = (undefined8 *)puVar15[2];
        puVar15 = puVar6;
      }
      else {
        puVar12 = (undefined8 *)*puVar15;
      }
      puVar6 = puVar15;
      puVar15 = puVar12;
      cVar3 = *(char *)((longlong)puVar12 + 0x19);
    }
    if ((puVar6 == puVar16) || (aiStack_278[0] < *(int *)(puVar6 + 4))) {
      piStack_200 = aiStack_278;
      puStack_288 = (undefined1 *)FUN_180085630(plVar20,aiStack_278[0],&piStack_200);
      FUN_180085670(plVar20,&puStack_1d0,puVar6,(longlong)puStack_288 + 0x20);
      puVar6 = puStack_1d0;
    }
    if (plStack_230 != (longlong *)0x0) {
      LOCK();
      *(int *)(plStack_230 + 1) = (int)plStack_230[1] + 1;
      UNLOCK();
    }
    plVar20 = (longlong *)puVar6[6];
    puVar6[6] = plStack_230;
    puVar6[5] = uStack_238;
    plStack_168 = plVar20;
    if (plVar20 != (longlong *)0x0) {
      LOCK();
      plVar21 = plVar20 + 1;
      *(int *)plVar21 = (int)*plVar21 + -1;
      UNLOCK();
      if ((int)*plVar21 == 0) {
        (**(code **)*plVar20)(plVar20);
        LOCK();
        piVar1 = (int *)((longlong)plVar20 + 0xc);
        *piVar1 = *piVar1 + -1;
        UNLOCK();
        if (*piVar1 == 0) {
          (**(code **)(*plStack_168 + 8))();
        }
      }
    }
    plVar20 = plStack_230;
    iVar4 = _Mtx_unlock(lStack_1f8);
    if (iVar4 != 0) {
      __Throw_C_error_std__YAXH_Z(iVar4);
    }
    puStack_270 = auStack_190;
    puStack_220 = (undefined8 *)auStack_160;
    uStack_178 = 0xf;
    uStack_180 = 0;
    auStack_190[0] = 0;
    FUN_180006470(auStack_190,puStack_250,0,0xffffffffffffffff);
    uStack_148 = 0xf;
    uStack_150 = 0;
    auStack_160[0] = 0;
    FUN_180006470(auStack_160,puStack_248,0,0xffffffffffffffff);
    puVar16 = puStack_240;
    uStack_b8 = 0xf;
    uStack_c0 = 0;
    auStack_d0[0] = 0;
    FUN_180006470(auStack_d0,puStack_240,0,0xffffffffffffffff);
    puStack_280 = auStack_190;
    puStack_288 = auStack_160;
    FUN_1800b8620(uStack_238,auStack_268[0],aiStack_278[0],auStack_d0);
    if ((DAT_1802b2ac8 & 1) == 0) {
      DAT_1802b2ac8 = DAT_1802b2ac8 | 1;
      FUN_1800b5280();
      FUN_1801e8930(&DAT_1802029e0);
    }
    puStack_288 = (undefined1 *)CONCAT44(puStack_288._4_4_,param_6);
    iVar4 = FUN_1800b54e0(&DAT_1802b2ad0,appppuStack_60,puStack_248,aiStack_278[0]);
    puStack_270 = (undefined1 *)CONCAT44(puStack_270._4_4_,iVar4);
    if (iVar4 == 0) {
      if ((DAT_1802b2ac8 & 1) == 0) {
        DAT_1802b2ac8 = DAT_1802b2ac8 | 1;
        FUN_1800b5280();
        FUN_1801e8930(&DAT_1802029e0);
      }
      puVar7 = (undefined8 *)FUN_1800b6b70(&DAT_1802b2ad0,aiStack_278[0]);
      if (puVar7 == (undefined8 *)0x0) {
        cVar3 = FUN_18001dbc0(4);
        if (cVar3 != '\0') {
          puVar8 = (undefined1 *)FUN_180006450(puVar16);
          uVar9 = FUN_18001db50();
          puStack_288 = (undefined1 *)CONCAT44(puStack_288._4_4_,0x844);
          puStack_280 = puVar8;
          FUN_180015b40(uVar9,4,&DAT_180231138,&DAT_1802310f8);
        }
        puStack_270 = (undefined1 *)CONCAT44(puStack_270._4_4_,0xe0000318);
      }
    }
    else {
      cVar3 = FUN_18001dbc0(4);
      if (cVar3 != '\0') {
        if ((DAT_1802b1218 & 1) == 0) {
          DAT_1802b1218 = DAT_1802b1218 | 1;
          _DAT_1802b1230 = 1;
          DAT_1802b1238 = 0;
          DAT_1802b1240 = 0;
          uRam00000001802b1248 = 0;
          DAT_1802b1250 = 0;
          uRam00000001802b1258 = 0;
          DAT_1802b1260 = 0;
          uRam00000001802b1268 = 0;
          _DAT_1802b1270 = 0;
          FUN_1801e8930(&DAT_180202850);
        }
        puStack_280 = (undefined1 *)FUN_180006450(puVar16);
        puStack_288 = (undefined1 *)CONCAT44(puStack_288._4_4_,0x83b);
        FUN_180015b40(&DAT_1802b1230,4,&DAT_1802310e0,&DAT_1802310f8);
        puVar7 = (undefined8 *)0x0;
      }
    }
    plVar21 = plStack_230;
    if (plVar20 == (longlong *)0x0) goto LAB_180113841;
    LOCK();
    plVar20 = plVar20 + 1;
    *(int *)plVar20 = (int)*plVar20 + -1;
    UNLOCK();
    if ((int)*plVar20 != 0) goto LAB_180113841;
    (**(code **)*plStack_230)(plStack_230);
    LOCK();
    piVar1 = (int *)((longlong)plVar21 + 0xc);
    *piVar1 = *piVar1 + -1;
    iVar4 = *piVar1;
    UNLOCK();
    plVar20 = plStack_230;
  }
  if (iVar4 == 0) {
    (**(code **)(*plVar20 + 8))();
  }
LAB_180113841:
  iVar4 = _Mtx_lock(param_1 + 0x118);
  if (iVar4 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar4);
  }
  puVar16 = *(undefined8 **)(param_1 + 0x40);
  cVar3 = *(char *)((longlong)puVar16[1] + 0x19);
  puVar6 = puVar16;
  puVar15 = (undefined8 *)puVar16[1];
  while (cVar3 == '\0') {
    if (*(int *)(puVar15 + 4) < aiStack_278[0]) {
      puVar12 = (undefined8 *)puVar15[2];
      puVar15 = puVar6;
    }
    else {
      puVar12 = (undefined8 *)*puVar15;
    }
    puVar6 = puVar15;
    puVar15 = puVar12;
    cVar3 = *(char *)((longlong)puVar12 + 0x19);
  }
  if ((puVar6 == puVar16) || (aiStack_278[0] < *(int *)(puVar6 + 4))) {
    puStack_1f0 = puVar16;
    ppuVar10 = &puStack_1f0;
  }
  else {
    puStack_1c0 = puVar6;
    ppuVar10 = &puStack_1c0;
  }
  if (*ppuVar10 != *(undefined8 **)(param_1 + 0x40)) {
    *(undefined8 **)((*ppuVar10)[5] + 0x128) = puVar7;
  }
  iVar4 = _Mtx_unlock(param_1 + 0x118);
  if (iVar4 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar4);
  }
  if (7 < uStack_48) {
    __3_YAXPEAX_Z(appppuStack_60[0]);
  }
  lVar2 = lStack_218;
  puVar16 = puStack_240;
  puVar7 = puStack_248;
  iVar4 = (int)puStack_270;
  lVar22 = lStack_218 + 0x118;
  lStack_b0 = lVar22;
  iVar5 = _Mtx_lock(lVar22);
  if (iVar5 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar5);
  }
  uVar9 = FUN_18009e260(lVar2 + 0x10,auStack_268);
  puStack_288 = (undefined1 *)CONCAT71(puStack_288._1_7_,DAT_1802b1205);
  FUN_180071a90(uVar9,auStack_88,0,aiStack_278);
  puVar6 = *(undefined8 **)(lVar2 + 0x40);
  cVar3 = *(char *)((longlong)puVar6[1] + 0x19);
  puVar15 = puVar6;
  puVar12 = (undefined8 *)puVar6[1];
  while (cVar3 == '\0') {
    if (*(int *)(puVar12 + 4) < aiStack_278[0]) {
      puVar13 = (undefined8 *)puVar12[2];
      puVar12 = puVar15;
    }
    else {
      puVar13 = (undefined8 *)*puVar12;
    }
    puVar15 = puVar12;
    puVar12 = puVar13;
    cVar3 = *(char *)((longlong)puVar13 + 0x19);
  }
  if ((puVar15 == puVar6) || (aiStack_278[0] < *(int *)(puVar15 + 4))) {
    puStack_208 = puVar6;
    ppuVar10 = &puStack_208;
  }
  else {
    puStack_1e8 = puVar15;
    ppuVar10 = &puStack_1e8;
  }
  if (*ppuVar10 != *(undefined8 **)(lVar2 + 0x40)) {
    puVar6 = *(undefined8 **)(lVar2 + 0x40);
    cVar3 = *(char *)((longlong)puVar6[1] + 0x19);
    puVar15 = puVar6;
    puVar12 = (undefined8 *)puVar6[1];
    while (cVar3 == '\0') {
      if (*(int *)(puVar12 + 4) < aiStack_278[0]) {
        puVar13 = (undefined8 *)puVar12[2];
        puVar12 = puVar15;
      }
      else {
        puVar13 = (undefined8 *)*puVar12;
      }
      puVar15 = puVar12;
      puVar12 = puVar13;
      cVar3 = *(char *)((longlong)puVar13 + 0x19);
    }
    if ((puVar15 == puVar6) || (aiStack_278[0] < *(int *)(puVar15 + 4))) {
      piStack_1d8 = aiStack_278;
      puStack_288 = (undefined1 *)FUN_180085630(lVar2 + 0x40,aiStack_278[0],&piStack_1d8);
      FUN_180085670(lVar2 + 0x40,&puStack_228,puVar15,(longlong)puStack_288 + 0x20);
      puVar15 = puStack_228;
    }
    lVar2 = puVar15[5];
    *(bool *)(lVar2 + 0x83) = iVar4 == 0;
    if (iVar4 != 0) {
      *(undefined1 *)(lVar2 + 0x82) = 0;
    }
  }
  iVar4 = _Mtx_unlock(lVar22);
  if (iVar4 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar4);
  }
  if (0xf < (ulonglong)puVar16[3]) {
    __3_YAXPEAX_Z(*puVar16);
  }
  puVar16[3] = 0xf;
  puVar16[2] = 0;
  *(undefined1 *)puVar16 = 0;
  if (0xf < (ulonglong)puVar7[3]) {
    __3_YAXPEAX_Z(*puVar7);
  }
  puVar16 = puStack_250;
  puVar7[3] = 0xf;
  puVar7[2] = 0;
  *(undefined1 *)puVar7 = 0;
  if (0xf < (ulonglong)puStack_250[3]) {
    __3_YAXPEAX_Z(*puStack_250);
  }
  puVar16[3] = 0xf;
  puVar16[2] = 0;
  *(undefined1 *)puVar16 = 0;
  FUN_1801e8810(uStack_40 ^ (ulonglong)auStack_2a8);
  return;
}

