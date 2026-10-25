
/* WARNING: Removing unreachable block (ram,0x0001800e958b) */
/* WARNING: Removing unreachable block (ram,0x0001800e9590) */
/* WARNING: Removing unreachable block (ram,0x0001800e9599) */
/* WARNING: Removing unreachable block (ram,0x0001800e95be) */
/* WARNING: Removing unreachable block (ram,0x0001800e95c8) */
/* WARNING: Removing unreachable block (ram,0x0001800e95ce) */
/* WARNING: Removing unreachable block (ram,0x0001800e95db) */
/* WARNING: Removing unreachable block (ram,0x0001800e95a3) */
/* WARNING: Removing unreachable block (ram,0x0001800e95b0) */
/* WARNING: Removing unreachable block (ram,0x0001800e95bc) */
/* WARNING: Removing unreachable block (ram,0x0001800e95de) */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_1800e8da0(longlong param_1,int param_2,longlong *param_3)

{
  int iVar1;
  int iVar2;
  longlong *plVar3;
  char cVar4;
  int iVar5;
  longlong lVar6;
  undefined8 uVar7;
  undefined8 *puVar8;
  longlong *plVar9;
  longlong lVar10;
  undefined8 *puVar11;
  int *piVar12;
  undefined8 uVar13;
  longlong lVar14;
  undefined8 uVar15;
  undefined8 *puVar16;
  longlong *plVar17;
  undefined8 *puVar18;
  undefined8 *puVar19;
  undefined8 *puVar20;
  undefined8 *puVar21;
  longlong *plVar22;
  undefined4 extraout_XMM0_Da;
  undefined4 extraout_XMM0_Da_00;
  undefined4 uVar23;
  undefined4 extraout_XMM0_Da_01;
  undefined4 extraout_XMM0_Da_02;
  undefined4 extraout_XMM0_Da_03;
  undefined4 extraout_XMM0_Da_04;
  undefined4 extraout_XMM0_Da_05;
  undefined4 extraout_XMM0_Da_06;
  undefined4 extraout_XMM0_Da_07;
  undefined1 auStack_338 [32];
  longlong **pplStack_318;
  int iStack_310;
  int iStack_308;
  undefined8 *puStack_300;
  int aiStack_2f8 [2];
  int iStack_2f0;
  int iStack_2ec;
  undefined8 *puStack_2e8;
  undefined8 uStack_2e0;
  undefined8 *puStack_2d8;
  undefined8 uStack_2d0;
  undefined8 uStack_2c8;
  longlong *plStack_2c0;
  undefined4 auStack_2b8 [2];
  undefined8 uStack_2b0;
  longlong *plStack_2a8;
  longlong *plStack_2a0;
  undefined8 uStack_298;
  longlong *plStack_290;
  undefined8 uStack_288;
  longlong *plStack_280;
  undefined8 uStack_278;
  longlong lStack_270;
  longlong *plStack_268;
  undefined1 auStack_260 [8];
  int aiStack_258 [2];
  longlong lStack_250;
  longlong *plStack_248;
  undefined1 auStack_240 [8];
  undefined4 auStack_238 [2];
  undefined1 auStack_230 [8];
  longlong lStack_228;
  longlong *plStack_220;
  longlong lStack_218;
  undefined1 auStack_210 [16];
  undefined8 uStack_200;
  longlong *plStack_1f8;
  undefined8 *puStack_1f0;
  int *piStack_1e8;
  undefined1 auStack_1e0 [8];
  int *piStack_1d8;
  undefined1 auStack_1d0 [16];
  undefined8 *puStack_1c0;
  int *piStack_1b8;
  undefined1 auStack_1b0 [8];
  int *piStack_1a8;
  int *piStack_1a0;
  undefined8 *puStack_198;
  longlong lStack_190;
  undefined8 *puStack_188;
  undefined8 *apuStack_180 [2];
  undefined8 *puStack_170;
  undefined1 auStack_168 [16];
  undefined1 auStack_158 [24];
  longlong *plStack_140;
  longlong *plStack_130;
  undefined8 uStack_128;
  undefined8 uStack_120;
  longlong *plStack_110;
  undefined1 auStack_108 [16];
  undefined1 auStack_f8 [8];
  longlong alStack_f0 [2];
  undefined8 uStack_e0;
  undefined1 auStack_d8 [16];
  undefined1 auStack_c8 [16];
  undefined1 auStack_b8 [16];
  undefined1 auStack_a8 [32];
  undefined8 auStack_88 [3];
  ulonglong uStack_70;
  undefined8 auStack_68 [3];
  ulonglong uStack_50;
  ulonglong uStack_48;
  
  uStack_e0 = 0xfffffffffffffffe;
  uStack_48 = _DAT_1802a5230 ^ (ulonglong)auStack_338;
  aiStack_2f8[0] = param_2;
  plStack_220 = param_3;
  lStack_190 = param_1;
  if ((_DAT_1802b1888 & 1) == 0) {
    _DAT_1802b1888 = _DAT_1802b1888 | 1;
    FUN_180107fc0();
    FUN_1801e8930(&UNK_180202980);
  }
  FUN_180108ba0(&DAT_1802b1890,&lStack_270,aiStack_2f8[0]);
  plVar17 = plStack_268;
  lVar14 = lStack_270;
  if (lStack_270 == 0) {
    if (plStack_268 != (longlong *)0x0) {
      LOCK();
      plVar22 = plStack_268 + 1;
      *(int *)plVar22 = (int)*plVar22 + -1;
      UNLOCK();
      if ((int)*plVar22 == 0) {
        (**(code **)*plStack_268)(plStack_268);
        LOCK();
        piVar12 = (int *)((longlong)plVar17 + 0xc);
        *piVar12 = *piVar12 + -1;
        UNLOCK();
        if (*piVar12 == 0) {
          (**(code **)(*plStack_268 + 8))();
        }
      }
    }
  }
  else {
    lVar6 = __2_YAPEAX_K_Z(0x250);
    lStack_228 = lVar6;
    if (lVar6 == 0) {
      uVar7 = 0;
    }
    else {
      uVar7 = FUN_1800dcb60(lVar14,auStack_a8);
      uVar7 = FUN_1800dc300(lVar6,aiStack_2f8[0],uVar7);
    }
    uStack_200 = 0;
    plStack_1f8 = (longlong *)0x0;
    FUN_1800eb5b0(&uStack_200,uVar7);
    FUN_18001d870(param_3,&uStack_200);
    plVar17 = plStack_1f8;
    if (plStack_1f8 != (longlong *)0x0) {
      LOCK();
      plVar22 = plStack_1f8 + 1;
      *(int *)plVar22 = (int)*plVar22 + -1;
      UNLOCK();
      lVar14 = lStack_270;
      if ((int)*plVar22 == 0) {
        (**(code **)*plStack_1f8)(plStack_1f8);
        LOCK();
        piVar12 = (int *)((longlong)plVar17 + 0xc);
        *piVar12 = *piVar12 + -1;
        UNLOCK();
        lVar14 = lStack_270;
        if (*piVar12 == 0) {
          (**(code **)(*plStack_1f8 + 8))();
          lVar14 = lStack_270;
        }
      }
    }
    if (*param_3 + 0xa8 != lVar14 + 0xa8) {
      FUN_180006470(*param_3 + 0xa8,lVar14 + 0xa8,0,0xffffffffffffffff);
    }
    if ((undefined8 *)(*param_3 + 0x150) != (undefined8 *)(lVar14 + 0x150)) {
      puVar8 = *(undefined8 **)(lVar14 + 0x150);
      FUN_1800e2940((undefined8 *)(*param_3 + 0x150),*puVar8,puVar8);
    }
    if ((undefined8 *)(*param_3 + 0x160) != (undefined8 *)(lVar14 + 0x160)) {
      puVar8 = *(undefined8 **)(lVar14 + 0x160);
      FUN_1800e2940((undefined8 *)(*param_3 + 0x160),*puVar8,puVar8);
    }
    *(undefined4 *)(*param_3 + 0x170) = *(undefined4 *)(lVar14 + 0x170);
    *(undefined1 *)(*param_3 + 400) = 0;
    *(undefined1 *)(*param_3 + 0x1a9) = *(undefined1 *)(lVar14 + 0x1a9);
    lVar6 = *param_3;
    *(undefined1 *)(lVar6 + 0xe8) = *(undefined1 *)(lVar14 + 0xe8);
    plStack_290 = (longlong *)0x0;
    uStack_288 = 0;
    plStack_290 = (longlong *)FUN_18001f150(lVar6,0,0);
    plStack_280 = (longlong *)0x0;
    uStack_278 = 0;
    plStack_280 = (longlong *)FUN_18001f150(extraout_XMM0_Da,0,0);
    plStack_2a0 = (longlong *)0x0;
    uStack_298 = 0;
    plStack_2a0 = (longlong *)FUN_18001f150(extraout_XMM0_Da_00,0,0);
    if ((_DAT_1802b1888 & 1) == 0) {
      _DAT_1802b1888 = _DAT_1802b1888 | 1;
      FUN_180107fc0();
      FUN_1801e8930(&UNK_180202980);
    }
    pplStack_318 = &plStack_2a0;
    iVar5 = FUN_180115440(&DAT_1802b1890,aiStack_2f8[0],&plStack_290,&plStack_280);
    if (iVar5 == 0) {
      puStack_2e8 = (undefined8 *)0x0;
      uStack_2e0 = 0;
      puStack_2e8 = (undefined8 *)FUN_18003c7f0();
      puStack_2d8 = (undefined8 *)0x0;
      uStack_2d0 = 0;
      puStack_2d8 = (undefined8 *)FUN_18003c7f0();
      plVar22 = plStack_290;
      uVar23 = 0;
      uStack_2b0 = 0;
      plStack_2a8 = (longlong *)0x0;
      plVar17 = (longlong *)*plStack_290;
      iVar5 = 0;
LAB_1800e92c0:
      if (plVar17 != plVar22) {
        iStack_2ec = *(int *)(plVar17[2] + 0x2c);
        iVar5 = FUN_1800e8880(uVar23,iStack_2ec,&uStack_2b0);
        if (iVar5 == 0) goto LAB_1800e9685;
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
        plVar17 = _DAT_1802b1240;
        lVar14 = _DAT_1802b1238;
        if (_DAT_1802b1240 != (longlong *)0x0) {
          LOCK();
          *(int *)(_DAT_1802b1240 + 1) = (int)_DAT_1802b1240[1] + 1;
          UNLOCK();
        }
        if (lVar14 == 0) {
          if (plVar17 != (longlong *)0x0) {
            LOCK();
            plVar22 = plVar17 + 1;
            lVar14 = *plVar22;
            *(int *)plVar22 = (int)*plVar22 + -1;
            UNLOCK();
            if ((int)lVar14 == 1) {
              (**(code **)*plVar17)(plVar17);
              LOCK();
              piVar12 = (int *)((longlong)plVar17 + 0xc);
              iVar1 = *piVar12;
              *piVar12 = *piVar12 + -1;
              UNLOCK();
              if (iVar1 == 1) {
                (**(code **)(*plVar17 + 8))(plVar17);
              }
            }
          }
        }
        else {
          iVar1 = *(int *)(lVar14 + 0x40);
          if (plVar17 != (longlong *)0x0) {
            LOCK();
            plVar22 = plVar17 + 1;
            lVar14 = *plVar22;
            *(int *)plVar22 = (int)*plVar22 + -1;
            UNLOCK();
            if ((int)lVar14 == 1) {
              (**(code **)*plVar17)(plVar17);
              LOCK();
              piVar12 = (int *)((longlong)plVar17 + 0xc);
              iVar2 = *piVar12;
              *piVar12 = *piVar12 + -1;
              UNLOCK();
              if (iVar2 == 1) {
                (**(code **)(*plVar17 + 8))(plVar17);
              }
            }
          }
          if (iVar1 < 5) {
            puStack_300 = (undefined8 *)FUN_18008f750(auStack_88,iVar5);
            if (0xf < (ulonglong)puStack_300[3]) {
              puStack_300 = (undefined8 *)*puStack_300;
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
            iStack_308 = aiStack_2f8[0];
            iStack_310 = iStack_2ec;
            pplStack_318 = (longlong **)CONCAT44(pplStack_318._4_4_,0xb0);
            FUN_180015b40(&DAT_1802b1230,4,&UNK_18022dc30,&UNK_18022dbd8);
            if (0xf < uStack_70) {
              __3_YAXPEAX_Z(auStack_88[0]);
            }
          }
        }
        plVar17 = plStack_2a8;
        if (iVar5 != 0) {
          if (plStack_2a8 != (longlong *)0x0) {
            LOCK();
            plVar22 = plStack_2a8 + 1;
            lVar14 = *plVar22;
            *(int *)plVar22 = (int)*plVar22 + -1;
            UNLOCK();
            if ((int)lVar14 == 1) {
              (**(code **)*plStack_2a8)(plStack_2a8);
              LOCK();
              piVar12 = (int *)((longlong)plVar17 + 0xc);
              iVar5 = *piVar12;
              *piVar12 = *piVar12 + -1;
              UNLOCK();
              if (iVar5 == 1) {
                (**(code **)(*plStack_2a8 + 8))();
              }
            }
          }
          cVar4 = *(char *)((longlong)puStack_2d8[1] + 0x19);
          plVar17 = (longlong *)puStack_2d8[1];
          while (cVar4 == '\0') {
            FUN_18006e810(&puStack_2d8,plVar17[2]);
            plVar22 = (longlong *)*plVar17;
            plVar9 = (longlong *)plVar17[6];
            if (plVar9 != (longlong *)0x0) {
              LOCK();
              plVar3 = plVar9 + 1;
              lVar14 = *plVar3;
              *(int *)plVar3 = (int)*plVar3 + -1;
              UNLOCK();
              if ((int)lVar14 == 1) {
                (**(code **)*plVar9)(plVar9);
                LOCK();
                piVar12 = (int *)((longlong)plVar9 + 0xc);
                iVar5 = *piVar12;
                *piVar12 = *piVar12 + -1;
                UNLOCK();
                if (iVar5 == 1) {
                  (**(code **)(*plVar9 + 8))(plVar9);
                }
              }
            }
            __3_YAXPEAX_Z(plVar17);
            plVar17 = plVar22;
            cVar4 = *(char *)((longlong)plVar22 + 0x19);
          }
          puStack_2d8[1] = puStack_2d8;
          *puStack_2d8 = puStack_2d8;
          puStack_2d8[2] = puStack_2d8;
          uStack_2d0 = 0;
          __3_YAXPEAX_Z(puStack_2d8);
          plVar17 = alStack_f0;
          goto LAB_1800e960c;
        }
      }
      plVar22 = plStack_2a0;
      uVar23 = 0;
      uStack_2c8 = 0;
      plStack_2c0 = (longlong *)0x0;
      plVar17 = (longlong *)*plStack_2a0;
LAB_1800e9865:
      if (plVar17 != plVar22) {
        iStack_2f0 = *(int *)(plVar17[2] + 0x2c);
        iVar5 = FUN_1800e8ab0(uVar23,iStack_2f0,&uStack_2c8);
        if (iVar5 == 0) goto LAB_1800e99cd;
        cVar4 = FUN_18001dbc0(4);
        if (cVar4 != '\0') {
          puVar8 = (undefined8 *)FUN_18008f750(auStack_68,iVar5);
          if (0xf < (ulonglong)puVar8[3]) {
            puVar8 = (undefined8 *)*puVar8;
          }
          uVar7 = FUN_18001db50();
          iStack_308 = aiStack_2f8[0];
          iStack_310 = iStack_2f0;
          pplStack_318 = (longlong **)CONCAT44(pplStack_318._4_4_,0xc3);
          puStack_300 = puVar8;
          FUN_180015b40(uVar7,4,&UNK_18022dd00,&UNK_18022dbd8);
          if (0xf < uStack_50) {
            __3_YAXPEAX_Z(auStack_68[0]);
          }
        }
        plVar17 = plStack_2c0;
        if (iVar5 != 0) {
          if (plStack_2c0 != (longlong *)0x0) {
            LOCK();
            plVar22 = plStack_2c0 + 1;
            lVar14 = *plVar22;
            *(int *)plVar22 = (int)*plVar22 + -1;
            UNLOCK();
            if ((int)lVar14 == 1) {
              (**(code **)*plStack_2c0)(plStack_2c0);
              LOCK();
              piVar12 = (int *)((longlong)plVar17 + 0xc);
              iVar5 = *piVar12;
              *piVar12 = *piVar12 + -1;
              UNLOCK();
              if (iVar5 == 1) {
                (**(code **)(*plStack_2c0 + 8))();
              }
            }
          }
          plVar17 = plStack_2a8;
          if (plStack_2a8 != (longlong *)0x0) {
            LOCK();
            plVar22 = plStack_2a8 + 1;
            lVar14 = *plVar22;
            *(int *)plVar22 = (int)*plVar22 + -1;
            UNLOCK();
            if ((int)lVar14 == 1) {
              (**(code **)*plStack_2a8)(plStack_2a8);
              LOCK();
              piVar12 = (int *)((longlong)plVar17 + 0xc);
              iVar5 = *piVar12;
              *piVar12 = *piVar12 + -1;
              UNLOCK();
              if (iVar5 == 1) {
                (**(code **)(*plStack_2a8 + 8))();
              }
            }
          }
          FUN_18006ee20(&puStack_2d8,auStack_f8,*puStack_2d8,puStack_2d8);
          __3_YAXPEAX_Z(puStack_2d8);
          plVar17 = &lStack_218;
LAB_1800e960c:
          FUN_18006ee20(&puStack_2e8,plVar17,*puStack_2e8,puStack_2e8);
          __3_YAXPEAX_Z(puStack_2e8);
          FUN_18001ee90(&plStack_2a0);
          __3_YAXPEAX_Z(plStack_2a0);
          FUN_18001ee90(&plStack_280);
          __3_YAXPEAX_Z(plStack_280);
          FUN_18001ee90(&plStack_290);
          __3_YAXPEAX_Z(plStack_290);
          goto LAB_1800ea3bb;
        }
      }
      puVar19 = puStack_2e8;
      puVar8 = (undefined8 *)*puStack_2e8;
      while (puVar8 != puVar19) {
        if ((_DAT_1802b1888 & 1) == 0) {
          _DAT_1802b1888 = _DAT_1802b1888 | 1;
          FUN_180107fc0();
          FUN_1801e8930(&UNK_180202980);
        }
        FUN_18010a720(&DAT_1802b1890,&lStack_250,*(undefined4 *)(puVar8 + 4));
        if (lStack_250 == 0) {
          cVar4 = FUN_18001dbc0(4);
          if (cVar4 != '\0') {
            uVar7 = FUN_18001db50();
            iStack_310 = *(int *)(puVar8 + 4);
            pplStack_318 = (longlong **)CONCAT44(pplStack_318._4_4_,0xd6);
            FUN_180015b40(uVar7,4,&UNK_18022dcc8,&UNK_18022dbd8);
          }
          plVar17 = plStack_248;
          if (plStack_248 != (longlong *)0x0) {
            LOCK();
            plVar22 = plStack_248 + 1;
            lVar14 = *plVar22;
            *(int *)plVar22 = (int)*plVar22 + -1;
            UNLOCK();
            if ((int)lVar14 == 1) {
              (**(code **)*plStack_248)(plStack_248);
              LOCK();
              piVar12 = (int *)((longlong)plVar17 + 0xc);
              iVar5 = *piVar12;
              *piVar12 = *piVar12 + -1;
              UNLOCK();
              if (iVar5 == 1) {
                (**(code **)(*plStack_248 + 8))();
              }
            }
          }
          goto LAB_1800ea362;
        }
        FUN_18001e310(lStack_250,apuStack_180);
        puVar16 = apuStack_180[0];
        for (puVar18 = (undefined8 *)*apuStack_180[0]; puVar18 != puVar16;
            puVar18 = (undefined8 *)*puVar18) {
          aiStack_258[0] = *(int *)(puVar18[2] + 0x2c);
          cVar4 = *(char *)((longlong)puStack_2e8[1] + 0x19);
          puVar11 = (undefined8 *)puStack_2e8[1];
          puVar21 = puStack_2e8;
          while (puVar20 = puVar11, cVar4 == '\0') {
            if (*(int *)(puVar20 + 4) < aiStack_258[0]) {
              puVar11 = (undefined8 *)puVar20[2];
              puVar20 = puVar21;
            }
            else {
              puVar11 = (undefined8 *)*puVar20;
            }
            cVar4 = *(char *)((longlong)puVar11 + 0x19);
            puVar21 = puVar20;
          }
          if ((puVar21 == puStack_2e8) || (aiStack_258[0] < *(int *)(puVar21 + 4))) {
            piStack_1e8 = aiStack_258;
            pplStack_318 = (longlong **)FUN_180085630(&puStack_2e8,puStack_2e8,&piStack_1e8);
            FUN_180085670(&puStack_2e8,&puStack_188,puVar21,pplStack_318 + 4);
            puVar21 = puStack_188;
          }
          uStack_128 = 0;
          uStack_120 = 0;
          uVar7 = puVar21[5];
          if (puVar21[6] != 0) {
            LOCK();
            piVar12 = (int *)(puVar21[6] + 8);
            *piVar12 = *piVar12 + 1;
            UNLOCK();
          }
          FUN_180015c40(&uStack_128,uVar7);
          FUN_18001dfd0(puVar8[5],&uStack_128);
        }
        FUN_18001ee20(*plStack_220 + 0x118,puVar8 + 5);
        FUN_18001ee90(apuStack_180);
        __3_YAXPEAX_Z(apuStack_180[0]);
        plVar17 = plStack_248;
        if (plStack_248 != (longlong *)0x0) {
          LOCK();
          plVar22 = plStack_248 + 1;
          lVar14 = *plVar22;
          *(int *)plVar22 = (int)*plVar22 + -1;
          UNLOCK();
          if ((int)lVar14 == 1) {
            (**(code **)*plStack_248)(plStack_248);
            LOCK();
            piVar12 = (int *)((longlong)plVar17 + 0xc);
            iVar1 = *piVar12;
            *piVar12 = *piVar12 + -1;
            UNLOCK();
            if (iVar1 == 1) {
              (**(code **)(*plStack_248 + 8))();
            }
          }
        }
        if (*(char *)((longlong)puVar8 + 0x19) == '\0') {
          puVar18 = (undefined8 *)puVar8[2];
          if (*(char *)((longlong)puVar18 + 0x19) == '\0') {
            cVar4 = *(char *)((longlong)*puVar18 + 0x19);
            puVar8 = puVar18;
            puVar18 = (undefined8 *)*puVar18;
            while (cVar4 == '\0') {
              cVar4 = *(char *)((longlong)*puVar18 + 0x19);
              puVar8 = puVar18;
              puVar18 = (undefined8 *)*puVar18;
            }
          }
          else {
            cVar4 = *(char *)((longlong)puVar8[1] + 0x19);
            puVar16 = (undefined8 *)puVar8[1];
            puVar18 = puVar8;
            while ((puVar8 = puVar16, cVar4 == '\0' && (puVar18 == (undefined8 *)puVar8[2]))) {
              cVar4 = *(char *)((longlong)puVar8[1] + 0x19);
              puVar16 = (undefined8 *)puVar8[1];
              puVar18 = puVar8;
            }
          }
        }
      }
      if (iVar5 == 0) {
        func_0x00018001ee00(&puStack_2d8,auStack_230);
        func_0x00018001ee10(&puStack_2d8,auStack_1e0);
        uVar7 = 0;
        while (cVar4 = func_0x00018001eff0(auStack_230,auStack_1e0), cVar4 != '\0') {
          piVar12 = (int *)func_0x0001800eb4a0(auStack_230);
          uVar13 = FUN_18008e5d0();
          FUN_1801111d0(uVar13,auStack_210,*piVar12);
          cVar4 = func_0x000180070730(auStack_210);
          if (cVar4 != '\0') {
            cVar4 = FUN_18001dbc0(4);
            if (cVar4 != '\0') {
              uVar7 = FUN_18001db50();
              iStack_310 = *piVar12;
              pplStack_318 = (longlong **)CONCAT44(pplStack_318._4_4_,0xee);
              FUN_180015b40(uVar7,4,&UNK_18022dd48,&UNK_18022dbd8);
            }
            FUN_18001d910(auStack_210);
            goto LAB_1800ea362;
          }
          FUN_18001ed50(auStack_168);
          lVar14 = func_0x00018001efe0(auStack_210);
          lVar14 = func_0x00018001efe0(lVar14 + 0x158);
          func_0x00018001ee00(lVar14 + 0x28,auStack_240);
          func_0x00018001ee10(lVar14 + 0x28,auStack_1b0);
          while (cVar4 = func_0x00018001eff0(auStack_240,auStack_1b0), cVar4 != '\0') {
            uVar13 = func_0x000180020df0(auStack_240);
            uVar13 = func_0x00018001efe0(uVar13);
            auStack_238[0] = func_0x00018001e3f0(uVar13);
            uVar13 = FUN_1800853b0(&puStack_2e8,auStack_238);
            FUN_18001ee20(auStack_168,uVar13);
            func_0x00018001f1a0(auStack_240);
          }
          lVar14 = __2_YAPEAX_K_Z(0x38);
          uVar13 = uVar7;
          lStack_218 = lVar14;
          if (lVar14 != 0) {
            uVar13 = func_0x00018001efe0(auStack_210);
            uVar23 = func_0x00018001e3e0(uVar13);
            uVar13 = FUN_180020ae0(lVar14,auStack_168,uVar23);
          }
          FUN_1800eb4b0(auStack_108,uVar13);
          uVar13 = FUN_18006e3e0(auStack_b8,auStack_108);
          uVar15 = func_0x00018001efe0(piVar12 + 2);
          FUN_1800a2160(uVar15,uVar13);
          FUN_18001d910(auStack_108);
          FUN_18001ede0(auStack_168);
          FUN_18001d910(auStack_210);
          func_0x0001800790e0(auStack_230);
        }
        FUN_18001ed50(auStack_1d0);
        lVar14 = func_0x00018001efe0(&lStack_270);
        lVar14 = func_0x00018001efe0(lVar14 + 0x180);
        func_0x00018001ee00(lVar14 + 0x28,auStack_260);
        func_0x00018001ee10(lVar14 + 0x28,&lStack_228);
        while (cVar4 = func_0x00018001eff0(auStack_260,&lStack_228), cVar4 != '\0') {
          uVar13 = func_0x000180020df0(auStack_260);
          uVar13 = func_0x00018001efe0(uVar13);
          auStack_2b8[0] = func_0x00018001e3f0(uVar13);
          uVar13 = FUN_1800853b0(&puStack_2e8,auStack_2b8);
          FUN_18001ee20(auStack_1d0,uVar13);
          func_0x00018001f1a0(auStack_260);
        }
        lStack_218 = __2_YAPEAX_K_Z(0x38);
        if (lStack_218 != 0) {
          uVar7 = FUN_180020ae0(lStack_218,auStack_1d0,aiStack_2f8[0]);
        }
        FUN_1800eb4b0(auStack_158,uVar7);
        uVar7 = FUN_18006e3e0(auStack_d8,auStack_158);
        plVar17 = plStack_220;
        uVar13 = func_0x00018001efe0(plStack_220);
        FUN_1800dcba0(uVar13,uVar7);
        lVar14 = lStack_190;
        uVar7 = FUN_1800eb270(lStack_190 + 0x28,aiStack_2f8);
        iVar5 = func_0x00018006dda0(uVar7);
        uVar7 = func_0x0001800ea7e0(extraout_XMM0_Da_07,aiStack_2f8[0],iVar5 + 1);
        lVar6 = func_0x00018001efe0(plVar17);
        *(undefined8 *)(lVar6 + 0x198) = uVar7;
        uVar7 = FUN_1800eb270(lVar14 + 0x28,aiStack_2f8);
        FUN_18001ee20(uVar7,plVar17);
        lVar6 = func_0x00018001efe0(plVar17);
        uVar7 = FUN_1800b48c0(lVar14 + 0x68,lVar6 + 0x198);
        FUN_18007dc50(uVar7,plVar17);
        uVar7 = FUN_18008e610();
        FUN_18015ad70(uVar7);
        uVar7 = FUN_18006e3e0(auStack_c8,plVar17);
        FUN_1800eac90(lVar14,uVar7);
        FUN_18001d910(auStack_158);
        FUN_18001ede0(auStack_1d0);
      }
LAB_1800ea362:
      FUN_18001d910(&uStack_2c8);
      FUN_18001d910(&uStack_2b0);
      FUN_180040100(&puStack_2d8);
      FUN_180040100(&puStack_2e8);
      FUN_18001ede0(&plStack_2a0);
      FUN_18001ede0(&plStack_280);
      FUN_18001ede0(&plStack_290);
    }
    else {
      cVar4 = FUN_18001dbc0(4);
      if (cVar4 != '\0') {
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
        iStack_310 = aiStack_2f8[0];
        pplStack_318 = (longlong **)CONCAT44(pplStack_318._4_4_,0xa2);
        FUN_180015b40(&DAT_1802b1230,4,&UNK_18022dc80,&UNK_18022dbd8);
      }
      plVar17 = (longlong *)*plStack_2a0;
      *plStack_2a0 = (longlong)plStack_2a0;
      plStack_2a0[1] = (longlong)plStack_2a0;
      plVar22 = plStack_2a0;
      if (plVar17 != plStack_2a0) {
        do {
          plVar9 = (longlong *)*plVar17;
          plVar3 = (longlong *)plVar17[3];
          if (plVar3 != (longlong *)0x0) {
            LOCK();
            plVar22 = plVar3 + 1;
            lVar14 = *plVar22;
            *(int *)plVar22 = (int)*plVar22 + -1;
            UNLOCK();
            plVar22 = plStack_2a0;
            if ((int)lVar14 == 1) {
              (**(code **)*plVar3)(plVar3);
              LOCK();
              piVar12 = (int *)((longlong)plVar3 + 0xc);
              iVar5 = *piVar12;
              *piVar12 = *piVar12 + -1;
              UNLOCK();
              plVar22 = plStack_2a0;
              if (iVar5 == 1) {
                (**(code **)(*plVar3 + 8))(plVar3);
                plVar22 = plStack_2a0;
              }
            }
          }
          __3_YAXPEAX_Z(plVar17);
          plVar17 = plVar9;
        } while (plVar9 != plVar22);
      }
      __3_YAXPEAX_Z(plVar22);
      plVar17 = (longlong *)*plStack_280;
      *plStack_280 = (longlong)plStack_280;
      plStack_280[1] = (longlong)plStack_280;
      plVar22 = plStack_280;
      if (plVar17 != plStack_280) {
        do {
          plVar9 = (longlong *)*plVar17;
          plVar3 = (longlong *)plVar17[3];
          if (plVar3 != (longlong *)0x0) {
            LOCK();
            plVar22 = plVar3 + 1;
            lVar14 = *plVar22;
            *(int *)plVar22 = (int)*plVar22 + -1;
            UNLOCK();
            plVar22 = plStack_280;
            if ((int)lVar14 == 1) {
              (**(code **)*plVar3)(plVar3);
              LOCK();
              piVar12 = (int *)((longlong)plVar3 + 0xc);
              iVar5 = *piVar12;
              *piVar12 = *piVar12 + -1;
              UNLOCK();
              plVar22 = plStack_280;
              if (iVar5 == 1) {
                (**(code **)(*plVar3 + 8))(plVar3);
                plVar22 = plStack_280;
              }
            }
          }
          __3_YAXPEAX_Z(plVar17);
          plVar17 = plVar9;
        } while (plVar9 != plVar22);
      }
      __3_YAXPEAX_Z(plVar22);
      plVar17 = (longlong *)*plStack_290;
      *plStack_290 = (longlong)plStack_290;
      plStack_290[1] = (longlong)plStack_290;
      plVar22 = plStack_290;
      if (plVar17 != plStack_290) {
        do {
          plVar9 = (longlong *)*plVar17;
          plVar3 = (longlong *)plVar17[3];
          if (plVar3 != (longlong *)0x0) {
            LOCK();
            plVar22 = plVar3 + 1;
            lVar14 = *plVar22;
            *(int *)plVar22 = (int)*plVar22 + -1;
            UNLOCK();
            plVar22 = plStack_290;
            if ((int)lVar14 == 1) {
              (**(code **)*plVar3)(plVar3);
              LOCK();
              piVar12 = (int *)((longlong)plVar3 + 0xc);
              iVar5 = *piVar12;
              *piVar12 = *piVar12 + -1;
              UNLOCK();
              plVar22 = plStack_290;
              if (iVar5 == 1) {
                (**(code **)(*plVar3 + 8))(plVar3);
                plVar22 = plStack_290;
              }
            }
          }
          __3_YAXPEAX_Z(plVar17);
          plVar17 = plVar9;
        } while (plVar9 != plVar22);
      }
      __3_YAXPEAX_Z(plVar22);
    }
LAB_1800ea3bb:
    FUN_18001d910(&lStack_270);
  }
  FUN_1801e8810(uStack_48 ^ (ulonglong)auStack_338);
  return;
LAB_1800e9685:
  puVar8 = *(undefined8 **)(param_1 + 8);
  cVar4 = *(char *)((longlong)puVar8[1] + 0x19);
  puVar19 = puVar8;
  puVar18 = (undefined8 *)puVar8[1];
  while (cVar4 == '\0') {
    if (*(int *)(puVar18 + 4) < iStack_2ec) {
      puVar16 = (undefined8 *)puVar18[2];
      puVar18 = puVar19;
    }
    else {
      puVar16 = (undefined8 *)*puVar18;
    }
    puVar19 = puVar18;
    puVar18 = puVar16;
    cVar4 = *(char *)((longlong)puVar16 + 0x19);
  }
  if ((puVar19 == puVar8) || (uVar23 = extraout_XMM0_Da_01, iStack_2ec < *(int *)(puVar19 + 4))) {
    piStack_1a0 = &iStack_2ec;
    pplStack_318 = (longlong **)FUN_1800eb690(param_1 + 8,iStack_2ec,&piStack_1a0);
    uVar23 = FUN_1800eb710(param_1 + 8,&puStack_198,puVar19,pplStack_318 + 4);
    puVar19 = puStack_198;
  }
  lVar14 = puVar19[5];
  lVar10 = FUN_18001f260(uVar23,lVar14,*(undefined8 *)(lVar14 + 8),&uStack_2b0);
  lVar6 = puVar19[6];
  uVar23 = extraout_XMM0_Da_02;
  if (lVar6 == 0x7fffffffffffffe) {
    lVar6 = __Xlength_error_std__YAXPEBD_Z(&UNK_18021d318);
    uVar23 = extraout_XMM0_Da_03;
  }
  puVar19[6] = lVar6 + 1;
  *(longlong *)(lVar14 + 8) = lVar10;
  **(longlong **)(lVar10 + 8) = lVar10;
  cVar4 = *(char *)((longlong)puStack_2e8[1] + 0x19);
  puVar8 = (undefined8 *)puStack_2e8[1];
  puVar19 = puStack_2e8;
  while (puVar18 = puVar8, cVar4 == '\0') {
    if (*(int *)(puVar18 + 4) < iStack_2ec) {
      puVar8 = (undefined8 *)puVar18[2];
      puVar18 = puVar19;
    }
    else {
      puVar8 = (undefined8 *)*puVar18;
    }
    cVar4 = *(char *)((longlong)puVar8 + 0x19);
    puVar19 = puVar18;
  }
  if ((puVar19 == puStack_2e8) || (iStack_2ec < *(int *)(puVar19 + 4))) {
    piStack_1b8 = &iStack_2ec;
    pplStack_318 = (longlong **)FUN_180085630(&puStack_2e8,puStack_2e8,&piStack_1b8);
    uVar23 = FUN_180085670(&puStack_2e8,&puStack_170,puVar19,pplStack_318 + 4);
    puVar19 = puStack_170;
  }
  if (plStack_2a8 != (longlong *)0x0) {
    LOCK();
    *(int *)(plStack_2a8 + 1) = (int)plStack_2a8[1] + 1;
    UNLOCK();
  }
  plVar9 = (longlong *)puVar19[6];
  puVar19[6] = plStack_2a8;
  puVar19[5] = uStack_2b0;
  plStack_130 = plVar9;
  if (plVar9 != (longlong *)0x0) {
    LOCK();
    plVar3 = plVar9 + 1;
    lVar14 = *plVar3;
    *(int *)plVar3 = (int)*plVar3 + -1;
    UNLOCK();
    if ((int)lVar14 == 1) {
      uVar23 = (**(code **)*plVar9)(plVar9);
      LOCK();
      piVar12 = (int *)((longlong)plVar9 + 0xc);
      iVar1 = *piVar12;
      *piVar12 = *piVar12 + -1;
      UNLOCK();
      if (iVar1 == 1) {
        uVar23 = (**(code **)(*plStack_130 + 8))();
      }
    }
  }
  plVar17 = (longlong *)*plVar17;
  goto LAB_1800e92c0;
LAB_1800e99cd:
  if ((_DAT_1802b2980 & 1) == 0) {
    _DAT_1802b2980 = _DAT_1802b2980 | 1;
    FUN_18015a9d0();
    FUN_1801e8930(FUN_180202990);
  }
  FUN_18015afe0(&DAT_1802b2990);
  plVar9 = (longlong *)FUN_1800eb270(param_1 + 0x18,&iStack_2f0);
  lVar14 = *plVar9;
  lVar10 = FUN_18001f260(extraout_XMM0_Da_04,lVar14,*(undefined8 *)(lVar14 + 8),&uStack_2c8);
  lVar6 = plVar9[1];
  uVar23 = extraout_XMM0_Da_05;
  if (lVar6 == 0x7fffffffffffffe) {
    lVar6 = __Xlength_error_std__YAXPEBD_Z(&UNK_18021d318);
    uVar23 = extraout_XMM0_Da_06;
  }
  plVar9[1] = lVar6 + 1;
  *(longlong *)(lVar14 + 8) = lVar10;
  **(longlong **)(lVar10 + 8) = lVar10;
  cVar4 = *(char *)((longlong)puStack_2e8[1] + 0x19);
  puVar8 = (undefined8 *)puStack_2e8[1];
  puVar19 = puStack_2e8;
  while (puVar18 = puVar8, cVar4 == '\0') {
    if (*(int *)(puVar18 + 4) < iStack_2f0) {
      puVar8 = (undefined8 *)puVar18[2];
      puVar18 = puVar19;
    }
    else {
      puVar8 = (undefined8 *)*puVar18;
    }
    cVar4 = *(char *)((longlong)puVar8 + 0x19);
    puVar19 = puVar18;
  }
  if ((puVar19 == puStack_2e8) || (iStack_2f0 < *(int *)(puVar19 + 4))) {
    piStack_1a8 = &iStack_2f0;
    pplStack_318 = (longlong **)FUN_180085630(&puStack_2e8,puStack_2e8,&piStack_1a8);
    uVar23 = FUN_180085670(&puStack_2e8,&puStack_1f0,puVar19,pplStack_318 + 4);
    puVar19 = puStack_1f0;
  }
  if (plStack_2c0 != (longlong *)0x0) {
    LOCK();
    *(int *)(plStack_2c0 + 1) = (int)plStack_2c0[1] + 1;
    UNLOCK();
  }
  plVar9 = (longlong *)puVar19[6];
  puVar19[6] = plStack_2c0;
  puVar19[5] = uStack_2c8;
  plStack_110 = plVar9;
  if (plVar9 != (longlong *)0x0) {
    LOCK();
    plVar3 = plVar9 + 1;
    lVar14 = *plVar3;
    *(int *)plVar3 = (int)*plVar3 + -1;
    UNLOCK();
    if ((int)lVar14 == 1) {
      uVar23 = (**(code **)*plVar9)(plVar9);
      LOCK();
      piVar12 = (int *)((longlong)plVar9 + 0xc);
      iVar1 = *piVar12;
      *piVar12 = *piVar12 + -1;
      UNLOCK();
      if (iVar1 == 1) {
        uVar23 = (**(code **)(*plStack_110 + 8))();
      }
    }
  }
  plVar9 = plStack_2c0;
  cVar4 = *(char *)((longlong)puStack_2d8[1] + 0x19);
  puVar8 = (undefined8 *)puStack_2d8[1];
  puVar19 = puStack_2d8;
  while (puVar18 = puVar8, cVar4 == '\0') {
    if (*(int *)(puVar18 + 4) < iStack_2f0) {
      puVar8 = (undefined8 *)puVar18[2];
      puVar18 = puVar19;
    }
    else {
      puVar8 = (undefined8 *)*puVar18;
    }
    cVar4 = *(char *)((longlong)puVar8 + 0x19);
    puVar19 = puVar18;
  }
  if ((puVar19 == puStack_2d8) || (iStack_2f0 < *(int *)(puVar19 + 4))) {
    piStack_1d8 = &iStack_2f0;
    pplStack_318 = (longlong **)FUN_180085630(&puStack_2d8,puStack_2d8,&piStack_1d8);
    uVar23 = FUN_180085670(&puStack_2d8,&puStack_1c0,puVar19,pplStack_318 + 4);
    puVar19 = puStack_1c0;
  }
  if (plVar9 != (longlong *)0x0) {
    LOCK();
    *(int *)(plVar9 + 1) = (int)plVar9[1] + 1;
    UNLOCK();
    plVar9 = plStack_2c0;
  }
  plVar3 = (longlong *)puVar19[6];
  puVar19[6] = plVar9;
  puVar19[5] = uStack_2c8;
  plStack_140 = plVar3;
  if (plVar3 != (longlong *)0x0) {
    LOCK();
    plVar9 = plVar3 + 1;
    lVar14 = *plVar9;
    *(int *)plVar9 = (int)*plVar9 + -1;
    UNLOCK();
    if ((int)lVar14 == 1) {
      uVar23 = (**(code **)*plVar3)(plVar3);
      LOCK();
      piVar12 = (int *)((longlong)plVar3 + 0xc);
      iVar1 = *piVar12;
      *piVar12 = *piVar12 + -1;
      UNLOCK();
      if (iVar1 == 1) {
        uVar23 = (**(code **)(*plStack_140 + 8))();
      }
    }
  }
  plVar17 = (longlong *)*plVar17;
  goto LAB_1800e9865;
}

