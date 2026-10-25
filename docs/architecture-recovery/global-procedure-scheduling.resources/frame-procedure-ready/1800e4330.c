
undefined8 FUN_1800e4330(longlong param_1,int param_2,undefined4 param_3,int *param_4)

{
  longlong *plVar1;
  char cVar2;
  undefined8 *puVar3;
  int iVar4;
  longlong lVar5;
  longlong lVar6;
  undefined8 uVar7;
  undefined8 *puVar8;
  undefined8 *puVar9;
  undefined8 *puStackX_8;
  int aiStackX_10 [2];
  undefined4 auStackX_18 [4];
  
  plVar1 = (longlong *)(param_1 + 0x18);
  puVar3 = (undefined8 *)*plVar1;
  cVar2 = *(char *)((longlong)puVar3[1] + 0x19);
  puStackX_8 = puVar3;
  puVar9 = (undefined8 *)puVar3[1];
  while (cVar2 == '\0') {
    if (*(int *)(puVar9 + 4) < param_2) {
      puVar8 = (undefined8 *)puVar9[2];
      puVar9 = puStackX_8;
    }
    else {
      puVar8 = (undefined8 *)*puVar9;
    }
    puStackX_8 = puVar9;
    puVar9 = puVar8;
    cVar2 = *(char *)((longlong)puVar8 + 0x19);
  }
  if ((puStackX_8 == puVar3) || (param_2 < *(int *)(puStackX_8 + 4))) {
    puStackX_8 = puVar3;
  }
  if (puStackX_8 == puVar3) {
    uVar7 = 0xe0000001;
  }
  else {
    aiStackX_10[0] = param_2;
    auStackX_18[0] = param_3;
    iVar4 = _Mtx_lock(param_1 + 0x28);
    if (iVar4 != 0) {
      __Throw_C_error_std__YAXH_Z(iVar4);
    }
    lVar5 = FUN_1800e4570(plVar1,aiStackX_10);
    FUN_18006ddb0(lVar5 + 0x10,auStackX_18);
    lVar5 = FUN_1800e4570(plVar1,aiStackX_10);
    iVar4 = (int)*(undefined8 *)(lVar5 + 0x18);
    *param_4 = iVar4;
    if (iVar4 == 0) {
      lVar6 = FUN_1800e4570(plVar1,aiStackX_10);
      lVar5 = FUN_1800e4570(plVar1,aiStackX_10);
      lVar5 = lVar5 + 0x10;
      if (lVar5 != lVar6) {
        FUN_180070160(lVar5);
        FUN_180077b00(lVar5,lVar6,0);
      }
    }
    iVar4 = _Mtx_unlock(param_1 + 0x28);
    if (iVar4 != 0) {
      __Throw_C_error_std__YAXH_Z(iVar4);
    }
    uVar7 = 0;
  }
  return uVar7;
}

