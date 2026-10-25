
/* WARNING: Removing unreachable block (ram,0x00018010b767) */
/* WARNING: Removing unreachable block (ram,0x00018010b770) */
/* WARNING: Removing unreachable block (ram,0x00018010b779) */
/* WARNING: Removing unreachable block (ram,0x00018010b79e) */
/* WARNING: Removing unreachable block (ram,0x00018010b7a8) */
/* WARNING: Removing unreachable block (ram,0x00018010b7ae) */
/* WARNING: Removing unreachable block (ram,0x00018010b7bb) */
/* WARNING: Removing unreachable block (ram,0x00018010b783) */
/* WARNING: Removing unreachable block (ram,0x00018010b790) */
/* WARNING: Removing unreachable block (ram,0x00018010b79c) */
/* WARNING: Removing unreachable block (ram,0x00018010b7be) */
/* WARNING: Removing unreachable block (ram,0x00018010b5a6) */
/* WARNING: Removing unreachable block (ram,0x00018010b5ab) */
/* WARNING: Removing unreachable block (ram,0x00018010b5b0) */
/* WARNING: Removing unreachable block (ram,0x00018010b5b9) */
/* WARNING: Removing unreachable block (ram,0x00018010b5de) */
/* WARNING: Removing unreachable block (ram,0x00018010b5e8) */
/* WARNING: Removing unreachable block (ram,0x00018010b5ee) */
/* WARNING: Removing unreachable block (ram,0x00018010b5fb) */
/* WARNING: Removing unreachable block (ram,0x00018010b5c3) */
/* WARNING: Removing unreachable block (ram,0x00018010b5d0) */
/* WARNING: Removing unreachable block (ram,0x00018010b5dc) */
/* WARNING: Removing unreachable block (ram,0x00018010b5fe) */
/* WARNING: Removing unreachable block (ram,0x00018010b684) */
/* WARNING: Removing unreachable block (ram,0x00018010b690) */
/* WARNING: Removing unreachable block (ram,0x00018010b699) */
/* WARNING: Removing unreachable block (ram,0x00018010b6be) */
/* WARNING: Removing unreachable block (ram,0x00018010b6c8) */
/* WARNING: Removing unreachable block (ram,0x00018010b6ce) */
/* WARNING: Removing unreachable block (ram,0x00018010b6db) */
/* WARNING: Removing unreachable block (ram,0x00018010b6a3) */
/* WARNING: Removing unreachable block (ram,0x00018010b6b0) */
/* WARNING: Removing unreachable block (ram,0x00018010b6bc) */
/* WARNING: Removing unreachable block (ram,0x00018010b6de) */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_18010ae80(longlong param_1,int param_2)

