
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_180095f40(undefined8 param_1,undefined4 param_2,undefined8 **param_3,undefined8 param_4,
                  undefined1 param_5)

{
  longlong *plVar1;
  int *piVar2;
  bool bVar3;
  longlong lVar4;
  char cVar5;
  int iVar6;
  undefined8 **ppuVar7;
  undefined8 uVar8;
  longlong *plVar9;
  undefined8 uVar10;
  int iVar11;
  undefined4 extraout_XMM0_Da;
  undefined4 uVar12;
  undefined1 auStack_158 [32];
  undefined8 uStack_138;
  undefined8 *puStack_130;
  undefined8 **ppuStack_128;
  int *piStack_120;
  longlong *plStack_118;
  int iStack_110;
  int aiStack_108 [2];
  undefined8 *puStack_100;
  undefined8 uStack_f8;
  undefined8 uStack_f0;
  undefined8 uStack_e8;
  undefined8 auStack_e0 [3];
  ulonglong uStack_c8;
  undefined8 auStack_c0 [3];
  ulonglong uStack_a8;
  longlong alStack_98 [8];
  ulonglong uStack_58;
  
  uStack_e8 = 0xfffffffffffffffe;
  uStack_58 = _DAT_1802a5230 ^ (ulonglong)auStack_158;
  iVar11 = 0;
  aiStack_108[0] = 0;
  bVar3 = false;
  uStack_f0 = param_1;
  while( true ) {
    uVar10 = uStack_f0;
    uStack_f8 = 0;
    puStack_100 = (undefined8 *)0x0;
    plStack_118 = alStack_98;
    piStack_120 = aiStack_108;
    ppuStack_128 = &puStack_100;
    puStack_130 = &uStack_f8;
    uStack_138 = param_4;
    iVar6 = FUN_180097d10();
    if (iVar6 != 0) {
      cVar5 = FUN_18001dbc0(4);
      if (cVar5 != '\0') {
        plVar9 = (longlong *)FUN_18008f750(auStack_c0,iVar6);
        if (0xf < (ulonglong)plVar9[3]) {
          plVar9 = (longlong *)*plVar9;
        }
        if ((undefined8 *)0xf < param_3[3]) {
          param_3 = (undefined8 **)*param_3;
        }
        uVar10 = FUN_18001db50();
        piStack_120 = (int *)CONCAT44(piStack_120._4_4_,iVar11);
        puStack_130 = (undefined8 *)CONCAT44(puStack_130._4_4_,param_2);
        uStack_138 = CONCAT44(uStack_138._4_4_,0x211);
        ppuStack_128 = param_3;
        plStack_118 = plVar9;
        FUN_180015b40(uVar10,4,&UNK_180226200,&UNK_180226240);
        if (0xf < uStack_a8) {
          __3_YAXPEAX_Z(auStack_c0[0]);
        }
      }
      goto LAB_1800964b7;
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
    plVar9 = _DAT_1802b1240;
    lVar4 = _DAT_1802b1238;
    if (_DAT_1802b1240 != (longlong *)0x0) {
      LOCK();
      *(int *)(_DAT_1802b1240 + 1) = (int)_DAT_1802b1240[1] + 1;
      UNLOCK();
    }
    if (lVar4 == 0) {
      if (plVar9 != (longlong *)0x0) {
        LOCK();
        plVar1 = plVar9 + 1;
        *(int *)plVar1 = (int)*plVar1 + -1;
        UNLOCK();
        if ((int)*plVar1 == 0) {
          uVar12 = (**(code **)*plVar9)(plVar9);
          LOCK();
          piVar2 = (int *)((longlong)plVar9 + 0xc);
          *piVar2 = *piVar2 + -1;
          UNLOCK();
          if (*piVar2 == 0) {
            uVar12 = (**(code **)(*plVar9 + 8))(plVar9);
          }
        }
      }
    }
    else {
      iVar6 = *(int *)(lVar4 + 0x40);
      if (plVar9 != (longlong *)0x0) {
        LOCK();
        plVar1 = plVar9 + 1;
        *(int *)plVar1 = (int)*plVar1 + -1;
        UNLOCK();
        if ((int)*plVar1 == 0) {
          uVar12 = (**(code **)*plVar9)(plVar9);
          LOCK();
          piVar2 = (int *)((longlong)plVar9 + 0xc);
          *piVar2 = *piVar2 + -1;
          UNLOCK();
          if (*piVar2 == 0) {
            uVar12 = (**(code **)(*plVar9 + 8))(plVar9);
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
        ppuStack_128 = (undefined8 **)func_0x000180006450(param_3);
        iStack_110 = aiStack_108[0];
        plStack_118 = alStack_98;
        piStack_120 = (int *)CONCAT44(piStack_120._4_4_,iVar11);
        puStack_130 = (undefined8 *)CONCAT44(puStack_130._4_4_,param_2);
        uStack_138 = CONCAT44(uStack_138._4_4_,0x215);
        uVar12 = FUN_180015b40(&DAT_1802b1230,0,&UNK_1802262b8,&UNK_180226240);
      }
    }
    if (!bVar3) {
      iVar6 = FUN_18009a7e0(uVar12,param_2,param_3,4);
      if ((iVar6 != 0) && (cVar5 = FUN_18001dbc0(4), cVar5 != '\0')) {
        ppuVar7 = (undefined8 **)func_0x000180006450(param_3);
        uVar8 = FUN_18001db50();
        piStack_120 = (int *)CONCAT44(piStack_120._4_4_,iVar6);
        puStack_130 = (undefined8 *)CONCAT44(puStack_130._4_4_,param_2);
        uStack_138 = CONCAT44(uStack_138._4_4_,0x21d);
        ppuStack_128 = ppuVar7;
        FUN_180015b40(uVar8,4,&UNK_180226270,&UNK_180226240);
      }
      bVar3 = true;
    }
    iStack_110 = CONCAT31(iStack_110._1_3_,param_5);
    plStack_118 = alStack_98;
    puStack_130 = puStack_100;
    uStack_138 = uStack_f8;
    iVar6 = FUN_180098ae0(uVar10,param_2,param_3,iVar11);
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
    plVar9 = _DAT_1802b1240;
    lVar4 = _DAT_1802b1238;
    if (_DAT_1802b1240 != (longlong *)0x0) {
      LOCK();
      *(int *)(_DAT_1802b1240 + 1) = (int)_DAT_1802b1240[1] + 1;
      UNLOCK();
    }
    if (lVar4 == 0) {
      if (plVar9 != (longlong *)0x0) {
        LOCK();
        plVar1 = plVar9 + 1;
        *(int *)plVar1 = (int)*plVar1 + -1;
        UNLOCK();
        if ((int)*plVar1 == 0) {
          (**(code **)*plVar9)(plVar9);
          LOCK();
          piVar2 = (int *)((longlong)plVar9 + 0xc);
          *piVar2 = *piVar2 + -1;
          UNLOCK();
          if (*piVar2 == 0) {
            (**(code **)(*plVar9 + 8))(plVar9);
          }
        }
      }
    }
    else {
      iVar6 = *(int *)(lVar4 + 0x40);
      if (plVar9 != (longlong *)0x0) {
        LOCK();
        plVar1 = plVar9 + 1;
        *(int *)plVar1 = (int)*plVar1 + -1;
        UNLOCK();
        if ((int)*plVar1 == 0) {
          (**(code **)*plVar9)(plVar9);
          LOCK();
          piVar2 = (int *)((longlong)plVar9 + 0xc);
          *piVar2 = *piVar2 + -1;
          UNLOCK();
          if (*piVar2 == 0) {
            (**(code **)(*plVar9 + 8))(plVar9);
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
        ppuStack_128 = (undefined8 **)func_0x000180006450(param_3);
        plStack_118 = alStack_98;
        piStack_120 = (int *)CONCAT44(piStack_120._4_4_,iVar11);
        puStack_130 = (undefined8 *)CONCAT44(puStack_130._4_4_,param_2);
        uStack_138 = CONCAT44(uStack_138._4_4_,0x229);
        FUN_180015b40(&DAT_1802b1230,0,&UNK_1802262f8,&UNK_180226240);
      }
    }
    iVar11 = iVar11 + 1;
    if (aiStack_108[0] <= iVar11) {
LAB_1800964b7:
      FUN_1801e8810(uStack_58 ^ (ulonglong)auStack_158);
      return;
    }
  }
  cVar5 = FUN_18001dbc0(4);
  if (cVar5 != '\0') {
    plVar9 = (longlong *)FUN_18008f750(auStack_e0,iVar6);
    if (0xf < (ulonglong)plVar9[3]) {
      plVar9 = (longlong *)*plVar9;
    }
    if ((undefined8 *)0xf < param_3[3]) {
      param_3 = (undefined8 **)*param_3;
    }
    uVar10 = FUN_18001db50();
    piStack_120 = (int *)CONCAT44(piStack_120._4_4_,iVar11);
    puStack_130 = (undefined8 *)CONCAT44(puStack_130._4_4_,param_2);
    uStack_138 = CONCAT44(uStack_138._4_4_,0x225);
    ppuStack_128 = param_3;
    plStack_118 = plVar9;
    FUN_180015b40(uVar10,4,&UNK_180226330,&UNK_180226240);
    if (0xf < uStack_c8) {
      __3_YAXPEAX_Z(auStack_e0[0]);
    }
  }
  goto LAB_1800964b7;
}

