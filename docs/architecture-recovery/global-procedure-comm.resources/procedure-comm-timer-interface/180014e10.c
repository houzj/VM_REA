
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_180014e10(longlong param_1,char *param_2,char *param_3,undefined4 param_4)

{
  longlong *plVar1;
  int *piVar2;
  longlong *plVar3;
  undefined8 *puVar4;
  char cVar5;
  int iVar6;
  undefined4 uVar7;
  undefined8 *puVar8;
  undefined8 uVar9;
  longlong lVar10;
  longlong lVar11;
  longlong lVar12;
  undefined4 extraout_XMM0_Da;
  undefined1 auStack_1b8 [32];
  undefined4 *puStack_198;
  undefined1 *puStack_190;
  char *pcStack_188;
  char *pcStack_180;
  undefined4 uStack_178;
  int iStack_168;
  int iStack_164;
  undefined4 uStack_160;
  undefined4 uStack_15c;
  undefined4 uStack_158;
  int iStack_154;
  undefined4 uStack_150;
  undefined4 uStack_14c;
  undefined4 uStack_148;
  undefined4 uStack_144;
  undefined4 uStack_140;
  undefined4 uStack_13c;
  undefined4 auStack_138 [2];
  undefined8 *puStack_130;
  undefined8 *puStack_128;
  undefined8 uStack_118;
  longlong *plStack_110;
  undefined8 uStack_108;
  longlong lStack_100;
  undefined1 auStack_f8 [8];
  longlong *plStack_f0;
  undefined1 auStack_e8 [16];
  undefined1 uStack_d8;
  undefined7 uStack_d7;
  undefined8 uStack_c8;
  ulonglong uStack_c0;
  undefined1 uStack_b8;
  undefined7 uStack_b7;
  undefined8 uStack_a8;
  ulonglong uStack_a0;
  undefined1 auStack_98 [64];
  ulonglong uStack_58;
  
  uStack_108 = 0xfffffffffffffffe;
  uStack_58 = _DAT_180025000 ^ (ulonglong)auStack_1b8;
  lVar12 = 0;
  if ((param_2 == (char *)0x0) || (param_3 == (char *)0x0)) {
    cVar5 = HSlog_ShouldLog(4);
    if (cVar5 != '\0') {
      if ((_DAT_180025eb0 & 1) == 0) {
        _DAT_180025eb0 = _DAT_180025eb0 | 1;
        FUN_1800013f0();
        FUN_180018df8(FUN_18001a860);
      }
      puVar8 = (undefined8 *)&DAT_180025e90;
      if (0xf < _DAT_180025ea8) {
        puVar8 = _DAT_180025e90;
      }
      puStack_190 = (undefined1 *)CONCAT44(puStack_190._4_4_,0x290);
      puStack_198 = (undefined4 *)&UNK_18001d270;
      pcStack_188 = param_2;
      pcStack_180 = param_3;
      uStack_178 = param_4;
      HSlog_LogPrint(4,&UNK_18001d290,puVar8,*(undefined4 *)(param_1 + 0x48));
    }
  }
  else {
    cVar5 = HSlog_ShouldLog(2);
    lVar11 = lVar12;
    if (cVar5 != '\0') {
      if ((_DAT_180025eb0 & 1) == 0) {
        _DAT_180025eb0 = _DAT_180025eb0 | 1;
        FUN_1800013f0();
        FUN_180018df8(FUN_18001a860);
      }
      puVar8 = (undefined8 *)&DAT_180025e90;
      if (0xf < _DAT_180025ea8) {
        puVar8 = _DAT_180025e90;
      }
      puStack_190 = (undefined1 *)CONCAT44(puStack_190._4_4_,0x294);
      puStack_198 = (undefined4 *)&UNK_18001d270;
      pcStack_188 = param_2;
      HSlog_LogPrint(2,&UNK_18001d2d8,puVar8,*(undefined4 *)(param_1 + 0x48));
    }
    do {
      lVar10 = lVar11 + 1;
      if (param_2[lVar11] != (&UNK_18001d300)[lVar11]) {
        iVar6 = func_0x00018001986e(param_2,&UNK_18001d1e0);
        lVar11 = lVar12;
        if (iVar6 != 0) goto LAB_180014ff0;
        uStack_150 = 0xffffffff;
        iVar6 = sscanf_s(param_3,&UNK_18001b5e8,&uStack_150);
        if (iVar6 < 1) {
          cVar5 = HSlog_ShouldLog(4);
          if (cVar5 != '\0') {
            if ((_DAT_180025eb0 & 1) == 0) {
              _DAT_180025eb0 = _DAT_180025eb0 | 1;
              FUN_1800013f0();
              FUN_180018df8(FUN_18001a860);
            }
            puVar8 = (undefined8 *)&DAT_180025e90;
            if (0xf < _DAT_180025ea8) {
              puVar8 = _DAT_180025e90;
            }
            pcStack_180 = (char *)CONCAT44(pcStack_180._4_4_,param_4);
            puStack_190 = (undefined1 *)CONCAT44(puStack_190._4_4_,0x29e);
            goto LAB_180014f9e;
          }
        }
        else if (*(longlong *)(param_1 + 0x50) != 0) {
          *(undefined4 *)(*(longlong *)(param_1 + 0x50) + 4) = uStack_150;
        }
        break;
      }
      lVar11 = lVar10;
    } while (lVar10 != 8);
  }
  goto LAB_180015aa3;
  while (lVar11 = lVar10, lVar10 != 8) {
LAB_180014ff0:
    lVar10 = lVar11 + 1;
    if (param_2[lVar11] != (&UNK_18001d358)[lVar11]) {
      iVar6 = func_0x00018001986e(param_2,&UNK_18001d360);
      if (iVar6 == 0) {
        uStack_148 = 0xffffffff;
        iVar6 = sscanf_s(param_3,&UNK_18001b5e8,&uStack_148);
        if (0 < iVar6) {
          FUN_180015e10(param_1,uStack_148);
          goto LAB_180015aa3;
        }
        cVar5 = HSlog_ShouldLog(4);
        if (cVar5 == '\0') goto LAB_180015aa3;
        puVar8 = (undefined8 *)FUN_1800013b0();
        if (0xf < (ulonglong)puVar8[3]) {
          puVar8 = (undefined8 *)*puVar8;
        }
        pcStack_180 = (char *)CONCAT44(pcStack_180._4_4_,param_4);
        puStack_190 = (undefined1 *)CONCAT44(puStack_190._4_4_,0x2ac);
        goto LAB_180014f9e;
      }
      iVar6 = func_0x00018001986e(param_2,&UNK_18001d370);
      if (iVar6 == 0) {
        uStack_118 = 0;
        plStack_110 = (longlong *)0x0;
        FUN_180016c00(param_1 + 0x50,&uStack_118);
        plVar3 = plStack_110;
        if (plStack_110 != (longlong *)0x0) {
          LOCK();
          plVar1 = plStack_110 + 1;
          *(int *)plVar1 = (int)*plVar1 + -1;
          UNLOCK();
          if ((int)*plVar1 == 0) {
            (**(code **)*plStack_110)(plStack_110);
            LOCK();
            piVar2 = (int *)((longlong)plVar3 + 0xc);
            *piVar2 = *piVar2 + -1;
            UNLOCK();
            if (*piVar2 == 0) {
              (**(code **)(*plStack_110 + 8))();
            }
          }
        }
        uStack_158 = 0xffffffff;
        iVar6 = sscanf_s(param_3,&UNK_18001b5e8,&uStack_158);
        if (0 < iVar6) {
          uVar9 = FUN_180015b50(param_1,auStack_f8,uStack_158);
          FUN_180016c00(param_1 + 0x50,uVar9);
          if (plStack_f0 != (longlong *)0x0) {
            LOCK();
            plVar3 = plStack_f0 + 1;
            *(int *)plVar3 = (int)*plVar3 + -1;
            UNLOCK();
            if ((int)*plVar3 == 0) {
              (**(code **)*plStack_f0)(plStack_f0);
              LOCK();
              piVar2 = (int *)((longlong)plStack_f0 + 0xc);
              *piVar2 = *piVar2 + -1;
              UNLOCK();
              if (*piVar2 == 0) {
                (**(code **)(*plStack_f0 + 8))(plStack_f0);
              }
            }
          }
          goto LAB_180015aa3;
        }
        cVar5 = HSlog_ShouldLog(4);
        if (cVar5 == '\0') goto LAB_180015aa3;
        puVar8 = (undefined8 *)FUN_1800013b0();
        if (0xf < (ulonglong)puVar8[3]) {
          puVar8 = (undefined8 *)*puVar8;
        }
        pcStack_180 = (char *)CONCAT44(pcStack_180._4_4_,param_4);
        puStack_190 = (undefined1 *)CONCAT44(puStack_190._4_4_,0x2b2);
        goto LAB_180014f9e;
      }
      iVar6 = func_0x00018001986e(param_2,&UNK_18001d1f0);
      if (iVar6 == 0) {
        if ((*(longlong *)(param_1 + 0x50) == 0) ||
           (*(int *)(*(longlong *)(param_1 + 0x50) + 4) == 2)) goto LAB_180015aa3;
        uStack_140 = 0xffffffff;
        iVar6 = sscanf_s(param_3,&UNK_18001b5e8,&uStack_140);
        if (0 < iVar6) {
          *(undefined4 *)(*(longlong *)(param_1 + 0x50) + 0x18) = uStack_140;
          goto LAB_180015aa3;
        }
        cVar5 = HSlog_ShouldLog(4);
        if (cVar5 == '\0') goto LAB_180015aa3;
        puVar8 = (undefined8 *)FUN_1800013b0();
        if (0xf < (ulonglong)puVar8[3]) {
          puVar8 = (undefined8 *)*puVar8;
        }
        pcStack_180 = (char *)CONCAT44(pcStack_180._4_4_,param_4);
        puStack_190 = (undefined1 *)CONCAT44(puStack_190._4_4_,0x2bd);
        goto LAB_180014f9e;
      }
      iVar6 = func_0x00018001986e(param_2,&UNK_18001d200);
      if (iVar6 == 0) {
        if (*(longlong *)(param_1 + 0x50) != 0) {
          uStack_a0 = 0xf;
          uStack_a8 = 0;
          uStack_b8 = 0;
          FUN_180002190(&uStack_b8,&UNK_18001d210,1);
          uStack_c0 = 0xf;
          uStack_c8 = 0;
          uStack_d8 = 0;
          if (*param_3 != '\0') {
            lVar12 = -1;
            do {
              lVar12 = lVar12 + 1;
            } while (param_3[lVar12] != '\0');
          }
          FUN_180002190(&uStack_d8,param_3,lVar12);
          FUN_1800019b0(&puStack_130,&uStack_d8,&uStack_b8);
          if (0xf < uStack_c0) {
            __3_YAXPEAX_Z(CONCAT71(uStack_d7,uStack_d8));
          }
          uStack_c0 = 0xf;
          uStack_c8 = 0;
          uStack_d8 = 0;
          if (0xf < uStack_a0) {
            __3_YAXPEAX_Z(CONCAT71(uStack_b7,uStack_b8));
          }
          uStack_a0 = 0xf;
          uStack_a8 = 0;
          uStack_b8 = 0;
          if ((ulonglong)((longlong)puStack_128 - (longlong)puStack_130 >> 5) < 3) {
            cVar5 = HSlog_ShouldLog(4);
            if (cVar5 != '\0') {
              puVar8 = (undefined8 *)FUN_1800013b0();
              if (0xf < (ulonglong)puVar8[3]) {
                puVar8 = (undefined8 *)*puVar8;
              }
              puStack_190 = (undefined1 *)CONCAT44(puStack_190._4_4_,0x2d1);
LAB_180015508:
              puStack_198 = (undefined4 *)&UNK_18001d270;
              pcStack_188 = param_3;
              HSlog_LogPrint(4,&UNK_18001d380,puVar8,*(undefined4 *)(param_1 + 0x48));
            }
LAB_18001552e:
            puVar8 = puStack_130;
            if (puStack_130 != (undefined8 *)0x0) {
              for (; puVar8 != puStack_128; puVar8 = puVar8 + 4) {
                if (0xf < (ulonglong)puVar8[3]) {
                  __3_YAXPEAX_Z(*puVar8);
                }
                puVar8[3] = 0xf;
                puVar8[2] = 0;
                *(undefined1 *)puVar8 = 0;
              }
              __3_YAXPEAX_Z(puStack_130);
            }
          }
          else {
            puVar8 = puStack_130;
            if (0xf < (ulonglong)puStack_130[3]) {
              puVar8 = (undefined8 *)*puStack_130;
            }
            lVar12 = *(longlong *)(param_1 + 0x50);
            uVar7 = atoi(puVar8);
            *(undefined4 *)(lVar12 + 8) = uVar7;
            puVar8 = puStack_130 + 4;
            if (0xf < (ulonglong)puStack_130[7]) {
              puVar8 = (undefined8 *)*puVar8;
            }
            lVar12 = *(longlong *)(param_1 + 0x50);
            uVar7 = atoi(puVar8);
            *(undefined4 *)(lVar12 + 0x10) = uVar7;
            puVar8 = puStack_130 + 8;
            if (0xf < (ulonglong)puStack_130[0xb]) {
              puVar8 = (undefined8 *)*puVar8;
            }
            lVar12 = *(longlong *)(param_1 + 0x50);
            uVar7 = FUN_180016840(extraout_XMM0_Da,puVar8);
            *(undefined4 *)(lVar12 + 0xc) = uVar7;
            lVar12 = *(longlong *)(param_1 + 0x50);
            if (*(int *)(lVar12 + 0xc) == 0x10) {
              if ((*(int *)(lVar12 + 0xc) != 0x20) && (*(int *)(lVar12 + 0xc) != 0x40)) {
                *(undefined4 *)(lVar12 + 0x14) = 0xffffffff;
              }
            }
            else {
              *(undefined4 *)(lVar12 + 0x1c) = 0xffffffff;
              *(undefined4 *)(*(longlong *)(param_1 + 0x50) + 0x20) = 0xffffffff;
              *(undefined4 *)(*(longlong *)(param_1 + 0x50) + 0x24) = 0xffffffff;
            }
            lVar12 = *(longlong *)(param_1 + 0x50);
            if (*(int *)(lVar12 + 0xc) == 0x10) {
              if ((ulonglong)((longlong)puStack_128 - (longlong)puStack_130 >> 5) < 5) {
                cVar5 = HSlog_ShouldLog(4);
                if (cVar5 != '\0') {
                  puVar8 = (undefined8 *)FUN_1800013b0();
                  if (0xf < (ulonglong)puVar8[3]) {
                    puVar8 = (undefined8 *)*puVar8;
                  }
                  puStack_190 = (undefined1 *)CONCAT44(puStack_190._4_4_,0x2eb);
                  goto LAB_180015508;
                }
                goto LAB_18001552e;
              }
              puVar8 = puStack_130 + 0xc;
              if (0xf < (ulonglong)puStack_130[0xf]) {
                puVar8 = (undefined8 *)*puVar8;
              }
              uVar7 = atoi(puVar8);
              *(undefined4 *)(lVar12 + 0x28) = uVar7;
              puVar8 = puStack_130 + 0x10;
              if (0xf < (ulonglong)puStack_130[0x13]) {
                puVar8 = (undefined8 *)*puVar8;
              }
              sprintf_s(*(longlong *)(param_1 + 0x50) + 0x2c,0x40,&UNK_18001b658,puVar8);
            }
            puVar8 = puStack_130;
            if (puStack_130 != (undefined8 *)0x0) {
              for (; puVar8 != puStack_128; puVar8 = puVar8 + 4) {
                if (0xf < (ulonglong)puVar8[3]) {
                  __3_YAXPEAX_Z(*puVar8);
                }
                puVar8[3] = 0xf;
                puVar8[2] = 0;
                *(undefined1 *)puVar8 = 0;
              }
              __3_YAXPEAX_Z(puStack_130);
            }
          }
        }
        goto LAB_180015aa3;
      }
      iVar6 = func_0x00018001986e(param_2,&UNK_18001d218);
      if (iVar6 == 0) {
        if (*(longlong *)(param_1 + 0x50) != 0) {
          auStack_138[0] = 0xffffffff;
          uStack_14c = 0xffffffff;
          uStack_13c = 0xffffffff;
          pcStack_188 = (char *)0x40;
          puStack_190 = auStack_98;
          puStack_198 = &uStack_13c;
          iVar6 = sscanf_s(param_3,&UNK_18001d3a8,&uStack_14c,auStack_138);
          if (iVar6 < 4) {
            cVar5 = HSlog_ShouldLog(4);
            if (cVar5 != '\0') {
              puVar8 = (undefined8 *)FUN_1800013b0();
              if (0xf < (ulonglong)puVar8[3]) {
                puVar8 = (undefined8 *)*puVar8;
              }
              puStack_190 = (undefined1 *)CONCAT44(puStack_190._4_4_,0x2ff);
              puStack_198 = (undefined4 *)&UNK_18001d270;
              pcStack_188 = param_3;
              HSlog_LogPrint(4,&UNK_18001d3c0,puVar8,*(undefined4 *)(param_1 + 0x48));
            }
            *(undefined4 *)(*(longlong *)(param_1 + 0x50) + 8) = 0xffffffff;
            *(undefined4 *)(*(longlong *)(param_1 + 0x50) + 0x10) = 0xffffffff;
            *(undefined4 *)(*(longlong *)(param_1 + 0x50) + 0xc) = 0;
          }
          else {
            *(undefined4 *)(*(longlong *)(param_1 + 0x50) + 8) = auStack_138[0];
            *(undefined4 *)(*(longlong *)(param_1 + 0x50) + 0x10) = uStack_14c;
            *(undefined4 *)(*(longlong *)(param_1 + 0x50) + 0xc) = 0x80;
            *(undefined4 *)(*(longlong *)(param_1 + 0x50) + 0x28) = uStack_13c;
            sprintf_s(*(longlong *)(param_1 + 0x50) + 0x2c,0x40,&UNK_18001b658,auStack_98);
          }
        }
        goto LAB_180015aa3;
      }
      iVar6 = func_0x00018001986e(param_2,&UNK_18001d228);
      if (iVar6 == 0) {
        if (*(longlong *)(param_1 + 0x50) == 0) goto LAB_180015aa3;
        uStack_144 = 0xffffffff;
        iVar6 = sscanf_s(param_3,&UNK_18001b5e8,&uStack_144);
        if (0 < iVar6) {
          *(undefined4 *)(*(longlong *)(param_1 + 0x50) + 0x14) = uStack_144;
          goto LAB_180015aa3;
        }
        cVar5 = HSlog_ShouldLog(4);
        if (cVar5 == '\0') goto LAB_180015aa3;
        puVar8 = (undefined8 *)FUN_1800013b0();
        if (0xf < (ulonglong)puVar8[3]) {
          puVar8 = (undefined8 *)*puVar8;
        }
        pcStack_180 = (char *)CONCAT44(pcStack_180._4_4_,param_4);
        puStack_190 = (undefined1 *)CONCAT44(puStack_190._4_4_,0x317);
        goto LAB_180014f9e;
      }
      iVar6 = func_0x00018001986e(param_2,&UNK_18001d238);
      if (iVar6 == 0) {
        if (*(longlong *)(param_1 + 0x50) == 0) goto LAB_180015aa3;
        iStack_154 = -1;
        iVar6 = sscanf_s(param_3,&UNK_18001b5e8,&iStack_154);
        if (0 < iVar6) {
          *(int *)(*(longlong *)(param_1 + 0x50) + 0x1c) = iStack_154 + -1;
          goto LAB_180015aa3;
        }
        cVar5 = HSlog_ShouldLog(4);
        if (cVar5 == '\0') goto LAB_180015aa3;
        puVar8 = (undefined8 *)FUN_1800013b0();
        if (0xf < (ulonglong)puVar8[3]) {
          puVar8 = (undefined8 *)*puVar8;
        }
        pcStack_180 = (char *)CONCAT44(pcStack_180._4_4_,param_4);
        puStack_190 = (undefined1 *)CONCAT44(puStack_190._4_4_,0x322);
      }
      else {
        iVar6 = func_0x00018001986e(param_2,&UNK_18001d248);
        if (iVar6 == 0) {
          if (*(longlong *)(param_1 + 0x50) == 0) goto LAB_180015aa3;
          iStack_168 = -1;
          iVar6 = sscanf_s(param_3,&UNK_18001b5e8,&iStack_168);
          if (0 < iVar6) {
            *(int *)(*(longlong *)(param_1 + 0x50) + 0x20) = iStack_168 + -1;
            goto LAB_180015aa3;
          }
          cVar5 = HSlog_ShouldLog(4);
          if (cVar5 == '\0') goto LAB_180015aa3;
          puVar8 = (undefined8 *)FUN_1800013b0();
          if (0xf < (ulonglong)puVar8[3]) {
            puVar8 = (undefined8 *)*puVar8;
          }
          pcStack_180 = (char *)CONCAT44(pcStack_180._4_4_,param_4);
          puStack_190 = (undefined1 *)CONCAT44(puStack_190._4_4_,0x32d);
        }
        else {
          iVar6 = func_0x00018001986e(param_2,&UNK_18001d258);
          if (iVar6 == 0) {
            if (*(longlong *)(param_1 + 0x50) == 0) goto LAB_180015aa3;
            iStack_164 = -1;
            iVar6 = sscanf_s(param_3,&UNK_18001b5e8,&iStack_164);
            if (0 < iVar6) {
              *(int *)(*(longlong *)(param_1 + 0x50) + 0x24) = iStack_164 + -1;
              goto LAB_180015aa3;
            }
            cVar5 = HSlog_ShouldLog(4);
            if (cVar5 == '\0') goto LAB_180015aa3;
            puVar8 = (undefined8 *)FUN_1800013b0();
            if (0xf < (ulonglong)puVar8[3]) {
              puVar8 = (undefined8 *)*puVar8;
            }
            pcStack_180 = (char *)CONCAT44(pcStack_180._4_4_,param_4);
            puStack_190 = (undefined1 *)CONCAT44(puStack_190._4_4_,0x338);
          }
          else {
            iVar6 = func_0x00018001986e(param_2,&UNK_18001d408);
            if (iVar6 != 0) {
              iVar6 = func_0x00018001986e(param_2,&UNK_18001d418);
              if (iVar6 == 0) {
                lVar12 = param_1 + 0x70;
                lStack_100 = lVar12;
                iVar6 = _Mtx_lock(lVar12);
                if (iVar6 != 0) {
                  __Throw_C_error_std__YAXH_Z(iVar6);
                }
                puVar8 = *(undefined8 **)(param_1 + 0x60);
                for (puVar4 = (undefined8 *)*puVar8; puVar4 != puVar8;
                    puVar4 = (undefined8 *)*puVar4) {
                  if (*(int *)(puVar4[2] + 4) == 2) {
                    uVar9 = FUN_180016bc0(auStack_e8);
                    FUN_180012f00(param_1,uVar9);
                  }
                }
                iVar6 = _Mtx_unlock(lVar12);
                if (iVar6 != 0) {
                  __Throw_C_error_std__YAXH_Z(iVar6);
                }
              }
              else {
                iVar6 = func_0x00018001986e(param_2,&UNK_18001d430);
                if (iVar6 == 0) {
                  lVar12 = *(longlong *)(param_1 + 0x68);
                  while (lVar12 != 0) {
                    FUN_180015e10(param_1,**(undefined4 **)(**(longlong **)(param_1 + 0x60) + 0x10))
                    ;
                    lVar12 = *(longlong *)(param_1 + 0x68);
                  }
                }
                else {
                  FUN_180008550(param_1,param_2,param_3,param_4);
                }
              }
              goto LAB_180015aa3;
            }
            uStack_15c = 0xffffffff;
            iVar6 = sscanf_s(param_3,&UNK_18001b5e8,&uStack_15c);
            if (0 < iVar6) {
              FUN_180012dc0(param_1,uStack_15c);
              goto LAB_180015aa3;
            }
            cVar5 = HSlog_ShouldLog(4);
            if (cVar5 == '\0') goto LAB_180015aa3;
            puVar8 = (undefined8 *)FUN_1800013b0();
            if (0xf < (ulonglong)puVar8[3]) {
              puVar8 = (undefined8 *)*puVar8;
            }
            pcStack_180 = (char *)CONCAT44(pcStack_180._4_4_,param_4);
            puStack_190 = (undefined1 *)CONCAT44(puStack_190._4_4_,0x33d);
          }
        }
      }
      goto LAB_180014f9e;
    }
  }
  uStack_160 = 0xffffffff;
  iVar6 = sscanf_s(param_3,&UNK_18001b5e8,&uStack_160);
  if (iVar6 < 1) {
    cVar5 = HSlog_ShouldLog(4);
    if (cVar5 != '\0') {
      puVar8 = (undefined8 *)FUN_1800013b0();
      if (0xf < (ulonglong)puVar8[3]) {
        puVar8 = (undefined8 *)*puVar8;
      }
      pcStack_180 = (char *)CONCAT44(pcStack_180._4_4_,param_4);
      puStack_190 = (undefined1 *)CONCAT44(puStack_190._4_4_,0x2a7);
LAB_180014f9e:
      puStack_198 = (undefined4 *)&UNK_18001d270;
      pcStack_188 = param_3;
      HSlog_LogPrint(4,&UNK_18001d310,puVar8,*(undefined4 *)(param_1 + 0x48));
    }
  }
  else {
    FUN_180015c40(param_1,uStack_160);
  }
LAB_180015aa3:
  FUN_180018cd0(uStack_58 ^ (ulonglong)auStack_1b8);
  return;
}

