
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_180096a60(undefined8 param_1,undefined4 param_2,longlong *param_3,undefined8 *param_4,
                  undefined1 param_5)

{
  longlong *plVar1;
  int *piVar2;
  bool bVar3;
  longlong lVar4;
  char cVar5;
  int iVar6;
  longlong *plVar7;
  undefined8 uVar8;
  undefined8 *puVar9;
  undefined8 uVar10;
  int iVar11;
  undefined4 extraout_XMM0_Da;
  undefined4 uVar12;
  undefined1 auStack_108 [32];
  undefined8 *puStack_e8;
  undefined8 **ppuStack_e0;
  longlong *plStack_d8;
  int *piStack_d0;
  undefined8 *puStack_c8;
  int iStack_c0;
  int iStack_b8;
  undefined4 uStack_b4;
  undefined8 *puStack_b0;
  undefined8 uStack_a8;
  undefined8 uStack_a0;
  undefined8 auStack_98 [3];
  ulonglong uStack_80;
  undefined8 auStack_78 [3];
  ulonglong uStack_60;
  ulonglong uStack_58;
  
  uStack_a0 = 0xfffffffffffffffe;
  uStack_58 = _DAT_1802a5230 ^ (ulonglong)auStack_108;
  iVar11 = 0;
  iStack_b8 = 0;
  bVar3 = false;
  uStack_a8 = param_1;
  while( true ) {
    uVar10 = uStack_a8;
    puStack_b0 = (undefined8 *)0x0;
    uStack_b4 = 0;
    piStack_d0 = &iStack_b8;
    plStack_d8 = (longlong *)&uStack_b4;
    ppuStack_e0 = &puStack_b0;
    puStack_e8 = param_4;
    iVar6 = FUN_180098150();
    if (iVar6 != 0) {
      cVar5 = FUN_18001dbc0(4);
      if (cVar5 != '\0') {
        puVar9 = (undefined8 *)FUN_18008f750(auStack_78,iVar6);
        if (0xf < (ulonglong)puVar9[3]) {
          puVar9 = (undefined8 *)*puVar9;
        }
        if (0xf < (ulonglong)param_3[3]) {
          param_3 = (longlong *)*param_3;
        }
        uVar10 = FUN_18001db50();
        piStack_d0 = (int *)CONCAT44(piStack_d0._4_4_,iVar11);
        ppuStack_e0 = (undefined8 **)CONCAT44(ppuStack_e0._4_4_,param_2);
        puStack_e8 = (undefined8 *)CONCAT44(puStack_e8._4_4_,0x26b);
        plStack_d8 = param_3;
        puStack_c8 = puVar9;
        FUN_180015b40(uVar10,4,&UNK_180226518,&UNK_180226558);
        if (0xf < uStack_60) {
          __3_YAXPEAX_Z(auStack_78[0]);
        }
      }
      goto LAB_180096fb4;
    }
    uVar12 = extraout_XMM0_Da;
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
      uVar12 = FUN_1801e8930(&UNK_180202850);
    }
    plVar7 = _DAT_1802b1240;
    lVar4 = _DAT_1802b1238;
    if (_DAT_1802b1240 != (longlong *)0x0) {
      LOCK();
      *(int *)(_DAT_1802b1240 + 1) = (int)_DAT_1802b1240[1] + 1;
      UNLOCK();
    }
    if (lVar4 == 0) {
      if (plVar7 != (longlong *)0x0) {
        LOCK();
        plVar1 = plVar7 + 1;
        *(int *)plVar1 = (int)*plVar1 + -1;
        UNLOCK();
        if ((int)*plVar1 == 0) {
          uVar12 = (**(code **)*plVar7)(plVar7);
          LOCK();
          piVar2 = (int *)((longlong)plVar7 + 0xc);
          *piVar2 = *piVar2 + -1;
          UNLOCK();
          if (*piVar2 == 0) {
            uVar12 = (**(code **)(*plVar7 + 8))(plVar7);
          }
        }
      }
    }
    else {
      iVar6 = *(int *)(lVar4 + 0x40);
      if (plVar7 != (longlong *)0x0) {
        LOCK();
        plVar1 = plVar7 + 1;
        *(int *)plVar1 = (int)*plVar1 + -1;
        UNLOCK();
        if ((int)*plVar1 == 0) {
          uVar12 = (**(code **)*plVar7)(plVar7);
          LOCK();
          piVar2 = (int *)((longlong)plVar7 + 0xc);
          *piVar2 = *piVar2 + -1;
          UNLOCK();
          if (*piVar2 == 0) {
            uVar12 = (**(code **)(*plVar7 + 8))(plVar7);
          }
        }
      }
      if (iVar6 < 1) {
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
        plStack_d8 = (longlong *)func_0x000180006450(param_3);
        iStack_c0 = iStack_b8;
        puStack_c8 = puStack_b0;
        piStack_d0 = (int *)CONCAT44(piStack_d0._4_4_,iVar11);
        ppuStack_e0 = (undefined8 **)CONCAT44(ppuStack_e0._4_4_,param_2);
        puStack_e8 = (undefined8 *)CONCAT44(puStack_e8._4_4_,0x26f);
        uVar12 = FUN_180015b40(&DAT_1802b1230,0,&UNK_1802265d8,&UNK_180226558);
      }
    }
    if (!bVar3) {
      iVar6 = FUN_18009a7e0(uVar12,param_2,param_3,6);
      if ((iVar6 != 0) && (cVar5 = FUN_18001dbc0(4), cVar5 != '\0')) {
        plVar7 = (longlong *)func_0x000180006450(param_3);
        uVar8 = FUN_18001db50();
        piStack_d0 = (int *)CONCAT44(piStack_d0._4_4_,iVar6);
        ppuStack_e0 = (undefined8 **)CONCAT44(ppuStack_e0._4_4_,param_2);
        puStack_e8 = (undefined8 *)CONCAT44(puStack_e8._4_4_,0x277);
        plStack_d8 = plVar7;
        FUN_180015b40(uVar8,4,&UNK_180226590,&UNK_180226558);
      }
      bVar3 = true;
    }
    plStack_d8 = (longlong *)CONCAT71(plStack_d8._1_7_,param_5);
    ppuStack_e0 = (undefined8 **)CONCAT44(ppuStack_e0._4_4_,uStack_b4);
    puStack_e8 = puStack_b0;
    iVar6 = FUN_180099100(uVar10,param_2,param_3,iVar11);
    if (iVar6 != 0) break;
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
    plVar7 = _DAT_1802b1240;
    lVar4 = _DAT_1802b1238;
    if (_DAT_1802b1240 != (longlong *)0x0) {
      LOCK();
      *(int *)(_DAT_1802b1240 + 1) = (int)_DAT_1802b1240[1] + 1;
      UNLOCK();
    }
    if (lVar4 == 0) {
      if (plVar7 != (longlong *)0x0) {
        LOCK();
        plVar1 = plVar7 + 1;
        *(int *)plVar1 = (int)*plVar1 + -1;
        UNLOCK();
        if ((int)*plVar1 == 0) {
          (**(code **)*plVar7)(plVar7);
          LOCK();
          piVar2 = (int *)((longlong)plVar7 + 0xc);
          *piVar2 = *piVar2 + -1;
          UNLOCK();
          if (*piVar2 == 0) {
            (**(code **)(*plVar7 + 8))(plVar7);
          }
        }
      }
    }
    else {
      iVar6 = *(int *)(lVar4 + 0x40);
      if (plVar7 != (longlong *)0x0) {
        LOCK();
        plVar1 = plVar7 + 1;
        *(int *)plVar1 = (int)*plVar1 + -1;
        UNLOCK();
        if ((int)*plVar1 == 0) {
          (**(code **)*plVar7)(plVar7);
          LOCK();
          piVar2 = (int *)((longlong)plVar7 + 0xc);
          *piVar2 = *piVar2 + -1;
          UNLOCK();
          if (*piVar2 == 0) {
            (**(code **)(*plVar7 + 8))(plVar7);
          }
        }
      }
      if (iVar6 < 1) {
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
        plStack_d8 = (longlong *)func_0x000180006450(param_3);
        puStack_c8 = puStack_b0;
        piStack_d0 = (int *)CONCAT44(piStack_d0._4_4_,iVar11);
        ppuStack_e0 = (undefined8 **)CONCAT44(ppuStack_e0._4_4_,param_2);
        puStack_e8 = (undefined8 *)CONCAT44(puStack_e8._4_4_,0x283);
        FUN_180015b40(&DAT_1802b1230,0,&UNK_180226618,&UNK_180226558);
      }
    }
    iVar11 = iVar11 + 1;
    if (iStack_b8 <= iVar11) {
LAB_180096fb4:
      FUN_1801e8810(uStack_58 ^ (ulonglong)auStack_108);
      return;
    }
  }
  cVar5 = FUN_18001dbc0(4);
  if (cVar5 != '\0') {
    puVar9 = (undefined8 *)FUN_18008f750(auStack_98,iVar6);
    if (0xf < (ulonglong)puVar9[3]) {
      puVar9 = (undefined8 *)*puVar9;
    }
    if (0xf < (ulonglong)param_3[3]) {
      param_3 = (longlong *)*param_3;
    }
    uVar10 = FUN_18001db50();
    piStack_d0 = (int *)CONCAT44(piStack_d0._4_4_,iVar11);
    ppuStack_e0 = (undefined8 **)CONCAT44(ppuStack_e0._4_4_,param_2);
    puStack_e8 = (undefined8 *)CONCAT44(puStack_e8._4_4_,0x27f);
    plStack_d8 = param_3;
    puStack_c8 = puVar9;
    FUN_180015b40(uVar10,4,&UNK_180226650,&UNK_180226558);
    if (0xf < uStack_80) {
      __3_YAXPEAX_Z(auStack_98[0]);
    }
  }
  goto LAB_180096fb4;
}

