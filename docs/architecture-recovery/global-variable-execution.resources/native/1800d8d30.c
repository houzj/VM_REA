
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_1800d8d30(longlong param_1,undefined4 param_2)

{
  int *piVar1;
  longlong *plVar2;
  undefined4 uVar3;
  longlong lVar4;
  longlong *plVar5;
  longlong *plVar6;
  char cVar7;
  int iVar8;
  undefined8 *puVar9;
  undefined8 uVar10;
  undefined8 uVar11;
  undefined8 *puVar12;
  undefined8 *puVar13;
  undefined8 *puVar14;
  longlong *plVar15;
  longlong *plVar16;
  undefined4 extraout_XMM0_Da;
  undefined4 extraout_XMM0_Da_00;
  undefined1 auStack_118 [32];
  longlong **pplStack_f8;
  undefined4 uStack_f0;
  undefined8 *puStack_e8;
  undefined8 uStack_e0;
  longlong *plStack_d8;
  undefined8 uStack_d0;
  int aiStack_c8 [2];
  longlong *plStack_c0;
  undefined8 uStack_b8;
  longlong *plStack_b0;
  undefined8 uStack_a8;
  undefined8 *puStack_a0;
  int *piStack_98;
  undefined8 uStack_90;
  longlong lStack_88;
  longlong *plStack_80;
  undefined8 uStack_78;
  undefined8 uStack_70;
  undefined8 auStack_68 [3];
  ulonglong uStack_50;
  ulonglong uStack_48;
  
  uStack_90 = 0xfffffffffffffffe;
  uStack_48 = _DAT_1802a5230 ^ (ulonglong)auStack_118;
  uStack_d0 = 0;
  plStack_d8 = (longlong *)FUN_18001f150(param_1,0,0);
  uStack_a8 = 0;
  plStack_b0 = (longlong *)FUN_18001f150(extraout_XMM0_Da,0,0);
  uStack_b8 = 0;
  plStack_c0 = (longlong *)FUN_18001f150(extraout_XMM0_Da_00,0,0);
  if ((_DAT_1802b1888 & 1) == 0) {
    _DAT_1802b1888 = _DAT_1802b1888 | 1;
    FUN_180107fc0();
    FUN_1801e8930(&UNK_180202980);
  }
  pplStack_f8 = &plStack_c0;
  iVar8 = FUN_180115440(&DAT_1802b1890,param_2,&plStack_d8,&plStack_b0);
  if ((iVar8 == 0) && (plVar15 = (longlong *)*plStack_d8, plVar15 != plStack_d8)) {
    do {
      plVar16 = (longlong *)plVar15[3];
      lVar4 = plVar15[2];
      if (plVar16 != (longlong *)0x0) {
        LOCK();
        *(int *)(plVar16 + 1) = (int)plVar16[1] + 1;
        UNLOCK();
      }
      lStack_88 = lVar4;
      plStack_80 = plVar16;
      if (lVar4 != 0) {
        aiStack_c8[0] = *(int *)(plVar15[2] + 0x2c);
        puVar9 = *(undefined8 **)(param_1 + 0x10);
        cVar7 = *(char *)((longlong)puVar9[1] + 0x19);
        puVar14 = puVar9;
        puVar13 = (undefined8 *)puVar9[1];
        while (cVar7 == '\0') {
          if (*(int *)(puVar13 + 4) < aiStack_c8[0]) {
            puVar12 = (undefined8 *)puVar13[2];
            puVar13 = puVar14;
          }
          else {
            puVar12 = (undefined8 *)*puVar13;
          }
          puVar14 = puVar13;
          puVar13 = puVar12;
          cVar7 = *(char *)((longlong)puVar12 + 0x19);
        }
        if ((puVar14 == puVar9) || (aiStack_c8[0] < *(int *)(puVar14 + 4))) {
          piStack_98 = aiStack_c8;
          pplStack_f8 = (longlong **)FUN_180085630(param_1 + 0x10,aiStack_c8[0],&piStack_98);
          FUN_180085670(param_1 + 0x10,&puStack_a0,puVar14,pplStack_f8 + 4);
          puVar14 = puStack_a0;
        }
        puVar9 = (undefined8 *)puVar14[5];
        if ((puVar9 != (undefined8 *)0x0) && (*(int *)(puVar9 + 4) == 1)) {
          uStack_78 = *puVar9;
          uStack_70 = puVar9[1];
          cVar7 = FUN_18001dbc0(2);
          if (cVar7 != '\0') {
            puVar9 = (undefined8 *)FUN_18001ec50(lVar4,auStack_68);
            uVar10 = _ctime64(&uStack_78);
            if (0xf < (ulonglong)puVar9[3]) {
              puVar9 = (undefined8 *)*puVar9;
            }
            uVar3 = *(undefined4 *)(lVar4 + 0x2c);
            uVar11 = FUN_18001db50();
            pplStack_f8 = (longlong **)CONCAT44(pplStack_f8._4_4_,0x5e);
            uStack_f0 = uVar3;
            puStack_e8 = puVar9;
            uStack_e0 = uVar10;
            FUN_180015b40(uVar11,2,&UNK_18022c318,&UNK_18022c348);
            if (0xf < uStack_50) {
              __3_YAXPEAX_Z(auStack_68[0]);
            }
          }
        }
      }
      if (plVar16 != (longlong *)0x0) {
        LOCK();
        plVar5 = plVar16 + 1;
        *(int *)plVar5 = (int)*plVar5 + -1;
        UNLOCK();
        if ((int)*plVar5 == 0) {
          (**(code **)*plVar16)(plVar16);
          LOCK();
          piVar1 = (int *)((longlong)plVar16 + 0xc);
          *piVar1 = *piVar1 + -1;
          UNLOCK();
          if (*piVar1 == 0) {
            (**(code **)(*plVar16 + 8))(plVar16);
          }
        }
      }
      plVar15 = (longlong *)*plVar15;
    } while (plVar15 != plStack_d8);
  }
  plVar15 = (longlong *)*plStack_c0;
  *plStack_c0 = (longlong)plStack_c0;
  plStack_c0[1] = (longlong)plStack_c0;
  plVar16 = plStack_c0;
  if (plVar15 != plStack_c0) {
    do {
      plVar5 = (longlong *)*plVar15;
      plVar6 = (longlong *)plVar15[3];
      if (plVar6 != (longlong *)0x0) {
        LOCK();
        plVar2 = plVar6 + 1;
        *(int *)plVar2 = (int)*plVar2 + -1;
        UNLOCK();
        plVar16 = plStack_c0;
        if ((int)*plVar2 == 0) {
          (**(code **)*plVar6)(plVar6);
          LOCK();
          piVar1 = (int *)((longlong)plVar6 + 0xc);
          *piVar1 = *piVar1 + -1;
          UNLOCK();
          plVar16 = plStack_c0;
          if (*piVar1 == 0) {
            (**(code **)(*plVar6 + 8))(plVar6);
            plVar16 = plStack_c0;
          }
        }
      }
      __3_YAXPEAX_Z(plVar15);
      plVar15 = plVar5;
    } while (plVar5 != plVar16);
  }
  __3_YAXPEAX_Z(plVar16);
  plVar15 = (longlong *)*plStack_b0;
  *plStack_b0 = (longlong)plStack_b0;
  plStack_b0[1] = (longlong)plStack_b0;
  plVar16 = plStack_b0;
  if (plVar15 != plStack_b0) {
    do {
      plVar5 = (longlong *)*plVar15;
      plVar6 = (longlong *)plVar15[3];
      if (plVar6 != (longlong *)0x0) {
        LOCK();
        plVar2 = plVar6 + 1;
        *(int *)plVar2 = (int)*plVar2 + -1;
        UNLOCK();
        plVar16 = plStack_b0;
        if ((int)*plVar2 == 0) {
          (**(code **)*plVar6)(plVar6);
          LOCK();
          piVar1 = (int *)((longlong)plVar6 + 0xc);
          *piVar1 = *piVar1 + -1;
          UNLOCK();
          plVar16 = plStack_b0;
          if (*piVar1 == 0) {
            (**(code **)(*plVar6 + 8))(plVar6);
            plVar16 = plStack_b0;
          }
        }
      }
      __3_YAXPEAX_Z(plVar15);
      plVar15 = plVar5;
    } while (plVar5 != plVar16);
  }
  __3_YAXPEAX_Z(plVar16);
  plVar15 = (longlong *)*plStack_d8;
  *plStack_d8 = (longlong)plStack_d8;
  plStack_d8[1] = (longlong)plStack_d8;
  if (plVar15 != plStack_d8) {
    do {
      plVar16 = (longlong *)*plVar15;
      plVar5 = (longlong *)plVar15[3];
      if (plVar5 != (longlong *)0x0) {
        LOCK();
        plVar6 = plVar5 + 1;
        *(int *)plVar6 = (int)*plVar6 + -1;
        UNLOCK();
        if ((int)*plVar6 == 0) {
          (**(code **)*plVar5)(plVar5);
          LOCK();
          piVar1 = (int *)((longlong)plVar5 + 0xc);
          *piVar1 = *piVar1 + -1;
          UNLOCK();
          if (*piVar1 == 0) {
            (**(code **)(*plVar5 + 8))(plVar5);
          }
        }
      }
      __3_YAXPEAX_Z(plVar15);
      plVar15 = plVar16;
    } while (plVar16 != plStack_d8);
  }
  __3_YAXPEAX_Z(plStack_d8);
  FUN_1801e8810(uStack_48 ^ (ulonglong)auStack_118);
  return;
}

