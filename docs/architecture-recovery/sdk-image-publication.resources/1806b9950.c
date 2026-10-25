
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_1806b9950(undefined8 param_1,longlong param_2,char param_3,int *param_4)

{
  float fVar1;
  float fVar2;
  undefined4 uVar3;
  undefined *puVar4;
  undefined1 auVar5 [16];
  undefined1 auVar6 [16];
  undefined1 auVar7 [16];
  undefined1 auVar8 [16];
  undefined1 auVar9 [16];
  undefined1 auVar10 [16];
  undefined1 auVar11 [16];
  undefined1 auVar12 [16];
  undefined1 auVar13 [16];
  undefined1 auVar14 [16];
  char cVar15;
  undefined4 uVar16;
  int iVar17;
  uint uVar18;
  int iVar19;
  undefined8 uVar20;
  longlong lVar21;
  undefined8 *puVar22;
  undefined8 uVar23;
  undefined8 *puVar24;
  undefined8 *puVar25;
  longlong *plVar26;
  uint uVar27;
  undefined **ppuVar28;
  ulonglong uVar30;
  int iVar31;
  int *piVar32;
  ulonglong uVar33;
  ulonglong uVar34;
  undefined *puVar35;
  undefined4 *puVar36;
  longlong lVar37;
  bool bVar38;
  undefined1 auStack_2d8 [32];
  undefined8 *puStack_2b8;
  undefined4 *puStack_2b0;
  undefined8 *puStack_2a8;
  undefined8 *puStack_2a0;
  double dStack_298;
  int iStack_290;
  char cStack_288;
  int iStack_284;
  char cStack_280;
  int iStack_27c;
  int *piStack_278;
  longlong lStack_270;
  int iStack_268;
  int iStack_264;
  undefined4 uStack_260;
  undefined4 uStack_25c;
  undefined4 uStack_258;
  undefined4 uStack_254;
  undefined4 uStack_250;
  undefined4 uStack_24c;
  undefined4 uStack_248;
  undefined4 uStack_244;
  undefined4 uStack_240;
  undefined4 uStack_23c;
  undefined4 uStack_238;
  undefined4 uStack_234;
  undefined4 uStack_230;
  undefined4 uStack_22c;
  undefined4 uStack_228;
  undefined4 uStack_224;
  undefined4 uStack_220;
  undefined4 uStack_21c;
  undefined4 uStack_218;
  undefined4 uStack_214;
  undefined4 uStack_210;
  undefined4 uStack_20c;
  undefined8 *puStack_208;
  undefined8 *puStack_200;
  undefined8 uStack_1f8;
  undefined *puStack_1f0;
  ulonglong uStack_1e8;
  uint uStack_1e0;
  undefined4 uStack_1dc;
  undefined *puStack_1d8;
  undefined4 uStack_1d0;
  undefined8 uStack_1c8;
  undefined *puStack_1c0;
  undefined8 uStack_1b8;
  undefined4 uStack_1b0;
  undefined8 uStack_1ac;
  undefined8 uStack_1a4;
  undefined8 uStack_198;
  undefined8 uStack_190;
  undefined *puStack_188;
  undefined8 uStack_180;
  undefined8 uStack_178;
  undefined8 *puStack_170;
  undefined8 *puStack_168;
  undefined8 uStack_160;
  undefined8 uStack_158;
  longlong lStack_150;
  undefined8 uStack_148;
  longlong lStack_140;
  undefined8 uStack_138;
  undefined8 uStack_130;
  longlong lStack_128;
  undefined8 uStack_120;
  longlong lStack_118;
  undefined8 uStack_110;
  undefined8 uStack_108;
  undefined8 auStack_f8 [2];
  ulonglong uStack_e8;
  ulonglong uStack_e0;
  undefined8 auStack_d8 [2];
  ulonglong uStack_c8;
  ulonglong uStack_c0;
  undefined *puStack_b8;
  undefined8 uStack_b0;
  undefined8 uStack_a8;
  undefined8 *puStack_a0;
  undefined8 *puStack_98;
  int iStack_90;
  undefined *puStack_88;
  undefined8 uStack_80;
  undefined8 uStack_78;
  undefined8 *puStack_70;
  undefined8 uStack_68;
  ulonglong uStack_60;
  ulonglong uVar29;
  
  uStack_190 = 0xfffffffffffffffe;
  uStack_60 = _DAT_180c35580 ^ (ulonglong)auStack_2d8;
  puVar35 = (undefined *)0x0;
  iStack_268 = 0;
  iStack_284 = -0x1fffff01;
  iVar19 = *(int *)(param_2 + 0x18);
  lVar21 = *(longlong *)(param_2 + 0x28);
  if (lVar21 == 0) {
    lVar21 = *(longlong *)(_DAT_180c9f118 + 0x28);
  }
  puStack_88 = &UNK_180aab320;
  uStack_80 = 0;
  cStack_280 = param_3;
  piStack_278 = param_4;
  lStack_270 = param_2;
  uStack_198 = param_1;
  FUN_18058c440();
  uStack_78 = 0;
  uStack_68 = 0;
  puStack_70 = _DAT_180c9e508;
  FUN_18059ad40(&puStack_88,lVar21);
  lVar21 = *(longlong *)(param_2 + 0x30);
  if (lVar21 == 0) {
    lVar21 = *(longlong *)(_DAT_180c9f118 + 0x30);
  }
  puStack_b8 = &UNK_180aab0c8;
  uStack_b0 = 0;
  FUN_18058c440();
  uStack_a8 = 0;
  iStack_90 = 0;
  puStack_a0 = _DAT_180c9e508;
  puStack_98 = _DAT_180c9e508;
  FUN_18059b5a0(&puStack_b8,lVar21);
  lVar21 = *(longlong *)(param_2 + 0x50);
  if (lVar21 == 0) {
    lVar21 = *(longlong *)(_DAT_180c9f118 + 0x50);
  }
  puStack_1c0 = &UNK_180aab260;
  uStack_1b8 = 0;
  uStack_1ac = 0;
  uStack_1a4 = 0;
  uStack_1b0 = 0;
  FUN_18059bc20(&puStack_1c0,lVar21);
  ppuVar28 = *(undefined ***)(param_2 + 0x20);
  if (ppuVar28 == (undefined **)0x0) {
    ppuVar28 = *(undefined ***)(_DAT_180c9f118 + 0x20);
  }
  puStack_1f0 = &UNK_180aab4b0;
  uStack_1e8 = 0;
  uStack_1e0 = 0;
  uStack_1dc = 0;
  puStack_1d8 = (undefined *)0x0;
  uStack_1d0 = 0;
  if (ppuVar28 == &puStack_1f0) {
    FUN_18099f0b0(&UNK_180aaa848,0xd0f);
  }
  uVar27 = *(uint *)(ppuVar28 + 2);
  uVar16 = 0;
  if ((char)uVar27 != '\0') {
    bVar38 = (uVar27 & 1) != 0;
    if (bVar38) {
      puVar35 = ppuVar28[3];
      uStack_1e0 = 1;
      puStack_1d8 = puVar35;
    }
    uVar16 = 0;
    if ((uVar27 >> 1 & 1) != 0) {
      uVar16 = *(undefined4 *)(ppuVar28 + 4);
      uStack_1e0 = bVar38 | 2;
      uStack_1d0 = uVar16;
    }
  }
  if (((ulonglong)ppuVar28[1] & 1) != 0) {
    puVar4 = ppuVar28[1];
    if ((uStack_1e8 & 1) == 0) {
      uVar20 = FUN_18058c630(&uStack_1e8);
      puVar35 = puStack_1d8;
      uVar16 = uStack_1d0;
    }
    else {
      uVar20 = 0;
    }
    FUN_1809a6650(uVar20,(ulonglong)puVar4 & 0xfffffffffffffffe);
  }
  piVar32 = piStack_278;
  if (*(longlong *)(piStack_278 + 0xc) == 0) {
    lVar21 = FUN_1809e82b0(0x110,0x180c9ea09);
    *(longlong *)(piVar32 + 0xc) = lVar21;
    if (lVar21 == 0) {
      cVar15 = FUN_1805a2010(4);
      if (cVar15 != '\0') {
        uVar20 = FUN_1805a1d70();
        puStack_2b8 = (undefined8 *)CONCAT44(puStack_2b8._4_4_,0x5b11);
        FUN_1805a1f10(uVar20,4,&UNK_180ad2880,&UNK_180ad2720);
      }
      iStack_284 = 0xe0000002;
      uStack_210 = 0xe0000002;
                    /* WARNING: Subroutine does not return */
      _CxxThrowException(&uStack_210,&UNK_180c23c00);
    }
  }
  memset(*(undefined8 *)(piVar32 + 0xc),0,0x110);
  piVar32[3] = iStack_90;
  *piVar32 = iVar19;
  if (iVar19 == 1) {
    piVar32[1] = 0;
  }
  else {
    piVar32[1] = *(int *)(param_2 + 0x58);
    cVar15 = FUN_1805a2010(1);
    iVar31 = iStack_90;
    if (cVar15 != '\0') {
      uVar3 = *(undefined4 *)(param_2 + 0x58);
      uVar20 = FUN_1805a1d70();
      puStack_2a0 = (undefined8 *)CONCAT44(puStack_2a0._4_4_,uVar3);
      puStack_2a8 = (undefined8 *)CONCAT44(puStack_2a8._4_4_,iVar19);
      puStack_2b0 = (undefined4 *)CONCAT44(puStack_2b0._4_4_,iVar31);
      puStack_2b8 = (undefined8 *)CONCAT44(puStack_2b8._4_4_,0x5b21);
      FUN_1805a1f10(uVar20,1,&UNK_180ad2848,&UNK_180ad2720);
      piVar32 = piStack_278;
    }
  }
  piVar32[2] = (int)uStack_68;
  piVar32[0x2d] = uStack_68._4_4_;
  piVar32[0x2c] = (uint)(*(int *)(param_2 + 0x1c) == 2);
  *(undefined **)(*(longlong *)(piVar32 + 0xc) + 0x10) = puVar35;
  *(undefined4 *)(*(longlong *)(piVar32 + 0xc) + 0x18) = uVar16;
  puStack_2b8._0_4_ = piVar32[3];
  _snprintf_s(piVar32 + 0x1c,0x40,0x3f,&UNK_180ad2830);
  piVar32[4] = (int)uStack_1a4;
  piVar32[5] = uStack_1a4._4_4_;
  puVar22 = *(undefined8 **)(piVar32 + 8);
  if (puVar22 == (undefined8 *)0x0) {
    puVar22 = (undefined8 *)thunk_FUN_1809e82b0(0x40,0x180c9ea09);
    *(undefined8 **)(piVar32 + 8) = puVar22;
    if (puVar22 == (undefined8 *)0x0) {
      cVar15 = FUN_1805a2010(4);
      if (cVar15 != '\0') {
        uVar16 = GetLastError();
        uVar20 = FUN_1805a1d70();
        puStack_2b0 = (undefined4 *)CONCAT44(puStack_2b0._4_4_,uVar16);
        puStack_2b8._0_4_ = 0x5b3d;
        FUN_1805a1f10(uVar20,4,&UNK_180ad27f8,&UNK_180ad2720);
      }
      iStack_284 = 0xe0000002;
      uStack_240 = 0xe0000002;
                    /* WARNING: Subroutine does not return */
      _CxxThrowException(&uStack_240,&UNK_180c23c00);
    }
  }
  *puVar22 = 0;
  puVar22[1] = 0;
  puVar22[2] = 0;
  puVar22[3] = 0;
  puVar22[4] = 0;
  puVar22[5] = 0;
  puVar22[6] = 0;
  puVar22[7] = 0;
  lVar21 = *(longlong *)(piVar32 + 10);
  if (lVar21 == 0) {
    lVar21 = thunk_FUN_1809e82b0(0x100,0x180c9ea09);
    *(longlong *)(piVar32 + 10) = lVar21;
    if (lVar21 == 0) {
      cVar15 = FUN_1805a2010(4);
      if (cVar15 != '\0') {
        uVar16 = GetLastError();
        uVar20 = FUN_1805a1d70();
        puStack_2b0 = (undefined4 *)CONCAT44(puStack_2b0._4_4_,uVar16);
        puStack_2b8._0_4_ = 0x5b4d;
        FUN_1805a1f10(uVar20,4,&UNK_180ad2968,&UNK_180ad2720);
      }
      iStack_284 = 0xe0000002;
      uStack_238 = 0xe0000002;
                    /* WARNING: Subroutine does not return */
      _CxxThrowException(&uStack_238,&UNK_180c23c00);
    }
  }
  memset(lVar21,0,0x100);
  puStack_2b8 = puStack_a0;
  if (0xf < (ulonglong)puStack_a0[3]) {
    puStack_2b8 = (undefined8 *)*puStack_a0;
  }
  _snprintf_s(*(undefined8 *)(piVar32 + 8),0x40,0x3f,&UNK_180aad270);
  puStack_2b8 = puStack_98;
  if (0xf < (ulonglong)puStack_98[3]) {
    puStack_2b8 = (undefined8 *)*puStack_98;
  }
  _snprintf_s(*(undefined8 *)(piVar32 + 10),0x100,0xff,&UNK_180aad270);
  cVar15 = FUN_1805a2010(1);
  if (cVar15 != '\0') {
    puVar22 = puStack_a0;
    if (0xf < (ulonglong)puStack_a0[3]) {
      puVar22 = (undefined8 *)*puStack_a0;
    }
    fVar1 = (float)piVar32[5];
    fVar2 = (float)piVar32[4];
    uVar20 = FUN_1805a1d70();
    iStack_290 = piVar32[0x2d];
    puStack_2b0 = (undefined4 *)CONCAT44(puStack_2b0._4_4_,piVar32[3]);
    puStack_2b8 = (undefined8 *)CONCAT44(puStack_2b8._4_4_,0x5b5b);
    puStack_2a8 = puVar22;
    puStack_2a0 = (undefined8 *)(double)fVar2;
    dStack_298 = (double)fVar1;
    FUN_1805a1f10(uVar20,1,&UNK_180ad2900,&UNK_180ad2720);
  }
  iVar19 = *(int *)(param_2 + 0x40);
  piVar32[0x2f] = iVar19;
  if (*(longlong *)(piVar32 + 0x30) == 0) {
    auVar5._8_8_ = 0;
    auVar5._0_8_ = (longlong)iVar19;
    uVar20 = SUB168(ZEXT816(0xd0) * auVar5,0);
    if (SUB168(ZEXT816(0xd0) * auVar5,8) != 0) {
      uVar20 = 0xffffffffffffffff;
    }
    lVar21 = thunk_FUN_1809e82b0(uVar20,0x180c9ea09);
    *(longlong *)(piVar32 + 0x30) = lVar21;
    if (lVar21 == 0) {
      cVar15 = FUN_1805a2010(4);
      if (cVar15 != '\0') {
        uVar16 = GetLastError();
        uVar20 = FUN_1805a1d70();
        puStack_2b0 = (undefined4 *)CONCAT44(puStack_2b0._4_4_,uVar16);
        puStack_2b8 = (undefined8 *)CONCAT44(puStack_2b8._4_4_,0x5b63);
        FUN_1805a1f10(uVar20,4,&UNK_180ad28c0,&UNK_180ad2720);
      }
      iStack_284 = 0xe0000002;
      uStack_214 = 0xe0000002;
                    /* WARNING: Subroutine does not return */
      _CxxThrowException(&uStack_214,&UNK_180c23c00);
    }
    memset(lVar21,0,(longlong)piVar32[0x2f] * 0xd0);
  }
  puStack_188 = &UNK_180aaae58;
  uStack_180 = 0;
  uStack_160 = 0;
  uStack_158 = 0;
  lStack_150 = 0;
  uStack_148 = 0;
  lStack_140 = 0;
  uStack_138 = 0;
  uStack_130 = 0;
  lStack_128 = 0;
  uStack_120 = 0;
  lStack_118 = 0;
  uStack_110 = 0;
  uStack_108 = 0;
  FUN_18058c440();
  uStack_178 = 0;
  puStack_170 = _DAT_180c9e508;
  puStack_168 = _DAT_180c9e508;
  uStack_c0 = 0xf;
  uStack_c8 = 0;
  auStack_d8[0]._0_1_ = 0;
  FUN_1805911d0(auStack_d8,&DAT_180aab591,0);
  cStack_288 = '\x01';
  iVar19 = -1;
  iStack_268 = 0;
  do {
    cVar15 = cStack_288;
    iVar31 = 0;
    uVar20 = 0xffffffffffffffff;
    if (piVar32[0x2f] <= iStack_268) {
      piVar32[0x2f] = iVar19 + 1;
      if (0xf < uStack_c0) {
        __3_YAXPEAX_Z(CONCAT71(auStack_d8[0]._1_7_,(undefined1)auStack_d8[0]));
      }
      uStack_c0 = 0xf;
      uStack_c8 = 0;
      auStack_d8[0]._0_1_ = 0;
      FUN_18059c520(&puStack_188);
      puStack_1f0 = &UNK_180aab4b0;
      FUN_18058c5c0(&uStack_1e8);
      puStack_1c0 = &UNK_180aab260;
      FUN_18058c5c0(&uStack_1b8);
      puStack_b8 = &UNK_180aab0c8;
      FUN_18058c4a0(&puStack_a0,_DAT_180c9e508);
      FUN_18058c4a0(&puStack_98,_DAT_180c9e508);
      FUN_18058c5c0(&uStack_b0);
      puStack_b8 = &UNK_180a995e0;
      puStack_88 = &UNK_180aab320;
      FUN_18058c4a0(&puStack_70,_DAT_180c9e508);
      FUN_18058c5c0(&uStack_80);
      cVar15 = FUN_1805a2010(1);
      if (cVar15 != '\0') {
        uVar20 = FUN_1805a1d70();
        puStack_2b8 = (undefined8 *)CONCAT44(puStack_2b8._4_4_,0x5d4c);
        FUN_1805a1f10(uVar20,1,&UNK_180ab52d8,&UNK_180ad2720);
      }
      FUN_1809e7e30(uStack_60 ^ (ulonglong)auStack_2d8);
      return;
    }
    ppuVar28 = *(undefined ***)(*(longlong *)(param_2 + 0x48) + 8 + (longlong)iStack_268 * 8);
    if (ppuVar28 != &puStack_188) {
      (**(code **)(puStack_188 + 0x30))(&puStack_188);
      FUN_18059d1d0(&puStack_188,ppuVar28);
    }
    if (cVar15 == '\0') {
LAB_1806ba1ad:
      iStack_27c = iVar19 + 1;
      puVar36 = (undefined4 *)((longlong)iStack_27c * 0xd0 + *(longlong *)(piVar32 + 0x30));
      puStack_2b8 = puStack_170;
      if (0xf < (ulonglong)puStack_170[3]) {
        puStack_2b8 = (undefined8 *)*puStack_170;
      }
      _snprintf_s(puVar36 + 1,0x80,0x7f,&UNK_180aad270);
      cVar15 = FUN_1805a2010(1);
      if (cVar15 != '\0') {
        puVar22 = puStack_168;
        if (0xf < (ulonglong)puStack_168[3]) {
          puVar22 = (undefined8 *)*puStack_168;
        }
        uVar23 = FUN_1805a1d70();
        puStack_2b0 = (undefined4 *)CONCAT44(puStack_2b0._4_4_,piStack_278[3]);
        puStack_2b8 = (undefined8 *)CONCAT44(puStack_2b8._4_4_,0x5b7e);
        puStack_2a8 = (undefined8 *)(puVar36 + 1);
        puStack_2a0 = puVar22;
        FUN_1805a1f10(uVar23,1,&UNK_180ad2a58,&UNK_180ad2720);
      }
      if (auStack_d8 != puStack_168) {
        FUN_180591340(auStack_d8,puStack_168,0,0xffffffffffffffff);
      }
      uVar34 = uStack_c0;
      uVar30 = uStack_c8;
      uVar27 = -(uint)(DAT_180ab4914 != '\0') & 5;
      uVar29 = (ulonglong)uVar27;
      puVar22 = (undefined8 *)CONCAT71(auStack_d8[0]._1_7_,(undefined1)auStack_d8[0]);
      puVar24 = auStack_d8;
      if (0xf < uStack_c0) {
        puVar24 = puVar22;
      }
      uVar33 = (ulonglong)uVar27;
      if (uStack_c8 < uVar29) {
        uVar33 = uStack_c8;
      }
      uVar18 = 0;
      uVar27 = uVar18;
      if (uVar33 != 0) {
        uVar27 = memcmp(puVar24);
      }
      if (uVar27 == 0) {
        if (uVar30 < uVar29) {
          uVar27 = 0xffffffff;
        }
        else {
          uVar27 = (uint)(uVar30 != uVar29);
        }
      }
      if (uVar27 == 0) {
        puVar36[0x22] = (int)uStack_148;
        puVar36[0x21] = 2;
        if (*(longlong *)(puVar36 + 0x26) == 0) {
          auVar6._8_8_ = 0;
          auVar6._0_8_ = (longlong)(int)uStack_148;
          uVar23 = SUB168(ZEXT816(4) * auVar6,0);
          if (SUB168(ZEXT816(4) * auVar6,8) != 0) {
            uVar23 = uVar20;
          }
          lVar21 = thunk_FUN_1809e82b0(uVar23,0x180c9ea09);
          *(longlong *)(puVar36 + 0x26) = lVar21;
          if (lVar21 == 0) {
            cVar15 = FUN_1805a2010(4);
            if (cVar15 != '\0') {
              uVar16 = GetLastError();
              uVar20 = FUN_1805a1d70();
              puStack_2b0 = (undefined4 *)CONCAT44(puStack_2b0._4_4_,uVar16);
              puStack_2b8 = (undefined8 *)CONCAT44(puStack_2b8._4_4_,0x5b8c);
              FUN_1805a1f10(uVar20,4,&UNK_180ad2a20,&UNK_180ad2720);
            }
            iStack_284 = 0xe0000002;
            uStack_254 = 0xe0000002;
                    /* WARNING: Subroutine does not return */
            _CxxThrowException(&uStack_254,&UNK_180c23c00);
          }
        }
        if (puVar36[0x22] != 0) {
          *puVar36 = 1;
        }
        memcpy(*(undefined8 *)(puVar36 + 0x26),lStack_140 + 8,(longlong)(int)puVar36[0x22] << 2);
        piVar32 = piStack_278;
        param_2 = lStack_270;
        iVar19 = iStack_27c;
      }
      else {
        uVar27 = -(uint)(DAT_180ab491c != '\0') & 3;
        uVar29 = (ulonglong)uVar27;
        puVar24 = auStack_d8;
        if (0xf < uVar34) {
          puVar24 = puVar22;
        }
        uVar33 = (ulonglong)uVar27;
        if (uVar30 < uVar29) {
          uVar33 = uVar30;
        }
        uVar27 = uVar18;
        if (uVar33 != 0) {
          uVar27 = memcmp(puVar24);
        }
        if (uVar27 == 0) {
          if (uVar30 < uVar29) {
            uVar27 = 0xffffffff;
          }
          else {
            uVar27 = (uint)(uVar30 != uVar29);
          }
        }
        if (uVar27 == 0) {
          puVar36[0x22] = (int)uStack_120;
          puVar36[0x21] = 1;
          if (*(longlong *)(puVar36 + 0x24) == 0) {
            auVar7._8_8_ = 0;
            auVar7._0_8_ = (longlong)(int)uStack_120;
            uVar23 = SUB168(ZEXT816(4) * auVar7,0);
            if (SUB168(ZEXT816(4) * auVar7,8) != 0) {
              uVar23 = uVar20;
            }
            lVar21 = thunk_FUN_1809e82b0(uVar23,0x180c9ea09);
            *(longlong *)(puVar36 + 0x24) = lVar21;
            if (lVar21 == 0) {
              cVar15 = FUN_1805a2010(4);
              if (cVar15 != '\0') {
                uVar16 = GetLastError();
                uVar20 = FUN_1805a1d70();
                puStack_2b0 = (undefined4 *)CONCAT44(puStack_2b0._4_4_,uVar16);
                puStack_2b8 = (undefined8 *)CONCAT44(puStack_2b8._4_4_,0x5bab);
                FUN_1805a1f10(uVar20,4,&UNK_180ad29f0,&UNK_180ad2720);
              }
              iStack_284 = 0xe0000002;
              uStack_230 = 0xe0000002;
                    /* WARNING: Subroutine does not return */
              _CxxThrowException(&uStack_230,&UNK_180c23c00);
            }
          }
          if (puVar36[0x22] != 0) {
            *puVar36 = 1;
          }
          memcpy(*(undefined8 *)(puVar36 + 0x24),lStack_118 + 8,(longlong)(int)puVar36[0x22] << 2);
          piVar32 = piStack_278;
          param_2 = lStack_270;
          iVar19 = iStack_27c;
        }
        else {
          uVar27 = -(uint)(DAT_180ab4928 != '\0') & 6;
          uVar29 = (ulonglong)uVar27;
          puVar24 = auStack_d8;
          if (0xf < uVar34) {
            puVar24 = puVar22;
          }
          uVar33 = (ulonglong)uVar27;
          if (uVar30 < uVar29) {
            uVar33 = uVar30;
          }
          uVar27 = uVar18;
          if (uVar33 != 0) {
            uVar27 = memcmp(puVar24);
          }
          if (uVar27 == 0) {
            if (uVar30 < uVar29) {
              uVar27 = 0xffffffff;
            }
            else {
              uVar27 = (uint)(uVar30 != uVar29);
            }
          }
          if (uVar27 == 0) {
            puVar36[0x22] = (int)uStack_158;
            puVar36[0x21] = 3;
            if (*(longlong *)(puVar36 + 0x28) == 0) {
              auVar8._8_8_ = 0;
              auVar8._0_8_ = (longlong)(int)uStack_158;
              uVar23 = SUB168(ZEXT816(0x1010) * auVar8,0);
              if (SUB168(ZEXT816(0x1010) * auVar8,8) != 0) {
                uVar23 = uVar20;
              }
              lVar21 = thunk_FUN_1809e82b0(uVar23);
              *(longlong *)(puVar36 + 0x28) = lVar21;
              if (lVar21 == 0) {
                cVar15 = FUN_1805a2010(4);
                if (cVar15 != '\0') {
                  uVar16 = GetLastError();
                  uVar20 = FUN_1805a1d70();
                  puStack_2b0 = (undefined4 *)CONCAT44(puStack_2b0._4_4_,uVar16);
                  puStack_2b8 = (undefined8 *)CONCAT44(puStack_2b8._4_4_,0x5bca);
                  FUN_1805a1f10(uVar20,4,&UNK_180ad29b8,&UNK_180ad2720);
                }
                iStack_284 = 0xe0000002;
                uStack_24c = 0xe0000002;
                    /* WARNING: Subroutine does not return */
                _CxxThrowException(&uStack_24c,&UNK_180c23c00);
              }
              memset(lVar21,0,(longlong)(int)puVar36[0x22] * 0x1010);
            }
            if (puVar36[0x22] != 0) {
              *puVar36 = 1;
            }
            for (; piVar32 = piStack_278, param_2 = lStack_270, iVar19 = iStack_27c,
                iVar31 < (int)puVar36[0x22]; iVar31 = iVar31 + 1) {
              uStack_e0 = 0xf;
              uStack_e8 = 0;
              auStack_f8[0]._0_1_ = 0;
              FUN_180591340(auStack_f8,*(undefined8 *)(lStack_150 + 8 + (longlong)iVar31 * 8),0,
                            0xffffffffffffffff);
              puStack_2b8 = auStack_f8;
              if (0xf < uStack_e0) {
                puStack_2b8 = (undefined8 *)CONCAT71(auStack_f8[0]._1_7_,(undefined1)auStack_f8[0]);
              }
              lVar21 = (longlong)iVar31 * 0x1010;
              _snprintf_s(*(longlong *)(puVar36 + 0x28) + lVar21,0x1000,0xfff,&UNK_180aad270);
              if ((0xfff < uStack_e8) && (cVar15 = FUN_1805a2010(3), cVar15 != '\0')) {
                uVar20 = FUN_1805a1d70();
                puStack_2b0 = (undefined4 *)CONCAT44(puStack_2b0._4_4_,0x1000);
                puStack_2b8 = (undefined8 *)CONCAT44(puStack_2b8._4_4_,0x5bde);
                FUN_1805a1f10(uVar20,3,&UNK_180ad2b28,&UNK_180ad2720);
              }
              cVar15 = FUN_1805a2010(1);
              if (cVar15 != '\0') {
                lVar37 = *(longlong *)(puVar36 + 0x28);
                uVar20 = FUN_1805a1d70();
                puStack_2b8 = (undefined8 *)CONCAT44(puStack_2b8._4_4_,0x5be1);
                puStack_2b0 = puVar36 + 1;
                puStack_2a8 = (undefined8 *)(lVar21 + lVar37);
                FUN_1805a1f10(uVar20,1,&UNK_180ad2af8,&UNK_180ad2720);
              }
              if (0xf < uStack_e0) {
                __3_YAXPEAX_Z(CONCAT71(auStack_f8[0]._1_7_,(undefined1)auStack_f8[0]));
              }
            }
          }
          else {
            uVar27 = -(uint)(DAT_180ab4920 != '\0') & 5;
            uVar29 = (ulonglong)uVar27;
            puVar24 = auStack_d8;
            if (0xf < uVar34) {
              puVar24 = puVar22;
            }
            uVar34 = (ulonglong)uVar27;
            if (uVar30 < uVar29) {
              uVar34 = uVar30;
            }
            uVar27 = uVar18;
            if (uVar34 != 0) {
              uVar27 = memcmp(puVar24);
            }
            if (uVar27 == 0) {
              if (uVar30 < uVar29) {
                uVar27 = 0xffffffff;
              }
              else {
                uVar27 = (uint)(uVar30 != uVar29);
              }
            }
            if (uVar27 == 0) {
              puVar36[0x22] = (int)uStack_158;
              puVar36[0x21] = 4;
              if (*(longlong *)(puVar36 + 0x2a) == 0) {
                auVar9._8_8_ = 0;
                auVar9._0_8_ = (longlong)(int)uStack_158;
                uVar23 = SUB168(ZEXT816(0x120) * auVar9,0);
                if (SUB168(ZEXT816(0x120) * auVar9,8) != 0) {
                  uVar23 = uVar20;
                }
                lVar21 = thunk_FUN_1809e82b0(uVar23);
                *(longlong *)(puVar36 + 0x2a) = lVar21;
                if (lVar21 == 0) {
                  cVar15 = FUN_1805a2010(4);
                  if (cVar15 != '\0') {
                    uVar16 = GetLastError();
                    uVar20 = FUN_1805a1d70();
                    puStack_2b0 = (undefined4 *)CONCAT44(puStack_2b0._4_4_,uVar16);
                    puStack_2b8 = (undefined8 *)CONCAT44(puStack_2b8._4_4_,0x5bee);
                    FUN_1805a1f10(uVar20,4,&UNK_180ad2ac0,&UNK_180ad2720);
                  }
                  iStack_284 = 0xe0000002;
                  uStack_22c = 0xe0000002;
                    /* WARNING: Subroutine does not return */
                  _CxxThrowException(&uStack_22c,&UNK_180c23c00);
                }
                memset(lVar21,0,(longlong)(int)puVar36[0x22] * 0x120);
              }
              iStack_264 = iVar31;
              if (puVar36[0x22] != 0) {
                *puVar36 = 1;
              }
              for (; piVar32 = piStack_278, param_2 = lStack_270, iVar19 = iStack_27c,
                  iStack_264 < (int)puVar36[0x22]; iStack_264 = iStack_264 + 1) {
                if (cStack_280 == '\0') {
                  lVar21 = (longlong)iStack_264 * 0x120;
                  *(undefined8 *)(lVar21 + *(longlong *)(puVar36 + 0x2a)) = 0;
                  *(undefined4 *)(lVar21 + 8 + *(longlong *)(puVar36 + 0x2a)) = 0;
                  *(undefined4 *)(lVar21 + 0x110 + *(longlong *)(puVar36 + 0x2a)) = 1;
                  *(undefined4 *)(lVar21 + 0x114 + *(longlong *)(puVar36 + 0x2a)) = 0;
                  cVar15 = FUN_1805a2010(1);
                  if (cVar15 != '\0') {
                    uVar20 = FUN_1805a1d70();
                    puStack_2b8 = (undefined8 *)CONCAT44(puStack_2b8._4_4_,0x5c3a);
                    puStack_2b0 = puVar36 + 1;
                    FUN_1805a1f10(uVar20,1,&UNK_180ad2d20,&UNK_180ad2720);
                  }
                }
                else {
                  lVar21 = (longlong)iStack_264;
                  uStack_e0 = 0xf;
                  uStack_e8 = 0;
                  auStack_f8[0]._0_1_ = 0;
                  FUN_180591340(auStack_f8,*(undefined8 *)(lStack_150 + 8 + lVar21 * 8),0,
                                0xffffffffffffffff);
                  puStack_208 = (undefined8 *)0x0;
                  puStack_200 = (undefined8 *)0x0;
                  uStack_1f8 = 0;
                  puVar22 = auStack_f8;
                  if (0xf < uStack_e0) {
                    puVar22 = (undefined8 *)CONCAT71(auStack_f8[0]._1_7_,(undefined1)auStack_f8[0]);
                  }
                  iStack_284 = FUN_18063d5d0(puVar22,&UNK_180aac794,&puStack_208);
                  puVar24 = puStack_200;
                  puVar22 = puStack_208;
                  if ((iStack_284 != 0) ||
                     (uVar30 = (longlong)puStack_200 - (longlong)puStack_208 >> 5, uVar30 < 2)) {
                    cVar15 = FUN_1805a2010(4);
                    if (cVar15 != '\0') {
                      puVar22 = auStack_f8;
                      if (0xf < uStack_e0) {
                        puVar22 = (undefined8 *)
                                  CONCAT71(auStack_f8[0]._1_7_,(undefined1)auStack_f8[0]);
                      }
                      uVar20 = FUN_1805a1d70();
                      puStack_2b8 = (undefined8 *)CONCAT44(puStack_2b8._4_4_,0x5c10);
                      puStack_2b0 = (undefined4 *)puVar22;
                      FUN_1805a1f10(uVar20,4,&UNK_180ad2a98,&UNK_180ad2720);
                    }
                    iStack_284 = 0xe0000017;
                    uStack_224 = 0xe0000017;
                    /* WARNING: Subroutine does not return */
                    _CxxThrowException(&uStack_224,&UNK_180c23c00);
                  }
                  puVar25 = puStack_208;
                  if (0xf < (ulonglong)puStack_208[3]) {
                    puVar25 = (undefined8 *)*puStack_208;
                  }
                  uVar16 = atoi(puVar25);
                  puVar25 = puVar22 + 4;
                  if (0xf < (ulonglong)puVar22[7]) {
                    puVar25 = (undefined8 *)*puVar25;
                  }
                  lVar21 = lVar21 * 0x120;
                  sscanf_s(puVar25,&UNK_180ad2c30,*(longlong *)(puVar36 + 0x2a) + lVar21);
                  if (*(longlong *)(lVar21 + *(longlong *)(puVar36 + 0x2a)) == 0) {
                    cVar15 = FUN_1805a2010(4);
                    if (cVar15 != '\0') {
                      uVar20 = FUN_1805a1d70();
                      puStack_2b8 = (undefined8 *)CONCAT44(puStack_2b8._4_4_,0x5c19);
                      FUN_1805a1f10(uVar20,4,&UNK_180ad2c08,&UNK_180ad2720);
                    }
                    iStack_284 = 0xe0000002;
                    uStack_244 = 0xe0000002;
                    /* WARNING: Subroutine does not return */
                    _CxxThrowException(&uStack_244,&UNK_180c23c00);
                  }
                  *(undefined4 *)(lVar21 + 8 + *(longlong *)(puVar36 + 0x2a)) = uVar16;
                  *(undefined4 *)(lVar21 + 0x114 + *(longlong *)(puVar36 + 0x2a)) = 1;
                  if (uVar30 < 3) {
                    memset(*(longlong *)(puVar36 + 0x2a) + 0xc + lVar21);
                  }
                  else {
                    puStack_2b8 = puVar22 + 8;
                    if (0xf < (ulonglong)puVar22[0xb]) {
                      puStack_2b8 = (undefined8 *)*puStack_2b8;
                    }
                    _snprintf_s(*(longlong *)(puVar36 + 0x2a) + 0xc + lVar21,0x104,0x103,
                                &UNK_180aad270);
                    if ((0x103 < (ulonglong)puVar22[10]) &&
                       (cVar15 = FUN_1805a2010(3), cVar15 != '\0')) {
                      uVar20 = FUN_1805a1d70();
                      puStack_2b8 = (undefined8 *)CONCAT44(puStack_2b8._4_4_,0x5c26);
                      FUN_1805a1f10(uVar20,3,&UNK_180ad2bd0,&UNK_180ad2720);
                    }
                  }
                  *(undefined4 *)(lVar21 + 0x110 + *(longlong *)(puVar36 + 0x2a)) = 2;
                  cVar15 = FUN_1805a2010(1);
                  puVar25 = puVar22;
                  if (cVar15 != '\0') {
                    lVar37 = *(longlong *)(puVar36 + 0x2a);
                    uVar20 = FUN_1805a1d70();
                    puStack_2a0 = (undefined8 *)
                                  CONCAT44(puStack_2a0._4_4_,*(undefined4 *)(lVar21 + 8 + lVar37));
                    puStack_2b8 = (undefined8 *)CONCAT44(puStack_2b8._4_4_,0x5c30);
                    puStack_2b0 = puVar36 + 1;
                    puStack_2a8 = (undefined8 *)(lVar37 + 0xc + lVar21);
                    FUN_1805a1f10(uVar20,1,&UNK_180ad2b60,&UNK_180ad2720);
                  }
                  for (; puVar25 != puVar24; puVar25 = puVar25 + 4) {
                    if (0xf < (ulonglong)puVar25[3]) {
                      __3_YAXPEAX_Z(*puVar25);
                    }
                    puVar25[3] = 0xf;
                    puVar25[2] = 0;
                    *(undefined1 *)puVar25 = 0;
                  }
                  __3_YAXPEAX_Z(puVar22);
                  if (0xf < uStack_e0) {
                    __3_YAXPEAX_Z(CONCAT71(auStack_f8[0]._1_7_,(undefined1)auStack_f8[0]));
                  }
                }
              }
            }
            else {
              uVar27 = -(uint)(DAT_180ab4930 != '\0') & 8;
              uVar29 = (ulonglong)uVar27;
              uVar34 = (ulonglong)uVar27;
              if (uVar30 < uVar29) {
                uVar34 = uVar30;
              }
              if (uVar34 != 0) {
                uVar18 = memcmp();
              }
              if (uVar18 == 0) {
                if (uVar30 < uVar29) {
                  uVar18 = 0xffffffff;
                }
                else {
                  uVar18 = (uint)(uVar30 != uVar29);
                }
              }
              if (uVar18 == 0) {
                puVar36[0x22] = (int)uStack_158;
                puVar36[0x21] = 5;
                if (*(longlong *)(puVar36 + 0x2c) == 0) {
                  auVar10._8_8_ = 0;
                  auVar10._0_8_ = (longlong)(int)uStack_158;
                  uVar23 = SUB168(ZEXT816(0x120) * auVar10,0);
                  if (SUB168(ZEXT816(0x120) * auVar10,8) != 0) {
                    uVar23 = uVar20;
                  }
                  lVar21 = thunk_FUN_1809e82b0(uVar23);
                  *(longlong *)(puVar36 + 0x2c) = lVar21;
                  if (lVar21 == 0) {
                    cVar15 = FUN_1805a2010(4);
                    if (cVar15 != '\0') {
                      uVar16 = GetLastError();
                      uVar20 = FUN_1805a1d70();
                      puStack_2b0 = (undefined4 *)CONCAT44(puStack_2b0._4_4_,uVar16);
                      puStack_2b8 = (undefined8 *)CONCAT44(puStack_2b8._4_4_,0x5c48);
                      FUN_1805a1f10(uVar20,4,&UNK_180ad2ce8,&UNK_180ad2720);
                    }
                    iStack_284 = 0xe0000002;
                    uStack_23c = 0xe0000002;
                    /* WARNING: Subroutine does not return */
                    _CxxThrowException(&uStack_23c,&UNK_180c23c00);
                  }
                  memset(lVar21,0,(longlong)(int)puVar36[0x22] * 0x120);
                }
                if (puVar36[0x22] != 0) {
                  *puVar36 = 1;
                }
                for (; piVar32 = piStack_278, param_2 = lStack_270, iVar19 = iStack_27c,
                    iVar31 < (int)puVar36[0x22]; iVar31 = iVar31 + 1) {
                  if (cStack_280 == '\0') {
                    lVar21 = (longlong)iVar31 * 0x120;
                    *(undefined8 *)(lVar21 + *(longlong *)(puVar36 + 0x2c)) = 0;
                    *(undefined4 *)(*(longlong *)(puVar36 + 0x2c) + 8 + lVar21) = 0;
                    *(undefined4 *)(*(longlong *)(puVar36 + 0x2c) + 0x110 + lVar21) = 1;
                    *(undefined4 *)(*(longlong *)(puVar36 + 0x2c) + 0x114 + lVar21) = 0;
                    cVar15 = FUN_1805a2010(1);
                    if (cVar15 != '\0') {
                      uVar20 = FUN_1805a1d70();
                      puStack_2b8 = (undefined8 *)CONCAT44(puStack_2b8._4_4_,0x5c83);
                      puStack_2b0 = puVar36 + 1;
                      FUN_1805a1f10(uVar20,1,&UNK_180ad2c40,&UNK_180ad2720);
                    }
                  }
                  else {
                    uStack_e0 = 0xf;
                    uStack_e8 = 0;
                    auStack_f8[0]._0_1_ = 0;
                    FUN_180591340(auStack_f8,*(undefined8 *)(lStack_150 + 8 + (longlong)iVar31 * 8),
                                  0,0xffffffffffffffff);
                    puStack_208 = (undefined8 *)0x0;
                    puStack_200 = (undefined8 *)0x0;
                    uStack_1f8 = 0;
                    FUN_180635d70(&puStack_208);
                    puVar22 = auStack_f8;
                    if (0xf < uStack_e0) {
                      puVar22 = (undefined8 *)
                                CONCAT71(auStack_f8[0]._1_7_,(undefined1)auStack_f8[0]);
                    }
                    iStack_284 = FUN_18063d5d0(puVar22,&UNK_180aac794,&puStack_208);
                    puVar24 = puStack_200;
                    puVar22 = puStack_208;
                    if (((iStack_284 != 0) || (puStack_208 == puStack_200)) ||
                       ((ulonglong)((longlong)puStack_200 - (longlong)puStack_208 >> 5) < 2)) {
                      cVar15 = FUN_1805a2010(4);
                      if (cVar15 != '\0') {
                        puVar22 = auStack_f8;
                        if (0xf < uStack_e0) {
                          puVar22 = (undefined8 *)
                                    CONCAT71(auStack_f8[0]._1_7_,(undefined1)auStack_f8[0]);
                        }
                        uVar20 = FUN_1805a1d70();
                        puStack_2b8 = (undefined8 *)CONCAT44(puStack_2b8._4_4_,0x5c5f);
                        puStack_2b0 = (undefined4 *)puVar22;
                        FUN_1805a1f10(uVar20,4,&UNK_180ad2a98,&UNK_180ad2720);
                      }
                      iStack_284 = 0xe0000017;
                      uStack_234 = 0xe0000017;
                    /* WARNING: Subroutine does not return */
                      _CxxThrowException(&uStack_234,&UNK_180c23c00);
                    }
                    puVar25 = puStack_208;
                    if (0xf < (ulonglong)puStack_208[3]) {
                      puVar25 = (undefined8 *)*puStack_208;
                    }
                    iVar19 = atoi(puVar25);
                    puVar25 = puVar22 + 4;
                    if (0xf < (ulonglong)puVar22[7]) {
                      puVar25 = (undefined8 *)*puVar25;
                    }
                    lVar21 = (longlong)iVar31 * 0x120;
                    sscanf_s(puVar25);
                    if (*(longlong *)(*(longlong *)(puVar36 + 0x2c) + lVar21) == 0) {
                      cVar15 = FUN_1805a2010(4);
                      if (cVar15 != '\0') {
                        uVar20 = FUN_1805a1d70();
                        puStack_2b8 = (undefined8 *)CONCAT44(puStack_2b8._4_4_,0x5c67);
                        FUN_1805a1f10(uVar20,4,&UNK_180ad2c08,&UNK_180ad2720);
                      }
                      iStack_284 = 0xe0000002;
                      uStack_21c = 0xe0000002;
                    /* WARNING: Subroutine does not return */
                      _CxxThrowException(&uStack_21c,&UNK_180c23c00);
                    }
                    *(int *)(*(longlong *)(puVar36 + 0x2c) + 8 + lVar21) = iVar19;
                    *(undefined4 *)(*(longlong *)(puVar36 + 0x2c) + 0x110 + lVar21) = 1;
                    *(undefined4 *)(*(longlong *)(puVar36 + 0x2c) + 0x114 + lVar21) = 1;
                    cVar15 = FUN_1805a2010(1);
                    puVar25 = puVar22;
                    if (cVar15 != '\0') {
                      uVar20 = FUN_1805a1d70();
                      puStack_2b8 = (undefined8 *)CONCAT44(puStack_2b8._4_4_,0x5c6e);
                      puStack_2b0 = puVar36 + 1;
                      puStack_2a8 = (undefined8 *)(longlong)iVar19;
                      FUN_1805a1f10(uVar20,1,&UNK_180ad2c90,&UNK_180ad2720);
                    }
                    do {
                      if (0xf < (ulonglong)puVar25[3]) {
                        __3_YAXPEAX_Z(*puVar25);
                      }
                      puVar25[3] = 0xf;
                      puVar25[2] = 0;
                      *(undefined1 *)puVar25 = 0;
                      puVar25 = puVar25 + 4;
                    } while (puVar25 != puVar24);
                    __3_YAXPEAX_Z(puVar22);
                    if (0xf < uStack_e0) {
                      __3_YAXPEAX_Z(CONCAT71(auStack_f8[0]._1_7_,(undefined1)auStack_f8[0]));
                    }
                  }
                }
              }
              else {
                puStack_2b8 = (undefined8 *)(ulonglong)(-(uint)(DAT_180ab4950 != '\0') & 9);
                iVar19 = FUN_1805b4640(auStack_d8,0,uVar30,&DAT_180ab4950);
                if (iVar19 == 0) {
                  puVar36[0x22] = (int)uStack_130;
                  puVar36[0x21] = 6;
                  if (*(longlong *)(puVar36 + 0x2e) == 0) {
                    auVar11._8_8_ = 0;
                    auVar11._0_8_ = (longlong)(int)uStack_130;
                    uVar23 = SUB168(ZEXT816(0x20) * auVar11,0);
                    if (SUB168(ZEXT816(0x20) * auVar11,8) != 0) {
                      uVar23 = uVar20;
                    }
                    lVar21 = thunk_FUN_1809e82b0(uVar23);
                    *(longlong *)(puVar36 + 0x2e) = lVar21;
                    if (lVar21 == 0) {
                      cVar15 = FUN_1805a2010(4);
                      if (cVar15 != '\0') {
                        uVar16 = GetLastError();
                        uVar20 = FUN_1805a1d70();
                        puStack_2b0 = (undefined4 *)CONCAT44(puStack_2b0._4_4_,uVar16);
                        puStack_2b8 = (undefined8 *)CONCAT44(puStack_2b8._4_4_,0x5c91);
                        FUN_1805a1f10(uVar20,4,&UNK_180ad2e08,&UNK_180ad2720);
                      }
                      iStack_284 = 0xe0000002;
                      uStack_20c = 0xe0000002;
                    /* WARNING: Subroutine does not return */
                      _CxxThrowException(&uStack_20c,&UNK_180c23c00);
                    }
                    memset(lVar21,0,(longlong)(int)puVar36[0x22] << 5);
                  }
                  if (puVar36[0x22] != 0) {
                    *puVar36 = 1;
                  }
                  for (; piVar32 = piStack_278, param_2 = lStack_270, iVar19 = iStack_27c,
                      iVar31 < (int)puVar36[0x22]; iVar31 = iVar31 + 1) {
                    lVar21 = (longlong)iVar31 * 8;
                    uVar27 = *(uint *)(*(longlong *)(lStack_128 + 8 + lVar21) + 0x20);
                    puVar22 = (undefined8 *)(ulonglong)uVar27;
                    uVar20 = thunk_FUN_1809e82b0(puVar22,0x180c9ea09);
                    lVar37 = (longlong)iVar31 * 0x20;
                    *(undefined8 *)(lVar37 + *(longlong *)(puVar36 + 0x2e)) = uVar20;
                    if (*(longlong *)(lVar37 + *(longlong *)(puVar36 + 0x2e)) == 0) {
                      cVar15 = FUN_1805a2010(4);
                      if (cVar15 != '\0') {
                        uVar20 = FUN_1805a1d70();
                        puStack_2b8 = (undefined8 *)CONCAT44(puStack_2b8._4_4_,0x5ca2);
                        FUN_1805a1f10(uVar20,4,&UNK_180ad2dd8,&UNK_180ad2720);
                      }
                      iStack_284 = 0xe0000002;
                      uStack_260 = 0xe0000002;
                    /* WARNING: Subroutine does not return */
                      _CxxThrowException(&uStack_260,&UNK_180c23c00);
                    }
                    plVar26 = *(longlong **)(*(longlong *)(lStack_128 + 8 + lVar21) + 0x18);
                    if (0xf < (ulonglong)plVar26[3]) {
                      plVar26 = (longlong *)*plVar26;
                    }
                    memcpy(*(longlong *)(lVar37 + *(longlong *)(puVar36 + 0x2e)),plVar26,puVar22);
                    *(uint *)(*(longlong *)(puVar36 + 0x2e) + 8 + lVar37) = uVar27;
                    cVar15 = FUN_1805a2010(1);
                    if (cVar15 != '\0') {
                      uVar20 = FUN_1805a1d70();
                      puStack_2b8 = (undefined8 *)CONCAT44(puStack_2b8._4_4_,0x5ca8);
                      puStack_2b0 = puVar36 + 1;
                      puStack_2a8 = puVar22;
                      FUN_1805a1f10(uVar20,1,&UNK_180ad2d98,&UNK_180ad2720);
                    }
                  }
                }
                else {
                  puStack_2b8 = (undefined8 *)(ulonglong)(-(uint)(DAT_180ab4970 != '\0') & 9);
                  iVar19 = FUN_1805b4640(auStack_d8,0,uStack_c8,&DAT_180ab4970);
                  if (iVar19 == 0) {
                    puVar36[0x22] = (int)uStack_158;
                    puVar36[0x21] = 4;
                    if (*(longlong *)(puVar36 + 0x2a) == 0) {
                      auVar12._8_8_ = 0;
                      auVar12._0_8_ = (longlong)(int)uStack_158;
                      uVar23 = SUB168(ZEXT816(0x120) * auVar12,0);
                      if (SUB168(ZEXT816(0x120) * auVar12,8) != 0) {
                        uVar23 = uVar20;
                      }
                      lVar21 = thunk_FUN_1809e82b0(uVar23);
                      *(longlong *)(puVar36 + 0x2a) = lVar21;
                      if (lVar21 == 0) {
                        cVar15 = FUN_1805a2010(4);
                        if (cVar15 != '\0') {
                          uVar20 = FUN_1805a1d70();
                          puStack_2b8 = (undefined8 *)CONCAT44(puStack_2b8._4_4_,0x5cb5);
                          FUN_1805a1f10(uVar20,4,&UNK_180ad2d70,&UNK_180ad2720);
                        }
                        FUN_18063fd30();
                        iStack_284 = 0xe0000002;
                        uStack_228 = 0xe0000002;
                    /* WARNING: Subroutine does not return */
                        _CxxThrowException(&uStack_228,&UNK_180c23c00);
                      }
                      memset(lVar21,0,(longlong)(int)puVar36[0x22] * 0x120);
                    }
                    if (puVar36[0x22] != 0) {
                      *puVar36 = 1;
                    }
                    for (; uVar20 = uStack_198, piVar32 = piStack_278, param_2 = lStack_270,
                        iVar19 = iStack_27c, iVar31 < (int)puVar36[0x22]; iVar31 = iVar31 + 1) {
                      plVar26 = *(longlong **)(lStack_150 + 8 + (longlong)iVar31 * 8);
                      uStack_1c8 = 0;
                      if (0xf < (ulonglong)plVar26[3]) {
                        plVar26 = (longlong *)*plVar26;
                      }
                      lVar21 = (longlong)iVar31 * 0x120;
                      FUN_1806dc130(uStack_198,*(longlong *)(puVar36 + 0x2a) + lVar21,&uStack_1c8,
                                    plVar26);
                      FUN_1806dc240(uVar20);
                      if (*(longlong *)(*(longlong *)(puVar36 + 0x2a) + lVar21) == 0) {
                        cVar15 = FUN_1805a2010(4);
                        if (cVar15 != '\0') {
                          uVar20 = FUN_1805a1d70();
                          puStack_2b8 = (undefined8 *)CONCAT44(puStack_2b8._4_4_,0x5ccd);
                          FUN_1805a1f10(uVar20,4,&UNK_180ad2f10,&UNK_180ad2720);
                        }
                        iStack_284 = 0xe0000002;
                        uStack_218 = 0xe0000002;
                    /* WARNING: Subroutine does not return */
                        _CxxThrowException(&uStack_218,&UNK_180c23c00);
                      }
                      *(undefined4 *)(*(longlong *)(puVar36 + 0x2a) + 8 + lVar21) =
                           (undefined4)uStack_1c8;
                      *(undefined4 *)(*(longlong *)(puVar36 + 0x2a) + 0x110 + lVar21) = 1;
                      cVar15 = FUN_1805a2010(2);
                      if (cVar15 != '\0') {
                        lVar37 = *(longlong *)(puVar36 + 0x2a);
                        uVar20 = FUN_1805a1d70();
                        puStack_2a8 = (undefined8 *)
                                      CONCAT44(puStack_2a8._4_4_,
                                               *(undefined4 *)(lVar37 + 8 + lVar21));
                        puStack_2b8 = (undefined8 *)CONCAT44(puStack_2b8._4_4_,0x5cd4);
                        puStack_2b0 = puVar36 + 1;
                        FUN_1805a1f10(uVar20,2,&UNK_180ad2ec0,&UNK_180ad2720);
                      }
                    }
                  }
                  else {
                    puStack_2b8 = (undefined8 *)(ulonglong)(-(uint)(DAT_180ab4960 != '\0') & 0xc);
                    iVar19 = FUN_1805b4640(auStack_d8,0,uStack_c8,&DAT_180ab4960);
                    if (iVar19 == 0) {
                      puVar36[0x22] = (int)uStack_158;
                      puVar36[0x21] = 5;
                      if (*(longlong *)(puVar36 + 0x2c) == 0) {
                        auVar13._8_8_ = 0;
                        auVar13._0_8_ = (longlong)(int)uStack_158;
                        uVar23 = SUB168(ZEXT816(0x120) * auVar13,0);
                        if (SUB168(ZEXT816(0x120) * auVar13,8) != 0) {
                          uVar23 = uVar20;
                        }
                        lVar21 = thunk_FUN_1809e82b0(uVar23);
                        *(longlong *)(puVar36 + 0x2c) = lVar21;
                        if (lVar21 == 0) {
                          cVar15 = FUN_1805a2010(4);
                          if (cVar15 != '\0') {
                            uVar16 = GetLastError();
                            uVar20 = FUN_1805a1d70();
                            puStack_2b0 = (undefined4 *)CONCAT44(puStack_2b0._4_4_,uVar16);
                            puStack_2b8 = (undefined8 *)CONCAT44(puStack_2b8._4_4_,0x5ce1);
                            FUN_1805a1f10(uVar20,4,&UNK_180ad2ce8,&UNK_180ad2720);
                          }
                          iStack_284 = 0xe0000002;
                          uStack_220 = 0xe0000002;
                    /* WARNING: Subroutine does not return */
                          _CxxThrowException(&uStack_220,&UNK_180c23c00);
                        }
                        memset(lVar21,0,(longlong)(int)puVar36[0x22] * 0x120);
                      }
                      if (puVar36[0x22] != 0) {
                        *puVar36 = 1;
                      }
                      for (; piVar32 = piStack_278, param_2 = lStack_270, iVar19 = iStack_27c,
                          iVar31 < (int)puVar36[0x22]; iVar31 = iVar31 + 1) {
                        if (cStack_280 == '\0') {
                          lVar21 = (longlong)iVar31 * 0x120;
                          *(undefined8 *)(lVar21 + *(longlong *)(puVar36 + 0x2c)) = 0;
                          *(undefined4 *)(*(longlong *)(puVar36 + 0x2c) + 8 + lVar21) = 0;
                          *(undefined4 *)(*(longlong *)(puVar36 + 0x2c) + 0x110 + lVar21) = 1;
                          *(undefined4 *)(*(longlong *)(puVar36 + 0x2c) + 0x114 + lVar21) = 0;
                          cVar15 = FUN_1805a2010(1);
                          if (cVar15 != '\0') {
                            uVar20 = FUN_1805a1d70();
                            puStack_2b8 = (undefined8 *)CONCAT44(puStack_2b8._4_4_,0x5d05);
                            puStack_2b0 = puVar36 + 1;
                            FUN_1805a1f10(uVar20,1,&UNK_180ad3000,&UNK_180ad2720);
                          }
                        }
                        else {
                          lVar21 = (longlong)iVar31;
                          uVar20 = *(undefined8 *)
                                    (*(longlong *)(lStack_150 + 8 + lVar21 * 8) + 0x10);
                          uVar23 = thunk_FUN_1809e82b0(uVar20,0x180c9ea09);
                          lVar37 = lVar21 * 0x120;
                          *(undefined8 *)(lVar37 + *(longlong *)(puVar36 + 0x2c)) = uVar23;
                          if (*(longlong *)(lVar37 + *(longlong *)(puVar36 + 0x2c)) == 0) {
                            cVar15 = FUN_1805a2010(4);
                            if (cVar15 != '\0') {
                              uVar20 = FUN_1805a1d70();
                              puStack_2b8 = (undefined8 *)CONCAT44(puStack_2b8._4_4_,0x5cf4);
                              FUN_1805a1f10(uVar20,4,&UNK_180ad2e88,&UNK_180ad2720);
                            }
                            iStack_284 = 0xe0000002;
                            uStack_25c = 0xe0000002;
                    /* WARNING: Subroutine does not return */
                            _CxxThrowException(&uStack_25c,&UNK_180c23c00);
                          }
                          plVar26 = *(longlong **)(lStack_150 + 8 + lVar21 * 8);
                          if (0xf < (ulonglong)plVar26[3]) {
                            plVar26 = (longlong *)*plVar26;
                          }
                          memcpy(*(longlong *)(lVar37 + *(longlong *)(puVar36 + 0x2c)),plVar26,
                                 uVar20);
                          *(int *)(*(longlong *)(puVar36 + 0x2c) + 8 + lVar37) = (int)uVar20;
                          *(undefined4 *)(*(longlong *)(puVar36 + 0x2c) + 0x110 + lVar37) = 1;
                          *(undefined4 *)(*(longlong *)(puVar36 + 0x2c) + 0x114 + lVar37) = 0;
                          cVar15 = FUN_1805a2010(2);
                          if (cVar15 != '\0') {
                            uVar20 = FUN_1805a1d70();
                            puStack_2b8 = (undefined8 *)CONCAT44(puStack_2b8._4_4_,0x5cfc);
                            puStack_2b0 = puVar36 + 1;
                            FUN_1805a1f10(uVar20,2,&UNK_180ad2e40,&UNK_180ad2720);
                          }
                        }
                      }
                    }
                    else {
                      puStack_2b8 = (undefined8 *)(ulonglong)(-(uint)(DAT_180ab4940 != '\0') & 9);
                      iVar19 = FUN_1805b4640(auStack_d8,0,uStack_c8,&DAT_180ab4940);
                      if (iVar19 != 0) {
                        cVar15 = FUN_1805a2010(4);
                        if (cVar15 != '\0') {
                          uVar20 = FUN_1805a1d70();
                          puStack_2b8 = (undefined8 *)CONCAT44(puStack_2b8._4_4_,0x5d3d);
                          FUN_1805a1f10(uVar20,4,&UNK_180ad3128,&UNK_180ad2720);
                        }
                        puVar36[0x21] = 0;
                        iStack_284 = 0xe0000001;
                        uStack_248 = 0xe0000001;
                    /* WARNING: Subroutine does not return */
                        _CxxThrowException(&uStack_248,&UNK_180c23c00);
                      }
                      puVar36[0x22] = (int)uStack_158;
                      puVar36[0x21] = 4;
                      if (*(longlong *)(puVar36 + 0x2a) == 0) {
                        auVar14._8_8_ = 0;
                        auVar14._0_8_ = (longlong)(int)uStack_158;
                        uVar23 = SUB168(ZEXT816(0x120) * auVar14,0);
                        if (SUB168(ZEXT816(0x120) * auVar14,8) != 0) {
                          uVar23 = uVar20;
                        }
                        lVar21 = thunk_FUN_1809e82b0(uVar23);
                        *(longlong *)(puVar36 + 0x2a) = lVar21;
                        if (lVar21 == 0) {
                          cVar15 = FUN_1805a2010(4);
                          if (cVar15 != '\0') {
                            uVar16 = GetLastError();
                            uVar20 = FUN_1805a1d70();
                            puStack_2b0 = (undefined4 *)CONCAT44(puStack_2b0._4_4_,uVar16);
                            puStack_2b8 = (undefined8 *)CONCAT44(puStack_2b8._4_4_,0x5d13);
                            FUN_1805a1f10(uVar20,4,&UNK_180ad2ac0,&UNK_180ad2720);
                          }
                          iStack_284 = 0xe0000002;
                          uStack_258 = 0xe0000002;
                    /* WARNING: Subroutine does not return */
                          _CxxThrowException(&uStack_258,&UNK_180c23c00);
                        }
                        memset(lVar21,0,(longlong)(int)puVar36[0x22] * 0x120);
                      }
                      if (puVar36[0x22] != 0) {
                        *puVar36 = 1;
                      }
                      for (; piVar32 = piStack_278, param_2 = lStack_270, iVar19 = iStack_27c,
                          iVar31 < (int)puVar36[0x22]; iVar31 = iVar31 + 1) {
                        if (cStack_280 == '\0') {
                          lVar21 = (longlong)iVar31 * 0x120;
                          *(undefined8 *)(lVar21 + *(longlong *)(puVar36 + 0x2a)) = 0;
                          *(undefined4 *)(*(longlong *)(puVar36 + 0x2a) + 8 + lVar21) = 0;
                          *(undefined4 *)(*(longlong *)(puVar36 + 0x2a) + 0x110 + lVar21) = 1;
                          *(undefined4 *)(*(longlong *)(puVar36 + 0x2a) + 0x114 + lVar21) = 0;
                          cVar15 = FUN_1805a2010(1);
                          if (cVar15 != '\0') {
                            uVar20 = FUN_1805a1d70();
                            puStack_2b8 = (undefined8 *)CONCAT44(puStack_2b8._4_4_,0x5d37);
                            puStack_2b0 = puVar36 + 1;
                            FUN_1805a1f10(uVar20,1,&UNK_180ad2f40,&UNK_180ad2720);
                          }
                        }
                        else {
                          lVar21 = (longlong)iVar31;
                          uVar20 = *(undefined8 *)
                                    (*(longlong *)(lStack_150 + 8 + lVar21 * 8) + 0x10);
                          uVar23 = thunk_FUN_1809e82b0(uVar20,0x180c9ea09);
                          lVar37 = lVar21 * 0x120;
                          *(undefined8 *)(lVar37 + *(longlong *)(puVar36 + 0x2a)) = uVar23;
                          if (*(longlong *)(lVar37 + *(longlong *)(puVar36 + 0x2a)) == 0) {
                            cVar15 = FUN_1805a2010(4);
                            if (cVar15 != '\0') {
                              uVar20 = FUN_1805a1d70();
                              puStack_2b8 = (undefined8 *)CONCAT44(puStack_2b8._4_4_,0x5d26);
                              FUN_1805a1f10(uVar20,4,&UNK_180ad2fc8,&UNK_180ad2720);
                            }
                            iStack_284 = 0xe0000002;
                            uStack_250 = 0xe0000002;
                    /* WARNING: Subroutine does not return */
                            _CxxThrowException(&uStack_250,&UNK_180c23c00);
                          }
                          plVar26 = *(longlong **)(lStack_150 + 8 + lVar21 * 8);
                          if (0xf < (ulonglong)plVar26[3]) {
                            plVar26 = (longlong *)*plVar26;
                          }
                          memcpy(*(longlong *)(lVar37 + *(longlong *)(puVar36 + 0x2a)),plVar26,
                                 uVar20);
                          *(int *)(*(longlong *)(puVar36 + 0x2a) + 8 + lVar37) = (int)uVar20;
                          *(undefined4 *)(*(longlong *)(puVar36 + 0x2a) + 0x110 + lVar37) = 1;
                          *(undefined4 *)(*(longlong *)(puVar36 + 0x2a) + 0x114 + lVar37) = 0;
                          cVar15 = FUN_1805a2010(2);
                          if (cVar15 != '\0') {
                            uVar20 = FUN_1805a1d70();
                            puStack_2b8 = (undefined8 *)CONCAT44(puStack_2b8._4_4_,0x5d2e);
                            puStack_2b0 = puVar36 + 1;
                            FUN_1805a1f10(uVar20,2,&UNK_180ad2f88,&UNK_180ad2720);
                          }
                        }
                      }
                    }
                  }
                }
              }
            }
          }
        }
      }
    }
    else {
      puStack_2b8 = (undefined8 *)0xe;
      iVar17 = FUN_1805b4640(puStack_170,0,puStack_170[2],&UNK_180ad28b0);
      if (iVar17 != 0) goto LAB_1806ba1ad;
      cStack_288 = '\0';
    }
    iStack_268 = iStack_268 + 1;
  } while( true );
}

