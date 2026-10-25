
undefined8 * FUN_1800f74b0(longlong *param_1,undefined8 *param_2,undefined8 *param_3)

{
  longlong *plVar1;
  int *piVar2;
  int iVar3;
  longlong lVar4;
  undefined8 *puVar5;
  undefined8 *puVar6;
  undefined8 uVar7;
  longlong *plVar8;
  longlong lStackX_8;
  undefined8 *puStackX_10;
  longlong lStackX_20;
  undefined8 uStack_38;
  longlong *plStack_30;
  
  puVar6 = (undefined8 *)0x0;
  *param_2 = 0;
  param_2[1] = 0;
  puStackX_10 = param_2;
  lVar4 = FUN_1800202d0(param_1,param_3);
  if ((lVar4 == *param_1) || (iVar3 = FUN_180020a60(param_3,lVar4 + 0x20), iVar3 < 0)) {
    lStackX_20 = *param_1;
    plVar8 = &lStackX_20;
  }
  else {
    lStackX_8 = lVar4;
    plVar8 = &lStackX_8;
  }
  if (*plVar8 == *param_1) {
    puVar5 = (undefined8 *)__2_YAPEAX_K_Z(0x88);
    if (puVar5 != (undefined8 *)0x0) {
      puVar5[6] = 0;
      puVar5[10] = 0xf;
      puVar5[9] = 0;
      *(undefined1 *)(puVar5 + 7) = 0;
      puVar5[0xe] = 0xf;
      puVar5[0xd] = 0;
      *(undefined1 *)(puVar5 + 0xb) = 0;
      *(undefined4 *)(puVar5 + 1) = 0;
      puVar5[2] = 0;
      *(undefined1 *)(puVar5 + 0xf) = 1;
      puVar5[0x10] = 0;
      *puVar5 = &UNK_18022e690;
      puVar6 = puVar5;
    }
    uStack_38 = 0;
    plStack_30 = (longlong *)0x0;
    FUN_1800fa090(&uStack_38,puVar6);
    FUN_18001d870(param_2,&uStack_38);
    plVar8 = plStack_30;
    if (plStack_30 != (longlong *)0x0) {
      LOCK();
      plVar1 = plStack_30 + 1;
      *(int *)plVar1 = (int)*plVar1 + -1;
      UNLOCK();
      if ((int)*plVar1 == 0) {
        (**(code **)*plStack_30)(plStack_30);
        LOCK();
        piVar2 = (int *)((longlong)plVar8 + 0xc);
        *piVar2 = *piVar2 + -1;
        UNLOCK();
        if (*piVar2 == 0) {
          (**(code **)(*plStack_30 + 8))();
        }
      }
    }
    uVar7 = FUN_1800d9920(param_1,param_3);
    FUN_18007dc50(uVar7,param_2);
    if (0xf < (ulonglong)param_3[3]) {
      param_3 = (undefined8 *)*param_3;
    }
    FUN_1800f3e00(*param_2,param_3,&UNK_180225e64,param_1 + 0x23);
  }
  else {
    FUN_18007dc50(param_2,*plVar8 + 0x40);
  }
  return param_2;
}

