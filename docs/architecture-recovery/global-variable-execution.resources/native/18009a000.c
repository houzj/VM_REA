
undefined8 FUN_18009a000(longlong param_1,undefined4 param_2,char param_3)

{
  longlong *plVar1;
  longlong lVar2;
  code *pcVar3;
  longlong lVar4;
  undefined8 uVar5;
  longlong *plVar6;
  byte bVar7;
  longlong *plStackX_8;
  longlong lStackX_10;
  longlong **pplVar8;
  longlong *aplStack_28 [2];
  
  lStackX_10._0_4_ = param_2;
  FUN_18009ab20(param_1 + 0x18,&plStackX_8,&lStackX_10);
  if (plStackX_8 == *(longlong **)(param_1 + 0x18)) {
    return 0xe0000007;
  }
  FUN_18009ab20(param_1 + 0x18,&plStackX_8,&lStackX_10);
  plVar1 = *(longlong **)(param_1 + 0x18);
  if (plStackX_8 == plVar1) {
    lVar2 = *plVar1;
    plStackX_8 = &lStackX_10;
    pplVar8 = &plStackX_8;
    lVar4 = FUN_18009b300(plVar1,lVar2,*(undefined8 *)(lVar2 + 8));
    if (*(longlong *)(param_1 + 0x20) == 0x3fffffffffffffe) {
      __Xlength_error_std__YAXPEBD_Z(&UNK_18021d318);
      pcVar3 = (code *)swi(3);
      uVar5 = (*pcVar3)();
      return uVar5;
    }
    *(longlong *)(param_1 + 0x20) = *(longlong *)(param_1 + 0x20) + 1;
    *(longlong *)(lVar2 + 8) = lVar4;
    **(longlong **)(lVar4 + 8) = lVar4;
    FUN_18009b010(param_1 + 0x18,aplStack_28,**(longlong **)(param_1 + 0x18) + 0x10,
                  **(longlong **)(param_1 + 0x18),pplVar8);
    plStackX_8 = aplStack_28[0];
  }
  *(char *)(plStackX_8 + 3) = param_3;
  if (param_3 != '\0') {
    *(undefined1 *)(param_1 + 0x10) = 1;
    return 0;
  }
  *(undefined1 *)(param_1 + 0x10) = 0;
  plVar1 = *(longlong **)(param_1 + 0x18);
  plVar6 = (longlong *)*plVar1;
  if (plVar6 != plVar1) {
    bVar7 = 0;
    do {
      bVar7 = bVar7 | *(byte *)(plVar6 + 3);
      *(byte *)(param_1 + 0x10) = bVar7;
      plVar6 = (longlong *)*plVar6;
    } while (plVar6 != plVar1);
  }
  return 0;
}

