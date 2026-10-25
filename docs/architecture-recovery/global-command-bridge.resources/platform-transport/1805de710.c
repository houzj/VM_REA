
/* WARNING: Removing unreachable block (ram,0x0001805ded34) */
/* WARNING: Removing unreachable block (ram,0x0001805ded77) */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_1805de710(longlong param_1,char *param_2,longlong param_3,int param_4,undefined8 param_5,
                  char param_6)

{
  undefined4 uVar1;
  int iVar2;
  char cVar3;
  int iVar4;
  code *pcVar5;
  undefined8 uVar6;
  code *pcVar7;
  char *pcVar8;
  longlong *plVar9;
  int *piVar10;
  ulonglong uVar11;
  longlong *plVar12;
  longlong lVar13;
  undefined4 *puVar14;
  int *piVar15;
  ulonglong uVar16;
  ushort uVar17;
  undefined1 auStack_3c8 [32];
  char **ppcStack_3a8;
  char *pcStack_3a0;
  char *pcStack_398;
  int iStack_390;
  char *pcStack_388;
  int iStack_380;
  undefined8 uStack_378;
  char *pcStack_370;
  undefined8 uStack_368;
  undefined8 uStack_360;
  undefined8 uStack_358;
  undefined8 uStack_350;
  undefined8 uStack_348;
  undefined8 uStack_340;
  undefined4 uStack_338;
  undefined8 uStack_330;
  undefined4 uStack_328;
  undefined8 uStack_318;
  undefined1 auStack_310 [16];
  ulonglong *puStack_300;
  undefined8 uStack_2f8;
  ulonglong uStack_2e8;
  ulonglong uStack_2e0;
  longlong *plStack_2d8;
  longlong *plStack_2d0;
  undefined8 uStack_2c8;
  undefined8 uStack_2c0;
  undefined8 uStack_2b8;
  longlong lStack_2b0;
  undefined8 uStack_2a8;
  undefined8 uStack_2a0;
  int iStack_298;
  int iStack_294;
  undefined8 uStack_290;
  char **ppcStack_288;
  undefined8 uStack_280;
  char *pcStack_278;
  char *pcStack_270;
  char *pcStack_268;
  char *pcStack_260;
  char cStack_258;
  undefined1 auStack_257 [511];
  ulonglong uStack_58;
  
  uStack_318 = 0xfffffffffffffffe;
  uStack_58 = _DAT_1808559e0 ^ (ulonglong)auStack_3c8;
  uStack_378 = param_5;
  iStack_380 = param_4;
  if ((param_2 == (char *)0x0) || (param_3 == 0)) {
    cVar3 = FUN_18057a5b0(4);
    if (cVar3 != '\0') {
      uVar6 = FUN_18057a310();
      ppcStack_3a8 = (char **)CONCAT44(ppcStack_3a8._4_4_,0xef0);
      FUN_18057a4b0(uVar6,4,&UNK_1807d7628,&UNK_1807d7508);
    }
    goto LAB_1805df18b;
  }
  pcVar5 = (code *)0x0;
  pcStack_370 = (char *)0x0;
  cStack_258 = '\0';
  func_0x000180746610(auStack_257,0,0x1ff);
  pcStack_388 = (char *)0x1ff;
  pcVar8 = &cStack_258;
  pcVar7 = *(code **)(param_1 + 0x7d518);
  if (*(code **)(param_1 + 0x7d518) == (code *)0x0) {
    if (*(longlong *)(param_1 + 0x7d510) != 0) {
      pcVar5 = (code *)GetProcAddress(*(longlong *)(param_1 + 0x7d510),&UNK_1807e3ba8);
    }
    *(code **)(param_1 + 0x7d518) = pcVar5;
    pcVar7 = pcVar5;
    if (pcVar5 != (code *)0x0) goto LAB_1805de843;
    cVar3 = FUN_18057a5b0(4);
    if (cVar3 != '\0') {
      pcStack_3a0 = (char *)FUN_18063bb80(&pcStack_278);
      if (0xf < *(ulonglong *)(pcStack_3a0 + 0x18)) {
        pcStack_3a0 = *(char **)pcStack_3a0;
      }
      uVar6 = FUN_18057a310();
      ppcStack_3a8 = (char **)CONCAT44(ppcStack_3a8._4_4_,99);
      FUN_18057a4b0(uVar6,4,&UNK_1807e3b88,&UNK_1807e3bc8);
      if ((char *)0xf < pcStack_260) {
        __3_YAXPEAX_Z(pcStack_278);
      }
    }
    iVar4 = -0x1ffffff6;
LAB_1805de97e:
    cVar3 = FUN_18057a5b0(4);
    if (cVar3 != '\0') {
      uVar6 = FUN_18057a310();
      pcStack_3a0 = (char *)CONCAT44(pcStack_3a0._4_4_,iVar4);
      ppcStack_3a8 = (char **)CONCAT44(ppcStack_3a8._4_4_,0xf12);
      FUN_18057a4b0(uVar6,4,&UNK_1807d75b0,&UNK_1807d7508);
    }
  }
  else {
LAB_1805de843:
    ppcStack_3a8 = (char **)0x0;
    iVar4 = (*pcVar7)(param_2,param_3,&cStack_258,&pcStack_388);
    if (iVar4 == -0x1fffffee) {
      cVar3 = FUN_18057a5b0(3);
      if (cVar3 != '\0') {
        uVar6 = FUN_18057a310();
        pcStack_3a0 = pcStack_388;
        ppcStack_3a8 = (char **)CONCAT44(ppcStack_3a8._4_4_,0xeff);
        FUN_18057a4b0(uVar6,3,&UNK_1807d75f0,&UNK_1807d7508);
      }
      pcVar8 = (char *)func_0x000180745d3c(pcStack_388 + 1,0x180878bb9);
      pcStack_370 = pcVar8;
      if (pcVar8 == (char *)0x0) {
        cVar3 = FUN_18057a5b0(4);
        if (cVar3 != '\0') {
          uVar6 = FUN_18057a310();
          ppcStack_3a8 = (char **)CONCAT44(ppcStack_3a8._4_4_,0xf04);
          FUN_18057a4b0(uVar6,4,&UNK_1807d75d8,&UNK_1807d7508);
        }
        goto LAB_1805df18b;
      }
      pcStack_3a0 = (char *)0x0;
      ppcStack_3a8 = &pcStack_388;
      iVar4 = FUN_18063c0c0(param_1 + 0x7d510,param_2,param_3,pcVar8);
      if (iVar4 == 0) goto LAB_1805de9c4;
      cVar3 = FUN_18057a5b0(4);
      if (cVar3 != '\0') {
        uVar6 = FUN_18057a310();
        pcStack_3a0 = (char *)CONCAT44(pcStack_3a0._4_4_,iVar4);
        ppcStack_3a8 = (char **)CONCAT44(ppcStack_3a8._4_4_,0xf0c);
        FUN_18057a4b0(uVar6,4,&UNK_1807d75b0,&UNK_1807d7508);
      }
    }
    else {
      if (iVar4 != 0) goto LAB_1805de97e;
LAB_1805de9c4:
      plVar12 = (longlong *)0x0;
      pcStack_388[(longlong)pcVar8] = '\0';
      uStack_2e8 = 0;
      uStack_2e0 = 0;
      plStack_2d8 = (longlong *)0x0;
      plStack_2d0 = (longlong *)0x0;
      uStack_2c8 = 0;
      uStack_2c0 = 0;
      uStack_2b8 = 0;
      lStack_2b0 = 0;
      uStack_2a8 = 0;
      uStack_2a0 = 0x400;
      iStack_298 = 0;
      uStack_290 = 0;
      plVar9 = (longlong *)__2_YAPEAX_K_Z(0x28);
      if (plVar9 != (longlong *)0x0) {
        *plVar9 = 0;
        plVar9[1] = 0x10000;
        plVar9[2] = 0;
        plVar9[3] = 0;
        plVar9[4] = 0;
        plVar12 = plVar9;
      }
      pcStack_268 = pcStack_388 + (longlong)pcVar8;
      pcStack_260 = pcStack_388;
      ppcStack_288 = &pcStack_278;
      pcStack_278 = pcVar8;
      if (pcVar8 != pcStack_268) {
        if (*pcVar8 == -0x11) {
          pcStack_278 = pcVar8 + 1;
        }
        if (pcStack_278 != pcStack_268) {
          if (*pcStack_278 == -0x45) {
            pcStack_278 = pcStack_278 + 1;
          }
          if ((pcStack_278 != pcStack_268) && (*pcStack_278 == -0x41)) {
            pcStack_278 = pcStack_278 + 1;
          }
        }
      }
      uStack_368 = uStack_2c8;
      uStack_360 = 0;
      uStack_358 = 0;
      uStack_350 = 0;
      uStack_348 = 0;
      uStack_340 = 0x100;
      uStack_338 = 0;
      uStack_330 = 0;
      uStack_328 = 2;
      puStack_300 = &uStack_2e8;
      plStack_2d8 = plVar12;
      plStack_2d0 = plVar12;
      pcStack_270 = pcVar8;
      piVar10 = (int *)FUN_1805fbff0(&uStack_368,auStack_310,&ppcStack_288,&uStack_2e8);
      iStack_298 = *piVar10;
      iStack_294 = piVar10[1];
      uStack_290 = *(undefined8 *)(piVar10 + 2);
      if ((iStack_298 == 0) && (&uStack_2e8 != (ulonglong *)(lStack_2b0 + -0x10))) {
        uStack_2e8 = *(ulonglong *)(lStack_2b0 + -0x10);
        uStack_2e0 = *(ulonglong *)(lStack_2b0 + -8);
        *(undefined2 *)(lStack_2b0 + -2) = 0;
      }
      lStack_2b0 = uStack_2b8;
      free();
      uStack_2b8 = 0;
      lStack_2b0 = 0;
      uStack_2a8 = 0;
      free(uStack_358);
      __3_YAXPEAX_Z(uStack_360);
      if (iStack_298 == 0) {
        uVar16 = uStack_2e8 & 0xffffffff;
        uVar11 = uStack_2e0 & 0xffffffffffff;
        plVar12 = (longlong *)FUN_1805fb710(&uStack_2e8,&puStack_300,&UNK_1807d7690);
        if (*plVar12 == uVar16 * 0x20 + uVar11) {
          cVar3 = FUN_18057a5b0(4);
          if (cVar3 != '\0') {
            uVar6 = FUN_18057a310();
            ppcStack_3a8 = (char **)CONCAT44(ppcStack_3a8._4_4_,0xf22);
            FUN_18057a4b0(uVar6,4,&UNK_1807d7658,&UNK_1807d7508);
          }
          plVar12 = plStack_2d0;
          if (plStack_2d0 != (longlong *)0x0) {
            if (*plStack_2d0 == 0) goto LAB_1805debc3;
            do {
              if (*plVar12 == plVar12[2]) goto LAB_1805debc3;
              lVar13 = *(longlong *)(*plVar12 + 0x10);
              free(*plVar12);
              *plVar12 = lVar13;
            } while (lVar13 != 0);
            goto LAB_1805df112;
          }
        }
        else {
          uStack_2f8 = 0x4050001807d7690;
          puStack_300 = (ulonglong *)0x4;
          piVar10 = (int *)(uStack_2e0 & 0xffffffffffff);
          uVar11 = uStack_2e8 & 0xffffffff;
          if (piVar10 != piVar10 + uVar11 * 8) {
            do {
              ppcStack_288 = (char **)0x0;
              uStack_280 = 0x1f6000000000000;
              cVar3 = FUN_1805fc0f0(piVar10,&ppcStack_288);
              if (cVar3 == '\0') {
                uVar17 = *(ushort *)((longlong)piVar10 + 0xe) & 0x1000;
                if (uVar17 == 0) {
                  iVar4 = *piVar10;
                }
                else {
                  iVar4 = 0xd - *(char *)((longlong)piVar10 + 0xd);
                }
                if (iVar4 == 4) {
                  piVar15 = piVar10;
                  if (uVar17 == 0) {
                    piVar15 = (int *)(*(ulonglong *)(piVar10 + 2) & 0xffffffffffff);
                  }
                  if ((piVar15 == (int *)&UNK_1807d7690) ||
                     (iVar4 = func_0x00018074660a(), iVar4 == 0)) {
                    uVar11 = uStack_2e8 & 0xffffffff;
                    param_4 = iStack_380;
                    break;
                  }
                }
              }
              piVar10 = piVar10 + 8;
              uVar11 = uStack_2e8 & 0xffffffff;
              param_4 = iStack_380;
            } while (piVar10 != (int *)(uVar11 * 0x20 + (uStack_2e0 & 0xffffffffffff)));
          }
          if (piVar10 == (int *)(uVar11 * 0x20 + (uStack_2e0 & 0xffffffffffff))) {
            _DAT_1808793b8 = 0;
            _DAT_1808793c0 = 0;
            piVar10 = (int *)&DAT_1808793b8;
          }
          else {
            piVar10 = piVar10 + 4;
          }
          lVar13 = FUN_1805fb9b0(piVar10,&UNK_1807d7650);
          if ((((*(byte *)(lVar13 + 0xe) >> 5 & 1) == 0) ||
              (lVar13 = FUN_1805fb9b0(piVar10,&UNK_1807d7738),
              (*(byte *)(lVar13 + 0xe) >> 5 & 1) == 0)) ||
             (lVar13 = FUN_1805fb9b0(piVar10,&UNK_1807d7728),
             (*(byte *)(lVar13 + 0xe) >> 5 & 1) == 0)) {
            cVar3 = FUN_18057a5b0(4);
            if (cVar3 != '\0') {
              uVar6 = FUN_18057a310();
              ppcStack_3a8 = (char **)CONCAT44(ppcStack_3a8._4_4_,0xf2e);
              pcStack_3a0 = param_2;
              pcStack_398 = pcVar8;
              FUN_18057a4b0(uVar6,4,&UNK_1807d7700,&UNK_1807d7508);
            }
            plVar12 = plStack_2d0;
            if (plStack_2d0 != (longlong *)0x0) {
              if (*plStack_2d0 == 0) {
LAB_1805df100:
                lVar13 = *plVar12;
                if ((lVar13 != 0) && (lVar13 == plVar12[2])) {
                  *(undefined8 *)(lVar13 + 8) = 0;
                }
              }
              else {
                do {
                  if (*plVar12 == plVar12[2]) goto LAB_1805df100;
                  lVar13 = *(longlong *)(*plVar12 + 0x10);
                  free(*plVar12);
                  *plVar12 = lVar13;
                } while (lVar13 != 0);
              }
              goto LAB_1805df112;
            }
          }
          else {
            puVar14 = (undefined4 *)FUN_1805fb9b0(piVar10,&UNK_1807d7650);
            uVar1 = *puVar14;
            piVar15 = (int *)FUN_1805fb9b0(piVar10,&UNK_1807d7738);
            iVar4 = *piVar15;
            piVar10 = (int *)FUN_1805fb9b0(piVar10,&UNK_1807d7728);
            iVar2 = *piVar10;
            if (iVar4 == param_4) {
              if (iVar2 == 0) {
                if (param_6 != '\0') {
                  if (*pcVar8 == '\0') {
                    lVar13 = 0;
                  }
                  else {
                    lVar13 = -1;
                    do {
                      lVar13 = lVar13 + 1;
                    } while (pcVar8[lVar13] != '\0');
                  }
                  FUN_180577de0(uStack_378,pcVar8,lVar13);
                }
                plVar12 = plStack_2d0;
                if (plStack_2d0 != (longlong *)0x0) {
                  if (*plStack_2d0 == 0) goto LAB_1805def45;
                  do {
                    if (*plVar12 == plVar12[2]) goto LAB_1805def45;
                    lVar13 = *(longlong *)(*plVar12 + 0x10);
                    free();
                    *plVar12 = lVar13;
                  } while (lVar13 != 0);
                  goto LAB_1805df112;
                }
              }
              else {
                cVar3 = FUN_18057a5b0(4);
                if (cVar3 != '\0') {
                  uVar6 = FUN_18057a310();
                  pcStack_398 = (char *)CONCAT44(pcStack_398._4_4_,iVar2);
                  pcStack_3a0 = (char *)CONCAT44(pcStack_3a0._4_4_,uVar1);
                  ppcStack_3a8 = (char **)CONCAT44(ppcStack_3a8._4_4_,0xf3f);
                  FUN_18057a4b0(uVar6,4,&UNK_1807d7748,&UNK_1807d7508);
                }
                if (*pcVar8 == '\0') {
                  lVar13 = 0;
                }
                else {
                  lVar13 = -1;
                  do {
                    lVar13 = lVar13 + 1;
                  } while (pcVar8[lVar13] != '\0');
                }
                FUN_180577de0(uStack_378,pcVar8,lVar13);
                plVar12 = plStack_2d0;
                if (plStack_2d0 != (longlong *)0x0) {
                  if (*plStack_2d0 == 0) goto LAB_1805df100;
                  do {
                    if (*plVar12 == plVar12[2]) goto LAB_1805df100;
                    lVar13 = *(longlong *)(*plVar12 + 0x10);
                    free(*plVar12);
                    *plVar12 = lVar13;
                  } while (lVar13 != 0);
LAB_1805df112:
                  __3_YAXPEAX_Z(plVar12[4]);
                  __3_YAXPEAX_Z(plVar12);
                }
              }
            }
            else {
              cVar3 = FUN_18057a5b0();
              if (cVar3 != '\0') {
                uVar6 = FUN_18057a310();
                pcStack_398 = (char *)CONCAT44(pcStack_398._4_4_,param_4);
                pcStack_3a0 = (char *)CONCAT44(pcStack_3a0._4_4_,uVar1);
                ppcStack_3a8 = (char **)CONCAT44(ppcStack_3a8._4_4_,0xf38);
                iStack_390 = iVar4;
                FUN_18057a4b0(uVar6,4,&UNK_1807d76c0,&UNK_1807d7508);
              }
              plVar12 = plStack_2d0;
              if (plStack_2d0 != (longlong *)0x0) {
                if (*plStack_2d0 == 0) {
LAB_1805def45:
                  lVar13 = *plVar12;
                  if ((lVar13 != 0) && (lVar13 == plVar12[2])) {
                    *(undefined8 *)(lVar13 + 8) = 0;
                  }
                }
                else {
                  do {
                    if (*plVar12 == plVar12[2]) goto LAB_1805def45;
                    lVar13 = *(longlong *)(*plVar12 + 0x10);
                    free();
                    *plVar12 = lVar13;
                  } while (lVar13 != 0);
                }
                goto LAB_1805df112;
              }
            }
          }
        }
      }
      else {
        cVar3 = FUN_18057a5b0(4);
        if (cVar3 != '\0') {
          uVar6 = FUN_18057a310();
          ppcStack_3a8 = (char **)CONCAT44(ppcStack_3a8._4_4_,0xf1c);
          pcStack_3a0 = pcVar8;
          FUN_18057a4b0(uVar6,4,&UNK_1807d7698,&UNK_1807d7508);
        }
        plVar12 = plStack_2d0;
        if (plStack_2d0 != (longlong *)0x0) {
          if (*plStack_2d0 == 0) {
LAB_1805debc3:
            lVar13 = *plVar12;
            if ((lVar13 != 0) && (lVar13 == plVar12[2])) {
              *(undefined8 *)(lVar13 + 8) = 0;
            }
          }
          else {
            do {
              if (*plVar12 == plVar12[2]) goto LAB_1805debc3;
              lVar13 = *(longlong *)(*plVar12 + 0x10);
              free(*plVar12);
              *plVar12 = lVar13;
            } while (lVar13 != 0);
          }
          goto LAB_1805df112;
        }
      }
      free(uStack_2b8);
      __3_YAXPEAX_Z(uStack_2c0);
    }
    if (pcStack_370 != (char *)0x0) {
      ___V_YAXPEAX_Z(pcStack_370);
    }
  }
LAB_1805df18b:
  FUN_180745b60(uStack_58 ^ (ulonglong)auStack_3c8);
  return;
}

