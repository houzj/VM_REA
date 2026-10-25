
undefined8 FUN_180080dc0(longlong param_1,longlong param_2)

{
  longlong lVar1;
  longlong lVar2;
  longlong lVar3;
  undefined8 *puVar4;
  undefined8 *puVar5;
  int iVar6;
  longlong lVar7;
  undefined8 *puVar8;
  undefined8 *puVar9;
  undefined8 *puVar10;
  undefined8 uVar11;
  undefined8 *puStackX_8;
  longlong lStackX_10;
  longlong lStackX_18;
  longlong lStackX_20;
  undefined1 auStack_40 [24];
  
  lVar1 = param_1 + 0x18;
  lStackX_10 = param_2;
  lStackX_18 = lVar1;
  iVar6 = _Mtx_lock(lVar1);
  if (iVar6 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar6);
  }
  lVar2 = param_1 + 0x10;
  lStackX_20 = lVar2;
  iVar6 = _Mtx_lock(lVar2);
  if (iVar6 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar6);
  }
  lVar7 = func_0x000180081b20(param_1 + 0x70,&lStackX_10);
  lVar3 = *(longlong *)(param_1 + 0x80);
  puVar10 = *(undefined8 **)(lVar3 + lVar7 * 0x10);
  puVar4 = *(undefined8 **)(param_1 + 0x70);
  while( true ) {
    puVar8 = puVar4;
    if (*(undefined8 **)(lVar3 + lVar7 * 0x10) != puVar4) {
      puVar8 = (undefined8 *)**(undefined8 **)(lVar3 + lVar7 * 0x10 + 8);
    }
    puVar9 = puVar4;
    puVar5 = puStackX_8;
    if ((puVar10 == puVar8) || (puVar9 = puVar10, puVar5 = puVar10, param_2 == puVar10[2])) break;
    puVar10 = (undefined8 *)*puVar10;
  }
  puStackX_8 = puVar5;
  if (puVar9 == *(undefined8 **)(param_1 + 0x70)) {
    FUN_180082190(param_1 + 0xb0,auStack_40,&lStackX_10,DAT_1802b1206);
    uVar11 = 0;
  }
  else {
    lVar7 = func_0x000180081b20(param_1 + 0x70,&lStackX_10);
    lVar3 = *(longlong *)(param_1 + 0x80);
    puStackX_8 = *(undefined8 **)(lVar3 + lVar7 * 0x10);
    puVar10 = *(undefined8 **)(param_1 + 0x70);
    puVar4 = *(undefined8 **)(lVar3 + lVar7 * 0x10);
    lVar3 = lVar3 + lVar7 * 0x10;
    while( true ) {
      puVar8 = puVar10;
      if (puVar4 != puVar10) {
        puVar8 = (undefined8 *)**(undefined8 **)(lVar3 + 8);
      }
      if (puStackX_8 == puVar8) goto LAB_180080f06;
      puVar8 = puStackX_8;
      if (puStackX_8[2] == param_2) break;
      puStackX_8 = (undefined8 *)*puStackX_8;
    }
    while( true ) {
      puVar9 = puVar10;
      if (puVar4 != puVar10) {
        puVar9 = (undefined8 *)**(undefined8 **)(lVar3 + 8);
      }
      if ((puVar8 == puVar9) || (param_2 != puVar8[2])) break;
      puVar8 = (undefined8 *)*puVar8;
    }
    if (puStackX_8 == puVar8) {
LAB_180080f06:
      puStackX_8 = puVar10;
      puVar8 = puVar10;
    }
    FUN_180081a20(param_1 + 0x70,&puStackX_8,puStackX_8,puVar8);
    FUN_180081200(param_1,param_2);
    uVar11 = 1;
  }
  iVar6 = _Mtx_unlock(lVar2);
  if (iVar6 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar6);
  }
  iVar6 = _Mtx_unlock(lVar1);
  if (iVar6 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar6);
  }
  return uVar11;
}

