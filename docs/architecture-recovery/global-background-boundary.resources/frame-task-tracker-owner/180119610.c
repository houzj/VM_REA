
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined8 FUN_180119610(longlong param_1,int param_2,int param_3,int param_4)

{
  int *piVar1;
  longlong *plVar2;
  longlong *plVar3;
  char cVar4;
  undefined8 *puVar5;
  code *pcVar6;
  bool bVar7;
  undefined8 **ppuVar8;
  longlong **pplVar9;
  longlong lVar10;
  undefined8 *puVar11;
  undefined8 *puVar12;
  undefined8 *puVar13;
  longlong *plVar14;
  undefined8 uVar15;
  undefined4 uStackX_8;
  int iStackX_c;
  int aiStackX_18 [4];
  longlong *plStack_c8;
  longlong *plStack_c0;
  longlong *plStack_b8;
  longlong *plStack_b0;
  undefined4 uStack_a8;
  int iStack_a4;
  longlong lStack_a0;
  longlong *plStack_98;
  longlong lStack_90;
  longlong *plStack_88;
  undefined8 *puStack_80;
  int *piStack_78;
  undefined8 *puStack_70;
  undefined8 *apuStack_68 [2];
  longlong *plStack_58;
  undefined8 uStack_50;
  undefined1 auStack_48 [8];
  longlong *plStack_40;
  
  uStack_50 = 0xfffffffffffffffe;
  uVar15 = 0;
  plVar3 = (longlong *)(param_1 + 0xf0);
  puVar5 = (undefined8 *)*plVar3;
  cVar4 = *(char *)((longlong)puVar5[1] + 0x19);
  puVar13 = puVar5;
  puVar12 = (undefined8 *)puVar5[1];
  while (cVar4 == '\0') {
    if (*(int *)(puVar12 + 4) < param_3) {
      puVar11 = (undefined8 *)puVar12[2];
      puVar12 = puVar13;
    }
    else {
      puVar11 = (undefined8 *)*puVar12;
    }
    puVar13 = puVar12;
    puVar12 = puVar11;
    cVar4 = *(char *)((longlong)puVar11 + 0x19);
  }
  if ((puVar13 == puVar5) || (param_3 < *(int *)(puVar13 + 4))) {
    apuStack_68[0] = puVar5;
    ppuVar8 = apuStack_68;
  }
  else {
    puStack_70 = puVar13;
    ppuVar8 = &puStack_70;
  }
  puVar5 = *ppuVar8;
  aiStackX_18[0] = param_3;
  if (puVar5 == (undefined8 *)*plVar3) {
    pplVar9 = (longlong **)FUN_18011b9c0(auStack_48);
    plStack_b8 = (longlong *)0x0;
    plStack_b0 = (longlong *)0x0;
    if (&plStack_b8 != pplVar9) {
      plStack_b0 = pplVar9[1];
      pplVar9[1] = (longlong *)0x0;
      plStack_b8 = *pplVar9;
      *pplVar9 = (longlong *)0x0;
    }
    plStack_c0 = plStack_b0;
    plStack_c8 = plStack_b8;
    if (plStack_40 != (longlong *)0x0) {
      LOCK();
      plVar14 = plStack_40 + 1;
      *(int *)plVar14 = (int)*plVar14 + -1;
      UNLOCK();
      if ((int)*plVar14 == 0) {
        (**(code **)*plStack_40)(plStack_40);
        LOCK();
        piVar1 = (int *)((longlong)plStack_40 + 0xc);
        *piVar1 = *piVar1 + -1;
        UNLOCK();
        if (*piVar1 == 0) {
          (**(code **)(*plStack_40 + 8))(plStack_40);
        }
      }
    }
    plVar14 = plStack_b0;
    puVar5 = (undefined8 *)*plVar3;
    cVar4 = *(char *)((longlong)puVar5[1] + 0x19);
    puVar13 = puVar5;
    puVar12 = (undefined8 *)puVar5[1];
    while (cVar4 == '\0') {
      if (*(int *)(puVar12 + 4) < aiStackX_18[0]) {
        puVar11 = (undefined8 *)puVar12[2];
        puVar12 = puVar13;
      }
      else {
        puVar11 = (undefined8 *)*puVar12;
      }
      puVar13 = puVar12;
      puVar12 = puVar11;
      cVar4 = *(char *)((longlong)puVar11 + 0x19);
    }
    if ((puVar13 == puVar5) || (aiStackX_18[0] < *(int *)(puVar13 + 4))) {
      piStack_78 = aiStackX_18;
      lVar10 = FUN_180085630(plVar3,aiStackX_18[0],&piStack_78);
      FUN_180085670(plVar3,&puStack_80,puVar13,lVar10 + 0x20,lVar10);
      puVar13 = puStack_80;
    }
    if (plVar14 != (longlong *)0x0) {
      LOCK();
      *(int *)(plVar14 + 1) = (int)plVar14[1] + 1;
      UNLOCK();
      plVar14 = plStack_b0;
    }
    plVar3 = (longlong *)puVar13[6];
    puVar13[6] = plVar14;
    puVar13[5] = plStack_b8;
    plStack_58 = plVar3;
    if (plVar3 != (longlong *)0x0) {
      LOCK();
      plVar14 = plVar3 + 1;
      *(int *)plVar14 = (int)*plVar14 + -1;
      UNLOCK();
      if ((int)*plVar14 == 0) {
        (**(code **)*plVar3)();
        LOCK();
        piVar1 = (int *)((longlong)plVar3 + 0xc);
        *piVar1 = *piVar1 + -1;
        UNLOCK();
        if (*piVar1 == 0) {
          (**(code **)(*plStack_58 + 8))();
        }
      }
    }
  }
  else {
    plStack_c0 = (longlong *)puVar5[6];
    plStack_c8 = (longlong *)puVar5[5];
    if (plStack_c0 != (longlong *)0x0) {
      LOCK();
      *(int *)(plStack_c0 + 1) = (int)plStack_c0[1] + 1;
      UNLOCK();
    }
  }
  bVar7 = false;
  plVar3 = (longlong *)*plStack_c8;
  plVar14 = (longlong *)*plVar3;
  do {
    if (plVar14 == plVar3) {
LAB_180119807:
      if (param_4 == 1) {
        if (bVar7) {
          uVar15 = 0xe0000303;
        }
        else {
          uStackX_8 = 1;
          iStackX_c = param_2;
          lVar10 = FUN_18011c640(0,plVar3,plVar3[1],&uStackX_8);
          if (plStack_c8[1] == 0xaaaaaaaaaaaaaa9) {
            __Xlength_error_std__YAXPEBD_Z(&UNK_18021d318);
            pcVar6 = (code *)swi(3);
            uVar15 = (*pcVar6)();
            return uVar15;
          }
          plStack_c8[1] = plStack_c8[1] + 1;
          plVar3[1] = lVar10;
          **(longlong **)(lVar10 + 8) = lVar10;
        }
      }
      else if (param_4 == 2) {
        if (bVar7) {
          uVar15 = 0xe0000303;
        }
        else {
          uStack_a8 = 2;
          iStack_a4 = param_2;
          lVar10 = FUN_18011c640(0,plVar3,plVar3[1],&uStack_a8);
          if (plStack_c8[1] == 0xaaaaaaaaaaaaaa9) {
            __Xlength_error_std__YAXPEBD_Z(&UNK_18021d318);
            pcVar6 = (code *)swi(3);
            uVar15 = (*pcVar6)();
            return uVar15;
          }
          plStack_c8[1] = plStack_c8[1] + 1;
          plVar3[1] = lVar10;
          **(longlong **)(lVar10 + 8) = lVar10;
          if ((_DAT_1802b1888 & 1) == 0) {
            _DAT_1802b1888 = _DAT_1802b1888 | 1;
            FUN_180107fc0();
            FUN_1801e8930(&UNK_180202980);
          }
          FUN_180108ba0(&DAT_1802b1890,&lStack_90,aiStackX_18[0]);
          if ((_DAT_1802b2ac8 & 1) == 0) {
            _DAT_1802b2ac8 = _DAT_1802b2ac8 | 1;
            FUN_1800b5280();
            FUN_1801e8930(&UNK_1802029e0);
          }
          lVar10 = FUN_1800b6b70(&DAT_1802b2ad0,param_2);
          plVar3 = plStack_88;
          if ((lStack_90 == 0) || (lVar10 == 0)) {
            uVar15 = 0xe0000302;
          }
          else {
            *(longlong *)(lStack_90 + 0x1e8) = lVar10;
          }
          if (plStack_88 != (longlong *)0x0) {
            LOCK();
            plVar14 = plStack_88 + 1;
            *(int *)plVar14 = (int)*plVar14 + -1;
            UNLOCK();
            if ((int)*plVar14 == 0) {
              (**(code **)*plStack_88)(plStack_88);
              LOCK();
              piVar1 = (int *)((longlong)plVar3 + 0xc);
              *piVar1 = *piVar1 + -1;
              UNLOCK();
              if (*piVar1 == 0) {
                (**(code **)(*plStack_88 + 8))();
              }
            }
          }
        }
      }
      else if (bVar7) {
        if ((int)plVar14[2] == 2) {
          if ((_DAT_1802b1888 & 1) == 0) {
            _DAT_1802b1888 = _DAT_1802b1888 | 1;
            FUN_180107fc0();
            FUN_1801e8930(&UNK_180202980);
          }
          FUN_180108ba0(&DAT_1802b1890,&lStack_a0,aiStackX_18[0]);
          plVar3 = plStack_98;
          if (lStack_a0 != 0) {
            *(undefined8 *)(lStack_a0 + 0x1e8) = 0;
          }
          if (plStack_98 != (longlong *)0x0) {
            LOCK();
            plVar2 = plStack_98 + 1;
            *(int *)plVar2 = (int)*plVar2 + -1;
            UNLOCK();
            if ((int)*plVar2 == 0) {
              (**(code **)*plStack_98)(plStack_98);
              LOCK();
              piVar1 = (int *)((longlong)plVar3 + 0xc);
              *piVar1 = *piVar1 + -1;
              UNLOCK();
              if (*piVar1 == 0) {
                (**(code **)(*plStack_98 + 8))();
              }
            }
          }
        }
        *(longlong *)plVar14[1] = *plVar14;
        *(longlong *)(*plVar14 + 8) = plVar14[1];
        plStack_c8[1] = plStack_c8[1] + -1;
        __3_YAXPEAX_Z(plVar14);
      }
      else {
        uVar15 = 0xe0000302;
      }
      if (plStack_c0 != (longlong *)0x0) {
        LOCK();
        plVar3 = plStack_c0 + 1;
        *(int *)plVar3 = (int)*plVar3 + -1;
        UNLOCK();
        if ((int)*plVar3 == 0) {
          (**(code **)*plStack_c0)(plStack_c0);
          LOCK();
          piVar1 = (int *)((longlong)plStack_c0 + 0xc);
          *piVar1 = *piVar1 + -1;
          UNLOCK();
          if (*piVar1 == 0) {
            (**(code **)(*plStack_c0 + 8))();
          }
        }
      }
      return uVar15;
    }
    if (*(int *)((longlong)plVar14 + 0x14) == param_2) {
      bVar7 = true;
      goto LAB_180119807;
    }
    plVar14 = (longlong *)*plVar14;
  } while( true );
}

