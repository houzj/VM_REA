
/* WARNING: Removing unreachable block (ram,0x0001800e3ae4) */
/* WARNING: Removing unreachable block (ram,0x0001800e3af0) */
/* WARNING: Removing unreachable block (ram,0x0001800e3af9) */
/* WARNING: Removing unreachable block (ram,0x0001800e3b1e) */
/* WARNING: Removing unreachable block (ram,0x0001800e3b28) */
/* WARNING: Removing unreachable block (ram,0x0001800e3b2e) */
/* WARNING: Removing unreachable block (ram,0x0001800e3b3b) */
/* WARNING: Removing unreachable block (ram,0x0001800e3b03) */
/* WARNING: Removing unreachable block (ram,0x0001800e3b10) */
/* WARNING: Removing unreachable block (ram,0x0001800e3b1c) */
/* WARNING: Removing unreachable block (ram,0x0001800e3b3e) */
/* WARNING: Removing unreachable block (ram,0x0001800e3bc4) */
/* WARNING: Removing unreachable block (ram,0x0001800e3bd0) */
/* WARNING: Removing unreachable block (ram,0x0001800e3bd9) */
/* WARNING: Removing unreachable block (ram,0x0001800e3bfe) */
/* WARNING: Removing unreachable block (ram,0x0001800e3c08) */
/* WARNING: Removing unreachable block (ram,0x0001800e3c0e) */
/* WARNING: Removing unreachable block (ram,0x0001800e3c1b) */
/* WARNING: Removing unreachable block (ram,0x0001800e3be3) */
/* WARNING: Removing unreachable block (ram,0x0001800e3bf0) */
/* WARNING: Removing unreachable block (ram,0x0001800e3bfc) */
/* WARNING: Removing unreachable block (ram,0x0001800e3c1e) */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined8 FUN_1800e38a0(longlong param_1,undefined4 param_2,int param_3)

