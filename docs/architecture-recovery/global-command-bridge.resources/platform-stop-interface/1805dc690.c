
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

int FUN_1805dc690(longlong param_1,int param_2,char param_3)

{
  int *piVar1;
  longlong *plVar2;
  longlong lVar3;
  longlong lVar4;
  longlong **pplVar5;
  int iVar6;
  undefined8 *puVar7;
  undefined8 *puVar8;
  longlong lVar9;
  longlong *plVar10;
  longlong *plVar11;
  char cVar12;
  undefined8 uVar13;
  longlong *plVar14;
  undefined8 *puVar15;
  undefined8 *puVar16;
  longlong *plVar17;
  undefined8 **ppuVar18;
  undefined4 uVar19;
  undefined4 extraout_XMM0_Da;
  int iStackX_10;
  undefined8 *puStackX_20;
  longlong *plStack_e8;
  undefined8 uStack_e0;
  longlong *plStack_d8;
  undefined8 uStack_d0;
  longlong *plStack_c8;
  longlong *plStack_c0;
  longlong *plStack_b8;
  longlong *plStack_b0;
  longlong *plStack_a8;
  undefined8 uStack_a0;
  longlong *plStack_98;
  undefined8 uStack_90;
  longlong *plStack_88;
  longlong *plStack_80;
  undefined8 *apuStack_78 [2];
  longlong *plStack_68;
  longlong lStack_60;
  undefined8 uStack_58;
  longlong lStack_50;
  undefined1 auStack_48 [16];
  
  uStack_58 = 0xfffffffffffffffe;
  iStackX_10 = 0;
  if (param_2 < 10000) {
    cVar12 = FUN_18057a5b0(4);
    if (cVar12 != '\0') {
      uVar13 = FUN_18057a310();
      FUN_18057a4b0(uVar13,4,&UNK_1807d71f8,&UNK_1807d70b0,0xc8a,param_2);
    }
    iStackX_10 = -0x1fffffff;
  }
  else {
    if (param_3 != '\0') {
      iStackX_10 = FUN_1805df3b0();
      if (iStackX_10 == 0) {
        cVar12 = FUN_18057a5b0(2);
        if (cVar12 != '\0') {
          uVar13 = FUN_18057a310();
          FUN_18057a4b0(uVar13,2,&UNK_1807d7190,&UNK_1807d70b0,0xc98,param_2);
        }
      }
      else {
        cVar12 = FUN_18057a5b0(4);
        if (cVar12 != '\0') {
          uVar13 = FUN_18057a310();
          FUN_18057a4b0(uVar13,4,&UNK_1807d71c0,&UNK_1807d70b0,0xc94,param_2,iStackX_10);
        }
      }
    }
    uVar19 = FUN_1805dd580(param_1,&plStack_88,param_2);
    plVar14 = plStack_88;
    if (plStack_88 == (longlong *)0x0) {
      cVar12 = FUN_18057a5b0(4);
      if (cVar12 != '\0') {
        if ((_DAT_1808793b0 & 1) == 0) {
          _DAT_1808793b0 = _DAT_1808793b0 | 1;
          _DAT_180879360 = 1;
          _DAT_180879368 = 0;
          _DAT_180879370 = (longlong *)0x0;
          uRam0000000180879378 = 0;
          _DAT_180879380 = 0;
          uRam0000000180879388 = 0;
          _DAT_180879390 = 0;
          uRam0000000180879398 = 0;
          _DAT_1808793a0 = 0;
          FUN_180745c88(&UNK_18075e4e0);
        }
        FUN_18057a4b0(&DAT_180879360,4,&UNK_1807d7170,&UNK_1807d70b0,0xca2,param_2);
      }
    }
    else {
      plStack_e8 = (longlong *)0x0;
      uStack_e0 = 0;
      plStack_e8 = (longlong *)FUN_1805a18b0(uVar19,0,0);
      (**(code **)(*plVar14 + 0xe0))(plVar14,&plStack_e8);
      plVar17 = (longlong *)*plStack_e8;
      if (plVar17 != plStack_e8) {
        do {
          FUN_1805dfb50(param_1,param_2,(int)plVar17[2]);
          FUN_1805d6520(param_1,&plStack_c8,(int)plVar17[2]);
          if (plStack_c8 == (longlong *)0x0) {
            if ((_DAT_1808793b0 & 1) == 0) {
              _DAT_1808793b0 = _DAT_1808793b0 | 1;
              _DAT_180879360 = 1;
              _DAT_180879368 = 0;
              _DAT_180879370 = (longlong *)0x0;
              uRam0000000180879378 = 0;
              _DAT_180879380 = 0;
              uRam0000000180879388 = 0;
              _DAT_180879390 = 0;
              uRam0000000180879398 = 0;
              _DAT_1808793a0 = 0;
              FUN_180745c88(&UNK_18075e4e0);
            }
            plVar14 = _DAT_180879370;
            lVar3 = _DAT_180879368;
            if (_DAT_180879370 != (longlong *)0x0) {
              LOCK();
              *(int *)(_DAT_180879370 + 1) = (int)_DAT_180879370[1] + 1;
              UNLOCK();
            }
            if (lVar3 == 0) {
              if (plVar14 != (longlong *)0x0) {
                LOCK();
                plVar10 = plVar14 + 1;
                *(int *)plVar10 = (int)*plVar10 + -1;
                UNLOCK();
                if ((int)*plVar10 == 0) {
                  (**(code **)*plVar14)(plVar14);
                  LOCK();
                  piVar1 = (int *)((longlong)plVar14 + 0xc);
                  *piVar1 = *piVar1 + -1;
                  UNLOCK();
                  if (*piVar1 == 0) {
                    (**(code **)(*plVar14 + 8))(plVar14);
                  }
                }
              }
            }
            else {
              iVar6 = *(int *)(lVar3 + 0x40);
              if (plVar14 != (longlong *)0x0) {
                LOCK();
                plVar10 = plVar14 + 1;
                *(int *)plVar10 = (int)*plVar10 + -1;
                UNLOCK();
                if ((int)*plVar10 == 0) {
                  (**(code **)*plVar14)(plVar14);
                  LOCK();
                  piVar1 = (int *)((longlong)plVar14 + 0xc);
                  *piVar1 = *piVar1 + -1;
                  UNLOCK();
                  if (*piVar1 == 0) {
                    (**(code **)(*plVar14 + 8))(plVar14);
                  }
                }
              }
              if (iVar6 < 4) {
                if ((_DAT_1808793b0 & 1) == 0) {
                  _DAT_1808793b0 = _DAT_1808793b0 | 1;
                  _DAT_180879360 = 1;
                  _DAT_180879368 = 0;
                  _DAT_180879370 = (longlong *)0x0;
                  uRam0000000180879378 = 0;
                  _DAT_180879380 = 0;
                  uRam0000000180879388 = 0;
                  _DAT_180879390 = 0;
                  uRam0000000180879398 = 0;
                  _DAT_1808793a0 = 0;
                  FUN_180745c88(&UNK_18075e4e0);
                }
                FUN_18057a4b0(&DAT_180879360,3,&UNK_1807d7270,&UNK_1807d70b0,0xcb1,(int)plVar17[2]);
              }
            }
          }
          else {
            (**(code **)(*plStack_c8 + 200))(plStack_c8,param_2);
          }
          plVar14 = plStack_c0;
          if (plStack_c0 != (longlong *)0x0) {
            LOCK();
            plVar10 = plStack_c0 + 1;
            *(int *)plVar10 = (int)*plVar10 + -1;
            UNLOCK();
            if ((int)*plVar10 == 0) {
              (**(code **)*plStack_c0)(plStack_c0);
              LOCK();
              piVar1 = (int *)((longlong)plVar14 + 0xc);
              *piVar1 = *piVar1 + -1;
              UNLOCK();
              if (*piVar1 == 0) {
                (**(code **)(*plStack_c0 + 8))();
              }
            }
          }
          plVar17 = (longlong *)*plVar17;
          plVar14 = plStack_88;
        } while (plVar17 != plStack_e8);
      }
      (**(code **)(*plVar14 + 0xd8))(plVar14,&plStack_e8);
      plVar14 = (longlong *)*plStack_e8;
      if (plVar14 != plStack_e8) {
        do {
          FUN_1805dfb50(param_1,(int)plVar14[2],param_2);
          FUN_1805d6520(param_1,&plStack_b8,(int)plVar14[2]);
          if (plStack_b8 == (longlong *)0x0) {
            if ((_DAT_1808793b0 & 1) == 0) {
              _DAT_1808793b0 = _DAT_1808793b0 | 1;
              _DAT_180879360 = 1;
              _DAT_180879368 = 0;
              _DAT_180879370 = (longlong *)0x0;
              uRam0000000180879378 = 0;
              _DAT_180879380 = 0;
              uRam0000000180879388 = 0;
              _DAT_180879390 = 0;
              uRam0000000180879398 = 0;
              _DAT_1808793a0 = 0;
              FUN_180745c88(&UNK_18075e4e0);
            }
            plVar17 = _DAT_180879370;
            lVar3 = _DAT_180879368;
            if (_DAT_180879370 != (longlong *)0x0) {
              LOCK();
              *(int *)(_DAT_180879370 + 1) = (int)_DAT_180879370[1] + 1;
              UNLOCK();
            }
            if (lVar3 == 0) {
              if (plVar17 != (longlong *)0x0) {
                LOCK();
                plVar10 = plVar17 + 1;
                *(int *)plVar10 = (int)*plVar10 + -1;
                UNLOCK();
                if ((int)*plVar10 == 0) {
                  (**(code **)*plVar17)(plVar17);
                  LOCK();
                  piVar1 = (int *)((longlong)plVar17 + 0xc);
                  *piVar1 = *piVar1 + -1;
                  UNLOCK();
                  if (*piVar1 == 0) {
                    (**(code **)(*plVar17 + 8))(plVar17);
                  }
                }
              }
            }
            else {
              iVar6 = *(int *)(lVar3 + 0x40);
              if (plVar17 != (longlong *)0x0) {
                LOCK();
                plVar10 = plVar17 + 1;
                *(int *)plVar10 = (int)*plVar10 + -1;
                UNLOCK();
                if ((int)*plVar10 == 0) {
                  (**(code **)*plVar17)(plVar17);
                  LOCK();
                  piVar1 = (int *)((longlong)plVar17 + 0xc);
                  *piVar1 = *piVar1 + -1;
                  UNLOCK();
                  if (*piVar1 == 0) {
                    (**(code **)(*plVar17 + 8))(plVar17);
                  }
                }
              }
              if (iVar6 < 4) {
                if ((_DAT_1808793b0 & 1) == 0) {
                  _DAT_1808793b0 = _DAT_1808793b0 | 1;
                  _DAT_180879360 = 1;
                  _DAT_180879368 = 0;
                  _DAT_180879370 = (longlong *)0x0;
                  uRam0000000180879378 = 0;
                  _DAT_180879380 = 0;
                  uRam0000000180879388 = 0;
                  _DAT_180879390 = 0;
                  uRam0000000180879398 = 0;
                  _DAT_1808793a0 = 0;
                  FUN_180745c88(&UNK_18075e4e0);
                }
                FUN_18057a4b0(&DAT_180879360,3,&UNK_1807d7270,&UNK_1807d70b0,0xcc4,(int)plVar14[2]);
              }
            }
          }
          else {
            (**(code **)(*plStack_b8 + 0xd0))(plStack_b8,param_2);
          }
          plVar17 = plStack_b0;
          if (plStack_b0 != (longlong *)0x0) {
            LOCK();
            plVar10 = plStack_b0 + 1;
            *(int *)plVar10 = (int)*plVar10 + -1;
            UNLOCK();
            if ((int)*plVar10 == 0) {
              (**(code **)*plStack_b0)(plStack_b0);
              LOCK();
              piVar1 = (int *)((longlong)plVar17 + 0xc);
              *piVar1 = *piVar1 + -1;
              UNLOCK();
              if (*piVar1 == 0) {
                (**(code **)(*plStack_b0 + 8))();
              }
            }
          }
          plVar14 = (longlong *)*plVar14;
        } while (plVar14 != plStack_e8);
      }
      plVar14 = (longlong *)*plStack_e8;
      *plStack_e8 = (longlong)plStack_e8;
      plStack_e8[1] = (longlong)plStack_e8;
      uStack_e0 = 0;
      if (plVar14 != plStack_e8) {
        do {
          plVar14 = (longlong *)*plVar14;
          __3_YAXPEAX_Z();
        } while (plVar14 != plStack_e8);
      }
      __3_YAXPEAX_Z(plStack_e8);
    }
    plStack_d8 = (longlong *)0x0;
    uStack_d0 = 0;
    plStack_d8 = (longlong *)FUN_1805a1950(&plStack_d8,0,0);
    uStack_a0 = 0;
    plStack_a8 = (longlong *)FUN_18058c5d0(extraout_XMM0_Da,0,0);
    FUN_1805dd720(param_1,&plStack_a8);
    plVar17 = plStack_a8;
    for (plVar14 = (longlong *)*plStack_a8; plVar14 != plVar17; plVar14 = (longlong *)*plVar14) {
      (**(code **)(*(longlong *)plVar14[2] + 0x88))((longlong *)plVar14[2],param_2,&plStack_d8);
    }
    FUN_1805e2dd0(param_1,&plStack_d8);
    lVar3 = param_1 + 0x7d3c0;
    lStack_60 = lVar3;
    if (lVar3 != 0) {
      EnterCriticalSection(lVar3);
    }
    puVar7 = *(undefined8 **)(param_1 + 0x7d3e8);
    cVar12 = *(char *)((longlong)puVar7[1] + 0x19);
    puVar8 = puVar7;
    puVar16 = (undefined8 *)puVar7[1];
    while (cVar12 == '\0') {
      if (*(int *)(puVar16 + 4) < param_2) {
        puVar15 = (undefined8 *)puVar16[2];
        puVar16 = puVar8;
      }
      else {
        puVar15 = (undefined8 *)*puVar16;
      }
      puVar8 = puVar16;
      puVar16 = puVar15;
      cVar12 = *(char *)((longlong)puVar15 + 0x19);
    }
    if ((puVar8 == puVar7) || (param_2 < *(int *)(puVar8 + 4))) {
      apuStack_78[0] = puVar7;
      ppuVar18 = apuStack_78;
    }
    else {
      ppuVar18 = &puStackX_20;
      puStackX_20 = puVar8;
    }
    puVar8 = *ppuVar18;
    if (puVar8 != puVar7) {
      uStack_90 = 0;
      plVar14 = (longlong *)FUN_1805a18b0(puVar16,0,0);
      lVar9 = puVar8[5];
      lVar4 = lVar9 + 0x238;
      plStack_98 = plVar14;
      lStack_50 = lVar4;
      if (lVar4 != 0) {
        EnterCriticalSection(lVar4);
      }
      pplVar5 = (longlong **)(lVar9 + 0x228);
      if (&plStack_98 != pplVar5) {
        plVar14 = *pplVar5;
        FUN_1805a2030(&plStack_98,*plVar14,plVar14);
        plVar14 = plStack_98;
      }
      if (lVar4 != 0) {
        LeaveCriticalSection(lVar4);
      }
      for (plVar10 = (longlong *)*plVar14; plVar10 != plVar14; plVar10 = (longlong *)*plVar10) {
        FUN_180698490(param_1 + 0x7d490,(int)plVar10[2]);
      }
      (**(code **)(*(longlong *)puVar8[5] + 0x48))();
      plVar10 = (longlong *)puVar8[6];
      puVar8[6] = 0;
      puVar8[5] = 0;
      plStack_68 = plVar10;
      if (plVar10 != (longlong *)0x0) {
        LOCK();
        plVar11 = plVar10 + 1;
        *(int *)plVar11 = (int)*plVar11 + -1;
        UNLOCK();
        plVar17 = plStack_a8;
        plVar14 = plStack_98;
        if ((int)*plVar11 == 0) {
          (**(code **)*plVar10)(plVar10);
          LOCK();
          piVar1 = (int *)((longlong)plVar10 + 0xc);
          *piVar1 = *piVar1 + -1;
          UNLOCK();
          plVar17 = plStack_a8;
          plVar14 = plStack_98;
          if (*piVar1 == 0) {
            (**(code **)(*plStack_68 + 8))();
            plVar17 = plStack_a8;
            plVar14 = plStack_98;
          }
        }
      }
      FUN_1805a01a0(param_1 + 0x7d3e8,auStack_48,puVar8);
      plVar10 = (longlong *)*plVar14;
      *plVar14 = (longlong)plVar14;
      plVar14[1] = (longlong)plVar14;
      while (plVar10 != plVar14) {
        plVar10 = (longlong *)*plVar10;
        __3_YAXPEAX_Z();
      }
      __3_YAXPEAX_Z(plVar14);
    }
    if (lVar3 != 0) {
      LeaveCriticalSection(lVar3);
    }
    plVar14 = (longlong *)*plVar17;
    *plVar17 = (longlong)plVar17;
    plVar17[1] = (longlong)plVar17;
    if (plVar14 != plVar17) {
      do {
        plVar10 = (longlong *)*plVar14;
        plVar11 = (longlong *)plVar14[3];
        if (plVar11 != (longlong *)0x0) {
          LOCK();
          plVar2 = plVar11 + 1;
          *(int *)plVar2 = (int)*plVar2 + -1;
          UNLOCK();
          plVar17 = plStack_a8;
          if ((int)*plVar2 == 0) {
            (**(code **)*plVar11)(plVar11);
            LOCK();
            piVar1 = (int *)((longlong)plVar11 + 0xc);
            *piVar1 = *piVar1 + -1;
            UNLOCK();
            plVar17 = plStack_a8;
            if (*piVar1 == 0) {
              (**(code **)(*plVar11 + 8))(plVar11);
              plVar17 = plStack_a8;
            }
          }
        }
        __3_YAXPEAX_Z(plVar14);
        plVar14 = plVar10;
      } while (plVar10 != plVar17);
    }
    __3_YAXPEAX_Z(plVar17);
    plVar14 = (longlong *)*plStack_d8;
    *plStack_d8 = (longlong)plStack_d8;
    plStack_d8[1] = (longlong)plStack_d8;
    uStack_d0 = 0;
    if (plVar14 != plStack_d8) {
      do {
        plVar17 = (longlong *)*plVar14;
        if (0xf < (ulonglong)plVar14[5]) {
          __3_YAXPEAX_Z(plVar14[2]);
        }
        plVar14[5] = 0xf;
        plVar14[4] = 0;
        *(undefined1 *)(plVar14 + 2) = 0;
        __3_YAXPEAX_Z(plVar14);
        plVar14 = plVar17;
      } while (plVar17 != plStack_d8);
    }
    __3_YAXPEAX_Z();
    plVar14 = plStack_80;
    if (plStack_80 != (longlong *)0x0) {
      LOCK();
      plVar17 = plStack_80 + 1;
      *(int *)plVar17 = (int)*plVar17 + -1;
      UNLOCK();
      if ((int)*plVar17 == 0) {
        (**(code **)*plStack_80)(plStack_80);
        LOCK();
        piVar1 = (int *)((longlong)plVar14 + 0xc);
        *piVar1 = *piVar1 + -1;
        UNLOCK();
        if (*piVar1 == 0) {
          (**(code **)(*plStack_80 + 8))();
        }
      }
    }
  }
  return iStackX_10;
}

