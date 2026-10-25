
void FUN_1800f4080(longlong param_1)

{
  longlong lVar1;
  int iVar2;
  longlong *plVar3;
  longlong lVar4;
  int *piVar5;
  undefined8 uVar6;
  longlong *plVar7;
  undefined4 *puVar8;
  int *piVar9;
  longlong lVar10;
  undefined1 auStack_a8 [32];
  undefined1 auStack_88 [4];
  int iStack_84;
  int aiStack_80 [2];
  undefined8 uStack_78;
  undefined1 auStack_70 [56];
  ulonglong uStack_38;
  
  aiStack_80[0] = *(int *)(param_1 + 8);
  lVar4 = *(longlong *)(param_1 + 0x10);
  uStack_78 = 0xfffffffffffffffe;
  uStack_38 = DAT_1802a5230 ^ (ulonglong)auStack_a8;
  iStack_84 = 0;
  if (*(int *)(lVar4 + 0x44) < aiStack_80[0]) {
    plVar3 = *(longlong **)(lVar4 + 0x48);
    lVar10 = 0;
    if (plVar3 != (longlong *)0x0) {
      lVar10 = *plVar3;
    }
    iStack_84 = *(int *)(lVar4 + 0x44) * 2;
    piVar9 = aiStack_80;
    if (aiStack_80[0] <= iStack_84) {
      piVar9 = &iStack_84;
    }
    piVar5 = &DAT_18021a994;
    if (4 < *piVar9) {
      piVar5 = piVar9;
    }
    iVar2 = *piVar5;
    if (0x3ffffffffffffffd < (ulonglong)(longlong)iVar2) {
      auStack_88[0] = 0;
      uVar6 = FUN_1801935b0(auStack_70,3,&DAT_18021a380,0x4f5);
      iStack_84 = 1;
      uVar6 = FUN_1801937a0(uVar6,&DAT_18021bc00);
      uVar6 = FUN_1801937a0(uVar6,&DAT_18021bc80);
      FUN_1801936d0(auStack_88,uVar6);
      FUN_180193670(auStack_70);
    }
    lVar1 = (longlong)iVar2 * 4 + 8;
    if (lVar10 == 0) {
      plVar7 = (longlong *)__2_YAPEAX_K_Z(lVar1);
    }
    else {
      plVar7 = (longlong *)FUN_18000f3a0(lVar10,lVar1);
    }
    *(longlong **)(lVar4 + 0x48) = plVar7;
    *plVar7 = lVar10;
    *(int *)(lVar4 + 0x44) = iVar2;
    lVar10 = *(longlong *)(lVar4 + 0x48);
    for (puVar8 = (undefined4 *)(lVar10 + 8); puVar8 < (undefined4 *)(lVar1 + lVar10);
        puVar8 = puVar8 + 1) {
      if (puVar8 != (undefined4 *)0x0) {
        *puVar8 = 0;
      }
    }
    if (0 < *(int *)(lVar4 + 0x40)) {
      memcpy(*(longlong *)(lVar4 + 0x48) + 8,plVar3 + 1,(longlong)*(int *)(lVar4 + 0x40) << 2);
    }
    if ((plVar3 != (longlong *)0x0) && (*plVar3 == 0)) {
      ___V_YAXPEAX_Z(plVar3);
    }
  }
  FUN_1801e8810(uStack_38 ^ (ulonglong)auStack_a8);
  return;
}

