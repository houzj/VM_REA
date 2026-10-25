
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_1800b54e0(undefined8 *param_1,undefined8 *param_2,undefined8 *param_3,undefined4 param_4,
                  undefined4 param_5)

{
  longlong *plVar1;
  int *piVar2;
  char cVar3;
  longlong *plVar4;
  double dVar5;
  char cVar6;
  int iVar7;
  undefined8 uVar8;
  longlong *******ppppppplVar9;
  longlong *******ppppppplVar10;
  undefined8 **ppuVar11;
  undefined8 *puVar12;
  undefined8 *puVar13;
  undefined8 *puVar14;
  char *pcVar15;
  undefined8 *puVar16;
  undefined8 *puVar17;
  undefined8 *puVar18;
  int iVar19;
  longlong lVar20;
  undefined8 *puVar21;
  undefined4 uVar22;
  undefined4 uVar23;
  undefined1 auStack_3b8 [32];
  undefined8 **ppuStack_398;
  longlong ******pppppplStack_390;
  longlong ******pppppplStack_388;
  undefined8 *puStack_380;
  double dStack_378;
  undefined8 *puStack_368;
  undefined4 auStack_360 [2];
  undefined8 *puStack_358;
  undefined8 *puStack_350;
  undefined8 *puStack_348;
  undefined8 *puStack_340;
  longlong *plStack_338;
  longlong ******pppppplStack_330;
  undefined8 **ppuStack_328;
  longlong ******pppppplStack_320;
  undefined8 **ppuStack_318;
  undefined8 *puStack_310;
  double dStack_308;
  double dStack_300;
  double dStack_2f8;
  double dStack_2f0;
  longlong lStack_2e8;
  undefined8 *puStack_2e0;
  undefined8 *puStack_2d8;
  longlong lStack_2d0;
  longlong lStack_2c8;
  longlong lStack_2c0;
  longlong lStack_2b8;
  longlong lStack_2b0;
  undefined8 *puStack_2a8;
  longlong *plStack_2a0;
  undefined8 *puStack_298;
  longlong *plStack_290;
  longlong *plStack_280;
  undefined2 auStack_278 [8];
  undefined8 uStack_268;
  undefined8 uStack_260;
  undefined2 auStack_258 [8];
  undefined8 uStack_248;
  undefined8 uStack_240;
  undefined2 auStack_238 [8];
  undefined8 uStack_228;
  undefined8 uStack_220;
  undefined2 auStack_218 [8];
  undefined8 uStack_208;
  undefined8 uStack_200;
  undefined2 auStack_1f8 [8];
  undefined8 uStack_1e8;
  undefined8 uStack_1e0;
  undefined8 uStack_1d8;
  undefined8 *puStack_1d0;
  undefined8 *puStack_1c8;
  undefined1 auStack_1c0 [16];
  longlong ******apppppplStack_1b0 [3];
  ulonglong uStack_198;
  undefined8 auStack_190 [2];
  longlong lStack_180;
  ulonglong uStack_178;
  undefined2 uStack_170;
  undefined6 uStack_16e;
  undefined8 uStack_160;
  ulonglong uStack_158;
  undefined1 uStack_150;
  undefined7 uStack_14f;
  undefined8 uStack_140;
  ulonglong uStack_138;
  undefined2 uStack_130;
  undefined6 uStack_12e;
  undefined8 uStack_120;
  ulonglong uStack_118;
  undefined1 uStack_110;
  undefined7 uStack_10f;
  undefined8 uStack_100;
  ulonglong uStack_f8;
  undefined8 auStack_f0 [3];
  ulonglong uStack_d8;
  undefined8 auStack_d0 [3];
  ulonglong uStack_b8;
  undefined8 auStack_b0 [3];
  ulonglong uStack_98;
  undefined8 auStack_90 [3];
  ulonglong uStack_78;
  ulonglong uStack_70;
  
  uStack_1d8 = 0xfffffffffffffffe;
  uStack_70 = DAT_1802a5230 ^ (ulonglong)auStack_3b8;
  uStack_200 = 7;
  iVar19 = 0;
  uStack_208 = 0;
  auStack_218[0] = 0;
  puStack_368 = param_1;
  auStack_360[0] = param_4;
  puStack_348 = param_1;
  FUN_18003ef70(auStack_218,param_2,0,0xffffffffffffffff);
  FUN_1800da820(apppppplStack_1b0,auStack_218,1);
  cVar6 = FUN_18001dbc0(2);
  if (cVar6 != '\0') {
    if (0xf < (ulonglong)param_3[3]) {
      param_3 = (undefined8 *)*param_3;
    }
    ppppppplVar9 = apppppplStack_1b0;
    if (0xf < uStack_198) {
      ppppppplVar9 = (longlong *******)apppppplStack_1b0[0];
    }
    uVar8 = FUN_18001db50();
    pppppplStack_390 = (longlong ******)CONCAT44(pppppplStack_390._4_4_,auStack_360[0]);
    ppuStack_398 = (undefined8 **)CONCAT44(ppuStack_398._4_4_,0x1e);
    pppppplStack_388 = (longlong ******)ppppppplVar9;
    puStack_380 = param_3;
    FUN_180015b40(uVar8,2,&DAT_1802292a0,&DAT_1802292c8);
    param_1 = puStack_368;
  }
  puStack_358 = (undefined8 *)0x0;
  puStack_340 = (undefined8 *)0x0;
  plStack_338 = (longlong *)0x0;
  ppuStack_318 = (undefined8 **)0x0;
  pppppplStack_330 = (longlong ******)0x0;
  dStack_2f0 = 0.0;
  dStack_308 = 0.0;
  puVar21 = (undefined8 *)0x0;
  puStack_350 = (undefined8 *)0x0;
  if ((DAT_1802b2fd8 & 1) == 0) {
    DAT_1802b2fd8 = DAT_1802b2fd8 | 1;
    FUN_1800c2a70();
    FUN_1801e8930(&DAT_180202ef0);
  }
  FUN_1800c3cc0(&DAT_1802b2fe0,auStack_190,param_2);
  if (lStack_180 == 0) {
    iVar19 = -0x1ffffdfd;
    cVar6 = FUN_18001dbc0(4);
    if (cVar6 != '\0') {
      ppppppplVar9 = apppppplStack_1b0;
      if (0xf < uStack_198) {
        ppppppplVar9 = (longlong *******)apppppplStack_1b0[0];
      }
      uVar8 = FUN_18001db50();
      ppuStack_398 = (undefined8 **)CONCAT44(ppuStack_398._4_4_,0x35);
      pppppplStack_390 = (longlong ******)ppppppplVar9;
      FUN_180015b40(uVar8,4,&DAT_180229300,&DAT_1802292c8);
    }
    if (7 < uStack_178) {
      __3_YAXPEAX_Z(auStack_190[0]);
    }
  }
  else {
    param_1 = param_1 + 7;
    puStack_1c8 = param_1;
    iVar7 = _Mtx_lock(param_1);
    if (iVar7 != 0) {
      __Throw_C_error_std__YAXH_Z(iVar7);
    }
    dStack_300 = 0.0;
    iVar7 = QueryPerformanceFrequency(&lStack_2b0);
    if ((iVar7 == 0) || (iVar7 = QueryPerformanceCounter(&lStack_2d0), iVar7 == 0)) {
      uVar22 = SUB84(DAT_18023a7b0,0);
      uVar23 = (undefined4)((ulonglong)DAT_18023a7b0 >> 0x20);
    }
    else {
      uVar22 = SUB84(DAT_18023a7b0,0);
      uVar23 = (undefined4)((ulonglong)DAT_18023a7b0 >> 0x20);
      dStack_300 = ((double)lStack_2d0 / (double)lStack_2b0) * DAT_18023a7b0;
    }
    ppppppplVar9 = apppppplStack_1b0;
    if (0xf < uStack_198) {
      ppppppplVar9 = (longlong *******)apppppplStack_1b0[0];
    }
    puVar13 = *(undefined8 **)puStack_368[5];
    while (puVar13 != (undefined8 *)puStack_368[5]) {
      pcVar15 = (char *)(puVar13 + 4);
      if (0xf < (ulonglong)puVar13[7]) {
        pcVar15 = *(char **)pcVar15;
      }
      lVar20 = (longlong)ppppppplVar9 - (longlong)pcVar15;
      do {
        cVar6 = *pcVar15;
        cVar3 = pcVar15[lVar20];
        if (cVar6 != cVar3) break;
        pcVar15 = pcVar15 + 1;
      } while (cVar3 != '\0');
      if (cVar6 == cVar3) {
        iVar19 = -0x1ffffff8;
        goto LAB_1800b5bfc;
      }
      if (*(char *)((longlong)puVar13 + 0x19) == '\0') {
        puVar12 = (undefined8 *)puVar13[2];
        if (*(char *)((longlong)puVar12 + 0x19) == '\0') {
          cVar6 = *(char *)((longlong)*puVar12 + 0x19);
          puVar13 = puVar12;
          puVar12 = (undefined8 *)*puVar12;
          while (cVar6 == '\0') {
            cVar6 = *(char *)((longlong)*puVar12 + 0x19);
            puVar13 = puVar12;
            puVar12 = (undefined8 *)*puVar12;
          }
        }
        else {
          cVar6 = *(char *)((longlong)puVar13[1] + 0x19);
          puVar18 = (undefined8 *)puVar13[1];
          puVar12 = puVar13;
          while ((puVar13 = puVar18, cVar6 == '\0' && (puVar12 == (undefined8 *)puVar13[2]))) {
            cVar6 = *(char *)((longlong)puVar13[1] + 0x19);
            puVar18 = (undefined8 *)puVar13[1];
            puVar12 = puVar13;
          }
        }
      }
    }
    ppppppplVar9 = apppppplStack_1b0;
    if (0xf < uStack_198) {
      ppppppplVar9 = (longlong *******)apppppplStack_1b0[0];
    }
    puVar21 = (undefined8 *)FUN_1800da730(&uStack_130,ppppppplVar9,DAT_1802a56c4 == 1);
    if (7 < (ulonglong)puVar21[3]) {
      puVar21 = (undefined8 *)*puVar21;
    }
    puVar21 = (undefined8 *)GetModuleHandleW(puVar21);
    if (7 < uStack_118) {
      __3_YAXPEAX_Z(CONCAT62(uStack_12e,uStack_130));
    }
    uStack_118 = 7;
    uStack_120 = 0;
    uStack_130 = 0;
    puStack_350 = puVar21;
    if (puVar21 == (undefined8 *)0x0) {
      cVar6 = FUN_18001dbc0(1);
      if (cVar6 != '\0') {
        ppppppplVar9 = apppppplStack_1b0;
        if (0xf < uStack_198) {
          ppppppplVar9 = (longlong *******)apppppplStack_1b0[0];
        }
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
        ppuStack_398 = (undefined8 **)CONCAT44(ppuStack_398._4_4_,0x43);
        pppppplStack_390 = (longlong ******)ppppppplVar9;
        FUN_180015b40(&DAT_1802b1230,1,&DAT_1802292e0,&DAT_1802292c8);
      }
      uStack_240 = 7;
      uStack_248 = 0;
      auStack_258[0] = 0;
      FUN_18003ef70(auStack_258,auStack_190,0,0xffffffffffffffff);
      puVar21 = (undefined8 *)FUN_1800da820(&uStack_110,auStack_258,1);
      if (0xf < (ulonglong)puVar21[3]) {
        puVar21 = (undefined8 *)*puVar21;
      }
      puVar21 = (undefined8 *)FUN_1800da730(&uStack_170,puVar21,DAT_1802a56c4 == 1);
      if (7 < (ulonglong)puVar21[3]) {
        puVar21 = (undefined8 *)*puVar21;
      }
      puVar21 = (undefined8 *)LoadLibraryW(puVar21);
      if (7 < uStack_158) {
        __3_YAXPEAX_Z(CONCAT62(uStack_16e,uStack_170));
      }
      uStack_158 = 7;
      uStack_160 = 0;
      uStack_170 = 0;
      puStack_350 = puVar21;
      if (0xf < uStack_f8) {
        __3_YAXPEAX_Z(CONCAT71(uStack_10f,uStack_110));
      }
      uStack_f8 = 0xf;
      uStack_100 = 0;
      uStack_110 = 0;
      if (puVar21 == (undefined8 *)0x0) {
        cVar6 = FUN_18001dbc0(4);
        if (cVar6 != '\0') {
          ppppppplVar9 = (longlong *******)FUN_1800f3d40(auStack_f0);
          uStack_220 = 7;
          uStack_228 = 0;
          auStack_238[0] = 0;
          FUN_18003ef70(auStack_238,auStack_190,0,0xffffffffffffffff);
          ppppppplVar10 = (longlong *******)FUN_1800da820(&uStack_150,auStack_238,1);
          if ((longlong ******)0xf < ppppppplVar9[3]) {
            ppppppplVar9 = (longlong *******)*ppppppplVar9;
          }
          if ((longlong ******)0xf < ppppppplVar10[3]) {
            ppppppplVar10 = (longlong *******)*ppppppplVar10;
          }
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
          ppuStack_398 = (undefined8 **)CONCAT44(ppuStack_398._4_4_,0x4a);
          pppppplStack_390 = (longlong ******)ppppppplVar10;
          pppppplStack_388 = (longlong ******)ppppppplVar9;
          FUN_180015b40(&DAT_1802b1230,4,&DAT_180229350,&DAT_1802292c8);
          if (0xf < uStack_138) {
            __3_YAXPEAX_Z(CONCAT71(uStack_14f,uStack_150));
          }
          uStack_138 = 0xf;
          uStack_140 = 0;
          uStack_150 = 0;
          if (0xf < uStack_d8) {
            __3_YAXPEAX_Z(auStack_f0[0]);
          }
        }
        iVar19 = -0x1ffffff0;
      }
    }
LAB_1800b5bfc:
    puVar13 = (undefined8 *)0x0;
    dStack_2f8 = 0.0;
    iVar7 = QueryPerformanceFrequency(&lStack_2c8);
    if ((iVar7 != 0) && (iVar7 = QueryPerformanceCounter(&lStack_2c0), iVar7 != 0)) {
      dStack_2f8 = ((double)lStack_2c0 / (double)lStack_2c8) * (double)CONCAT44(uVar23,uVar22);
    }
    iVar7 = _Mtx_unlock(param_1);
    if (iVar7 != 0) {
      __Throw_C_error_std__YAXH_Z(iVar7);
    }
    if (iVar19 == 0) {
      ppuVar11 = (undefined8 **)GetProcAddress(puVar21,&DAT_180229340);
      ppuStack_328 = ppuVar11;
      ppuStack_318 = ppuVar11;
      if (ppuVar11 == (undefined8 **)0x0) {
        cVar6 = FUN_18001dbc0(4);
        if (cVar6 != '\0') {
          uStack_1e0 = 7;
          uStack_1e8 = 0;
          auStack_1f8[0] = 0;
          FUN_18003ef70(auStack_1f8,param_2,0,0xffffffffffffffff);
          ppppppplVar9 = (longlong *******)FUN_1800da820(auStack_b0,auStack_1f8,1);
          if ((longlong ******)0xf < ppppppplVar9[3]) {
            ppppppplVar9 = (longlong *******)*ppppppplVar9;
          }
          uVar8 = FUN_18001db50();
          ppuStack_398 = (undefined8 **)CONCAT44(ppuStack_398._4_4_,0x5c);
          pppppplStack_390 = (longlong ******)ppppppplVar9;
          FUN_180015b40(uVar8,4,&DAT_180229390,&DAT_1802292c8);
          auStack_d0[0] = auStack_b0[0];
          uStack_b8 = uStack_98;
joined_r0x0001800b5e3c:
          if (0xf < uStack_b8) {
            __3_YAXPEAX_Z(auStack_d0[0]);
          }
        }
      }
      else {
        ppppppplVar9 = (longlong *******)GetProcAddress(puVar21,&DAT_180229380);
        pppppplStack_330 = (longlong ******)ppppppplVar9;
        pppppplStack_320 = (longlong ******)ppppppplVar9;
        if (ppppppplVar9 != (longlong *******)0x0) {
          puVar12 = (undefined8 *)__2_YAPEAX_K_Z(0x18);
          puVar13 = (undefined8 *)0x0;
          puStack_358 = puVar13;
          if (puVar12 != (undefined8 *)0x0) {
            *(undefined4 *)puVar12 = 0xffffffff;
            *(undefined8 *)((longlong)puVar12 + 4) = 0;
            puVar12[2] = 0;
            puStack_358 = puVar12;
          }
          *(undefined4 *)puStack_358 = auStack_360[0];
          *(undefined4 *)((longlong)puStack_358 + 4) = 0;
          *(undefined4 *)(puStack_358 + 1) = param_5;
          puVar12 = (undefined8 *)__2_YAPEAX_K_Z(0x40);
          if (puVar12 != (undefined8 *)0x0) {
            puVar12[7] = 7;
            puVar12[6] = 0;
            *(undefined2 *)(puVar12 + 4) = 0;
            *puVar12 = 0;
            puVar12[1] = 0;
            puVar12[2] = 0;
            puVar12[3] = 0;
            puVar13 = puVar12;
          }
          puStack_2a8 = (undefined8 *)0x0;
          plStack_2a0 = (longlong *)0x0;
          FUN_1800b7be0(&puStack_2a8,puVar13);
          puVar13 = puStack_2a8;
          plStack_338 = plStack_2a0;
          puStack_310 = puStack_2a8;
          puStack_340 = puStack_2a8;
          *puStack_2a8 = puVar21;
          puStack_2a8[2] = ppuVar11;
          puStack_2a8[3] = ppppppplVar9;
          if (puStack_2a8 + 4 != param_2) {
            FUN_18003ef70(puStack_2a8 + 4,param_2,0,0xffffffffffffffff);
          }
          iVar7 = QueryPerformanceFrequency(&lStack_2b8);
          dVar5 = 0.0;
          if ((iVar7 != 0) &&
             (iVar7 = QueryPerformanceCounter(&lStack_2e8), dVar5 = 0.0, iVar7 != 0)) {
            dVar5 = ((double)lStack_2e8 / (double)lStack_2b8) * (double)CONCAT44(uVar23,uVar22);
          }
          dStack_2f0 = dVar5;
          uVar8 = (*(code *)ppuVar11)(puStack_358);
          puVar13[1] = uVar8;
          if (puVar13[1] != 0) {
            iVar7 = QueryPerformanceFrequency(&pppppplStack_320);
            dVar5 = 0.0;
            if ((iVar7 != 0) &&
               (iVar7 = QueryPerformanceCounter(&puStack_310), dVar5 = 0.0, iVar7 != 0)) {
              dVar5 = ((double)(longlong)puStack_310 / (double)(longlong)pppppplStack_320) *
                      (double)CONCAT44(uVar23,uVar22);
            }
            *(undefined8 **)(puVar13[1] + 8) = puStack_358;
            dStack_308 = dVar5;
            goto LAB_1800b61f7;
          }
          cVar6 = FUN_18001dbc0(4);
          if (cVar6 != '\0') {
            uVar8 = FUN_18001db50();
            ppuStack_398 = (undefined8 **)CONCAT44(ppuStack_398._4_4_,0x8e);
            FUN_180015b40(uVar8,4,&DAT_180229478,&DAT_1802292c8);
          }
          iVar19 = -0x1ffffce8;
          if (*(int *)((longlong)puStack_358 + 4) != 0) {
            iVar19 = *(int *)((longlong)puStack_358 + 4);
          }
          goto LAB_1800b5d52;
        }
        cVar6 = FUN_18001dbc0(4);
        if (cVar6 != '\0') {
          uStack_260 = 7;
          uStack_268 = 0;
          auStack_278[0] = 0;
          FUN_18003ef70(auStack_278,param_2,0,0xffffffffffffffff);
          ppppppplVar9 = (longlong *******)FUN_1800da820(auStack_d0,auStack_278,1);
          if ((longlong ******)0xf < ppppppplVar9[3]) {
            ppppppplVar9 = (longlong *******)*ppppppplVar9;
          }
          uVar8 = FUN_18001db50();
          ppuStack_398 = (undefined8 **)CONCAT44(ppuStack_398._4_4_,0x65);
          pppppplStack_390 = (longlong ******)ppppppplVar9;
          FUN_180015b40(uVar8,4,&DAT_180229410,&DAT_1802292c8);
          goto joined_r0x0001800b5e3c;
        }
      }
      iVar19 = -0x1fffffef;
LAB_1800b5d52:
      puVar13 = puStack_368;
      if (7 < uStack_178) {
        __3_YAXPEAX_Z(auStack_190[0]);
        puVar13 = puStack_368;
      }
    }
    else {
      if (((iVar19 != -0x1ffffff0) && (iVar19 != -0x1ffffdfd)) && (iVar19 != -0x1ffffff8)) {
        cVar6 = FUN_18001dbc0(4);
        if (cVar6 != '\0') {
          uVar8 = FUN_18001db50();
          pppppplStack_390 = (longlong ******)CONCAT44(pppppplStack_390._4_4_,iVar19);
          ppuStack_398 = (undefined8 **)CONCAT44(ppuStack_398._4_4_,0xb1);
          FUN_180015b40(uVar8,4,&DAT_180229450,&DAT_1802292c8);
        }
        goto LAB_1800b5d52;
      }
      puVar12 = (undefined8 *)__2_YAPEAX_K_Z(0x18);
      puStack_358 = puVar13;
      if (puVar12 != (undefined8 *)0x0) {
        *(undefined4 *)puVar12 = 0xffffffff;
        *(undefined8 *)((longlong)puVar12 + 4) = 0;
        puVar12[2] = 0;
        puStack_358 = puVar12;
      }
      *(undefined4 *)puStack_358 = auStack_360[0];
      *(undefined4 *)((longlong)puStack_358 + 4) = 0xe0000010;
      *(undefined4 *)(puStack_358 + 1) = param_5;
      puVar12 = (undefined8 *)__2_YAPEAX_K_Z(0x40);
      if (puVar12 != (undefined8 *)0x0) {
        puVar12[7] = 7;
        puVar12[6] = 0;
        *(undefined2 *)(puVar12 + 4) = 0;
        *puVar12 = 0;
        puVar12[1] = 0;
        puVar12[2] = 0;
        puVar12[3] = 0;
        puVar13 = puVar12;
      }
      puStack_298 = (undefined8 *)0x0;
      plStack_290 = (longlong *)0x0;
      FUN_1800b7be0(&puStack_298,puVar13);
      plStack_338 = plStack_290;
      puStack_340 = puStack_298;
      *puStack_298 = 0;
      puStack_298[2] = 0;
      puStack_298[3] = 0;
      if (puStack_298 + 4 != param_2) {
        FUN_18003ef70(puStack_298 + 4,param_2,0,0xffffffffffffffff);
      }
LAB_1800b61f7:
      puVar13 = puStack_368;
      puStack_350 = puStack_368 + 7;
      iVar7 = _Mtx_lock();
      if (iVar7 != 0) {
        __Throw_C_error_std__YAXH_Z(iVar7);
      }
      puVar12 = (undefined8 *)puVar13[1];
      cVar6 = *(char *)((longlong)puVar12[1] + 0x19);
      puVar18 = puVar12;
      puVar17 = (undefined8 *)puVar12[1];
      while (cVar6 == '\0') {
        if ((undefined8 *)puVar17[4] < puStack_358) {
          puVar14 = (undefined8 *)puVar17[2];
          puVar17 = puVar18;
        }
        else {
          puVar14 = (undefined8 *)*puVar17;
        }
        puVar18 = puVar17;
        puVar17 = puVar14;
        cVar6 = *(char *)((longlong)puVar14 + 0x19);
      }
      if ((puVar18 == puVar12) || (puStack_358 < (undefined8 *)puVar18[4])) {
        ppuStack_328 = &puStack_358;
        ppuStack_398 = (undefined8 **)FUN_1800b4a70(puVar13 + 1,puStack_358,&ppuStack_328);
        FUN_180091e60(puVar13 + 1,&puStack_348,puVar18,ppuStack_398 + 4);
        puVar18 = puStack_348;
      }
      if (plStack_338 != (longlong *)0x0) {
        LOCK();
        *(int *)(plStack_338 + 1) = (int)plStack_338[1] + 1;
        UNLOCK();
      }
      plVar4 = (longlong *)puVar18[6];
      puVar18[6] = plStack_338;
      puVar18[5] = puStack_340;
      plStack_280 = plVar4;
      if (plVar4 != (longlong *)0x0) {
        LOCK();
        plVar1 = plVar4 + 1;
        lVar20 = *plVar1;
        *(int *)plVar1 = (int)*plVar1 + -1;
        UNLOCK();
        if ((int)lVar20 == 1) {
          (**(code **)*plVar4)(plVar4);
          LOCK();
          piVar2 = (int *)((longlong)plVar4 + 0xc);
          iVar7 = *piVar2;
          *piVar2 = *piVar2 + -1;
          UNLOCK();
          if (iVar7 == 1) {
            (**(code **)(*plStack_280 + 8))();
          }
        }
      }
      puVar12 = (undefined8 *)FUN_1800b7880(puVar13 + 3,auStack_360);
      *puVar12 = puStack_358;
      if (DAT_1802b129e != '\0') {
        if ((DAT_1802b3158 & 1) == 0) {
          DAT_1802b3158 = DAT_1802b3158 | 1;
          FUN_1800e8550();
          FUN_1801e8930(&DAT_180202f40);
        }
        pppppplStack_390 = pppppplStack_330;
        ppuStack_398 = ppuStack_318;
        FUN_1800fda40(&DAT_1802b31d8,auStack_360[0],apppppplStack_1b0,puStack_340[1]);
      }
      iVar7 = _Mtx_unlock(puStack_350);
      if (iVar7 != 0) {
        __Throw_C_error_std__YAXH_Z(iVar7);
      }
      if (7 < uStack_178) {
        __3_YAXPEAX_Z(auStack_190[0]);
      }
    }
    if (puVar21 != (undefined8 *)0x0) {
      puVar12 = puVar13 + 8;
      puStack_1d0 = puVar12;
      iVar7 = _Mtx_lock(puVar12);
      if (iVar7 != 0) {
        __Throw_C_error_std__YAXH_Z(iVar7);
      }
      puVar18 = (undefined8 *)puVar13[9];
      cVar6 = *(char *)((longlong)puVar18[1] + 0x19);
      puVar17 = puVar18;
      puVar14 = (undefined8 *)puVar18[1];
      while (cVar6 == '\0') {
        if ((undefined8 *)puVar14[4] < puVar21) {
          puVar16 = (undefined8 *)puVar14[2];
          puVar14 = puVar17;
        }
        else {
          puVar16 = (undefined8 *)*puVar14;
        }
        puVar17 = puVar14;
        puVar14 = puVar16;
        cVar6 = *(char *)((longlong)puVar16 + 0x19);
      }
      if ((puVar17 == puVar18) || (puVar21 < (undefined8 *)puVar17[4])) {
        puStack_2e0 = puVar18;
        ppuVar11 = &puStack_2e0;
      }
      else {
        puStack_368 = puVar17;
        ppuVar11 = &puStack_368;
      }
      if (*ppuVar11 == puVar18) {
        ppuStack_398 = (undefined8 **)CONCAT71(ppuStack_398._1_7_,DAT_1802b1210);
        puStack_2d8 = puVar21;
        FUN_1800b7a90(puVar13 + 9,auStack_1c0,puVar17,&puStack_2d8);
      }
      iVar7 = _Mtx_unlock(puVar12);
      if (iVar7 != 0) {
        __Throw_C_error_std__YAXH_Z(iVar7);
      }
    }
    if (((iVar19 == 0) || (iVar19 == -0x1ffffff0)) || (iVar19 == -0x1ffffff8)) {
      cVar6 = FUN_18001dbc0(2);
      if (cVar6 != '\0') {
        ppppppplVar9 = apppppplStack_1b0;
        if (0xf < uStack_198) {
          ppppppplVar9 = (longlong *******)apppppplStack_1b0[0];
        }
        dVar5 = dStack_308 - dStack_2f0;
        puVar21 = (undefined8 *)(dStack_2f8 - dStack_300);
        uVar8 = FUN_18001db50();
        pppppplStack_390 = (longlong ******)CONCAT44(pppppplStack_390._4_4_,auStack_360[0]);
        ppuStack_398 = (undefined8 **)CONCAT44(ppuStack_398._4_4_,0xe3);
        pppppplStack_388 = (longlong ******)ppppppplVar9;
        puStack_380 = puVar21;
        dStack_378 = dVar5;
        FUN_180015b40(uVar8,2,&DAT_1802294f0,&DAT_1802292c8);
      }
      goto LAB_1800b6535;
    }
  }
  if (puStack_358 != (undefined8 *)0x0) {
    __3_YAXPEAX_Z();
  }
LAB_1800b6535:
  cVar6 = FUN_18001dbc0(2);
  if (cVar6 != '\0') {
    puVar21 = (undefined8 *)FUN_18008f750(auStack_90,iVar19);
    if (0xf < (ulonglong)puVar21[3]) {
      puVar21 = (undefined8 *)*puVar21;
    }
    ppppppplVar9 = apppppplStack_1b0;
    if (0xf < uStack_198) {
      ppppppplVar9 = (longlong *******)apppppplStack_1b0[0];
    }
    uVar8 = FUN_18001db50();
    pppppplStack_390 = (longlong ******)CONCAT44(pppppplStack_390._4_4_,auStack_360[0]);
    ppuStack_398 = (undefined8 **)CONCAT44(ppuStack_398._4_4_,0xe6);
    pppppplStack_388 = (longlong ******)ppppppplVar9;
    puStack_380 = puVar21;
    FUN_180015b40(uVar8,2,&DAT_1802294b8,&DAT_1802292c8);
    if (0xf < uStack_78) {
      __3_YAXPEAX_Z(auStack_90[0]);
    }
  }
  plVar4 = plStack_338;
  if (plStack_338 != (longlong *)0x0) {
    LOCK();
    plVar1 = plStack_338 + 1;
    lVar20 = *plVar1;
    *(int *)plVar1 = (int)*plVar1 + -1;
    UNLOCK();
    if ((int)lVar20 == 1) {
      (**(code **)*plStack_338)(plStack_338);
      LOCK();
      piVar2 = (int *)((longlong)plVar4 + 0xc);
      iVar19 = *piVar2;
      *piVar2 = *piVar2 + -1;
      UNLOCK();
      if (iVar19 == 1) {
        (**(code **)(*plStack_338 + 8))();
      }
    }
  }
  if (0xf < uStack_198) {
    __3_YAXPEAX_Z(apppppplStack_1b0[0]);
  }
  FUN_1801e8810(uStack_70 ^ (ulonglong)auStack_3b8);
  return;
}

