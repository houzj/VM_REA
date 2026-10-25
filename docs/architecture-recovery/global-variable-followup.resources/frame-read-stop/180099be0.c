
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_180099be0(undefined8 param_1,int *param_2)

{
  int *piVar1;
  longlong *plVar2;
  int *piVar3;
  undefined8 *puVar4;
  ulonglong uVar5;
  undefined8 *puVar6;
  ulonglong uVar7;
  char cVar8;
  uint uVar9;
  int iVar10;
  undefined8 uVar11;
  longlong lVar12;
  undefined8 **ppuVar13;
  longlong *plVar14;
  undefined1 *puVar15;
  undefined8 *puVar16;
  ulonglong uVar17;
  undefined8 *puVar18;
  undefined1 auStack_108 [32];
  undefined4 uStack_e8;
  int iStack_e0;
  int *piStack_d8;
  undefined4 uStack_c8;
  longlong *plStack_c0;
  longlong *plStack_b8;
  int aiStack_b0 [2];
  int *piStack_a8;
  undefined8 *puStack_a0;
  undefined8 *puStack_98;
  undefined4 uStack_90;
  longlong lStack_88;
  longlong *plStack_80;
  longlong lStack_78;
  undefined8 *puStack_70;
  undefined8 uStack_68;
  undefined1 uStack_60;
  undefined7 uStack_5f;
  ulonglong uStack_50;
  ulonglong uStack_48;
  ulonglong uStack_40;
  
  uStack_68 = 0xfffffffffffffffe;
  uStack_40 = _DAT_1802a5230 ^ (ulonglong)auStack_108;
  uStack_90 = 0;
  uStack_c8 = 0;
  iVar10 = *param_2;
  piStack_a8 = param_2;
  if (((iVar10 == 0) || (iVar10 == 1)) || (iVar10 == 2)) goto LAB_180099fcf;
  if (iVar10 != 3) {
    cVar8 = FUN_18001dbc0(4);
    if (cVar8 != '\0') {
      uVar11 = FUN_18001db50();
      uStack_e8 = 0x57c;
      FUN_180015b40(uVar11,4,&UNK_18021c668,&UNK_180226b90);
    }
    goto LAB_180099fcf;
  }
  piVar1 = param_2 + 2;
  uStack_48 = 0xf;
  uStack_50 = 0;
  uStack_60 = 0;
  if ((char)*piVar1 == '\0') {
    lVar12 = 0;
  }
  else {
    lVar12 = -1;
    do {
      lVar12 = lVar12 + 1;
    } while (*(char *)((longlong)piVar1 + lVar12) != '\0');
  }
  FUN_180006300(&uStack_60,piVar1,lVar12);
  iVar10 = param_2[1];
  lVar12 = FUN_18008e6c0(0);
  aiStack_b0[0] = iVar10;
  FUN_18009ab20(lVar12 + 8,&lStack_78,aiStack_b0);
  if (lStack_78 == *(longlong *)(lVar12 + 8)) {
    iVar10 = -0x1ffffcec;
  }
  else {
    FUN_1800cae00(lVar12,&plStack_c0,iVar10);
    uVar7 = uStack_50;
    plVar14 = plStack_b8;
    if (plStack_c0 == (longlong *)0x0) {
      if (plStack_b8 != (longlong *)0x0) {
        LOCK();
        plVar2 = plStack_b8 + 1;
        *(int *)plVar2 = (int)*plVar2 + -1;
        UNLOCK();
        if ((int)*plVar2 == 0) {
          (**(code **)*plStack_b8)(plStack_b8);
          LOCK();
          piVar3 = (int *)((longlong)plVar14 + 0xc);
          *piVar3 = *piVar3 + -1;
          UNLOCK();
          if (*piVar3 == 0) {
            (**(code **)(*plStack_b8 + 8))();
          }
        }
      }
      iVar10 = -0x1ffffffd;
    }
    else {
      puVar4 = (undefined8 *)*plStack_c0;
      cVar8 = *(char *)((longlong)puVar4[1] + 0x19);
      puVar6 = puVar4;
      puVar18 = (undefined8 *)puVar4[1];
      while (cVar8 == '\0') {
        plVar14 = puVar18 + 4;
        puVar15 = &uStack_60;
        if (0xf < uStack_48) {
          puVar15 = (undefined1 *)CONCAT71(uStack_5f,uStack_60);
        }
        uVar5 = puVar18[6];
        if (0xf < (ulonglong)puVar18[7]) {
          plVar14 = (longlong *)*plVar14;
        }
        uVar17 = uVar7;
        if (uVar5 < uVar7) {
          uVar17 = uVar5;
        }
        if (uVar17 == 0) {
          uVar9 = 0;
        }
        else {
          uVar9 = func_0x0001801e92c0(plVar14,puVar15);
        }
        if (uVar9 == 0) {
          if (uVar5 < uVar7) {
            uVar9 = 0xffffffff;
          }
          else {
            uVar9 = (uint)(uVar5 != uVar7);
          }
        }
        if ((int)uVar9 < 0) {
          puVar16 = (undefined8 *)puVar18[2];
          puVar18 = puVar6;
        }
        else {
          puVar16 = (undefined8 *)*puVar18;
        }
        puVar6 = puVar18;
        puVar18 = puVar16;
        cVar8 = *(char *)((longlong)puVar16 + 0x19);
      }
      if (puVar6 == puVar4) {
LAB_180099e2f:
        puStack_98 = puVar4;
        ppuVar13 = &puStack_98;
      }
      else {
        plVar14 = puVar6 + 4;
        uVar5 = puVar6[6];
        if (0xf < (ulonglong)puVar6[7]) {
          plVar14 = (longlong *)*plVar14;
        }
        puVar15 = &uStack_60;
        if (0xf < uStack_48) {
          puVar15 = (undefined1 *)CONCAT71(uStack_5f,uStack_60);
        }
        uVar17 = uVar5;
        if (uVar7 < uVar5) {
          uVar17 = uVar7;
        }
        if (uVar17 == 0) {
          uVar9 = 0;
        }
        else {
          uVar9 = func_0x0001801e92c0(puVar15,plVar14);
        }
        if (uVar9 == 0) {
          if (uVar7 < uVar5) {
            uVar9 = 0xffffffff;
          }
          else {
            uVar9 = (uint)(uVar7 != uVar5);
          }
        }
        if ((int)uVar9 < 0) goto LAB_180099e2f;
        puStack_a0 = puVar6;
        ppuVar13 = &puStack_a0;
      }
      puStack_70 = *ppuVar13;
      if (puStack_70 == puVar4) {
        iVar10 = -0x1ffffcec;
      }
      else {
        FUN_1800f74b0(plStack_c0,&lStack_88,&uStack_60);
        if (lStack_88 == 0) {
          if (plStack_80 != (longlong *)0x0) {
            LOCK();
            plVar14 = plStack_80 + 1;
            *(int *)plVar14 = (int)*plVar14 + -1;
            UNLOCK();
            if ((int)*plVar14 == 0) {
              (**(code **)*plStack_80)(plStack_80);
              LOCK();
              piVar3 = (int *)((longlong)plStack_80 + 0xc);
              *piVar3 = *piVar3 + -1;
              UNLOCK();
              if (*piVar3 == 0) {
                (**(code **)(*plStack_80 + 8))(plStack_80);
              }
            }
          }
          iVar10 = -0x1ffffffd;
        }
        else {
          lVar12 = *(longlong *)(lStack_88 + 0x10);
          if (lVar12 == 0) {
            iVar10 = -0x1ffffcec;
          }
          else if (*(int *)(lVar12 + 0x68) < 1) {
            iVar10 = -0x1fffffec;
            if (*(int *)(lVar12 + 0x68) == 0) {
              iVar10 = -0x1ffffcec;
            }
          }
          else {
            uStack_c8 = *(undefined4 *)(*(longlong *)(lVar12 + 0x70) + 8);
            iVar10 = 0;
          }
          if (plStack_80 != (longlong *)0x0) {
            LOCK();
            plVar14 = plStack_80 + 1;
            *(int *)plVar14 = (int)*plVar14 + -1;
            UNLOCK();
            if ((int)*plVar14 == 0) {
              (**(code **)*plStack_80)(plStack_80);
              LOCK();
              piVar3 = (int *)((longlong)plStack_80 + 0xc);
              *piVar3 = *piVar3 + -1;
              UNLOCK();
              if (*piVar3 == 0) {
                (**(code **)(*plStack_80 + 8))(plStack_80);
              }
            }
          }
          plVar14 = plStack_b8;
          if (iVar10 == 0) {
            if (plStack_b8 != (longlong *)0x0) {
              LOCK();
              plVar2 = plStack_b8 + 1;
              *(int *)plVar2 = (int)*plVar2 + -1;
              UNLOCK();
              if ((int)*plVar2 == 0) {
                (**(code **)*plStack_b8)(plStack_b8);
                LOCK();
                piVar3 = (int *)((longlong)plVar14 + 0xc);
                *piVar3 = *piVar3 + -1;
                UNLOCK();
                if (*piVar3 == 0) {
                  (**(code **)(*plStack_b8 + 8))();
                }
              }
            }
            iVar10 = 0;
            param_2 = piStack_a8;
            goto LAB_180099f50;
          }
        }
      }
      plVar14 = plStack_b8;
      param_2 = piStack_a8;
      if (plStack_b8 != (longlong *)0x0) {
        LOCK();
        plVar2 = plStack_b8 + 1;
        *(int *)plVar2 = (int)*plVar2 + -1;
        UNLOCK();
        if ((int)*plVar2 == 0) {
          (**(code **)*plStack_b8)(plStack_b8);
          LOCK();
          piVar3 = (int *)((longlong)plVar14 + 0xc);
          *piVar3 = *piVar3 + -1;
          UNLOCK();
          param_2 = piStack_a8;
          if (*piVar3 == 0) {
            (**(code **)(*plStack_b8 + 8))();
            param_2 = piStack_a8;
          }
        }
      }
    }
  }
LAB_180099f50:
  if (0xf < uStack_48) {
    __3_YAXPEAX_Z(CONCAT71(uStack_5f,uStack_60));
  }
  uStack_48 = 0xf;
  uStack_50 = 0;
  uStack_60 = 0;
  if ((iVar10 != 0) && (cVar8 = FUN_18001dbc0(4), cVar8 != '\0')) {
    uVar11 = FUN_18001db50();
    iStack_e0 = param_2[1];
    uStack_e8 = 0x574;
    piStack_d8 = piVar1;
    FUN_180015b40(uVar11,4,&UNK_180224a28,&UNK_180226b90);
  }
LAB_180099fcf:
  FUN_1801e8810(uStack_40 ^ (ulonglong)auStack_108);
  return;
}

