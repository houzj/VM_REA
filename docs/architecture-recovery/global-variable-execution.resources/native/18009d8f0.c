
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_18009d8f0(longlong param_1,undefined4 param_2,undefined8 *param_3,undefined4 param_4,
                  longlong *param_5)

{
  longlong *plVar1;
  int *piVar2;
  longlong *plVar3;
  undefined8 *puVar4;
  char cVar5;
  int iVar6;
  undefined8 uVar7;
  undefined8 *puVar8;
  undefined8 *puVar9;
  undefined8 *puVar10;
  longlong lVar11;
  undefined1 auStack_138 [32];
  longlong *plStack_118;
  undefined8 *puStack_110;
  int iStack_108;
  int aiStack_f8 [2];
  undefined4 auStack_f0 [2];
  undefined4 auStack_e8 [2];
  int *piStack_e0;
  longlong lStack_d8;
  undefined8 *puStack_d0;
  int *piStack_c8;
  undefined8 *puStack_c0;
  undefined8 uStack_b8;
  longlong *plStack_b0;
  undefined1 auStack_a8 [16];
  undefined1 auStack_98 [8];
  longlong *plStack_90;
  undefined1 uStack_80;
  undefined7 uStack_7f;
  undefined8 uStack_70;
  ulonglong uStack_68;
  ulonglong uStack_58;
  
  uStack_b8 = 0xfffffffffffffffe;
  uStack_58 = _DAT_1802a5230 ^ (ulonglong)auStack_138;
  plStack_b0 = param_5;
  auStack_f0[0] = param_2;
  auStack_e8[0] = param_4;
  if ((int *)*param_5 == (int *)0x0) {
    plVar3 = (longlong *)param_5[1];
    if (plVar3 != (longlong *)0x0) {
      LOCK();
      plVar1 = plVar3 + 1;
      *(int *)plVar1 = (int)*plVar1 + -1;
      UNLOCK();
      if ((int)*plVar1 == 0) {
        (**(code **)*plVar3)(plVar3);
        LOCK();
        piVar2 = (int *)((longlong)plVar3 + 0xc);
        *piVar2 = *piVar2 + -1;
        UNLOCK();
        if (*piVar2 == 0) {
          (**(code **)(*plVar3 + 8))(plVar3);
        }
      }
    }
  }
  else {
    if (*(int *)*param_5 == 3) {
      lVar11 = param_1 + 0x28;
      lStack_d8 = lVar11;
      iVar6 = _Mtx_lock(lVar11);
      if (iVar6 != 0) {
        __Throw_C_error_std__YAXH_Z(iVar6);
      }
      aiStack_f8[0] = *(int *)(*param_5 + 0x8c);
      plStack_118 = param_5;
      piStack_e0 = (int *)FUN_18009f160(auStack_98,auStack_f0,param_3,auStack_e8);
      puVar4 = *(undefined8 **)(param_1 + 0x30);
      cVar5 = *(char *)((longlong)puVar4[1] + 0x19);
      puVar10 = puVar4;
      puVar9 = (undefined8 *)puVar4[1];
      while (cVar5 == '\0') {
        if (*(int *)(puVar9 + 4) < aiStack_f8[0]) {
          puVar8 = (undefined8 *)puVar9[2];
          puVar9 = puVar10;
        }
        else {
          puVar8 = (undefined8 *)*puVar9;
        }
        puVar10 = puVar9;
        puVar9 = puVar8;
        cVar5 = *(char *)((longlong)puVar8 + 0x19);
      }
      if ((puVar10 == puVar4) || (aiStack_f8[0] < *(int *)(puVar10 + 4))) {
        piStack_c8 = aiStack_f8;
        plStack_118 = (longlong *)FUN_18009f2f0(param_1 + 0x30,puVar9,&piStack_c8);
        FUN_18009f370(param_1 + 0x30,&puStack_d0,puVar10,plStack_118 + 4);
        puVar10 = puStack_d0;
      }
      FUN_18009e400(puVar10 + 5,piStack_e0);
      if (0xf < uStack_68) {
        __3_YAXPEAX_Z(CONCAT71(uStack_7f,uStack_80));
      }
      uStack_68 = 0xf;
      uStack_70 = 0;
      uStack_80 = 0;
      if (plStack_90 != (longlong *)0x0) {
        LOCK();
        plVar3 = plStack_90 + 1;
        *(int *)plVar3 = (int)*plVar3 + -1;
        UNLOCK();
        lVar11 = lStack_d8;
        if ((int)*plVar3 == 0) {
          (**(code **)*plStack_90)(plStack_90);
          LOCK();
          piVar2 = (int *)((longlong)plStack_90 + 0xc);
          *piVar2 = *piVar2 + -1;
          UNLOCK();
          lVar11 = lStack_d8;
          if (*piVar2 == 0) {
            (**(code **)(*plStack_90 + 8))(plStack_90);
            lVar11 = lStack_d8;
          }
        }
      }
      puVar4 = *(undefined8 **)(param_1 + 0x40);
      cVar5 = *(char *)((longlong)puVar4[1] + 0x19);
      puVar10 = puVar4;
      puVar9 = (undefined8 *)puVar4[1];
      while (cVar5 == '\0') {
        if (*(int *)(puVar9 + 4) < aiStack_f8[0]) {
          puVar8 = (undefined8 *)puVar9[2];
          puVar9 = puVar10;
        }
        else {
          puVar8 = (undefined8 *)*puVar9;
        }
        puVar10 = puVar9;
        puVar9 = puVar8;
        cVar5 = *(char *)((longlong)puVar8 + 0x19);
      }
      if ((puVar10 == puVar4) || (aiStack_f8[0] < *(int *)(puVar10 + 4))) {
        piStack_e0 = aiStack_f8;
        plStack_118 = (longlong *)FUN_18009f5c0(param_1 + 0x40,aiStack_f8[0],&piStack_e0);
        FUN_18009f640(param_1 + 0x40,&puStack_c0,puVar10,plStack_118 + 4);
        puVar10 = puStack_c0;
      }
      plStack_118 = (longlong *)CONCAT71(plStack_118._1_7_,DAT_1802b1205);
      FUN_180071a90(puVar10 + 5,auStack_a8,0,auStack_f0);
      cVar5 = FUN_18001dbc0(1);
      if (cVar5 != '\0') {
        if (0xf < (ulonglong)param_3[3]) {
          param_3 = (undefined8 *)*param_3;
        }
        uVar7 = FUN_18001db50();
        iStack_108 = aiStack_f8[0];
        plStack_118 = (longlong *)CONCAT44(plStack_118._4_4_,0x266);
        puStack_110 = param_3;
        FUN_180015b40(uVar7,1,&UNK_1802271b8,&UNK_180227110);
      }
      iVar6 = _Mtx_unlock(lVar11);
      if (iVar6 != 0) {
        __Throw_C_error_std__YAXH_Z(iVar6);
      }
    }
    plVar3 = (longlong *)param_5[1];
    if (plVar3 != (longlong *)0x0) {
      LOCK();
      plVar1 = plVar3 + 1;
      *(int *)plVar1 = (int)*plVar1 + -1;
      UNLOCK();
      if ((int)*plVar1 == 0) {
        (**(code **)*plVar3)(plVar3);
        LOCK();
        piVar2 = (int *)((longlong)plVar3 + 0xc);
        *piVar2 = *piVar2 + -1;
        UNLOCK();
        if (*piVar2 == 0) {
          (**(code **)(*plVar3 + 8))(plVar3);
        }
      }
    }
  }
  FUN_1801e8810(uStack_58 ^ (ulonglong)auStack_138);
  return;
}

