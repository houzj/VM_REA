
/* WARNING: Type propagation algorithm not settling */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_18069df50(undefined8 *param_1,uint param_2,uint param_3,undefined4 param_4,
                  undefined8 *param_5,char param_6,undefined1 param_7,char param_8,char param_9,
                  undefined4 param_10)

{
  int *piVar1;
  uint uVar2;
  longlong *plVar3;
  longlong *plVar4;
  longlong *plVar5;
  undefined1 auVar6 [16];
  bool bVar7;
  longlong *plVar8;
  char cVar9;
  int iVar10;
  int iVar11;
  uint uVar12;
  undefined8 uVar13;
  undefined8 *puVar14;
  longlong lVar15;
  longlong lVar16;
  undefined8 **ppuVar17;
  undefined8 *puVar18;
  undefined8 *puVar19;
  undefined1 *puVar20;
  undefined8 *puVar21;
  undefined8 *puVar22;
  longlong *plVar23;
  ulonglong uVar24;
  undefined *puVar25;
  int iVar26;
  bool bVar27;
  undefined4 uVar28;
  undefined4 extraout_XMM0_Da;
  undefined1 auStack_258 [32];
  undefined8 *puStack_238;
  ulonglong uStack_230;
  undefined8 uStack_228;
  undefined4 uStack_220;
  uint auStack_218 [2];
  int iStack_210;
  undefined8 *puStack_208;
  ulonglong uStack_200;
  ulonglong uStack_1f8;
  undefined8 *puStack_1f0;
  undefined8 *puStack_1e8;
  undefined8 uStack_1e0;
  int iStack_1d8;
  int aiStack_1d4 [3];
  undefined8 *puStack_1c8;
  int aiStack_1c0 [2];
  undefined8 uStack_1b8;
  int iStack_1b0;
  undefined4 uStack_1ac;
  longlong lStack_1a8;
  undefined8 uStack_1a0;
  undefined8 uStack_198;
  undefined8 uStack_190;
  undefined8 *puStack_188;
  uint *puStack_180;
  undefined8 *puStack_178;
  undefined8 *puStack_170;
  uint *puStack_168;
  ulonglong *puStack_160;
  undefined8 *puStack_158;
  undefined8 *puStack_150;
  undefined8 *puStack_148;
  undefined8 *puStack_140;
  ulonglong *puStack_138;
  undefined8 *puStack_130;
  uint *puStack_128;
  undefined8 *puStack_120;
  undefined4 uStack_118;
  undefined4 uStack_114;
  undefined8 *puStack_110;
  undefined8 uStack_108;
  undefined8 uStack_100;
  undefined8 uStack_f8;
  ulonglong uStack_f0;
  undefined8 *puStack_e8;
  undefined8 *puStack_e0;
  undefined8 *puStack_d8;
  ulonglong *puStack_d0;
  undefined8 uStack_c8;
  undefined1 auStack_c0 [16];
  undefined1 auStack_b0 [16];
  undefined1 auStack_a0 [16];
  undefined1 auStack_90 [16];
  undefined8 **ppuStack_80;
  undefined8 uStack_78;
  undefined8 uStack_70;
  undefined8 uStack_68;
  undefined8 auStack_60 [2];
  undefined8 uStack_50;
  ulonglong uStack_48;
  ulonglong uStack_40;
  
  uStack_c8 = 0xfffffffffffffffe;
  uStack_40 = _DAT_180c35580 ^ (ulonglong)auStack_258;
  uStack_1b8 = CONCAT44(uStack_1b8._4_4_,param_4);
  uVar24 = (ulonglong)param_3;
  puStack_1f0 = (undefined8 *)CONCAT44(puStack_1f0._4_4_,param_3);
  puStack_1c8 = param_5;
  auStack_218[0] = param_2;
  puStack_208 = param_1;
  cVar9 = FUN_1805a2010(2);
  if (cVar9 != '\0') {
    uVar13 = FUN_1805a1d70();
    puStack_238 = (undefined8 *)CONCAT44(puStack_238._4_4_,0x3b22);
    FUN_1805a1f10(uVar13,2,&UNK_180abc948,&UNK_180ace670);
  }
  iStack_210 = 0;
  if ((*(char *)(param_1 + 0x7c) != '\0') && (param_6 != '\0')) {
    cVar9 = FUN_1805a2010(2);
    if (cVar9 != '\0') {
      uVar13 = FUN_1805a1d70();
      puStack_238 = (undefined8 *)CONCAT44(puStack_238._4_4_,0x3b27);
      FUN_1805a1f10(uVar13,2,&UNK_180ace638,&UNK_180ace670);
    }
    bVar7 = false;
    uStack_200 = 0;
    uStack_1f8 = 0;
    FUN_180001070(auStack_b0,param_1 + 0x469);
    puVar14 = param_1 + 0x465;
    FUN_180001070(auStack_90,puVar14);
    plVar5 = param_1 + 0x467;
    plVar3 = (longlong *)*plVar5;
    plVar23 = (longlong *)*plVar3;
    bVar27 = false;
    if (plVar23 != plVar3) {
      do {
        puVar21 = *(undefined8 **)plVar23[5];
        while (param_1 = puStack_208, puVar21 != (undefined8 *)plVar23[5]) {
          puVar22 = *(undefined8 **)puVar21[5];
          while (puVar22 != (undefined8 *)puVar21[5]) {
            if ((ulonglong)auStack_218[0] == puVar22[4]) {
              uStack_200 = plVar23[4];
              bVar27 = true;
              goto LAB_18069e1f1;
            }
            if (*(char *)((longlong)puVar22 + 0x19) == '\0') {
              puVar19 = (undefined8 *)puVar22[2];
              if (*(char *)((longlong)puVar19 + 0x19) == '\0') {
                cVar9 = *(char *)((longlong)*puVar19 + 0x19);
                puVar22 = puVar19;
                puVar19 = (undefined8 *)*puVar19;
                while (cVar9 == '\0') {
                  cVar9 = *(char *)((longlong)*puVar19 + 0x19);
                  puVar22 = puVar19;
                  puVar19 = (undefined8 *)*puVar19;
                }
              }
              else {
                cVar9 = *(char *)((longlong)puVar22[1] + 0x19);
                puVar18 = (undefined8 *)puVar22[1];
                puVar19 = puVar22;
                while ((puVar22 = puVar18, cVar9 == '\0' && (puVar19 == (undefined8 *)puVar22[2])))
                {
                  cVar9 = *(char *)((longlong)puVar22[1] + 0x19);
                  puVar18 = (undefined8 *)puVar22[1];
                  puVar19 = puVar22;
                }
              }
            }
          }
          if (*(char *)((longlong)puVar21 + 0x19) == '\0') {
            puVar22 = (undefined8 *)puVar21[2];
            if (*(char *)((longlong)puVar22 + 0x19) == '\0') {
              cVar9 = *(char *)((longlong)*puVar22 + 0x19);
              puVar21 = puVar22;
              puVar22 = (undefined8 *)*puVar22;
              while (cVar9 == '\0') {
                cVar9 = *(char *)((longlong)*puVar22 + 0x19);
                puVar21 = puVar22;
                puVar22 = (undefined8 *)*puVar22;
              }
            }
            else {
              cVar9 = *(char *)((longlong)puVar21[1] + 0x19);
              puVar19 = (undefined8 *)puVar21[1];
              puVar22 = puVar21;
              while ((puVar21 = puVar19, cVar9 == '\0' && (puVar22 == (undefined8 *)puVar21[2]))) {
                cVar9 = *(char *)((longlong)puVar21[1] + 0x19);
                puVar19 = (undefined8 *)puVar21[1];
                puVar22 = puVar21;
              }
            }
          }
        }
        if (*(char *)((longlong)plVar23 + 0x19) == '\0') {
          plVar4 = (longlong *)plVar23[2];
          if (*(char *)((longlong)plVar4 + 0x19) == '\0') {
            cVar9 = *(char *)(*plVar4 + 0x19);
            plVar23 = plVar4;
            plVar4 = (longlong *)*plVar4;
            while (cVar9 == '\0') {
              cVar9 = *(char *)(*plVar4 + 0x19);
              plVar23 = plVar4;
              plVar4 = (longlong *)*plVar4;
            }
          }
          else {
            cVar9 = *(char *)(plVar23[1] + 0x19);
            plVar8 = (longlong *)plVar23[1];
            plVar4 = plVar23;
            while ((plVar23 = plVar8, cVar9 == '\0' && (plVar4 == (longlong *)plVar23[2]))) {
              cVar9 = *(char *)(plVar23[1] + 0x19);
              plVar8 = (longlong *)plVar23[1];
              plVar4 = plVar23;
            }
          }
        }
        bVar27 = false;
      } while (plVar23 != plVar3);
    }
LAB_18069e1f1:
    func_0x0001800010b0(auStack_90);
    if (bVar27) {
      if (uStack_200 - 10000 < 1000) {
        uStack_1f8 = param_1[(ulonglong)((int)uStack_200 - 10000) + 0x7d];
        FUN_180001070(auStack_a0,puVar14);
        puVar14 = (undefined8 *)*plVar5;
        cVar9 = *(char *)((longlong)puVar14[1] + 0x19);
        puVar21 = puVar14;
        puVar22 = (undefined8 *)puVar14[1];
        while (cVar9 == '\0') {
          if ((ulonglong)puVar22[4] < uStack_200) {
            puVar19 = (undefined8 *)puVar22[2];
            puVar22 = puVar21;
          }
          else {
            puVar19 = (undefined8 *)*puVar22;
          }
          puVar21 = puVar22;
          puVar22 = puVar19;
          cVar9 = *(char *)((longlong)puVar19 + 0x19);
        }
        if ((puVar21 == puVar14) || (uStack_200 < (ulonglong)puVar21[4])) {
          puStack_e0 = puVar14;
          ppuVar17 = &puStack_e0;
        }
        else {
          puStack_130 = puVar21;
          ppuVar17 = &puStack_130;
        }
        if ((undefined8 *)*plVar5 == *ppuVar17) {
          cVar9 = FUN_1805a2010(3);
          if (cVar9 != '\0') {
            uVar13 = FUN_1805a1d70();
            uStack_230 = uStack_200;
            puStack_238 = (undefined8 *)CONCAT44(puStack_238._4_4_,0x3b84);
            FUN_1805a1f10(uVar13,3,&UNK_180ace758,&UNK_180ace670);
          }
        }
        else {
          puVar14 = (undefined8 *)*plVar5;
          cVar9 = *(char *)((longlong)puVar14[1] + 0x19);
          puVar21 = puVar14;
          puVar22 = (undefined8 *)puVar14[1];
          while (cVar9 == '\0') {
            if ((ulonglong)puVar22[4] < uStack_200) {
              puVar19 = (undefined8 *)puVar22[2];
              puVar22 = puVar21;
            }
            else {
              puVar19 = (undefined8 *)*puVar22;
            }
            puVar21 = puVar22;
            puVar22 = puVar19;
            cVar9 = *(char *)((longlong)puVar19 + 0x19);
          }
          if ((puVar21 == puVar14) || (uStack_200 < (ulonglong)puVar21[4])) {
            puStack_d0 = &uStack_200;
            puStack_238 = (undefined8 *)FUN_1806f1b60(plVar5,uStack_200,&puStack_d0);
            FUN_1806f1be0(plVar5,&puStack_148,puVar21,puStack_238 + 4);
            puVar21 = puStack_148;
          }
          plVar5 = puVar21 + 5;
          puVar14 = (undefined8 *)*plVar5;
          puVar21 = (undefined8 *)puVar14[1];
          cVar9 = *(char *)((longlong)puVar21 + 0x19);
          puVar22 = puVar14;
          puVar19 = puVar21;
          while (cVar9 == '\0') {
            if ((ulonglong)puVar19[4] < uStack_1f8) {
              puVar18 = (undefined8 *)puVar19[2];
              puVar19 = puVar22;
            }
            else {
              puVar18 = (undefined8 *)*puVar19;
            }
            puVar22 = puVar19;
            puVar19 = puVar18;
            cVar9 = *(char *)((longlong)puVar18 + 0x19);
          }
          if ((puVar22 == puVar14) || (uStack_1f8 < (ulonglong)puVar22[4])) {
            puStack_140 = puVar14;
            ppuVar17 = &puStack_140;
          }
          else {
            puStack_170 = puVar22;
            ppuVar17 = &puStack_170;
          }
          if (puVar14 == *ppuVar17) {
            cVar9 = FUN_1805a2010(3);
            param_1 = puStack_208;
            if (cVar9 != '\0') {
              uVar13 = FUN_1805a1d70();
              uStack_230 = uStack_1f8;
              puStack_238 = (undefined8 *)CONCAT44(puStack_238._4_4_,0x3b7f);
              puVar25 = &UNK_180ace5c8;
LAB_18069e64f:
              FUN_1805a1f10(uVar13,3,puVar25,&UNK_180ace670);
              param_1 = puStack_208;
            }
          }
          else {
            cVar9 = *(char *)((longlong)puVar21 + 0x19);
            puVar22 = puVar14;
            while (cVar9 == '\0') {
              if ((ulonglong)puVar21[4] < uStack_1f8) {
                puVar19 = (undefined8 *)puVar21[2];
                puVar21 = puVar22;
              }
              else {
                puVar19 = (undefined8 *)*puVar21;
              }
              puVar22 = puVar21;
              puVar21 = puVar19;
              cVar9 = *(char *)((longlong)puVar19 + 0x19);
            }
            if ((puVar22 == puVar14) || (uStack_1f8 < (ulonglong)puVar22[4])) {
              puStack_160 = &uStack_1f8;
              puStack_238 = (undefined8 *)FUN_1806f2320(plVar5,puVar21,&puStack_160);
              FUN_1806f23a0(plVar5,&puStack_188,puVar22,puStack_238 + 4);
              puVar22 = puStack_188;
            }
            plVar5 = puVar22 + 5;
            uVar24 = (ulonglong)auStack_218[0];
            puVar14 = (undefined8 *)*plVar5;
            puVar21 = (undefined8 *)puVar14[1];
            cVar9 = *(char *)((longlong)puVar21 + 0x19);
            puVar22 = puVar14;
            puVar19 = puVar21;
            while (cVar9 == '\0') {
              if ((ulonglong)puVar19[4] < uVar24) {
                puVar18 = (undefined8 *)puVar19[2];
                puVar19 = puVar22;
              }
              else {
                puVar18 = (undefined8 *)*puVar19;
              }
              puVar22 = puVar19;
              puVar19 = puVar18;
              cVar9 = *(char *)((longlong)puVar18 + 0x19);
            }
            if ((puVar22 == puVar14) || (uVar24 < (ulonglong)puVar22[4])) {
              puStack_120 = puVar14;
              ppuVar17 = &puStack_120;
            }
            else {
              puStack_150 = puVar22;
              ppuVar17 = &puStack_150;
            }
            if (puVar14 == *ppuVar17) {
              cVar9 = FUN_1805a2010(3);
              param_1 = puStack_208;
              if (cVar9 != '\0') {
                uVar13 = FUN_1805a1d70();
                uStack_230 = CONCAT44(uStack_230._4_4_,auStack_218[0]);
                puStack_238 = (undefined8 *)CONCAT44(puStack_238._4_4_,0x3b7a);
                puVar25 = &UNK_180ace600;
                goto LAB_18069e64f;
              }
            }
            else {
              puStack_110 = (undefined8 *)0x0;
              uStack_108 = 0;
              uStack_100 = 0;
              uStack_f8 = 0;
              cVar9 = *(char *)((longlong)puVar21 + 0x19);
              puVar22 = puVar14;
              while (cVar9 == '\0') {
                if ((ulonglong)puVar21[4] < uVar24) {
                  puVar19 = (undefined8 *)puVar21[2];
                  puVar21 = puVar22;
                }
                else {
                  puVar19 = (undefined8 *)*puVar21;
                }
                puVar22 = puVar21;
                puVar21 = puVar19;
                cVar9 = *(char *)((longlong)puVar19 + 0x19);
              }
              uStack_f0 = uVar24;
              if ((puVar22 == puVar14) || (uVar24 < (ulonglong)puVar22[4])) {
                puStack_138 = &uStack_f0;
                puStack_238 = (undefined8 *)FUN_1806f25f0(plVar5,0,&puStack_138);
                FUN_1806f2640(plVar5,&puStack_d8,puVar22,puStack_238 + 4);
                puVar22 = puStack_d8;
              }
              param_1 = puStack_208;
              puStack_110 = puVar22 + 5;
              uStack_118 = 1;
              uStack_108 = CONCAT44(uStack_108._4_4_,1);
              puVar14 = (undefined8 *)FUN_1806eac30(puStack_208 + 0x1de3,auStack_218);
              *puVar14 = CONCAT44(uStack_114,uStack_118);
              puVar14[1] = puStack_110;
              *(undefined4 *)(puVar14 + 2) = (undefined4)uStack_108;
              *(undefined4 *)((longlong)puVar14 + 0x14) = uStack_108._4_4_;
              *(undefined4 *)(puVar14 + 3) = (undefined4)uStack_100;
              *(undefined4 *)((longlong)puVar14 + 0x1c) = uStack_100._4_4_;
              puVar14[4] = uStack_f8;
              if (param_8 == '\0') {
                iStack_210 = -0x1ffffff9;
                bVar7 = true;
              }
              else {
                uVar13 = FUN_1806eac30(param_1 + 0x1de3,auStack_218);
                *puStack_1c8 = uVar13;
                bVar7 = true;
              }
            }
          }
        }
        func_0x0001800010b0(auStack_a0);
      }
      else {
        cVar9 = FUN_1805a2010(3);
        if (cVar9 != '\0') {
          if ((_DAT_180ca0020 & 1) == 0) {
            _DAT_180ca0020 = _DAT_180ca0020 | 1;
            _DAT_180c9ffd0 = 1;
            _DAT_180c9ffd8 = 0;
            _DAT_180c9ffe0 = 0;
            uRam0000000180c9ffe8 = 0;
            _DAT_180c9fff0 = 0;
            uRam0000000180c9fff8 = 0;
            _DAT_180ca0000 = 0;
            uRam0000000180ca0008 = 0;
            _DAT_180ca0010 = 0;
            FUN_1809e7f58(&UNK_180a29880);
          }
          uStack_230 = uStack_200;
          puStack_238 = (undefined8 *)CONCAT44(puStack_238._4_4_,0x3b89);
          puVar25 = &UNK_180ace710;
          goto LAB_18069e781;
        }
      }
    }
    else {
      cVar9 = FUN_1805a2010(3);
      if (cVar9 != '\0') {
        if ((_DAT_180ca0020 & 1) == 0) {
          _DAT_180ca0020 = _DAT_180ca0020 | 1;
          _DAT_180c9ffd0 = 1;
          _DAT_180c9ffd8 = 0;
          _DAT_180c9ffe0 = 0;
          uRam0000000180c9ffe8 = 0;
          _DAT_180c9fff0 = 0;
          uRam0000000180c9fff8 = 0;
          _DAT_180ca0000 = 0;
          uRam0000000180ca0008 = 0;
          _DAT_180ca0010 = 0;
          FUN_1809e7f58(&UNK_180a29880);
        }
        uStack_230 = CONCAT44(uStack_230._4_4_,auStack_218[0]);
        puStack_238 = (undefined8 *)CONCAT44(puStack_238._4_4_,0x3b8e);
        puVar25 = &UNK_180ace6d8;
LAB_18069e781:
        FUN_1805a1f10(&DAT_180c9ffd0,3,puVar25,&UNK_180ace670);
      }
    }
    func_0x0001800010b0(auStack_b0);
    if (bVar7) {
      cVar9 = FUN_1805a2010(2);
      if (cVar9 != '\0') {
        if ((_DAT_180ca0020 & 1) == 0) {
          _DAT_180ca0020 = _DAT_180ca0020 | 1;
          _DAT_180c9ffd0 = 1;
          _DAT_180c9ffd8 = 0;
          _DAT_180c9ffe0 = 0;
          uRam0000000180c9ffe8 = 0;
          _DAT_180c9fff0 = 0;
          uRam0000000180c9fff8 = 0;
          _DAT_180ca0000 = 0;
          uRam0000000180ca0008 = 0;
          _DAT_180ca0010 = 0;
          FUN_1809e7f58(&UNK_180a29880);
        }
        puStack_238 = (undefined8 *)CONCAT44(puStack_238._4_4_,0x3b94);
        FUN_1805a1f10(&DAT_180c9ffd0,2,&UNK_180ab52d8,&UNK_180ace670);
      }
      goto LAB_18069f27d;
    }
    uVar24 = (ulonglong)puStack_1f0 & 0xffffffff;
    param_4 = (undefined4)uStack_1b8;
  }
  cVar9 = FUN_1805a2010(2);
  if (cVar9 != '\0') {
    if ((_DAT_180ca0020 & 1) == 0) {
      _DAT_180ca0020 = _DAT_180ca0020 | 1;
      _DAT_180c9ffd0 = 1;
      _DAT_180c9ffd8 = 0;
      _DAT_180c9ffe0 = 0;
      uRam0000000180c9ffe8 = 0;
      _DAT_180c9fff0 = 0;
      uRam0000000180c9fff8 = 0;
      _DAT_180ca0000 = 0;
      uRam0000000180ca0008 = 0;
      _DAT_180ca0010 = 0;
      FUN_1809e7f58(&UNK_180a29880);
    }
    puStack_238 = (undefined8 *)CONCAT44(puStack_238._4_4_,0x3b99);
    FUN_1805a1f10(&DAT_180c9ffd0,2,&UNK_180ace6a0,&UNK_180ace670);
  }
  cVar9 = FUN_1805a2010(2);
  if (cVar9 != '\0') {
    if ((_DAT_180ca0020 & 1) == 0) {
      _DAT_180ca0020 = _DAT_180ca0020 | 1;
      _DAT_180c9ffd0 = 1;
      _DAT_180c9ffd8 = 0;
      _DAT_180c9ffe0 = 0;
      uRam0000000180c9ffe8 = 0;
      _DAT_180c9fff0 = 0;
      uRam0000000180c9fff8 = 0;
      _DAT_180ca0000 = 0;
      uRam0000000180ca0008 = 0;
      _DAT_180ca0010 = 0;
      FUN_1809e7f58(&UNK_180a29880);
    }
    uStack_228 = CONCAT44(uStack_228._4_4_,(int)uVar24);
    uStack_230 = CONCAT44(uStack_230._4_4_,auStack_218[0]);
    puStack_238 = (undefined8 *)CONCAT44(puStack_238._4_4_,0x3b9e);
    uStack_220 = param_4;
    FUN_1805a1f10(&DAT_180c9ffd0,2,&UNK_180ace870,&UNK_180ace670);
  }
  uStack_48 = 0xf;
  uStack_50 = 0;
  auStack_60[0]._0_1_ = 0;
  uVar28 = FUN_1805911d0(auStack_60,&DAT_180aab591,0);
  uStack_220 = param_10;
  uStack_228 = CONCAT71(uStack_228._1_7_,param_9);
  uStack_230 = CONCAT71(uStack_230._1_7_,param_7);
  puStack_238 = auStack_60;
  iVar10 = FUN_18068a830(uVar28,auStack_218[0],uVar24,param_4);
  iStack_210 = iVar10;
  if (iVar10 != 0) {
    cVar9 = FUN_1805a2010(4);
    if (cVar9 != '\0') {
      if ((_DAT_180ca0020 & 1) == 0) {
        _DAT_180ca0020 = _DAT_180ca0020 | 1;
        _DAT_180c9ffd0 = 1;
        _DAT_180c9ffd8 = 0;
        _DAT_180c9ffe0 = 0;
        uRam0000000180c9ffe8 = 0;
        _DAT_180c9fff0 = 0;
        uRam0000000180c9fff8 = 0;
        _DAT_180ca0000 = 0;
        uRam0000000180ca0008 = 0;
        _DAT_180ca0010 = 0;
        FUN_1809e7f58(&UNK_180a29880);
      }
      uStack_230 = CONCAT44(uStack_230._4_4_,iVar10);
      puStack_238 = (undefined8 *)CONCAT44(puStack_238._4_4_,0x3ba3);
      FUN_1805a1f10(&DAT_180c9ffd0,4,&UNK_180ace828,&UNK_180ace670);
    }
    aiStack_1c0[0] = iVar10;
                    /* WARNING: Subroutine does not return */
    _CxxThrowException(aiStack_1c0,&UNK_180c23c00);
  }
  iStack_1b0 = 0;
  lVar15 = 0;
  lStack_1a8 = 0;
  uStack_1a0 = 0;
  uStack_198 = 0;
  uStack_190 = 0;
  ppuStack_80 = (undefined8 **)0x0;
  uStack_78 = 0;
  uStack_70 = 0;
  uStack_68 = 0;
  puStack_1e8 = (undefined8 *)0x0;
  uStack_1e0 = 0;
  puStack_1e8 = (undefined8 *)FUN_1806ed040();
  ppuStack_80 = &puStack_1e8;
  uStack_1b8 = 0x20;
  puVar20 = (undefined1 *)auStack_60;
  if (0xf < uStack_48) {
    puVar20 = (undefined1 *)CONCAT71(auStack_60[0]._1_7_,(undefined1)auStack_60[0]);
  }
  if ((code *)param_1[0x474] == (code *)0x0) {
    cVar9 = FUN_1805a2010(4,puVar20);
    if (cVar9 != '\0') {
      uVar13 = FUN_1805a1d70();
      puStack_238 = (undefined8 *)CONCAT44(puStack_238._4_4_,0x377);
      FUN_1805a1f10(uVar13,4,&UNK_180ac4c90,&UNK_180ac4cd8);
    }
    iVar10 = -0x1ffffff0;
  }
  else {
    puStack_238 = &uStack_1b8;
    iVar10 = (*(code *)param_1[0x474])(0x486,puVar20,&ppuStack_80,0x20);
  }
  iStack_210 = iVar10;
  if (iVar10 != 0) {
    cVar9 = FUN_1805a2010(4);
    if (cVar9 != '\0') {
      uVar13 = FUN_1805a1d70();
      uStack_230 = CONCAT44(uStack_230._4_4_,iVar10);
      puStack_238 = (undefined8 *)CONCAT44(puStack_238._4_4_,0x3bb1);
      FUN_1805a1f10(uVar13,4,&UNK_180ace7e8,&UNK_180ace670);
    }
    aiStack_1d4[0] = iVar10;
                    /* WARNING: Subroutine does not return */
    _CxxThrowException(aiStack_1d4,&UNK_180c23c00);
  }
  iVar10 = (int)uStack_78;
  if (0 < iVar10) {
    auVar6._8_8_ = 0;
    auVar6._0_8_ = (longlong)iVar10;
    uVar13 = SUB168(ZEXT816(0xe8) * auVar6,0);
    if (SUB168(ZEXT816(0xe8) * auVar6,8) != 0) {
      uVar13 = 0xffffffffffffffff;
    }
    lVar15 = thunk_FUN_1809e82b0(uVar13,0x180c9ea09);
    lStack_1a8 = lVar15;
    if (lVar15 == 0) {
      cVar9 = FUN_1805a2010(4);
      if (cVar9 != '\0') {
        uVar13 = FUN_1805a1d70();
        puStack_238 = (undefined8 *)CONCAT44(puStack_238._4_4_,0x3bc1);
        FUN_1805a1f10(uVar13,4,&UNK_180acba78,&UNK_180ace670);
      }
      iStack_210 = 0xe0000002;
      aiStack_1c0[1] = 0xe0000002;
                    /* WARNING: Subroutine does not return */
      _CxxThrowException(aiStack_1c0 + 1,&UNK_180c23c00);
    }
    memset(lVar15,0,(longlong)(int)uStack_78 * 0xe8);
    iVar10 = (int)uStack_78;
  }
  iStack_1b0 = iVar10;
  cVar9 = FUN_1805a2010(2);
  if (cVar9 != '\0') {
    uVar13 = FUN_1805a1d70();
    uStack_228 = CONCAT44(uStack_228._4_4_,uStack_78._4_4_);
    uStack_230 = CONCAT44(uStack_230._4_4_,(int)uStack_78);
    puStack_238 = (undefined8 *)CONCAT44(puStack_238._4_4_,0x3bc9);
    FUN_1805a1f10(uVar13,2,&UNK_180acba48,&UNK_180ace670);
  }
  puVar21 = puStack_1e8;
  iVar26 = 0;
  bVar27 = (int)puStack_1f0 == 0;
  for (puVar14 = (undefined8 *)*puStack_1e8; puVar22 = puStack_208, puVar14 != puVar21;
      puVar14 = (undefined8 *)*puVar14) {
    if ((puVar14[2] != 0) && (iVar26 < iVar10)) {
      lVar16 = (longlong)iVar26;
      iVar26 = iVar26 + 1;
      iVar11 = FUN_1806b9950(puStack_208,puVar14[2],bVar27,lVar16 * 0xe8 + lVar15);
      if (iVar11 != 0) {
        iStack_210 = iVar11;
        cVar9 = FUN_1805a2010(4);
        uVar28 = extraout_XMM0_Da;
        if (cVar9 != '\0') {
          uVar13 = FUN_1805a1d70();
          uStack_230 = CONCAT44(uStack_230._4_4_,iVar11);
          puStack_238 = (undefined8 *)CONCAT44(puStack_238._4_4_,0x3bd6);
          uVar28 = FUN_1805a1f10(uVar13,4,&UNK_180acbab0,&UNK_180ace670);
        }
        FUN_1806c1390(uVar28,&iStack_1b0);
        iStack_1d8 = iVar11;
                    /* WARNING: Subroutine does not return */
        _CxxThrowException(&iStack_1d8,&UNK_180c23c00);
      }
      bVar27 = false;
      iStack_210 = 0;
    }
  }
  uStack_1a0 = CONCAT44(uStack_1a0._4_4_,uStack_78._4_4_);
  if ((((int)puStack_1f0 == 0) && (iVar10 == 1)) && (param_9 == '\0')) {
    uVar2 = *(uint *)(lVar15 + 0xb4);
    uVar12 = *(int *)(lVar15 + 8) - 10000;
    if (((uVar12 < 1000) && ((ulonglong)uVar2 != puStack_208[(ulonglong)uVar12 + 0x7d])) &&
       (cVar9 = FUN_1805a2010(3), cVar9 != '\0')) {
      uVar13 = FUN_1805a1d70();
      uStack_228 = puVar22[(ulonglong)uVar12 + 0x7d];
      uStack_230 = CONCAT44(uStack_230._4_4_,uVar2);
      puStack_238 = (undefined8 *)CONCAT44(puStack_238._4_4_,0x3c2d);
      FUN_1805a1f10(uVar13,3,&UNK_180ace790,&UNK_180ace670);
    }
  }
  if (param_8 == '\0') {
    puVar14 = (undefined8 *)FUN_1809e82b0(0x28,0x180c9ea09);
    if (puVar14 == (undefined8 *)0x0) {
      cVar9 = FUN_1805a2010(4);
      if (cVar9 != '\0') {
        uVar13 = FUN_1805a1d70();
        puStack_238 = (undefined8 *)CONCAT44(puStack_238._4_4_,0x3c46);
        FUN_1805a1f10(uVar13,4,&UNK_180ace960,&UNK_180ace670);
      }
      iStack_210 = 0xe0000002;
      aiStack_1d4[1] = 0xe0000002;
                    /* WARNING: Subroutine does not return */
      _CxxThrowException(aiStack_1d4 + 1,&UNK_180c23c00);
    }
    *puVar14 = CONCAT44(uStack_1ac,iStack_1b0);
    puVar14[1] = lStack_1a8;
    *(undefined4 *)(puVar14 + 2) = (undefined4)uStack_1a0;
    *(undefined4 *)((longlong)puVar14 + 0x14) = uStack_1a0._4_4_;
    *(undefined4 *)(puVar14 + 3) = (undefined4)uStack_198;
    *(undefined4 *)((longlong)puVar14 + 0x1c) = uStack_198._4_4_;
    puVar14[4] = uStack_190;
    *puStack_1c8 = puVar14;
  }
  else {
    uVar28 = FUN_180001070(auStack_c0,puVar22 + 0x469);
    plVar5 = puVar22 + 0x1de5;
    puVar14 = (undefined8 *)*plVar5;
    cVar9 = *(char *)((longlong)puVar14[1] + 0x19);
    puVar21 = puVar14;
    puVar22 = (undefined8 *)puVar14[1];
    while (cVar9 == '\0') {
      if (*(uint *)(puVar22 + 4) < auStack_218[0]) {
        puVar19 = (undefined8 *)puVar22[2];
        puVar22 = puVar21;
      }
      else {
        puVar19 = (undefined8 *)*puVar22;
      }
      puVar21 = puVar22;
      puVar22 = puVar19;
      cVar9 = *(char *)((longlong)puVar19 + 0x19);
    }
    if ((puVar21 == puVar14) || (auStack_218[0] < *(uint *)(puVar21 + 4))) {
      puStack_1f0 = puVar14;
      ppuVar17 = &puStack_1f0;
    }
    else {
      puStack_208 = puVar21;
      ppuVar17 = &puStack_208;
    }
    if ((undefined8 *)*plVar5 != *ppuVar17) {
      puVar14 = (undefined8 *)*plVar5;
      cVar9 = *(char *)((longlong)puVar14[1] + 0x19);
      puVar21 = puVar14;
      puVar22 = (undefined8 *)puVar14[1];
      while (cVar9 == '\0') {
        if (*(uint *)(puVar22 + 4) < auStack_218[0]) {
          puVar19 = (undefined8 *)puVar22[2];
          puVar22 = puVar21;
        }
        else {
          puVar19 = (undefined8 *)*puVar22;
        }
        puVar21 = puVar22;
        puVar22 = puVar19;
        cVar9 = *(char *)((longlong)puVar19 + 0x19);
      }
      if ((puVar21 == puVar14) || (auStack_218[0] < *(uint *)(puVar21 + 4))) {
        puStack_128 = auStack_218;
        puStack_238 = (undefined8 *)FUN_1806f2080(plVar5,auStack_218[0],&puStack_128);
        uVar28 = FUN_1806f20d0(plVar5,&puStack_e8,puVar21,puStack_238 + 4);
        puVar21 = puStack_e8;
      }
      FUN_1806c1390(uVar28,puVar21 + 5);
    }
    puVar14 = (undefined8 *)*plVar5;
    cVar9 = *(char *)((longlong)puVar14[1] + 0x19);
    puVar21 = puVar14;
    puVar22 = (undefined8 *)puVar14[1];
    while (cVar9 == '\0') {
      if (*(uint *)(puVar22 + 4) < auStack_218[0]) {
        puVar19 = (undefined8 *)puVar22[2];
        puVar22 = puVar21;
      }
      else {
        puVar19 = (undefined8 *)*puVar22;
      }
      puVar21 = puVar22;
      puVar22 = puVar19;
      cVar9 = *(char *)((longlong)puVar19 + 0x19);
    }
    if ((puVar21 == puVar14) || (auStack_218[0] < *(uint *)(puVar21 + 4))) {
      puStack_180 = auStack_218;
      puStack_238 = (undefined8 *)FUN_1806f2080(plVar5,auStack_218[0],&puStack_180);
      FUN_1806f20d0(plVar5,&puStack_178,puVar21,puStack_238 + 4);
      puVar21 = puStack_178;
    }
    puVar21[5] = CONCAT44(uStack_1ac,iStack_1b0);
    puVar21[6] = lStack_1a8;
    *(undefined4 *)(puVar21 + 7) = (undefined4)uStack_1a0;
    *(undefined4 *)((longlong)puVar21 + 0x3c) = uStack_1a0._4_4_;
    *(undefined4 *)(puVar21 + 8) = (undefined4)uStack_198;
    *(undefined4 *)((longlong)puVar21 + 0x44) = uStack_198._4_4_;
    puVar21[9] = uStack_190;
    puVar14 = (undefined8 *)*plVar5;
    cVar9 = *(char *)((longlong)puVar14[1] + 0x19);
    puVar21 = puVar14;
    puVar22 = (undefined8 *)puVar14[1];
    while (cVar9 == '\0') {
      if (*(uint *)(puVar22 + 4) < auStack_218[0]) {
        puVar19 = (undefined8 *)puVar22[2];
        puVar22 = puVar21;
      }
      else {
        puVar19 = (undefined8 *)*puVar22;
      }
      puVar21 = puVar22;
      puVar22 = puVar19;
      cVar9 = *(char *)((longlong)puVar19 + 0x19);
    }
    if ((puVar21 == puVar14) || (auStack_218[0] < *(uint *)(puVar21 + 4))) {
      puStack_168 = auStack_218;
      puStack_238 = (undefined8 *)FUN_1806f2080(plVar5,auStack_218[0],&puStack_168);
      FUN_1806f20d0(plVar5,&puStack_158,puVar21,puStack_238 + 4);
      puVar21 = puStack_158;
    }
    *puStack_1c8 = puVar21 + 5;
    func_0x0001800010b0(auStack_c0);
  }
  puVar14 = (undefined8 *)*puStack_1e8;
  *puStack_1e8 = puStack_1e8;
  puStack_1e8[1] = puStack_1e8;
  uStack_1e0 = 0;
  if (puVar14 != puStack_1e8) {
    do {
      puVar21 = (undefined8 *)*puVar14;
      plVar5 = (longlong *)puVar14[3];
      if (plVar5 != (longlong *)0x0) {
        LOCK();
        plVar3 = plVar5 + 1;
        lVar15 = *plVar3;
        *(int *)plVar3 = (int)*plVar3 + -1;
        UNLOCK();
        if ((int)lVar15 == 1) {
          (**(code **)*plVar5)(plVar5);
          LOCK();
          piVar1 = (int *)((longlong)plVar5 + 0xc);
          iVar10 = *piVar1;
          *piVar1 = *piVar1 + -1;
          UNLOCK();
          if (iVar10 == 1) {
            (**(code **)(*plVar5 + 8))(plVar5);
          }
        }
      }
      __3_YAXPEAX_Z(puVar14);
      puVar14 = puVar21;
    } while (puVar21 != puStack_1e8);
  }
  __3_YAXPEAX_Z();
  if (0xf < uStack_48) {
    __3_YAXPEAX_Z(CONCAT71(auStack_60[0]._1_7_,(undefined1)auStack_60[0]));
  }
  cVar9 = FUN_1805a2010(2);
  if (cVar9 != '\0') {
    if ((_DAT_180ca0020 & 1) == 0) {
      _DAT_180ca0020 = _DAT_180ca0020 | 1;
      _DAT_180c9ffd0 = 1;
      _DAT_180c9ffd8 = 0;
      _DAT_180c9ffe0 = 0;
      uRam0000000180c9ffe8 = 0;
      _DAT_180c9fff0 = 0;
      uRam0000000180c9fff8 = 0;
      _DAT_180ca0000 = 0;
      uRam0000000180ca0008 = 0;
      _DAT_180ca0010 = 0;
      FUN_1809e7f58(&UNK_180a29880);
    }
    puStack_238 = (undefined8 *)CONCAT44(puStack_238._4_4_,0x3c5a);
    FUN_1805a1f10(&DAT_180c9ffd0,2,&UNK_180ab52d8,&UNK_180ace670);
  }
LAB_18069f27d:
  FUN_1809e7e30(uStack_40 ^ (ulonglong)auStack_258);
  return;
}

