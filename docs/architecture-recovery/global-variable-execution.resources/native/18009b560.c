
/* WARNING: Type propagation algorithm not settling */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

ulonglong FUN_18009b560(longlong param_1,undefined4 param_2,undefined8 param_3,undefined4 param_4,
                       int *param_5)

{
  code *pcVar1;
  int iVar2;
  int iVar3;
  int *piVar4;
  int *piVar5;
  char cVar6;
  int iVar7;
  uint uVar8;
  ulonglong uVar9;
  longlong *plVar10;
  longlong lVar11;
  longlong *plVar12;
  undefined8 *puVar13;
  undefined8 uVar14;
  undefined4 *puVar15;
  longlong lVar16;
  int *piVar17;
  int *piVar18;
  int *piVar19;
  longlong *plVar20;
  undefined4 extraout_XMM0_Da;
  undefined4 auStackX_10 [2];
  undefined8 in_stack_ffffffffffffff38;
  undefined4 uVar21;
  longlong *plStack_b8;
  longlong *plStack_b0;
  int *piStack_a8;
  longlong *plStack_a0;
  longlong lStack_98;
  longlong lStack_90;
  longlong *plStack_88;
  undefined8 uStack_80;
  longlong *plStack_78;
  undefined4 uStack_70;
  undefined8 uStack_68;
  longlong *plStack_60;
  longlong lStack_58;
  longlong alStack_50 [3];
  undefined8 *puStack_38;
  undefined8 uStack_30;
  
  uVar21 = (undefined4)((ulonglong)in_stack_ffffffffffffff38 >> 0x20);
  uStack_30 = 0xfffffffffffffffe;
  auStackX_10[0] = param_2;
  if ((_DAT_1802b1888 & 1) == 0) {
    _DAT_1802b1888 = _DAT_1802b1888 | 1;
    FUN_180107fc0();
    FUN_1801e8930(&UNK_180202980);
  }
  FUN_18010a690(&DAT_1802b1890,&lStack_90,auStackX_10[0]);
  plVar10 = plStack_88;
  if (lStack_90 == 0) {
    if (plStack_88 != (longlong *)0x0) {
      LOCK();
      plVar12 = plStack_88 + 1;
      *(int *)plVar12 = (int)*plVar12 + -1;
      UNLOCK();
      if ((int)*plVar12 == 0) {
        (**(code **)*plStack_88)(plStack_88);
        LOCK();
        piVar18 = (int *)((longlong)plVar10 + 0xc);
        *piVar18 = *piVar18 + -1;
        UNLOCK();
        if (*piVar18 == 0) {
          (**(code **)(*plStack_88 + 8))();
        }
      }
    }
    uVar9 = 0xe0000302;
  }
  else {
    if (plStack_88 != (longlong *)0x0) {
      LOCK();
      plVar12 = plStack_88 + 1;
      *(int *)plVar12 = (int)*plVar12 + -1;
      UNLOCK();
      if ((int)*plVar12 == 0) {
        (**(code **)*plStack_88)(plStack_88);
        LOCK();
        piVar18 = (int *)((longlong)plVar10 + 0xc);
        *piVar18 = *piVar18 + -1;
        UNLOCK();
        if (*piVar18 == 0) {
          (**(code **)(*plStack_88 + 8))();
        }
      }
    }
    lStack_98 = param_1;
    iVar7 = _Mtx_lock(param_1);
    if (iVar7 != 0) {
      __Throw_C_error_std__YAXH_Z(iVar7);
    }
    plVar10 = (longlong *)FUN_18007d980(param_1 + 8,auStackX_10);
    plStack_b8 = (longlong *)0x0;
    plStack_b0 = (longlong *)0x0;
    lVar11 = FUN_1800202d0(plVar10,param_3);
    if ((lVar11 == *plVar10) || (iVar7 = FUN_180020a60(param_3,lVar11 + 0x20), iVar7 < 0)) {
      lStack_58 = *plVar10;
      plVar12 = &lStack_58;
    }
    else {
      alStack_50[0] = lVar11;
      plVar12 = alStack_50;
    }
    lVar11 = *plVar12;
    if (lVar11 == *plVar10) {
      puVar13 = (undefined8 *)FUN_1801e8cd0(0x10,0x1802b0a11);
      puStack_38 = puVar13;
      if (puVar13 == (undefined8 *)0x0) {
        puVar13 = (undefined8 *)0x0;
      }
      else {
        *puVar13 = 0;
        puVar13[1] = 0;
        uVar14 = FUN_18001f150(extraout_XMM0_Da,0,0);
        *puVar13 = uVar14;
      }
      uStack_80 = 0;
      plStack_78 = (longlong *)0x0;
      FUN_18009f200(&uStack_80,puVar13);
      FUN_18001d870(&plStack_b8,&uStack_80);
      plVar12 = plStack_78;
      if (plStack_78 != (longlong *)0x0) {
        LOCK();
        plVar20 = plStack_78 + 1;
        lVar11 = *plVar20;
        *(int *)plVar20 = (int)*plVar20 + -1;
        UNLOCK();
        if ((int)lVar11 == 1) {
          (**(code **)*plStack_78)(plStack_78);
          LOCK();
          piVar18 = (int *)((longlong)plVar12 + 0xc);
          iVar7 = *piVar18;
          *piVar18 = *piVar18 + -1;
          UNLOCK();
          if (iVar7 == 1) {
            (**(code **)(*plStack_78 + 8))();
          }
        }
      }
      plVar12 = plStack_b0;
      plVar20 = plStack_b8;
      if (plStack_b8 == (longlong *)0x0) {
        cVar6 = FUN_18001dbc0(4);
        if (cVar6 != '\0') {
          uVar14 = FUN_18001db50();
          FUN_180015b40(uVar14,4,&UNK_18021f998,&UNK_180226dc0,CONCAT44(uVar21,0x27));
        }
        plVar10 = plStack_b0;
        if (plStack_b0 != (longlong *)0x0) {
          LOCK();
          plVar12 = plStack_b0 + 1;
          lVar11 = *plVar12;
          *(int *)plVar12 = (int)*plVar12 + -1;
          UNLOCK();
          if ((int)lVar11 == 1) {
            (**(code **)*plStack_b0)(plStack_b0);
            LOCK();
            piVar18 = (int *)((longlong)plVar10 + 0xc);
            iVar7 = *piVar18;
            *piVar18 = *piVar18 + -1;
            UNLOCK();
            if (iVar7 == 1) {
              (**(code **)(*plStack_b0 + 8))();
            }
          }
        }
        iVar7 = _Mtx_unlock(param_1);
        if (iVar7 != 0) {
          __Throw_C_error_std__YAXH_Z(iVar7);
        }
        return 0xe0000003;
      }
      uStack_68 = 0;
      plStack_60 = (longlong *)0x0;
      if (plStack_b0 != (longlong *)0x0) {
        LOCK();
        *(int *)(plStack_b0 + 1) = (int)plStack_b0[1] + 1;
        UNLOCK();
      }
      uStack_70 = param_4;
      FUN_180015c40(&uStack_68,plStack_b8,plStack_b0);
      puVar15 = (undefined4 *)FUN_18007db20(plVar10,param_3);
      *puVar15 = uStack_70;
      FUN_18001d870(puVar15 + 2,&uStack_68);
      plVar10 = plStack_60;
      if (plStack_60 != (longlong *)0x0) {
        LOCK();
        plVar12 = plStack_60 + 1;
        lVar11 = *plVar12;
        *(int *)plVar12 = (int)*plVar12 + -1;
        UNLOCK();
        plVar12 = plStack_b0;
        plVar20 = plStack_b8;
        if ((int)lVar11 == 1) {
          (**(code **)*plStack_60)(plStack_60);
          LOCK();
          piVar18 = (int *)((longlong)plVar10 + 0xc);
          iVar7 = *piVar18;
          *piVar18 = *piVar18 + -1;
          UNLOCK();
          plVar12 = plStack_b0;
          plVar20 = plStack_b8;
          if (iVar7 == 1) {
            (**(code **)(*plStack_60 + 8))();
            plVar12 = plStack_b0;
            plVar20 = plStack_b8;
          }
        }
      }
    }
    else {
      plStack_b0 = *(longlong **)(lVar11 + 0x50);
      plStack_b8 = *(longlong **)(lVar11 + 0x48);
      plVar12 = plStack_b0;
      plVar20 = plStack_b8;
      if (plStack_b0 != (longlong *)0x0) {
        LOCK();
        *(int *)(plStack_b0 + 1) = (int)plStack_b0[1] + 1;
        UNLOCK();
      }
    }
    for (puVar13 = *(undefined8 **)*plVar20; puVar13 != (undefined8 *)*plVar20;
        puVar13 = (undefined8 *)*puVar13) {
      piVar18 = (int *)puVar13[2];
      if (*piVar18 == *param_5) {
        if (*piVar18 != 3) {
LAB_18009b940:
          cVar6 = FUN_18001dbc0(2);
          if (cVar6 != '\0') {
            uVar14 = FUN_18001db50();
            FUN_180015b40(uVar14,2,&UNK_180226e10,&UNK_180226dc0,CONCAT44(uVar21,0x36));
          }
          plVar10 = plStack_b0;
          if (plVar12 != (longlong *)0x0) {
            LOCK();
            plVar12 = plVar12 + 1;
            lVar11 = *plVar12;
            *(int *)plVar12 = (int)*plVar12 + -1;
            UNLOCK();
            if ((int)lVar11 == 1) {
              (**(code **)*plStack_b0)(plStack_b0);
              LOCK();
              piVar18 = (int *)((longlong)plVar10 + 0xc);
              iVar7 = *piVar18;
              *piVar18 = *piVar18 + -1;
              UNLOCK();
              if (iVar7 == 1) {
                (**(code **)(*plStack_b0 + 8))();
              }
            }
          }
          iVar7 = _Mtx_unlock(lStack_98);
          if (iVar7 != 0) {
            __Throw_C_error_std__YAXH_Z(iVar7);
          }
          return 0xe0000337;
        }
        if (piVar18[0x23] == param_5[0x23]) {
          piVar18 = piVar18 + 0x24;
          lVar11 = 0x90 - (longlong)piVar18;
          do {
            iVar7 = *piVar18;
            cVar6 = *(char *)((longlong)piVar18 + (longlong)param_5 + lVar11);
            if ((char)iVar7 != cVar6) break;
            piVar18 = (int *)((longlong)piVar18 + 1);
          } while (cVar6 != '\0');
          if ((char)iVar7 == cVar6) goto LAB_18009b940;
        }
      }
    }
    uVar14 = FUN_1801e8cd0(0x110,0x1802b0a11);
    piStack_a8 = (int *)0x0;
    plStack_a0 = (longlong *)0x0;
    FUN_18007e960(&piStack_a8,uVar14);
    piVar18 = piStack_a8;
    if (piStack_a8 == (int *)0x0) {
      cVar6 = FUN_18001dbc0(4);
      if (cVar6 != '\0') {
        uVar14 = FUN_18001db50();
        FUN_180015b40(uVar14,4,&UNK_18021f998,&UNK_180226dc0,CONCAT44(uVar21,0x43));
      }
      plVar10 = plStack_a0;
      if (plStack_a0 != (longlong *)0x0) {
        LOCK();
        plVar12 = plStack_a0 + 1;
        lVar11 = *plVar12;
        *(int *)plVar12 = (int)*plVar12 + -1;
        UNLOCK();
        plVar12 = plStack_b0;
        if ((int)lVar11 == 1) {
          (**(code **)*plStack_a0)(plStack_a0);
          LOCK();
          piVar18 = (int *)((longlong)plVar10 + 0xc);
          iVar7 = *piVar18;
          *piVar18 = *piVar18 + -1;
          UNLOCK();
          plVar12 = plStack_b0;
          if (iVar7 == 1) {
            (**(code **)(*plStack_a0 + 8))();
            plVar12 = plStack_b0;
          }
        }
      }
      plVar10 = plStack_b0;
      if (plVar12 != (longlong *)0x0) {
        LOCK();
        plVar12 = plVar12 + 1;
        lVar11 = *plVar12;
        *(int *)plVar12 = (int)*plVar12 + -1;
        UNLOCK();
        if ((int)lVar11 == 1) {
          (**(code **)*plStack_b0)(plStack_b0);
          LOCK();
          piVar18 = (int *)((longlong)plVar10 + 0xc);
          iVar7 = *piVar18;
          *piVar18 = *piVar18 + -1;
          UNLOCK();
          if (iVar7 == 1) {
            (**(code **)(*plStack_b0 + 8))();
          }
        }
      }
      iVar7 = _Mtx_unlock(lStack_98);
      if (iVar7 != 0) {
        __Throw_C_error_std__YAXH_Z(iVar7);
      }
      uVar9 = 0xe0000003;
    }
    else {
      *piStack_a8 = *param_5;
      lVar11 = 2;
      piVar4 = piStack_a8 + 1;
      piVar5 = param_5 + 1;
      do {
        piVar19 = piVar5;
        piVar17 = piVar4;
        uVar14 = *(undefined8 *)(piVar19 + 2);
        *(undefined8 *)piVar17 = *(undefined8 *)piVar19;
        *(undefined8 *)(piVar17 + 2) = uVar14;
        uVar14 = *(undefined8 *)(piVar19 + 6);
        *(undefined8 *)(piVar17 + 4) = *(undefined8 *)(piVar19 + 4);
        *(undefined8 *)(piVar17 + 6) = uVar14;
        uVar14 = *(undefined8 *)(piVar19 + 10);
        *(undefined8 *)(piVar17 + 8) = *(undefined8 *)(piVar19 + 8);
        *(undefined8 *)(piVar17 + 10) = uVar14;
        uVar14 = *(undefined8 *)(piVar19 + 0xe);
        *(undefined8 *)(piVar17 + 0xc) = *(undefined8 *)(piVar19 + 0xc);
        *(undefined8 *)(piVar17 + 0xe) = uVar14;
        uVar14 = *(undefined8 *)(piVar19 + 0x12);
        *(undefined8 *)(piVar17 + 0x10) = *(undefined8 *)(piVar19 + 0x10);
        *(undefined8 *)(piVar17 + 0x12) = uVar14;
        uVar14 = *(undefined8 *)(piVar19 + 0x16);
        *(undefined8 *)(piVar17 + 0x14) = *(undefined8 *)(piVar19 + 0x14);
        *(undefined8 *)(piVar17 + 0x16) = uVar14;
        iVar7 = piVar19[0x19];
        iVar2 = piVar19[0x1a];
        iVar3 = piVar19[0x1b];
        piVar17[0x18] = piVar19[0x18];
        piVar17[0x19] = iVar7;
        piVar17[0x1a] = iVar2;
        piVar17[0x1b] = iVar3;
        piVar4 = piVar17 + 0x20;
        iVar7 = piVar19[0x1d];
        iVar2 = piVar19[0x1e];
        iVar3 = piVar19[0x1f];
        piVar17[0x1c] = piVar19[0x1c];
        piVar17[0x1d] = iVar7;
        piVar17[0x1e] = iVar2;
        piVar17[0x1f] = iVar3;
        lVar11 = lVar11 + -1;
        piVar5 = piVar19 + 0x20;
      } while (lVar11 != 0);
      *(undefined8 *)piVar4 = *(undefined8 *)(piVar19 + 0x20);
      piVar17[0x22] = piVar19[0x22];
      lVar11 = *plVar20;
      lVar16 = FUN_18001f260(piVar4,lVar11,*(undefined8 *)(lVar11 + 8),&piStack_a8);
      plVar10 = plStack_a0;
      if (plVar20[1] == 0x7fffffffffffffe) {
        __Xlength_error_std__YAXPEBD_Z(&UNK_18021d318);
        pcVar1 = (code *)swi(3);
        uVar9 = (*pcVar1)();
        return uVar9;
      }
      plVar20[1] = plVar20[1] + 1;
      *(longlong *)(lVar11 + 8) = lVar16;
      **(longlong **)(lVar16 + 8) = lVar16;
      alStack_50[1] = 0;
      alStack_50[2] = 0;
      if (plStack_a0 != (longlong *)0x0) {
        LOCK();
        *(int *)(plStack_a0 + 1) = (int)plStack_a0[1] + 1;
        UNLOCK();
        plVar12 = plStack_b0;
        piVar18 = piStack_a8;
      }
      FUN_180015c40(alStack_50 + 1,piVar18,plStack_a0);
      uVar8 = FUN_18009d8f0(param_1,auStackX_10[0],param_3,param_4,alStack_50 + 1);
      plVar20 = plStack_a0;
      uVar9 = (ulonglong)uVar8;
      if (plVar10 != (longlong *)0x0) {
        LOCK();
        plVar10 = plVar10 + 1;
        lVar11 = *plVar10;
        *(int *)plVar10 = (int)*plVar10 + -1;
        UNLOCK();
        plVar12 = plStack_b0;
        if ((int)lVar11 == 1) {
          (**(code **)*plStack_a0)(plStack_a0);
          LOCK();
          piVar18 = (int *)((longlong)plVar20 + 0xc);
          iVar7 = *piVar18;
          *piVar18 = *piVar18 + -1;
          UNLOCK();
          plVar12 = plStack_b0;
          if (iVar7 == 1) {
            (**(code **)(*plStack_a0 + 8))();
            plVar12 = plStack_b0;
          }
        }
      }
      plVar10 = plStack_b0;
      if (plVar12 != (longlong *)0x0) {
        LOCK();
        plVar12 = plVar12 + 1;
        lVar11 = *plVar12;
        *(int *)plVar12 = (int)*plVar12 + -1;
        UNLOCK();
        if ((int)lVar11 == 1) {
          (**(code **)*plStack_b0)(plStack_b0);
          LOCK();
          piVar18 = (int *)((longlong)plVar10 + 0xc);
          iVar7 = *piVar18;
          *piVar18 = *piVar18 + -1;
          UNLOCK();
          if (iVar7 == 1) {
            (**(code **)(*plStack_b0 + 8))();
          }
        }
      }
      iVar7 = _Mtx_unlock(lStack_98);
      if (iVar7 != 0) {
        __Throw_C_error_std__YAXH_Z(iVar7);
      }
    }
  }
  return uVar9;
}