{
  longlong *plVar1;
  int *piVar2;
  longlong lVar3;
  undefined4 uVar4;
  longlong *plVar5;
  longlong *plVar6;
  undefined8 *puVar7;
  longlong *plVar8;
  char cVar9;
  int iVar10;
  undefined8 uVar11;
  longlong *plVar12;
  longlong lVar13;
  undefined8 uVar14;
  undefined8 *puVar15;
  undefined8 **ppuVar16;
  undefined4 *puVar17;
  undefined8 *puVar18;
  undefined8 *puVar19;
  undefined8 uVar20;
  undefined8 *puVar21;
  longlong *plVar22;
  bool bVar23;
  undefined1 auStack_358 [32];
  longlong **pplStack_338;
  int iStack_330;
  undefined8 uStack_328;
  int aiStack_318 [2];
  longlong lStack_310;
  longlong lStack_308;
  longlong *plStack_300;
  undefined8 uStack_2f8;
  longlong *plStack_2f0;
  undefined8 uStack_2e8;
  undefined8 *puStack_2e0;
  undefined8 uStack_2d8;
  undefined8 uStack_2d0;
  undefined8 uStack_2c8;
  longlong *plStack_2c0;
  undefined8 *puStack_2b8;
  undefined8 uStack_2b0;
  undefined1 auStack_2a8 [8];
  longlong *plStack_2a0;
  undefined8 uStack_298;
  longlong *aplStack_290 [2];
  undefined1 auStack_280 [8];
  longlong *plStack_278;
  longlong *aplStack_270 [2];
  longlong *aplStack_260 [2];
  undefined8 uStack_250;
  undefined8 uStack_248;
  longlong lStack_240;
  longlong *plStack_238;
  longlong lStack_230;
  longlong *plStack_228;
  longlong lStack_220;
  longlong *plStack_218;
  longlong lStack_210;
  undefined1 auStack_208 [16];
  longlong *aplStack_1f8 [2];
  undefined1 auStack_1e8 [16];
  undefined1 auStack_1d8 [8];
  undefined8 *puStack_1d0;
  undefined8 *puStack_1c8;
  undefined8 *puStack_1c0;
  undefined8 *puStack_1b8;
  undefined1 auStack_1b0 [8];
  undefined8 *puStack_1a8;
  undefined1 auStack_1a0 [16];
  undefined8 *puStack_190;
  undefined8 *puStack_188;
  undefined8 *puStack_180;
  longlong lStack_178;
  int *piStack_170;
  undefined8 *puStack_168;
  undefined8 *puStack_160;
  undefined8 *puStack_158;
  undefined1 auStack_150 [40];
  undefined1 auStack_128 [16];
  undefined1 auStack_118 [8];
  longlong lStack_110;
  longlong *plStack_108;
  undefined1 auStack_100 [8];
  longlong lStack_f8;
  longlong *plStack_f0;
  undefined1 auStack_e8 [8];
  longlong lStack_e0;
  longlong *plStack_d8;
  undefined1 auStack_d0 [8];
  undefined1 auStack_c8 [8];
  undefined1 auStack_c0 [16];
  undefined8 uStack_b0;
  undefined1 auStack_98 [16];
  undefined1 auStack_88 [16];
  undefined1 auStack_78 [16];
  undefined1 auStack_68 [16];
  undefined1 auStack_58 [32];
  ulonglong uStack_38;
  
  uStack_b0 = 0xfffffffffffffffe;
  uStack_38 = _DAT_1802a5230 ^ (ulonglong)auStack_358;
  lVar3 = param_1 + 0x120;
  aiStack_318[0] = param_2;
  lStack_210 = lVar3;
  iVar10 = _Mtx_lock(lVar3);
  if (iVar10 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar10);
  }
  puVar18 = *(undefined8 **)(param_1 + 0x10);
  cVar9 = *(char *)((longlong)puVar18[1] + 0x19);
  puVar19 = puVar18;
  puVar15 = (undefined8 *)puVar18[1];
  while (cVar9 == '\0') {
    if (*(int *)(puVar15 + 4) < aiStack_318[0]) {
      puVar21 = (undefined8 *)puVar15[2];
      puVar15 = puVar19;
    }
    else {
      puVar21 = (undefined8 *)*puVar15;
    }
    puVar19 = puVar15;
    puVar15 = puVar21;
    cVar9 = *(char *)((longlong)puVar21 + 0x19);
  }
  if ((puVar19 == puVar18) || (aiStack_318[0] < *(int *)(puVar19 + 4))) {
    puStack_168 = puVar18;
    ppuVar16 = &puStack_168;
  }
  else {
    puStack_188 = puVar19;
    ppuVar16 = &puStack_188;
  }
  if (*ppuVar16 == puVar18) {
    cVar9 = FUN_18001dbc0(4);
    if (cVar9 != '\0') {
      uVar11 = FUN_18001db50();
      iStack_330 = aiStack_318[0];
      pplStack_338 = (longlong **)CONCAT44(pplStack_338._4_4_,0x215);
      FUN_180015b40(uVar11,4,&UNK_180230c70,&UNK_180230be8);
    }
    iVar10 = _Mtx_unlock(lVar3);
    if (iVar10 != 0) {
      __Throw_C_error_std__YAXH_Z(iVar10);
    }
  }
  else {
    FUN_18010f640(param_1,aiStack_318[0]);
    FUN_18010d540(param_1,aiStack_318[0]);
    FUN_18010dcc0(param_1,aiStack_318[0]);
    FUN_18010e500(param_1,aiStack_318[0]);
    FUN_18010e9a0(param_1,aplStack_260,aiStack_318[0]);
    FUN_18010eb70(param_1,aplStack_270,aiStack_318[0]);
    uVar11 = 0;
    lStack_308 = 0;
    lStack_310 = FUN_18001f150();
    plVar22 = (longlong *)*aplStack_270[0];
    bVar23 = plVar22 == aplStack_270[0];
    while (!bVar23) {
      puVar18 = *(undefined8 **)(param_1 + 0xc0);
      puVar19 = puVar18;
      if (*(char *)((longlong)puVar18[1] + 0x19) == '\0') {
        puVar15 = (undefined8 *)puVar18[1];
        do {
          if (*(int *)(puVar15 + 4) < *(int *)((longlong)plVar22 + 0x1c)) {
            puVar21 = (undefined8 *)puVar15[2];
          }
          else {
            puVar21 = (undefined8 *)*puVar15;
            puVar19 = puVar15;
          }
          puVar15 = puVar21;
        } while (*(char *)((longlong)puVar21 + 0x19) == '\0');
      }
      if ((puVar19 == puVar18) || (*(int *)((longlong)plVar22 + 0x1c) < *(int *)(puVar19 + 4))) {
        puStack_1c8 = puVar18;
        ppuVar16 = &puStack_1c8;
      }
      else {
        puStack_158 = puVar19;
        ppuVar16 = &puStack_158;
      }
      if (*ppuVar16 != puVar18) {
        plVar12 = (longlong *)FUN_1800853b0(param_1 + 0xc0,(longlong)plVar22 + 0x1c);
        lVar3 = lStack_310;
        plVar5 = (longlong *)plVar12[1];
        lStack_110 = *plVar12;
        if (plVar5 != (longlong *)0x0) {
          LOCK();
          *(int *)(plVar5 + 1) = (int)plVar5[1] + 1;
          UNLOCK();
        }
        plStack_108 = plVar5;
        if (lStack_110 != 0) {
          if (plVar5 != (longlong *)0x0) {
            LOCK();
            *(int *)(plVar5 + 1) = (int)plVar5[1] + 1;
            UNLOCK();
          }
          lStack_240 = lStack_110;
          plStack_238 = plVar5;
          lVar13 = FUN_180072dc0(lStack_110,lStack_310,*(undefined8 *)(lStack_310 + 8),&lStack_240);
          plVar12 = plStack_238;
          if (lStack_308 == 0x7fffffffffffffe) {
            __Xlength_error_std__YAXPEBD_Z(&UNK_18021d318);
            break;
          }
          lStack_308 = lStack_308 + 1;
          *(longlong *)(lVar3 + 8) = lVar13;
          **(longlong **)(lVar13 + 8) = lVar13;
          if (plStack_238 != (longlong *)0x0) {
            LOCK();
            plVar1 = plStack_238 + 1;
            *(int *)plVar1 = (int)*plVar1 + -1;
            UNLOCK();
            if ((int)*plVar1 == 0) {
              (**(code **)*plStack_238)(plStack_238);
              LOCK();
              piVar2 = (int *)((longlong)plVar12 + 0xc);
              *piVar2 = *piVar2 + -1;
              UNLOCK();
              if (*piVar2 == 0) {
                (**(code **)(*plStack_238 + 8))();
              }
            }
          }
        }
        if (plVar5 != (longlong *)0x0) {
          LOCK();
          plVar12 = plVar5 + 1;
          *(int *)plVar12 = (int)*plVar12 + -1;
          UNLOCK();
          if ((int)*plVar12 == 0) {
            (**(code **)*plVar5)(plVar5);
            LOCK();
            piVar2 = (int *)((longlong)plVar5 + 0xc);
            *piVar2 = *piVar2 + -1;
            UNLOCK();
            if (*piVar2 == 0) {
              (**(code **)(*plVar5 + 8))(plVar5);
            }
          }
        }
      }
      if (*(char *)((longlong)plVar22 + 0x19) == '\0') {
        plVar5 = (longlong *)plVar22[2];
        if (*(char *)((longlong)plVar5 + 0x19) == '\0') {
          cVar9 = *(char *)(*plVar5 + 0x19);
          plVar22 = plVar5;
          plVar5 = (longlong *)*plVar5;
          while (cVar9 == '\0') {
            cVar9 = *(char *)(*plVar5 + 0x19);
            plVar22 = plVar5;
            plVar5 = (longlong *)*plVar5;
          }
        }
        else {
          cVar9 = *(char *)(plVar22[1] + 0x19);
          plVar12 = (longlong *)plVar22[1];
          plVar5 = plVar22;
          while ((plVar22 = plVar12, cVar9 == '\0' && (plVar5 == (longlong *)plVar22[2]))) {
            cVar9 = *(char *)(plVar22[1] + 0x19);
            plVar12 = (longlong *)plVar22[1];
            plVar5 = plVar22;
          }
        }
      }
      bVar23 = plVar22 == aplStack_270[0];
    }
    puStack_2b8 = (undefined8 *)0x0;
    uStack_2b0 = 0;
    puStack_2b8 = (undefined8 *)FUN_180079000(&puStack_2b8);
    plStack_2a0 = (longlong *)0x0;
    uStack_298 = 0;
    plStack_2a0 = (longlong *)FUN_180079000(&plStack_2a0);
    uStack_2d0 = 0;
    uStack_2c8 = 0;
    uStack_2d0 = FUN_180079000(&uStack_2d0);
    puVar18 = *(undefined8 **)(param_1 + 0x30);
    cVar9 = *(char *)((longlong)puVar18[1] + 0x19);
    puVar19 = puVar18;
    puVar15 = (undefined8 *)puVar18[1];
    while (cVar9 == '\0') {
      if (*(int *)(puVar15 + 4) < aiStack_318[0]) {
        puVar21 = (undefined8 *)puVar15[2];
        puVar15 = puVar19;
      }
      else {
        puVar21 = (undefined8 *)*puVar15;
      }
      puVar19 = puVar15;
      puVar15 = puVar21;
      cVar9 = *(char *)((longlong)puVar21 + 0x19);
    }
    if ((puVar19 == puVar18) || (aiStack_318[0] < *(int *)(puVar19 + 4))) {
      piStack_170 = aiStack_318;
      pplStack_338 = (longlong **)FUN_18009f5c0(param_1 + 0x30,aiStack_318[0],&piStack_170);
      FUN_18009f640(param_1 + 0x30,&puStack_1b8,puVar19,pplStack_338 + 4);
      puVar19 = puStack_1b8;
    }
    plVar22 = *(longlong **)puVar19[5];
    if (plVar22 != (longlong *)puVar19[5]) {
      do {
        puStack_2e0 = (undefined8 *)0x0;
        uStack_2d8 = 0;
        puStack_2e0 = (undefined8 *)FUN_180079000(&puStack_2e0);
        plStack_2f0 = (longlong *)0x0;
        uStack_2e8 = 0;
        plStack_2f0 = (longlong *)FUN_180079000(&plStack_2f0);
        plStack_300 = (longlong *)0x0;
        uStack_2f8 = 0;
        plStack_300 = (longlong *)FUN_180079000(&plStack_300);
        pplStack_338 = &plStack_300;
        iVar10 = FUN_180111cc0(param_1,*(undefined4 *)((longlong)plVar22 + 0x1c),&puStack_2b8,
                               &plStack_2f0);
        puVar18 = puStack_2e0;
        if (iVar10 == 0) {
          puVar15 = (undefined8 *)*puStack_2e0;
          plVar5 = plStack_2f0;
          while (plStack_2f0 = plVar5, puVar15 != puVar18) {
            pplStack_338 = (longlong **)FUN_1800735e0(&puStack_2b8,(longlong)puVar15 + 0x1c);
            FUN_180073610(&puStack_2b8,auStack_98,0,(longlong)pplStack_338 + 0x1c);
            plVar5 = plStack_2f0;
            if (*(char *)((longlong)puVar15 + 0x19) == '\0') {
              puVar21 = (undefined8 *)puVar15[2];
              if (*(char *)((longlong)puVar21 + 0x19) == '\0') {
                cVar9 = *(char *)((longlong)*puVar21 + 0x19);
                puVar15 = puVar21;
                puVar21 = (undefined8 *)*puVar21;
                while (cVar9 == '\0') {
                  cVar9 = *(char *)((longlong)*puVar21 + 0x19);
                  puVar15 = puVar21;
                  puVar21 = (undefined8 *)*puVar21;
                }
              }
              else {
                cVar9 = *(char *)((longlong)puVar15[1] + 0x19);
                puVar7 = (undefined8 *)puVar15[1];
                puVar21 = puVar15;
                while ((puVar15 = puVar7, cVar9 == '\0' && (puVar21 == (undefined8 *)puVar15[2]))) {
                  cVar9 = *(char *)((longlong)puVar15[1] + 0x19);
                  puVar7 = (undefined8 *)puVar15[1];
                  puVar21 = puVar15;
                }
              }
            }
          }
          plVar12 = (longlong *)*plVar5;
          plVar1 = plStack_300;
          while (plStack_300 = plVar1, plVar12 != plVar5) {
            pplStack_338 = (longlong **)FUN_1800735e0(&plStack_2a0,(longlong)plVar12 + 0x1c);
            FUN_180073610(&plStack_2a0,auStack_78,0,(longlong)pplStack_338 + 0x1c);
            plVar1 = plStack_300;
            if (*(char *)((longlong)plVar12 + 0x19) == '\0') {
              plVar6 = (longlong *)plVar12[2];
              if (*(char *)((longlong)plVar6 + 0x19) == '\0') {
                cVar9 = *(char *)(*plVar6 + 0x19);
                plVar12 = plVar6;
                plVar6 = (longlong *)*plVar6;
                while (cVar9 == '\0') {
                  cVar9 = *(char *)(*plVar6 + 0x19);
                  plVar12 = plVar6;
                  plVar6 = (longlong *)*plVar6;
                }
              }
              else {
                cVar9 = *(char *)(plVar12[1] + 0x19);
                plVar8 = (longlong *)plVar12[1];
                plVar6 = plVar12;
                while ((plVar12 = plVar8, cVar9 == '\0' && (plVar6 == (longlong *)plVar12[2]))) {
                  cVar9 = *(char *)(plVar12[1] + 0x19);
                  plVar8 = (longlong *)plVar12[1];
                  plVar6 = plVar12;
                }
              }
            }
          }
          plVar5 = (longlong *)*plVar1;
          while (plVar5 != plVar1) {
            pplStack_338 = (longlong **)FUN_1800735e0(&uStack_2d0,(longlong)plVar5 + 0x1c);
            FUN_180073610(&uStack_2d0,auStack_68,0,(longlong)pplStack_338 + 0x1c);
            if (*(char *)((longlong)plVar5 + 0x19) == '\0') {
              plVar12 = (longlong *)plVar5[2];
              if (*(char *)((longlong)plVar12 + 0x19) == '\0') {
                cVar9 = *(char *)(*plVar12 + 0x19);
                plVar5 = plVar12;
                plVar12 = (longlong *)*plVar12;
                while (cVar9 == '\0') {
                  cVar9 = *(char *)(*plVar12 + 0x19);
                  plVar5 = plVar12;
                  plVar12 = (longlong *)*plVar12;
                }
              }
              else {
                cVar9 = *(char *)(plVar5[1] + 0x19);
                plVar6 = (longlong *)plVar5[1];
                plVar12 = plVar5;
                while ((plVar5 = plVar6, cVar9 == '\0' && (plVar12 == (longlong *)plVar5[2]))) {
                  cVar9 = *(char *)(plVar5[1] + 0x19);
                  plVar6 = (longlong *)plVar5[1];
                  plVar12 = plVar5;
                }
              }
            }
          }
        }
        cVar9 = *(char *)(plStack_300[1] + 0x19);
        plVar5 = (longlong *)plStack_300[1];
        while (cVar9 == '\0') {
          FUN_18003c6d0(&plStack_300,plVar5[2]);
          plVar12 = (longlong *)*plVar5;
          __3_YAXPEAX_Z(plVar5);
          plVar5 = plVar12;
          cVar9 = *(char *)((longlong)plVar12 + 0x19);
        }
        plStack_300[1] = (longlong)plStack_300;
        *plStack_300 = (longlong)plStack_300;
        plStack_300[2] = (longlong)plStack_300;
        uStack_2f8 = 0;
        __3_YAXPEAX_Z(plStack_300);
        cVar9 = *(char *)(plStack_2f0[1] + 0x19);
        plVar5 = (longlong *)plStack_2f0[1];
        while (cVar9 == '\0') {
          FUN_18003c6d0(&plStack_2f0,plVar5[2]);
          plVar12 = (longlong *)*plVar5;
          __3_YAXPEAX_Z(plVar5);
          plVar5 = plVar12;
          cVar9 = *(char *)((longlong)plVar12 + 0x19);
        }
        plStack_2f0[1] = (longlong)plStack_2f0;
        *plStack_2f0 = (longlong)plStack_2f0;
        plStack_2f0[2] = (longlong)plStack_2f0;
        uStack_2e8 = 0;
        __3_YAXPEAX_Z(plStack_2f0);
        cVar9 = *(char *)((longlong)puStack_2e0[1] + 0x19);
        plVar5 = (longlong *)puStack_2e0[1];
        while (cVar9 == '\0') {
          FUN_18003c6d0(&puStack_2e0,plVar5[2]);
          plVar12 = (longlong *)*plVar5;
          __3_YAXPEAX_Z(plVar5);
          plVar5 = plVar12;
          cVar9 = *(char *)((longlong)plVar12 + 0x19);
        }
        puStack_2e0[1] = puStack_2e0;
        *puStack_2e0 = puStack_2e0;
        puStack_2e0[2] = puStack_2e0;
        uStack_2d8 = 0;
        __3_YAXPEAX_Z(puStack_2e0);
        if (*(char *)((longlong)plVar22 + 0x19) == '\0') {
          plVar5 = (longlong *)plVar22[2];
          if (*(char *)((longlong)plVar5 + 0x19) == '\0') {
            cVar9 = *(char *)(*plVar5 + 0x19);
            plVar22 = plVar5;
            plVar5 = (longlong *)*plVar5;
            while (cVar9 == '\0') {
              cVar9 = *(char *)(*plVar5 + 0x19);
              plVar22 = plVar5;
              plVar5 = (longlong *)*plVar5;
            }
          }
          else {
            cVar9 = *(char *)(plVar22[1] + 0x19);
            plVar12 = (longlong *)plVar22[1];
            plVar5 = plVar22;
            while ((plVar22 = plVar12, cVar9 == '\0' && (plVar5 == (longlong *)plVar22[2]))) {
              cVar9 = *(char *)(plVar22[1] + 0x19);
              plVar12 = (longlong *)plVar22[1];
              plVar5 = plVar22;
            }
          }
        }
      } while (plVar22 != (longlong *)puVar19[5]);
    }
    FUN_180071720(&uStack_2d0,*puStack_2b8,puStack_2b8);
    plVar22 = (longlong *)*plStack_2a0;
    if (plVar22 != plStack_2a0) {
      do {
        FUN_18006ddb0(aplStack_260,(longlong)plVar22 + 0x1c);
        if (*(char *)((longlong)plVar22 + 0x19) == '\0') {
          plVar5 = (longlong *)plVar22[2];
          if (*(char *)((longlong)plVar5 + 0x19) == '\0') {
            cVar9 = *(char *)(*plVar5 + 0x19);
            plVar22 = plVar5;
            plVar5 = (longlong *)*plVar5;
            while (cVar9 == '\0') {
              cVar9 = *(char *)(*plVar5 + 0x19);
              plVar22 = plVar5;
              plVar5 = (longlong *)*plVar5;
            }
          }
          else {
            cVar9 = *(char *)(plVar22[1] + 0x19);
            plVar12 = (longlong *)plVar22[1];
            plVar5 = plVar22;
            while ((plVar22 = plVar12, cVar9 == '\0' && (plVar5 == (longlong *)plVar22[2]))) {
              cVar9 = *(char *)(plVar22[1] + 0x19);
              plVar12 = (longlong *)plVar22[1];
              plVar5 = plVar22;
            }
          }
        }
      } while (plVar22 != plStack_2a0);
    }
    plVar22 = (longlong *)*aplStack_260[0];
    bVar23 = plVar22 == aplStack_260[0];
    while (!bVar23) {
      puVar18 = *(undefined8 **)(param_1 + 0xa0);
      puVar19 = puVar18;
      if (*(char *)((longlong)puVar18[1] + 0x19) == '\0') {
        puVar15 = (undefined8 *)puVar18[1];
        do {
          if (*(int *)(puVar15 + 4) < *(int *)((longlong)plVar22 + 0x1c)) {
            puVar21 = (undefined8 *)puVar15[2];
          }
          else {
            puVar21 = (undefined8 *)*puVar15;
            puVar19 = puVar15;
          }
          puVar15 = puVar21;
        } while (*(char *)((longlong)puVar21 + 0x19) == '\0');
      }
      if ((puVar19 == puVar18) || (*(int *)((longlong)plVar22 + 0x1c) < *(int *)(puVar19 + 4))) {
        puStack_1a8 = puVar18;
        ppuVar16 = &puStack_1a8;
      }
      else {
        puStack_180 = puVar19;
        ppuVar16 = &puStack_180;
      }
      if (*ppuVar16 != puVar18) {
        plVar12 = (longlong *)FUN_1800853b0(param_1 + 0xa0,(longlong)plVar22 + 0x1c);
        lVar3 = lStack_310;
        plVar5 = (longlong *)plVar12[1];
        lStack_f8 = *plVar12;
        if (plVar5 != (longlong *)0x0) {
          LOCK();
          *(int *)(plVar5 + 1) = (int)plVar5[1] + 1;
          UNLOCK();
        }
        plStack_f0 = plVar5;
        if (lStack_f8 != 0) {
          if (plVar5 != (longlong *)0x0) {
            LOCK();
            *(int *)(plVar5 + 1) = (int)plVar5[1] + 1;
            UNLOCK();
          }
          lStack_230 = lStack_f8;
          plStack_228 = plVar5;
          lVar13 = FUN_180072dc0(lStack_f8,lStack_310,*(undefined8 *)(lStack_310 + 8),&lStack_230);
          plVar12 = plStack_228;
          if (lStack_308 == 0x7fffffffffffffe) {
            __Xlength_error_std__YAXPEBD_Z(&UNK_18021d318);
            break;
          }
          lStack_308 = lStack_308 + 1;
          *(longlong *)(lVar3 + 8) = lVar13;
          **(longlong **)(lVar13 + 8) = lVar13;
          if (plStack_228 != (longlong *)0x0) {
            LOCK();
            plVar1 = plStack_228 + 1;
            *(int *)plVar1 = (int)*plVar1 + -1;
            UNLOCK();
            if ((int)*plVar1 == 0) {
              (**(code **)*plStack_228)(plStack_228);
              LOCK();
              piVar2 = (int *)((longlong)plVar12 + 0xc);
              *piVar2 = *piVar2 + -1;
              UNLOCK();
              if (*piVar2 == 0) {
                (**(code **)(*plStack_228 + 8))();
              }
            }
          }
        }
        if (plVar5 != (longlong *)0x0) {
          LOCK();
          plVar12 = plVar5 + 1;
          *(int *)plVar12 = (int)*plVar12 + -1;
          UNLOCK();
          if ((int)*plVar12 == 0) {
            (**(code **)*plVar5)(plVar5);
            LOCK();
            piVar2 = (int *)((longlong)plVar5 + 0xc);
            *piVar2 = *piVar2 + -1;
            UNLOCK();
            if (*piVar2 == 0) {
              (**(code **)(*plVar5 + 8))(plVar5);
            }
          }
        }
      }
      if (*(char *)((longlong)plVar22 + 0x19) == '\0') {
        plVar5 = (longlong *)plVar22[2];
        if (*(char *)((longlong)plVar5 + 0x19) == '\0') {
          cVar9 = *(char *)(*plVar5 + 0x19);
          plVar22 = plVar5;
          plVar5 = (longlong *)*plVar5;
          while (cVar9 == '\0') {
            cVar9 = *(char *)(*plVar5 + 0x19);
            plVar22 = plVar5;
            plVar5 = (longlong *)*plVar5;
          }
        }
        else {
          cVar9 = *(char *)(plVar22[1] + 0x19);
          plVar12 = (longlong *)plVar22[1];
          plVar5 = plVar22;
          while ((plVar22 = plVar12, cVar9 == '\0' && (plVar5 == (longlong *)plVar22[2]))) {
            cVar9 = *(char *)(plVar22[1] + 0x19);
            plVar12 = (longlong *)plVar22[1];
            plVar5 = plVar22;
          }
        }
      }
      bVar23 = plVar22 == aplStack_260[0];
    }
    uVar14 = FUN_18009e260(param_1 + 0x10,aiStack_318);
    FUN_18010ed40(param_1,aplStack_1f8,uVar14,aplStack_260);
    plVar22 = (longlong *)*aplStack_1f8[0];
    bVar23 = plVar22 == aplStack_1f8[0];
    while (!bVar23) {
      puVar18 = *(undefined8 **)(param_1 + 0x40);
      puVar19 = puVar18;
      if (*(char *)((longlong)puVar18[1] + 0x19) == '\0') {
        puVar15 = (undefined8 *)puVar18[1];
        do {
          if (*(int *)(puVar15 + 4) < *(int *)((longlong)plVar22 + 0x1c)) {
            puVar21 = (undefined8 *)puVar15[2];
          }
          else {
            puVar21 = (undefined8 *)*puVar15;
            puVar19 = puVar15;
          }
          puVar15 = puVar21;
        } while (*(char *)((longlong)puVar21 + 0x19) == '\0');
      }
      if ((puVar19 == puVar18) || (*(int *)((longlong)plVar22 + 0x1c) < *(int *)(puVar19 + 4))) {
        puStack_190 = puVar18;
        ppuVar16 = &puStack_190;
      }
      else {
        puStack_160 = puVar19;
        ppuVar16 = &puStack_160;
      }
      if (*ppuVar16 != puVar18) {
        plVar12 = (longlong *)FUN_1800853b0(param_1 + 0x40,(longlong)plVar22 + 0x1c);
        lVar3 = lStack_310;
        plVar5 = (longlong *)plVar12[1];
        lStack_e0 = *plVar12;
        if (plVar5 != (longlong *)0x0) {
          LOCK();
          *(int *)(plVar5 + 1) = (int)plVar5[1] + 1;
          UNLOCK();
        }
        plStack_d8 = plVar5;
        if (lStack_e0 != 0) {
          puVar18 = (undefined8 *)puStack_2b8[1];
          puVar19 = puStack_2b8;
          if (*(char *)((longlong)puStack_2b8[1] + 0x19) == '\0') {
            do {
              if (*(int *)((longlong)puVar18 + 0x1c) < *(int *)(lStack_e0 + 0x2c)) {
                puVar15 = (undefined8 *)puVar18[2];
              }
              else {
                puVar15 = (undefined8 *)*puVar18;
                puVar19 = puVar18;
              }
              puVar18 = puVar15;
            } while (*(char *)((longlong)puVar15 + 0x19) == '\0');
            if ((puVar19 == puStack_2b8) ||
               (*(int *)(lStack_e0 + 0x2c) < *(int *)((longlong)puVar19 + 0x1c)))
            goto LAB_18010bbd3;
            puStack_1d0 = puVar19;
            ppuVar16 = &puStack_1d0;
          }
          else {
LAB_18010bbd3:
            puStack_1c0 = puStack_2b8;
            ppuVar16 = &puStack_1c0;
          }
          if (*ppuVar16 == puStack_2b8) {
            if (plVar5 != (longlong *)0x0) {
              LOCK();
              *(int *)(plVar5 + 1) = (int)plVar5[1] + 1;
              UNLOCK();
            }
            lStack_220 = lStack_e0;
            plStack_218 = plVar5;
            lVar13 = FUN_180072dc0(puVar19,lStack_310,*(undefined8 *)(lStack_310 + 8),&lStack_220);
            plVar12 = plStack_218;
            if (lStack_308 == 0x7fffffffffffffe) {
              __Xlength_error_std__YAXPEBD_Z(&UNK_18021d318);
              break;
            }
            lStack_308 = lStack_308 + 1;
            *(longlong *)(lVar3 + 8) = lVar13;
            **(longlong **)(lVar13 + 8) = lVar13;
            if (plStack_218 != (longlong *)0x0) {
              LOCK();
              plVar1 = plStack_218 + 1;
              *(int *)plVar1 = (int)*plVar1 + -1;
              UNLOCK();
              if ((int)*plVar1 == 0) {
                (**(code **)*plStack_218)(plStack_218);
                LOCK();
                piVar2 = (int *)((longlong)plVar12 + 0xc);
                *piVar2 = *piVar2 + -1;
                UNLOCK();
                if (*piVar2 == 0) {
                  (**(code **)(*plStack_218 + 8))();
                }
              }
            }
          }
        }
        if (plVar5 != (longlong *)0x0) {
          LOCK();
          plVar12 = plVar5 + 1;
          *(int *)plVar12 = (int)*plVar12 + -1;
          UNLOCK();
          if ((int)*plVar12 == 0) {
            (**(code **)*plVar5)(plVar5);
            LOCK();
            piVar2 = (int *)((longlong)plVar5 + 0xc);
            *piVar2 = *piVar2 + -1;
            UNLOCK();
            if (*piVar2 == 0) {
              (**(code **)(*plVar5 + 8))(plVar5);
            }
          }
        }
      }
      if (*(char *)((longlong)plVar22 + 0x19) == '\0') {
        plVar5 = (longlong *)plVar22[2];
        if (*(char *)((longlong)plVar5 + 0x19) == '\0') {
          cVar9 = *(char *)(*plVar5 + 0x19);
          plVar22 = plVar5;
          plVar5 = (longlong *)*plVar5;
          while (cVar9 == '\0') {
            cVar9 = *(char *)(*plVar5 + 0x19);
            plVar22 = plVar5;
            plVar5 = (longlong *)*plVar5;
          }
        }
        else {
          cVar9 = *(char *)(plVar22[1] + 0x19);
          plVar12 = (longlong *)plVar22[1];
          plVar5 = plVar22;
          while ((plVar22 = plVar12, cVar9 == '\0' && (plVar5 == (longlong *)plVar22[2]))) {
            cVar9 = *(char *)(plVar22[1] + 0x19);
            plVar12 = (longlong *)plVar22[1];
            plVar5 = plVar22;
          }
        }
      }
      bVar23 = plVar22 == aplStack_1f8[0];
    }
    uStack_250 = 0;
    uStack_248 = 0;
    uStack_250 = FUN_18003c7f0();
    plVar22 = (longlong *)*aplStack_270[0];
    plStack_278 = aplStack_270[0];
    plStack_2c0 = plVar22;
    cVar9 = func_0x00018001eff0(&plStack_2c0,&plStack_278);
    while (cVar9 != '\0') {
      FUN_18011af10(auStack_150);
      uVar14 = func_0x00018006df90(&plStack_2c0);
      func_0x00018011afc0(auStack_150,uVar14);
      uVar14 = FUN_18009e260(&uStack_250,uVar14);
      cVar9 = func_0x00018011af60(auStack_150);
      while (cVar9 == '\0') {
        FUN_1800790b0(auStack_1e8);
        FUN_1800790b0(auStack_208);
        FUN_1800790b0(aplStack_290);
        puVar17 = (undefined4 *)FUN_18011af70(auStack_150);
        uVar4 = *puVar17;
        func_0x00018011afd0(auStack_150);
        pplStack_338 = aplStack_290;
        iVar10 = FUN_180111cc0(param_1,uVar4,auStack_1e8,auStack_208);
        if (iVar10 == 0) {
          puVar18 = (undefined8 *)func_0x00018001ee10(auStack_1e8,auStack_100);
          puVar19 = (undefined8 *)func_0x00018001ee00(auStack_1e8,auStack_c8);
          FUN_180071720(uVar14,*puVar19,*puVar18);
          puVar18 = (undefined8 *)func_0x00018001ee10(auStack_208,auStack_e8);
          puVar19 = (undefined8 *)func_0x00018001ee00(auStack_208,auStack_c0);
          FUN_180071720(uVar14,*puVar19,*puVar18);
          puVar18 = (undefined8 *)func_0x00018001ee10(aplStack_290,auStack_d0);
          puVar19 = (undefined8 *)func_0x00018001ee00(aplStack_290,auStack_118);
          FUN_180071720(uVar14,*puVar19,*puVar18);
          func_0x00018001ee00(aplStack_290,auStack_2a8);
          uVar20 = func_0x00018001ee10(aplStack_290,auStack_1b0);
          cVar9 = func_0x00018001eff0(auStack_2a8,uVar20);
          while (cVar9 != '\0') {
            uVar20 = func_0x00018006df90(auStack_2a8);
            func_0x00018011afc0(auStack_150,uVar20);
            func_0x000180079050(auStack_2a8,&lStack_178,0);
            uVar20 = func_0x00018001ee10(aplStack_290,auStack_1b0);
            cVar9 = func_0x00018001eff0(auStack_2a8,uVar20);
          }
        }
        FUN_180061470(aplStack_290);
        FUN_180061470(auStack_208);
        FUN_180061470(auStack_1e8);
        cVar9 = func_0x00018011af60(auStack_150);
      }
      FUN_18007fbe0(auStack_150);
      if (*(char *)((longlong)plVar22 + 0x19) == '\0') {
        plVar5 = (longlong *)plVar22[2];
        if (*(char *)((longlong)plVar5 + 0x19) == '\0') {
          cVar9 = *(char *)(*plVar5 + 0x19);
          plVar22 = plVar5;
          plVar5 = (longlong *)*plVar5;
          while (plStack_2c0 = plVar22, cVar9 == '\0') {
            cVar9 = *(char *)(*plVar5 + 0x19);
            plVar22 = plVar5;
            plVar5 = (longlong *)*plVar5;
          }
        }
        else {
          cVar9 = *(char *)(plVar22[1] + 0x19);
          plVar12 = (longlong *)plVar22[1];
          plVar5 = plVar22;
          while ((plVar22 = plVar12, plStack_2c0 = plVar22, cVar9 == '\0' &&
                 (plVar5 == (longlong *)plVar22[2]))) {
            cVar9 = *(char *)(plVar22[1] + 0x19);
            plVar12 = (longlong *)plVar22[1];
            plVar5 = plVar22;
          }
        }
      }
      plStack_278 = aplStack_270[0];
      cVar9 = func_0x00018001eff0(&plStack_2c0,&plStack_278);
    }
    func_0x00018011ae50(param_1 + 0xe0,&uStack_250);
    cVar9 = FUN_18001dbc0(1);
    if (cVar9 != '\0') {
      uVar14 = FUN_180106a40(auStack_58,&lStack_310);
      uVar14 = func_0x000180006450(uVar14);
      uVar20 = FUN_18001db50();
      iStack_330 = aiStack_318[0];
      pplStack_338 = (longlong **)CONCAT44(pplStack_338._4_4_,0x293);
      uStack_328 = uVar14;
      FUN_180015b40(uVar20,1,&UNK_180230c40,&UNK_180230be8);
      FUN_1800062c0(auStack_58);
    }
    lStack_178 = __2_YAPEAX_K_Z(0x38);
    if (lStack_178 != 0) {
      uVar11 = FUN_180020ae0(lStack_178,&lStack_310,aiStack_318[0]);
    }
    FUN_1800eb4b0(auStack_128,uVar11);
    FUN_180108ba0(param_1,auStack_1a0,aiStack_318[0]);
    cVar9 = func_0x000180070740(auStack_1a0);
    if (cVar9 != '\0') {
      uVar11 = FUN_18006e3e0(auStack_88,auStack_128);
      uVar14 = func_0x00018001efe0(auStack_1a0);
      FUN_1800dcba0(uVar14,uVar11);
      FUN_1800e23d0(uVar14,&uStack_2d0);
      func_0x00018001ee00(&uStack_2d0,auStack_280);
      func_0x00018001ee10(&uStack_2d0,auStack_1d8);
      while (cVar9 = func_0x00018001eff0(auStack_280,auStack_1d8), cVar9 != '\0') {
        func_0x00018006df90(auStack_280);
        uVar11 = func_0x0001800e2420(uVar14);
        uVar11 = FUN_18008e6c0(uVar11);
        func_0x00018008ffb0(uVar11);
        FUN_1800ce430();
        func_0x0001800790e0(auStack_280);
      }
    }
    FUN_18001d910(auStack_1a0);
    FUN_18001d910(auStack_128);
    FUN_18009b450(&uStack_250);
    FUN_180061470(aplStack_1f8);
    FUN_180061470(&uStack_2d0);
    FUN_180061470(&plStack_2a0);
    FUN_180061470(&puStack_2b8);
    FUN_18001ede0(&lStack_310);
    FUN_180061470(aplStack_270);
    FUN_180061470(aplStack_260);
    FUN_18001f940(&lStack_210);
  }
  FUN_1801e8810(uStack_38 ^ (ulonglong)auStack_358);
  return;
}