{
  int *piVar1;
  char cVar2;
  undefined8 *puVar3;
  longlong *plVar4;
  undefined8 *puVar5;
  longlong *plVar6;
  int iVar7;
  undefined8 **ppuVar8;
  undefined8 uVar9;
  undefined8 uVar10;
  longlong lVar11;
  undefined8 *puVar12;
  undefined8 *puVar13;
  undefined8 uVar14;
  undefined8 *puStackX_8;
  undefined4 auStackX_10 [2];
  int aiStackX_18 [2];
  undefined8 *puStackX_20;
  longlong lStack_108;
  undefined8 uStack_100;
  longlong lStack_f8;
  undefined8 uStack_f0;
  longlong lStack_e8;
  longlong *plStack_e0;
  longlong lStack_d8;
  longlong *plStack_d0;
  longlong lStack_c8;
  undefined8 *apuStack_c0 [2];
  undefined8 *apuStack_b0 [2];
  undefined8 uStack_a0;
  undefined1 auStack_88 [8];
  undefined1 auStack_80 [8];
  undefined1 auStack_78 [16];
  undefined1 auStack_68 [16];
  undefined1 auStack_58 [24];
  
  uStack_a0 = 0xfffffffffffffffe;
  lStack_c8 = param_1 + 0x28;
  auStackX_10[0] = param_2;
  aiStackX_18[0] = param_3;
  iVar7 = _Mtx_lock();
  if (iVar7 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar7);
  }
  if ((_DAT_1802b1888 & 1) == 0) {
    _DAT_1802b1888 = _DAT_1802b1888 | 1;
    FUN_180107fc0();
    FUN_1801e8930(&UNK_180202980);
  }
  FUN_180108ba0(&DAT_1802b1890,&lStack_e8,auStackX_10[0]);
  if (lStack_e8 == 0) {
    uVar14 = 0xe0000316;
  }
  else {
    if ((_DAT_1802b1888 & 1) == 0) {
      _DAT_1802b1888 = _DAT_1802b1888 | 1;
      FUN_180107fc0();
      FUN_1801e8930(&UNK_180202980);
    }
    FUN_180108ba0(&DAT_1802b1890,&lStack_d8,aiStackX_18[0]);
    if (lStack_d8 == 0) {
      uVar14 = 0xe0000316;
    }
    else {
      puVar3 = *(undefined8 **)(param_1 + 0x18);
      cVar2 = *(char *)((longlong)puVar3[1] + 0x19);
      puVar5 = puVar3;
      puVar13 = (undefined8 *)puVar3[1];
      while (cVar2 == '\0') {
        if (*(int *)(puVar13 + 4) < aiStackX_18[0]) {
          puVar12 = (undefined8 *)puVar13[2];
          puVar13 = puVar5;
        }
        else {
          puVar12 = (undefined8 *)*puVar13;
        }
        puVar5 = puVar13;
        puVar13 = puVar12;
        cVar2 = *(char *)((longlong)puVar12 + 0x19);
      }
      if ((puVar5 == puVar3) || (aiStackX_18[0] < *(int *)(puVar5 + 4))) {
        puStackX_20 = puVar3;
        ppuVar8 = &puStackX_20;
      }
      else {
        puStackX_8 = puVar5;
        ppuVar8 = &puStackX_8;
      }
      uVar14 = 0;
      if (*ppuVar8 == puVar3) {
        lStack_f8 = 0;
        uStack_f0 = 0;
        lStack_f8 = FUN_180079000(&lStack_f8);
        lStack_108 = 0;
        uStack_100 = 0;
        lStack_108 = FUN_180079000(&lStack_108);
        uVar9 = FUN_1800e4d00(apuStack_c0,&lStack_108,&lStack_f8);
        uVar10 = FUN_1800e4570(param_1 + 0x18,aiStackX_18);
        FUN_1800e4710(uVar10,uVar9);
        FUN_18006efa0(apuStack_b0,auStack_80,*apuStack_b0[0],apuStack_b0[0]);
        __3_YAXPEAX_Z(apuStack_b0[0]);
        FUN_18006efa0(apuStack_c0,auStack_88,*apuStack_c0[0],apuStack_c0[0]);
        __3_YAXPEAX_Z(apuStack_c0[0]);
        cVar2 = *(char *)((longlong)*(longlong **)(lStack_108 + 8) + 0x19);
        plVar6 = *(longlong **)(lStack_108 + 8);
        while (cVar2 == '\0') {
          FUN_18003c6d0(&lStack_108,plVar6[2]);
          plVar4 = (longlong *)*plVar6;
          __3_YAXPEAX_Z(plVar6);
          plVar6 = plVar4;
          cVar2 = *(char *)((longlong)plVar4 + 0x19);
        }
        *(longlong *)(lStack_108 + 8) = lStack_108;
        *(longlong *)lStack_108 = lStack_108;
        *(longlong *)(lStack_108 + 0x10) = lStack_108;
        uStack_100 = 0;
        __3_YAXPEAX_Z(lStack_108);
        cVar2 = *(char *)((longlong)*(longlong **)(lStack_f8 + 8) + 0x19);
        plVar6 = *(longlong **)(lStack_f8 + 8);
        while (cVar2 == '\0') {
          FUN_18003c6d0(&lStack_f8,plVar6[2]);
          plVar4 = (longlong *)*plVar6;
          __3_YAXPEAX_Z(plVar6);
          plVar6 = plVar4;
          cVar2 = *(char *)((longlong)plVar4 + 0x19);
        }
        *(longlong *)(lStack_f8 + 8) = lStack_f8;
        *(longlong *)lStack_f8 = lStack_f8;
        *(longlong *)(lStack_f8 + 0x10) = lStack_f8;
        uStack_f0 = 0;
        __3_YAXPEAX_Z(lStack_f8);
      }
      uVar9 = FUN_18009e260(param_1 + 8,auStackX_10);
      lVar11 = func_0x00018007c900(uVar9,aiStackX_18);
      if (lVar11 == 0) {
        uVar9 = FUN_18009e260(param_1 + 8,auStackX_10);
        FUN_180071a90(uVar9,auStack_78,0,aiStackX_18,DAT_1802b1205);
        uVar9 = FUN_1800e4570(param_1 + 0x18,aiStackX_18);
        FUN_180071a90(uVar9,auStack_58,0,auStackX_10,DAT_1802b1205);
        lVar11 = FUN_1800e4570(param_1 + 0x18,aiStackX_18);
        FUN_180071a90(lVar11 + 0x10,auStack_68,0,auStackX_10,DAT_1802b1205);
      }
    }
    plVar6 = plStack_d0;
    if (plStack_d0 != (longlong *)0x0) {
      LOCK();
      plVar4 = plStack_d0 + 1;
      *(int *)plVar4 = (int)*plVar4 + -1;
      UNLOCK();
      if ((int)*plVar4 == 0) {
        (**(code **)*plStack_d0)(plStack_d0);
        LOCK();
        piVar1 = (int *)((longlong)plVar6 + 0xc);
        *piVar1 = *piVar1 + -1;
        UNLOCK();
        if (*piVar1 == 0) {
          (**(code **)(*plStack_d0 + 8))();
        }
      }
    }
  }
  plVar6 = plStack_e0;
  if (plStack_e0 != (longlong *)0x0) {
    LOCK();
    plVar4 = plStack_e0 + 1;
    *(int *)plVar4 = (int)*plVar4 + -1;
    UNLOCK();
    if ((int)*plVar4 == 0) {
      (**(code **)*plStack_e0)(plStack_e0);
      LOCK();
      piVar1 = (int *)((longlong)plVar6 + 0xc);
      *piVar1 = *piVar1 + -1;
      UNLOCK();
      if (*piVar1 == 0) {
        (**(code **)(*plStack_e0 + 8))();
      }
    }
  }
  iVar7 = _Mtx_unlock(lStack_c8);
  if (iVar7 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar7);
  }
  return uVar14;
}

