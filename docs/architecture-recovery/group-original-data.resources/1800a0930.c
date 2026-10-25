
/* WARNING: Removing unreachable block (ram,0x0001800a0dbd) */
/* WARNING: Removing unreachable block (ram,0x0001800a0dc3) */
/* WARNING: Removing unreachable block (ram,0x0001800a0dd8) */
/* WARNING: Removing unreachable block (ram,0x0001800a1613) */
/* WARNING: Removing unreachable block (ram,0x0001800a1619) */
/* WARNING: Removing unreachable block (ram,0x0001800a1627) */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_1800a0930(longlong param_1)

{
  int *piVar1;
  int iVar2;
  byte bVar3;
  longlong *plVar4;
  bool bVar5;
  double dVar6;
  double dVar7;
  double dVar8;
  double dVar9;
  double dVar10;
  bool bVar11;
  longlong lVar12;
  double dVar13;
  char cVar14;
  int iVar15;
  uint uVar16;
  int iVar17;
  int iVar18;
  undefined8 uVar19;
  undefined **ppuVar20;
  undefined8 *puVar21;
  longlong *plVar22;
  undefined4 extraout_XMM0_Da;
  undefined4 extraout_XMM0_Da_00;
  undefined4 uVar23;
  undefined4 extraout_XMM0_Da_01;
  float fVar24;
  undefined1 auStack_368 [32];
  double dStack_348;
  double dStack_340;
  undefined **ppuStack_338;
  undefined8 *puStack_330;
  undefined4 uStack_328;
  undefined8 *puStack_320;
  int iStack_318;
  undefined8 uStack_310;
  double dStack_308;
  int iStack_300;
  char cStack_2f8;
  undefined4 uStack_2f4;
  int iStack_2f0;
  int iStack_2ec;
  undefined4 uStack_2e8;
  int iStack_2e4;
  undefined8 uStack_2e0;
  longlong *plStack_2d8;
  longlong *plStack_2d0;
  undefined8 uStack_2c8;
  longlong *plStack_2c0;
  longlong *plStack_2b8;
  undefined4 uStack_2b0;
  undefined8 uStack_2a8;
  longlong *plStack_2a0;
  longlong lStack_298;
  longlong lStack_290;
  longlong lStack_288;
  longlong lStack_280;
  longlong lStack_278;
  longlong lStack_270;
  longlong lStack_268;
  longlong lStack_260;
  longlong lStack_258;
  longlong lStack_250;
  longlong lStack_248;
  longlong lStack_240;
  longlong lStack_238;
  longlong lStack_230;
  longlong lStack_228;
  longlong lStack_220;
  longlong lStack_218;
  longlong lStack_210;
  longlong lStack_208;
  longlong lStack_200;
  longlong lStack_1f8;
  longlong lStack_1f0;
  longlong lStack_1e8;
  longlong lStack_1e0;
  longlong lStack_1d8;
  longlong lStack_1d0;
  longlong alStack_1c8 [2];
  undefined8 uStack_1b8;
  undefined8 uStack_1b0;
  undefined1 auStack_1a8 [8];
  longlong *plStack_1a0;
  undefined4 uStack_198;
  undefined1 uStack_194;
  undefined2 uStack_193;
  undefined1 uStack_191;
  undefined8 uStack_190;
  undefined *puStack_188;
  undefined8 uStack_180;
  undefined4 uStack_178;
  undefined4 uStack_174;
  undefined4 uStack_170;
  undefined4 uStack_16c;
  undefined8 uStack_168;
  undefined8 uStack_160;
  undefined8 uStack_158;
  undefined8 uStack_150;
  undefined8 uStack_148;
  undefined8 uStack_140;
  undefined8 uStack_138;
  undefined4 uStack_130;
  undefined8 auStack_128 [3];
  ulonglong uStack_110;
  undefined8 auStack_108 [3];
  ulonglong uStack_f0;
  undefined8 auStack_e8 [3];
  ulonglong uStack_d0;
  undefined8 auStack_c8 [3];
  ulonglong uStack_b0;
  ulonglong uStack_a8;
  
  uStack_1b0 = 0xfffffffffffffffe;
  uStack_a8 = DAT_1802a5230 ^ (ulonglong)auStack_368;
  lStack_260 = param_1 + 0x1b0;
  iVar15 = _Mtx_lock();
  if (iVar15 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar15);
  }
  cVar14 = FUN_18001dbc0(1);
  if (cVar14 != '\0') {
    ppuVar20 = (undefined **)(param_1 + 0xa8);
    if (0xf < *(ulonglong *)(param_1 + 0xc0)) {
      ppuVar20 = (undefined **)*ppuVar20;
    }
    bVar3 = *(byte *)(param_1 + 0x78);
    uVar19 = FUN_18001db50();
    puStack_330 = (undefined8 *)CONCAT44(puStack_330._4_4_,(uint)bVar3);
    dStack_340 = (double)CONCAT44(dStack_340._4_4_,*(undefined4 *)(param_1 + 0x2c));
    dStack_348 = (double)CONCAT44(dStack_348._4_4_,0x3a);
    ppuStack_338 = ppuVar20;
    FUN_180015b40(uVar19,1,&DAT_1802272d0,&DAT_180227288);
  }
  if (*(char *)(param_1 + 0x82) == '\0') {
    cVar14 = FUN_18001dbc0(1);
    if (cVar14 != '\0') {
      ppuVar20 = (undefined **)(param_1 + 0xa8);
      if (0xf < *(ulonglong *)(param_1 + 0xc0)) {
        ppuVar20 = (undefined **)*ppuVar20;
      }
      uVar19 = FUN_18001db50();
      dStack_340 = (double)CONCAT44(dStack_340._4_4_,*(undefined4 *)(param_1 + 0x2c));
      dStack_348 = (double)CONCAT44(dStack_348._4_4_,0x3d);
      ppuStack_338 = ppuVar20;
      FUN_180015b40(uVar19,1,&DAT_1802272a0,&DAT_180227288);
    }
    cVar14 = FUN_18001e5c0(param_1);
    uVar23 = 2;
    if (cVar14 != '\0') {
      uVar23 = 4;
    }
    FUN_18001e4e0(param_1,0);
    *(undefined1 *)(param_1 + 0x81) = 0;
    *(undefined4 *)(param_1 + 0x84) = uVar23;
    goto LAB_1800a20e9;
  }
  *(undefined1 *)(param_1 + 0x1b8) = 1;
  *(undefined4 *)(param_1 + 0x21c) = 0;
  *(undefined8 *)(param_1 + 0x220) = 1;
  *(undefined4 *)(param_1 + 0x22c) = 0;
  uStack_2f4 = 3;
  iVar15 = QueryPerformanceFrequency(&lStack_200);
  dVar13 = DAT_18023a7b0;
  uVar23 = extraout_XMM0_Da;
  dVar10 = 0.0;
  if ((iVar15 != 0) &&
     (iVar15 = QueryPerformanceCounter(&lStack_228), uVar23 = extraout_XMM0_Da_00, dVar10 = 0.0,
     iVar15 != 0)) {
    uVar23 = SUB84((double)lStack_200,0);
    dVar10 = ((double)lStack_228 / (double)lStack_200) * dVar13;
  }
  dVar8 = 0.0;
  dVar9 = 0.0;
  uStack_2c8 = 0;
  plStack_2d0 = (longlong *)FUN_1800a8b50(uVar23,0,0);
  iStack_2ec = 0;
  iVar15 = 0;
  cVar14 = FUN_18001e5c0(param_1);
  if (cVar14 == '\0') {
    uVar16 = *(uint *)(param_1 + 0x58) >> 1;
    if (((uVar16 & 1) != 0) && (*(char *)(param_1 + 0x78) != '\0')) goto LAB_1800a0c3d;
    iVar17 = iVar15;
    if ((*(uint *)(param_1 + 0x58) >> 2 & 1) == 0) {
      if ((uVar16 & 1) != 0) {
        cVar14 = FUN_18001dbc0();
        uVar23 = 1;
        dVar8 = 0.0;
        if (cVar14 != '\0') {
          uVar19 = FUN_18001db50();
          dStack_340 = (double)CONCAT44(dStack_340._4_4_,*(undefined4 *)(param_1 + 0x2c));
          dStack_348 = (double)CONCAT44(dStack_348._4_4_,0xe6);
          FUN_180015b40(uVar19,1,&DAT_180227630,&DAT_180227288);
          dVar8 = 0.0;
        }
        goto LAB_1800a1e65;
      }
      cVar14 = FUN_18001dbc0(4);
      iVar17 = 0;
      if (cVar14 != '\0') {
        uVar19 = FUN_18001db50();
        dStack_348 = (double)CONCAT44(dStack_348._4_4_,0xeb);
        FUN_180015b40(uVar19,4,&DAT_180227610,&DAT_180227288);
        iVar17 = 0;
      }
LAB_1800a1e5f:
      uVar23 = 3;
    }
    else {
      cVar14 = FUN_18001dbc0();
      uVar23 = 2;
      iVar17 = 0;
      dVar8 = 0.0;
      if (cVar14 != '\0') {
        uVar19 = FUN_18001db50();
        dStack_340 = (double)CONCAT44(dStack_340._4_4_,*(undefined4 *)(param_1 + 0x2c));
        dStack_348 = (double)CONCAT44(dStack_348._4_4_,0xe1);
        FUN_180015b40(uVar19,1,&DAT_1802275b0,&DAT_180227288);
        iVar17 = iVar15;
        dVar8 = 0.0;
      }
    }
LAB_1800a1e65:
    bVar11 = false;
  }
  else {
LAB_1800a0c3d:
    if (*(char *)(param_1 + 0x230) == '\0') {
      cVar14 = FUN_18001dbc0(1);
      if (cVar14 != '\0') {
        uVar19 = FUN_18001db50();
        dStack_340 = (double)CONCAT44(dStack_340._4_4_,*(undefined4 *)(param_1 + 0x2c));
        dStack_348 = (double)CONCAT44(dStack_348._4_4_,99);
        FUN_180015b40(uVar19,1,&DAT_180227310,&DAT_180227288);
      }
    }
    else {
      iVar17 = FUN_1800a53a0(param_1);
      if (iVar17 != 0) {
        cVar14 = FUN_18001dbc0(4);
        if (cVar14 != '\0') {
          ppuVar20 = (undefined **)FUN_18008f750(auStack_c8,iVar17);
          if ((undefined *)0xf < ppuVar20[3]) {
            ppuVar20 = (undefined **)*ppuVar20;
          }
          uVar19 = FUN_18001db50();
          dStack_340 = (double)CONCAT44(dStack_340._4_4_,*(undefined4 *)(param_1 + 0x2c));
          dStack_348 = (double)CONCAT44(dStack_348._4_4_,0x5d);
          ppuStack_338 = ppuVar20;
          FUN_180015b40(uVar19,4,&DAT_180227338,&DAT_180227288);
          if (0xf < uStack_b0) {
            __3_YAXPEAX_Z(auStack_c8[0]);
          }
        }
        goto LAB_1800a1e5f;
      }
    }
    cVar14 = FUN_18001dbc0(1);
    if (cVar14 != '\0') {
      uVar19 = FUN_18001db50();
      uStack_328 = *(undefined4 *)(param_1 + 0x228);
      puStack_330 = (undefined8 *)CONCAT44(puStack_330._4_4_,*(undefined4 *)(param_1 + 0x220));
      ppuStack_338 = (undefined **)CONCAT44(ppuStack_338._4_4_,*(undefined4 *)(param_1 + 0x21c));
      dStack_340 = (double)CONCAT44(dStack_340._4_4_,*(undefined4 *)(param_1 + 0x2c));
      dStack_348 = (double)CONCAT44(dStack_348._4_4_,0x66);
      FUN_180015b40(uVar19,1,&DAT_1802273a0,&DAT_180227288);
    }
    plVar22 = *(longlong **)(param_1 + 0x160);
    plStack_2c0 = *(longlong **)(param_1 + 0x158);
    if (plVar22 != (longlong *)0x0) {
      LOCK();
      *(int *)(plVar22 + 1) = (int)plVar22[1] + 1;
      UNLOCK();
      LOCK();
      *(int *)(plVar22 + 1) = (int)plVar22[1] + 1;
      UNLOCK();
    }
    uStack_1b8 = 0;
    uStack_2b0 = 1;
    plStack_2b8 = plVar22;
    if (plVar22 != (longlong *)0x0) {
      LOCK();
      plVar4 = plVar22 + 1;
      *(int *)plVar4 = (int)*plVar4 + -1;
      UNLOCK();
      if ((int)*plVar4 == 0) {
        (**(code **)*plVar22)(plVar22);
        LOCK();
        piVar1 = (int *)((longlong)plVar22 + 0xc);
        *piVar1 = *piVar1 + -1;
        UNLOCK();
        if (*piVar1 == 0) {
          (**(code **)(*plVar22 + 8))(plVar22);
        }
      }
    }
    if (*(int *)(param_1 + 0x21c) < *(int *)(param_1 + 0x220)) {
      iStack_2e4 = *(int *)(param_1 + 0x234);
    }
    else {
      iStack_2e4 = -*(int *)(param_1 + 0x234);
    }
    cVar14 = FUN_18001dbc0(1);
    if (cVar14 != '\0') {
      uVar19 = FUN_18001db50();
      dStack_340 = (double)CONCAT44(dStack_340._4_4_,*(undefined4 *)(param_1 + 0x2c));
      dStack_348 = (double)CONCAT44(dStack_348._4_4_,0x6d);
      FUN_180015b40(uVar19,1,&DAT_180227378,&DAT_180227288);
    }
    iVar17 = FUN_1800a4a00(param_1);
    if ((iVar17 != 0) && (cVar14 = FUN_18001dbc0(4), cVar14 != '\0')) {
      ppuVar20 = (undefined **)FUN_18008f750(auStack_128,iVar17);
      if ((undefined *)0xf < ppuVar20[3]) {
        ppuVar20 = (undefined **)*ppuVar20;
      }
      uVar19 = FUN_18001db50();
      dStack_340 = (double)CONCAT44(dStack_340._4_4_,*(undefined4 *)(param_1 + 0x2c));
      dStack_348 = (double)CONCAT44(dStack_348._4_4_,0x71);
      ppuStack_338 = ppuVar20;
      FUN_180015b40(uVar19,4,&DAT_180227418,&DAT_180227288);
      if (0xf < uStack_110) {
        __3_YAXPEAX_Z(auStack_128[0]);
      }
    }
    iVar15 = QueryPerformanceFrequency(&lStack_1e0);
    dVar8 = 0.0;
    if ((iVar15 != 0) && (iVar15 = QueryPerformanceCounter(&lStack_1f0), dVar8 = 0.0, iVar15 != 0))
    {
      dVar8 = ((double)lStack_1f0 / (double)lStack_1e0) * dVar13;
    }
    FUN_1800a74c0(param_1,param_1 + 0x180,&plStack_2d0);
    cVar14 = FUN_18001dbc0(1);
    if (cVar14 != '\0') {
      iVar15 = QueryPerformanceFrequency(&lStack_248);
      dVar6 = 0.0;
      if ((iVar15 != 0) && (iVar15 = QueryPerformanceCounter(&lStack_1d0), dVar6 = 0.0, iVar15 != 0)
         ) {
        dVar6 = ((double)lStack_1d0 / (double)lStack_248) * dVar13;
      }
      uVar19 = FUN_18001db50();
      dStack_348 = (double)CONCAT44(dStack_348._4_4_,0x7c);
      dStack_340 = dVar6 - dVar8;
      FUN_180015b40(uVar19,1,&DAT_1802273e0,&DAT_180227288);
    }
    bVar5 = false;
    cStack_2f8 = '\0';
    for (plVar22 = (longlong *)*plStack_2d0; plVar22 != plStack_2d0; plVar22 = (longlong *)*plVar22)
    {
      if (plVar22[0xf] != 0) {
        bVar5 = true;
        cStack_2f8 = '\x01';
        break;
      }
    }
    iVar15 = QueryPerformanceFrequency(&lStack_240);
    dVar8 = 0.0;
    if ((iVar15 != 0) && (iVar15 = QueryPerformanceCounter(&lStack_278), dVar8 = 0.0, iVar15 != 0))
    {
      dVar8 = ((double)lStack_278 / (double)lStack_240) * dVar13;
    }
    puStack_188 = &DAT_18021aea0;
    uStack_180 = 0;
    uStack_150 = 0;
    uStack_148 = 0;
    uStack_140 = 0;
    uStack_174 = 0;
    uStack_170 = 1;
    uStack_16c = 1;
    uStack_168 = 0;
    uStack_160 = 0;
    uStack_158 = 0;
    uStack_138 = 0;
    uStack_130 = 0;
    uStack_178 = 0;
    uStack_2e0 = 0;
    plStack_2d8 = (longlong *)0x0;
    if (bVar5) {
      uVar19 = FUN_18008e6c0(*(undefined8 *)(param_1 + 0xf8));
      puVar21 = (undefined8 *)FUN_1800cae00(uVar19,auStack_1a8,*(undefined4 *)(param_1 + 0x2c));
      uStack_2a8 = 0;
      plStack_2a0 = (longlong *)0x0;
      if (&uStack_2a8 != puVar21) {
        plStack_2a0 = (longlong *)puVar21[1];
        puVar21[1] = 0;
        uStack_2a8 = *puVar21;
        *puVar21 = 0;
      }
      uStack_2e0 = uStack_2a8;
      plStack_2d8 = plStack_2a0;
      if (plStack_1a0 != (longlong *)0x0) {
        LOCK();
        plVar22 = plStack_1a0 + 1;
        *(int *)plVar22 = (int)*plVar22 + -1;
        UNLOCK();
        if ((int)*plVar22 == 0) {
          (**(code **)*plStack_1a0)(plStack_1a0);
          LOCK();
          piVar1 = (int *)((longlong)plStack_1a0 + 0xc);
          *piVar1 = *piVar1 + -1;
          UNLOCK();
          if (*piVar1 == 0) {
            (**(code **)(*plStack_1a0 + 8))(plStack_1a0);
          }
        }
      }
      FUN_18008e6c0(*(undefined8 *)(param_1 + 0xf8));
      dStack_348 = (double)(param_1 + 0xa8);
      ppuStack_338 = &puStack_188;
      dStack_340 = (double)CONCAT44(dStack_340._4_4_,*(undefined4 *)(param_1 + 0xf0));
      iVar17 = FUN_1800cbb30(dStack_348,*(undefined4 *)(param_1 + 0x28),
                             *(undefined4 *)(param_1 + 0x2c),param_1 + 0x88);
      if ((iVar17 != 0) && (cVar14 = FUN_18001dbc0(3), cVar14 != '\0')) {
        if ((DAT_1802b1218 & 1) == 0) {
          DAT_1802b1218 = DAT_1802b1218 | 1;
          _DAT_1802b1230 = 1;
          DAT_1802b1238 = 0;
          DAT_1802b1240 = (longlong *)0x0;
          uRam00000001802b1248 = 0;
          DAT_1802b1250 = 0;
          uRam00000001802b1258 = 0;
          DAT_1802b1260 = 0;
          uRam00000001802b1268 = 0;
          _DAT_1802b1270 = 0;
          FUN_1801e8930(&DAT_180202850);
        }
        dStack_348 = (double)CONCAT44(dStack_348._4_4_,0x94);
        FUN_180015b40(&DAT_1802b1230,3,&DAT_180227480,&DAT_180227288);
      }
    }
    if ((DAT_1802b1218 & 1) == 0) {
      DAT_1802b1218 = DAT_1802b1218 | 1;
      _DAT_1802b1230 = 1;
      DAT_1802b1238 = 0;
      DAT_1802b1240 = (longlong *)0x0;
      uRam00000001802b1248 = 0;
      DAT_1802b1250 = 0;
      uRam00000001802b1258 = 0;
      DAT_1802b1260 = 0;
      uRam00000001802b1268 = 0;
      _DAT_1802b1270 = 0;
      FUN_1801e8930(&DAT_180202850);
    }
    plVar22 = DAT_1802b1240;
    lVar12 = DAT_1802b1238;
    if (DAT_1802b1240 != (longlong *)0x0) {
      LOCK();
      *(int *)(DAT_1802b1240 + 1) = (int)DAT_1802b1240[1] + 1;
      UNLOCK();
    }
    if (lVar12 == 0) {
      if (plVar22 != (longlong *)0x0) {
        LOCK();
        plVar4 = plVar22 + 1;
        lVar12 = *plVar4;
        *(int *)plVar4 = (int)*plVar4 + -1;
        UNLOCK();
        if ((int)lVar12 == 1) {
          (**(code **)*plVar22)(plVar22);
          LOCK();
          piVar1 = (int *)((longlong)plVar22 + 0xc);
          iVar15 = *piVar1;
          *piVar1 = *piVar1 + -1;
          UNLOCK();
          if (iVar15 == 1) {
            (**(code **)(*plVar22 + 8))(plVar22);
          }
        }
      }
    }
    else {
      iVar15 = *(int *)(lVar12 + 0x40);
      if (plVar22 != (longlong *)0x0) {
        LOCK();
        plVar4 = plVar22 + 1;
        lVar12 = *plVar4;
        *(int *)plVar4 = (int)*plVar4 + -1;
        UNLOCK();
        if ((int)lVar12 == 1) {
          (**(code **)*plVar22)(plVar22);
          LOCK();
          piVar1 = (int *)((longlong)plVar22 + 0xc);
          iVar18 = *piVar1;
          *piVar1 = *piVar1 + -1;
          UNLOCK();
          if (iVar18 == 1) {
            (**(code **)(*plVar22 + 8))(plVar22);
          }
        }
      }
      if (iVar15 < 2) {
        iVar15 = QueryPerformanceFrequency(&lStack_230);
        dVar6 = 0.0;
        if ((iVar15 != 0) &&
           (iVar15 = QueryPerformanceCounter(&lStack_268), dVar6 = 0.0, iVar15 != 0)) {
          dVar6 = ((double)lStack_268 / (double)lStack_230) * dVar13;
        }
        if ((DAT_1802b1218 & 1) == 0) {
          DAT_1802b1218 = DAT_1802b1218 | 1;
          _DAT_1802b1230 = 1;
          DAT_1802b1238 = 0;
          DAT_1802b1240 = (longlong *)0x0;
          uRam00000001802b1248 = 0;
          DAT_1802b1250 = 0;
          uRam00000001802b1258 = 0;
          DAT_1802b1260 = 0;
          uRam00000001802b1268 = 0;
          _DAT_1802b1270 = 0;
          FUN_1801e8930(&DAT_180202850);
        }
        dStack_340 = dVar6 - dVar8;
        dStack_348 = (double)CONCAT44(dStack_348._4_4_,0x97);
        FUN_180015b40(&DAT_1802b1230,1,&DAT_180227450,&DAT_180227288);
      }
    }
    bVar5 = false;
    uStack_2e8 = 0;
    iVar15 = *(int *)(param_1 + 0x21c);
    dVar8 = 0.0;
    iStack_2f0 = iVar15;
    if (iVar15 < *(int *)(param_1 + 0x220)) {
      do {
        iStack_2ec = iStack_2ec + 1;
        uStack_2e8 = 0;
        *(int *)(param_1 + 0x224) = iVar15;
        iStack_2f0 = iVar15;
        iVar18 = QueryPerformanceFrequency(&lStack_220);
        dVar8 = 0.0;
        if ((iVar18 != 0) &&
           (iVar18 = QueryPerformanceCounter(&lStack_258), dVar8 = 0.0, iVar18 != 0)) {
          dVar8 = ((double)lStack_258 / (double)lStack_220) * dVar13;
        }
        FUN_1800a7190(param_1,*(undefined4 *)(param_1 + 0x21c),*(undefined4 *)(param_1 + 0x220),
                      iVar15);
        cVar14 = FUN_1800a55b0(param_1);
        if (cVar14 != '\0') {
          if ((DAT_1802b1218 & 1) == 0) {
            DAT_1802b1218 = DAT_1802b1218 | 1;
            _DAT_1802b1230 = 1;
            DAT_1802b1238 = 0;
            DAT_1802b1240 = (longlong *)0x0;
            uRam00000001802b1248 = 0;
            DAT_1802b1250 = 0;
            uRam00000001802b1258 = 0;
            DAT_1802b1260 = 0;
            uRam00000001802b1268 = 0;
            _DAT_1802b1270 = 0;
            FUN_1801e8930(&DAT_180202850);
          }
          plVar22 = DAT_1802b1240;
          lVar12 = DAT_1802b1238;
          if (DAT_1802b1240 != (longlong *)0x0) {
            LOCK();
            *(int *)(DAT_1802b1240 + 1) = (int)DAT_1802b1240[1] + 1;
            UNLOCK();
          }
          if (lVar12 == 0) {
            if (plVar22 != (longlong *)0x0) {
              LOCK();
              plVar4 = plVar22 + 1;
              lVar12 = *plVar4;
              *(int *)plVar4 = (int)*plVar4 + -1;
              UNLOCK();
              if ((int)lVar12 == 1) {
                (**(code **)*plVar22)(plVar22);
                LOCK();
                piVar1 = (int *)((longlong)plVar22 + 0xc);
                iVar15 = *piVar1;
                *piVar1 = *piVar1 + -1;
                UNLOCK();
                if (iVar15 == 1) {
                  (**(code **)(*plVar22 + 8))(plVar22);
                }
              }
            }
          }
          else {
            iVar18 = *(int *)(lVar12 + 0x40);
            if (plVar22 != (longlong *)0x0) {
              LOCK();
              plVar4 = plVar22 + 1;
              lVar12 = *plVar4;
              *(int *)plVar4 = (int)*plVar4 + -1;
              UNLOCK();
              if ((int)lVar12 == 1) {
                (**(code **)*plVar22)(plVar22);
                LOCK();
                piVar1 = (int *)((longlong)plVar22 + 0xc);
                iVar2 = *piVar1;
                *piVar1 = *piVar1 + -1;
                UNLOCK();
                if (iVar2 == 1) {
                  (**(code **)(*plVar22 + 8))(plVar22);
                }
              }
            }
            if (iVar18 < 2) {
              if ((DAT_1802b1218 & 1) == 0) {
                DAT_1802b1218 = DAT_1802b1218 | 1;
                _DAT_1802b1230 = 1;
                DAT_1802b1238 = 0;
                DAT_1802b1240 = (longlong *)0x0;
                uRam00000001802b1248 = 0;
                DAT_1802b1250 = 0;
                uRam00000001802b1258 = 0;
                DAT_1802b1260 = 0;
                uRam00000001802b1268 = 0;
                _DAT_1802b1270 = 0;
                FUN_1801e8930(&DAT_180202850);
              }
              ppuStack_338 = (undefined **)CONCAT44(ppuStack_338._4_4_,iVar15);
              dStack_340 = (double)CONCAT44(dStack_340._4_4_,*(undefined4 *)(param_1 + 0x2c));
              dStack_348 = (double)CONCAT44(dStack_348._4_4_,0xa6);
              FUN_180015b40(&DAT_1802b1230,1,&DAT_1802274d8,&DAT_180227288);
            }
          }
          bVar5 = true;
          dVar8 = dVar9;
          goto LAB_1800a1d74;
        }
        if ((DAT_1802b1218 & 1) == 0) {
          DAT_1802b1218 = DAT_1802b1218 | 1;
          _DAT_1802b1230 = 1;
          DAT_1802b1238 = 0;
          DAT_1802b1240 = (longlong *)0x0;
          uRam00000001802b1248 = 0;
          DAT_1802b1250 = 0;
          uRam00000001802b1258 = 0;
          DAT_1802b1260 = 0;
          uRam00000001802b1268 = 0;
          _DAT_1802b1270 = 0;
          FUN_1801e8930(&DAT_180202850);
        }
        plVar22 = DAT_1802b1240;
        lVar12 = DAT_1802b1238;
        if (DAT_1802b1240 != (longlong *)0x0) {
          LOCK();
          *(int *)(DAT_1802b1240 + 1) = (int)DAT_1802b1240[1] + 1;
          UNLOCK();
        }
        if (lVar12 == 0) {
          if (plVar22 != (longlong *)0x0) {
            LOCK();
            plVar4 = plVar22 + 1;
            *(int *)plVar4 = (int)*plVar4 + -1;
            UNLOCK();
            if ((int)*plVar4 == 0) {
              (**(code **)*plVar22)(plVar22);
              LOCK();
              piVar1 = (int *)((longlong)plVar22 + 0xc);
              *piVar1 = *piVar1 + -1;
              UNLOCK();
              if (*piVar1 == 0) {
                (**(code **)(*plVar22 + 8))(plVar22);
              }
            }
          }
        }
        else {
          iVar17 = *(int *)(lVar12 + 0x40);
          if (plVar22 != (longlong *)0x0) {
            LOCK();
            plVar4 = plVar22 + 1;
            *(int *)plVar4 = (int)*plVar4 + -1;
            UNLOCK();
            if ((int)*plVar4 == 0) {
              (**(code **)*plVar22)(plVar22);
              LOCK();
              piVar1 = (int *)((longlong)plVar22 + 0xc);
              *piVar1 = *piVar1 + -1;
              UNLOCK();
              if (*piVar1 == 0) {
                (**(code **)(*plVar22 + 8))(plVar22);
              }
            }
          }
          if (iVar17 < 2) {
            iVar17 = QueryPerformanceFrequency(alStack_1c8);
            dVar6 = 0.0;
            if ((iVar17 != 0) &&
               (iVar17 = QueryPerformanceCounter(&lStack_210), dVar6 = 0.0, iVar17 != 0)) {
              dVar6 = ((double)lStack_210 / (double)alStack_1c8[0]) * dVar13;
            }
            if ((DAT_1802b1218 & 1) == 0) {
              DAT_1802b1218 = DAT_1802b1218 | 1;
              _DAT_1802b1230 = 1;
              DAT_1802b1238 = 0;
              DAT_1802b1240 = (longlong *)0x0;
              uRam00000001802b1248 = 0;
              DAT_1802b1250 = 0;
              uRam00000001802b1258 = 0;
              DAT_1802b1260 = 0;
              uRam00000001802b1268 = 0;
              _DAT_1802b1270 = 0;
              FUN_1801e8930(&DAT_180202850);
            }
            ppuStack_338 = (undefined **)(dVar6 - dVar8);
            dStack_340 = (double)CONCAT44(dStack_340._4_4_,iVar15);
            dStack_348 = (double)CONCAT44(dStack_348._4_4_,0xab);
            FUN_180015b40(&DAT_1802b1230,1,&DAT_1802274a0,&DAT_180227288);
          }
        }
        *(int *)(param_1 + 0x22c) = *(int *)(param_1 + 0x22c) + 1;
        uVar23 = *(undefined4 *)(param_1 + 0xec);
        uStack_2b0 = 2;
        lStack_290 = plStack_2c0[2];
        lStack_298 = plStack_2c0[1];
        if (lStack_290 != 0) {
          LOCK();
          *(int *)(lStack_290 + 8) = *(int *)(lStack_290 + 8) + 1;
          UNLOCK();
        }
        FUN_1800ffaa0(&plStack_2c0,&lStack_298,uVar23);
        (**(code **)(*plStack_2c0 + 8))();
        uStack_2b0 = 1;
        if ((DAT_1802b1218 & 1) == 0) {
          DAT_1802b1218 = DAT_1802b1218 | 1;
          _DAT_1802b1230 = 1;
          DAT_1802b1238 = 0;
          DAT_1802b1240 = (longlong *)0x0;
          uRam00000001802b1248 = 0;
          DAT_1802b1250 = 0;
          uRam00000001802b1258 = 0;
          DAT_1802b1260 = 0;
          uRam00000001802b1268 = 0;
          _DAT_1802b1270 = 0;
          FUN_1801e8930(&DAT_180202850);
        }
        plVar22 = DAT_1802b1240;
        lVar12 = DAT_1802b1238;
        if (DAT_1802b1240 != (longlong *)0x0) {
          LOCK();
          *(int *)(DAT_1802b1240 + 1) = (int)DAT_1802b1240[1] + 1;
          UNLOCK();
        }
        if (lVar12 == 0) {
          iVar15 = iStack_2f0;
          if (plVar22 != (longlong *)0x0) {
            LOCK();
            plVar4 = plVar22 + 1;
            *(int *)plVar4 = (int)*plVar4 + -1;
            UNLOCK();
            if ((int)*plVar4 == 0) {
              (**(code **)*plVar22)(plVar22);
              LOCK();
              piVar1 = (int *)((longlong)plVar22 + 0xc);
              *piVar1 = *piVar1 + -1;
              UNLOCK();
              iVar15 = iStack_2f0;
              if (*piVar1 == 0) {
                (**(code **)(*plVar22 + 8))(plVar22);
                iVar15 = iStack_2f0;
              }
            }
          }
        }
        else {
          iVar17 = *(int *)(lVar12 + 0x40);
          if (plVar22 != (longlong *)0x0) {
            LOCK();
            plVar4 = plVar22 + 1;
            *(int *)plVar4 = (int)*plVar4 + -1;
            UNLOCK();
            if ((int)*plVar4 == 0) {
              (**(code **)*plVar22)(plVar22);
              LOCK();
              piVar1 = (int *)((longlong)plVar22 + 0xc);
              *piVar1 = *piVar1 + -1;
              UNLOCK();
              if (*piVar1 == 0) {
                (**(code **)(*plVar22 + 8))(plVar22);
              }
            }
          }
          iVar15 = iStack_2f0;
          if (iVar17 < 2) {
            iVar15 = QueryPerformanceFrequency(&lStack_1f8);
            dVar6 = 0.0;
            if ((iVar15 != 0) &&
               (iVar15 = QueryPerformanceCounter(&lStack_238), dVar6 = 0.0, iVar15 != 0)) {
              dVar6 = ((double)lStack_238 / (double)lStack_1f8) * dVar13;
            }
            if ((DAT_1802b1218 & 1) == 0) {
              DAT_1802b1218 = DAT_1802b1218 | 1;
              _DAT_1802b1230 = 1;
              DAT_1802b1238 = 0;
              DAT_1802b1240 = (longlong *)0x0;
              uRam00000001802b1248 = 0;
              DAT_1802b1250 = 0;
              uRam00000001802b1258 = 0;
              DAT_1802b1260 = 0;
              uRam00000001802b1268 = 0;
              _DAT_1802b1270 = 0;
              FUN_1801e8930(&DAT_180202850);
            }
            iVar15 = iStack_2f0;
            ppuStack_338 = (undefined **)(dVar6 - dVar8);
            dStack_340 = (double)CONCAT44(dStack_340._4_4_,iStack_2f0);
            dStack_348 = (double)CONCAT44(dStack_348._4_4_,0xae);
            FUN_180015b40(&DAT_1802b1230,1,&DAT_180227578,&DAT_180227288);
          }
        }
        iVar17 = QueryPerformanceFrequency(&lStack_1d8);
        dVar6 = 0.0;
        if ((iVar17 != 0) &&
           (iVar17 = QueryPerformanceCounter(&lStack_288), dVar6 = 0.0, iVar17 != 0)) {
          dVar6 = ((double)lStack_288 / (double)lStack_1d8) * dVar13;
        }
        iVar17 = FUN_1800a7830(param_1,&plStack_2d0,&uStack_2e8);
        if (iVar17 != 0) {
          if ((DAT_1802b1218 & 1) == 0) {
            DAT_1802b1218 = DAT_1802b1218 | 1;
            _DAT_1802b1230 = 1;
            DAT_1802b1238 = 0;
            DAT_1802b1240 = (longlong *)0x0;
            uRam00000001802b1248 = 0;
            DAT_1802b1250 = 0;
            uRam00000001802b1258 = 0;
            DAT_1802b1260 = 0;
            uRam00000001802b1268 = 0;
            _DAT_1802b1270 = 0;
            FUN_1801e8930(&DAT_180202850);
          }
          plVar22 = DAT_1802b1240;
          lVar12 = DAT_1802b1238;
          if (DAT_1802b1240 != (longlong *)0x0) {
            LOCK();
            *(int *)(DAT_1802b1240 + 1) = (int)DAT_1802b1240[1] + 1;
            UNLOCK();
          }
          if (lVar12 == 0) {
            if (plVar22 != (longlong *)0x0) {
              LOCK();
              plVar4 = plVar22 + 1;
              *(int *)plVar4 = (int)*plVar4 + -1;
              UNLOCK();
              if ((int)*plVar4 == 0) {
                (**(code **)*plVar22)(plVar22);
                LOCK();
                piVar1 = (int *)((longlong)plVar22 + 0xc);
                *piVar1 = *piVar1 + -1;
                UNLOCK();
                if (*piVar1 == 0) {
                  (**(code **)(*plVar22 + 8))(plVar22);
                }
              }
            }
            bVar5 = false;
          }
          else {
            bVar5 = *(int *)(lVar12 + 0x40) < 5;
            if (plVar22 != (longlong *)0x0) {
              LOCK();
              plVar4 = plVar22 + 1;
              *(int *)plVar4 = (int)*plVar4 + -1;
              UNLOCK();
              if ((int)*plVar4 == 0) {
                (**(code **)*plVar22)(plVar22);
                LOCK();
                piVar1 = (int *)((longlong)plVar22 + 0xc);
                *piVar1 = *piVar1 + -1;
                UNLOCK();
                if (*piVar1 == 0) {
                  (**(code **)(*plVar22 + 8))(plVar22);
                }
              }
            }
          }
          if (bVar5) {
            puVar21 = (undefined8 *)FUN_18008f750(auStack_e8,iVar17);
            if (0xf < (ulonglong)puVar21[3]) {
              puVar21 = (undefined8 *)*puVar21;
            }
            if ((DAT_1802b1218 & 1) == 0) {
              DAT_1802b1218 = DAT_1802b1218 | 1;
              _DAT_1802b1230 = 1;
              DAT_1802b1238 = 0;
              DAT_1802b1240 = (longlong *)0x0;
              uRam00000001802b1248 = 0;
              DAT_1802b1250 = 0;
              uRam00000001802b1258 = 0;
              DAT_1802b1260 = 0;
              uRam00000001802b1268 = 0;
              _DAT_1802b1270 = 0;
              FUN_1801e8930(&DAT_180202850);
            }
            uStack_328 = uStack_2e8;
            ppuStack_338 = (undefined **)CONCAT44(ppuStack_338._4_4_,iVar15);
            dStack_340 = (double)CONCAT44(dStack_340._4_4_,*(undefined4 *)(param_1 + 0x2c));
            dStack_348 = (double)CONCAT44(dStack_348._4_4_,0xb9);
            puStack_330 = puVar21;
            FUN_180015b40(&DAT_1802b1230,4,&DAT_180227510,&DAT_180227288);
            if (0xf < uStack_d0) {
              __3_YAXPEAX_Z(auStack_e8[0]);
            }
          }
          uStack_2f4 = uStack_2e8;
        }
        iVar18 = QueryPerformanceFrequency(&lStack_1e8);
        dVar7 = 0.0;
        if ((iVar18 != 0) &&
           (iVar18 = QueryPerformanceCounter(&lStack_218), dVar7 = 0.0, iVar18 != 0)) {
          dVar7 = ((double)lStack_218 / (double)lStack_1e8) * dVar13;
        }
        dVar9 = dVar9 + (dVar7 - dVar6);
        if ((cStack_2f8 != '\0') && (iStack_2e4 + iVar15 < *(int *)(param_1 + 0x220))) {
          FUN_1800a8040(param_1,iVar15,&uStack_2e0,&puStack_188);
        }
        if (0 < *(int *)(param_1 + 0x228)) {
          FUN_1800da1c0(*(undefined8 *)(param_1 + 600),(longlong)*(int *)(param_1 + 0x228));
        }
        iVar18 = QueryPerformanceFrequency(&lStack_250);
        dVar6 = 0.0;
        if ((iVar18 != 0) &&
           (iVar18 = QueryPerformanceCounter(&lStack_208), dVar6 = 0.0, iVar18 != 0)) {
          dVar6 = ((double)lStack_208 / (double)lStack_250) * dVar13;
        }
        if ((DAT_1802b1218 & 1) == 0) {
          DAT_1802b1218 = DAT_1802b1218 | 1;
          _DAT_1802b1230 = 1;
          DAT_1802b1238 = 0;
          DAT_1802b1240 = (longlong *)0x0;
          uRam00000001802b1248 = 0;
          DAT_1802b1250 = 0;
          uRam00000001802b1258 = 0;
          DAT_1802b1260 = 0;
          uRam00000001802b1268 = 0;
          _DAT_1802b1270 = 0;
          FUN_1801e8930(&DAT_180202850);
        }
        plVar22 = DAT_1802b1240;
        lVar12 = DAT_1802b1238;
        if (DAT_1802b1240 != (longlong *)0x0) {
          LOCK();
          *(int *)(DAT_1802b1240 + 1) = (int)DAT_1802b1240[1] + 1;
          UNLOCK();
        }
        if (lVar12 == 0) {
          if (plVar22 != (longlong *)0x0) {
            LOCK();
            plVar4 = plVar22 + 1;
            lVar12 = *plVar4;
            *(int *)plVar4 = (int)*plVar4 + -1;
            UNLOCK();
            if ((int)lVar12 == 1) {
              (**(code **)*plVar22)(plVar22);
              LOCK();
              piVar1 = (int *)((longlong)plVar22 + 0xc);
              iVar18 = *piVar1;
              *piVar1 = *piVar1 + -1;
              UNLOCK();
              if (iVar18 == 1) {
                (**(code **)(*plVar22 + 8))(plVar22);
              }
            }
          }
        }
        else {
          iVar18 = *(int *)(lVar12 + 0x40);
          if (plVar22 != (longlong *)0x0) {
            LOCK();
            plVar4 = plVar22 + 1;
            lVar12 = *plVar4;
            *(int *)plVar4 = (int)*plVar4 + -1;
            UNLOCK();
            if ((int)lVar12 == 1) {
              (**(code **)*plVar22)(plVar22);
              LOCK();
              piVar1 = (int *)((longlong)plVar22 + 0xc);
              iVar2 = *piVar1;
              *piVar1 = *piVar1 + -1;
              UNLOCK();
              if (iVar2 == 1) {
                (**(code **)(*plVar22 + 8))(plVar22);
              }
            }
          }
          if (iVar18 < 2) {
            uVar19 = FUN_18001db50();
            dStack_340 = (double)CONCAT44(dStack_340._4_4_,iVar15);
            dStack_348 = (double)CONCAT44(dStack_348._4_4_,0xd1);
            ppuStack_338 = (undefined **)(dVar6 - dVar8);
            FUN_180015b40(uVar19,1,&DAT_1802275e8,&DAT_180227288);
          }
        }
        iVar15 = iVar15 + iStack_2e4;
      } while (iVar15 < *(int *)(param_1 + 0x220));
      bVar5 = false;
      dVar8 = dVar9;
      iStack_2f0 = iVar15;
    }
LAB_1800a1d74:
    plVar22 = plStack_2d8;
    if (iVar17 != 0) {
      if (plStack_2d8 != (longlong *)0x0) {
        LOCK();
        plVar4 = plStack_2d8 + 1;
        *(int *)plVar4 = (int)*plVar4 + -1;
        UNLOCK();
        if ((int)*plVar4 == 0) {
          (**(code **)*plStack_2d8)(plStack_2d8);
          LOCK();
          piVar1 = (int *)((longlong)plVar22 + 0xc);
          *piVar1 = *piVar1 + -1;
          UNLOCK();
          if (*piVar1 == 0) {
            (**(code **)(*plStack_2d8 + 8))();
          }
        }
      }
      FUN_18000b690(&puStack_188);
      plVar22 = plStack_2b8;
      uVar23 = uStack_2f4;
      if (plStack_2b8 != (longlong *)0x0) {
        LOCK();
        plVar4 = plStack_2b8 + 1;
        *(int *)plVar4 = (int)*plVar4 + -1;
        UNLOCK();
        if ((int)*plVar4 == 0) {
          (**(code **)*plStack_2b8)(plStack_2b8);
          LOCK();
          piVar1 = (int *)((longlong)plVar22 + 0xc);
          *piVar1 = *piVar1 + -1;
          UNLOCK();
          uVar23 = uStack_2f4;
          if (*piVar1 == 0) {
            (**(code **)(*plVar22 + 8))(plVar22);
            uVar23 = uStack_2f4;
          }
        }
      }
      goto LAB_1800a1e65;
    }
    bVar11 = true;
    uStack_2f4 = uStack_2e8;
    if (bVar5) {
      uStack_2f4 = 5;
    }
    if (plStack_2d8 != (longlong *)0x0) {
      LOCK();
      plVar4 = plStack_2d8 + 1;
      *(int *)plVar4 = (int)*plVar4 + -1;
      UNLOCK();
      if ((int)*plVar4 == 0) {
        (**(code **)*plStack_2d8)(plStack_2d8);
        LOCK();
        piVar1 = (int *)((longlong)plVar22 + 0xc);
        *piVar1 = *piVar1 + -1;
        UNLOCK();
        if (*piVar1 == 0) {
          (**(code **)(*plStack_2d8 + 8))();
        }
      }
    }
    FUN_18000b690(&puStack_188);
    plVar22 = plStack_2b8;
    uVar23 = uStack_2f4;
    if (plStack_2b8 != (longlong *)0x0) {
      LOCK();
      plVar4 = plStack_2b8 + 1;
      *(int *)plVar4 = (int)*plVar4 + -1;
      UNLOCK();
      if ((int)*plVar4 == 0) {
        (**(code **)*plStack_2b8)(plStack_2b8);
        LOCK();
        piVar1 = (int *)((longlong)plVar22 + 0xc);
        *piVar1 = *piVar1 + -1;
        UNLOCK();
        uVar23 = uStack_2f4;
        if (*piVar1 == 0) {
          (**(code **)(*plVar22 + 8))(plVar22);
          uVar23 = uStack_2f4;
        }
      }
    }
  }
  uStack_198 = *(undefined4 *)(param_1 + 0x2c);
  uStack_194 = 0;
  uStack_193 = 0;
  uStack_191 = 0;
  uStack_190 = *(undefined8 *)(param_1 + 0xf8);
  VM_M_SetInt(&uStack_198,&DAT_180227678,0,iVar17 == 0);
  FUN_18001e4e0(param_1,0);
  *(int *)(param_1 + 0x7c) = iVar17;
  *(undefined4 *)(param_1 + 0x84) = uVar23;
  *(undefined1 *)(param_1 + 0x1b8) = 0;
  iVar15 = QueryPerformanceFrequency(&lStack_270);
  dVar9 = 0.0;
  if ((iVar15 != 0) && (iVar15 = QueryPerformanceCounter(&lStack_280), dVar9 = 0.0, iVar15 != 0)) {
    dVar9 = ((double)lStack_280 / (double)lStack_270) * dVar13;
  }
  fVar24 = (float)(dVar9 - dVar10);
  VM_M_SetFloat(&uStack_198,&DAT_180227668,0,fVar24);
  if (bVar11) {
    if ((DAT_1802b2f70 & 1) == 0) {
      DAT_1802b2f70 = DAT_1802b2f70 | 1;
      FUN_180093820();
      FUN_1801e8930(FUN_180202e40);
    }
    FUN_180093b20(&DAT_1802b2f80,*(undefined4 *)(param_1 + 0x2c));
  }
  uVar19 = FUN_18008e6c0(*(undefined8 *)(param_1 + 0xf8));
  dStack_348 = dVar10;
  dStack_340 = dVar9;
  FUN_1800c9ea0(uVar19,*(undefined4 *)(param_1 + 0x2c),fVar24,fVar24);
  if ((DAT_1802b3080 & 1) == 0) {
    DAT_1802b3080 = DAT_1802b3080 | 1;
    FUN_180082fe0();
    FUN_1801e8930(&DAT_180202f00);
  }
  FUN_180083300(&DAT_1802b3088,*(undefined4 *)(param_1 + 0x2c),fVar24);
  cVar14 = FUN_18001dbc0(2);
  if (cVar14 != '\0') {
    puStack_320 = (undefined8 *)FUN_18001ead0(extraout_XMM0_Da_01,auStack_108,uVar23);
    if (0xf < (ulonglong)puStack_320[3]) {
      puStack_320 = (undefined8 *)*puStack_320;
    }
    ppuVar20 = (undefined **)(param_1 + 0xa8);
    if (0xf < *(ulonglong *)(param_1 + 0xc0)) {
      ppuVar20 = (undefined **)*ppuVar20;
    }
    uVar19 = FUN_18001db50();
    iStack_300 = iStack_2ec;
    uStack_310 = uStack_2c8;
    dStack_340 = (double)CONCAT44(dStack_340._4_4_,*(undefined4 *)(param_1 + 0x2c));
    dStack_348 = (double)CONCAT44(dStack_348._4_4_,0x10b);
    ppuStack_338 = ppuVar20;
    puStack_330 = (undefined8 *)(double)fVar24;
    uStack_328 = uVar23;
    iStack_318 = iVar17;
    dStack_308 = dVar8;
    FUN_180015b40(uVar19,2,&DAT_1802276a0,&DAT_180227288);
    if (0xf < uStack_f0) {
      __3_YAXPEAX_Z(auStack_108[0]);
    }
  }
  plVar22 = (longlong *)*plStack_2d0;
  *plStack_2d0 = (longlong)plStack_2d0;
  plStack_2d0[1] = (longlong)plStack_2d0;
  iStack_2ec = uVar23;
  if (plVar22 != plStack_2d0) {
    do {
      plVar4 = (longlong *)*plVar22;
      FUN_1800a77a0(plVar22 + 0x15);
      FUN_1800a03a0(plVar22 + 3);
      __3_YAXPEAX_Z(plVar22);
      plVar22 = plVar4;
    } while (plVar4 != plStack_2d0);
  }
  __3_YAXPEAX_Z(plStack_2d0);
LAB_1800a20e9:
  iVar15 = _Mtx_unlock(lStack_260);
  if (iVar15 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar15);
  }
  FUN_1801e8810(uStack_a8 ^ (ulonglong)auStack_368);
  return;
}

