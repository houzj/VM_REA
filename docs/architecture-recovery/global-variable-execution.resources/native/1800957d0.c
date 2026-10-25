
/* WARNING: Removing unreachable block (ram,0x000180095f0a) */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_1800957d0(longlong param_1,undefined4 param_2,undefined8 *param_3,longlong *param_4,
                  char param_5)

{
  int *piVar1;
  undefined8 *puVar2;
  char cVar3;
  int iVar4;
  longlong lVar5;
  longlong *plVar6;
  longlong *plVar7;
  undefined8 uVar8;
  longlong *plVar9;
  longlong *plVar10;
  int iVar11;
  longlong *plVar12;
  longlong *plVar13;
  undefined4 extraout_XMM0_Da;
  undefined4 extraout_XMM0_Da_00;
  undefined4 uVar14;
  undefined1 auStack_118 [32];
  longlong *plStack_f8;
  longlong *plStack_f0;
  undefined8 *puStack_e8;
  int *piStack_e0;
  longlong *plStack_d8;
  int iStack_d0;
  undefined4 auStack_c8 [2];
  int iStack_c0;
  int iStack_bc;
  int iStack_b8;
  int iStack_b4;
  longlong *plStack_b0;
  longlong *plStack_a8;
  longlong *plStack_a0;
  undefined8 uStack_98;
  undefined8 uStack_90;
  undefined8 auStack_88 [3];
  ulonglong uStack_70;
  undefined8 auStack_68 [3];
  ulonglong uStack_50;
  ulonglong uStack_48;
  
  uStack_90 = 0xfffffffffffffffe;
  uStack_48 = _DAT_1802a5230 ^ (ulonglong)auStack_118;
  plVar13 = (longlong *)0x0;
  plVar12 = (longlong *)0x0;
  iVar4 = 0x400;
  iStack_b4 = 0x400;
  plVar10 = (longlong *)0x0;
  plStack_b0 = (longlong *)0x0;
  iStack_b8 = 0;
  iStack_bc = 0;
  iStack_c0 = 0;
  uStack_98 = 0;
  plStack_a0 = (longlong *)0x0;
  plVar7 = plVar13;
  plVar9 = plVar13;
  plVar6 = plVar13;
  auStack_c8[0] = param_2;
  plStack_a8 = param_4;
  if (param_5 != '\0') {
    lVar5 = FUN_18009a990(param_1 + 0x18,auStack_c8);
    plVar6 = (longlong *)FUN_180007510(*(undefined8 *)(lVar5 + 0x18));
    plStack_a0 = plVar6;
    FUN_18003fc70(plVar6,&UNK_1802260f8);
    *(uint *)(plVar6 + 2) = *(uint *)(plVar6 + 2) | 1;
    puVar2 = (undefined8 *)plVar6[3];
    plVar7 = plVar10;
    plVar9 = plVar12;
    if (puVar2 == _DAT_1802b05e8) {
      FUN_180001ac0(plVar6 + 3,param_3);
    }
    else if (puVar2 != param_3) {
      FUN_180006470(puVar2,param_3,0,0xffffffffffffffff);
    }
  }
  do {
    if ((plVar7 == (longlong *)0x0) &&
       (plVar7 = (longlong *)func_0x0001801e8c60((longlong)iVar4,0x1802b0a11), plStack_b0 = plVar7,
       plVar7 == (longlong *)0x0)) {
      cVar3 = FUN_18001dbc0(4);
      if (cVar3 != '\0') {
        uVar8 = FUN_18001db50();
        plStack_f8 = (longlong *)CONCAT44(plStack_f8._4_4_,0x18d);
        FUN_180015b40(uVar8,4,&UNK_18021f998,&UNK_1802260c8);
      }
      iStack_c0 = -0x1ffffffd;
      goto LAB_180095f05;
    }
    plStack_d8 = (longlong *)&iStack_bc;
    piStack_e0 = &iStack_b8;
    puStack_e8 = (undefined8 *)CONCAT44(puStack_e8._4_4_,iVar4);
    iVar11 = (int)plVar9;
    plStack_f8 = param_4;
    plStack_f0 = plVar7;
    iVar4 = FUN_180097920();
    uVar14 = extraout_XMM0_Da;
    if (iVar4 == -0x1fffffee) {
      iStack_c0 = iVar4;
      if (plVar7 != (longlong *)0x0) {
        ___V_YAXPEAX_Z(plVar7);
      }
      iVar4 = iStack_b8;
      iStack_b4 = iStack_b8;
      plVar7 = (longlong *)func_0x0001801e8c60((longlong)iStack_b8,0x1802b0a11);
      plStack_b0 = plVar7;
      if (plVar7 == (longlong *)0x0) {
        cVar3 = FUN_18001dbc0(4);
        if (cVar3 != '\0') {
          uVar8 = FUN_18001db50();
          plStack_f8 = (longlong *)CONCAT44(plStack_f8._4_4_,0x1a2);
          FUN_180015b40(uVar8,4,&UNK_18021f998,&UNK_1802260c8);
        }
        iStack_c0 = -0x1ffffffd;
        goto LAB_180095f05;
      }
      plStack_d8 = (longlong *)&iStack_bc;
      piStack_e0 = &iStack_b8;
      puStack_e8 = (undefined8 *)CONCAT44(puStack_e8._4_4_,iVar4);
      plStack_f8 = plStack_a8;
      plStack_f0 = plVar7;
      iVar4 = FUN_180097920();
      uVar14 = extraout_XMM0_Da_00;
    }
    iStack_c0 = iVar4;
    if (iVar4 != 0) {
      cVar3 = FUN_18001dbc0(4);
      if (cVar3 != '\0') {
        plVar9 = (longlong *)FUN_18008f750(auStack_88,iVar4);
        if (0xf < (ulonglong)plVar9[3]) {
          plVar9 = (longlong *)*plVar9;
        }
        if (0xf < (ulonglong)param_3[3]) {
          param_3 = (undefined8 *)*param_3;
        }
        uVar8 = FUN_18001db50();
        piStack_e0 = (int *)CONCAT44(piStack_e0._4_4_,iVar11);
        plStack_f0 = (longlong *)CONCAT44(plStack_f0._4_4_,auStack_c8[0]);
        plStack_f8 = (longlong *)CONCAT44(plStack_f8._4_4_,0x1ac);
        puStack_e8 = param_3;
        plStack_d8 = plVar9;
        FUN_180015b40(uVar8,4,&UNK_180226140,&UNK_1802260c8);
        if (0xf < uStack_70) {
          __3_YAXPEAX_Z(auStack_88[0]);
        }
      }
      break;
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
      uVar14 = FUN_1801e8930(&UNK_180202850);
    }
    plVar10 = _DAT_1802b1240;
    lVar5 = _DAT_1802b1238;
    if (_DAT_1802b1240 != (longlong *)0x0) {
      LOCK();
      *(int *)(_DAT_1802b1240 + 1) = (int)_DAT_1802b1240[1] + 1;
      UNLOCK();
    }
    if (lVar5 == 0) {
      if (plVar10 != (longlong *)0x0) {
        LOCK();
        plVar12 = plVar10 + 1;
        *(int *)plVar12 = (int)*plVar12 + -1;
        UNLOCK();
        if ((int)*plVar12 == 0) {
          uVar14 = (**(code **)*plVar10)(plVar10);
          LOCK();
          piVar1 = (int *)((longlong)plVar10 + 0xc);
          *piVar1 = *piVar1 + -1;
          UNLOCK();
          if (*piVar1 == 0) {
            uVar14 = (**(code **)(*plVar10 + 8))(plVar10);
          }
        }
      }
    }
    else {
      iVar4 = *(int *)(lVar5 + 0x40);
      if (plVar10 != (longlong *)0x0) {
        LOCK();
        plVar12 = plVar10 + 1;
        *(int *)plVar12 = (int)*plVar12 + -1;
        UNLOCK();
        if ((int)*plVar12 == 0) {
          uVar14 = (**(code **)*plVar10)(plVar10);
          LOCK();
          piVar1 = (int *)((longlong)plVar10 + 0xc);
          *piVar1 = *piVar1 + -1;
          UNLOCK();
          if (*piVar1 == 0) {
            uVar14 = (**(code **)(*plVar10 + 8))(plVar10);
          }
        }
      }
      if (iVar4 < 1) {
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
        puStack_e8 = (undefined8 *)func_0x000180006450(param_3);
        iStack_d0 = iStack_bc;
        piStack_e0 = (int *)CONCAT44(piStack_e0._4_4_,iVar11);
        plStack_f0 = (longlong *)CONCAT44(plStack_f0._4_4_,auStack_c8[0]);
        plStack_f8 = (longlong *)CONCAT44(plStack_f8._4_4_,0x1b0);
        plStack_d8 = plVar7;
        uVar14 = FUN_180015b40(&DAT_1802b1230,0,&UNK_180226100,&UNK_1802260c8);
      }
    }
    if (param_5 != '\0') {
      piVar1 = (int *)plVar6[7];
      if (piVar1 == (int *)0x0) {
LAB_180095c4c:
        func_0x0001801a3b90(plVar6 + 5,*(int *)((longlong)plVar6 + 0x34) + 1);
LAB_180095c5b:
        *(int *)plVar6[7] = *(int *)plVar6[7] + 1;
        uVar8 = FUN_18000ddc0(plVar6[5]);
        *(undefined8 *)(plVar6[7] + 8 + (longlong)(int)plVar6[6] * 8) = uVar8;
        *(int *)(plVar6 + 6) = (int)plVar6[6] + 1;
      }
      else {
        iVar4 = (int)plVar6[6];
        if (*piVar1 <= iVar4) {
          if ((piVar1 == (int *)0x0) || (*piVar1 == *(int *)((longlong)plVar6 + 0x34)))
          goto LAB_180095c4c;
          goto LAB_180095c5b;
        }
        uVar8 = *(undefined8 *)(piVar1 + (longlong)iVar4 * 2 + 2);
        *(int *)(plVar6 + 6) = iVar4 + 1;
      }
      plVar6 = plVar13;
      if ((char)*plVar7 != '\0') {
        plVar6 = (longlong *)0xffffffffffffffff;
        do {
          plVar6 = (longlong *)((longlong)plVar6 + 1);
        } while (*(char *)((longlong)plVar7 + (longlong)plVar6) != '\0');
      }
      uVar14 = FUN_180006300(uVar8,plVar7,plVar6);
    }
    plStack_f8 = plVar7;
    iVar4 = FUN_180098360(uVar14,auStack_c8[0],param_3,plVar9);
    iStack_c0 = iVar4;
    if (iVar4 != 0) {
      cVar3 = FUN_18001dbc0(4);
      if (cVar3 != '\0') {
        plVar9 = (longlong *)FUN_18008f750(auStack_68,iVar4);
        if (0xf < (ulonglong)plVar9[3]) {
          plVar9 = (longlong *)*plVar9;
        }
        if (0xf < (ulonglong)param_3[3]) {
          param_3 = (undefined8 *)*param_3;
        }
        uVar8 = FUN_18001db50();
        piStack_e0 = (int *)CONCAT44(piStack_e0._4_4_,iVar11);
        plStack_f0 = (longlong *)CONCAT44(plStack_f0._4_4_,auStack_c8[0]);
        plStack_f8 = (longlong *)CONCAT44(plStack_f8._4_4_,0x1e2);
        puStack_e8 = param_3;
        plStack_d8 = plVar9;
        FUN_180015b40(uVar8,4,&UNK_1802261b0,&UNK_1802260c8);
        if (0xf < uStack_50) {
          __3_YAXPEAX_Z(auStack_68[0]);
        }
      }
      break;
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
    plVar9 = _DAT_1802b1240;
    lVar5 = _DAT_1802b1238;
    if (_DAT_1802b1240 != (longlong *)0x0) {
      LOCK();
      *(int *)(_DAT_1802b1240 + 1) = (int)_DAT_1802b1240[1] + 1;
      UNLOCK();
    }
    if (lVar5 == 0) {
      if (plVar9 != (longlong *)0x0) {
        LOCK();
        plVar6 = plVar9 + 1;
        *(int *)plVar6 = (int)*plVar6 + -1;
        UNLOCK();
        if ((int)*plVar6 == 0) {
          (**(code **)*plVar9)(plVar9);
          LOCK();
          piVar1 = (int *)((longlong)plVar9 + 0xc);
          *piVar1 = *piVar1 + -1;
          UNLOCK();
          if (*piVar1 == 0) {
            (**(code **)(*plVar9 + 8))(plVar9);
          }
        }
      }
    }
    else {
      iVar4 = *(int *)(lVar5 + 0x40);
      if (plVar9 != (longlong *)0x0) {
        LOCK();
        plVar6 = plVar9 + 1;
        *(int *)plVar6 = (int)*plVar6 + -1;
        UNLOCK();
        if ((int)*plVar6 == 0) {
          (**(code **)*plVar9)(plVar9);
          LOCK();
          piVar1 = (int *)((longlong)plVar9 + 0xc);
          *piVar1 = *piVar1 + -1;
          UNLOCK();
          if (*piVar1 == 0) {
            (**(code **)(*plVar9 + 8))(plVar9);
          }
        }
      }
      if (iVar4 < 1) {
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
        puStack_e8 = (undefined8 *)func_0x000180006450(param_3);
        piStack_e0 = (int *)CONCAT44(piStack_e0._4_4_,iVar11);
        plStack_f0 = (longlong *)CONCAT44(plStack_f0._4_4_,auStack_c8[0]);
        plStack_f8 = (longlong *)CONCAT44(plStack_f8._4_4_,0x1e6);
        plStack_d8 = plVar7;
        FUN_180015b40(&DAT_1802b1230,0,&UNK_180226180,&UNK_1802260c8);
      }
    }
    plVar9 = (longlong *)(ulonglong)(iVar11 + 1U);
    param_4 = plStack_a8;
    plVar6 = plStack_a0;
    iVar4 = iStack_b4;
  } while ((int)(iVar11 + 1U) < iStack_bc);
  if (plVar7 != (longlong *)0x0) {
    ___V_YAXPEAX_Z(plVar7);
  }
LAB_180095f05:
  FUN_1801e8810(uStack_48 ^ (ulonglong)auStack_118);
  return;
}

