
undefined8 FUN_180080a30(longlong param_1,longlong param_2,char param_3)

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
  undefined8 *puStackX_8;
  longlong alStackX_10 [2];
  longlong lStackX_20;
  undefined8 uVar11;
  longlong lVar12;
  undefined1 auStack_48 [16];
  
  uVar11 = 0xfffffffffffffffe;
  lVar1 = param_1 + 0x18;
  alStackX_10[0] = param_2;
  lStackX_20 = lVar1;
  iVar6 = _Mtx_lock(lVar1);
  if (iVar6 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar6);
  }
  lVar2 = param_1 + 0x10;
  lVar12 = lVar2;
  iVar6 = _Mtx_lock(lVar2);
  if (iVar6 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar6);
  }
  if (*(longlong *)(param_1 + 0xb8) != 0) {
    lVar7 = func_0x000180081b20(param_1 + 0xb0,alStackX_10);
    lVar3 = *(longlong *)(param_1 + 0xc0);
    puVar9 = *(undefined8 **)(lVar3 + lVar7 * 0x10);
    puVar4 = *(undefined8 **)(param_1 + 0xb0);
    while( true ) {
      puVar8 = puVar4;
      if (*(undefined8 **)(lVar3 + lVar7 * 0x10) != puVar4) {
        puVar8 = (undefined8 *)**(undefined8 **)(lVar3 + lVar7 * 0x10 + 8);
      }
      puVar10 = puVar4;
      puVar5 = puStackX_8;
      if ((puVar9 == puVar8) || (puVar10 = puVar9, puVar5 = puVar9, param_2 == puVar9[2])) break;
      puVar9 = (undefined8 *)*puVar9;
    }
    puStackX_8 = puVar5;
    if (puVar10 != *(undefined8 **)(param_1 + 0xb0)) {
      FUN_180081200(param_1,param_2);
      FUN_180081620(param_1 + 0xb0,&puStackX_8,puVar10);
      uVar11 = 1;
      goto LAB_180080b68;
    }
  }
  if (param_3 != '\0') {
    FUN_180082190(param_1 + 0x70,auStack_48,alStackX_10,DAT_1802b1206,uVar11,lVar12);
  }
  uVar11 = 0;
LAB_180080b68:
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

