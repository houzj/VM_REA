
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined8 FUN_1800e2d70(longlong *param_1)

{
  longlong lVar1;
  char cVar2;
  int iVar3;
  int *piVar4;
  undefined8 uVar5;
  undefined8 *puVar6;
  longlong lVar7;
  int iVar8;
  int *piVar9;
  int aiStackX_8 [2];
  undefined1 *puStackX_10;
  undefined8 in_stack_ffffffffffffff78;
  undefined4 uVar11;
  undefined8 uVar10;
  undefined1 auStack_60 [24];
  undefined8 *puStack_48;
  
  uVar11 = (undefined4)((ulonglong)in_stack_ffffffffffffff78 >> 0x20);
  if (*param_1 == 0) {
    return 0;
  }
  piVar4 = (int *)func_0x0001801e8c60(0x100,0x1802b0a11);
  if (piVar4 == (int *)0x0) {
    cVar2 = FUN_18001dbc0(4);
    if (cVar2 == '\0') {
      return 0xe0000003;
    }
    uVar5 = FUN_18001db50();
    uVar10 = CONCAT44(uVar11,0x23);
LAB_1800e2dd0:
    FUN_180015b40(uVar5,4,&UNK_18021f998,&UNK_18022d6a8,uVar10);
    return 0xe0000003;
  }
  iVar8 = 0;
  aiStackX_8[0] = 0;
  piVar9 = aiStackX_8;
  iVar3 = (**(code **)(*(longlong *)*param_1 + 0x10))
                    ((longlong *)*param_1,&UNK_18022d6f8,piVar4,0x100,piVar9);
  uVar11 = (undefined4)((ulonglong)piVar9 >> 0x20);
  if (iVar3 == -0x1fffffee) {
    ___V_YAXPEAX_Z(piVar4);
    iVar3 = aiStackX_8[0];
    piVar4 = (int *)func_0x0001801e8c60((longlong)aiStackX_8[0],0x1802b0a11);
    if (piVar4 == (int *)0x0) {
      cVar2 = FUN_18001dbc0(4);
      if (cVar2 == '\0') {
        return 0xe0000003;
      }
      uVar5 = FUN_18001db50();
      uVar10 = CONCAT44(uVar11,0x36);
      goto LAB_1800e2dd0;
    }
    iVar3 = (**(code **)(*(longlong *)*param_1 + 0x10))
                      ((longlong *)*param_1,&UNK_18022d6f8,piVar4,iVar3,aiStackX_8);
  }
  if ((iVar3 == 0) && (0 < *piVar4)) {
    puStackX_10 = auStack_60;
    piVar9 = piVar4 + 1;
    do {
      iVar3 = *piVar9;
      puStack_48 = (undefined8 *)0x0;
      puVar6 = (undefined8 *)__2_YAPEAX_K_Z(0x28);
      if (puVar6 == (undefined8 *)0x0) {
LAB_1800e2fa1:
        __Xbad_alloc_std__YAXXZ();
        break;
      }
      *puVar6 = &UNK_18022d740;
      puVar6[1] = FUN_1800e3460;
      *(int *)(puVar6 + 2) = iVar3;
      puVar6[3] = param_1;
      puStack_48 = puVar6;
      if ((_DAT_1802b3230 & 1) == 0) {
        _DAT_1802b3230 = _DAT_1802b3230 | 1;
        FUN_18007fa60();
        FUN_1801e8930(&UNK_180202f50);
      }
      FUN_180080bb0(0x1802b3240,piVar9[1],auStack_60);
      lVar1 = param_1[1];
      lVar7 = FUN_1800e34f0();
      if (param_1[2] == 0x7fffffffffffffe) {
        __Xlength_error_std__YAXPEBD_Z(&UNK_18021d318);
        goto LAB_1800e2fa1;
      }
      param_1[2] = param_1[2] + 1;
      *(longlong *)(lVar1 + 8) = lVar7;
      **(longlong **)(lVar7 + 8) = lVar7;
      iVar8 = iVar8 + 1;
      piVar9 = piVar9 + 2;
    } while (iVar8 < *piVar4);
  }
  ___V_YAXPEAX_Z(piVar4);
  return 0;
}

