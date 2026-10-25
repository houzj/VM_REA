
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_1800eee30(longlong param_1,undefined4 param_2,char *param_3,int param_4,longlong param_5,
                  ulonglong param_6,char *param_7,ulonglong *param_8)

{
  ulonglong *puVar1;
  int *piVar2;
  char cVar3;
  ulonglong uVar4;
  char **ppcVar5;
  char **ppcVar6;
  char *pcVar7;
  char cVar8;
  int iVar9;
  uint uVar10;
  uint uVar11;
  longlong *plVar12;
  char *pcVar13;
  ulonglong uVar14;
  ulonglong uVar15;
  undefined8 uVar16;
  longlong *plVar17;
  ulonglong uVar18;
  longlong *plVar19;
  undefined8 *puVar20;
  undefined8 *puVar21;
  undefined8 *puVar22;
  longlong *plVar23;
  longlong lVar24;
  ulonglong uVar25;
  undefined1 auStack_178 [32];
  longlong lStack_158;
  undefined4 uStack_150;
  ulonglong uStack_148;
  longlong lStack_140;
  char *pcStack_138;
  char *pcStack_128;
  longlong *plStack_120;
  undefined4 auStack_118 [2];
  char *pcStack_110;
  longlong *plStack_108;
  ulonglong uStack_100;
  char *pcStack_f8;
  longlong *plStack_f0;
  undefined8 *puStack_e8;
  int iStack_e0;
  ulonglong *puStack_d8;
  longlong lStack_d0;
  longlong lStack_c8;
  longlong *plStack_b8;
  longlong *plStack_a8;
  undefined1 auStack_a0 [16];
  undefined8 uStack_90;
  undefined8 uStack_88;
  undefined8 uStack_80;
  undefined1 uStack_78;
  undefined7 uStack_77;
  ulonglong uStack_68;
  ulonglong uStack_60;
  ulonglong uStack_58;
  
  uStack_80 = 0xfffffffffffffffe;
  uStack_58 = _DAT_1802a5230 ^ (ulonglong)auStack_178;
  lStack_d0 = param_5;
  uStack_100 = param_6;
  pcStack_f8 = param_7;
  puStack_d8 = param_8;
  auStack_118[0] = param_2;
  iStack_e0 = param_4;
  if ((param_3 == (char *)0x0) || (param_5 == 0)) goto LAB_1800ef3e5;
  lStack_c8 = param_1 + 400;
  iVar9 = _Mtx_lock();
  if (iVar9 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar9);
  }
  uStack_60 = 0xf;
  uVar14 = 0;
  uStack_68 = 0;
  uStack_78 = 0;
  uVar11 = 0;
  uVar15 = uVar14;
  if (*param_3 != '\0') {
    uVar15 = 0xffffffffffffffff;
    do {
      uVar15 = uVar15 + 1;
    } while (param_3[uVar15] != '\0');
  }
  FUN_180006300(&uStack_78,param_3,uVar15);
  plVar12 = (longlong *)FUN_1800f07a0(param_1 + 0x88,auStack_118);
  uVar4 = uStack_60;
  uVar15 = uStack_68;
  puStack_e8 = (undefined8 *)*plVar12;
  cVar8 = *(char *)((longlong)puStack_e8[1] + 0x19);
  puVar21 = puStack_e8;
  puVar20 = (undefined8 *)puStack_e8[1];
  while (plStack_f0 = plVar12, cVar8 == '\0') {
    plVar12 = puVar20 + 4;
    plVar19 = (longlong *)&uStack_78;
    if (0xf < uVar4) {
      plVar19 = (longlong *)CONCAT71(uStack_77,uStack_78);
    }
    uVar18 = puVar20[6];
    if (0xf < (ulonglong)puVar20[7]) {
      plVar12 = (longlong *)*plVar12;
    }
    uVar25 = uVar15;
    if (uVar18 < uVar15) {
      uVar25 = uVar18;
    }
    uVar10 = uVar11;
    if (uVar25 != 0) {
      uVar10 = func_0x0001801e92c0(plVar12,plVar19);
    }
    if (uVar10 == 0) {
      if (uVar18 < uVar15) {
        uVar10 = 0xffffffff;
      }
      else {
        uVar10 = (uint)(uVar18 != uVar15);
      }
    }
    if ((int)uVar10 < 0) {
      puVar22 = (undefined8 *)puVar20[2];
      puVar20 = puVar21;
    }
    else {
      puVar22 = (undefined8 *)*puVar20;
    }
    puVar21 = puVar20;
    puVar20 = puVar22;
    plVar12 = plStack_f0;
    cVar8 = *(char *)((longlong)puVar22 + 0x19);
  }
  if (puVar21 == puStack_e8) {
LAB_1800ef00f:
    plStack_f0 = (longlong *)&uStack_78;
    lStack_158 = FUN_1800f1f10(plVar12);
    FUN_1800f2230(plVar12,&puStack_e8,puVar21,lStack_158 + 0x20);
    plVar23 = (longlong *)CONCAT71(uStack_77,uStack_78);
    puVar21 = puStack_e8;
  }
  else {
    plVar19 = puVar21 + 4;
    uVar4 = puVar21[6];
    if (0xf < (ulonglong)puVar21[7]) {
      plVar19 = (longlong *)*plVar19;
    }
    plVar23 = (longlong *)CONCAT71(uStack_77,uStack_78);
    plVar17 = (longlong *)&uStack_78;
    if (0xf < uStack_60) {
      plVar17 = plVar23;
    }
    uVar18 = uVar4;
    if (uVar15 < uVar4) {
      uVar18 = uVar15;
    }
    if (uVar18 != 0) {
      uVar11 = func_0x0001801e92c0(plVar17,plVar19);
    }
    if (uVar11 == 0) {
      if (uVar15 < uVar4) {
        uVar11 = 0xffffffff;
      }
      else {
        uVar11 = (uint)(uVar15 != uVar4);
      }
    }
    if ((int)uVar11 < 0) goto LAB_1800ef00f;
  }
  puVar1 = puVar21 + 8;
  if (0xf < uStack_60) {
    __3_YAXPEAX_Z(plVar23);
  }
  pcVar7 = pcStack_f8;
  uVar4 = uStack_100;
  uStack_60 = 0xf;
  uStack_68 = 0;
  uStack_78 = 0;
  pcStack_128 = (char *)0x0;
  plStack_120 = (longlong *)0x0;
  uVar15 = *puVar1;
  uVar18 = (longlong)(puVar21[9] - uVar15) >> 4;
  uVar25 = (ulonglong)iStack_e0;
  if (uVar25 < uVar18) {
    plStack_120 = *(longlong **)(uVar15 + 8 + uVar25 * 0x10);
    pcStack_128 = *(char **)(uVar15 + uVar25 * 0x10);
    if (plStack_120 != (longlong *)0x0) {
      LOCK();
      *(int *)(plStack_120 + 1) = (int)plStack_120[1] + 1;
      UNLOCK();
    }
LAB_1800ef0cc:
    pcVar13 = pcStack_128;
    if (0xf < *(ulonglong *)(pcStack_128 + 0x18)) {
      pcVar13 = *(char **)pcStack_128;
    }
    lVar24 = (longlong)pcStack_f8 - (longlong)pcVar13;
    do {
      cVar8 = *pcVar13;
      cVar3 = pcVar13[lVar24];
      if (cVar8 != cVar3) break;
      pcVar13 = pcVar13 + 1;
    } while (cVar3 != '\0');
    if (cVar8 != cVar3) goto LAB_1800ef14a;
    uVar15 = uVar14;
    if (*(longlong *)(pcStack_128 + 0x28) != 0) {
      uVar15 = *(longlong *)(pcStack_128 + 0x28) - 0x80;
    }
    if (uVar15 < uStack_100) goto LAB_1800ef14a;
    if (*(longlong *)(pcStack_128 + 0x30) != 0) {
      uVar14 = *(longlong *)(pcStack_128 + 0x30) + 0x80;
    }
    *puStack_d8 = uVar14;
    *(ulonglong *)(pcStack_128 + 0x38) = uStack_100;
  }
  else {
    iVar9 = -0x1fffffec;
    if ((int)uVar18 == 0) {
      iVar9 = -0x1ffffcec;
    }
    if (iVar9 == 0) goto LAB_1800ef0cc;
LAB_1800ef14a:
    pcVar13 = (char *)FUN_1801e8cd0(0x40,0x1802b0a11);
    pcStack_f8 = pcVar13;
    if (pcVar13 != (char *)0x0) {
      uStack_88 = 0xf;
      uStack_90 = 0;
      auStack_a0[0] = 0;
      if (*pcVar7 != '\0') {
        uVar14 = 0xffffffffffffffff;
        do {
          uVar14 = uVar14 + 1;
        } while (pcVar7[uVar14] != '\0');
      }
      FUN_180006300(auStack_a0,pcVar7,uVar14);
      uVar14 = FUN_1800eff60(pcVar13,auStack_a0,uVar4);
    }
    pcStack_110 = (char *)0x0;
    plStack_108 = (longlong *)0x0;
    FUN_1800f1750(&pcStack_110,uVar14);
    plVar12 = plStack_120;
    plStack_b8 = plStack_120;
    plStack_120 = plStack_108;
    pcStack_128 = pcStack_110;
    if (plVar12 != (longlong *)0x0) {
      LOCK();
      plVar19 = plVar12 + 1;
      *(int *)plVar19 = (int)*plVar19 + -1;
      UNLOCK();
      if ((int)*plVar19 == 0) {
        (**(code **)*plVar12)(plVar12);
        LOCK();
        piVar2 = (int *)((longlong)plVar12 + 0xc);
        *piVar2 = *piVar2 + -1;
        UNLOCK();
        if (*piVar2 == 0) {
          (**(code **)(*plStack_b8 + 8))();
        }
      }
    }
    plVar12 = plStack_108;
    pcVar13 = pcStack_110;
    if (pcStack_110 != (char *)0x0) {
      uVar15 = *(ulonglong *)(pcStack_110 + 0x30);
      if (uVar15 != 0) {
        uVar15 = uVar15 + 0x80;
      }
      *puStack_d8 = uVar15;
      cVar8 = FUN_18001dbc0(0);
      if (cVar8 != '\0') {
        uVar16 = FUN_18001db50();
        pcStack_138 = pcVar7;
        lStack_140 = lStack_d0;
        uStack_148 = uStack_100;
        uStack_150 = auStack_118[0];
        lStack_158 = CONCAT44(lStack_158._4_4_,0x8f);
        FUN_180015b40(uVar16,0,&UNK_18022e138,&UNK_18022e160);
      }
      ppcVar5 = (char **)puVar21[9];
      ppcVar6 = (char **)*puVar1;
      if (uVar25 < (ulonglong)((longlong)ppcVar5 - (longlong)ppcVar6 >> 4)) {
        if (plVar12 != (longlong *)0x0) {
          LOCK();
          *(int *)(plVar12 + 1) = (int)plVar12[1] + 1;
          UNLOCK();
          plVar12 = plStack_108;
          pcVar13 = pcStack_110;
        }
        plVar19 = (longlong *)ppcVar6[uVar25 * 2 + 1];
        ppcVar6[uVar25 * 2 + 1] = (char *)plVar12;
        ppcVar6[uVar25 * 2] = pcVar13;
        plStack_a8 = plVar19;
        if (plVar19 != (longlong *)0x0) {
          LOCK();
          plVar12 = plVar19 + 1;
          *(int *)plVar12 = (int)*plVar12 + -1;
          UNLOCK();
          if ((int)*plVar12 == 0) {
            (**(code **)*plVar19)(plVar19);
            LOCK();
            piVar2 = (int *)((longlong)plVar19 + 0xc);
            *piVar2 = *piVar2 + -1;
            UNLOCK();
            if (*piVar2 == 0) {
              (**(code **)(*plStack_a8 + 8))();
            }
          }
        }
      }
      else {
        if ((&pcStack_128 < ppcVar5) && (ppcVar6 <= &pcStack_128)) {
          if (ppcVar5 == (char **)puVar21[10]) {
            FUN_1800f0e70(puVar1);
          }
          puVar22 = (undefined8 *)
                    (((longlong)&pcStack_128 - (longlong)ppcVar6 & 0xfffffffffffffff0U) + *puVar1);
          puVar20 = (undefined8 *)puVar21[9];
          if (puVar20 != (undefined8 *)0x0) {
            *puVar20 = 0;
            puVar20[1] = 0;
            lVar24 = puVar22[1];
            uVar16 = *puVar22;
            if (lVar24 != 0) {
              LOCK();
              piVar2 = (int *)(lVar24 + 8);
              *piVar2 = *piVar2 + 1;
              UNLOCK();
            }
            FUN_180015c40(puVar20,uVar16);
            puVar21[9] = puVar21[9] + 0x10;
            goto LAB_1800ef396;
          }
        }
        else {
          if (ppcVar5 == (char **)puVar21[10]) {
            FUN_1800f0e70(puVar1);
          }
          puVar20 = (undefined8 *)puVar21[9];
          if (puVar20 != (undefined8 *)0x0) {
            *puVar20 = 0;
            puVar20[1] = 0;
            if (plVar12 != (longlong *)0x0) {
              LOCK();
              *(int *)(plVar12 + 1) = (int)plVar12[1] + 1;
              UNLOCK();
              plVar12 = plStack_108;
              pcVar13 = pcStack_110;
            }
            FUN_180015c40(puVar20,pcVar13,plVar12);
          }
        }
        puVar21[9] = puVar21[9] + 0x10;
      }
    }
  }
LAB_1800ef396:
  plVar12 = plStack_120;
  if (plStack_120 != (longlong *)0x0) {
    LOCK();
    plVar19 = plStack_120 + 1;
    *(int *)plVar19 = (int)*plVar19 + -1;
    UNLOCK();
    if ((int)*plVar19 == 0) {
      (**(code **)*plStack_120)(plStack_120);
      LOCK();
      piVar2 = (int *)((longlong)plVar12 + 0xc);
      *piVar2 = *piVar2 + -1;
      UNLOCK();
      if (*piVar2 == 0) {
        (**(code **)(*plStack_120 + 8))();
      }
    }
  }
  iVar9 = _Mtx_unlock(lStack_c8);
  if (iVar9 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar9);
  }
LAB_1800ef3e5:
  FUN_1801e8810(uStack_58 ^ (ulonglong)auStack_178);
  return;
}

