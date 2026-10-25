
/* WARNING: Removing unreachable block (ram,0x0001800d4745) */
/* WARNING: Removing unreachable block (ram,0x0001800d474a) */
/* WARNING: Removing unreachable block (ram,0x0001800d4750) */
/* WARNING: Removing unreachable block (ram,0x0001800d4759) */
/* WARNING: Removing unreachable block (ram,0x0001800d477e) */
/* WARNING: Removing unreachable block (ram,0x0001800d4788) */
/* WARNING: Removing unreachable block (ram,0x0001800d478e) */
/* WARNING: Removing unreachable block (ram,0x0001800d479b) */
/* WARNING: Removing unreachable block (ram,0x0001800d4763) */
/* WARNING: Removing unreachable block (ram,0x0001800d4770) */
/* WARNING: Removing unreachable block (ram,0x0001800d477c) */
/* WARNING: Removing unreachable block (ram,0x0001800d479e) */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_1800d45b0(longlong param_1)

{
  longlong **pplVar1;
  longlong *plVar2;
  int *piVar3;
  longlong **pplVar4;
  longlong *plVar5;
  bool bVar6;
  ulonglong uVar7;
  longlong *plVar8;
  longlong lVar9;
  char cVar10;
  uint uVar11;
  int iVar12;
  int iVar13;
  undefined8 *puVar14;
  longlong lVar15;
  longlong *plVar16;
  undefined8 uVar17;
  undefined4 *puVar18;
  longlong *plVar19;
  undefined8 *puVar20;
  longlong *plVar21;
  undefined8 *puVar22;
  undefined8 *puVar23;
  undefined8 **ppuVar24;
  undefined4 uVar25;
  undefined4 extraout_XMM0_Da;
  undefined4 uVar26;
  undefined4 extraout_XMM0_Da_00;
  undefined4 extraout_XMM0_Da_01;
  undefined1 auStack_198 [32];
  longlong lStack_178;
  undefined8 uStack_170;
  longlong *plStack_168;
  undefined4 uStack_160;
  longlong *plStack_158;
  undefined8 uStack_150;
  longlong *plStack_148;
  undefined8 uStack_140;
  undefined4 uStack_138;
  int aiStack_130 [2];
  longlong lStack_128;
  undefined8 uStack_120;
  longlong *plStack_118;
  int iStack_110;
  undefined4 uStack_10c;
  undefined4 uStack_108;
  undefined4 uStack_104;
  longlong *plStack_100;
  undefined8 uStack_f8;
  longlong *plStack_f0;
  undefined8 *puStack_e8;
  longlong lStack_e0;
  int *piStack_d8;
  undefined8 *puStack_d0;
  undefined8 *puStack_c8;
  undefined8 *puStack_c0;
  undefined8 *puStack_b8;
  undefined4 uStack_b0;
  undefined4 uStack_ac;
  undefined4 uStack_a8;
  undefined4 uStack_a4;
  longlong *plStack_98;
  undefined8 uStack_90;
  longlong lStack_88;
  undefined1 auStack_80 [8];
  longlong lStack_78;
  longlong lStack_70;
  undefined1 auStack_68 [8];
  undefined1 auStack_60 [16];
  undefined8 auStack_50 [3];
  ulonglong uStack_38;
  ulonglong uStack_30;
  
  uStack_90 = 0xfffffffffffffffe;
  uStack_30 = _DAT_1802a5230 ^ (ulonglong)auStack_198;
  lStack_128 = param_1;
  lStack_e0 = param_1;
  do {
    while( true ) {
      uVar11 = WaitForSingleObject(*(undefined8 *)(param_1 + 8),0);
      if (((uVar11 & 0xffffff7f) == 0) ||
         ((uVar11 != 0x102 && (iVar12 = GetLastError(), iVar12 == 0)))) {
        FUN_1801e8810(uStack_30 ^ (ulonglong)auStack_198);
        return;
      }
      iVar12 = WaitForSingleObject(*(undefined8 *)(param_1 + 0x10));
      uVar26 = extraout_XMM0_Da;
      if (((iVar12 != 0) && (iVar12 != 0x80)) && (iVar12 != 0x102)) {
        uVar26 = GetLastError();
      }
      uStack_140 = 0;
      plStack_148 = (longlong *)FUN_18001f150(uVar26,0,0);
      plStack_158 = (longlong *)0x0;
      uStack_150 = 0;
      plStack_158 = (longlong *)FUN_1800703a0();
      lStack_78 = param_1;
      FUN_1801e5430(param_1);
      if ((*(longlong *)(param_1 + 0x58) != 0) || (*(longlong *)(param_1 + 0x88) != 0)) break;
      iVar12 = ResetEvent(*(undefined8 *)(param_1 + 0x10));
      if (iVar12 == 0) {
        GetLastError();
      }
      FUN_1801e5390(param_1);
      cVar10 = *(char *)(plStack_158[1] + 0x19);
      plVar21 = (longlong *)plStack_158[1];
      while (cVar10 == '\0') {
        FUN_18006e620(&plStack_158);
        plVar19 = (longlong *)*plVar21;
        if (0xf < (ulonglong)plVar21[9]) {
          __3_YAXPEAX_Z(plVar21[6]);
        }
        plVar21[9] = 0xf;
        plVar21[8] = 0;
        *(undefined1 *)(plVar21 + 6) = 0;
        __3_YAXPEAX_Z(plVar21);
        plVar21 = plVar19;
        cVar10 = *(char *)((longlong)plVar19 + 0x19);
      }
      plStack_158[1] = (longlong)plStack_158;
      *plStack_158 = (longlong)plStack_158;
      plStack_158[2] = (longlong)plStack_158;
      uStack_150 = 0;
      __3_YAXPEAX_Z(plStack_158);
      plVar21 = (longlong *)*plStack_148;
      *plStack_148 = (longlong)plStack_148;
      plStack_148[1] = (longlong)plStack_148;
      uStack_140 = 0;
      if (plVar21 != plStack_148) {
        do {
          plVar21 = (longlong *)*plVar21;
          __3_YAXPEAX_Z();
        } while (plVar21 != plStack_148);
      }
      __3_YAXPEAX_Z(plStack_148);
    }
    pplVar1 = (longlong **)(param_1 + 0x50);
    if (&plStack_148 != pplVar1) {
      plVar21 = *pplVar1;
      *pplVar1 = plStack_148;
      uStack_140 = *(undefined8 *)(param_1 + 0x58);
      *(undefined8 *)(param_1 + 0x58) = 0;
      plStack_148 = plVar21;
    }
    pplVar4 = (longlong **)(param_1 + 0x80);
    if (&plStack_158 != pplVar4) {
      plVar21 = *pplVar4;
      *pplVar4 = plStack_158;
      uVar17 = *(undefined8 *)(param_1 + 0x88);
      *(undefined8 *)(param_1 + 0x88) = uStack_150;
      plStack_158 = plVar21;
      uStack_150 = uVar17;
    }
    plVar21 = *pplVar1;
    plVar19 = (longlong *)*plVar21;
    *plVar21 = (longlong)plVar21;
    (*pplVar1)[1] = (longlong)*pplVar1;
    *(undefined8 *)(param_1 + 0x58) = 0;
    if (plVar19 != *pplVar1) {
      do {
        plVar19 = (longlong *)*plVar19;
        __3_YAXPEAX_Z();
      } while (plVar19 != *pplVar1);
    }
    FUN_18006e620(pplVar4,(*pplVar4)[1]);
    (*pplVar4)[1] = (longlong)*pplVar4;
    **pplVar4 = (longlong)*pplVar4;
    (*pplVar4)[2] = (longlong)*pplVar4;
    *(undefined8 *)(param_1 + 0x88) = 0;
    FUN_1801e5390(param_1);
    plVar19 = plStack_158;
    plVar21 = (longlong *)*plStack_158;
    lVar9 = param_1;
    lVar15 = lStack_88;
    while (lStack_88 = lVar9, plVar21 != plVar19) {
      plVar16 = plVar21 + 6;
      if (0xf < (ulonglong)plVar21[9]) {
        plVar16 = (longlong *)*plVar16;
      }
      if (_DAT_1802a5260 != (code *)0x0) {
        (*_DAT_1802a5260)(plVar16,(int)plVar21[8],_DAT_1802b1188);
      }
      FUN_1801e5430(param_1);
      puVar14 = *(undefined8 **)(param_1 + 0x90);
      puVar23 = puVar14;
      if (*(char *)((longlong)puVar14[1] + 0x19) == '\0') {
        puVar22 = (undefined8 *)puVar14[1];
        do {
          if ((*(int *)(puVar22 + 4) < (int)plVar21[4]) ||
             ((*(int *)(puVar22 + 4) <= (int)plVar21[4] &&
              ((ulonglong)puVar22[5] < (ulonglong)plVar21[5])))) {
            puVar20 = (undefined8 *)puVar22[2];
          }
          else {
            puVar20 = (undefined8 *)*puVar22;
            puVar23 = puVar22;
          }
          puVar22 = puVar20;
        } while (*(char *)((longlong)puVar20 + 0x19) == '\0');
      }
      if (puVar23 == puVar14) {
LAB_1800d4980:
        puStack_e8 = puVar14;
        ppuVar24 = &puStack_e8;
      }
      else {
        if (((int)plVar21[4] < *(int *)(puVar23 + 4)) ||
           (((int)plVar21[4] <= *(int *)(puVar23 + 4) &&
            ((ulonglong)plVar21[5] < (ulonglong)puVar23[5])))) goto LAB_1800d4980;
        puStack_b8 = puVar23;
        ppuVar24 = &puStack_b8;
      }
      if (*ppuVar24 != puVar14) {
        FUN_18006fdd0(param_1 + 0x90,auStack_80);
      }
      puVar14 = (undefined8 *)FUN_1800d6310(param_1 + 0x30,plVar21 + 4);
      iVar12 = SetEvent(*puVar14);
      if (iVar12 == 0) {
        GetLastError();
      }
      FUN_1801e5390(param_1);
      lVar9 = lStack_88;
      lVar15 = lStack_88;
      if (*(char *)((longlong)plVar21 + 0x19) == '\0') {
        plVar16 = (longlong *)plVar21[2];
        if (*(char *)((longlong)plVar16 + 0x19) == '\0') {
          cVar10 = *(char *)(*plVar16 + 0x19);
          plVar21 = plVar16;
          plVar16 = (longlong *)*plVar16;
          while (cVar10 == '\0') {
            cVar10 = *(char *)(*plVar16 + 0x19);
            plVar21 = plVar16;
            plVar16 = (longlong *)*plVar16;
          }
        }
        else {
          cVar10 = *(char *)(plVar21[1] + 0x19);
          plVar8 = (longlong *)plVar21[1];
          plVar16 = plVar21;
          while ((plVar21 = plVar8, cVar10 == '\0' && (plVar16 == (longlong *)plVar21[2]))) {
            cVar10 = *(char *)(plVar21[1] + 0x19);
            plVar8 = (longlong *)plVar21[1];
            plVar16 = plVar21;
          }
        }
      }
    }
    lStack_88 = lVar15;
    (**(code **)(*(longlong *)(param_1 + 0xa0) + 0x30))();
    plVar19 = plStack_148;
    uStack_f8 = 0;
    uStack_138 = 0;
    uVar26 = 0;
    uStack_120 = 0;
    plStack_118 = (longlong *)0x0;
    plVar21 = (longlong *)*plStack_148;
    plStack_f0 = plStack_148;
    for (; plStack_100 = plVar21, plVar21 != plVar19; plVar21 = (longlong *)*plVar21) {
      uVar7 = plVar21[2];
      plVar8 = (longlong *)plVar21[3];
      iVar12 = (int)uVar7;
      aiStack_130[0] = iVar12;
      lVar15 = FUN_18008e6c0(plVar8);
      plVar16 = (longlong *)(lVar15 + 0x60);
      puVar14 = (undefined8 *)*plVar16;
      cVar10 = *(char *)((longlong)puVar14[1] + 0x19);
      puVar23 = puVar14;
      puVar22 = (undefined8 *)puVar14[1];
      while (cVar10 == '\0') {
        if (*(int *)(puVar22 + 4) < aiStack_130[0]) {
          puVar20 = (undefined8 *)puVar22[2];
          puVar22 = puVar23;
        }
        else {
          puVar20 = (undefined8 *)*puVar22;
        }
        puVar23 = puVar22;
        puVar22 = puVar20;
        cVar10 = *(char *)((longlong)puVar20 + 0x19);
      }
      if ((puVar23 == puVar14) ||
         (uVar26 = extraout_XMM0_Da_00, aiStack_130[0] < *(int *)(puVar23 + 4))) {
        piStack_d8 = aiStack_130;
        lStack_178 = FUN_1800d0f90(plVar16,aiStack_130[0],&piStack_d8);
        uVar26 = FUN_1800d1010(plVar16,&puStack_c8,puVar23,lStack_178 + 0x20);
        puVar23 = puStack_c8;
      }
      plVar16 = plStack_118;
      if (puVar23[6] == 0) {
        iVar13 = -0x1ffffffd;
        uVar25 = uStack_138;
      }
      else {
        lVar15 = *(longlong *)puVar23[5];
        uStack_f8 = *(undefined8 *)(lVar15 + 0x30);
        uVar25 = *(undefined4 *)(lVar15 + 0x20);
        plVar5 = *(longlong **)(lVar15 + 0x18);
        uStack_120 = *(undefined8 *)(lVar15 + 0x10);
        if (plVar5 != (longlong *)0x0) {
          LOCK();
          *(int *)(plVar5 + 1) = (int)plVar5[1] + 1;
          UNLOCK();
        }
        plStack_98 = plStack_118;
        uStack_138 = uVar25;
        if (plStack_118 != (longlong *)0x0) {
          LOCK();
          plVar2 = plStack_118 + 1;
          *(int *)plVar2 = (int)*plVar2 + -1;
          UNLOCK();
          if ((int)*plVar2 == 0) {
            puVar14 = (undefined8 *)*plStack_118;
            plStack_118 = plVar5;
            uVar26 = (*(code *)*puVar14)(plVar16);
            LOCK();
            piVar3 = (int *)((longlong)plVar16 + 0xc);
            *piVar3 = *piVar3 + -1;
            UNLOCK();
            plVar5 = plStack_118;
            if (*piVar3 == 0) {
              uVar26 = (**(code **)(*plStack_98 + 8))();
              plVar5 = plStack_118;
            }
          }
        }
        plStack_118 = plVar5;
        iVar13 = 0;
      }
      if (iVar13 == 0) {
        uStack_170 = uStack_120;
        lStack_178 = CONCAT44(lStack_178._4_4_,uVar25);
        iVar13 = FUN_1800d5d20(uVar26,lStack_128 + 0xa0,uVar7 & 0xffffffff,uStack_f8);
        uVar26 = extraout_XMM0_Da_01;
        if (iVar13 == 0) {
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
            uVar26 = FUN_1801e8930(&UNK_180202850);
          }
          plVar16 = _DAT_1802b1240;
          lVar15 = _DAT_1802b1238;
          if (_DAT_1802b1240 != (longlong *)0x0) {
            LOCK();
            *(int *)(_DAT_1802b1240 + 1) = (int)_DAT_1802b1240[1] + 1;
            UNLOCK();
          }
          if (lVar15 == 0) {
            if (plVar16 != (longlong *)0x0) {
              LOCK();
              plVar8 = plVar16 + 1;
              *(int *)plVar8 = (int)*plVar8 + -1;
              UNLOCK();
              if ((int)*plVar8 == 0) {
                uVar26 = (**(code **)*plVar16)(plVar16);
                LOCK();
                piVar3 = (int *)((longlong)plVar16 + 0xc);
                *piVar3 = *piVar3 + -1;
                UNLOCK();
                if (*piVar3 == 0) {
                  uVar26 = (**(code **)(*plVar16 + 8))(plVar16);
                }
              }
            }
          }
          else {
            iVar13 = *(int *)(lVar15 + 0x40);
            if (plVar16 != (longlong *)0x0) {
              LOCK();
              plVar5 = plVar16 + 1;
              *(int *)plVar5 = (int)*plVar5 + -1;
              UNLOCK();
              if ((int)*plVar5 == 0) {
                uVar26 = (**(code **)*plVar16)(plVar16);
                LOCK();
                piVar3 = (int *)((longlong)plVar16 + 0xc);
                *piVar3 = *piVar3 + -1;
                UNLOCK();
                if (*piVar3 == 0) {
                  uVar26 = (**(code **)(*plVar16 + 8))(plVar16);
                }
              }
            }
            if (iVar13 < 2) {
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
              uStack_160 = uStack_138;
              uStack_170 = CONCAT44(uStack_170._4_4_,iVar12);
              lStack_178 = CONCAT44(lStack_178._4_4_,0x195);
              plStack_168 = plVar8;
              uVar26 = FUN_180015b40(&DAT_1802b1230,1,&UNK_18022bef0,&UNK_18022be28);
            }
          }
        }
        else {
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
            uVar26 = FUN_1801e8930(&UNK_180202850);
          }
          plVar16 = _DAT_1802b1240;
          lVar15 = _DAT_1802b1238;
          if (_DAT_1802b1240 != (longlong *)0x0) {
            LOCK();
            *(int *)(_DAT_1802b1240 + 1) = (int)_DAT_1802b1240[1] + 1;
            UNLOCK();
          }
          if (lVar15 == 0) {
            if (plVar16 != (longlong *)0x0) {
              LOCK();
              plVar8 = plVar16 + 1;
              *(int *)plVar8 = (int)*plVar8 + -1;
              UNLOCK();
              if ((int)*plVar8 == 0) {
                uVar26 = (**(code **)*plVar16)(plVar16);
                LOCK();
                piVar3 = (int *)((longlong)plVar16 + 0xc);
                *piVar3 = *piVar3 + -1;
                UNLOCK();
                if (*piVar3 == 0) {
                  uVar26 = (**(code **)(*plVar16 + 8))(plVar16);
                }
              }
            }
          }
          else {
            iVar12 = *(int *)(lVar15 + 0x40);
            if (plVar16 != (longlong *)0x0) {
              LOCK();
              plVar8 = plVar16 + 1;
              *(int *)plVar8 = (int)*plVar8 + -1;
              UNLOCK();
              if ((int)*plVar8 == 0) {
                uVar26 = (**(code **)*plVar16)(plVar16);
                LOCK();
                piVar3 = (int *)((longlong)plVar16 + 0xc);
                *piVar3 = *piVar3 + -1;
                UNLOCK();
                if (*piVar3 == 0) {
                  uVar26 = (**(code **)(*plVar16 + 8))(plVar16);
                }
              }
            }
            if (iVar12 < 5) {
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
              lStack_178 = CONCAT44(lStack_178._4_4_,0x192);
              uVar26 = FUN_180015b40(&DAT_1802b1230,4,&UNK_1802285f0,&UNK_18022be28);
            }
          }
        }
      }
      else {
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
          uVar26 = FUN_1801e8930(&UNK_180202850);
        }
        plVar16 = _DAT_1802b1240;
        lVar15 = _DAT_1802b1238;
        if (_DAT_1802b1240 != (longlong *)0x0) {
          LOCK();
          *(int *)(_DAT_1802b1240 + 1) = (int)_DAT_1802b1240[1] + 1;
          UNLOCK();
        }
        if (lVar15 == 0) {
          if (plVar16 != (longlong *)0x0) {
            LOCK();
            plVar8 = plVar16 + 1;
            *(int *)plVar8 = (int)*plVar8 + -1;
            UNLOCK();
            if ((int)*plVar8 == 0) {
              uVar26 = (**(code **)*plVar16)(plVar16);
              LOCK();
              piVar3 = (int *)((longlong)plVar16 + 0xc);
              *piVar3 = *piVar3 + -1;
              UNLOCK();
              if (*piVar3 == 0) {
                uVar26 = (**(code **)(*plVar16 + 8))(plVar16);
              }
            }
          }
          bVar6 = false;
        }
        else {
          bVar6 = *(int *)(lVar15 + 0x40) < 5;
          if (plVar16 != (longlong *)0x0) {
            LOCK();
            plVar8 = plVar16 + 1;
            *(int *)plVar8 = (int)*plVar8 + -1;
            UNLOCK();
            if ((int)*plVar8 == 0) {
              uVar26 = (**(code **)*plVar16)(plVar16);
              LOCK();
              piVar3 = (int *)((longlong)plVar16 + 0xc);
              *piVar3 = *piVar3 + -1;
              UNLOCK();
              if (*piVar3 == 0) {
                uVar26 = (**(code **)(*plVar16 + 8))(plVar16);
              }
            }
          }
        }
        if (bVar6) {
          plVar16 = (longlong *)FUN_18008f750(auStack_50,iVar13);
          if (0xf < (ulonglong)plVar16[3]) {
            plVar16 = (longlong *)*plVar16;
          }
          uVar17 = FUN_18001db50();
          uStack_170 = CONCAT44(uStack_170._4_4_,iVar12);
          lStack_178 = CONCAT44(lStack_178._4_4_,0x199);
          plStack_168 = plVar16;
          uVar26 = FUN_180015b40(uVar17,4,&UNK_18022bea8,&UNK_18022be28);
          if (0xf < uStack_38) {
            uVar26 = __3_YAXPEAX_Z(auStack_50[0]);
          }
        }
      }
      param_1 = lStack_128;
    }
    iVar12 = FUN_1800d6010(uVar26,param_1 + 0xa0,param_1 + 0xd0,param_1 + 0xe0);
    if ((iVar12 != 0) && (cVar10 = FUN_18001dbc0(4), cVar10 != '\0')) {
      uVar17 = FUN_18001db50();
      uStack_170 = CONCAT44(uStack_170._4_4_,iVar12);
      lStack_178 = CONCAT44(lStack_178._4_4_,0x1a6);
      FUN_180015b40(uVar17,4,&UNK_18022bf40,&UNK_18022be28);
    }
    lStack_70 = param_1;
    FUN_1801e5430(param_1);
    plVar21 = (longlong *)*plStack_148;
    if (plVar21 != plStack_148) {
      do {
        iStack_110 = (int)plVar21[2];
        uStack_10c = *(undefined4 *)((longlong)plVar21 + 0x14);
        uStack_108 = (undefined4)plVar21[3];
        uStack_104 = *(undefined4 *)((longlong)plVar21 + 0x1c);
        puVar18 = (undefined4 *)FUN_1800d6540(param_1 + 0x70,&iStack_110);
        uStack_b0 = *puVar18;
        uStack_ac = puVar18[1];
        uStack_a8 = puVar18[2];
        uStack_a4 = puVar18[3];
        plVar19 = (longlong *)FUN_1800d6480(param_1 + 0x60,&uStack_b0);
        puVar14 = (undefined8 *)*plVar19;
        cVar10 = *(char *)((longlong)puVar14[1] + 0x19);
        puVar23 = puVar14;
        puVar22 = (undefined8 *)puVar14[1];
        while (cVar10 == '\0') {
          if ((*(int *)(puVar22 + 4) < iStack_110) ||
             ((*(int *)(puVar22 + 4) <= iStack_110 &&
              ((ulonglong)puVar22[5] < CONCAT44(uStack_104,uStack_108))))) {
            puVar20 = (undefined8 *)puVar22[2];
          }
          else {
            puVar20 = (undefined8 *)*puVar22;
            puVar23 = puVar22;
          }
          puVar22 = puVar20;
          cVar10 = *(char *)((longlong)puVar20 + 0x19);
        }
        if (((puVar23 == puVar14) || (iStack_110 < *(int *)(puVar23 + 4))) ||
           ((iStack_110 <= *(int *)(puVar23 + 4) &&
            (CONCAT44(uStack_104,uStack_108) < (ulonglong)puVar23[5])))) {
          puStack_d0 = puVar14;
          ppuVar24 = &puStack_d0;
        }
        else {
          puStack_c0 = puVar23;
          ppuVar24 = &puStack_c0;
        }
        if (*ppuVar24 != puVar14) {
          FUN_18006fdd0(plVar19,auStack_60);
        }
        if (plVar19[1] == 0) {
          puVar14 = (undefined8 *)FUN_1800d6310(param_1 + 0x30,&uStack_b0);
          iVar12 = SetEvent(*puVar14);
          if (iVar12 == 0) {
            GetLastError();
          }
        }
        puVar14 = (undefined8 *)FUN_1800d6310(param_1 + 0x20,&iStack_110);
        iVar12 = SetEvent(*puVar14);
        if (iVar12 == 0) {
          GetLastError();
        }
        cVar10 = FUN_18001dbc0(2);
        if (cVar10 != '\0') {
          uVar17 = FUN_18001db50();
          uStack_170 = CONCAT44(uStack_170._4_4_,iStack_110);
          lStack_178 = CONCAT44(lStack_178._4_4_,0x1c1);
          FUN_180015b40(uVar17,2,&UNK_18022bfb8,&UNK_18022be28);
        }
        plVar21 = (longlong *)*plVar21;
      } while (plVar21 != plStack_148);
    }
    FUN_1801e5390(param_1);
    plVar21 = plStack_118;
    if (plStack_118 != (longlong *)0x0) {
      LOCK();
      plVar19 = plStack_118 + 1;
      *(int *)plVar19 = (int)*plVar19 + -1;
      UNLOCK();
      if ((int)*plVar19 == 0) {
        (**(code **)*plStack_118)(plStack_118);
        LOCK();
        piVar3 = (int *)((longlong)plVar21 + 0xc);
        *piVar3 = *piVar3 + -1;
        UNLOCK();
        if (*piVar3 == 0) {
          (**(code **)(*plStack_118 + 8))();
        }
      }
    }
    FUN_18006ec00(&plStack_158,auStack_68,*plStack_158,plStack_158);
    __3_YAXPEAX_Z(plStack_158);
    plVar21 = (longlong *)*plStack_148;
    *plStack_148 = (longlong)plStack_148;
    plStack_148[1] = (longlong)plStack_148;
    uStack_140 = 0;
    if (plVar21 != plStack_148) {
      do {
        plVar21 = (longlong *)*plVar21;
        __3_YAXPEAX_Z();
      } while (plVar21 != plStack_148);
    }
    __3_YAXPEAX_Z(plStack_148);
  } while( true );
}

